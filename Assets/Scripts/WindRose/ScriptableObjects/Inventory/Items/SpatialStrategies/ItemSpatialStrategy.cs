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
                namespace SpatialStrategies
                {
                    /// <summary>
                    ///   <para>
                    ///     Spatial strategies are data bundles telling to which
                    ///       extent is the item allowed to be added to an inventory.
                    ///   </para>
                    ///   <para>
                    ///     Quite frequently, stacks occupy one inventory slot, which
                    ///       is indexed. However, in games like Neverwinter Nights,
                    ///       stacks may have irregular dimensions, with the inventory
                    ///       being a bidimensional matrix.The player is compelled to
                    ///       optimize the way they organize their bag.
                    ///   </para>
                    ///   <para>
                    ///     This class will have no behaviour, but just data.There is
                    ///       no need to hold a counterpart Stack Spatial Strategy
                    ///       since all the behaviour will exist on the inventory
                    ///       strategy, and the stack will have no need to know
                    ///       anything additional.
                    ///   </para>
                    ///   <para>
                    ///     This class remains abstract, since data has to be added.
                    ///   </para>
                    /// </summary>
                    public abstract class ItemSpatialStrategy : ItemStrategy<object>
                    {
                    }
                }
            }
        }
    }
}
