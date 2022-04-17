using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AutoPinWindows
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            bool success;
            var applicationContext = new AutoPinWindowsApplicationContext(out success);
            if (success)
            {
                Application.Run(applicationContext);
            }
        }
        public static void ShowOuptut()
        {
            _outputConsole.Show();
            _outputConsole.WindowState = FormWindowState.Normal;
            _outputConsole.Activate();
        }
        public static void OutputMessage(string msg)
        {
            _outputConsole.AppendMessge(msg);
        }

        private static OutputConsole _outputConsole = new OutputConsole();
    }

    public class AutoPinWindowsApplicationContext : ApplicationContext
    {
        private NotifyIcon _trayIcon;
        private System.Windows.Forms.Timer _timer;
        private PinWindows _pinWindows;
        public AutoPinWindowsApplicationContext(out bool success)
        {
            _pinWindows = CreatePinWindows.CreateInstance();

            if (_pinWindows == null)
            {
                MessageBox.Show("Windows version not supported!");
                success = false;
                return;
            }

            _trayIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.AppIcon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                new MenuItem("Show output", ShowOutput),
                new MenuItem("Exit", Exit)
            }),
                Visible = true
            };

            _timer = new System.Windows.Forms.Timer();
            _timer.Tick += new EventHandler(TimerEvent);
            _timer.Interval = 100;
            _timer.Start();

            success = true;
        }

        private void TimerEvent(Object myObject, EventArgs myEventArgs)
        {
            uint idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            uint environmentTicks = (uint)Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                uint lastInputTick = lastInputInfo.dwTime;
                idleTime = environmentTicks - lastInputTick;
            }

            if (idleTime > 5000)
            {
                _trayIcon.Icon = Properties.Resources.Disabled;
                return;
            }
            else
            {
                _trayIcon.Icon = Properties.Resources.AppIcon;
            }

            var openWindows = OpenWindowsGetter.GetOpenWindows();
            foreach (Tuple<IntPtr, string> window in openWindows)
            {
                IntPtr handle = window.Item1;
                string title = window.Item2;
                if (!System.Windows.Forms.Screen.FromHandle(handle).Primary)
                {
                    _pinWindows.PinWindow(handle, title);
                }
                else
                {
                    _pinWindows.UnpinWindow(handle, title);
                }
            }
        }

        void Exit(object sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            Application.Exit();
        }

        void ShowOutput(object sender, EventArgs e)
        {
            Program.ShowOuptut();
        }

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
    }
}
