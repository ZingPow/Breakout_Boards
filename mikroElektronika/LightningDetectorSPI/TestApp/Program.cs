using Gadgeteer.Modules.mikroElektronika;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

namespace TestApp
{
    public partial class Program
    {
        private readonly LightningDetectorSPI lightningDetectorSpi = new LightningDetectorSPI(9);
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

            lightningDetectorSpi.NoiseDetected += lightningDetectorSpi_NoiseDetected;
            lightningDetectorSpi.DisturbanceDetected += lightningDetectorSpi_DisturbanceDetected;
            lightningDetectorSpi.LightningDetected += lightningDetectorSpi_LightningDetected;
        }

        private void lightningDetectorSpi_LightningDetected(LightningDetectorSPI sender,
            LightningDetectorSPI.LightningEventArgs e)
        {
            Debug.Print("Lightning Detected " + e.Distance + " km away with energy " + e.Energy);
        }

        private void lightningDetectorSpi_DisturbanceDetected(LightningDetectorSPI sender)
        {
            Debug.Print("Disturbance  Detected");
        }

        private void lightningDetectorSpi_NoiseDetected(LightningDetectorSPI sender)
        {
            Debug.Print("Noise Detected");
        }
    }
}