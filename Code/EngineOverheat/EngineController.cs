#region Using

using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;
using Ini;

#endregion

namespace EngineOverheat
{
    internal class EngineController
    {
        private readonly EngineCollection _engineCollection;
        private readonly IniFile _vehicleSettings;
        private readonly Dictionary<string, VehicleSetting> _vehicleModifiers;

        private const float IncTempModifier = 0.1f;
        private const float DecTempModifier = 0.25f;

        public EngineController( int maxSize = 10 )
        {
            this._engineCollection = new EngineCollection( maxSize );
            this._vehicleSettings = new IniFile( "scripts\\EngineOverheatVehicle.ini" );
            this._vehicleModifiers = new Dictionary<string, VehicleSetting>();
        }

        public Engine EngineForCurrentVehicle()
        {
            var currVeh = Game.Player.Character.CurrentVehicle;
            return currVeh == null ? null : this._engineCollection.GetEngine( currVeh );
        }

        private VehicleSetting ReadVehicleSettings( VehicleHash vehicle )
        {
            string vehStr = vehicle.ToString();
            if ( !this._vehicleModifiers.ContainsKey( vehStr ) )
            {
                float incTempMod = (float)this._vehicleSettings.Read( "IncTempModifier", vehStr, (double)IncTempModifier );
                float decTempMod = (float)this._vehicleSettings.Read( "DecTempModifier", vehStr, (double)DecTempModifier );
                this._vehicleModifiers.Add( vehStr, new VehicleSetting( incTempMod, decTempMod, vehStr ) );
            }
            return this._vehicleModifiers[ vehStr ];
        }

        public void Tick()
        {
            var player = Game.Player;

            this._engineCollection.GetEngine( player.Character.CurrentVehicle );

            foreach ( KeyValuePair<int, EngineData> kvp in this._engineCollection )
            {
                var veh = new Vehicle( kvp.Key );
                Engine engine = kvp.Value.Engine;
                float acceleration = veh.Acceleration;
                VehicleSetting vehicleSettings = this.ReadVehicleSettings( (VehicleHash)veh.Model.Hash );
                float incTempMod = vehicleSettings.IncTempModifier;
                float decTempMod = vehicleSettings.DecTempModifier;

                if ( veh.EngineRunning )
                {
                    var val = incTempMod * acceleration;
                    engine.Temperature += val == 0 ? 0.0006f : val;
                }

                if ( engine.Temperature > 30 || !veh.EngineRunning )
                {
                    engine.Temperature -= 0.025f * ( decTempMod + ( !veh.EngineRunning ? 1.35f : 0 ) );
                }
                if ( veh.EngineHealth > 400 && engine.Broken )
                {
                    engine.Broken = false;
                }

                if ( engine.Damage > 0 || veh.EngineHealth < 1000 )
                {
                    float val = ( ( veh.EngineRunning ? engine.Temperature : 30 ) / 100 + acceleration ) *
                                ( 0.25f + ( engine.Broken ? 1.20f : 0 ) );
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
                        float val = ( engine.Temperature / 100 + acceleration ) * 0.5f;
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