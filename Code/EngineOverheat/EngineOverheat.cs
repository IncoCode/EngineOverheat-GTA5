#region Using

using System;
using System.Windows.Forms;
using GTA;

#endregion

namespace EngineOverheat
{
    public class EngineOverheat : Script
    {
        private readonly EngineController _engineController;
        private readonly GUI _gui;
        private readonly MySettings _settings;

        public EngineOverheat()
        {
            this.Tick += this.EngineOverheat_Tick;
            this.KeyDown += this.EngineOverheat_KeyDown;

            this._engineController = new EngineController();
            this._settings = new MySettings();
            this._gui = new GUI( this._settings );
        }

        private void EngineOverheat_KeyDown( object sender, KeyEventArgs e )
        {
            //if ( e.KeyCode == Keys.I )
            //{
            //    this._engineController.EngineForCurrentVehicle().Temperature = 100;
            //}
            //else if ( e.KeyCode == Keys.K )
            //{
            //    this._engineController.EngineForCurrentVehicle().Temperature = 50;
            //}
        }

        private void EngineOverheat_Tick( object sender, EventArgs e )
        {
            this._engineController.Tick();
            var engine = this._engineController.EngineForCurrentVehicle();
            if ( engine == null )
            {
                return;
            }
            this._gui.DrawTempGauge( engine.Temperature );
        }
    }
}