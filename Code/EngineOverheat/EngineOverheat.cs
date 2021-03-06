﻿#region Using

using System;
using System.Windows.Forms;
using EngineOverheat.Controller;
using EngineOverheat.Model;
using GTA;
using GTA.Native;

#endregion

namespace EngineOverheat
{
    public class EngineOverheat : Script
    {
        private readonly EngineController _engineController = EngineController.Instance;
        private readonly MechanicController _mechanicController = MechanicController.Instance;
        private readonly TaskSequenceEventController _taskSequenceEventController = TaskSequenceEventController.Instance;
        private readonly MySettings _settings = MySettings.Instance;

        public static Engine Engine;
        public static float? EngineHealth = 0;

        public EngineOverheat()
        {
            this.Interval = 100;
            this.Tick += this.EngineOverheat_Tick;
            this.KeyDown += this.EngineOverheat_KeyDown;
        }

        protected override void Dispose(bool A_0)
        {
            if (A_0)
            {
                this._engineController.Dispose();
                this._mechanicController.Dispose();
                this._taskSequenceEventController.Dispose();
            }
        }

        private void EngineOverheat_KeyDown( object sender, KeyEventArgs e )
        {
#if DEBUG
            if ( e.KeyCode == Keys.I )
            {
                //this._engineController.EngineForCurrentVehicle().Temperature = 100;
                this._mechanicController._mechanicPed.Kill();
            }
            else if ( e.KeyCode == Keys.K )
            {
                this._engineController.EngineForCurrentVehicle().Temperature = 50;
            }
#endif
            if ( e.KeyCode == this._settings.CallMechanicKey )
            {
                this.CallMechanic();
            }
        }

        private void EngineOverheat_Tick( object sender, EventArgs e )
        {
            Player player = Game.Player;
            this._engineController.Tick();
            this._mechanicController.Tick();
            Engine = this._engineController.EngineForCurrentVehicle();
            EngineHealth = player.Character.CurrentVehicle?.EngineHealth;

            this._taskSequenceEventController.Update();
            if ( Engine != null )
            {
                Function.Call( Hash._SET_DECORATOR_FLOAT, Engine.Vehicle, "MethInPossession",
                    1 - Engine.Temperature / 100f );
            }
        }

        private void CallMechanic()
        {
            var currentVehicle = Game.Player.Character.CurrentVehicle;
            if ( currentVehicle == null )
            {
                return;
            }
            this._mechanicController.CallMechanic( currentVehicle, this._engineController.EngineForCurrentVehicle() );
        }
    }
}