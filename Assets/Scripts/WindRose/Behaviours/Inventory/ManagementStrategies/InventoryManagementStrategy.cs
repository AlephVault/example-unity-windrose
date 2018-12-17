using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Inventory
        {
            namespace ManagementStrategies
            {
                public abstract class InventoryManagementStrategy : MonoBehaviour
                {
                    /**
                     * This class is special: It does not refer a particular pack (this is
                     *   different to the Object-related strategies which are particular to a
                     *   specific object or map) but instead be configured to define the rules
                     *   of several packs. Examples:
                     *   
                     * - A chest, coffin (!!!) or bag is a standard container. A strategy being
                     *   defined to them will apply only to them.
                     * - The drops layer, directly above the floor, will have a reference to a
                     *   single strategy, but this will be applied to -at most- MxN stackpacks
                     *   there (each drop cell is a stack pack).
                     * - A vault is not a specific object in the map but an abstract concept. A
                     *   strategy being defined to them will apply only to them.
                     * 
                     * Few strategy types will matter here:
                     * - Spatial strategy: Will determine how to position items inside a pack.
                     *   Positioning may involve an R-tree or plainly indexed items.
                     * - Usage strategy: Will determine how items are used/related.
                     * - Data marshalling strategy. Loosely related to particular data loading
                     *   item strategies, will serialize/inflate the items inside an inventory
                     *   instance.
                     */

                    public InventoryManagementStrategyHolder StrategyHolder
                    {
                        get; private set;
                    }

                    protected virtual void Awake()
                    {
                        StrategyHolder = GetComponent<InventoryManagementStrategyHolder>();
                    }
                }
            }
        }
    }
}
