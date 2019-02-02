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
                /// <summary>
                ///   <para>
                ///     This class is special: It does not refer a particular container (this is
                ///       different to the Object-related strategies which are particular to a
                ///       specific object or map) but instead be configured to define the rules
                ///       of several containers. Examples:
                ///   </para>
                ///   <list type="bullet">
                ///     <item>
                ///       <term>A chest, coffin (!!!) or bag</term>
                ///       <description>A strategy being defined to them will apply only to them.</description>
                ///     </item>
                ///     <item>
                ///       <term>The drops layer, directly above the floor</term>
                ///       <description>One single strategy of each subtype will handle all the containers there.</description>
                ///     </item>
                ///     <item>
                ///       <term>Vaults</term>
                ///       <description>Will work as chests but the objects will NOT belong in any way to an in-map representation.</description>
                ///     </item>
                ///   </list>
                ///   <para>The following subtypes will matter here:</para>
                ///   <list type="bullet">
                ///     <item>
                ///       <term>Positioning strategy</term>
                ///       <description>Tells how to identify and locate a container.</description>
                ///     </item>
                ///     <item>
                ///       <term>Spatial strategy</term>
                ///       <description>Tells how to refer a position and dimensions of stacks inside a container.</description>
                ///     </item>
                ///     <item>
                ///       <term>Usage strategies</term>
                ///       <description>Provide logic and handle the state of the stacks, if any.</description>
                ///     </item>
                ///     <item>
                ///       <term>Rendering strategies</term>
                ///       <description>Present the UI appropriately to the player, updated with the inventory contents.</description>
                ///     </item>
                ///   </list>
                /// </summary>
                public abstract class InventoryManagementStrategy : MonoBehaviour
                {
                    /// <summary>
                    ///   The related strategy holder. It will belong to the same game object.
                    /// </summary>
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
