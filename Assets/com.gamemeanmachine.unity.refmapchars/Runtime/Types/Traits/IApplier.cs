namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Types
    {
        namespace Traits
        {
            /// <summary>
            ///   This interface is conceived to be used several
            ///   times by using different types.
            /// </summary>
            /// <typeparam name="T">The type of the object to use</typeparam>
            public interface IApplier<in T> where T : class
            {
                public void Use(T appliance);
            }
        }
    }
}