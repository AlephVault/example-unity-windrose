using System;
using System.Collections.Generic;

namespace BackPack
{
    namespace Types
    {
        namespace Inventory
        {
            /// <summary>
            ///   A serialized stack in terms of (item registrar key, item key, quantity, serialized data for use strategies).
            ///   It only makes sense for items with registrars.
            /// </summary>
            public class SerializedStack : Tuple<string, uint, object, object>
            {
                public SerializedStack(string first, uint second, object third, object fourth) : base(first, second, third, fourth) {}
            }
            /// <summary>
            ///   A serialized container holding (position) => <see cref="SerializedStack"/>.
            /// </summary>
            public class SerializedContainer : Dictionary<object, SerializedStack> {}
            /// <summary>
            ///   A serialized inventory holding (container position) => <see cref="SerializedContainer"/>. 
            /// </summary>
            public class SerializedInventory : Dictionary<object, SerializedContainer> {}
        }
    }
}
