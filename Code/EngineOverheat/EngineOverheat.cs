#region Using

using System;
using System.Windows.Forms;
using EngineOverheat.Model;
using GTA;

#endregion

namespace EngineOverheat
{
    public class EngineOverheat : Script
    {
        private readonly EngineController _engineController;

        public static Engine Engine;

        public EngineOverheat()
        {
            this.Interval = 100;
            this.Tick += this.EngineOverheat_Tick;
            this.KeyDown += this.EngineOverheat_KeyDown;

            this._engineController = new EngineController();
        }

        private void EngineOverheat_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.I )
            {
                this._engineController.EngineForCurrentVehicle().Temperature = 100;
            }
            else if ( e.KeyCode == Keys.K )
            {
                this._engineController.EngineForCurrentVehicle().Temperature = 50;
            }
        }

        private void EngineOverheat_Tick( object sender, EventArgs e )
        {
            this._engineController.Tick();
            Engine = this._engineController.EngineForCurrentVehicle();
        }
    }
}