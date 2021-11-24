using GameMeanMachine.Unity.WindRose.Types;


namespace GameMeanMachine.Unity.WindRose.Biomes
{
    namespace Types
    {
        /// <summary>
        ///   This exception is triggered when a biome is not set on behaviours
        ///   that make use of them.
        /// </summary>
        public class MissingBiomeSetException : Exception
        {
            public MissingBiomeSetException() { }
            public MissingBiomeSetException(string message) : base(message) { }
            public MissingBiomeSetException(string message, Exception inner) : base(message, inner) { }
        }
    }
}