using GameMeanMachine.Unity.WindRose.Types;


namespace GameMeanMachine.Unity.WindRose.Biomes
{
    namespace Types
    {
        public class MissingBiomeSetException : Exception
        {
            public MissingBiomeSetException() { }
            public MissingBiomeSetException(string message) : base(message) { }
            public MissingBiomeSetException(string message, Exception inner) : base(message, inner) { }
        }
    }
}