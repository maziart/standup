using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using StandUp.Properties;
using StandUp.Business;
using System.Drawing;
using System.Runtime.InteropServices;

namespace StandUp.UI
{
    internal class MainUI : IDisposable
    {
        #region Fields

        private StandUpTimer Timer;
        private NotifyIcon notifyIcon1;
        private ContextMenuStrip notifyMenuStrip;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem currentTimeToolStripMenuItem;
        private ToolStripMenuItem resetToolStripMenuItem;

        #endregion

        public MainUI()
        {
            InitializeComponent();
            Timer = new StandUpTimer(this);
            Timer.Tick += Timer_Tick;
            Timer.EnteredRedMode += Timer_EnteredRedMode;
            Timer.CanSitDown += Timer_CanSitDown;

            HotKeys.Register(HotKeyModifiers.Alt | HotKeyModifiers.Control, Keys.S, HotKeyRecieved);
        }

        void HotKeyRecieved(object sender, KeyPressedEventArgs e)
        {
            if (e.Keys == Keys.S && e.Modifiers == (HotKeyModifiers.Alt | HotKeyModifiers.Control))
            {
                ManualReset();
            }
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

        private void InitializeComponent()
        {
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon();
            this.notifyMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            this.currentTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            // 
            // notifyIcon1
            // 
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.ContextMenuStrip = notifyMenuStrip;
            notifyIcon1.Icon = Resources.Timer;
            notifyIcon1.Text = "Stand Up";
            notifyIcon1.Visible = true;
            // 
            // notifyMenuStrip
            // 
            notifyMenuStrip.Items.AddRange(new ToolStripItem[] {
            currentTimeToolStripMenuItem,
            resetToolStripMenuItem,
            settingsToolStripMenuItem,
            toolStripMenuItem1,
            exitToolStripMenuItem});
            notifyMenuStrip.Name = "notifyMenuStrip";
            // 
            // currentTimeToolStripMenuItem
            // 
            currentTimeToolStripMenuItem.Text = "CurrentTime";
            currentTimeToolStripMenuItem.Click += currentTimeToolStripMenuItem_Click;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Text = "Settings";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // resetToolStripMenuItem
            // 
            resetToolStripMenuItem.Text = "Reset";
            resetToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Alt | Keys.S;
            resetToolStripMenuItem.Click += resetToolStripMenuItem_Click;
        }


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

        public void ManualReset()
        {
            Timer.Reset();
            notifyIcon1.ShowBalloonTip(2000, "Timer has been reset", "You should stand-up in " + TimeTranslator.Translate(new TimeSpan(0, 0, StandUp.Business.Settings.TotalSeconds)), ToolTipIcon.Info);
        }

        public void Dispose()
        {
            notifyIcon1.Visible = false;
        }
    }
}
