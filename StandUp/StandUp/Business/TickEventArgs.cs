using System;
using System.Collections.Generic;
using System.Text;

namespace StandUp.Business
{
    class TickEventArgs : EventArgs
    {
        public bool Snoozing { get; private set; }
        public TimeSpan Time { get; private set; }
        public ShowTimeColor Color { get;  private set; }

        public TickEventArgs(bool snoozing, TimeSpan time, ShowTimeColor color)
        {
            Snoozing = snoozing;
            Time = time;
            Color = color;
        }
    }
}
