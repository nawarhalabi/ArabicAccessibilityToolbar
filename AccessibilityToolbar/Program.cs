using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace AccessibilityToolbar
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew;
            using (Mutex mutex = new Mutex(true, "ArabicAccessibilityToolbar", out createdNew))
                //This to prevent multiple toolbar instance from being launched in parallel
            {
                if (createdNew)
                {
                    Application.CurrentCulture = new System.Globalization.CultureInfo("ar");

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
            }
        }
    }
}
