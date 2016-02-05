#region Using

using System;
using System.Windows.Forms;

#endregion

namespace EngineOverheat
{
    internal static class Helper
    {
        public static Keys StringToKey( string key, Keys defaultValue )
        {
            if ( string.IsNullOrEmpty( key ) || key.Length == 0 )
            {
                return defaultValue;
            }
            Keys resKey = defaultValue;
            try
            {
                resKey = (Keys)Enum.Parse( typeof( Keys ), key.Trim(), true );
            }
            catch
            {
                return resKey;
            }
            return resKey;
        }
    }
}