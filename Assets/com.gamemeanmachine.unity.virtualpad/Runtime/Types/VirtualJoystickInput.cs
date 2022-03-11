namespace GameMeanMachine.Unity.VirtualPad
{
    namespace Types
    {
        /// <summary>
        ///   <para>
        ///     These are the virtual representations of joystick
        ///     keys and axes.
        ///   </para>
        ///   <para>
        ///     The keys will always be in {0, 1} states, since
        ///     they are either pressed or not pressed. The D-Pad
        ///     axes will always be in {-1, 0, 1} states (where
        ///     0 is not pressed, and -1 stands for up and left
        ///     directions, respectively).
        ///   </para>
        ///   <para>
        ///     While it is hard to implement analog input, it is
        ///     supported with both axes (on each of the two inputs)
        ///     in the range of [-1, 1], and also the analog press
        ///     which will be in {0, 1} for each analog input.
        ///   </para>
        /// </summary>
        public enum VirtualJoystickInput
        {
            // D-Pad (Axes in {-1, 0, 1} each).
            XAxis, YAxis,
            // Right pad (Buttons in {0, 1} each).
            South, West, East, North,
            // Select, Start (Buttons in {0, 1} each).
            Select, Start,
            // Left, Right (Buttons in {0, 1} each).
            // (they will have this layout)
            L1, L2, R2, R1,
            // Analog double clicks (Buttons in {0, 1} each).
            // They will be triggered on double click.
            LAnalog, RAnalog,
            // Analog axes (Axes in [-1, 1] each).
            LAnalogXAxis, LAnalogYAxis,
            RAnalogXAxis, RAnalogYAxis
        }
    }
}