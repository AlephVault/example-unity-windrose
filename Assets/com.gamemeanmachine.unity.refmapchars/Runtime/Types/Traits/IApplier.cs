namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Types
    {
        namespace Traits
        {
            /// <summary>
            ///   This interface is conceived to be used several
            ///   times by using different types. Implementors
            ///   use it to apply a certain trait.
            /// </summary>
            /// <typeparam name="T">The type of the object to use</typeparam>
            public interface IApplier<in T> where T : class
            {
                /// <summary>
                ///   Uses a trait (with null, clears a trait).
                ///   It can be told to not force an immediate
                ///   update, however.
                /// </summary>
                /// <param name="appliance">The appliance to use</param>
                /// <param name="force">Whether to force the update or not</param>
                public void Use(T appliance, bool force = true);
            }
        }
    }
}