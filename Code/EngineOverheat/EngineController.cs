#region Using

using System.Collections.Generic;
using GTA;

#endregion

namespace EngineOverheat
{
    internal class EngineController
    {
        private readonly EngineCollection _engineCollection;

        public EngineController( int maxSize = 10 )
        {
            this._engineCollection = new EngineCollection( maxSize );
        }

        public Engine EngineForCurrentVehicle()
        {
            var currVeh = Game.Player.Character.CurrentVehicle;
            return currVeh == null ? null : this._engineCollection.GetEngine( currVeh );
        }

        public void Tick()
        {
            var player = Game.Player;

            this._engineCollection.GetEngine( player.Character.CurrentVehicle );

            foreach ( KeyValuePair<int, EngineData> kvp in this._engineCollection )
            {
                var veh = new Vehicle( kvp.Key );
                Engine engine = kvp.Value.Engine;
                float rpm = veh.CurrentRPM;

                if ( veh.EngineRunning )
                {
                    engine.Temperature += 0.03f * rpm;
                }

                if ( engine.Temperature > 50 || !veh.EngineRunning )
                {
                    engine.Temperature -= 0.03f * 0.6f;
                }
                if ( veh.EngineHealth > 400 && engine.Broken )
                {
                    engine.Broken = false;
                }

                if ( engine.Damage > 0 || veh.EngineHealth < 1000 )
                {
                    float val = ( ( veh.EngineRunning ? engine.Temperature : 50 ) / 100 + rpm ) * 0.07f;
                    engine.Damage -= val;
                    if ( veh.EngineHealth < 1000 )
                    {
                        if ( veh.EngineHealth < 0 )
                        {
                            veh.EngineHealth = 0;
                        }
                        veh.EngineHealth += val;
                    }
                }
                if ( engine.Temperature >= 70 )
                {
                    if ( !engine.Broken && veh.EngineHealth > 0f )
                    {
                        float val = ( engine.Temperature / 100 + rpm ) * 0.14f;
                        veh.EngineHealth -= val;
                        engine.Damage += val;
                    }
                    if ( !engine.Broken && veh.EngineHealth <= 0 )
                    {
                        veh.EngineHealth = -1;
                        engine.Broken = true;
                    }
                }
                if ( engine.Broken && veh.EngineRunning )
                {
                    veh.EngineRunning = false;
                }
            }

            // debug info
            Vehicle vehP = player.Character.IsInVehicle() ? player.Character.CurrentVehicle : player.LastVehicle;
            if ( vehP != null )
            {
                var dt = this._engineCollection.GetEngine( vehP );
                string s = "Temperature = " + dt.Temperature + ", Damage = " + dt.Damage + ", Engine = " +
                           vehP.EngineHealth + ", RPM = " + vehP.CurrentRPM;
                UI.ShowSubtitle( s );
            }
        }
    }
}