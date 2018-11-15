#region Using

using GTA;
using GTA.Math;
using GTA.Native;
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

        public static void GetClosestParkingPositionWithHeading(Vector3 position, out Vector3 pos, out float heading)
        {
            pos = new Vector3();
            heading = 0;

            for (int i = 0; i < 40; i++)
            {
                OutputArgument outPos = new OutputArgument();
                OutputArgument outHeading = new OutputArgument();
                OutputArgument val = new OutputArgument();

                Function.Call(
                    Hash.GET_NTH_CLOSEST_VEHICLE_NODE_WITH_HEADING,
                    position.X,
                    position.Y,
                    position.Z,
                    i,
                    outPos,
                    outHeading,
                    val,
                    1, 
                    0x40400000, 
                    0
                );

                heading = outHeading.GetResult<float>();
                pos = outPos.GetResult<Vector3>();

                bool isPointBusy = Function.Call<bool>(
                    Hash.IS_POINT_OBSCURED_BY_A_MISSION_ENTITY,
                    pos.X,
                    pos.Y,
                    pos.Z,
                    5.0f, 
                    5.0f, 
                    5.0f, 
                    0
                );
                if (!isPointBusy) {
                    return;
                }
            }
        }
    }
}