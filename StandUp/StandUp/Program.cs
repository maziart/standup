using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using StandUp.UI;

namespace StandUp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Process.GetProcessesByName("StandUp").Length > 1)
            {
                MessageBox.Show("Another instance is running");
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (new MainUI())
            {
                Application.Run();
            }
        }
    }
}
