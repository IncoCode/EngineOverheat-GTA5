#region Using

using System.Drawing;
using GTA.Native;

#endregion

namespace EngineOverheat
{
    internal static class GUI
    {
        public static float GaugePosX = 0.125f;
        public static float GaugePosY = 0.78f;
        public static float GaugeWidth = 0.15f;
        public static float GaugeHeight = 0.01f;

        public static Color BackgroundColor = Color.FromArgb( 255, 255, 255 );

        private static float GetX( float width )
        {
            float maxX = GaugePosX * 2;
            return ( maxX / 2f - GaugeWidth / 2f ) + ( width / 2 );
        }

        private static Color ColorForPercent( float percent )
        {
            return percent < 0.5
                ? Color.FromArgb( (int)( 255 * percent * 2 ), 255, 0 )
                : Color.FromArgb( 255, (int)( 255 - 255 * ( ( percent - 0.5 ) * 2 ) ), 0 );
        }

        public static void DrawTempGauge( float temperature )
        {
            float width = GaugeWidth / 100 * temperature;

            // draws background gauge
            Function.Call( Hash.DRAW_RECT, GaugePosX, GaugePosY, GaugeWidth, GaugeHeight, BackgroundColor.R,
                BackgroundColor.G, BackgroundColor.B, 100 );

            Color color = ColorForPercent( temperature / 100 );
            // draws temperature gauge
            Function.Call( Hash.DRAW_RECT, GetX( width ), GaugePosY, width, GaugeHeight, color.R, color.G, color.B, 100 );
        }
    }
}