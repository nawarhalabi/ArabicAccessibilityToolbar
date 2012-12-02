using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Karna.Magnification;
using System.Globalization;
using System.Threading;
using System.Reflection;

namespace AccessibilityToolbar
{
    public partial class Form1 : Form
    {
        magnifier magnifierWindow = null;
        Magnifier mgn = null;
        Overlay overlay = new Overlay();
        System.Diagnostics.Process pKeyboard = null;
        System.Diagnostics.Process pNarrtor = null;
        PreferencesForm preferencesForm = null;

        public Form1()
        {
            //When the overlay is closed uncheck the overlay button-----------------
            overlay.FormClosing += new FormClosingEventHandler(uncheckContrast);
            //Apply settings on the toolbar when they get changed--------------------
            Properties.Settings.Default.PropertyChanged +=new PropertyChangedEventHandler(settingsChanged);
            InitializeComponent();
        }

        private void settingsChanged(Object sender, PropertyChangedEventArgs e)
        {
            applySettings();
        }

        //Magnifier------------------------------------------------------------------------------------
        private void magnifierButton_Click(object sender, EventArgs e)
        {
            startMagnifier();
        }

        private void startMagnifier()
        {
            if (magnifierButton.Checked)
            {
                magnifierWindow = new magnifier();
                magnifierWindow.Show();
                mgn = new Karna.Magnification.Magnifier(magnifierWindow, Properties.Settings.Default.isLens);
                magnifierWindow.FormClosing += new FormClosingEventHandler(uncheckMagnifier);
                applySettings();
            }
            else
            {
                if (magnifierWindow != null)
                {
                    magnifierWindow.Close();
                    magnifierWindow.Dispose();
                }
            }
        }

        private void uncheckMagnifier(Object sender, FormClosingEventArgs e)
        {//called when the magnifier is closed form an external actor
            magnifierButton.Checked = false;
        }

        //On-Screen keyboard--------------------------------------------------------------------------
        private void keyboardButton_Click(object sender, EventArgs e)
        {
            startKeyboard();
        }

        private void startKeyboard()
        {
            if (keyboardButton.Checked)
            {
                pKeyboard = System.Diagnostics.Process.Start("osk.exe");
                pKeyboard.Exited += new EventHandler(uncheckKeyboard);
                pKeyboard.EnableRaisingEvents = true;
            }
            else
                if (pKeyboard != null)
                {
                    try
                    {
                        pKeyboard.Kill();
                    }
                    catch { }
                }
        }

        private delegate void uncheck(Object sender, EventArgs ev);

        private void uncheckKeyboard(Object sender, EventArgs e)
        {//called when the on-screen keyboard is closed form an external actor
            if (keyboardButton.InvokeRequired)
            {
                uncheck u = new uncheck(uncheckKeyboard);
                try
                {
                    this.Invoke(u, new object[] { sender, e });
                }
                catch (Exception ex)
                {
                    Exception exep = ex;
                }
            }
            else
                keyboardButton.Checked = false;
        }

        //Overlay------------------------------------------------------------------------
        private void contrastButton_Click(object sender, EventArgs e)
        {
            startContrast();
        }

        private void startContrast()
        {
            if (contrastButton.Checked)
            {
                if (overlay == null || overlay.IsDisposed)
                {
                    overlay = new Overlay();
                    overlay.FormClosing +=new FormClosingEventHandler(uncheckContrast);
                }
                overlay.Show();
            }
            else
            {
                if (overlay != null)
                    overlay.Visible = false;
            }
        }

        private void uncheckContrast(Object sender, FormClosingEventArgs e)
        {//called when the overlay is closed form an external actor
            contrastButton.Checked = false;
        }

        //Narrator-------------------------------------------------------------------------------------
        private void narratorButton_Click(object sender, EventArgs e)
        {
            startNarrator();
        }

        private void startNarrator()
        {
            if (narratorButton.Checked)
            {
                pNarrtor = System.Diagnostics.Process.Start("Narrator.exe");
                pNarrtor.Exited += new EventHandler(uncheckNarrator);
                pNarrtor.EnableRaisingEvents = true;
            }
            else
                if (pNarrtor != null)
                {
                    try
                    {
                        pNarrtor.Kill();
                    }
                    catch { }
                }
        }

        private void uncheckNarrator(Object sender, EventArgs e)
        {//called when the narrator is closed form an external actor------------------
            if (narratorButton.InvokeRequired)
            {
                uncheck u = new uncheck(uncheckNarrator);
                this.Invoke(u, new object[] { sender, e });
            }
            else
                narratorButton.Checked = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Apply the related settings to the form when it loads---------------------
            applySettings();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ar");
            Properties.Settings.Default.Reload();
        }

        private void applySettings()
        {
            if (mgn != null)
            {
                mgn.isLens = Properties.Settings.Default.isLens;
                mgn.Magnification = (float)(Properties.Settings.Default.magnificationRate + 2.0) / 2;
            }

            TopMost = Properties.Settings.Default.alwaysOnTop;
            
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Save preferences-----------------------------------------------------------------------------
            Properties.Settings.Default.Save();

            //Close all sub-windows before closing the application (on-screen keyboard, narrator, overlay and magnifier)-------------
            if (magnifierWindow != null)
                magnifierWindow.Close();
            if (overlay != null)
                overlay.Close();
            if (pKeyboard != null)
                try
                {
                    pKeyboard.Kill();
                }
                catch { }
            if (pNarrtor != null)
                try
                {
                    pNarrtor.Kill();
                }
                catch { }
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (preferencesForm == null || preferencesForm.IsDisposed)
            {
                preferencesForm = new PreferencesForm();
                preferencesForm.Show();
            }
            else
            {
                preferencesForm.Focus();
            }
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case(Keys.Control | Keys.K):
                    keyboardButton.Checked = !keyboardButton.Checked;
                    //startKeyboard();
                    return true;
                case (Keys.Control | Keys.M):
                    magnifierButton.Checked = !magnifierButton.Checked;
                    //startMagnifier();
                    return true;
                case (Keys.Control | Keys.G):
                    contrastButton.Checked = !contrastButton.Checked;
                    //startContrast();
                    return true;
                case (Keys.Control | Keys.R):
                    narratorButton.Checked = !narratorButton.Checked;
                    //startNarrator();
                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }
    }
}
