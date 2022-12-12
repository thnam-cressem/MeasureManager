using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Collections;

namespace Cressem.Util.Helper
{
	/// <summary>
	/// This class contains functions handling with local user accounts on a machine; not for domain user accounts.
	/// </summary>	
	[PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
	public abstract class LocalUserHelper
	{
		#region Support downto .NET Framework 2.0

		// see http://msdn.microsoft.com/en-us/library/aa772300(VS.85).aspx
		[Flags]
		enum ADS_USER_FLAG_ENUM
		{
			ADS_UF_SCRIPT = 1,         // 0x1
			ADS_UF_ACCOUNTDISABLE = 2,         // 0x2
			ADS_UF_HOMEDIR_REQUIRED = 8,         // 0x8
			ADS_UF_LOCKOUT = 16,        // 0x10
			ADS_UF_PASSWD_NOTREQD = 32,        // 0x20
			ADS_UF_PASSWD_CANT_CHANGE = 64,        // 0x40
			ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 128,       // 0x80
			ADS_UF_TEMP_DUPLICATE_ACCOUNT = 256,       // 0x100
			ADS_UF_NORMAL_ACCOUNT = 512,       // 0x200
			ADS_UF_INTERDOMAIN_TRUST_ACCOUNT = 2048,      // 0x800
			ADS_UF_WORKSTATION_TRUST_ACCOUNT = 4096,      // 0x1000
			ADS_UF_SERVER_TRUST_ACCOUNT = 8192,      // 0x2000
			ADS_UF_DONT_EXPIRE_PASSWD = 65536,     // 0x10000
			ADS_UF_MNS_LOGON_ACCOUNT = 131072,    // 0x20000
			ADS_UF_SMARTCARD_REQUIRED = 262144,    // 0x40000
			ADS_UF_TRUSTED_FOR_DELEGATION = 524288,    // 0x80000
			ADS_UF_NOT_DELEGATED = 1048576,   // 0x100000
			ADS_UF_USE_DES_KEY_ONLY = 2097152,   // 0x200000
			ADS_UF_DONT_REQUIRE_PREAUTH = 4194304,   // 0x400000
			ADS_UF_PASSWORD_EXPIRED = 8388608,   // 0x800000
			ADS_UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 16777216   // 0x1000000
		};

		/// <summary>
		/// Sets or clears the "Password Never Expires" flag for a user on the current system.
		/// This is based on ideas in http://www.codeproject.com/KB/system/OSUserMangement.aspx,
		/// but not that the code there is wrong: it does not take into account the current userFlags value
		/// The code below does take that into account and is much more maintainable.
		/// 
		/// the logic can be made a bit easier by applying sets
		/// </summary>
		/// <param name="userName">name of the user to set the Password Never Expires flag</param>
		/// <param name="passwordNeverExpires">new flag value</param>
		/// <remarks>http://wiert.me/2009/10/11/c-net-setting-or-clearing-the-password-never-expires-flag-for-a-user/</remarks>
		public static void SetPasswordNeverExpiresNetFx20(string userName, bool passwordNeverExpires)
		{
			const string userNameString = "userName";
			const string userFlagsString = "userFlags";

			string machineName = Environment.MachineName;

			DirectoryEntry userInThisComputerDirectoryEntry = GetUserInThisComputerDirectoryEntry(machineName, userName);
			if (null == userInThisComputerDirectoryEntry)
				throw new ArgumentException("not found in " + machineName, userNameString);

			PropertyValueCollection userFlagsProperties = userInThisComputerDirectoryEntry.Properties[userFlagsString];

			ADS_USER_FLAG_ENUM userFlags = (ADS_USER_FLAG_ENUM)(userFlagsProperties.Value);
			ADS_USER_FLAG_ENUM newUserFlags = userFlags;

			if (passwordNeverExpires)
				newUserFlags = newUserFlags | ADS_USER_FLAG_ENUM.ADS_UF_DONT_EXPIRE_PASSWD;
			else
				newUserFlags = newUserFlags & (~ADS_USER_FLAG_ENUM.ADS_UF_DONT_EXPIRE_PASSWD);

			userFlagsProperties.Value = newUserFlags;

			userInThisComputerDirectoryEntry.CommitChanges();
		}

		public static bool GetPasswordNeverExpiredNetFx20(string userName)
		{
			const string userNameString = "userName";
			const string userFlagsString = "userFlags";

			string machineName = Environment.MachineName;

			DirectoryEntry userInThisComputerDirectoryEntry = GetUserInThisComputerDirectoryEntry(machineName, userName);
			if (null == userInThisComputerDirectoryEntry)
				throw new ArgumentException("not found in " + machineName, userNameString);

			PropertyValueCollection userFlagsProperties = userInThisComputerDirectoryEntry.Properties[userFlagsString];

			ADS_USER_FLAG_ENUM userFlags = (ADS_USER_FLAG_ENUM)(userFlagsProperties.Value);

			if (Convert.ToBoolean(userFlags & ADS_USER_FLAG_ENUM.ADS_UF_DONT_EXPIRE_PASSWD))
				return true;

			return false;
		}

		protected static DirectoryEntry GetUserInThisComputerDirectoryEntry(string machineName, string userName)
		{
			DirectoryEntry computerDirectoryEntry = GetComputerDirectoryEntry(machineName);
			const string userSchemaClassName = "user";

			return computerDirectoryEntry.Children.Find(userName, userSchemaClassName);
		}

		protected static DirectoryEntry GetComputerDirectoryEntry(string machineName)
		{
			//Initiate DirectoryEntry Class To Connect Through WINNT Protocol
			// see: http://msdn.microsoft.com/en-us/library/system.directoryservices.directoryentry.path.aspx
			const string pathUsingWinNTComputerMask = "WinNT://{0},computer";
			string path = string.Format(pathUsingWinNTComputerMask, machineName);
			DirectoryEntry thisComputerDirectoryEntry = new DirectoryEntry(path);

			return thisComputerDirectoryEntry;
		}

		public static void CreateGroupNetFx20(string name, string description)
		{
			DirectoryEntry root = new DirectoryEntry(string.Format("WinNT://{0},computer", Environment.MachineName));
			using (var group = root.Children.Add(name, "group"))
			{
				group.Invoke("Put", new object[] { "description", description });
				group.CommitChanges();
			}
		}

		public static bool ExistsGroupNetFx20(string name)
		{
			DirectoryEntry root = new DirectoryEntry(string.Format("WinNT://{0},computer", Environment.MachineName));
			try
			{
				root.Children.Find(name, "group");
				return true;
			}
			catch (COMException e)
			{
				if (e.ErrorCode == -2147022676)
					return false;
				throw;
			}
		}

		public static void AddUserToGroupNetFx20(string userName, string groupName)
		{
			string groupPath = string.Format("WinNT://{0}/{1},group", Environment.MachineName, groupName);
			string userPath = string.Format("WinNT://{0}/{1},user", Environment.MachineName, userName);
			using (DirectoryEntry root = new DirectoryEntry(groupPath))
			{
				root.Invoke("Add", new object[] { userPath });
				root.CommitChanges();
			}
		}

		public static bool IsUserInGroupNetFx20(string userName, string groupName)
		{
			DirectoryEntry root = new DirectoryEntry(string.Format("WinNT://{0},computer", Environment.MachineName));
			root = root.Children.Find(userName, "user");
			var groups = root.Invoke("groups");
			foreach (var group in (IEnumerable)groups)
			{
				DirectoryEntry groupEntry = new DirectoryEntry(group);
				if (string.Equals(groupEntry.Name, groupName, StringComparison.CurrentCultureIgnoreCase))
					return true;
			}

			return false;
		}

		#endregion

		/// <summary>
		/// Gets a local user account from account management of directory services
		/// </summary>
		/// <param name="userName">User name to get</param>
		/// <returns>UserPrincipal of the user if found, otherwise null</returns>
		public static UserPrincipal GetUser(string userName)
		{
			if (String.IsNullOrEmpty(userName))
				return null;

			using (var ctx = new PrincipalContext(ContextType.Machine))
			{
				try
				{
					return UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, userName);
				}
				catch
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Checks whether a specified user account exists.
		/// </summary>
		/// <param name="userName">Target username</param>
		/// <returns>True if found, otherwise false</returns>
		public static bool ExistsUser(string userName)
		{
			if (GetUser(userName) != null)
				return true;

			return false;
		}

		#region Create / Delete User

		/// <summary>
		/// Create a new user account.
		/// </summary>
		/// <param name="userName">Username</param>
		/// <param name="password">Password</param>
		/// <param name="displayName">Display name</param>
		/// <param name="enabled">Flag of account enabled</param>
		/// <param name="userCannotChangePassword">Flag of user cannot chanage password</param>
		/// <param name="passwordNeverExpires">Flag of password never expires</param>
		/// <returns>True if created, otherwise false</returns>
		/// <exception cref="System.DirectoryServices.AccountManagement.PasswordException">
		/// Throws an exception if the caller does not have appropriate rights, the new password does not meet password complexity requirements, 
		/// or for any other reason that the underlying stores reject the password change.
		/// </exception>
		public static bool CreateUser(string userName, string password, string displayName, bool enabled, bool userCannotChangePassword, bool passwordNeverExpires)
		{
			if (ExistsUser(userName))
				throw new Exception(String.Format("{0} already exists.", userName));

			using (var ctx = new PrincipalContext(ContextType.Machine))
			using (var newUser = new UserPrincipal(ctx))
			{
				newUser.SamAccountName = userName;
				if (String.IsNullOrWhiteSpace(password) == false)
				{
					newUser.SetPassword(password);
				}
				if (String.IsNullOrWhiteSpace(displayName) == false)
				{
					newUser.DisplayName = displayName;
				}
				newUser.Enabled = enabled;
				newUser.PasswordNeverExpires = passwordNeverExpires;
				newUser.UserCannotChangePassword = userCannotChangePassword;
				newUser.Save();
			}

			return ExistsUser(userName);
		}

		/// <summary>
		/// Deletes a specified user account.
		/// </summary>
		/// <param name="userName">Target username</param>
		/// <returns>True if deleted, otherwise false</returns>
		public static bool DeleteUser(string userName)
		{
			var user = GetUser(userName);

			if (user != null)
				user.Delete();

			return !ExistsUser(userName);
		}

		#endregion

		#region Enabled

		/// <summary>
		/// Gets a Nullable Boolean value that specifies whether this account is enabled for authentication
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public static bool? IsEnabled(string userName)
		{
			var user = GetUser(userName);

			if (user == null)
				throw new ArgumentException(String.Format("{0} doesn't exist.", userName));

			return user.Enabled;
		}

		/// <summary>
		/// Sets a Nullable Boolean value that specifies whether this account is enabled for authentication
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="enabled"></param>
		public static void SetEnabled(string userName, bool? enabled)
		{
			var user = GetUser(userName);

			if (user == null)
				throw new ArgumentException(String.Format("{0} doesn't exist.", userName));

			user.Enabled = enabled;
			user.Save();
		}

		#endregion

		#region Lock Out

		/// <summary>
		/// Specifies whether the account is currently locked out
		/// </summary>
		/// <param name="userName">Username to check</param>
		/// <returns>true if the account is locked out, otherwise false</returns>
		public static bool IsAccountLockedOut(string userName)
		{
			var user = GetUser(userName);

			if (user == null)
				throw new ArgumentException(String.Format("{0} doesn't exist.", userName));

			return user.IsAccountLockedOut();
		}

		/// <summary>
		/// Unlocks the account if it is currently locked out.
		/// </summary>
		/// <param name="userName">Username to unlock</param>
		public static void UnlockAccount(string userName)
		{
			var user = GetUser(userName);

			if (user == null)
				throw new ArgumentException(String.Format("{0} doesn't exist.", userName));

			if (user.IsAccountLockedOut())
				user.UnlockAccount();
		}

		#endregion

		#region Password Related

		/// <summary>
		/// Sets the account password to the specified value. 
		/// </summary>
		/// <param name="userName">Username</param>
		/// <param name="newPassword">New password</param>
		public static void SetPassword(string userName, string newPassword)
		{
			var user = GetUser(userName);

			if (user == null)
				throw new ArgumentException(String.Format("{0} doesn't exist.", userName));

			user.SetPassword(newPassword);
			user.Save();
		}

		/// <summary>
		/// Refreshes an expired password.
		/// </summary>
		/// <param name="userName">Username</param>
		public static void RefreshExpiredPassword(string userName)
		{
			var user = GetUser(userName);

			if (user == null)
				throw new ArgumentException(String.Format("{0} doesn't exist.", userName));

			user.RefreshExpiredPassword();
		}

		#region PasswordNeverExpires

		/// <summary>
		/// Gets a Boolean value that specifies whether the password expires for this account.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns>true if the password never expires, otherwise false</returns>
		public static bool GetPasswordNeverExpires(string userName)
		{
			var user = GetUser(userName);

			if (user == null)
				throw new Exception(String.Format("{0} doesn't exist.", userName));

			return user.PasswordNeverExpires;
		}

		/// <summary>
		/// Sets a Boolean value that specifies whether the password expires for this account.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="neverExpires"></param>
		public static void SetPasswordNeverExpires(string userName, bool neverExpires)
		{
			var user = GetUser(userName);

			if (user == null)
				throw new Exception(String.Format("{0} doesn't exist.", userName));

			user.PasswordNeverExpires = neverExpires;
			user.Save();
		}

		#endregion

		#region UserCannotChangePassword		

		/// <summary>
		/// Gets a Boolean value that specifies whether the user can change the password for this account.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public static bool GetUserCannotChangePassword(string userName)
		{
			var user = GetUser(userName);

			if (user == null)
				throw new Exception(String.Format("{0} doesn't exist.", userName));

			return user.UserCannotChangePassword;
		}

		/// <summary>
		/// Sets a Boolean value that specifies whether the user can change the password for this account.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="cannotChange"></param>
		public static void SetUserCannotChangePassword(string userName, bool cannotChange)
		{
			var user = GetUser(userName);

			if (user == null)
				throw new Exception(String.Format("{0} doesn't exist.", userName));

			user.UserCannotChangePassword = cannotChange;
			user.Save();
		}

		#endregion

		#region PasswordNotRequired

		/// <summary>
		/// Gets a Boolean value that specifies whether a password is required for this account. 
		/// </summary>
		/// <param name="userName">Username</param>
		/// <returns>true if a password is not required, otherwise false</returns>
		public static bool GetPasswordNotRequired(string userName)
		{
			var user = GetUser(userName);

			if (user == null)
				throw new Exception(String.Format("{0} doesn't exist.", userName));

			return user.PasswordNotRequired;
		}

		/// <summary>
		/// Sets a Boolean value that specifies whether a password is required for this account. 
		/// </summary>
		/// <param name="userName">Username</param>
		/// <param name="notRequired"></param>
		public static void SetPasswordNotRequired(string userName, bool notRequired)
		{
			var user = GetUser(userName);

			if (user == null)
				throw new Exception(String.Format("{0} doesn't exist.", userName));

			user.PasswordNotRequired = notRequired;
			user.Save();
		}

		#endregion

		#endregion

		#region Service Account Default

		/// <summary>
		/// Checks whether the specified user has default settings for service account.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public static bool IsSvcAccountDefault(string userName)
		{
			var user = GetUser(userName);

			if (user == null)
				throw new ArgumentException(String.Format("{0} doesn't exist.", userName));

			if (user.Enabled == false)
				return false;
			else if (user.IsAccountLockedOut() == true)
				return false;
			else if (user.PasswordNeverExpires == false)
				return false;
			else if (user.UserCannotChangePassword == false)
				return false;

			return true;
		}

		/// <summary>
		/// Sets default values for a service account.
		/// </summary>
		/// <param name="userName"></param>
		public static void SetSvcAccountDefault(string userName)
		{
			var user = GetUser(userName);

			if (user == null)
				throw new ArgumentException(String.Format("{0} doesn't exist.", userName));

			int totalChanges = 0;

			if (user.Enabled == false)
			{
				user.Enabled = true;
				totalChanges++;
			}

			if (user.IsAccountLockedOut() == true)
			{
				user.UnlockAccount();
				totalChanges++;
			}

			if (user.PasswordNeverExpires == false)
			{
				user.PasswordNeverExpires = true;
				totalChanges++;
			}

			if (user.UserCannotChangePassword == false)
			{
				user.UserCannotChangePassword = true;
				totalChanges++;
			}

			if (totalChanges > 0)
				user.Save();
		}

		#endregion

		#region Group Account

		/// <summary>
		/// Gets a local group account from account management of dirtectory services.
		/// </summary>
		/// <param name="groupName">Group name to get</param>
		/// <returns>GroupPrincipal of a group if found, otherwise null</returns>
		public static GroupPrincipal GetGroup(string groupName)
		{
			if (String.IsNullOrEmpty(groupName))
				return null;

			using (var ctx = new PrincipalContext(ContextType.Machine))
			{
				try
				{
					return GroupPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, groupName);
				}
				catch
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Checks whether a specified group account exists.
		/// </summary>
		/// <param name="groupName">Group name</param>
		/// <returns>True if found, otherwise false</returns>
		public static bool ExistsGroup(string groupName)
		{
			if (GetGroup(groupName) != null)
				return true;

			return false;
		}

		/// <summary>
		/// Creates a new group account.
		/// </summary>
		/// <param name="groupName">Group name</param>
		/// <param name="description">Description</param>
		/// <returns>True if created, otherwise false</returns>
		public static bool CreateGroup(string groupName, string description)
		{
			if (ExistsGroup(groupName))
				throw new Exception(String.Format("{0} already exists.", groupName));

			using (var ctx = new PrincipalContext(ContextType.Machine))
			using (var group = new GroupPrincipal(ctx))
			{
				group.SamAccountName = groupName;
				group.Description = description;
				group.Save();
			}

			return ExistsGroup(groupName);
		}

		/// <summary>
		/// Deletes a specified group account.
		/// </summary>
		/// <param name="groupName">Group name</param>
		/// <returns>True if deleted, otherwise false</returns>
		public static bool DeleteGroup(string groupName)
		{
			var group = GetGroup(groupName);

			if (group == null)
				throw new Exception(String.Format("{0} doesn't exist.", groupName));

			group.Delete();

			return !ExistsGroup(groupName);
		}

		/// <summary>
		/// Adds a user account to a group account.
		/// </summary>
		/// <param name="userName">User name</param>
		/// <param name="groupName">Group name</param>
		/// <returns>Tre if added, otherwise false</returns>
		public static bool AddUserToGroup(string userName, string groupName)
		{
			using (var group = GetGroup(groupName))
			{
				if (group == null)
					return false;

				var user = GetUser(userName);
				if (user == null)
					return false;

				group.Members.Add(user);
				group.Save();
			}

			return true;
		}

		/// <summary>
		/// Removes a user account from a group account.
		/// </summary>
		/// <param name="userName">User name</param>
		/// <param name="groupName">Group name</param>
		/// <returns>True if removed, otherwise false</returns>
		public static bool RemoveUserFromGroup(string userName, string groupName)
		{
			using (var group = GetGroup(groupName))
			{
				if (group == null)
					return false;

				var user = GetUser(userName);
				if (user == null)
					return false;

				group.Members.Remove(user);
				group.Save();
			}

			return true;
		}

		/// <summary>
		/// Checks whether a user is in a group.
		/// </summary>
		/// <param name="userName">User name</param>
		/// <param name="groupName">Group name</param>
		/// <returns>True if in the group, otherwise false</returns>
		public static bool IsUserInGroup(string userName, string groupName)
		{
			using (var group = GetGroup(groupName))
			{
				if (group == null)
					return false;

				var user = GetUser(userName);
				if (user == null)
					return false;

				return group.Members.Contains(user);
			}
		}

		#endregion
	}
}
