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
//  File       : PomodoroPausedState.cs
//  Author     : Mikael Brunnegård
//  Description: //TODO - Description of file PomodoroPausedState.cs
//  Notes      :
// -------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomodoroTimer.PomodoroState
{
    internal class Paused : PomodoroBaseState
    {
        private     DateTime            PauseTime       { get; set; }

        public      PomodoroBaseState   PreviousState   { get; private set; }


        private Paused()
        {
            throw new InvalidOperationException("Use the other constructor instead!");
        }

        public Paused(PomodoroBaseState previousState)
        {
            PauseTime       = DateTime.Now;

            previousState.Stop();

            Console.WriteLine($"{nameof(Paused)}");
            Console.WriteLine($"   {PauseTime}");

            StartTime       = previousState.StartTime;
            MinutesInState  = previousState.MinutesInState;
            MinuteCountDown = previousState.MinuteCountDown;
            
            PreviousState   = previousState;
        }
        
        public override void Start(PomodoroController controller)
        {
            TimeSpan    elapsedFullMinutes  = new TimeSpan(0, 0, MinutesInState - MinuteCountDown, 0);
            TimeSpan    elapsedTime         = PauseTime.Subtract(StartTime).Subtract(PreviousState.TotalPauseTimeSpan);
            TimeSpan    totalPauseTimeSpan  = DateTime.Now.Subtract(StartTime).Subtract(elapsedTime);
            double      minuteTimerTimeLeft = MilisecondsPerMinute - elapsedTime.Subtract(elapsedFullMinutes).TotalMilliseconds;

            Console.WriteLine($"{nameof(Paused)}");
            Console.WriteLine($"   elapsedFullMinutes  = {elapsedFullMinutes.TotalMilliseconds}");
            Console.WriteLine($"   elapsedTime         = {elapsedTime.TotalMilliseconds}");
            Console.WriteLine($"   totalPauseTimeSpan  = {totalPauseTimeSpan.TotalMilliseconds}");
            Console.WriteLine($"   minuteTimerTimeLeft = {minuteTimerTimeLeft}");

            controller.State = PreviousState;
            controller.State.Start(minuteTimerTimeLeft, totalPauseTimeSpan);
        }
    }
}