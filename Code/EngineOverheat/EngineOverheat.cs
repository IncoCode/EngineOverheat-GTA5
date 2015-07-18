#region Using

using System;
using GTA;

#endregion

namespace EngineOverheat
{
    public class EngineOverheat : Script
    {
        private readonly EngineController _engineController;

        public EngineOverheat()
        {
            this.Tick += this.EngineOverheat_Tick;

            this._engineController = new EngineController();
        }

        private void EngineOverheat_Tick( object sender, EventArgs e )
        {
            this._engineController.Tick();
        }
    }
}