#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using EngineOverheat.Model;
using GTA;

#endregion

namespace EngineOverheat.Controller
{
    internal class TaskSequenceEventController
    {
        #region Fields

        public static TaskSequenceEventController Instance
            => _instance ?? ( _instance = new TaskSequenceEventController() );

        #endregion

        private readonly List<TaskSequenceData> _taskSequencesData;
        private static TaskSequenceEventController _instance;
        private bool _isDisposing = false;

        private TaskSequenceEventController()
        {
            this._taskSequencesData = new List<TaskSequenceData>();
        }

        public void Dispose()
        {
            this._isDisposing = true;
            this._taskSequencesData.Clear();
        }

        private TaskSequenceData GetTaskSequenceData( Ped ped, TaskSequence taskSequence )
        {
            return this._taskSequencesData
                .FirstOrDefault( tsd => tsd.Ped == ped && tsd.TaskSequence == taskSequence );
        }

        public void Subscribe( int sequenceIndex, Ped ped, TaskSequence taskSequence, Action action,
            bool everyTick = false )
        {
            TaskSequenceData taskSequenceData = this.GetTaskSequenceData( ped, taskSequence );
            if ( taskSequenceData == null )
            {
                taskSequenceData = new TaskSequenceData( ped, taskSequence );
                this._taskSequencesData.Add( taskSequenceData );
            }
            if ( everyTick )
            {
                taskSequenceData.FireWhenSequenceIndexEveryTick( sequenceIndex, action );
            }
            else
            {
                taskSequenceData.FireWhenSequenceIndex( sequenceIndex, action );
            }
        }

        public void UnsubscribeAll( Ped ped, TaskSequence taskSequence )
        {
            TaskSequenceData taskSequenceData = this.GetTaskSequenceData( ped, taskSequence );
            if ( taskSequenceData == null )
            {
                return;
            }
            taskSequenceData.IsActive = false;
            this._taskSequencesData.Remove( taskSequenceData );
        }

        public void UnsubscribeAll(Ped ped)
        {
            this._taskSequencesData.RemoveAll(tsd => tsd.Ped == ped);
        }

        public void Update()
        {
            if (this._isDisposing)
            {
                return;
            }

            this._taskSequencesData.ForEach( taskSequence => taskSequence.Update() );
        }
    }
}