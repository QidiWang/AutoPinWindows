using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

public static class OpenWindowsGetter
{
	public static List<Tuple<IntPtr, string>> GetOpenWindows()
	{
		IntPtr shellWindow = GetShellWindow();
		var windows = new List<Tuple<IntPtr, string>>();

		EnumWindows(delegate (IntPtr hWnd, int lParam)
		{
			if (hWnd == shellWindow) return true;
			if (!IsWindowVisible(hWnd)) return true;

			int length = GetWindowTextLength(hWnd);
			StringBuilder builder = new StringBuilder(length);
			GetWindowText(hWnd, builder, length + 1);

			windows.Add(new Tuple<IntPtr, string>(hWnd, builder.ToString()));
			return true;

		}, 0);

		return windows;
	}

	private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

	[DllImport("USER32.DLL")]
	private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

	[DllImport("USER32.DLL")]
	private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

	[DllImport("USER32.DLL")]
	private static extern int GetWindowTextLength(IntPtr hWnd);

	[DllImport("USER32.DLL")]
	private static extern bool IsWindowVisible(IntPtr hWnd);

	[DllImport("USER32.DLL")]
	private static extern IntPtr GetShellWindow();
}