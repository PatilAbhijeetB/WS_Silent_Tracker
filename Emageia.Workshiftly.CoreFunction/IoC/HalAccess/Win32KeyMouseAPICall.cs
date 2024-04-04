using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.CoreFunction.IoC.HalAccess
{

	/// <summary>
	/// cbSize

	//Type: UINT

	//The size of the structure, in bytes.This member must be set to sizeof(LASTINPUTINFO).

	//dwTime

	//Type: DWORD

	//The tick count when the last input event was received.
	/// </summary>
	internal struct LASTINPUTINFO
	{
		public uint cbSize;

		public uint dwTime;
	}

	/// <summary>
	/// Pointer to call get last input details
	/// </summary>
	public class Win32KeyMouseAPICall
	{
		#region User32 pointer

		[DllImport("User32.dll")]
		public static extern bool LockWorkStation();

		[DllImport("User32.dll")]
		private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

		[DllImport("Kernel32.dll")]
		private static extern uint GetLastError();
		#endregion


		#region Method

		public static uint GetIdleTime()
		{
			LASTINPUTINFO lastInPut = new LASTINPUTINFO();
			lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
			GetLastInputInfo(ref lastInPut);

			return ((uint)Environment.TickCount - lastInPut.dwTime);
		}

		public static long GetTickCount()
		{
			return Environment.TickCount;
		}

		public static long GetLastInputTime()
		{
			LASTINPUTINFO lastInPut = new LASTINPUTINFO();
			lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
			if (!GetLastInputInfo(ref lastInPut))
			{
				throw new Exception(GetLastError().ToString());
			}

			return lastInPut.dwTime;
		}


		/// <summary>
		/// last Input Time
		/// </summary>
		/// <returns> return Datetime</returns>
		public static DateTime GetLastInputTimes()
		{
			int idleTime = 0;
			LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
			lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
			lastInputInfo.dwTime = 0;

			int envTicks = Environment.TickCount;

			if (GetLastInputInfo(ref lastInputInfo))
			{
				int lastInputTick = (int)lastInputInfo.dwTime;

				idleTime = envTicks - lastInputTick;
			}

			var idleTimeSecs = ((idleTime > 0) ? (idleTime / 1000) : idleTime);
			return DateTime.Now.AddSeconds(-idleTimeSecs);
		}


		public static Int32 GetLastInputTimesUnix()
		{
			int idleTime = 0;
			LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
			lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
			lastInputInfo.dwTime = 0;

			int envTicks = Environment.TickCount;

			if (GetLastInputInfo(ref lastInputInfo))
			{
				int lastInputTick = (int)lastInputInfo.dwTime;

				idleTime = envTicks - lastInputTick;
			}

			var idleTimeSecs = ((idleTime > 0) ? (idleTime / 1000) : idleTime);
			var date = DateTime.Now.AddSeconds(-idleTimeSecs);
			//	var ni = Int32.Parse(DateTime.Now.AddSeconds(-idleTimeSecs));


			Int32 currentTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
			Int32 currentTimestampc = currentTimestamp - idleTimeSecs;
			
			return currentTimestamp - idleTimeSecs;
		}

		/// <summary>
		/// with milisecond
		/// </summary>
		/// <returns></returns>
		public static TimeSpan GetLastInputTimess()
		{
			LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
			lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
			lastInputInfo.dwTime = 0;

			int envTicks = Environment.TickCount;

			if (GetLastInputInfo(ref lastInputInfo))
			{
				long lastInputTick = lastInputInfo.dwTime;
				long milliseconds = Environment.TickCount - lastInputTick;

				int seconds = (int)(milliseconds / 1000);
				TimeSpan result = new TimeSpan(0, 0, seconds);

				//Logger.Log("ResultTimeSpanSeconds: {0}", result.TotalSeconds);

				return result;
			}

			throw new Win32Exception();
		}


		public static int GetLastInputTimesTogetSignBit()
		{
			int idleTime = 0;
			LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
			lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
			lastInputInfo.dwTime = 0;

			int envTicks = Environment.TickCount;

			if (GetLastInputInfo(ref lastInputInfo))
			{
				int lastInputTick = (int)lastInputInfo.dwTime;

				idleTime = envTicks - lastInputTick;
			}

			var idleTimeSecs = ((idleTime > 0) ? (idleTime / 1000) : idleTime);
			var date = DateTime.Now.AddSeconds(-idleTimeSecs);
			return idleTimeSecs != 0 ? -idleTimeSecs : 0;
		}


		public static int GetIdleTimeSec()
		{
			// Get the system uptime
			int systemUptime = Environment.TickCount;
			// The tick at which the last input was recorded
			int LastInputTicks = 0;
			// The number of ticks that passed since last input
			int IdleTicks = 0;

			// Set the struct
			LASTINPUTINFO LastInputInfo = new LASTINPUTINFO();
			LastInputInfo.cbSize = (uint)Marshal.SizeOf(LastInputInfo);
			LastInputInfo.dwTime = 0;

			// If we have a value from the function
			if (GetLastInputInfo(ref LastInputInfo))
			{
				// Get the number of ticks at the point when the last activity was seen
				LastInputTicks = (int)LastInputInfo.dwTime;
				// Number of idle ticks = system uptime ticks - number of ticks at last input
				IdleTicks = systemUptime - LastInputTicks;
			}
			return IdleTicks / 1000;
		}


		/// <summary>
		/// is user Idle 
		/// </summary>
		/// <returns> return true or flase</returns>
		private static bool IsUserIdle()
		{
			uint idleTime = 0;
			LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
			lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
			lastInputInfo.dwTime = 0;

			uint envTicks = (uint)Environment.TickCount;

			if (GetLastInputInfo(ref lastInputInfo))
			{
				uint lastInputTick = lastInputInfo.dwTime;
				idleTime = envTicks - lastInputTick;
			}

			idleTime = ((idleTime > 0) ? (idleTime / 1000) : 0);

			return (idleTime > 600) ? true : false; // idle for 10 minutes
		}
		#endregion
	}
}
