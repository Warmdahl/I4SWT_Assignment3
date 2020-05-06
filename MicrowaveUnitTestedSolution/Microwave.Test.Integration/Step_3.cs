using System;
using System.IO;
using System.Threading;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using NUnit.Framework;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Unit
{
    public class Step_3
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
        public void DoorOpen_LighOn()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                door.Open();
                string exp = string.Format($"Light is turned on{Environment.NewLine}");
                Assert.AreEqual(exp, sw.ToString());
            }
        }

        [Test]
        public void DoorOpen_And_Close__LightOn_And_Off()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                door.Open();
                door.Close();
                string exp = string.Format($"Light is turned off{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void Power_press_Display_Show_50_W()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                powerButton.Press();
                string exp = string.Format($"Display shows: 50 W{Environment.NewLine}");
                Assert.AreEqual(exp, sw.ToString());
            }
        }

        [Test]
        public void Power_press_Display_Show_700_W()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                for(int i = 0; i <= 13; ++i)
                    powerButton.Press();
                string exp = string.Format($"Display shows: 700 W{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }
        

        [Test]
        public void Time_Press_Display_Show_1_m()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                powerButton.Press();
                timeButton.Press();
                string exp = string.Format($"Display shows: 01:00{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void Time_Press_Display_Show_2_m()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                powerButton.Press();
                for(int i=0; i<=1; ++i)
                    timeButton.Press();
                string exp = string.Format($"Display shows: 02:00{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void Time_Press_Display_Show_10_m()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                powerButton.Press();
                for(int i = 0; i <= 9; ++i)
                    timeButton.Press();
                string exp = string.Format($"Display shows: 10:00{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void Start_Press_LightOn()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                door.Open();
                door.Close();
                powerButton.Press();
                timeButton.Press();
                startCancelButton.Press();
                string exp = string.Format($"Light is turned on{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }

        [Test]
        public void Door_Open_During_Setup_LightOn()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                door.Open();
                door.Close();
                powerButton.Press();
                door.Open();
                string exp = string.Format($"Light is turned on{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }
        
        [Test]
        public void Door_Open_During_Setup_Display_Blank()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                door.Open();
                door.Close();
                powerButton.Press();
                door.Open();
                string exp = string.Format($"Display cleared{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }
        
        [Test]
        public void SetTime_StartButton_Timer_1_m_Less_Than_61_S()
        {
            using (StringWriter sw = new StringWriter())
            {
                
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
        public void SetTime_StartButton_Timer_1_m_More_Than_59_s()
        {
            using (StringWriter sw = new StringWriter())
            {
                
                Console.SetOut(sw);

                ManualResetEvent pause = new ManualResetEvent(true);
                powerButton.Press();
                
                timer.Expired += (sender, args) => pause.Set();
                timeButton.Press();
                
                startCancelButton.Press();
                    
                Assert.That(pause.WaitOne(59000));
            }
        }


        [Test]
        public void PowerTube_TurnsOn()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                door.Open();
                door.Close();
                powerButton.Press();
                timeButton.Press();
                startCancelButton.Press();
                string exp = string.Format($"PowerTube works with 50{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }
        
        [Test]
        public void PowerTube_TurnsOff()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                door.Open();
                door.Close();
                powerButton.Press();
                timeButton.Press();
                startCancelButton.Press();
                startCancelButton.Press();
                string exp = string.Format($"PowerTube turned off{Environment.NewLine}");
                StringAssert.Contains(exp, sw.ToString());
            }
        }
    }
}