using GMM.Types;
using GMM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BackPack
{
    namespace Behaviours
    {
        namespace Inventory
        {
            using ScriptableObjects.Inventory.Items;
            using ManagementStrategies.RenderingStrategies;
            using Types.Inventory.Standard;

            namespace Standard
            {
                /// <summary>
                ///   This is a rendering strategy for <see cref="StandardInventory"/>
				///     behaviours. This strategy will allow the connection of several
				///     objects acting as "viewers" (<see cref="RenderingListener"/>).
                /// </summary>
                public class InventoryStandardRenderingManagementStrategy : Inventory1DIndexedStaticRenderingManagementStrategy
                {
                    // The rendering broadcaster for this strategy.
                    public RenderingBroadcaster Broadcaster { get; private set; }

                    /// <summary>
                    ///   The <see cref="StandardInventory"/> this strategy is linked to.
                    /// </summary>
                    public StandardInventory SingleInventory
                    {
                        get; private set;
                    }

                    /// <summary>
                    ///   The max size of the container in the <see cref="StandardInventory"/>. This size will
					///     actually be taken from the related spatial strategy.
                    /// </summary>
                    public int MaxSize
                    {
                        get; private set;
                    }

                    protected override void Awake()
                    {
                        base.Awake();
                        MaxSize = spatialStrategy.GetSize();
						SingleInventory = GetComponent<StandardInventory>();
                        Broadcaster = new RenderingBroadcaster(FullStart);
                    }

                    void OnDestroy()
                    {
                        Broadcaster.DisconnectAll();
                    }

                    // Clears and fully updates a given listener.
                    private void FullStart(RenderingListener listener)
                    {
                        listener.Clear();
                        IEnumerable<Tuple<int, Types.Inventory.Stacks.Stack>> pairs = SingleInventory.StackPairs();
                        foreach (Tuple<int, Types.Inventory.Stacks.Stack> pair in pairs)
                        {
                            listener.UpdateStack(pair.Item1, pair.Item2.Item, pair.Item2.Quantity);
                        }
                    }

                    /**************************************
                     * Methods to delegate the rendering on the listener
                     **************************************/

                    /// <summary>
                    ///   This method is invoked by the related inventory management strategy holder
                    ///     and will delegate everything in the underlying : clearing
                    ///     its contents.
                    /// </summary>
                    public override void EverythingWasCleared()
                    {
                        Broadcaster.Clear();
                    }

                    /// <summary>
                    ///   This method is invoked by the related inventory management strategy holder
                    ///     and will delegate everything in the underlying listeners: updating
                    ///     a stack.
                    /// </summary>
                    protected override void StackWasUpdated(object containerPosition, int stackPosition, Item item, object quantity)
                    {
                        Broadcaster.Update(stackPosition, item, quantity);
                    }

                    /// <summary>
                    ///   This method is invoked by the related inventory management strategy holder
                    ///     and will delegate everything in the underlying listeners: removing a
                    ///     stack.
                    /// </summary>
                    protected override void StackWasRemoved(object containerPosition, int stackPosition)
                    {
                        Broadcaster.Remove(stackPosition);
                    }
                }
            }
        }
    }
}
