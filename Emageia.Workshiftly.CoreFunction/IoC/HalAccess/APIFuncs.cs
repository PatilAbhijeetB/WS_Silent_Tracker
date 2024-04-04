using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Emageia.Workshiftly.CoreFunction.IoC.HalAccess
{

	/// <summary>
	/// Summary description for APIFuncs.
	/// </summary>
	public class APIFuncs
	{
		#region Windows API Functions Declarations
		//This Function is used to get Active Window Title...
		[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		public static extern int GetWindowText(IntPtr hwnd, string lpString, int cch);

		//This Function is used to get Handle for Active Window...
		[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		private static extern IntPtr GetForegroundWindow();

		//This Function is used to get Active process ID...
		[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out Int32 lpdwProcessId);
		#endregion

		#region User-defined Functions
		public static Int32 GetWindowProcessID(IntPtr hwnd)
		{
			//This Function is used to get Active process ID...
			Int32 pid;
			GetWindowThreadProcessId(hwnd, out pid);
			return pid;
		}
		public static IntPtr getforegroundWindow()
		{
			//This method is used to get Handle for Active Window using GetForegroundWindow() method present in user32.dll
			return GetForegroundWindow();
		}
		public static string ActiveApplTitle()
		{
			//This method is used to get active application's title using GetWindowText() method present in user32.dll
			IntPtr hwnd = getforegroundWindow();
			if (hwnd.Equals(IntPtr.Zero)) return "";
			string lpText = new string((char)0, 100);
			int intLength = GetWindowText(hwnd, lpText, lpText.Length);
			if ((intLength <= 0) || (intLength > lpText.Length)) return "unknown";
			return lpText.Trim();
		}
		#endregion


		#region "WinAPI"

		//[DllImport("user32.dll")]
		//private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);


		//[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		//static extern bool GetComputerNameEx(COMPUTER_NAME_FORMAT NameType, System.Text.StringBuilder lpBuffer, ref uint lpnSize);

		#endregion

		// get currnet aa
		public static string getCurrentAppName()
		{
			IntPtr activeAppHandle = GetForegroundWindow();

			IntPtr activeAppProcessId;
			GetWindowThreadProcessId(activeAppHandle, out activeAppProcessId);

			Process currentAppProcess = Process.GetProcessById((int)activeAppProcessId);
			string currentAppName = FileVersionInfo.GetVersionInfo(currentAppProcess.MainModule.FileName).FileDescription;

			return currentAppName;
		}
		public APIFuncs()
		{
		}


		#region Get IP/Mac Address
		/// <summary>
		/// Finds the MAC address of the NIC with maximum speed.New version: returns the NIC with the fastest speed that also has a valid MAC address.
		/// </summary>
		/// <returns>The MAC address.</returns>
		public static string GetMacAddress()
		{
			const int MIN_MAC_ADDR_LENGTH = 12;
			string macAddress = string.Empty;
			long maxSpeed = -1;

			foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
			{
				//log.Debug(
				//	"Found MAC Address: " + nic.GetPhysicalAddress() +
				//	" Type: " + nic.NetworkInterfaceType);

				string tempMac = nic.GetPhysicalAddress().ToString();
				if (nic.Speed > maxSpeed &&
					!string.IsNullOrEmpty(tempMac) &&
					tempMac.Length >= MIN_MAC_ADDR_LENGTH)
				{
					//log.Debug("New Max Speed = " + nic.Speed + ", MAC: " + tempMac);
					maxSpeed = nic.Speed;
					macAddress = tempMac;
				}
			}

			return macAddress;
		}


		/// <summary>
		/// Finds the MAC address of the first operation NIC found.  Original Version: just returns the first one.
		/// </summary>
		/// <returns>The MAC address.</returns>
		public static string GetMacAddressOld()
		{
			string macAddresses = string.Empty;

			foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (nic.OperationalStatus == OperationalStatus.Up)
				{
					macAddresses += nic.GetPhysicalAddress().ToString();
					break;
				}
			}

			return macAddresses;
		}

		public static string GetSystemMACID()
		{
			string systemName = System.Windows.Forms.SystemInformation.ComputerName;
			try
			{
				ManagementScope theScope = new ManagementScope("\\\\" + Environment.MachineName + "\\root\\cimv2");
				ObjectQuery theQuery = new ObjectQuery("SELECT * FROM Win32_NetworkAdapter");
				ManagementObjectSearcher theSearcher = new ManagementObjectSearcher(theScope, theQuery);
				ManagementObjectCollection theCollectionOfResults = theSearcher.Get();

				foreach (ManagementObject theCurrentObject in theCollectionOfResults)
				{
					if (theCurrentObject["MACAddress"] != null)
					{
						string macAdd = theCurrentObject["MACAddress"].ToString();
						//return macAdd.Replace(':', '-');
						return macAdd;
					}
				}
			}
			catch (ManagementException e)
			{
			}
			catch (System.UnauthorizedAccessException e)
			{

			}
			return string.Empty;
		}

		public static  string GetIpAddress()
		{
			try
			{
				string hostName = Dns.GetHostName(); // Retrive the Name of HOST
													 // Get the IP
				string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

				return myIP;
			}
			catch
			{
				return string.Empty;
			}
			
		}


		public static string GetMachineName()
		{
			try
			{
				string hostName = Dns.GetHostName(); // Retrive the Name of HOST
													 // Get the IP
				string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

				return myIP;
			}
			catch
			{
				return string.Empty;
			}

		}

		/// <summary>
		/// Microsoft Windows 10 Home Single Language
		/// </summary>
		/// <returns> Microsoft Windows 10 Home Single Language </returns>
		public static string GetOSFriendlyName()
		{
			string result = string.Empty;
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
			foreach (ManagementObject os in searcher.Get())
			{
				result = os["Caption"].ToString();
				break;
			}
			return result;
		}


		/// <summary>
		/// Microsoft Windows 10 Home 
		/// </summary>
		/// <returns> Microsoft Windows 10 Home </returns>
		public static string FriendlyName()
		{
			string ProductName = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
			string CSDVersion = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
			if (ProductName != "")
			{
				return (ProductName.StartsWith("Microsoft") ? "" : "Microsoft ") + ProductName +
							(CSDVersion != "" ? " " + CSDVersion : "");
			}
			return "";
		}

		public static string HKLM_GetString(string path, string key)
		{
			try
			{
				RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
				if (rk == null) return "";
				return (string)rk.GetValue(key);
			}
			catch { return ""; }
		}

		#endregion
	}
}
