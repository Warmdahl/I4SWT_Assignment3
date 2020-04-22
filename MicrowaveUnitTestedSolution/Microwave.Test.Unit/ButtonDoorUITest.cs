using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Unit
{
    public class ButtonDoorUITest
    {
        private Button powerButton;
        private Button timeButton;
        private Button startCancelButton;
        
        private Door door;
        
        private UserInterface ui;

        private IDisplay display;
        private ICookController cookController;
        private ILight light;

        [SetUp]
        public void Setup()
        {
            powerButton = new Button();
            timeButton = new Button();
            startCancelButton = new Button();
            door = new Door();
            
            display = Substitute.For<IDisplay>();
            cookController = Substitute.For<ICookController>();
            light = Substitute.For<ILight>();
            
            ui = new UserInterface(powerButton, timeButton, startCancelButton, door, display, light, cookController);
        }

        [Test]
        public void Ready_DoorOpen_LightOn()
        {
            // This test that uut has subscribed to door opened, and works correctly
            // simulating the event through NSubstitute
            door.Open();
            light.Received().TurnOn();
        }

        [Test]
        public void DoorOpen_DoorClose_LightOff()
        {
            // This test that uut has subscribed to door opened and closed, and works correctly
            // simulating the event through NSubstitute
            door.Open();
            door.Close();
            light.Received().TurnOff();
        }

        [Test]
        public void Ready_DoorOpenClose_Ready_PowerIs50()
        {
            // This test that uut has subscribed to power button, and works correctly
            // simulating the events through NSubstitute
            door.Open();
            door.Close();
            powerButton.Press();

            display.Received(1).ShowPower(Arg.Is<int>(50));
        }

        [Test]
        public void Ready_2PowerButton_PowerIs100()
        {
            powerButton.Press();
            powerButton.Press();
            display.Received(1).ShowPower(Arg.Is<int>(100));
        }

        [Test]
        public void Ready_14PowerButton_PowerIs700()
        {
            for (int i = 1; i <= 14; i++)
            {
                powerButton.Press();
            }
            display.Received(1).ShowPower(Arg.Is<int>(700));
        }

        [Test]
        public void Ready_15PowerButton_PowerIs50Again()
        {
            for (int i = 1; i <= 15; i++)
            {
                powerButton.Press();
            }
            // And then once more
            powerButton.Press();
            display.Received(2).ShowPower(50);
        }

        [Test]
        public void SetPower_CancelButton_DisplayCleared()
        {
            // Also checks if TimeButton is subscribed
           
            // Now in SetPower
            
            powerButton.Press();
            startCancelButton.Press();

            display.Received(1).Clear();
        }

        [Test]
        public void SetPower_DoorOpened_DisplayCleared()
        {
            // Also checks if TimeButton is subscribed
            powerButton.Press();
            // Now in SetPower
            door.Open();

            display.Received(1).Clear();
        }

        [Test]
        public void SetPower_DoorOpened_LightOn()
        {
            // Also checks if TimeButton is subscribed
            powerButton.Press();
            // Now in SetPower
            door.Open();

            light.Received(1).TurnOn();
        }

        [Test]
        public void SetPower_TimeButton_TimeIs1()
        {
            // Also checks if TimeButton is subscribed
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();

            display.Received(1).ShowTime(Arg.Is<int>(1), Arg.Is<int>(0));
        }

        [Test]
        public void SetPower_2TimeButton_TimeIs2()
        {
            powerButton.Press();
            
            // Now in SetPower
            timeButton.Press();
            timeButton.Press();

            display.Received(1).ShowTime(Arg.Is<int>(2), Arg.Is<int>(0));
        }

        [Test]
        public void SetTime_StartButton_CookerIsCalled()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            startCancelButton.Press();

            cookController.Received(1).StartCooking(50, 60);
        }

       [Test]
        public void SetTime_DoorOpened_DisplayCleared()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            door.Open();

            display.Received().Clear();
        }

        [Test]
        public void SetTime_DoorOpened_LightOn()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            door.Open();

            light.Received().TurnOn();
        }

        [Test]
        public void Ready_PowerAndTime_CookerIsCalledCorrectly()
        {
            powerButton.Press();
            // Now in SetPower
            powerButton.Press();

            timeButton.Press();
            // Now in SetTime
            timeButton.Press();

            // Should call with correct values
            startCancelButton.Press();

            cookController.Received(1).StartCooking(100, 120);
        }

        [Test]
        public void Ready_FullPower_CookerIsCalledCorrectly()
        {
            for (int i = 50; i <= 700; i += 50)
            {
                powerButton.Press();
            }

            timeButton.Press();
            // Now in SetTime

            // Should call with correct values
            startCancelButton.Press();

            cookController.Received(1).StartCooking(700, 60);

        }


        [Test]
        public void SetTime_StartButton_LightIsCalled()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            startCancelButton.Press();
            // Now cooking

            light.Received(1).TurnOn();
        }

        [Test]
        public void Cooking_CookingIsDone_LightOff()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            startCancelButton.Press();
            // Now in cooking

            ui.CookingIsDone();
            light.Received(1).TurnOff();
        }

        [Test]
        public void Cooking_CookingIsDone_ClearDisplay()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            startCancelButton.Press();
            // Now in cooking

            // Cooking is done
            ui.CookingIsDone();
            display.Received(1).Clear();
        }

        [Test]
        public void Cooking_DoorIsOpened_CookerCalled()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            startCancelButton.Press();
            // Now in cooking

            // Open door
            door.Open();

            cookController.Received(1).Stop();
        }

        [Test]
        public void Cooking_CancelButton_CookerCalled()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            startCancelButton.Press();
            // Now in cooking

            // Open door
            startCancelButton.Press();

            cookController.Received(1).Stop();
        }

        [Test]
        public void Cooking_CancelButton_LightCalled()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            startCancelButton.Press();
            // Now in cooking

            // Open door
            startCancelButton.Press();

            light.Received(1).TurnOff();
        }
    }
}