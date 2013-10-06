using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using StandUp.UI;

namespace StandUp.Business
{
    class StandUpTimer
    {
        private DateTime StartTime;
        private TimeSpan Length;
        private readonly TimeSpan OneSecond = new TimeSpan(0, 0, 1);
        private TimeSpan RedThreshold;
        private Timer MainTimer;
        private Timer StandTimer;
        private MainUI UI;
        private bool Snoozing;
        private bool IsInRedMode;

        public StandUpTimer(MainUI ui)
        {
            UI = ui;

            InitTimers();

            ApplySettings();
            Reset();
        }

        private void InitTimers()
        {
            MainTimer = new Timer();
            MainTimer.Interval = 1000;
            MainTimer.Tick += MainTimer_Tick;
        }

        public event EventHandler<TickEventArgs> Tick;
        public event EventHandler<TickEventArgs> EnteredRedMode;
        public event EventHandler CanSitDown;

        private void OnTick(TickEventArgs e)
        {
            if (Tick != null)
                Tick(this, e);
        }
        private void OnEnteredRedMode(TickEventArgs e)
        {
            if (EnteredRedMode != null)
                EnteredRedMode(this, e);
        }
        private void OnCanSitDown(EventArgs e)
        {
            if (CanSitDown != null)
                CanSitDown(this, e);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Exit(e.CloseReason == CloseReason.UserClosing))
                e.Cancel = true;
        }

        private void StandTimer_Tick(object sender, EventArgs e)
        {
            StandTimer.Stop();
            OnCanSitDown(e);

            Reset();
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            var time = OneSecond + Length - (DateTime.Now - StartTime);
            if (time.TotalSeconds > 0)
            {
                ShowTime(time);
            }
            else if (time.TotalSeconds < -2 * Settings.TotalSeconds)
            {
                Reset();
            }
            else
            {
                ShowStandUpNow(true);
            }
        }

        public void ApplySettings(bool setLocation = true)
        {
            RedThreshold = new TimeSpan(0, 0, Settings.RedSeconds);
        }


        public void ShowStandUpNow(bool allowSnooze)
        {
            MainTimer.Stop();
            using (var standUpNow = new NowStandUp(allowSnooze))
            {
                standUpNow.ShowDialog();
                if (standUpNow.Snooze)
                {
                    Snoozing = true;
                    Reset(60);
                    return;
                }
                else if (standUpNow.ShowDesktop && standUpNow.ShowDesktopSeconds > 0)
                {
                    StandTimer = new Timer();
                    StandTimer.Interval = standUpNow.ShowDesktopSeconds * 1000;
                    StandTimer.Tick += StandTimer_Tick;
                    StandTimer.Start();
                    return;
                }
            }
            Reset();
        }


        public void Reset()
        {
            Snoozing = false;
            Reset(Settings.TotalSeconds);
        }
        public void Reset(int seconds)
        {
            if (!Snoozing)
                IsInRedMode = false;

            StartTime = DateTime.Now;
            Length = new TimeSpan(0, 0, seconds);
            ShowTime(Length);
            MainTimer.Start();
        }

        public bool Exit(bool prompt = true)
        {
            if (prompt && MessageBox.Show("Are you sure you want to exit stand-up application?", "Exit Stand Up?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                return false;
            Settings.Save();
            Application.Exit();
            return true;
        }


        private void ShowTime(TimeSpan timeSpan)
        {
            var args = new TickEventArgs(Snoozing, timeSpan, timeSpan < RedThreshold ? ShowTimeColor.Red : ShowTimeColor.Normal);
            OnTick(args);
            if (timeSpan < RedThreshold)
            {
                if (!IsInRedMode && Settings.AllowPrepareNotification)
                {
                    IsInRedMode = true;
                    OnEnteredRedMode(args);
                }
            }
        }

        public void ShowSettings()
        {
            using (var form = new SettingsForm())
            {
                MainTimer.Stop();
                if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    MainTimer.Start();
                }
                else
                {
                    ApplySettings(false);
                    Reset();
                }
            }
        }
    }
}
