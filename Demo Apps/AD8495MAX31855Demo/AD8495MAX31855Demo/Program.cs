using System;
using System.IO;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.Adafruit;

namespace AD8495MAX31855Demo
{
    public partial class Program
    {
        private readonly Font _font = Resources.GetFont(Resources.FontResources.small);
        private readonly GT.Timer _timer = new GT.Timer(1000);
        private StreamWriter _sw;
        // This method is run when the mainboard is powered up or reset.   
        private void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/


            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");
            displayN18.Orientation = GTM.Module.DisplayModule.DisplayOrientation.UpsideDown;

            sdCard.Mounted += sdCard_Mounted;
            sdCard.Unmounted += sdCard_Unmounted;

            button.ButtonPressed += button_ButtonPressed;

            _timer.Tick += _timer_Tick;
        }

        private void button_ButtonPressed(Button sender, Button.ButtonState state)
        {
            if (sender.IsLedOn)
            {
                _timer.Stop();
                if (_sw != null)
                {
                    _sw.Flush();
                    _sw.Close();
                }
            }
            else
            {
                OpenFile();
                _timer.Start();
            }
            sender.ToggleLED();
        }

        private void OpenFile()
        {
            if (sdCard.IsCardInserted && sdCard.IsCardMounted)
            {
                var files = sdCard.StorageDevice.ListRootDirectoryFiles();

                var file = Path.Combine(sdCard.StorageDevice.RootDirectory, (files.Length + 1) + "data.txt");

                _sw = new StreamWriter(file, true);
            }
        }

        private void WriteData(String device, Double temp, Double jt)
        {
            if (sdCard.IsCardInserted && sdCard.IsCardMounted)
            {
                var result = device + " " + temp + ";" + jt;

                try
                {
                    _sw.WriteLine(result);
                }
                catch (Exception x)
                {
                    Debug.Print(x.Message);
                }
            }
        }

        private void sdCard_Unmounted(SDCard sender, EventArgs e)
        {
            Debug.Print("SD Card Unmounted");
            if (_sw != null)
                _sw.Close();
        }

        private void sdCard_Mounted(SDCard sender, GT.StorageDevice device)
        {
            Debug.Print("SD Card Unmounted");
        }

        private void _timer_Tick(GT.Timer timer)
        {
            var t = aD8495Thermocouple.TemperatureCelsius();

            var t2 = mAX31855Thermocouple.TemperatureCelsius();
            var t3 = mAX31855Thermocouple.InternalCelsius();
            //WriteData("1", t2, t3);
            
            var t4 = mAX31855Thermocouple2.TemperatureCelsius();
            var t5 = mAX31855Thermocouple2.InternalCelsius();
            //WriteData("2", t4, t5);

            displayN18.SimpleGraphics.ClearNoRedraw();

            displayN18.SimpleGraphics.DisplayText("AD8495", _font, GT.Color.Blue, 0, 0);
            displayN18.SimpleGraphics.DisplayText("Temp: " + t + " C", _font, GT.Color.Red, 0, 13);

            displayN18.SimpleGraphics.DisplayText("MAX31885 1", _font, GT.Color.Blue, 0, 38);
            displayN18.SimpleGraphics.DisplayText("Temp: " + t2 + " C", _font, GT.Color.Red, 0, 51);
            displayN18.SimpleGraphics.DisplayText("Internal: " + t3 + " C", _font, GT.Color.Red, 0, 64);

            displayN18.SimpleGraphics.DisplayText("MAX31885 2", _font, GT.Color.Blue, 0, 89);
            displayN18.SimpleGraphics.DisplayText("Temp: " + t4 + " C", _font, GT.Color.Red, 0, 102);
            displayN18.SimpleGraphics.DisplayText("Internal: " + t5 + " C", _font, GT.Color.Red, 0, 115);

            displayN18.SimpleGraphics.DisplayText(
                "Faults: " + mAX31855Thermocouple.Fault + "               " + mAX31855Thermocouple2.Fault, _font,
                GT.Color.Green, 0, 140);
        }
    }
}