namespace EngineOverheat.Model
{
    internal class VehicleSetting
    {
        public float IncTempModifier { get; set; }
        public float DecTempModifier { get; set; }
        public string Name { get; set; }

        public VehicleSetting( float incTempModifier, float decTempModifier, string name )
        {
            this.IncTempModifier = incTempModifier;
            this.DecTempModifier = decTempModifier;
            this.Name = name;
        }
    }
}