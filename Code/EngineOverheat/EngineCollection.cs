#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GTA;

#endregion

namespace EngineOverheat
{
    internal class EngineData
    {
        public Engine Engine { get; set; }
        public DateTime LastUsage { get; set; }

        public EngineData( Engine engine, DateTime lastUsage )
        {
            this.Engine = engine;
            this.LastUsage = lastUsage;
        }

        public void UpdateUsage()
        {
            this.LastUsage = DateTime.Now;
        }
    }

    internal class EngineCollection : IEnumerable
    {
        private Dictionary<int, EngineData> _vehicles;
        private readonly int _maxSize;

        public EngineCollection( int maxSize = 10 )
        {
            this._maxSize = maxSize;
            this._vehicles = new Dictionary<int, EngineData>();
        }

        private void Sort()
        {
            this._vehicles = this._vehicles.OrderByDescending( v => v.Value.LastUsage )
                .ToDictionary( v => v.Key, v => v.Value );
        }

        public Engine GetEngine( Vehicle vehicle )
        {
            if ( vehicle == null )
            {
                return null;
            }
            EngineData engineDt;
            if ( this._vehicles.ContainsKey( vehicle.Handle ) )
            {
                engineDt = this._vehicles[ vehicle.Handle ];
                engineDt.UpdateUsage();
            }
            else
            {
                if ( this._vehicles.Count > this._maxSize )
                {
                    this._vehicles.Remove( this._vehicles.Last().Key );
                }

                engineDt = new EngineData( new Engine( vehicle.EngineRunning ? 30 : 0 ), DateTime.Now );
                this._vehicles.Add( vehicle.Handle, engineDt );
            }
            this.Sort();
            return engineDt.Engine;
        }

        #region Members

        public IEnumerator GetEnumerator()
        {
            return this._vehicles.GetEnumerator();
        }

        #endregion
    }
}