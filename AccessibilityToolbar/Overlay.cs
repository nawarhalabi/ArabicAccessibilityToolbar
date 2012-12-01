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
    public partial class Overlay : Form
    {
        int test = 0;
        int rulerWidth = 50;
        public Overlay()
        {
            InitializeComponent();
        }

        private void Overlay_Load(object sender, EventArgs e)
        {
            Properties.Settings.Default.PropertyChanged += new PropertyChangedEventHandler(settingsChanged);
        }

        private void applySettings()
        {//apply selected settings on the overlay --------------------------------
            //IntPtr desktop = GetDesktopWindow();
            //SetParent(Handle, desktop);
            //Get the primary screen rectangle--------------------------------------
            Rectangle s = Screen.PrimaryScreen.Bounds;
            //set the size of the overlay to the size of the screen--------------
            Size = s.Size;
            //Set the start position to be manual at the top-left corner-----------
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            TopMost = true;

            //Overlay must be click-throughable ------------------------------------
            int initialStyle = GetWindowLong(Handle, -20);
            SetWindowLong(Handle, -20, initialStyle | 0x80000 | 0x20);
            //Set the overlay color-------------------------------------------------
            BackColor = Color.FromArgb(Properties.Settings.Default.overlayR, Properties.Settings.Default.overlayG, Properties.Settings.Default.overlayB);
            //Set the opacity of the overlay to 140 .... any suggestions???
            SetLayeredWindowAttributes(Handle, 0, 140, LayeredWindowAttributeFlags.LWA_ALPHA);

            if (Properties.Settings.Default.isRuler)//If ruler is on, hook the mouse moves with onMouseMove event
                Gma.UserActivityMonitor.HookManager.MouseMove += new MouseEventHandler(onMouseMove);
            else
            {//else unhool it
                Gma.UserActivityMonitor.HookManager.MouseMove -= new MouseEventHandler(onMouseMove);
                //If not the ruler is on cover the whole screen with the overlay
                Region = new Region(Screen.PrimaryScreen.WorkingArea);//Set the region
                Invalidate();//force redraw
            }
            //Apply ruler width-----------------------------------------------------
            rulerWidth = Properties.Settings.Default.rulerWidth;
            if (!Properties.Settings.Default.isRuler)
                rulerWidth = 0;
        }

        private void onMouseMove(object sender, MouseEventArgs e)
        {
            if (Properties.Settings.Default.isRuler)//If reading ruler is on update its posistion based on the cursor
            {
                Graphics ee = CreateGraphics();//The window graphics
                Rectangle rulerArea = new Rectangle(0, Cursor.Position.Y - rulerWidth / 2, Screen.PrimaryScreen.Bounds.Width, rulerWidth);

                //Get the screen rectangle------------------------------
                ee.Clip = new Region(Screen.PrimaryScreen.WorkingArea);
                //Exclude the ruler area--------------------------------
                ee.ExcludeClip(rulerArea);

                Region = ee.Clip;//Set the region
            }
        }

        private void settingsChanged(Object sender, PropertyChangedEventArgs args)
        {
            applySettings();
        }
        //Native methods needed-------------------------------------------------------------
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

        private void Overlay_VisibleChanged(object sender, EventArgs e)
        {
            applySettings();
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
    }
}