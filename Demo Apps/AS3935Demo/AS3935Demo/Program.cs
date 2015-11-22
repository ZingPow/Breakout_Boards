
using System;
using Gadgeteer.Modules.EmbeddedAdventures;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.mikroElektronika;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

namespace AS3935Demo
{
    public partial class Program
    {
        private string lightningI2C = "";
        private string lightningSPI = "";

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

            lightningDetectorSPI.NoiseDetected += lightningDetectorSPI_NoiseDetected;
            lightningDetectorSPI.DisturbanceDetected += lightningDetectorSPI_DisturbanceDetected;
            lightningDetectorSPI.LightningDetected += lightningDetectorSPI_LightningDetected;
            led7C.SetColor(LED7C.Color.White);

            lightningDetectorI2C.NoiseDetected += lightningDetectorI2C_NoiseDetected;
            lightningDetectorI2C.DisturbanceDetected += lightningDetectorI2C_DisturbanceDetected;
            lightningDetectorI2C.LightningDetected += lightningDetectorI2C_LightningDetected;
            led7C2.SetColor(LED7C.Color.White);
        }

        private void lightningDetectorI2C_LightningDetected(LightningDetectorI2C sender,
            LightningDetectorI2C.LightningEventArgs e)
        {
            led7C2.SetColor(LED7C.Color.Red);
            lightningI2C = "L " + e.Distance + " " + e.Energy;
            UpdateDisplay();
        }

        private void lightningDetectorI2C_DisturbanceDetected(LightningDetectorI2C sender)
        {
            led7C2.SetColor(LED7C.Color.Magenta);
            lightningI2C = "Disturbance";
            UpdateDisplay();
        }

        private void lightningDetectorI2C_NoiseDetected(LightningDetectorI2C sender)
        {
            led7C2.SetColor(LED7C.Color.Yellow);
            lightningI2C = "Noise";
            UpdateDisplay();
        }

        private void lightningDetectorSPI_LightningDetected(LightningDetectorSPI sender,
            LightningDetectorSPI.LightningEventArgs e)
        {
            led7C.SetColor(LED7C.Color.Red);
            lightningSPI = "L " + e.Distance + " " + e.Energy;
            UpdateDisplay();
        }

        private void lightningDetectorSPI_DisturbanceDetected(LightningDetectorSPI sender)
        {
            led7C.SetColor(LED7C.Color.Magenta);
            lightningSPI = "Disturbance";
            UpdateDisplay();
        }

        private void lightningDetectorSPI_NoiseDetected(LightningDetectorSPI sender)
        {
            led7C.SetColor(LED7C.Color.Yellow);
            lightningSPI = "Noise";
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            characterDisplay.Clear();
            characterDisplay.Print(lightningSPI);
            characterDisplay.SetCursorPosition(1, 0);
            characterDisplay.Print(lightningI2C);

            led7C.SetColor(LED7C.Color.Off);
            led7C2.SetColor(LED7C.Color.Off);
        }
    }
}