#region Using

using GTA;
using GTA.Math;
using GTA.Native;

#endregion

namespace EngineOverheat
{
    public class MechanicController
    {
        private Ped _mechanicPed;
        private Vehicle _mechanicVehicle;
        private Blip _mechanicBlip;
        private readonly TaskSequenceEventController _taskSequenceEventController;

        public MechanicController( TaskSequenceEventController taskSequenceEventController )
        {
            this._taskSequenceEventController = taskSequenceEventController;
        }

        public void CallMechanic( Vehicle vehicle )
        {
            vehicle.IsDriveable = false;
            var vehiclePosition = vehicle.Position;
            var spawnPosition = World.GetNextPositionOnStreet( vehiclePosition.Around( 130f ) );

            this._mechanicVehicle = World.CreateVehicle( VehicleHash.Panto, spawnPosition );
            this._mechanicPed = this._mechanicVehicle.CreatePedOnSeat( VehicleSeat.Driver, PedHash.FatWhite01AFM );
            this._mechanicPed.Weapons.Give( WeaponHash.FireExtinguisher, 10000, true, true );

            var openHoodPosition = vehicle.GetOffsetInWorldCoords( new Vector3( 0.35f, 3.0f, 0.0f ) );
            var shootPosition = vehicle.GetOffsetInWorldCoords( new Vector3( 0.35f, 5.0f, 0.0f ) );

            var tasks = new TaskSequence();
            tasks.AddTask.DriveTo( this._mechanicVehicle, vehiclePosition, 20f, 60, (int)DrivingStyle.Rushed );
            tasks.AddTask.RunTo( openHoodPosition );
            tasks.AddTask.TurnTo( vehicle, 1000 );
            tasks.AddTask.Wait( 1000 );
            tasks.AddTask.GoTo( shootPosition );
            tasks.AddTask.ShootAt( vehiclePosition, 15500, FiringPattern.FullAuto );
            tasks.AddTask.GoTo( openHoodPosition );
            tasks.AddTask.TurnTo( vehicle, 1000 );
            tasks.AddTask.Wait( 1000 );
            tasks.AddTask.CruiseWithVehicle( this._mechanicVehicle, 30f, (int)DrivingStyle.Rushed );
            tasks.Close();

            this._taskSequenceEventController.Subscribe( 4, this._mechanicPed, tasks,
                () => this.OpenVehicleHood( vehicle ) );
            this._taskSequenceEventController.Subscribe( 8, this._mechanicPed, tasks,
                () => this.OpenVehicleHood( vehicle, false ) );
            this._taskSequenceEventController.Subscribe( 9, this._mechanicPed, tasks,
                () =>
                {
                    this._mechanicPed.MarkAsNoLongerNeeded();
                    this._mechanicPed.Task.CruiseWithVehicle( this._mechanicVehicle, 30f, (int)DrivingStyle.AvoidTrafficExtremely );
                    vehicle.IsDriveable = true;
                    this._mechanicBlip.Remove();
                    this._taskSequenceEventController.UnsubscribeAll( this._mechanicPed, tasks );
                    this._mechanicVehicle.MarkAsNoLongerNeeded();
                }
            );

            this._mechanicPed.Task.PerformSequence( tasks );
            this._mechanicBlip = this._mechanicPed.AddBlip();
            this._mechanicBlip.Color = BlipColor.Blue;
            this._mechanicBlip.Name = "Mechanic";
        }

        private void OpenVehicleHood( Vehicle vehicle, bool state = true )
        {
            if ( state )
            {
                vehicle.OpenDoor( VehicleDoor.Hood, false, false );
            }
            else
            {
                vehicle.CloseDoor( VehicleDoor.Hood, false );
            }
        }
    }
}