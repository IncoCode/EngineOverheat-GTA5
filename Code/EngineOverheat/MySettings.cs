#region Using

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

        #endregion

        public MySettings()
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
        }
    }
}