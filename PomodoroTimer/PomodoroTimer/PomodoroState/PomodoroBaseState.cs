#region File Header
// -------------------------------------------------------------------------------
// 
//  Copyright ABB Lorentzen & Wettre AB 2016
// 
//  The copyright to the computer programs herein is the property of
//  ABB Lorentzen & Wettre AB. The programs may be used and/or copied only with
//  the written permission from ABB Lorentzen & Wettre AB or in accordance with
//  the terms or conditions stipulated in the agreement/contract under which
//  the programs have been supplied.
//  The copyright and the foregoing restriction on reproduction, disclosure and use,
//  extend to all media in which the information may be embodied.
// 
// -------------------------------------------------------------------------------
//  File       : PomodoroState.cs
//  Author     : Mikael Brunnegård
//  Description: //TODO - Description of file PomodoroState.cs
//  Notes      :
// -------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PomodoroTimer.PomodoroState
{
    internal abstract class PomodoroBaseState
    {
        public event EventHandler<MinuteElapsedEventArgs> MinuteElapsedEvent;

        protected const double  MilisecondsPerMinute    = 60000;

        protected       Timer       MinuteTimer         { get; set; }

        public          DateTime    StartTime           { get; protected set; }
        public          TimeSpan    TotalPauseTimeSpan  { get; protected set; }
        public          int         MinuteCountDown     { get; protected set; }
        public          int         MinutesInState      { get; protected set; }


        protected PomodoroBaseState()
        {
            MinuteTimer             = new Timer(MilisecondsPerMinute);
            MinuteTimer.Elapsed     += MinuteTimer_Elapsed;
        }

        protected void MinuteTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            MinuteCountDown--;

            OnMinuteElapsedEvent(new MinuteElapsedEventArgs(MinuteCountDown));

            if (MinuteCountDown > 0)
            {
                MinuteTimer.Start();
            }
        }

        public virtual void SetNextState(PomodoroController controller)
        {
            throw new InvalidOperationException($"{nameof(SetNextState)} not allowed from state '{GetType().Name}'!");
        }

        protected virtual void OnMinuteElapsedEvent(MinuteElapsedEventArgs e)
        {
            MinuteElapsedEvent?.Invoke(this, e);
        }

        public virtual void Start(PomodoroController controller)
        {
            throw new InvalidOperationException($"{nameof(Start)} not allowed from state '{GetType().Name}'!");
        }

        protected void Start(double milisecondsLeftOnMinuteTimer)
        {
            Start(milisecondsLeftOnMinuteTimer, new TimeSpan(0));
        }

        public void Start(double milisecondsLeftOnMinuteTimer, TimeSpan totalPauseTimeSpan)
        {
            TotalPauseTimeSpan      = totalPauseTimeSpan;
            MinuteTimer.Interval    = milisecondsLeftOnMinuteTimer;
            MinuteTimer.Start();
        }

        public void Stop()
        {
            MinuteTimer.Stop();
        }

        public virtual void Pause(PomodoroController controller)
        {
            throw new InvalidOperationException($"{nameof(Pause)} not allowed from state '{GetType().Name}'!");
        }

        protected void PauseImpl(PomodoroController controller)
        {
            controller.State = new Paused(this);
        }
    }
}