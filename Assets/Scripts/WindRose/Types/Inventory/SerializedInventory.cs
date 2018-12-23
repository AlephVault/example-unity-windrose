using System.Collections.Generic;

namespace WindRose
{
    namespace Types
    {
        namespace Inventory
        {
            using Support.Types;

            /**
             * Details of this class: (container position) => (stack position) => (registrar key, item key, quantity, serialized data for use strategies).
             */
            public class SerializedStack : Tuple<string, uint, object, object>
            {
                public SerializedStack(string first, uint second, object third, object fourth) : base(first, second, third, fourth) {}
            }
            public class SerializedContainer : Dictionary<object, SerializedStack> {}
            public class SerializedInventory : Dictionary<object, SerializedContainer> {}
        }
    }
}
