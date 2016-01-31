#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using EngineOverheat.Model;
using GTA;

#endregion

namespace EngineOverheat
{
    public class TaskSequenceEventController
    {
        #region Fields

        public static TaskSequenceEventController Instance
            => _instance ?? ( _instance = new TaskSequenceEventController() );

        #endregion

        private readonly List<TaskSequenceData> _taskSequencesData;
        private static TaskSequenceEventController _instance;

        private TaskSequenceEventController()
        {
            this._taskSequencesData = new List<TaskSequenceData>();
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

        public void Update()
        {
            this._taskSequencesData.ForEach( taskSequence => taskSequence.Update() );
        }
    }
}