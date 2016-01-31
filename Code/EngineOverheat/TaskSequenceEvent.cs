#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using EngineOverheat.Model;
using GTA;

#endregion

namespace EngineOverheat
{

    public class TaskSequenceEvent
    {
        #region Fields

        public static TaskSequenceEvent Instance => _instance ?? ( _instance = new TaskSequenceEvent() );

        #endregion

        private readonly List<TaskSequenceData> _taskSequencesData;
        private static TaskSequenceEvent _instance;

        private TaskSequenceEvent()
        {
            this._taskSequencesData = new List<TaskSequenceData>();
        }

        public void Subscribe( int sequenceIndex, Ped ped, TaskSequence taskSequence, Action action )
        {
            TaskSequenceData taskSequenceData = this._taskSequencesData
                .FirstOrDefault( tsd => tsd.Ped == ped && tsd.TaskSequence == taskSequence );
            if ( taskSequenceData == null )
            {
                taskSequenceData = new TaskSequenceData( ped, taskSequence );
                this._taskSequencesData.Add( taskSequenceData );
            }
            taskSequenceData.FireWhenSequenceIndex( sequenceIndex, action );
        }

        public void Update()
        {
            this._taskSequencesData.ForEach( taskSequence => taskSequence.Update() );
        }
    }
}