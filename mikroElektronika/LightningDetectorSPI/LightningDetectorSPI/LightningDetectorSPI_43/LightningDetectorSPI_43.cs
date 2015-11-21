using System;
using System.Threading;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GTI = Gadgeteer.SocketInterfaces;


namespace Gadgeteer.Modules.mikroElektronika
{
    /// <summary>
    ///     AMS AS3935 mikroElektronika Thunder Click Lighting Detector SPI interface module driver for Microsoft .NET
    ///     Gadgeteer
    ///     <para />
    ///     Wiring Connections
    ///     <para />
    ///     3.3V -- Gadgeteer Pin 1 (3.3V only)
    ///     <para />
    ///     GND -- Gadgeteer Pin 10 GND
    ///     <para />
    ///     IRQ -- Gadgeteer Pin 3
    ///     <para />
    ///     CS -- Gadgeteer Pin 6
    ///     <para />
    ///     SCK -- Gadgeteer Pin 9
    ///     <para />
    ///     SDD -- Gadgeteer Pin 8
    ///     <para />
    ///     SDI -- Gadgeteer Pin 7
    ///     <para />
    ///     Requires a Gadgeteer 'S' Socket
    /// </summary>
    public class LightningDetectorSPI : Module
    {
        /// <summary>Represents the delegate used for the Disturbance Detected event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        public delegate void DisturbanceEventHandler(LightningDetectorSPI sender);

        /// <summary>Represents the delegate used for the Lightning Detected event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        public delegate void LightningEventHandler(LightningDetectorSPI sender, LightningEventArgs e);

        /// <summary>Represents the delegate used for the Noise Detected event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        public delegate void NoiseEventHandler(LightningDetectorSPI sender);

        /// <summary>
        ///     AFE Gain Boost settings.
        /// </summary>
        public enum AFESetting : byte
        {
            /// <summary>
            ///     Sensor located indoors.
            /// </summary>
            Indoor = 0x12,

            /// <summary>
            ///     Sensor located outdoors.
            /// </summary>
            Outdoor = 0x0E,

            /// <summary>
            ///     Unknown Sensor location.
            /// </summary>
            Unknown = 0x0
        }

        /// <summary>
        ///     Bit Masks.
        /// </summary>
        public enum BitMask : byte
        {
            /// <summary>
            ///     AFE Gain Boost BitMask.
            /// </summary>
            AFE_GB = 0x3E,

            /// <summary>
            ///     Mode Control BitMask.
            /// </summary>
            PWD = 0x01,

            /// <summary>
            ///     Noise Floor Level BitMask.
            /// </summary>
            NF_LEV = 0x70,

            /// <summary>
            ///     Watchdog Threshold BitMask.
            /// </summary>
            WDTH = 0x0F,

            /// <summary>
            ///     Clear Statistics BitMask.
            /// </summary>
            CL_STAT = 0x40,

            /// <summary>
            ///     Minimum Number Of Lightning BitMask.
            /// </summary>
            MIN_NUM_LIGH = 0x30,

            /// <summary>
            ///     Spike Rejection BitMask.
            /// </summary>
            SREJ = 0x0F,

            /// <summary>
            ///     Frequency Division Ration For Antenna Tuning BitMask.
            /// </summary>
            LCO_FDIV = 0xC0,

            /// <summary>
            ///     Mask Disturber BitMask.
            /// </summary>
            MASK_DIST = 0x20,

            /// <summary>
            ///     Interrupt BitMask.
            /// </summary>
            INT = 0x0F,

            /// <summary>
            ///     Energy of the Single Lightning LSBYTE BitMask.
            /// </summary>
            S_LIG_L = 0xFF,

            /// <summary>
            ///     Energy of the Single Lightning MSBYTE BitMask.
            /// </summary>
            S_LIG_M = 0xFF,

            /// <summary>
            ///     Energy of the Single Lightning MMSBYTE BitMask.
            /// </summary>
            S_LIG_MM = 0x1F,

            /// <summary>
            ///     Distance estimation BitMask.
            /// </summary>
            DISTANCE = 0x3F,

            /// <summary>
            ///     Display LCO on IRQ pin BitMask.
            /// </summary>
            DISP_LCO = 0x80,

            /// <summary>
            ///     Display SRCO on IRQ pin BitMask.
            /// </summary>
            DISP_SRCO = 0x40,

            /// <summary>
            ///     Display TRCO on IRQ pin BitMask.
            /// </summary>
            DISP_TRCO = 0x20,

            /// <summary>
            ///     Internal Tuning Capacitors BitMask.
            /// </summary>
            TUN_CAP = 0x0F,

            /// <summary>
            ///     Calibration of TRCO done BitMask.
            /// </summary>
            TRCO_CALIB_DONE = 0x80,

            /// <summary>
            ///     Calibration of TRCO unsuccessful BitMask.
            /// </summary>
            TRCO_CALIB_NOK = 0x40,

            /// <summary>
            ///     Calibration of SRCO done BitMask.
            /// </summary>
            SRCO_CALIB_DONE = 0x80,

            /// <summary>
            ///     Calibration of SRCO unsuccessful BitMask.
            /// </summary>
            SRCO_CALIB_NOK = 0x40
        }

        /// <summary>
        ///     Minimum Number of Lightning settings.
        /// </summary>
        public enum MinNumberLightning : byte
        {
            /// <summary>
            ///     One.
            /// </summary>
            One = 0x0,

            /// <summary>
            ///     Five.
            /// </summary>
            Five = 0x01,

            /// <summary>
            ///     Nine.
            /// </summary>
            Nine = 0x02,

            /// <summary>
            ///     Sixteen.
            /// </summary>
            Sixteen = 0x03
        }

        /// <summary>
        ///     Mode settings.
        /// </summary>
        public enum Mode : byte
        {
            /// <summary>
            ///     Mode down mode disabled.
            /// </summary>
            Disabled = 0x0,

            /// <summary>
            ///     Mode down mode enabled
            /// </summary>
            Enabled = 0x1
        }

        /// <summary>
        ///     Register settings.
        /// </summary>
        public enum Register : byte
        {
            /// <summary>
            ///     AFE Gain Boost Register.
            /// </summary>
            AFE_GB = 0x00,

            /// <summary>
            ///     Mode Control Register.
            /// </summary>
            PWD = 0x00,

            /// <summary>
            ///     Noise Floor Level Register.
            /// </summary>
            NF_LEV = 0x01,

            /// <summary>
            ///     Watchdog Threshold Register.
            /// </summary>
            WDTH = 0x01,

            /// <summary>
            ///     Clear Statistics Register.
            /// </summary>
            CL_STAT = 0x02,

            /// <summary>
            ///     Minimum Number Of Lightning Register.
            /// </summary>
            MIN_NUM_LIGH = 0x02,

            /// <summary>
            ///     Spike Rejection Register.
            /// </summary>
            SREJ = 0x02,

            /// <summary>
            ///     Frequency Division Ration For Antenna Tuning Register.
            /// </summary>
            LCO_FDIV = 0x03,

            /// <summary>
            ///     Mask Disturber Register.
            /// </summary>
            MASK_DIST = 0x03,

            /// <summary>
            ///     Interrupt Register.
            /// </summary>
            INT = 0x03,

            /// <summary>
            ///     Energy of the Single Lightning LSBYTE Register.
            /// </summary>
            S_LIG_L = 0x04,

            /// <summary>
            ///     Energy of the Single Lightning MSBYTE Register.
            /// </summary>
            S_LIG_M = 0x05,

            /// <summary>
            ///     Energy of the Single Lightning MMSBYTE Register.
            /// </summary>
            S_LIG_MM = 0x06,

            /// <summary>
            ///     Distance estimation Register.
            /// </summary>
            DISTANCE = 0x07,

            /// <summary>
            ///     Display LCO on IRQ pin Register.
            /// </summary>
            DISP_LCO = 0x08,

            /// <summary>
            ///     Display SRCO on IRQ pin Register.
            /// </summary>
            DISP_SRCO = 0x08,

            /// <summary>
            ///     Display TRCO on IRQ pin Register.
            /// </summary>
            DISP_TRCO = 0x08,

            /// <summary>
            ///     Internal Tuning Capacitors Register.
            /// </summary>
            TUN_CAP = 0x08,

            /// <summary>
            ///     Calibration of TRCO unsuccessful Register.
            /// </summary>
            TRCO_CALIB_DONE = 0x3A,

            /// <summary>
            ///     Calibration of TRCO unsuccessful Register.
            /// </summary>
            TRCO_CALIB_NOK = 0x3A,

            /// <summary>
            ///     Calibration of SRCO done Register.
            /// </summary>
            SRCO_CALIB_DONE = 0x3B,

            /// <summary>
            ///     Calibration of SRCO unsuccessful Register.
            /// </summary>
            SRCO_CALIB_NOK = 0x3B
        }

        private const byte TuneCapSetting = 6; //Based on tests with my detectors
        private readonly Socket _socket;
        private readonly byte[] read2 = new byte[2];
        private readonly GTI.Spi spi;
        private readonly GTI.SpiConfiguration spiConfig;
        //default settings
        private AFESetting _AFEGainBoost = AFESetting.Indoor;
        private Mode _AS3935Power = Mode.Disabled;
        private bool _MaskDisturber;
        private MinNumberLightning _minimumLightning = MinNumberLightning.One;
        private int _NoiseFloorLevel = 2;
        private int _SpikeRejectionLevel = 2;
        private int _WatchDogLevel = 2;
        private GTI.InterruptInput input;
        private DisturbanceEventHandler onDisturbanceEvent;
        private LightningEventHandler onLightningEvent;
        private NoiseEventHandler onNoiseEvent;

        // Note: A constructor summary is auto-generated by the doc builder.
        /// <summary></summary>
        /// <param name="socketNumber">The socket that this module is plugged in to.</param>
        public LightningDetectorSPI(int socketNumber)
        {
            // This finds the Socket instance from the user-specified socket number.  
            // This will generate user-friendly error messages if the socket is invalid.
            // If there is more than one socket on this module, then instead of "null" for the last parameter, 
            // put text that identifies the socket to the user (e.g. "S" if there is a socket type S)
            var socket = Socket.GetSocket(socketNumber, true, this, null);

            socket.EnsureTypeIsSupported('S', this);

            _socket = socket;

            socket.ReservePin(Socket.Pin.Three, this);
            socket.ReservePin(Socket.Pin.Six, this);
            socket.ReservePin(Socket.Pin.Seven, this);
            socket.ReservePin(Socket.Pin.Eight, this);
            socket.ReservePin(Socket.Pin.Nine, this);


            // from as3935 datasheet Chip Select (active low)
            // data are sampled on the falling edge of SCLK (CPHA=1)

            spiConfig = new GTI.SpiConfiguration(false, 0, 0, false, false, 2000);

            spi = GTI.SpiFactory.Create(socket, spiConfig, GTI.SpiSharing.Exclusive, socket, Socket.Pin.Six, this);

            Initialize();
        }

        /// <summary>
        ///     AFE Gain Boost for Indoor or Outdoor locations
        /// </summary>
        public AFESetting AFEGainBoost
        {
            get { return _AFEGainBoost; }
            set
            {
                if (_AFEGainBoost != value)
                {
                    _AFEGainBoost = value;
                    RegisterWrite(Register.AFE_GB, BitMask.AFE_GB, (byte) _AFEGainBoost);
                }
            }
        }

        /// <summary>
        ///     LightningDetector Mode Enabled/Disabled
        ///     NOTE Mode Enabled performs Reset and Calibration
        /// </summary>
        public Mode PowerDownMode
        {
            get
            {
                if (RegisterRead(Register.PWD, BitMask.PWD) == (byte) Mode.Disabled)
                    _AS3935Power = Mode.Disabled;
                else
                {
                    _AS3935Power = Mode.Enabled;
                }

                return _AS3935Power;
            }
            set
            {
                if (RegisterRead(Register.PWD, BitMask.PWD) == (byte) Mode.Disabled)
                    _AS3935Power = Mode.Disabled;
                else
                {
                    _AS3935Power = Mode.Enabled;
                }

                if (_AS3935Power != value)
                {
                    switch (value)
                    {
                        case Mode.Disabled:
                            RegisterWrite(Register.PWD, BitMask.PWD, (byte) Mode.Disabled);
                            Thread.Sleep(2);
                            Calibrate();
                            break;
                        case Mode.Enabled:
                            RegisterWrite(Register.PWD, BitMask.PWD, (byte) Mode.Enabled);
                            break;
                    }

                    Thread.Sleep(10);

                    if (RegisterRead(Register.PWD, BitMask.PWD) == (byte) Mode.Disabled)
                        _AS3935Power = Mode.Disabled;
                    else
                    {
                        _AS3935Power = Mode.Enabled;
                    }
                }
            }
        }

        /// <summary>
        ///     Background Noise Level Setting
        /// </summary>
        public int NoiseFloorLevel
        {
            get { return _NoiseFloorLevel; }
            set
            {
                if (_NoiseFloorLevel != value)
                {
                    if (value < 0)
                        value = 0;
                    if (value > 7)
                        value = 7;

                    _NoiseFloorLevel = value;
                    SetNoiseFloorLevel((byte) _NoiseFloorLevel);
                }
            }
        }

        /// <summary>
        ///     Watchdog Level Setting
        /// </summary>
        public int WatchDogLevel
        {
            get { return _WatchDogLevel; }
            set
            {
                if (_WatchDogLevel != value)
                {
                    if (value < 0)
                        value = 0;
                    if (value > 15)
                        value = 15;

                    _WatchDogLevel = value;
                    SetWatchDogLevel((byte) _WatchDogLevel);
                }
            }
        }

        /// <summary>
        ///     minimum number of events (lightning) have
        ///     been detected in the last 15 minutes for
        ///     the device to raise interrupts
        /// </summary>
        public MinNumberLightning MinimumLightning
        {
            get { return _minimumLightning; }
            set
            {
                if (_minimumLightning != value)
                {
                    _minimumLightning = value;
                    RegisterWrite(Register.MIN_NUM_LIGH, BitMask.MIN_NUM_LIGH, (byte) _minimumLightning);
                }
            }
        }

        /// <summary>
        ///     Spike Rejection level to be used to increase
        ///     the robustness against false alarms from disturbers.
        /// </summary>
        public int SpikeRejectionLevel
        {
            get { return _SpikeRejectionLevel; }
            set
            {
                if (_SpikeRejectionLevel != value)
                {
                    if (value < 0)
                        value = 0;
                    if (value > 15)
                        value = 15;

                    _SpikeRejectionLevel = value;
                    RegisterWrite(Register.SREJ, BitMask.SREJ, (byte) _SpikeRejectionLevel);
                }
            }
        }

        /// <summary>
        ///     Sets if Disturbers will raise interrupts
        /// </summary>
        public bool MaskDisturber
        {
            get { return _MaskDisturber; }
            set
            {
                if (_MaskDisturber != value)
                {
                    _MaskDisturber = value;

                    if (_MaskDisturber)
                        RegisterWrite(Register.MASK_DIST, BitMask.MASK_DIST, 1);
                    else
                    {
                        RegisterWrite(Register.MASK_DIST, BitMask.MASK_DIST, 0);
                    }
                }
            }
        }

        /// <summary>
        ///     Displays registry values in debug window
        /// </summary>
        /// <param name="description">Description header</param>
        public void DisplayRegisters(string description)
        {
            Debug.Print("");
            Debug.Print("Register Dump " + description);
            Debug.Print("             7 6 5 4 3 2 1 0 Value");

            for (byte i = 0; i < 64; i++)
            {
                PrintRegister(i);
            }
            Debug.Print("");
        }

        /// <summary>
        ///     Displays values for a given register address in debug window
        /// </summary>
        /// <param name="register">Register Address</param>
        private void PrintRegister(byte register)
        {
            var display = new char[8];
            byte mask = 1;


            var bits = RawReadRegister(register);

            for (var j = 0; j < 8; j++)
            {
                display[j] = (bits & mask) != 0 ? '1' : '0';
                mask *= 2;
            }

            var output = "";

            foreach (var d in display)
            {
                output = d + " " + output;
            }

            Debug.Print("Register " + register.ToString("D2") + " " + output + " " + bits);
        }

        private void EnableInterrupts()
        {
            input = GTI.InterruptInputFactory.Create(_socket, Socket.Pin.Three, GTI.GlitchFilterMode.On,
                GTI.ResistorMode.Disabled, GTI.InterruptMode.RisingEdge, this);
            input.Interrupt += OnInterrupt;
        }

        private void DisableInterrupts()
        {
            if (input != null)
            {
                input.Interrupt -= OnInterrupt;
                input.Dispose();
            }
        }

        private void SetNoiseFloorLevel(byte level)
        {
            RegisterWrite(Register.NF_LEV, BitMask.NF_LEV, level);
            _NoiseFloorLevel = level;
            Thread.Sleep(2);
        }

        private void SetWatchDogLevel(byte level)
        {
            RegisterWrite(Register.WDTH, BitMask.WDTH, level);
            _WatchDogLevel = level;
            Thread.Sleep(2);
        }

        /// <summary>
        ///     clear the statistics built up by the lightning distance
        ///     estimation algorithm block
        /// </summary>
        public void ClearStatistics()
        {
            RegisterWrite(Register.CL_STAT, BitMask.CL_STAT, 1);
            Thread.Sleep(2);
            RegisterWrite(Register.CL_STAT, BitMask.CL_STAT, 0);
            Thread.Sleep(2);
            RegisterWrite(Register.CL_STAT, BitMask.CL_STAT, 1);
            Thread.Sleep(2);
        }

        /// <summary>
        ///     Event raise when Lightning is detected
        /// </summary>
        public event LightningEventHandler LightningDetected;

        /// <summary>
        ///     Event raise when a Disturbance is detected
        /// </summary>
        public event DisturbanceEventHandler DisturbanceDetected;

        /// <summary>
        ///     Event raise when Noise exceeds Noise Floor Level
        /// </summary>
        public event NoiseEventHandler NoiseDetected;

        private bool Initialize()
        {
            Reset();

            return Calibrate();
        }

        /// <summary>
        ///     Calibrates internal circuits.
        /// </summary>
        public bool Calibrate()
        {
            var success = true;

            DisableInterrupts();

            spi.Write(0x3D, 0x96); //Calibration command
            Thread.Sleep(1);
            //as required from a powered down state
            RegisterWrite(Register.DISP_SRCO, BitMask.DISP_SRCO, 1);
            Thread.Sleep(2);
            RegisterWrite(Register.DISP_SRCO, BitMask.DISP_SRCO, 0);

            var counter = 0;

            do
            {
                Thread.Sleep(5);
            } while ((RegisterRead(Register.SRCO_CALIB_DONE, BitMask.SRCO_CALIB_DONE) == 0 ||
                      RegisterRead(Register.TRCO_CALIB_DONE, BitMask.TRCO_CALIB_DONE) == 0) && counter++ < 7);

            if (counter < 7)
                DebugPrint("Calibration Done  + count = " + counter);
            else
            {
                ErrorPrint("Calibration Didn't Finish");
            }

            if (RegisterRead(Register.SRCO_CALIB_NOK, BitMask.SRCO_CALIB_NOK) == 1 ||
                RegisterRead(Register.TRCO_CALIB_NOK, BitMask.TRCO_CALIB_NOK) == 1)
            {
                success = false;
            }

            DebugPrint(success ? "Calibration Successful" : "Calibration failed");

            EnableInterrupts();

            return success;
        }

        private void OnInterrupt(GTI.InterruptInput input, bool value)
        {
            Thread.Sleep(2);
            var source = RegisterRead(Register.INT, BitMask.INT);

            switch (source)
            {
                case 0x1: // Noise
                    OnNoiseEvent(this);
                    break;
                case 0x4: // Disturbance
                    OnDisturbanceEvent(this);
                    break;
                case 0x8: // Lightning
                    Thread.Sleep(2);
                    OnLightningEvent(this, new LightningEventArgs(ReadDistance(), ReadEnergy()));
                    break;
                default:
                    DebugPrint("Unknown interrupt source = " + source);
                    break;
            }
        }

        private void OnLightningEvent(LightningDetectorSPI sender, LightningEventArgs e)
        {
            if (onLightningEvent == null)
                onLightningEvent = OnLightningEvent;

            if (Program.CheckAndInvoke(LightningDetected, onLightningEvent, sender, e))
            {
                LightningDetected(sender, e);

                if (onLightningEvent == null)
                    onLightningEvent = OnLightningEvent;
            }
        }

        private void OnDisturbanceEvent(LightningDetectorSPI sender)
        {
            if (onDisturbanceEvent == null)
                onDisturbanceEvent = OnDisturbanceEvent;

            if (Program.CheckAndInvoke(DisturbanceDetected, onDisturbanceEvent, sender))
            {
                DisturbanceDetected(sender);
            }
        }

        private void OnNoiseEvent(LightningDetectorSPI sender)
        {
            if (onNoiseEvent == null)
                onNoiseEvent = OnNoiseEvent;

            if (Program.CheckAndInvoke(NoiseDetected, onNoiseEvent, sender))
            {
                NoiseDetected(sender);
            }
        }

        private byte GetShiftCount(byte mask)
        {
            byte i = 0;
            if (mask != 0x0)
            {
                for (i = 1; (~mask & 1) != 0; i++)
                {
                    mask >>= 1;
                }
            }
            return i;
        }

        private byte RawReadRegister(byte register)
        {
            spi.WriteRead(new byte[2] {(byte) ((register & 0x3f) | 0x40), 0}, read2);
            return read2[1];
        }

        private void RegisterWrite(Register register, BitMask mask, byte data)
        {
            var regval = RawReadRegister((byte) register);
            regval &= (byte) ~(mask);

            if (mask != 0x0)
            {
                regval |= (byte) (data << (GetShiftCount((byte) mask) - 1));
            }
            else
            {
                regval |= data;
            }
            try
            {
                spi.Write((byte) ((byte) register & 0x3F), regval);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        private byte RegisterRead(Register register, BitMask mask)
        {
            var regval = RawReadRegister((byte) register);

            regval &= (byte) mask;

            if (mask != 0x0)
            {
                regval >>= (GetShiftCount((byte) mask) - 1);
            }

            return regval;
        }

        /// <summary>
        ///     Resets setting to factory defaults.
        /// </summary>
        public void Reset()
        {
            spi.Write(0x3C, 0x96); //reset command
            Thread.Sleep(10);

            RegisterWrite(Register.TUN_CAP, BitMask.TUN_CAP, TuneCapSetting);
            //from external testing I know this works best for my chip
            Thread.Sleep(5);

            DebugPrint("Tune Cap Setting " + RegisterRead(Register.TUN_CAP, BitMask.TUN_CAP));

            //set local variables to match default settings
            _AFEGainBoost = AFESetting.Indoor;

            _NoiseFloorLevel = 2;

            _WatchDogLevel = 2;

            _minimumLightning = MinNumberLightning.One;

            _SpikeRejectionLevel = 2;
            _MaskDisturber = false;
        }

        private int ReadEnergy()
        {
            var buffer = new byte[4];

            buffer[3] = 0;
            buffer[2] = RegisterRead(Register.S_LIG_L, BitMask.S_LIG_L);
            buffer[1] = RegisterRead(Register.S_LIG_M, BitMask.S_LIG_M);
            buffer[0] = RegisterRead(Register.S_LIG_MM, BitMask.S_LIG_MM);

            var e = (int) (((UInt32) buffer[0] << 16) | (UInt32) buffer[1] << 8 | buffer[2]);

            return e;
        }

        private int ReadDistance()
        {
            var d = RegisterRead(Register.DISTANCE, BitMask.DISTANCE);
            d = (byte) DistanceLookup(d);
            return d;
        }

        private int DistanceLookup(byte reading)
        {
            int d = reading;

            if (d > 40)
                d = -1;

            if (d < 5)
                d = 0;

            return d;
        }

        /// <summary>Event arguments for the Lighting Detected event.</summary>
        public class LightningEventArgs : EventArgs
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="LightningEventArgs" /> class.
            /// </summary>
            /// <param name="distance">Distance of the detected lightning.</param>
            /// <param name="energy">Energy of the detected lightning. This value is just a pure number and has no physical meaning.</param>
            internal LightningEventArgs(Int32 distance, Int32 energy)
            {
                Distance = distance;
                Energy = energy;
            }

            /// <summary>
            ///     Gets the distance of the detected lightning.
            /// </summary>
            /// <value>
            ///     The distance in kilometers. Max distance is 40 km.
            /// </value>
            public Int32 Distance { get; private set; }

            /// <summary>
            ///     Gets the energy of the detected lightning.
            /// </summary>
            /// <value>
            ///     This value is just a pure number and has no physical meaning.
            /// </value>
            public Int32 Energy { get; private set; }
        }
    }
}