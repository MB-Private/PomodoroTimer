﻿#region File Header
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
//  File       : PomodoroIdleState.cs
//  Author     : Mikael Brunnegård
//  Description: //TODO - Description of file PomodoroIdleState.cs
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
    internal class Idle : PomodoroBaseState
    {
        public override void SetNextState(PomodoroController controller)
        {
            controller.State = new LazerFocused();
        }

        public override void Start(PomodoroController controller)
        {
            if (controller.FocusOnText.Length > 0)
            {
                SetNextState(controller);
            }
        }
    }
}