#region Using

using GTA;

#endregion

namespace EngineOverheat.Model
{
    public class Engine
    {
        private float _temperature;
        private float _damage;
        private readonly Vehicle _vehicle;

        #region Fields

        public float Temperature
        {
            get { return this._temperature; }
            set
            {
                if ( value > 100 )
                {
                    value = 100;
                }
                else if ( value < 0 )
                {
                    value = 0;
                }
                this._temperature = value;
            }
        }

        public float Damage
        {
            get { return this._damage; }
            set
            {
                if ( value > 1000 )
                {
                    value = 1000;
                }
                else if ( value < 0 )
                {
                    value = 0;
                }
                this._damage = value;
            }
        }

        public bool Broken { get; set; }

        public Vehicle Vehicle => this._vehicle;

        #endregion

        public Engine( float temperature, Vehicle vehicle )
        {
            this._temperature = temperature;
            this._vehicle = vehicle;
            this.Broken = false;
        }

        public Engine( Vehicle vehicle )
            : this( 0, vehicle )
        {
        }
    }
}