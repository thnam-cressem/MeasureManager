using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;

namespace Cressem.Util.Helper
{
	/// <summary>
	/// 
	/// </summary>
	public static class NetHelper
	{
		/// <summary>
		/// Determine whether a string is a valid IP address
		/// </summary>
		/// <param name="ipString">IP string to check</param>
		/// <returns>True if valid, otherwise false</returns>
		public static bool IsIPAddress(string ipString)
		{
			IPAddress ipAddress;

			return IPAddress.TryParse(ipString, out ipAddress);
		}

		/// <summary>
		/// Tests ping synchronously
		/// </summary>
		/// <param name="ip">Address to ping</param>
		/// <param name="timeout">Timeout in milli-second</param>
		/// <returns></returns>
		/// <see cref="http://msdn.microsoft.com/en-us/library/system.net.networkinformation.ping.aspx"/>
		public static bool Ping(string ip, int timeout = 1000)
		{
			Ping pingSender = new Ping();
			PingOptions options = new PingOptions();

			// Use the default Ttl value which is 128, but change the fragmentation behavior.
			options.DontFragment = true;

			// Create a buffer of 32 bytes of data to be transmitted.
			string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
			byte[] buffer = Encoding.ASCII.GetBytes(data);

			try
			{
				PingReply reply = pingSender.Send(ip, timeout, buffer, options);

				if (reply.Status == IPStatus.Success)
				{
					//Console.WriteLine("Address: {0}", reply.Address.ToString());
					//Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
					//Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
					//Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
					//Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);

					return true;
				}
			}
			catch
			{
				// Silent catching
			}

			return false;
		}

		/// <summary>
		/// Determine whether IP address is local iP or not
		/// </summary>
		/// <param name="host">IP address to check</param>
		/// <returns>True if local IP, otherwise false</returns>
		public static bool IsLocalIpAddress(string host)
		{
			try
			{
				// Get host IP addresses
				IPAddress[] hostIPs = Dns.GetHostAddresses(host);
				// Get local IP addresses
				IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

				// Test if any host IP equals to any local IP or to localhost
				foreach (IPAddress hostIP in hostIPs)
				{
					// Is localhost
					if (IPAddress.IsLoopback(hostIP))
						return true;

					// Is local address
					foreach (IPAddress localIP in localIPs)
					{
						if (hostIP.Equals(localIP))
							return true;
					}
				}
			}
			catch
			{
				// Silent catching
			}

			return false;
		}
	}
}
