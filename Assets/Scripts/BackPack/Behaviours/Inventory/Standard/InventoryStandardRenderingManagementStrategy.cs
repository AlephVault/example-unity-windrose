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
				///     objects acting as "viewers" (<see cref="InventorySubRenderer"/>).
                /// </summary>
                public class InventoryStandardRenderingManagementStrategy : Inventory1DIndexedStaticRenderingManagementStrategy
                {
                    /// <summary>
                    ///   Sub-renderers will refresh icon and text for the stack, and will account for
                    ///     a single container and an 1D-indexed intra-container positioning.
                    /// </summary>
                    public interface InventorySubRenderer
                    {
                        void Connected();
                        void UpdateStack(int stackPosition, Sprite icon, string caption, object quantity);
                        void RemoveStack(int position);
                        void Clear();
                        void Disconnected();
                    }

                    /// <summary>
					///   Tells when trying to add a null <see cref="InventorySubRenderer"/>
					///     when calling <see cref="AddSubRenderer(InventorySubRenderer)"/>.
                    /// </summary>
                    public class InvalidSubRendererException : GMM.Types.Exception
                    {
                        public InvalidSubRendererException(string message) : base(message) { }
                    }

                    // Effective set of the renderers to be used, either by preloading from the editor or by
                    //   adding / removing sub-renderers.
                    private HashSet<InventorySubRenderer> subRenderersSet = new HashSet<InventorySubRenderer>();

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
                        HashSet<InventorySubRenderer> cloned = new HashSet<InventorySubRenderer>(subRenderersSet);
                        subRenderersSet.Clear();
                        foreach (InventorySubRenderer subRenderer in cloned)
                        {
                            subRenderer.Disconnected();
                        }
                    }

                    // Clears and fully updates a given sub-renderer.
                    private void FullUpdate(InventorySubRenderer subRenderer)
                    {
                        subRenderer.Clear();
                        IEnumerable<Tuple<int, Types.Inventory.Stacks.Stack>> pairs = SingleInventory.StackPairs();
                        foreach (Tuple<int, Types.Inventory.Stacks.Stack> pair in pairs)
                        {
                            Dictionary<string, object> target = new Dictionary<string, object>();
                            pair.Item2.MainRenderingStrategy.DumpRenderingData(target);
                            subRenderer.UpdateStack(pair.Item1, (Sprite)target["icon"], (string)target["caption"], target["quantity"]);
                        }
                    }

                    /// <summary>
                    ///   Adds a sub-renderer to this rendering management strategy. The sub-renderer will
                    ///     refresh with this renderer's data accordingly, and will be synchronized until
					///     it is removed by a call to <see cref="RemoveSubRenderer(SingleInventorySubRenderer)"/>.
                    /// </summary>
					/// <param name="subRenderer">The <see cref="InventorySubRenderer"/> to add</param>
                    /// <returns>Whether it could be added, or it was already added</returns>
                    public bool AddSubRenderer(InventorySubRenderer subRenderer)
                    {
                        if (subRenderer == null)
                        {
                            throw new InvalidSubRendererException("Sub-renderer to add cannot be null");
                        }

                        if (subRenderersSet.Contains(subRenderer))
                        {
                            return false;
                        }

                        subRenderersSet.Add(subRenderer);
                        subRenderer.Connected();
                        // We will force the sub-renderer to be cleared, and
                        // also refresh each item. This, to decouple from the
                        // inventory itself.
                        FullUpdate(subRenderer);
                        return true;
                    }

                    /// <summary>
                    ///   Removes a sub-renderer from this rendering management strategy. The sub-renderer will
                    ///     be cleared and removed.
                    /// </summary>
					/// <param name="subRenderer">The <see cref="SingleInventorySubRenderer"/> to remove</param>
                    /// <returns>Whether it could be removed, or it wasn't connected here on first place</returns>
                    public bool RemoveSubRenderer(InventorySubRenderer subRenderer)
                    {
                        if (!subRenderersSet.Contains(subRenderer))
                        {
                            return false;
                        }

                        subRenderersSet.Remove(subRenderer);
                        subRenderer.Disconnected();
                        return true;
                    }

                    /**************************************
                     * Methods to delegate the rendering on the sub-renderers
                     **************************************/

                    /// <summary>
                    ///   This method is invoked by the related inventory management strategy holder
                    ///     and will delegate everything in the underlying sub-renderers: clearing
                    ///     its contents.
                    /// </summary>
                    public override void EverythingWasCleared()
                    {
                        foreach(InventorySubRenderer subRenderer in subRenderersSet)
                        {
                            subRenderer.Clear();
                        }
                    }

                    /// <summary>
                    ///   This method is invoked by the related inventory management strategy holder
                    ///     and will delegate everything in the underlying sub-renderers: updating
                    ///     a stack.
                    /// </summary>
                    protected override void StackWasUpdated(object containerPosition, int stackPosition, Item item, object quantity)
                    {
                        foreach (InventorySubRenderer subRenderer in subRenderersSet)
                        {
                            ItemIconTextRenderingStrategy strategy = item.GetRenderingStrategy<ItemIconTextRenderingStrategy>();
                            subRenderer.UpdateStack(stackPosition, strategy.Icon, strategy.Caption, quantity);
                        }
                    }

                    /// <summary>
                    ///   This method is invoked by the related inventory management strategy holder
                    ///     and will delegate everything in the underlying sub-renderers: removing
                    ///     a stack.
                    /// </summary>
                    protected override void StackWasRemoved(object containerPosition, int stackPosition)
                    {
                        foreach (InventorySubRenderer subRenderer in subRenderersSet)
                        {
                            subRenderer.RemoveStack(stackPosition);
                        }
                    }
                }
            }
        }
    }
}
