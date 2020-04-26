﻿using System;
using System.IO;
using System.Net.Security;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Unit
{
    public class FullIntegrationTest
    {
        private Button powerButton;
        private Button timeButton;
        private Button startCancelButton;
        
        private Door door;
        
        private UserInterface ui;

        private Display display;
        private CookController cookController;
        private Light light;

        private Output output;
        private PowerTube powerTube;
        private Timer timer;

        [SetUp]
        public void Setup()
        {
            output = new Output();
            powerTube = new PowerTube(output);
            timer = new Timer();
            powerButton = new Button();
            timeButton = new Button();
            startCancelButton = new Button();
            door = new Door();
            
            display = new Display(output);
            cookController = new CookController(timer, display, powerTube);
            
            light = new Light(output);
            
            ui = new UserInterface(powerButton, timeButton, startCancelButton, door, display, light, cookController);
        }
        
        [Test]
        public void Ready_DoorOpen_LightOn()
        {
            // This test that uut has subscribed to door opened, and works correctly
            // simulating the event through NSubstitute
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                door.Open();
                string exp = string.Format($"Light is turned on{Environment.NewLine}");
                Assert.AreEqual(exp, sw.ToString());
            }

        }

        [Test]
        public void DoorOpen_DoorClose_LightOff()
        {
            // This test that uut has subscribed to door opened and closed, and works correctly
            // simulating the event through NSubstitute
            door.Open();
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                
                door.Close();
                string exp = string.Format($"Light is turned off{Environment.NewLine}");
                Assert.AreEqual(exp, sw.ToString());
            }
        }

        //Testene herunder skal laves om så de passer til dette system
        [Test]
        public void Ready_DoorOpenClose_Ready_PowerIs50()
        {
            // This test that uut has subscribed to power button, and works correctly
            // simulating the events through NSubstitute
            

            door.Open();
            door.Close();
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                
                powerButton.Press();
                
                string exp = string.Format($"Display shows: 50 W{Environment.NewLine}");
                Assert.AreEqual(exp, sw.ToString());
            }
        }

        [Test]
        public void Ready_2PowerButton_PowerIs100()
        {
            
            powerButton.Press();
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                powerButton.Press();
                
                string exp = string.Format($"Display shows: 100 W{Environment.NewLine}");
                Assert.AreEqual(exp, sw.ToString());
            }
        }

        [Test]
        public void Ready_14PowerButton_PowerIs700()
        {
            for (int i = 1; i <= 13; i++)
            {
                powerButton.Press();
            }
           
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                powerButton.Press();
                
                string exp = string.Format($"Display shows: 700 W{Environment.NewLine}");
                Assert.AreEqual(exp, sw.ToString());
            }
        }

        [Test]
        public void Ready_15PowerButton_PowerIs50Again()
        {
            for (int i = 1; i <= 14; i++)
            {
                powerButton.Press();
            }
            // And then once more
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                powerButton.Press();
                
                string exp = string.Format($"Display shows: 50 W{Environment.NewLine}");
                Assert.AreEqual(exp, sw.ToString());
            }
        }

        [Test]
        public void SetPower_CancelButton_DisplayCleared()
        {
            // Also checks if TimeButton is subscribed
           
            // Now in SetPower
            
            powerButton.Press();
            

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                startCancelButton.Press();
                
                string exp = string.Format($"Display cleared{Environment.NewLine}");
                Assert.AreEqual(exp, sw.ToString());
            }
           
        }

        //Denne test skal kigges på
        [Test]
        public void SetPower_DoorOpened_DisplayCleared()
        {
            // Also checks if TimeButton is subscribed
            powerButton.Press();
            // Now in SetPower

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                door.Open();
                
                string exp = string.Format($"Display cleared{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        //Denne test skal kigges på
        [Test]
        public void SetPower_DoorOpened_LightOn()
        {
            // Also checks if TimeButton is subscribed
            powerButton.Press();
            // Now in SetPower
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                door.Open();
                
                string exp = string.Format($"Light is turned on{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            };
        }

        [Test]
        public void SetPower_TimeButton_TimeIs1()
        {
            // Also checks if TimeButton is subscribed
            powerButton.Press();
            // Now in SetPower

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                timeButton.Press();
                
                string exp = string.Format($"Display shows: 01:00{Environment.NewLine}");
                Assert.AreEqual(exp, sw.ToString());
                //StringAssert.Contains(exp, sw.ToString());
            };

           
        }

        [Test]
        public void SetPower_2TimeButton_TimeIs2()
        {
            powerButton.Press();
            
            // Now in SetPower
            timeButton.Press();
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                timeButton.Press();
                
                string exp = string.Format($"Display shows: 02:00{Environment.NewLine}");
                Assert.AreEqual(exp, sw.ToString());
                //StringAssert.Contains(exp, sw.ToString());
            };
        }

        [Test]
        public void SetTime_StartButton_PowerTube_50()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                startCancelButton.Press();
                
                string exp = string.Format($"PowerTube works with 50{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            };
           // powerTube.Received(1).TurnOn(50);
        }
        
        [Test]
        public void SetTime_StartButton_PowerTube_150()
        {
            powerButton.Press();
            powerButton.Press();
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                startCancelButton.Press();
                
                string exp = string.Format($"PowerTube works with 150{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            };
            
        }
        
        [Test]
        public void SetTime_StartButton_Timer_60_seconds()
        {
            powerButton.Press();
            // Now in SetPower
            
            // Now in SetTime
            

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                timeButton.Press();
                startCancelButton.Press();
                
                string exp = string.Format($"Display shows: 00:59{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            };
            
            //timer.Received().Start(60);
        }

       /*[Test]
        public void SetTime_DoorOpened_DisplayCleared()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            door.Open();

            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("Display cleared")));
        }

        [Test]
        public void SetTime_DoorOpened_LightOn()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            door.Open();

            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("on")));
        }

        [Test]
        public void Ready_PowerAndTime_PowerTube()
        {
            powerButton.Press();
            // Now in SetPower
            powerButton.Press();

            timeButton.Press();
            // Now in SetTime

            // Should call with correct values
            startCancelButton.Press();

            powerTube.Received(1).TurnOn(100);
        }
        
        [Test]
        public void Ready_PowerAndTime_Timer()
        {
            powerButton.Press();
            // Now in SetPower

            timeButton.Press();
            // Now in SetTime
            timeButton.Press();

            // Should call with correct values
            startCancelButton.Press();

            timer.Received(1).Start(120);
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

            powerTube.Received(1).TurnOn(700);

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

            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("on")));
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
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("off")));
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
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("Display cleared")));
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

            powerTube.Received().TurnOff();
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

            powerTube.Received().TurnOff();
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

            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("off")));
        }*/
    }
}