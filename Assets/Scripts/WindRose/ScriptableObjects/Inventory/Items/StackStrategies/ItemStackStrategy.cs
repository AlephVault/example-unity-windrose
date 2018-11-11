using UnityEngine;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                namespace StackStrategies
                {
                    public abstract class ItemStackStrategy : ScriptableObject
                    {
                        /**
                         * This is just a marker class. Just a kind of data bundle
                         *   for the item. It will not execute particular logic
                         *   but it may provide methods to be executed by map
                         *   strategies.
                         */
                    }
                }
            }
        }
    }
}
