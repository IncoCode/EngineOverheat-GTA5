#region Using

using System;
using EngineOverheat.Model;
using GTA;

#endregion

namespace EngineOverheat
{
    public class EngineOverheatTick : Script
    {
        private readonly MySettings _settings;
        private readonly GUI _gui;

        public EngineOverheatTick()
        {
            this.Tick += this.EngineOverheatTick_Tick;
            this._settings = MySettings.Instance;
            this._gui = new GUI( this._settings );
        }

        private void UpdEngine()
        {
            Engine engine = EngineOverheat.Engine;
            if ( engine == null || !engine.Broken )
            {
                return;
            }
            engine.Vehicle.EngineRunning = false;
        }

        private void UpdGui()
        {
            Engine engine = EngineOverheat.Engine;
            float? engineHealth = EngineOverheat.EngineHealth;
            if ( engine != null && this._settings.ShowTempGauge )
            {
                this._gui.DrawTempGauge( engine.Temperature );
            }
            if ( engineHealth.HasValue && this._settings.ShowEngineHealthGauge )
            {
                this._gui.DrawEngineHealthGauge( engineHealth.Value );
            }
        }

        private void EngineOverheatTick_Tick( object sender, EventArgs e )
        {
            this.UpdEngine();
            this.UpdGui();
        }
    }
}