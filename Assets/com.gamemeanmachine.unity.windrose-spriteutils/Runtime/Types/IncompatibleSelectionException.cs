using AlephVault.Unity.Support.Types;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Types
    {
        /// <summary>
        ///   This error is thrown when a selection is not compatible
        ///   with the applier behaviour trying to use it.
        /// </summary>
        public class IncompatibleSelectionException : Exception
        {
            public IncompatibleSelectionException() : base() {}
            public IncompatibleSelectionException(string message) : base(message) {}
            public IncompatibleSelectionException(string message, System.Exception inner) : base(message, inner) {}
        }
    }
}