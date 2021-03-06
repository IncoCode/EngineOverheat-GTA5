﻿#region Using

using System;
using System.Collections.Generic;
using EngineOverheat.Model;
using GTA;
using GTA.Native;
using Ini;

#endregion

namespace EngineOverheat.Controller
{
    internal class EngineController
    {
        #region Fields

        public static EngineController Instance => _instance ?? ( _instance = new EngineController() );

        #endregion

        private readonly EngineCollection _engineCollection;
        private readonly IniFile _vehicleSettings;
        private readonly Dictionary<string, VehicleSetting> _vehicleModifiers;
        private readonly MySettings _settings = MySettings.Instance;
        private bool _isDisposing = false;

        private static EngineController _instance;

        private const float IncTempModifier = 0.07f;
        private const float DecTempModifier = 0.35f;

        private EngineController( int maxSize = 10 )
        {
            this._engineCollection = new EngineCollection( maxSize );
            this._vehicleSettings = new IniFile( "scripts\\EngineOverheatVehicle.ini" );
            this._vehicleModifiers = new Dictionary<string, VehicleSetting>();
        }

        public void Dispose()
        {
            this._isDisposing = true;
            this._engineCollection.Clear();
        }

        public Engine EngineForCurrentVehicle()
        {
            var currentVehicle = Game.Player.Character.CurrentVehicle;
            return currentVehicle == null ? null : this._engineCollection.GetEngine( currentVehicle );
        }

        private VehicleSetting ReadVehicleSettings( VehicleHash vehicle )
        {
            string vehicleStr = vehicle.ToString();
            if ( !this._vehicleModifiers.ContainsKey( vehicleStr ) )
            {
                float incTempMod = this._vehicleSettings.Read( "IncTempModifier", vehicleStr, IncTempModifier );
                float decTempMod = this._vehicleSettings.Read( "DecTempModifier", vehicleStr, DecTempModifier );
                this._vehicleModifiers.Add( vehicleStr, new VehicleSetting( incTempMod, decTempMod, vehicleStr ) );
            }
            return this._vehicleModifiers[ vehicleStr ];
        }

        public void Tick()
        {
            if (this._isDisposing)
            {
                return;
            }

            var player = Game.Player;

            foreach ( KeyValuePair<int, EngineData> kvp in this._engineCollection.ToList() )
            {
                var veh = new Vehicle( kvp.Key );
                Engine engine = kvp.Value.Engine;
                float acceleration = Math.Abs( veh.Acceleration );
                VehicleSetting vehicleSettings = this.ReadVehicleSettings( (VehicleHash)veh.Model.Hash );
                float incTempMod = vehicleSettings.IncTempModifier;
                float decTempMod = vehicleSettings.DecTempModifier;

                if ( veh.EngineRunning )
                {
                    float val = incTempMod * acceleration * this._settings.FactorIncTemp;
                    engine.Temperature += val == 0 ? 0.0006f : val;
                }

                if ( engine.Temperature > 30 || !veh.EngineRunning )
                {
                    float decreaseTempValue = 0.025f * ( decTempMod + ( !veh.EngineRunning ? 1.35f : 0 ) )
                                              * this._settings.FactorDecTemp;
                    if ( veh.Speed > 40 )
                    {
                        decreaseTempValue += ( veh.Speed - 40 ) / 1000;
                    }
                    engine.Temperature -= decreaseTempValue;
                }
                if ( veh.EngineHealth > 400 && engine.Broken )
                {
                    engine.Broken = false;
                }

                if ( engine.Damage > 0 || veh.EngineHealth < 1000 )
                {
                    float val = ( ( veh.EngineRunning ? engine.Temperature : 30 ) / 100 + acceleration ) *
                                ( 0.25f + ( engine.Broken ? 1.20f : 0 ) ) * this._settings.FactorDecEngDamage;
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
                        float val = ( engine.Temperature / 100 + acceleration ) * 0.5f *
                                    this._settings.FactorIncEngDamage;
                        veh.EngineHealth -= val;
                        engine.Damage += val;
                    }
                    if ( !engine.Broken && veh.EngineHealth <= 0 )
                    {
                        veh.EngineHealth = -1;
                        engine.Broken = true;
                    }
                }

                if (this._isDisposing)
                {
                    return;
                }
            }

            // debug info
#if DEBUG
            Vehicle vehP = player.Character.IsInVehicle() ? player.Character.CurrentVehicle : player.LastVehicle;
            if ( vehP != null )
            {
                var dt = this._engineCollection.GetEngine( vehP );
                string s = "Temperature = " + dt?.Temperature + ", Damage = " + dt?.Damage + ", Engine = " +
                           vehP.EngineHealth + ", RPM = " + vehP.Acceleration + ", Speed = " + vehP.Speed;
                UI.ShowSubtitle( s );
            }
#endif
        }
    }
}