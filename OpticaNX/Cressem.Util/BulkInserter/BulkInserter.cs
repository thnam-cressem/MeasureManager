using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cressem.Util.BulkInserter
{
	public class BulkInserter
	{
		private string _dbName; // Database name to insert data

		private string _outputMessage;
		private string _errorMessage;
		private int _exitCode;
		private string _batchFilePath;

		public BulkInserter(string dbName, string userId, string password)
		{
			_dbName = dbName;
			UserId = userId;
			Password = password;
		}

		#region Properties

		public int ExitCode
		{
			get { return _exitCode; }
		}

		/// <summary>
		/// 오류 메시지
		/// </summary>
		public string ErrorMessage
		{
			get
			{
				return _errorMessage;
			}
		}

		/// <summary>
		/// 출력 메시지
		/// </summary>
		public string OutputMessage
		{
			get
			{
				return _outputMessage;
			}
		}

		/// <summary>
		/// 배치 파일 경로
		/// </summary>
		public string BatFilePath
		{
			get
			{
				return _batchFilePath;
			}
		}

		public string UserId { get; set; }

		public string Password { get; set; }

		public string BulkProcessorPath { get; set; }

		#endregion

		/// <summary>
		/// 벌크삽입 실행
		/// </summary>
		/// <param name="fileName">CSV 데이터 파일명</param>
		/// <param name="dbTableName">데이터를 넣을 DB의 테이블명</param>
		/// <param name="isBatchNotToRun">배치 파일 실행 유무</param>
		/// <param name="fields">테이블명 목록, 기본값은 빈 텍스트</param>
		/// <returns>벌크삽입 소요시간 (단위: ms)</returns>
		public int Insert(string fileName, string dbTableName, bool isBatchNotToRun, string fields)
		{
			_exitCode = 0;
			_outputMessage = String.Empty;
			_errorMessage = String.Empty;

			int totalProcTime = 0;
			string batchFileName = String.Format("{0}.bat", dbTableName);
			_batchFilePath = Path.Combine(Directory.GetCurrentDirectory(), batchFileName);
			string psqlFilePath = BulkProcessorPath;// Path.Combine(BulkProcessorPath, "psql.exe");
			string csvFilePath = fileName;

			string tableNameWithFields = String.Empty;
			if (String.IsNullOrWhiteSpace(fields) == false)
			{
				tableNameWithFields = String.Format("{0}({1})", dbTableName, fields);
			}
			StringBuilder sb = new StringBuilder();
			using (StreamWriter sw = new StreamWriter(_batchFilePath, false, Encoding.Default))
			{
				// Build a batch file
				sb.AppendLine("@echo off");
				sb.AppendLine(String.Format("set pgpassword={0}", Password));
				sb.AppendLine(String.Format(""));
				sb.AppendLine(String.Format("\"{0}\" -h localhost -d {1} -U {2} -w -c \"copy {3} from '{4}' using delimiters ',' CSV ENCODING 'UTF8';\"",
														psqlFilePath,
														_dbName,
														UserId,
														//dbTableName,      
														tableNameWithFields,
														csvFilePath.Replace("\\", "/")
													));
				sb.AppendLine("echo %pgpassword%");

				sw.WriteLine(sb.ToString());
				sw.Flush();
				sw.Close();

				// Just create a batch file but don't run it
				if (isBatchNotToRun == true)
				{
					return 0;
				}

				// Ready to process the batch file
				ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", "/C " + batchFileName);
				startInfo.WindowStyle = ProcessWindowStyle.Hidden;
				//startInfo.WorkingDirectory = AppConfig.WORKSPACE;
				//startInfo.FileName = "cmd.exe", "/c " + batchFileName;//batchFileName;
				startInfo.CreateNoWindow = true;
				startInfo.UseShellExecute = false;
				startInfo.RedirectStandardError = true;
				startInfo.RedirectStandardOutput = true;
				startInfo.StandardErrorEncoding = Encoding.UTF8;
				startInfo.StandardOutputEncoding = Encoding.UTF8;

				// Process the batch file
				using (Process process = new Process { StartInfo = startInfo })
				{
					process.EnableRaisingEvents = true;
					process.Start();
					
					// DM에서 Bulk-Insert나 모델정보 Insert시에 1분 이상 걸릴 경우 임의로 해당 처리를 중단 하는 로직 추가. 
					// 특정 상황에서 끝나지 않고 hang걸리는 문제로 이후에 실행 되는 DM이 무한정 기다리는 문제가 발생하여 이와 같이 처리
					if (process.WaitForExit(60 * 1000 * 5) == false)
					{
						try
						{
							if (process.HasExited == false)
								process.Kill();
						}
						finally
						{
							_exitCode = process.ExitCode;
							_outputMessage = process.StandardOutput.ReadToEnd();
							_errorMessage = "Bulk-insert timeout.";// process.StandardError.ReadToEnd();
						}
					}
					else
					{
						_exitCode = process.ExitCode;
						_outputMessage = process.StandardOutput.ReadToEnd();
						_errorMessage = process.StandardError.ReadToEnd();
					}

					TimeSpan totalTime = process.TotalProcessorTime;
					totalProcTime = totalTime.Milliseconds;
				}
			}

			return totalProcTime;
		}
	}
}
