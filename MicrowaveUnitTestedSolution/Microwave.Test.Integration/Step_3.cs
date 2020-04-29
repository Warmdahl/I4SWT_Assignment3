using System;
using System.IO;
using System.Net.Security;
using System.Threading;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

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
            //door.Open();
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                door.Open();

                door.Close();
                string exp = string.Format($"Light is turned off{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        //Testene herunder skal laves om så de passer til dette system
        [Test]
        public void Ready_DoorOpenClose_Ready_PowerIs50()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                // This test that uut has subscribed to power button, and works correctly
                // simulating the events through NSubstitute
                door.Open();
                door.Close();

                powerButton.Press();
                
                string exp = string.Format($"Display shows: 50 W{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void Ready_2PowerButton_PowerIs100()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                powerButton.Press();

                powerButton.Press();
                
                string exp = string.Format($"Display shows: 100 W{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void Ready_14PowerButton_PowerIs700()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                for (int i = 1; i <= 14; i++)
                {
                    powerButton.Press();
                }
                
                
                string exp = string.Format($"Display shows: 700 W{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void Ready_15PowerButton_PowerIs50Again()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                for (int i = 1; i <= 15; i++)
                {
                    powerButton.Press();
                }
                
                
                string exp = string.Format($"Display shows: 50 W{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void SetPower_CancelButton_DisplayCleared()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Also checks if TimeButton is subscribed
                // Now in SetPower
                powerButton.Press();

                startCancelButton.Press();
                
                string exp = string.Format($"Display cleared{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
           
        }

        
        [Test]
        public void SetPower_DoorOpened_DisplayCleared()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Also checks if TimeButton is subscribed
                powerButton.Press();
                // Now in SetPower

                door.Open();
                
                string exp = string.Format($"Display cleared{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        //Denne test skal kigges på
        [Test]
        public void SetPower_DoorOpened_LightOn()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Also checks if TimeButton is subscribed
                powerButton.Press();
                // Now in SetPower

                door.Open();
                
                string exp = string.Format($"Light is turned on{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            };
        }

        [Test]
        public void SetPower_TimeButton_TimeIs1()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Also checks if TimeButton is subscribed
                powerButton.Press();
                // Now in SetPower

                timeButton.Press();
                
                string exp = string.Format($"Display shows: 01:00{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            };

           
        }

        [Test]
        public void SetPower_2TimeButton_TimeIs2()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                powerButton.Press();
                // Now in SetPower
                timeButton.Press();

                timeButton.Press();
                
                string exp = string.Format($"Display shows: 02:00{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            };
        }

        [Test]
        public void SetTime_StartButton_PowerTube_50()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                powerButton.Press();
                // Now in SetPower
                timeButton.Press();
                // Now in SetTime

                startCancelButton.Press();
                
                string exp = string.Format($"PowerTube works with 50{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            };
        }
        
        [Test]
        public void SetTime_StartButton_PowerTube_150()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                powerButton.Press();
                powerButton.Press();
                powerButton.Press();
                // Now in SetPower
                timeButton.Press();
                // Now in SetTime

                startCancelButton.Press();
                
                string exp = string.Format($"PowerTube works with 150{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
            
        }
        
        [Test]
        public void SetTime_StartButton_Timer_2_m_less_than_121_s()
        {
            using(StringWriter sw = new StringWriter())
            { 
                //Have implement the stringwriter otherwise the test cannot run, sw is not used for the test itself 
                Console.SetOut(sw);

                ManualResetEvent pause = new ManualResetEvent(false);
                powerButton.Press();


                timer.Expired += (sender, args) => pause.Set();
                timeButton.Press();
                timeButton.Press();

                
                    
                startCancelButton.Press();
                Assert.That(!pause.WaitOne(121000));
            }

        }
        
        [Test]
        public void SetTime_StartButton_Timer_1_M_Less_Than_61_S()
        {
            using (StringWriter sw = new StringWriter())
            {
                //Have implement the stringwriter otherwise the test cannot run, sw is not used for the test itself
                Console.SetOut(sw);

                ManualResetEvent pause = new ManualResetEvent(false);
                powerButton.Press();


                timer.Expired += (sender, args) => pause.Set();
                timeButton.Press();
                
                startCancelButton.Press();
                    
                Assert.That(!pause.WaitOne(61000));
            }
        }

       [Test]
        public void SetTime_DoorOpened_DisplayCleared()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                powerButton.Press();
                // Now in SetPower
                timeButton.Press();
                // Now in SetTime

                door.Open();

                string exp = string.Format($"Display cleared{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void SetTime_DoorOpened_LightOn()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                powerButton.Press();
                // Now in SetPower
                timeButton.Press();
                // Now in SetTime

                door.Open();

                string exp = string.Format($"Light is turned on{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void Ready_PowerAndTime_PowerTube()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                // Should call with correct values

                powerButton.Press();
                // Now in SetPower
                powerButton.Press();

                timeButton.Press();
                // Now in SetTime

                startCancelButton.Press();

                string exp = string.Format($"PowerTube works with 100{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }
        
        [Test]
        public void Ready_PowerAndTime_Timer()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                powerButton.Press();
                // Now in SetPower

                timeButton.Press();
                // Now in SetTime

                timeButton.Press();

                // Should call with correct values
                startCancelButton.Press();

                string exp = string.Format($"Display shows: 02:00{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void Ready_FullPower_CookerIsCalledCorrectly()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                for (int i = 50; i <= 700; i += 50)
                {
                    powerButton.Press();
                }

                timeButton.Press();
                // Now in SetTime

                // Should call with correct values
                startCancelButton.Press();

                string exp = string.Format($"PowerTube works with 700{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }


        [Test]
        public void SetTime_StartButton_LightIsCalled()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                powerButton.Press();
                // Now in SetPower
                timeButton.Press();
                // Now in SetTime

                startCancelButton.Press();
                // Now cooking

                string exp = string.Format($"Light is turned on{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void Cooking_CookingIsDone_LightOff()
        {
           using (StringWriter sw = new StringWriter())
           {
                Console.SetOut(sw);
                // Cooking is done

                powerButton.Press();
                // Now in SetPower
                timeButton.Press();
                // Now in SetTime
                startCancelButton.Press();
                // Now in cooking

                ui.CookingIsDone();

                string exp = string.Format($"Light is turned off{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
           }
        }

        [Test]
        public void Cooking_CookingIsDone_ClearDisplay()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                // Cooking is done

                powerButton.Press();
                // Now in SetPower
                timeButton.Press();
                // Now in SetTime
                startCancelButton.Press();
                // Now in cooking

                ui.CookingIsDone();

                string exp = string.Format($"Display cleared{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void Cooking_DoorIsOpened_CookerCalled()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                // Open door

                powerButton.Press();
                // Now in SetPower
                timeButton.Press();
                // Now in SetTime
                startCancelButton.Press();
                // Now in cooking

                door.Open();

                string exp = string.Format($"PowerTube turned off{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void Cooking_CancelButton_CookerCalled()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                // Open door

                powerButton.Press();
                // Now in SetPower
                timeButton.Press();
                // Now in SetTime
                startCancelButton.Press();
                // Now in cooking

                startCancelButton.Press();

                string exp = string.Format($"PowerTube turned off{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void Cooking_CancelButton_LightCalled()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                // Open door

                powerButton.Press();
                // Now in SetPower
                timeButton.Press();
                // Now in SetTime
                startCancelButton.Press();
                // Now in cooking

                startCancelButton.Press();

                string exp = string.Format($"Light is turned off{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }
    }
}