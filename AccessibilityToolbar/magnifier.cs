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
    public partial class magnifier : Form
    {
        int defaultStyle;

        public magnifier()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern int GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        static extern int SetWindowLong(IntPtr hWnd, int index, int val);

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

        private void magnifier_Load(object sender, EventArgs e)
        {
            //Don't let the magnifier grow very large------------------------------------
            Rectangle r = Screen.PrimaryScreen.Bounds;
            MaximumSize = new Size((int)(r.Width*0.75), (int)(r.Height * 0.75));

            defaultStyle = GetWindowLong(Handle, -20);
            Properties.Settings.Default.PropertyChanged += new PropertyChangedEventHandler(settingsChanged);
            applySettings();
        }


        private void settingsChanged(Object sender, PropertyChangedEventArgs args)
        {
            applySettings();
        }

        private void applySettings()
        {
            Size = new Size(Properties.Settings.Default.maxMagnifierWidth, Properties.Settings.Default.maxMagnifierHeight);
            TopMost = Properties.Settings.Default.alwaysOnTop;

            if (Properties.Settings.Default.isLens)
            {
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                SetWindowLong(this.Handle, -20, defaultStyle | 0x80000 | 0x20);
                SetLayeredWindowAttributes(Handle, 0, 255, LayeredWindowAttributeFlags.LWA_ALPHA);
            }
            else
            {
                SetWindowLong(Handle, -20, defaultStyle);
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
                SetLayeredWindowAttributes(Handle, 0, 0, LayeredWindowAttributeFlags.LWA_ALPHA);
            }
        }

        private void magnifier_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.PropertyChanged -= new PropertyChangedEventHandler(settingsChanged);
        }
    }
}
