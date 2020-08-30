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
            using ScriptableObjects.Inventory.Items.RenderingStrategies;
            using ManagementStrategies.RenderingStrategies;

            namespace Standard
            {
                /// <summary>
                ///   This is a rendering strategy for <see cref="StandardInventory"/>
				///     behaviours. This strategy will allow the connection of several
				///     objects acting as "viewers" (<see cref="RenderingListener"/>).
                /// </summary>
                public class InventoryStandardRenderingManagementStrategy : Inventory1DIndexedStaticRenderingManagementStrategy
                {
                    /// <summary>
                    ///   Listeners will refresh icon and text for the stack, and will account for
                    ///     a single container and an 1D-indexed intra-container positioning.
                    /// </summary>
                    public interface RenderingListener
                    {
                        void Connected();
                        void UpdateStack(int stackPosition, Item item, object quantity);
                        void RemoveStack(int position);
                        void Clear();
                        void Disconnected();
                    }

                    /// <summary>
					///   Tells when trying to add a null <see cref="RenderingListener"/>
					///     when calling <see cref="AddListener(RenderingListener)"/>.
                    /// </summary>
                    public class InvalidListenerException : GMM.Types.Exception
                    {
                        public InvalidListenerException(string message) : base(message) { }
                    }

                    // Effective set of the listeners to be used, either by preloading from the editor or by
                    //   adding / removing listeners.
                    private HashSet<RenderingListener> listenersSet = new HashSet<RenderingListener>();

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
                    }

                    void OnDestroy()
                    {
                        HashSet<RenderingListener> cloned = new HashSet<RenderingListener>(listenersSet);
                        listenersSet.Clear();
                        foreach (RenderingListener listener in cloned)
                        {
                            listener.Disconnected();
                        }
                    }

                    // Clears and fully updates a given listener.
                    private void FullUpdate(RenderingListener listener)
                    {
                        listener.Clear();
                        IEnumerable<Tuple<int, Types.Inventory.Stacks.Stack>> pairs = SingleInventory.StackPairs();
                        foreach (Tuple<int, Types.Inventory.Stacks.Stack> pair in pairs)
                        {
                            listener.UpdateStack(pair.Item1, pair.Item2.Item, pair.Item2.Quantity);
                        }
                    }

                    /// <summary>
                    ///   Adds a listener to this rendering management strategy. The listener will
                    ///     refresh with this listener's data accordingly, and will be synchronized until
					///     it is removed by a call to <see cref="RemoveListener(RenderingListener)"/>.
                    /// </summary>
					/// <param name="listener">The <see cref="RenderingListener"/> to add</param>
                    /// <returns>Whether it could be added, or it was already added</returns>
                    public bool AddListener(RenderingListener listener)
                    {
                        if (listener == null)
                        {
                            throw new InvalidListenerException("Listener to add cannot be null");
                        }

                        if (listenersSet.Contains(listener))
                        {
                            return false;
                        }

                        listenersSet.Add(listener);
                        listener.Connected();
                        // We will force the listener to be cleared, and
                        // also refresh each item. This, to decouple from
                        // the inventory itself.
                        FullUpdate(listener);
                        return true;
                    }

                    /// <summary>
                    ///   Removes a listener from this rendering management strategy. The listener will
                    ///     be cleared and removed.
                    /// </summary>
					/// <param name="listener">The <see cref="RenderingListener"/> to remove</param>
                    /// <returns>Whether it could be removed, or it wasn't connected here on first place</returns>
                    public bool RemoveListener(RenderingListener listener)
                    {
                        if (!listenersSet.Contains(listener))
                        {
                            return false;
                        }

                        listenersSet.Remove(listener);
                        listener.Disconnected();
                        return true;
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
                        foreach(RenderingListener listener in listenersSet)
                        {
                            listener.Clear();
                        }
                    }

                    /// <summary>
                    ///   This method is invoked by the related inventory management strategy holder
                    ///     and will delegate everything in the underlying listeners: updating
                    ///     a stack.
                    /// </summary>
                    protected override void StackWasUpdated(object containerPosition, int stackPosition, Item item, object quantity)
                    {
                        foreach (RenderingListener listener in listenersSet)
                        {
                            ItemIconTextRenderingStrategy strategy = item.GetRenderingStrategy<ItemIconTextRenderingStrategy>();
                            listener.UpdateStack(stackPosition, item, quantity);
                        }
                    }

                    /// <summary>
                    ///   This method is invoked by the related inventory management strategy holder
                    ///     and will delegate everything in the underlying listeners: removing a
                    ///     stack.
                    /// </summary>
                    protected override void StackWasRemoved(object containerPosition, int stackPosition)
                    {
                        foreach (RenderingListener listener in listenersSet)
                        {
                            listener.RemoveStack(stackPosition);
                        }
                    }
                }
            }
        }
    }
}
