using System;
using System.Collections.Generic;
using System.Text;

namespace StandUp.Business
{
    class StateMachine
    {
        private State state;

        public State State
        {
            get { return state; }
            set
            {
                if (SetState(value))
                {
                    OnStateChanged(EventArgs.Empty);
                }
            }
        }


        private StandUpTimer Timer;

        public StateMachine()
        {
            state = State.Ready;

            Timer = new StandUpTimer(this);
            Timer.Tick += Timer_Tick;
        }

        private bool SetState(State state)
        {
            if (this.state == state || (this.state == Business.State.Snoozing && state == Business.State.RedMode))
            {
                return false;
            }

            switch (state)
            {
                case State.Ready:
                    Timer.Reset();
                    break;
                case State.RedMode:
                    break;
                case State.Snoozing:
                    Timer.Reset(60);
                    break;
                case State.StandingUp:
                    if (this.state == Business.State.Snoozing)
                    {
                        Timer.AlrightStandingUp(Settings.StandUpSeconds);
                    }
                    else //ShowingStandUp
                    {
                        Timer.AlrightStandingUp();
                    }
                    break;
                case State.ShowingStandUp:
                    break;
                case State.ShowingSettings:
                    Timer.ShowSettings();
                    break;
                case State.CanSitDown:
                    this.state = Business.State.CanSitDown;
                    OnStateChanged(EventArgs.Empty);
                    State = state = Business.State.Ready;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException("state");
            }

            this.state = state;
            return true;
        }

        public event EventHandler StateChanged;
        private void OnStateChanged(EventArgs e)
        {
            if (StateChanged != null)
                StateChanged(this, e);
        }

        public event EventHandler<TickEventArgs> Tick;
        private void OnTick(TickEventArgs e)
        {
            if (Tick != null)
                Tick(this, e);
        }

        private void Timer_Tick(object sender, TickEventArgs e)
        {
            OnTick(e);
        }

        public void Reset()
        {
            this.state = Business.State.CanSitDown;
            State = Business.State.Ready;
        }
    }

    enum State
    {
        Ready,
        RedMode,
        Snoozing,
        StandingUp,
        CanSitDown,
        ShowingStandUp,
        ShowingSettings,
    }
}
