using System;
using System.Diagnostics;
using System.IO;

namespace Cressem.Util.Helper
{
	/// <summary>
	/// 
	/// </summary>
	public static class CommandLineHelper
	{
		/// <summary>
		/// Runs an executable file with argument and get its output
		/// </summary>
		/// <param name="executableFileName">file to execute</param>
		/// <param name="arguments">arguments for the file</param>
		/// <param name="output">standard output from the file excution</param>
		/// <returns>true if no error, otherwise false</returns>
		public static bool Run(string executableFileName, string arguments, out string output)
		{
			int? exitCode; // dummy variable

			return Run(executableFileName, arguments, out output, out exitCode);
		}

		/// <summary>
		/// Runs an executable file with argument and get its output
		/// </summary>
		/// <param name="executableFileName">file to execute</param>
		/// <param name="arguments">arguments for the file</param>
		/// <param name="output">standard output from the file excution</param>
		/// <param name="exitCode">The value that the associated process specified when it terminated</param>
		/// <returns>true if no error, otherwise false</returns>
		public static bool Run(string executableFileName, string arguments, out string output, out int? exitCode)
		{
			output = null;
			exitCode = null;

			if (String.IsNullOrWhiteSpace(executableFileName))
				return false;

			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.CreateNoWindow = true;
			startInfo.ErrorDialog = false;
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardInput = true;
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;
			startInfo.FileName = executableFileName;

			if (String.IsNullOrEmpty(arguments) == false)
				startInfo.Arguments = arguments;

			try
			{
				using (Process process = new Process { StartInfo = startInfo })
				{
					process.Start();
					StreamReader outputReader = process.StandardOutput;
					StreamReader errorReader = process.StandardError;
					output = outputReader.ReadToEnd();
					output += errorReader.ReadToEnd();

					// Remove unnecessary new line characters
					if (String.IsNullOrEmpty(output) == false)
						output.Trim(Environment.NewLine.ToCharArray());

					process.WaitForExit();
					exitCode = process.ExitCode;
				}
			}
			catch (Exception ex)
			{
				output = ex.ToString();
				return false;
			}

			return true;
		}

		/// <summary>
		/// Runs DOS command with arguments and write its log in a file
		/// </summary>
		/// <param name="command">dos command</param>
		/// <param name="arguments">arguments for the command</param>
		/// <param name="output">standard output from the command excution</param>
		/// <param name="loggerFileName">a log file to contain the output (null = no logger)</param>
		/// <returns>true if no error, otherwise false</returns>
		public static bool RunCmd(string command, string arguments, out string output, string loggerFileName = null)
		{
			output = null;

			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.CreateNoWindow = true;
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardOutput = true;
			startInfo.FileName = "cmd.exe";

			if (String.IsNullOrEmpty(arguments))
				startInfo.Arguments = "/C" + command;
			else
				startInfo.Arguments = String.Format("/C{0} {1}", command, arguments);

			try
			{
				using (Process process = new Process { StartInfo = startInfo })
				{
					process.Start();
					StreamReader outputReader = process.StandardOutput;

					// logger
					if (String.IsNullOrEmpty(loggerFileName) == false)
					{
						StreamWriter logger = new StreamWriter(loggerFileName);
						// Dump the output stream while the app runs
						do
						{
							using (outputReader)
							{
								char[] buffer = null;
								while (outputReader.Peek() >= 0)
								{
									buffer = new char[4096];
									outputReader.Read(buffer, 0, buffer.Length);
									// Write to the logger
									logger.Write(buffer, 0, buffer.Length);
								}
							}
						}
						while (process.HasExited == false);
					}

					process.WaitForExit();
					output = outputReader.ReadToEnd();

					// Remove unnecessary new line characters
					if (String.IsNullOrEmpty(output) == false)
						output.Trim(Environment.NewLine.ToCharArray());
				}
			}
			catch (Exception ex)
			{
				output = ex.ToString();
				return false;
			}

			return true;
		}
	}
}
