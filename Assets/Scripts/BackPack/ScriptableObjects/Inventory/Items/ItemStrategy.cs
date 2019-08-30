using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BackPack
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                /// <summary>
                ///   Item strategies are initialized against their item
                ///     and will instantiate stack strategies, which will
                ///     ba called from the item.
                /// </summary>
                /// <typeparam name="T">
                ///   Related stack strategy type for this item strategy type
                /// </typeparam>
                public abstract class ItemStrategy<T> : ScriptableObject where T : class
                {
                    /// <summary>
                    ///   Item this stategy is attached to.
                    /// </summary>
                    public Item Item
                    {
                        get; private set;
                    }

                    /// <summary>
                    ///   Invoked by the item, this method initializes this
                    ///     strategy's item only once.
                    /// </summary>
                    /// <param name="item">The item to assign</param>
                    public void Initialize(Item item)
                    {
                        if (Item == null) { Item = item; }
                    }
                }
            }
        }
    }
}
