#region Using

using Ini;

#endregion

namespace EngineOverheat
{
    internal class MySettings
    {
        private readonly IniFile _settings;

        #region Fields

        public float GaugePosX { get; private set; }
        public float GaugePosY { get; private set; }
        public float GaugeWidth { get; private set; }
        public float GaugeHeight { get; private set; }

        #endregion

        public MySettings()
        {
            this._settings = new IniFile( "scripts\\EngineOverheat.ini" );
            this.Load();
        }

        private void Load()
        {
            this.GaugePosX = this._settings.Read( "GaugePosX", "GUI", 0.125f );
            this.GaugePosY = this._settings.Read( "GaugePosY", "GUI", 0.78f );
            this.GaugeWidth = this._settings.Read( "GaugeWidth", "GUI", 0.15f );
            this.GaugeHeight = this._settings.Read( "GaugeHeight", "GUI", 0.01f );
        }
    }
}