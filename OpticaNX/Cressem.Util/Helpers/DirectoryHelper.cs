using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;

namespace Cressem.Util.Helper
{
	/// <summary>
	/// 
	/// </summary>
	public static class DirectoryHelper
	{
		#region Fields

		private static char[] INVALID_DIR_NAME_CHARS = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns invalid characters for directory name on NTFS.
		/// </summary>
		public static char[] InvalidDirectoryNameCharacters
		{
			get { return INVALID_DIR_NAME_CHARS; }
		}

		/// <summary>
		/// Checks whether the specified path exists.
		/// </summary>
		/// <param name="path">Path to check existance</param>
		/// <returns></returns>
		public static bool Exists(string path)
		{
			if (String.IsNullOrEmpty(path) == true)
				return false;

			bool exist = false;

			try
			{
				exist = Directory.Exists(path);
			}
			catch
			{
				exist = false;
			}

			return exist;
		}

		/// <summary>
		/// Copy folder to another folder recursively
		/// </summary>
		/// <param name="sourceFolder"></param>
		/// <param name="destFolder"></param>
		/// <param name="overwrite"></param>
		public static void Copy(string sourceFolder, string destFolder, bool overwrite = false)
		{
			CreateIfNotExist(destFolder);

			IEnumerable<string> files = Directory.EnumerateFiles(sourceFolder);
			IEnumerable<string> folders = Directory.EnumerateDirectories(sourceFolder);

			foreach (string file in files)
			{
				string name = PathEx.GetFileName(file);
				string dest = PathEx.Combine(destFolder, name);
				File.Copy(file, dest, overwrite);
			}

			foreach (string folder in folders)
			{
				string name = PathEx.GetFileName(folder);
				string dest = PathEx.Combine(destFolder, name);
				Copy(folder, dest, overwrite);
			}
		}

		/// <summary>
		/// Move folder to another folder recursively
		/// </summary>
		/// <param name="sourceFolder"></param>
		/// <param name="destFolder"></param>
		/// <param name="overwrite"></param>
		public static void Move(string sourceFolder, string destFolder, bool overwrite = false)
		{
			Copy(sourceFolder, destFolder, overwrite);
			Delete(sourceFolder);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <remarks>
		/// Directory.Delete(path, true) 
		/// Beware of this approach if your directory your deleting has shortcuts/symbolic links to other folders - you may end up deleting more then you expected
		/// </remarks>
		public static void Delete(string path)
		{
			if (Directory.Exists(path) == false)
				return;
			IEnumerable<string> files = Directory.EnumerateFiles(path);
			IEnumerable<string> dirs = Directory.EnumerateDirectories(path);

			foreach (string file in files)
			{
				File.SetAttributes(file, FileAttributes.Normal);
				File.Delete(file);
			}

			foreach (string dir in dirs)
			{
				Delete(dir);
			}

			Directory.Delete(path, false);
		}

		/// <summary>
		/// Creates a directory if it doesn't exist.
		/// </summary>
		/// <param name="path">Full path of a directory to create</param>
		/// <returns><c>true</c> if created, otherwise <c>false</c></returns>
		public static DirectoryInfo CreateIfNotExist(string path)
		{
			if (Exists(path) == false)
			{
				return Directory.CreateDirectory(path);
			}

			return null;
		}

		/// <summary>
		/// Creates a directory.
		/// </summary>
		/// <param name="path">Directory path to create</param>
		/// <returns><c>true</c> if created, otherwise <c>false</c></returns>
		public static bool Create(string path)
		{
			try
			{
				DirectoryInfo dir = CreateIfNotExist(path);
				if (dir == null)
					return false;
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Deletes a directory if it exists.
		/// </summary>
		/// <param name="path">Full path of a directory</param>
		public static void DeleteIfExist(string path)
		{
			if (Exists(path))
				Directory.Delete(path);
		}

		#region ACL

		/// <summary>
		/// 주어진 디렉토리에 특정 사용자 계정의 접근권한을 부여한다.
		/// </summary>
		/// <param name="path">디렉토리 경로</param>
		/// <param name="userName">계정명</param>		
		/// <param name="rights">접근권한</param>
		/// <returns>성공하면 <c>true</c>, 그렇지 않으면 <c>false</c></returns>
		/// <remarks>
		/// http://stackoverflow.com/questions/8944765/c-sharp-set-directory-permissions-for-all-users-in-windows-7
		/// </remarks>
		private static bool SetAcl(string path, string userName, FileSystemRights rights)
		{
			bool result = false;

			// Add access rule to the actual directory itself
			var accessRule = new FileSystemAccessRule(userName, rights, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow);
			var dir = new DirectoryInfo(path);
			var security = dir.GetAccessControl(AccessControlSections.Access);

			security.ModifyAccessRule(AccessControlModification.Set, accessRule, out result);
			if (result == false)
				return false;

			// Always allow objects to inherity on a directory
			InheritanceFlags iflags = InheritanceFlags.ObjectInherit;
			iflags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;

			// Add access rule for the inheritance
			accessRule = new FileSystemAccessRule(userName, rights, iflags, PropagationFlags.InheritOnly, AccessControlType.Allow);

			result = false;
			security.ModifyAccessRule(AccessControlModification.Add, accessRule, out result);
			if (result == false)
				return false;

			dir.SetAccessControl(security);

			return true;
		}

		/// <summary>
		/// 주어진 디렉토리에 특정 사용자 계정의 접근권한을 부여한다.
		/// </summary>
		/// <param name="path">디렉토리 경로</param>
		/// <param name="userName">계정명</param>		
		/// <param name="rights">접근권한</param>
		/// <returns>권한을 가지고 있으면 <c>true</c>, 그렇지 않으면 <c>false</c></returns>
		/// <remarks>
		/// http://forums.asp.net/t/1390009.aspx?how+can+read+file+and+folder+user+permission+in+C+
		/// </remarks>
		private static bool HasAcl(string path, string userName, FileSystemRights rights)
		{
			bool hasRight = false;

			try
			{
				var security = Directory.GetAccessControl(path);
				AuthorizationRuleCollection authRules = security.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));

				foreach (FileSystemAccessRule accessRule in authRules)
				{
					// For debugging
					//System.Diagnostics.Debug.WriteLine("{0} : {1}", accessRule.IdentityReference.Value, accessRule.FileSystemRights);

					if (accessRule.IdentityReference.Value == userName)
					{
						if (accessRule.FileSystemRights == rights)
						{
							hasRight = true;
							break;
						}
					}
				}
			}
			catch
			{
				hasRight = false;
			}

			return hasRight;
		}

		/// <summary>
		/// 접근권한 정보를 반환한다.
		/// </summary>
		/// <param name="path">디렉토리 경로</param>
		/// <returns>권한 정보</returns>
		private static string GetAcl(string path)
		{
			StringBuilder acl = new StringBuilder();

			var security = Directory.GetAccessControl(path);
			AuthorizationRuleCollection authRules = security.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));

			foreach (FileSystemAccessRule accessRule in authRules)
			{
				acl.AppendLine(String.Format("{0} : {1}", accessRule.IdentityReference.Value, accessRule.FileSystemRights));
			}

			return (acl.Length != 0) ? acl.ToString() : null;
		}

		/// <summary>
		/// Makes a directory full access for a specified user/group account
		/// </summary>
		/// <param name="path">Full path of a directory</param>
		/// <param name="accountName">User/group name</param>
		/// <returns>True if modified, otherwise false</returns>
		public static bool MakeFullAccess(string path, string accountName)
		{
			// Validate existance of the directory
			if (Exists(path) == false)
				return false;

			// Validate existance of the user name in user or group accounts
			if (LocalUserHelper.ExistsGroup(accountName) == false && LocalUserHelper.ExistsUser(accountName) == false)
				return false;

			var rights = (FileSystemRights)0;
			rights = FileSystemRights.FullControl;

			// add access rule to the actual dirtectory itself
			var accessRule = new FileSystemAccessRule(accountName, rights, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow);
			var directory = new DirectoryInfo(path);
			var directorySecurity = directory.GetAccessControl(AccessControlSections.Access);

			bool modified = false;
			directorySecurity.ModifyAccessRule(AccessControlModification.Set, accessRule, out modified);

			if (modified == false)
				return false;

			// Alwways allow objects to inherit on a directory
			var flags = InheritanceFlags.ObjectInherit;
			flags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;

			// Add access rule for the inheritance
			accessRule = new FileSystemAccessRule(accountName, rights, flags, PropagationFlags.None, AccessControlType.Allow);

			modified = false;
			directorySecurity.ModifyAccessRule(AccessControlModification.Add, accessRule, out modified);

			if (modified == false)
				return false;

			directory.SetAccessControl(directorySecurity);

			return true;
		}

		#endregion

		/// <summary>
		/// D:\a1\a2\a3\a4... 에서 a1을 반환한다.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetRootSubDirectoryName(string path)
		{
			DirectoryInfo info = new DirectoryInfo(path);
			while (info.Parent != null && info.Parent.Parent != null)
				info = info.Parent;
			string result = info.Name;

			return result;
		}

		/// <summary>
		/// path가 D:\a1\a2\a3\a4... a1이 공유폴더이다. \\ipAddress\a1\a2\a3\a4을 반환한다.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetRemotePath(string path, string ipAddress)
		{
			if (ipAddress == "127.0.0.1")
				return path;

			string sharedDirectoryName = GetRootSubDirectoryName(path);

			string newPath = path.Replace(Path.GetPathRoot(path), "");

			string result = Path.Combine(@"\\" + ipAddress + @"\" + newPath);
			
			return result;
		}

		#endregion // Public Methods
	}
}
