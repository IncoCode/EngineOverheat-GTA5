#region Using

using System.Drawing;
using GTA.Native;

#endregion

namespace EngineOverheat
{
    internal class GUI
    {
        private readonly MySettings _settings;

        #region Fields

        public Color BackgroundColor = Color.FromArgb( 255, 255, 255 );

        #endregion

        public GUI( MySettings settings )
        {
            this._settings = settings;
        }

        private float GetX( float width )
        {
            float maxX = this._settings.GaugePosX * 2;
            return ( maxX / 2f - this._settings.GaugeWidth / 2f ) + ( width / 2 );
        }

        private Color ColorForPercent( float percent )
        {
            return percent < 0.5
                ? Color.FromArgb( (int)( 255 * percent * 2 ), 255, 0 )
                : Color.FromArgb( 255, (int)( 255 - 255 * ( ( percent - 0.5 ) * 2 ) ), 0 );
        }

        public void DrawTempGauge( float temperature )
        {
            float width = this._settings.GaugeWidth / 100 * temperature;

            // draws background gauge
            Function.Call( Hash.DRAW_RECT, this._settings.GaugePosX, this._settings.GaugePosY, this._settings.GaugeWidth,
                this._settings.GaugeHeight, this.BackgroundColor.R, this.BackgroundColor.G, this.BackgroundColor.B, 100 );

            Color color = this.ColorForPercent( temperature / 100 );
            // draws temperature gauge
            Function.Call( Hash.DRAW_RECT, this.GetX( width ), this._settings.GaugePosY, width,
                this._settings.GaugeHeight, color.R, color.G, color.B, 100 );
        }
    }
}