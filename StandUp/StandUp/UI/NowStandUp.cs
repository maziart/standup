using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using StandUp.Business;

namespace StandUp.UI
{
    public partial class NowStandUp : Form
    {
        private DateTime StartTime;
        private bool AllowSnooze;
        private int TotalOpacitySteps;
        private int CurrentOpacityStep;

        public NowStandUp(bool allowSnooze)
        {
            InitializeComponent();

            StartTime = DateTime.Now;

            AllowSnooze = allowSnooze;

            var bounds = Screen.PrimaryScreen.Bounds;
            Size = new Size(bounds.Width, bounds.Height);
            Location = new Point(0, 0);
            lblMessage.Text = Settings.Message;


            if (Settings.MessageFadeInSeconds == 0)
            {
                ShowRightAway();
            }
            else
            {
                Opacity = 0;
                CurrentOpacityStep = 0;
                TotalOpacitySteps = Settings.MessageFadeInSeconds * 10;
                fadeInTimer.Start();
            }
        }

        private void ShowRightAway()
        {
            StartTime = DateTime.Now;
            ShowTime();
            standTimer.Start();

            Opacity = 1;
            BringToFront();
            Activate();
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            CloseForm();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && Settings.AllowEscapingMessage)
                CloseForm();
        }

        public bool Snooze { get; private set; }
        public bool ShowDesktop { get; private set; }
        public int ShowDesktopSeconds { get; private set; }

        private void CloseForm()
        {
            FormClosing -= NowStandUp_FormClosing;
            Snooze = Settings.AllowSnooze && AllowSnooze && (DateTime.Now - StartTime) < new TimeSpan(0, 0, Settings.StandUpSeconds);            
            Close();
        }

        private void btnHideForm_Click(object sender, EventArgs e)
        {
            ShowDesktop = true;
            ShowDesktopSeconds = Settings.StandUpSeconds - (int)(DateTime.Now - StartTime).TotalSeconds;
            FormClosing -= NowStandUp_FormClosing;
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ShowTime();
        }

        private void ShowTime()
        {
            var time = (int)Math.Ceiling((new TimeSpan(0, 0, Settings.StandUpSeconds) - (DateTime.Now - StartTime)).TotalSeconds);
            if (time < 0)
            {
                lblTime.Text = "You can sit down";
                BtnClose.Text = "I'm sitting down";
                btnHideForm.Visible = false;
                standTimer.Stop();
            }
            else
            {
                BtnClose.Text = "Snooze";
                btnHideForm.Visible = true;
                lblTime.Text = string.Format("{0:00}:{1:00}", time / 60, time % 60);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void NowStandUp_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseForm();
        }

        private void pnlFooter_MouseEnter(object sender, EventArgs e)
        {
            pnlButtons.Visible = true;
        }

        private void fadeInTimer_Tick(object sender, EventArgs e)
        {
            CurrentOpacityStep++;
            if (CurrentOpacityStep < TotalOpacitySteps)
            {
                Opacity = (float)CurrentOpacityStep / TotalOpacitySteps;
            }
            else
            {
                fadeInTimer.Stop();
                ShowRightAway();
            }
        }
    }
}
