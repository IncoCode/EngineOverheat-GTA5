#region Using

using System;

#endregion

namespace EngineOverheat.Model
{
    internal class EngineData
    {
        public Engine Engine { get; set; }
        public DateTime LastUsage { get; set; }

        public EngineData( Engine engine, DateTime lastUsage )
        {
            this.Engine = engine;
            this.LastUsage = lastUsage;
        }

        public void UpdateUsage()
        {
            this.LastUsage = DateTime.Now;
        }
    }
}