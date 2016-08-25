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
//  File       : PomodoroController.cs
//  Author     : Mikael Brunnegård
//  Description: //TODO - Description of file PomodoroController.cs
//  Notes      :
// -------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PomodoroTimer.PomodoroState;

namespace PomodoroTimer
{
    internal class PomodoroController
    {
        public event EventHandler MinuteElapsed;
        public event EventHandler StateChanged;

        private PomodoroBaseState m_State;

        public string               FocusOnText { get; set; }

        public PomodoroBaseState    State
        {
            get { return m_State; }
            set
            {
                if (m_State == value) { return; }

                if (m_State != null)
                {
                    m_State.MinuteElapsedEvent -= StateOnMinuteElapsedEvent;
                }

                m_State = value;

                if (m_State != null)
                {
                    m_State.MinuteElapsedEvent += StateOnMinuteElapsedEvent;
                    
                }

                OnStateChanged();
            }
        }

        private void StateOnMinuteElapsedEvent(object sender, MinuteElapsedEventArgs minuteElapsedEventArgs)
        {
            OnMinuteElapsed();
            if (minuteElapsedEventArgs.MinutesLeft == 0)
            {
                State.SetNextState(this);
            }
        }

        public PomodoroController()
        {
            InitializeState();
        }

        private void InitializeState()
        {
            State = new Idle();
        }

        public void Start(string focusedOnText)
        {
            FocusOnText = focusedOnText;
            State.Start(this);
        }

        public void Stop()
        {
            State.Stop();
            InitializeState();
        }

        public void Pause()
        {
            State.Pause(this);
        }

        protected virtual void OnStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnMinuteElapsed()
        {
            MinuteElapsed?.Invoke(this, EventArgs.Empty);
        }
    }
}