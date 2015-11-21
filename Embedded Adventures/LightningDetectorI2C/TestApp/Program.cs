using Gadgeteer.Modules.EmbeddedAdventures;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

namespace TestApp
{
    public partial class Program
    {
        private readonly LightningDetectorI2C lightningDetectorI2C = new LightningDetectorI2C(5);
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

            lightningDetectorI2C.NoiseDetected += lightningDetectorI2C_NoiseDetected;
            lightningDetectorI2C.DisturbanceDetected += lightningDetectorI2C_DisturbanceDetected;
            lightningDetectorI2C.LightningDetected += lightningDetectorI2C_LightningDetected;
        }

        private void lightningDetectorI2C_LightningDetected(LightningDetectorI2C sender,
            LightningDetectorI2C.LightningEventArgs e)
        {
            Debug.Print("Lightning Detected " + e.Distance + " km away with energy " + e.Energy);
        }

        private void lightningDetectorI2C_DisturbanceDetected(LightningDetectorI2C sender)
        {
            Debug.Print("Disturbance  Detected");
        }

        private void lightningDetectorI2C_NoiseDetected(LightningDetectorI2C sender)
        {
            Debug.Print("Noise Detected");
        }
    }
}