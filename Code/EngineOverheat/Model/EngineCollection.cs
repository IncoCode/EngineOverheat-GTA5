#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GTA;

#endregion

namespace EngineOverheat.Model
{
    internal class EngineCollection : IEnumerable
    {
        private Dictionary<int, EngineData> _vehicles; // key - vehicle handle
        private readonly int _maxSize;

        public EngineCollection( int maxSize = 10 )
        {
            this._maxSize = maxSize;
            this._vehicles = new Dictionary<int, EngineData>();
        }

        private void Sort()
        {
            this._vehicles = this._vehicles.OrderByDescending( v => v.Value.LastUsage )
                .Where( v => new Vehicle( v.Key ).Exists() )
                .ToDictionary( v => v.Key, v => v.Value );
        }

        private bool IsEnable( Vehicle vehicle )
        {
            GTA.Model model = vehicle.Model;
            return vehicle.GetPedOnSeat( VehicleSeat.Driver ) == Game.Player.Character
                   && ( model.IsBike || model.IsCar || model.IsQuadbike );
        }

        public Engine GetEngine( Vehicle vehicle )
        {
            if ( vehicle == null || !this.IsEnable( vehicle ) )
            {
                return null;
            }

            EngineData engineData;
            if ( this._vehicles.ContainsKey( vehicle.Handle ) )
            {
                engineData = this._vehicles[ vehicle.Handle ];
                engineData.UpdateUsage();
            }
            else
            {
                if ( this._vehicles.Count > this._maxSize )
                {
                    this._vehicles.Remove( this._vehicles.Last().Key );
                }

                engineData = new EngineData( new Engine( vehicle.EngineRunning ? 30 : 0, vehicle ), DateTime.Now );
                this._vehicles.Add( vehicle.Handle, engineData );
            }
            this.Sort();
            return engineData.Engine;
        }

        public void Clear()
        {
            this._vehicles.Clear();
        }

        #region Members

        public IEnumerator GetEnumerator()
        {
            return this._vehicles.GetEnumerator();
        }

        internal IEnumerable<KeyValuePair<int, EngineData>> ToList()
        {
            return this._vehicles.ToList();
        }

        #endregion
    }
}