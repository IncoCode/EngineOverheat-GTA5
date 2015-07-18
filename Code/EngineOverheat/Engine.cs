namespace EngineOverheat
{
    internal class Engine
    {
        private float _temperature = 0;
        private float _damage = 0;

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

        #endregion

        public Engine( float temperature )
        {
            this._temperature = temperature;
            this.Broken = false;
        }

        public Engine()
            : this( 0 )
        {
        }
    }
}