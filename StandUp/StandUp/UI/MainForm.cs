using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using StandUp.Business;

namespace StandUp.UI
{
    public partial class MainForm : Form
    {
        #region Fields

        private StandUpTimer Timer;

        #endregion

        #region Constructor

        public MainForm()
        {
            InitializeComponent();

            Timer = new StandUpTimer(this);
            Timer.Tick += Timer_Tick;
            Timer.EnteredRedMode += Timer_EnteredRedMode;
            Timer.CanSitDown += Timer_CanSitDown;

            RegisterHotKey(this.Handle, 1, HotKeyModifiers.Alt | HotKeyModifiers.Control, Keys.S);
        }

        void Timer_CanSitDown(object sender, EventArgs e)
        {
            notifyIcon1.ShowBalloonTip(10000, "You can now sit down", "Sit down now or reset the timer when you do so", ToolTipIcon.Info);
        }

        void Timer_EnteredRedMode(object sender, TickEventArgs e)
        {
            notifyIcon1.ShowBalloonTip(2000, "Prepair to stand up", "Stand-up time is getting close. So prepare to stand up in " + TimeTranslator.Translate(e.Time), ToolTipIcon.Info);
        }

        void Timer_Tick(object sender, TickEventArgs e)
        {
            var time = string.Format("{0:00}:{1:00}", e.Time.Minutes, e.Time.Seconds);

            currentTimeToolStripMenuItem.Text = e.Snoozing ? (time + " (Snoozing)") : time;
            currentTimeToolStripMenuItem.ForeColor = e.Color == ShowTimeColor.Red ? Color.Red : Color.Black;

            notifyIcon1.Text = "You should stand up in " + TimeTranslator.Translate(e.Time);
        }

        #endregion

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x312)
            {
                ManualReset();
            }
            base.WndProc(ref m);
        }

        public void ManualReset()
        {
            Timer.Reset();
            notifyIcon1.ShowBalloonTip(2000, "Timer has been reset", "You should stand-up in " + TimeTranslator.Translate(new TimeSpan(0, 0, Settings.TotalSeconds)), ToolTipIcon.Info);
        }

        public enum HotKeyModifiers : uint
        {
            Alt = 1,
            Control = 2,
            Shift = 4,
            Win = 8
        }

        [DllImport("user32.dll", CallingConvention=CallingConvention.Winapi)]
        public static extern int RegisterHotKey(IntPtr hWnd, int id, HotKeyModifiers fsModifiers, Keys vk);

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Timer.Exit();
        }

        private void currentTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Timer.ShowStandUpNow(false);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Timer.ShowSettings();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManualReset();
        }
    }
}
