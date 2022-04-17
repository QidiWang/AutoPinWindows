//Credits:
//https://github.com/MScholtes/VirtualDesktop
//https://github.com/Grabacr07/VirtualDesktop

using System;
using System.Runtime.InteropServices;

namespace PinWindows1803
{
	internal static class Guids
	{
		public static readonly Guid CLSID_ImmersiveShell = new Guid("C2F03A33-21F5-47FA-B4BB-156362A2F239");
		public static readonly Guid CLSID_VirtualDesktopPinnedApps = new Guid("B5A399E7-1C87-46B8-88E9-FC5747B171BD");
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
	[Guid("871F602A-2B58-42B4-8C4B-6C43D642C06F")]
	internal interface IApplicationView
	{
		int UselessFunction1();
		int UselessFunction2();
		int UselessFunction3();
		int UselessFunction4();
		int UselessFunction5();
		int GetVisibility(out int visibility);
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("2C08ADF0-A386-4B35-9250-0FE183476FCC")]
	internal interface IApplicationViewCollection
	{
		int UselessFunction1();
		int UselessFunction2();
		int UselessFunction3();
		int GetViewForHwnd(IntPtr hwnd, out IApplicationView view);
	}


	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("4CE81583-1E4C-4632-A621-07A53543148F")]
	internal interface IVirtualDesktopPinnedApps
	{
		bool UselessFunction1();
		void UselessFunction2();
		void UselessFunction3();
		bool IsViewPinned(IApplicationView applicationView);
		void PinView(IApplicationView applicationView);
		void UnpinView(IApplicationView applicationView);
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
	internal interface IServiceProvider10
	{
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object QueryService(ref Guid service, ref Guid riid);
	}

	public class PinWindows1803 : PinWindows
	{
		public PinWindows1803()
		{
			var shell = (IServiceProvider10)Activator.CreateInstance(Type.GetTypeFromCLSID(Guids.CLSID_ImmersiveShell));
			_applicationViewCollection = (IApplicationViewCollection)shell.QueryService(typeof(IApplicationViewCollection).GUID, typeof(IApplicationViewCollection).GUID);
			_virtualDesktopPinnedApps = (IVirtualDesktopPinnedApps)shell.QueryService(Guids.CLSID_VirtualDesktopPinnedApps, typeof(IVirtualDesktopPinnedApps).GUID);
		}

		internal IApplicationViewCollection _applicationViewCollection;
		internal IVirtualDesktopPinnedApps _virtualDesktopPinnedApps;
		public override int PinWindow(IntPtr hWnd, string title)
		{
			if (hWnd == IntPtr.Zero) return 1;

			IApplicationView view;
			try
			{
				_applicationViewCollection.GetViewForHwnd(hWnd, out view);
			}
			catch
			{
				return 1;
			}

			if (view == null)
			{
				return 1;
			}

			try
			{
				int visible = 0;
				view.GetVisibility(out visible);

				if (visible == 0)
				{
					return 1;
				}
				if (!_virtualDesktopPinnedApps.IsViewPinned(view))
				{
					_virtualDesktopPinnedApps.PinView(view);
					AutoPinWindows.Program.OutputMessage("pin " + title);
				}
			}
			catch (Exception e)
			{
				AutoPinWindows.Program.OutputMessage(title + " pin error: " + e.Message);
				return 1;
			}
			return 0;
		}

		public override int UnpinWindow(IntPtr hWnd, string title)
		{
			if (hWnd == IntPtr.Zero) return 1;

			IApplicationView view;
			try
			{
				_applicationViewCollection.GetViewForHwnd(hWnd, out view);
			}
			catch
			{
				return 1;
			}

			if (view == null)
			{
				return 1;
			}

			try
			{
				int visible = 0;
				view.GetVisibility(out visible);

				if (visible == 0)
				{
					return 1;
				}
				if (_virtualDesktopPinnedApps.IsViewPinned(view))
				{
					_virtualDesktopPinnedApps.UnpinView(view);
					AutoPinWindows.Program.OutputMessage("unpin " + title);
				}
			}
			catch (Exception e)
			{
				AutoPinWindows.Program.OutputMessage(title + " unpin error: " + e.Message);
				return 1;
			}
			return 0;
		}

	}
}
