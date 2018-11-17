using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                public abstract class ItemStrategy<T> : ScriptableObject where T : class
                {
                    /**
                     * Item strategies are initialized against their item
                     *   and can instantiate stack strategies, which will
                     *   be called from the item.
                     */

                    public Item Item
                    {
                        get; private set;
                    }

                    public void Initialize(Item item)
                    {
                        if (Item == null) { Item = item; }
                    }

                    public abstract T CreateStackStrategy(object argument);
                }
            }
        }
    }
}
