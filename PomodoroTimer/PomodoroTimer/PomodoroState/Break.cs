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
//  File       : Break.cs
//  Author     : Mikael Brunnegård
//  Description: //TODO - Description of file Break.cs
//  Notes      :
// -------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PomodoroTimer.Configuration;

namespace PomodoroTimer.PomodoroState
{
    internal class Break : PomodoroBaseState
    {
        public Break()
        {
            MinutesInState  = ApplicationSettings.Instance.BreakTime;
            MinuteCountDown = MinutesInState;
            StartTime       = DateTime.Now;

            Start(MilisecondsPerMinute);
        }

        public override void SetNextState(PomodoroController controller)
        {
            controller.State = new Idle();
        }

        public override void Pause(PomodoroController controller)
        {
            PauseImpl(controller);
        }
    }
}