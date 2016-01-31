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
        private int _lastFiredIndex = -1;

        public TaskSequenceData( Ped ped, TaskSequence taskSequence, bool isActive = true )
        {
            this.TaskSequence = taskSequence;
            this.Ped = ped;
            this.IsActive = isActive;

            this._actions = new Dictionary<int, List<Action>>();
        }

        public void FireWhenSequenceIndex( int sequenceIndex, Action action )
        {
            List<Action> actionsList;
            if ( !this._actions.TryGetValue( sequenceIndex, out actionsList ) )
            {
                actionsList = new List<Action>();
                this._actions.Add( sequenceIndex, actionsList );
            }
            actionsList.Add( action );
        }

        public void Update()
        {
            var sequenceIndex = this.Ped.TaskSequenceProgress;
            if ( !this.IsActive || this._lastFiredIndex == sequenceIndex )
            {
                return;
            }

            this._lastFiredIndex = sequenceIndex;
            List<Action> actions;
            if ( !this._actions.TryGetValue( sequenceIndex, out actions ) )
            {
                return;
            }
            actions.ForEach( action => action.Invoke() );
        }
    }
}