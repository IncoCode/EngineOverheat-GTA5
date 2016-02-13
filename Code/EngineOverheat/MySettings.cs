#region Using

using System.Windows.Forms;
using Ini;

#endregion

namespace EngineOverheat
{
    internal class MySettings
    {
        private readonly IniFile _settings;
        private static MySettings _instance;

        #region Fields

        public static MySettings Instance => _instance ?? ( _instance = new MySettings() );

        public float TempGaugePosX { get; private set; }
        public float TempGaugePosY { get; private set; }
        public float TempGaugeWidth { get; private set; }
        public float TempGaugeHeight { get; private set; }

        public float EngineHealthGaugePosX { get; private set; }
        public float EngineHealthGaugePosY { get; private set; }
        public float EngineHealthGaugeWidth { get; private set; }
        public float EngineHealthGaugeHeight { get; private set; }

        public bool ShowEngineHealthGauge { get; private set; }
        public bool ShowTempGauge { get; private set; }

        public Keys CallMechanicKey { get; private set; }
        public int CallMechanicPayment { get; private set; }

        public float FactorIncTemp { get; private set; }
        public float FactorDecTemp { get; private set; }
        public float FactorDecEngDamage { get; private set; }
        public float FactorIncEngDamage { get; private set; }

        #endregion

        private MySettings()
        {
            this._settings = new IniFile( "scripts\\EngineOverheat.ini" );
            this.Load();
        }

        private void Load()
        {
            this.TempGaugePosX = this._settings.Read( "TempGaugePosX", "GUI", 0.125f );
            this.TempGaugePosY = this._settings.Read( "TempGaugePosY", "GUI", 0.78f );
            this.TempGaugeWidth = this._settings.Read( "TempGaugeWidth", "GUI", 0.15f );
            this.TempGaugeHeight = this._settings.Read( "TempGaugeHeight", "GUI", 0.01f );

            this.EngineHealthGaugePosX = this._settings.Read( "EngineHealthGaugePosX", "GUI", 0.125f );
            this.EngineHealthGaugePosY = this._settings.Read( "EngineHealthGaugePosY", "GUI", 0.805f );
            this.EngineHealthGaugeWidth = this._settings.Read( "EngineHealthGaugeWidth", "GUI", 0.15f );
            this.EngineHealthGaugeHeight = this._settings.Read( "EngineHealthGaugeHeight", "GUI", 0.01f );

            this.ShowTempGauge = this._settings.Read( "ShowTempGauge", "GUI", true );
            this.ShowEngineHealthGauge = this._settings.Read( "ShowEngineHealthGauge", "GUI", true );

            this.CallMechanicKey = Helper.StringToKey( this._settings.Read( "CallMechanicKey", "Settings", "L" ), Keys.L );
            this.CallMechanicPayment = this._settings.Read( "CallMechanicPayment", "Settings", 10000 );

            this.FactorIncTemp = this._settings.Read( "FactorIncTemp", "Settings", 1f );
            this.FactorDecTemp = this._settings.Read( "FactorDecTemp", "Settings", 1f );
            this.FactorIncEngDamage = this._settings.Read( "FactorIncEngDamage", "Settings", 1f );
            this.FactorDecEngDamage = this._settings.Read( "FactorDecEngDamage", "Settings", 1f );
        }
    }
}