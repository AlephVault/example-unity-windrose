using UnityEngine;
using System.Collections.Generic;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                namespace DisplayStrategies
                {
                    public abstract class ItemDisplayStrategy : ScriptableObject
                    {
                        /**
                         * Display strategies add data being intended as purely
                         *   representational of the item. The addition is cummulative,
                         *   and there will be OTHER elements (which could be understood
                         *   as inventory displays) that will make use of them.
                         */

                        public class DisplayData : Dictionary<string, object> { }

                        public abstract void PopulateDisplayData(DisplayData data, Item item);
                    }
                }
            }
        }
    }
}
