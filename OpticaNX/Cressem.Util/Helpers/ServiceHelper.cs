using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;

namespace Cressem.Util.Helper
{
	/// <summary>
	/// Windows service helper class.
	/// </summary>
	public class ServiceHelper
	{
		/// <summary>
		/// Indicates whether a specific service is registered.
		/// </summary>
		/// <param name="serviceName"></param>
		/// <returns></returns>
		public static bool IsRegistered(string serviceName)
		{
			if (String.IsNullOrWhiteSpace(serviceName))
				return false;

			// try to find a service name
			//ServiceController[] services = ServiceController.GetServices();
			//foreach (ServiceController service in services)
			//{
			//   if (service.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
			//   {
			//      result = true;
			//      break;
			//   }
			//}

			//return result;

			return ServiceController.GetServices().Any(x => x.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Indicates whether a specific service is in service.
		/// </summary>
		/// <param name="serviceName">Service name to search</param>
		/// <returns>True if the service is in service, otherwise false</returns>
		public static bool IsInService(string serviceName)
		{
			bool result = false;

			if (String.IsNullOrWhiteSpace(serviceName))
				return result;

			// try to find a service name
			foreach (var s in ServiceController.GetServices())
			{
				if (s.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
				{
					s.Refresh();
					if (s.Status == ServiceControllerStatus.Running)
					{
						result = true;
						break;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Get status of a specific service
		/// </summary>
		/// <param name="serviceName"></param>
		/// <returns></returns>
		public static ServiceControllerStatus GetServiceStatus(string serviceName)
		{
			if (IsRegistered(serviceName) == false)
				throw new Exception(String.Format("{0} is not registered.", serviceName));

			ServiceController service = new ServiceController(serviceName);

			return service.Status;
		}

		/// <summary>
		/// Starts a specific service.
		/// </summary>
		/// <param name="serviceName">Service name</param>
		/// <param name="timeout">Timeout in second</param>
		/// <returns>True if the service is started, otherwise false</returns>
		public static bool StartService(string serviceName, int timeout)
		{
			return OperateService(serviceName, ServiceControllerStatus.Running, timeout);
		}

		/// <summary>
		/// Stops a specific service.
		/// </summary>
		/// <param name="serviceName">Service name</param>
		/// <param name="timeout">Timeout in second</param>
		/// <returns>True if the service is stopped, otherwise false</returns>
		public static bool StopService(string serviceName, int timeout)
		{
			return OperateService(serviceName, ServiceControllerStatus.Stopped, timeout);
		}

		/// <summary>
		/// Restarts a specific service.
		/// </summary>
		/// <param name="serviceName">Service name</param>
		/// <param name="timeout">Timeout in second</param>
		/// <returns>True if the service is restarted, otherwise false</returns>
		public static bool RestartService(string serviceName, int timeout)
		{
			bool success = false;

			DateTime startTime = DateTime.Now;

			// Stop the service first
			success = OperateService(serviceName, ServiceControllerStatus.Stopped, timeout);

			TimeSpan timeElapsed = DateTime.Now - startTime;
			int newTimeout = timeout - timeElapsed.Seconds;

			if (success == false)
				return success;

			// Start the serivce
			return OperateService(serviceName, ServiceControllerStatus.Running, newTimeout);
		}

		/// <summary>
		/// Operates a specific service with timeout
		/// </summary>
		/// <param name="serviceName">Service name</param>
		/// <param name="targetStatus">Target service status</param>
		/// <param name="timeout">Timeout in second</param>
		/// <returns>True if the operation is successfull, otherwise false</returns>
		private static bool OperateService(string serviceName, ServiceControllerStatus targetStatus, int timeout)
		{
			bool success = true;

			if (IsRegistered(serviceName) == false)
				return false;

			ServiceController service = new ServiceController();
			service.ServiceName = serviceName;

			ServiceControllerStatus currentStatus = service.Status; // Get status of the service

			if (currentStatus != targetStatus)
			{
				switch (targetStatus)
				{
					case ServiceControllerStatus.Stopped:
						service.Stop();
						break;
					case ServiceControllerStatus.Running:
						service.Start();
						break;
				}

				TimeSpan timeElapsed = TimeSpan.Zero;
				DateTime startTime = DateTime.Now;

				while (currentStatus != targetStatus)
				{
					service.Refresh();
					currentStatus = service.Status;

					// For safe operation, escape loop if elapsed time is more than the timeout in sec
					timeElapsed = DateTime.Now - startTime;
					if (timeElapsed.Seconds >= timeout)
					{
						success = false;
						break;
					}
				}
			}


			return success;
		}
	}
}
