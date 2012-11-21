using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AccessibilityToolbar
{
    public partial class ReadingRuler : Form
    {
        public ReadingRuler()
        {
            InitializeComponent();
        }

        private void ReadingRuler_Load(object sender, EventArgs e)
        {
            Properties.Settings.Default.PropertyChanged += new PropertyChangedEventHandler(settingsChanged);
            applySettings();
        }

        RulerPart top = new RulerPart();
        RulerPart buttom = new RulerPart();
        private void applySettings()
        {
            Rectangle s = Screen.PrimaryScreen.Bounds;

            Size = s.Size;
            Location = new Point(0, 0);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            TopMost = true;

            int initialStyle = GetWindowLong(Handle, -20);
            SetWindowLong(Handle, -20, initialStyle | 0x80000 | 0x20);
            SetLayeredWindowAttributes(Handle, 0, 0, LayeredWindowAttributeFlags.LWA_ALPHA);

            top.Location = new Point(0, 0);
            top.Width = Width;
            buttom.Width = Width;

            top.StartPosition = FormStartPosition.Manual;
            buttom.StartPosition = FormStartPosition.Manual;

            top.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            buttom.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            top.TopMost = true;
            buttom.TopMost = true;

            top.BackColor = Color.FromArgb(Properties.Settings.Default.overlayR, Properties.Settings.Default.overlayG, Properties.Settings.Default.overlayB);
            buttom.BackColor = Color.FromArgb(Properties.Settings.Default.overlayR, Properties.Settings.Default.overlayG, Properties.Settings.Default.overlayB);
            
            initialStyle = GetWindowLong(top.Handle, -20);
            SetWindowLong(top.Handle, -20, initialStyle | 0x80000 | 0x20);
            SetLayeredWindowAttributes(top.Handle, 0, 140, LayeredWindowAttributeFlags.LWA_ALPHA);

            initialStyle = GetWindowLong(buttom.Handle, -20);
            SetWindowLong(buttom.Handle, -20, initialStyle | 0x80000 | 0x20);
            SetLayeredWindowAttributes(buttom.Handle, 0, 140, LayeredWindowAttributeFlags.LWA_ALPHA);
            
            Gma.UserActivityMonitor.HookManager.MouseMove += new MouseEventHandler(onMouseMove);

            top.Show();
            buttom.Show();
        }

        private void settingsChanged(Object sender, PropertyChangedEventArgs args)
        {
            applySettings();
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern int GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        static extern int SetWindowLong(IntPtr hWnd, int index, int val);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(
        [MarshalAs(UnmanagedType.LPTStr)] string lpClassName,
        [MarshalAs(UnmanagedType.LPTStr)] string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(
        IntPtr hWndChild, // handle to window
        IntPtr hWndNewParent // new parent window
        );

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, LayeredWindowAttributeFlags dwFlags);
        [FlagsAttribute]
        [Description("Layered window flags")]
        public enum LayeredWindowAttributeFlags : int
        {
            /// <summary>
            /// Use key as a transparency color
            /// </summary>
            LWA_COLORKEY = 0x00000001,
            /// <summary>
            /// Use Alpha to determine the opacity of the layered window.
            /// </summary>
            LWA_ALPHA = 0x00000002
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        [FlagsAttribute]
        internal enum SetWindowPosFlags : int
        {
            SWP_NOSIZE = 1,
            SWP_NOMOVE = 2,
            SWP_NOZORDER = 4,
            SWP_NOREDRAW = 8,
            SWP_NOACTIVATE = 0x10,
            SWP_FRAMECHANGED = 0x20,
            SWP_SHOWWINDOW = 0x40,
            SWP_HIDEWINDOW = 0x80,
            SWP_NOCOPYBITS = 0x100,
            SWP_NOOWNERZORDER = 0x200,
            SWP_NOSENDCHANGING = 0x400
        }

        private void onMouseMove(object sender, MouseEventArgs e)
        {
            int y = Cursor.Position.Y;
            
            int topHeight = y - 50;
            int buttomHeight = Screen.PrimaryScreen.Bounds.Height - top.Height - 100;
            Point buttomLocation = new Point(0, y + 50);

            SetWindowPos(top.Handle, new IntPtr(-1), 0, 0, top.Width, topHeight, 0);
            SetWindowPos(buttom.Handle, new IntPtr(-1), 0, y + 50, buttom.Width, buttomHeight, 0);
        }
    }
}
