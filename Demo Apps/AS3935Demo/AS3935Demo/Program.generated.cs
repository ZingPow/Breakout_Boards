//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AS3935Demo {
    using Gadgeteer;
    using GTM = Gadgeteer.Modules;
    
    
    public partial class Program : Gadgeteer.Program {
        
        /// <summary>The Character Display module using socket 4 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.CharacterDisplay characterDisplay;
        
        /// <summary>The LED 7C module using socket 7 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.LED7C led7C;
        
        /// <summary>The LED 7C module using socket 11 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.LED7C led7C2;
        
        /// <summary>The LightningDetectorSPI module using socket 8 of the mainboard.</summary>
        private Gadgeteer.Modules.mikroElektronika.LightningDetectorSPI lightningDetectorSPI;
        
        /// <summary>The LightningDetectorI2C module using socket 10 of the mainboard.</summary>
        private Gadgeteer.Modules.EmbeddedAdventures.LightningDetectorI2C lightningDetectorI2C;
        
        /// <summary>This property provides access to the Mainboard API. This is normally not necessary for an end user program.</summary>
        protected new static GHIElectronics.Gadgeteer.FEZReaper Mainboard {
            get {
                return ((GHIElectronics.Gadgeteer.FEZReaper)(Gadgeteer.Program.Mainboard));
            }
            set {
                Gadgeteer.Program.Mainboard = value;
            }
        }
        
        /// <summary>This method runs automatically when the device is powered, and calls ProgramStarted.</summary>
        public static void Main() {
            // Important to initialize the Mainboard first
            Program.Mainboard = new GHIElectronics.Gadgeteer.FEZReaper();
            Program p = new Program();
            p.InitializeModules();
            p.ProgramStarted();
            // Starts Dispatcher
            p.Run();
        }
        
        private void InitializeModules() {
            this.characterDisplay = new GTM.GHIElectronics.CharacterDisplay(4);
            this.led7C = new GTM.GHIElectronics.LED7C(7);
            this.led7C2 = new GTM.GHIElectronics.LED7C(11);
            this.lightningDetectorSPI = new GTM.mikroElektronika.LightningDetectorSPI(8);
            this.lightningDetectorI2C = new GTM.EmbeddedAdventures.LightningDetectorI2C(10);
        }
    }
}
