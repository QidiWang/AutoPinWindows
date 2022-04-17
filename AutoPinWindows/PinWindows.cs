using System;
using Microsoft.Win32;

abstract public class PinWindows
{
	public abstract int PinWindow(IntPtr hWnd, string title);
	public abstract int UnpinWindow(IntPtr hWnd, string title);
}

static public class CreatePinWindows
{ 
	static public PinWindows CreateInstance()
	{
		var build = Int32.Parse(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuildNumber", string.Empty).ToString());
		if (build >= 17763)
		{
			return new PinWindows10.PinWindows10();
		}
		else if (build >= 17134)
		{
			return new PinWindows1803.PinWindows1803();
		}
		else if (build >= 14393)
		{
			return new PinWindows1607.PinWindows1607();
		}
		else
		{
			return null;
		}
	}
}
