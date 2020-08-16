namespace NetRose
{
    namespace Worlds
    {
        /// <summary>
        ///   Triggered when the new map to attach a networked map object, has no networked map behaviour.
        /// </summary>
        class MapNotSynchronized : Exception
        {
            public MapNotSynchronized() { }
            public MapNotSynchronized(string message) : base(message) { }
            public MapNotSynchronized(string message, System.Exception inner) : base(message, inner) { }
        }
    }
}
