#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using EngineOverheat.Model;
using GTA;
using GTA.Math;
using GTA.Native;

#endregion

namespace EngineOverheat.Controller
{
    internal class MechanicController
    {
        #region Fields

        public static MechanicController Instance => _instance ?? ( _instance = new MechanicController() );

        #endregion

        private static MechanicController _instance;

        public Ped _mechanicPed;
        private Vehicle _mechanicVehicle;
        private Blip _mechanicBlip;
        private readonly TaskSequenceEventController _taskSequenceEventController = TaskSequenceEventController.Instance;
        private readonly MySettings _settings = MySettings.Instance;
        private bool _isCalled;
        private bool _isPossibleToCancelCall;
        private readonly List<Vehicle> _vehiclesWaitingForMechanic;
        private bool _isDisposing = false;

        private MechanicController()
        {
            this._vehiclesWaitingForMechanic = new List<Vehicle>();
            this._isPossibleToCancelCall = false;
        }

        public void Dispose()
        {
            this._isDisposing = true;

            foreach (Vehicle vehicle in this._vehiclesWaitingForMechanic.ToList())
            {
                this.UncallMechanic(vehicle);
            }
        }

        private bool TakeMoney()
        {
            int callMechanicPrice = this._settings.CallMechanicPayment;
            var playerMoney = Game.Player.Money;
            if ( playerMoney < callMechanicPrice )
            {
                return false;
            }
            Game.Player.Money -= callMechanicPrice;
            return true;
        }

        private void CancelMechanicCall( Vehicle vehicle )
        {
            if (!this._isPossibleToCancelCall)
            {
                UI.Notify("Mechanic is already called!", true);
                return;
            }
            this._isPossibleToCancelCall = false;
            this.UncallMechanic(vehicle);
            Game.Player.Money += this._settings.CallMechanicPayment;
            UI.Notify("Call for Mechanic is cancelled.");
        }

        public void CallMechanic( Vehicle vehicle, Engine vehicleEngine )
        {
            if ( this._isCalled )
            {
                this.CancelMechanicCall(vehicle);
                return;
            }

            if ( Math.Round( vehicle.Speed, 2 ) > 0 )
            {
                UI.Notify( "You should stop your car before calling the mechanic!", true );
                return;
            }

            if ( !this.TakeMoney() )
            {
                UI.Notify( "Not enought money!", true );
                return;
            }

            vehicle.IsDriveable = false;
            var vehiclePosition = vehicle.Position;
            var spawnPosition = World.GetNextPositionOnStreet( vehiclePosition.Around( 80f ) );

            this._mechanicVehicle = World.CreateVehicle( VehicleHash.Panto, spawnPosition );
            this._mechanicPed = this._mechanicVehicle.CreatePedOnSeat( VehicleSeat.Driver, PedHash.FatWhite01AFM );
            this._mechanicPed.Weapons.Give( WeaponHash.FireExtinguisher, 10000, true, true );

            var openHoodPosition = vehicle.GetOffsetInWorldCoords( new Vector3( 0.35f, 3.0f, 0.0f ) );
            var shootPosition = vehicle.GetOffsetInWorldCoords( new Vector3( 0.35f, 5.0f, 0.0f ) );

            var tasks = new TaskSequence();
            tasks.AddTask.DriveTo( this._mechanicVehicle, vehiclePosition, 15f, 40, (int)DrivingStyle.Rushed );
            tasks.AddTask.RunTo(openHoodPosition);
            tasks.AddTask.TurnTo(vehicle, 1000);
            tasks.AddTask.Wait(1000);
            tasks.AddTask.GoTo(shootPosition);
            tasks.AddTask.ShootAt(vehiclePosition, 15500, FiringPattern.FullAuto);
            tasks.AddTask.GoTo(openHoodPosition);
            tasks.AddTask.TurnTo(vehicle, 1000);
            tasks.AddTask.Wait(1000);
            tasks.AddTask.CruiseWithVehicle(this._mechanicVehicle, 30f, (int)DrivingStyle.Rushed);
            tasks.Close();

            this._taskSequenceEventController.Subscribe(2, this._mechanicPed, tasks,
                () =>
                {
                    this._mechanicVehicle.LeftIndicatorLightOn = true;
                    this._mechanicVehicle.RightIndicatorLightOn = true;
                });

            this._taskSequenceEventController.Subscribe(4, this._mechanicPed, tasks,
                () => OpenVehicleHood(vehicle));

            this._taskSequenceEventController.Subscribe(5, this._mechanicPed, tasks,
                () =>
                {
                    this._isPossibleToCancelCall = false;
                    vehicleEngine.Temperature -= 0.5f;
                    vehicleEngine.Damage -= 5.5f;
                    vehicle.EngineHealth += vehicle.EngineHealth >= 1000 ? 0 : 2f;
                }, true);

            this._taskSequenceEventController.Subscribe(8, this._mechanicPed, tasks,
                () => OpenVehicleHood(vehicle, false));

            this._taskSequenceEventController.Subscribe(9, this._mechanicPed, tasks,
                () =>
                {
                    this._mechanicPed.MarkAsNoLongerNeeded();
                    this._mechanicPed.Task.CruiseWithVehicle(this._mechanicVehicle, 30f,
                        (int)DrivingStyle.AvoidTrafficExtremely);
                    vehicle.IsDriveable = true;
                    this._mechanicBlip.Remove();
                    this._taskSequenceEventController.UnsubscribeAll(this._mechanicPed, tasks);
                    this._mechanicVehicle.MarkAsNoLongerNeeded();
                    this._mechanicVehicle.LeftIndicatorLightOn = false;
                    this._mechanicVehicle.RightIndicatorLightOn = false;
                    this._isCalled = false;
                }
                );

            this._mechanicPed.Task.PerformSequence( tasks );
            this._mechanicBlip = this._mechanicPed.AddBlip();
            this._mechanicBlip.Color = BlipColor.Blue;
            this._mechanicBlip.Name = "Mechanic";
            this._isCalled = true;
            this._isPossibleToCancelCall = true;
            this._vehiclesWaitingForMechanic.Add(vehicle);
        }

        private static void OpenVehicleHood( Vehicle vehicle, bool state = true )
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

        private void UncallMechanic(Vehicle vehicle)
        {
            this._mechanicPed.MarkAsNoLongerNeeded();
            this._mechanicPed.Task.ClearAllImmediately();
            this._taskSequenceEventController.UnsubscribeAll(this._mechanicPed);

            this._mechanicVehicle.MarkAsNoLongerNeeded();
            this._mechanicVehicle.LeftIndicatorLightOn = false;
            this._mechanicVehicle.RightIndicatorLightOn = false;

            this._mechanicBlip.Remove();

            vehicle.IsDriveable = true;
            OpenVehicleHood(vehicle, false);

            this._vehiclesWaitingForMechanic.Remove(vehicle);
            this._isCalled = false;
        }

        public void Tick()
        {
            if (!this._isCalled || this._isDisposing)
            {
                return;
            }

            foreach (Vehicle vehicle in this._vehiclesWaitingForMechanic.ToList())
            {
                if (this._isDisposing)
                {
                    return;
                }

                if (!this._mechanicPed.IsAlive)
                {
                    this.UncallMechanic(vehicle);
                    UI.Notify("Something bad happened to the mechanic. Please try again.");
                }
            }
        }
    }
}