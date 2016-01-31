#region Using

using System;
using System.Collections.Generic;
using GTA;

#endregion

namespace EngineOverheat.Model
{
    public class TaskSequenceData
    {
        public bool IsActive;
        public Ped Ped { get; private set; }
        public TaskSequence TaskSequence { get; private set; }

        private readonly Dictionary<int, List<Action>> _actions;
        private readonly Dictionary<int, List<Action>> _actionsEveryTick;
        private int _lastFiredIndex = -1;

        public TaskSequenceData( Ped ped, TaskSequence taskSequence, bool isActive = true )
        {
            this.TaskSequence = taskSequence;
            this.Ped = ped;
            this.IsActive = isActive;

            this._actions = new Dictionary<int, List<Action>>();
            this._actionsEveryTick = new Dictionary<int, List<Action>>();
        }

        private void AddAction( Dictionary<int, List<Action>> actions, int sequenceIndex, Action action )
        {
            List<Action> actionsList;
            if ( !actions.TryGetValue( sequenceIndex, out actionsList ) )
            {
                actionsList = new List<Action>();
                actions.Add( sequenceIndex, actionsList );
            }
            actionsList.Add( action );
        }

        private void FireActions( int sequenceIndex, Dictionary<int, List<Action>> actions )
        {
            List<Action> actionsList;
            if ( !actions.TryGetValue( sequenceIndex, out actionsList ) )
            {
                return;
            }
            actionsList.ForEach( action => action.Invoke() );
        }

        public void FireWhenSequenceIndex( int sequenceIndex, Action action )
        {
            this.AddAction( this._actions, sequenceIndex, action );
        }

        public void FireWhenSequenceIndexEveryTick( int sequenceIndex, Action action )
        {
            this.AddAction( this._actions, sequenceIndex, action );
        }

        public void Update()
        {
            if ( !this.IsActive )
            {
                return;
            }
            var sequenceIndex = this.Ped.TaskSequenceProgress;
            this.FireActions( sequenceIndex, this._actionsEveryTick );

            if ( this._lastFiredIndex == sequenceIndex )
            {
                return;
            }
            this._lastFiredIndex = sequenceIndex;
            this.FireActions( sequenceIndex, this._actions );
        }
    }
}