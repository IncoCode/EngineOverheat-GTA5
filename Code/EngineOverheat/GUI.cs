#region Using

using System.Drawing;
using GTA.Native;

#endregion

namespace EngineOverheat
{
    internal class GUI
    {
        private readonly MySettings _settings = MySettings.Instance;

        #region Fields

        public Color BackgroundColor = Color.FromArgb( 255, 255, 255 );

        #endregion

        private float GetX( float width )
        {
            float maxX = this._settings.TempGaugePosX * 2;
            return ( maxX / 2f - this._settings.TempGaugeWidth / 2f ) + ( width / 2 );
        }

        private Color ColorForPercent( float percent )
        {
            return percent < 0.5
                ? Color.FromArgb( (int)( 255 * percent * 2 ), 255, 0 )
                : Color.FromArgb( 255, (int)( 255 - 255 * ( ( percent - 0.5 ) * 2 ) ), 0 );
        }

        public void DrawTempGauge( float temperature )
        {
            float width = this._settings.TempGaugeWidth / 100 * temperature;

            // draws background gauge
            Function.Call( Hash.DRAW_RECT, this._settings.TempGaugePosX, this._settings.TempGaugePosY,
                this._settings.TempGaugeWidth,
                this._settings.TempGaugeHeight, this.BackgroundColor.R, this.BackgroundColor.G, this.BackgroundColor.B,
                100 );

            Color color = this.ColorForPercent( temperature / 100 );
            // draws temperature gauge
            Function.Call( Hash.DRAW_RECT, this.GetX( width ), this._settings.TempGaugePosY, width,
                this._settings.TempGaugeHeight, color.R, color.G, color.B, 100 );
        }

        public void DrawEngineHealthGauge( float health )
        {
            if ( health > 1000 )
            {
                health = 1000;
            }
            else if ( health < 0 )
            {
                health = 0;
            }
            float width = this._settings.EngineHealthGaugeWidth / 1000 * health;

            // draws background gauge
            Function.Call( Hash.DRAW_RECT, this._settings.EngineHealthGaugePosX, this._settings.EngineHealthGaugePosY,
                this._settings.EngineHealthGaugeWidth, this._settings.EngineHealthGaugeHeight, this.BackgroundColor.R,
                this.BackgroundColor.G, this.BackgroundColor.B, 100 );

            Color color = this.ColorForPercent( ( 1000 - health ) / 1000 );
            // draws engine health gauge
            Function.Call( Hash.DRAW_RECT, this.GetX( width ), this._settings.EngineHealthGaugePosY, width,
                this._settings.EngineHealthGaugeHeight, color.R, color.G, color.B, 100 );
        }
    }
}