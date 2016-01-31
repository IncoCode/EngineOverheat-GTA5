#region Using

using System;
using System.Windows.Forms;
using EngineOverheat.Model;
using GTA;
using iFruitAddon;

#endregion

namespace EngineOverheat
{
    public class EngineOverheat : Script
    {
        private readonly EngineController _engineController;
        private readonly MechanicController _mechanicController;
        private readonly TaskSequenceEventController _taskSequenceEventController;

        public static Engine Engine;
        public static float? EngineHealth = 0;

        private CustomiFruit _iFruit;

        public EngineOverheat()
        {
            this.Interval = 100;
            this.Tick += this.EngineOverheat_Tick;
            this.KeyDown += this.EngineOverheat_KeyDown;

            this._engineController = new EngineController();
            this._taskSequenceEventController = TaskSequenceEventController.Instance;
            this._mechanicController = new MechanicController( this._taskSequenceEventController );

            this.InitMobile();
        }

        private void EngineOverheat_KeyDown( object sender, KeyEventArgs e )
        {
#if DEBUG
            if ( e.KeyCode == Keys.I )
            {
                this._engineController.EngineForCurrentVehicle().Temperature = 100;
            }
            else if ( e.KeyCode == Keys.K )
            {
                this._engineController.EngineForCurrentVehicle().Temperature = 50;
            }
            else if ( e.KeyCode == Keys.L )
            {
                this.Contact_Answered( null );
            }
#endif
        }

        private void EngineOverheat_Tick( object sender, EventArgs e )
        {
            Player player = Game.Player;
            this._engineController.Tick();
            Engine = this._engineController.EngineForCurrentVehicle();
            EngineHealth = player.Character.CurrentVehicle?.EngineHealth;

            this._iFruit.Update();
            this._taskSequenceEventController.Update();
        }

        private void InitMobile()
        {
            this._iFruit = new CustomiFruit();
            var contact = new iFruitContact( "Call Mechanic", 24 );
            contact.Answered += this.Contact_Answered;
            contact.DialTimeout = 0;
            contact.Active = true;
            this._iFruit.Contacts.Add( contact );
        }

        private void Contact_Answered( iFruitContact contact )
        {
            UI.Notify( "Answered" );
            this._mechanicController.CallMechanic( Game.Player.Character.CurrentVehicle );
            //contact.EndCall();
        }
    }
}