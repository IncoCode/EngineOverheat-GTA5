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
                //float rpm = veh.CurrentRPM;
                float rpm = veh.Acceleration;

                if ( veh.EngineRunning )
                {
                    var val = 0.1f * rpm;
                    engine.Temperature += val == 0 ? 0.0006f : val;
                }

                if ( engine.Temperature > 30 || !veh.EngineRunning )
                {
                    engine.Temperature -= 0.025f * ( 0.25f + ( !veh.EngineRunning ? 1.35f : 0 ) );
                    //var val = 0.020f * ( 1 - rpm );
                    //engine.Temperature -= val == 0 ? 0.002f : val;
                }
                if ( veh.EngineHealth > 400 && engine.Broken )
                {
                    engine.Broken = false;
                }

                if ( engine.Damage > 0 || veh.EngineHealth < 1000 )
                {
                    float val = ( ( veh.EngineRunning ? engine.Temperature : 30 ) / 100 + rpm ) * ( 0.25f + ( engine.Broken ? 1.20f : 0 ) );
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
                        float val = ( engine.Temperature / 100 + rpm ) * 0.5f;
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
                           vehP.EngineHealth + ", RPM = " + vehP.Acceleration;
                UI.ShowSubtitle( s );
            }
        }
    }
}