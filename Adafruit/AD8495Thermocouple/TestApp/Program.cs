using Gadgeteer.Modules.Adafruit;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

namespace TestApp
{
    public partial class Program
    {
        private readonly GT.Timer _timer = new GT.Timer(1000);
        private readonly AD8495Thermocouple ad8495Thermocouple = new AD8495Thermocouple(5);
        // This method is run when the mainboard is powered up or reset.   
        private void ProgramStarted()
        {
            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(GT.Timer timer)
        {
            Debug.Print("Temp: " + ad8495Thermocouple.TemperatureCelsius() + " C");
        }
    }
}