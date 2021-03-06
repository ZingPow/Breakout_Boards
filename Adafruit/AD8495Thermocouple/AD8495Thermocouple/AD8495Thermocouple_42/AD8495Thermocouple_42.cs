﻿using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GTI = Gadgeteer.Interfaces;

namespace Gadgeteer.Modules.Adafruit
{
    /// <summary>
    ///     A AD8495Thermocouple module for Microsoft .NET Gadgeteer
    /// </summary>
    public class AD8495Thermocouple : Module
    {
        /// <summary>
        ///     Represents the delegate that is used to handle the <see cref="ButtonPressed" />
        ///     and <see cref="ButtonReleased" /> events.
        /// </summary>
        /// <param name="sender">The <see cref="AD8495Thermocouple" /> object that raised the event.</param>
        /// <param name="state">The state of the button of the <see cref="AD8495Thermocouple" /></param>
        public delegate void ButtonEventHandler(AD8495Thermocouple sender, ButtonState state);

        /// <summary>
        ///     Represents the state of button of the <see cref="AD8495Thermocouple" />.
        /// </summary>
        public enum ButtonState
        {
            /// <summary>
            ///     The button is released.
            /// </summary>
            Released = 0,

            /// <summary>
            ///     The button is pressed.
            /// </summary>
            Pressed = 1
        }

        private readonly GTI.InterruptInput input;
        private ButtonEventHandler onButton;
        // This example implements a driver in managed code for a simple Gadgeteer module.  This module uses a 
        // single GTI.InterruptInput to interact with a button that can be in either of two states: pressed or released.
        // The example code shows the recommended code pattern for exposing a property (IsPressed). 
        // The example also uses the recommended code pattern for exposing two events: Pressed and Released. 
        // The triple-slash "///" comments shown will be used in the build process to create an XML file named
        // GTM.Adafruit.AD8495Thermocouple. This file will provide IntelliSense and documentation for the
        // interface and make it easier for developers to use the AD8495Thermocouple module.        

        // -- CHANGE FOR MICRO FRAMEWORK 4.2 --
        // If you want to use Serial, SPI, or DaisyLink (which includes GTI.SoftwareI2C), you must do a few more steps
        // since these have been moved to separate assemblies for NETMF 4.2 (to reduce the minimum memory footprint of Gadgeteer)
        // 1) add a reference to the assembly (named Gadgeteer.[interfacename])
        // 2) in GadgeteerHardware.xml, uncomment the lines under <Assemblies> so that end user apps using this module also add a reference.

        // Note: A constructor summary is auto-generated by the doc builder.
        /// <summary></summary>
        /// <param name="socketNumber">The socket that this module is plugged in to.</param>
        /// <param name="socketNumberTwo">The second socket that this module is plugged in to.</param>
        public AD8495Thermocouple(int socketNumber, int socketNumberTwo)
        {
            // This finds the Socket instance from the user-specified socket number.  
            // This will generate user-friendly error messages if the socket is invalid.
            // If there is more than one socket on this module, then instead of "null" for the last parameter, 
            // put text that identifies the socket to the user (e.g. "S" if there is a socket type S)
            var socket = Socket.GetSocket(socketNumber, true, this, null);

            // This creates an GTI.InterruptInput interface. The interfaces under the GTI namespace provide easy ways to build common modules.
            // This also generates user-friendly error messages automatically, e.g. if the user chooses a socket incompatible with an interrupt input.
            input = new GTI.InterruptInput(socket, Socket.Pin.Three, GTI.GlitchFilterMode.On, GTI.ResistorMode.PullUp,
                GTI.InterruptMode.RisingAndFallingEdge, this);

            // This registers a handler for the interrupt event of the interrupt input (which is bereleased)
            input.Interrupt += _input_Interrupt;
        }

        /// <summary>
        ///     Gets a value that indicates whether the button of the AD8495Thermocouple is pressed.
        /// </summary>
        public bool IsPressed
        {
            get { return input.Read(); }
        }

        private void _input_Interrupt(GTI.InterruptInput input, bool value)
        {
            OnButtonEvent(this, value ? ButtonState.Released : ButtonState.Pressed);
        }

        /// <summary>
        ///     Raised when the button of the <see cref="AD8495Thermocouple" /> is pressed.
        /// </summary>
        /// <remarks>
        ///     Implement this event handler and/or the <see cref="ButtonReleased" /> event handler
        ///     when you want to provide an action associated with button events.
        ///     Since the state of the button is passed to the <see cref="ButtonEventHandler" /> delegate,
        ///     so you can use the same event handler for both button states.
        /// </remarks>
        public event ButtonEventHandler ButtonPressed;

        /// <summary>
        ///     Raised when the button of the <see cref="AD8495Thermocouple" /> is released.
        /// </summary>
        /// <remarks>
        ///     Implement this event handler and/or the <see cref="ButtonPressed" /> event handler
        ///     when you want to provide an action associated with button events.
        ///     Since the state of the button is passed to the <see cref="ButtonEventHandler" /> delegate,
        ///     you can use the same event handler for both button states.
        /// </remarks>
        public event ButtonEventHandler ButtonReleased;

        /// <summary>
        ///     Raises the <see cref="ButtonPressed" /> or <see cref="ButtonReleased" /> event.
        /// </summary>
        /// <param name="sender">The <see cref="AD8495Thermocouple" /> that raised the event.</param>
        /// <param name="buttonState">The state of the button.</param>
        protected virtual void OnButtonEvent(AD8495Thermocouple sender, ButtonState buttonState)
        {
            if (onButton == null)
            {
                onButton = OnButtonEvent;
            }

            if (buttonState == ButtonState.Pressed)
            {
                // Program.CheckAndInvoke helps event callers get onto the Dispatcher thread.  
                // If the event is null then it returns false.
                // If it is called while not on the Dispatcher thread, it returns false but also re-invokes this method on the Dispatcher.
                // If on the thread, it returns true so that the caller can execute the event.
                if (Program.CheckAndInvoke(ButtonPressed, onButton, sender, buttonState))
                {
                    ButtonPressed(sender, buttonState);
                }
            }
            else
            {
                if (Program.CheckAndInvoke(ButtonReleased, onButton, sender, buttonState))
                {
                    ButtonReleased(sender, buttonState);
                }
            }
        }
    }
}