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
			using ManagementStrategies.RenderingStrategies;

            namespace Single
            {
                /// <summary>
                ///   This is a rendering strategy for <see cref="SingleSimpleInventory"/>
				///     behaviours. This strategy will allow the connection of several
				///     objects acting as "viewers" (<see cref="SingleInventorySubRenderer"/>).
                /// </summary>
                public class InventorySingleSimpleRenderingManagementStrategy : InventorySimpleRenderingManagementStrategy
                {
                    /// <summary>
                    ///   A sub-renderer is, basically, a view than can be connected
                    ///     to an <see cref="InventorySingleSimpleRenderingManagementStrategy"/>.
                    ///   It is not just a way to render items, but also a way to
                    ///     interact with them by -e.g.- pagination: different sub
                    ///     renderers may show different pages, but they will render
                    ///     the same underlying items.
                    /// </summary>
                    public abstract class SingleInventorySubRenderer : MonoBehaviour
                    {
                        /**
                         * This is a reference to the only rendering strategy one of this
                         *   subrenderers can be bound to. If you attach this component to
                         *   another renderer, the former renderer will disconnect from
                         *   this object.
                         */
                        private InventorySingleSimpleRenderingManagementStrategy sourceRenderer;

                        /// <summary>
                        ///   Contains the elements to render, in terms of its position
                        ///     and the simple data fields: icon, caption, and quantity.
                        /// </summary>
                        protected SortedDictionary<int, Tuple<Sprite, string, object>> elements;

                        /**
                         * Paging will imply two properties: PageSize and Page. Both properties
                         *   are unsigned integers:
                         * - Page size may be a positive value, or 0. If 0, no pagination will
                         *     be applied: all the elements will be rendered. Otherwise, only
                         *     a subset of N elements will be displayed at once.
                         * - Page makes sense when Page size is >= 0. Otherwise, page is 0.
                         *   The page value will determine an offset of: N*PageSize elements.
                         * - There is a protected method if you want to change the paging later.
                         */

                        /// <summary>
                        ///   Returns the underlying simple simple inventory (which is tied to
                        ///     the related renderer).
                        /// </summary>
                        public SingleSimpleInventory SourceSingleInventory
                        {
							get { return sourceRenderer != null ? sourceRenderer.SingleInventory : null; }
                        }

                        /// <summary>
                        ///   This value will be 0 if no paging is meant to be used in this
                        ///     view. Otherwise, it will be a >= number.
                        /// </summary>
                        public uint PageSize { get; protected set; }

                        /// <summary>
                        ///   This is the current page. If <see cref="PageSize"/> is zero, this
                        ///     value will be zero. Otherwise, this value will be multiplied
                        ///     by <see cref="PageSize"/> to get the current offset of elements
                        ///     to render.
                        /// </summary>
                        public uint Page { get; protected set; }

                        /// <summary>
                        ///   Tells whether this sub-renderer applies pagination. This will happen
                        ///     when <see cref="PageSize"/> is > 0.
                        /// </summary>
                        public bool Paginates { get { return PageSize > 0; } }

                        /// <summary>
                        ///   The actual offset of elements to render, as multiplication of
                        ///     <see cref="Page"/> and <see cref="PageSize"/>.
                        /// </summary>
                        public uint Offset { get { return PageSize * Page; } }

                        /// <summary>
                        ///   Returns the maximum available page to render, given the current
                        ///     <see cref="PageSize"/>.
                        /// </summary>
                        public uint MaxPage()
                        {
                            if (PageSize == 0) return 0;
                            if (elements.Count == 0) return 0;
                            return (uint)elements.Last().Key / PageSize;
                        }

                        /**
                         * Calculates the page on which you'll find a particular position.
                         */

                        /// <summary>
                        ///   Calculates the page -considering current page settings-
                        ///     for a particular position in the inventory.
                        /// </summary>
                        public uint PageFor(int position)
                        {
                            return PageSize == 0 ? 0 : (uint)position / PageSize;
                        }

                        /**
                         * Changes the page size and updates the page accordingly.
                         */

                        /// <summary>
                        ///   Changes the current <see cref="PageSize"/> and will
                        ///     also set the <see cref="Page"/> accordingly.
                        /// </summary>
                        protected void ChangePageSize(uint newPageSize)
                        {
                            if (PageSize == newPageSize) return;

                            if (newPageSize == 0)
                            {
                                PageSize = 0;
                                Page = 0;
                            }
                            else
                            {
                                uint offset = PageSize * Page;
                                PageSize = newPageSize;
                                Page = offset / PageSize;
                            }

                            Refresh();
                        }

                        /// <summary>
                        ///   Refreshes the content being rendered. This involves clearing everything,
                        ///     rendering each stack, and then applying a final rendering. These two
                        ///     steps are abstract and must be implemented by subclasses (since it
                        ///     is just a matter of the particular UI to create for them).
                        /// </summary>
                        public virtual void Refresh()
                        {
                            if (PageSize == 0)
                            {
                                Clear();
                                foreach (KeyValuePair<int, Tuple<Sprite, string, object>> pair in elements)
                                {
                                    /**
                                     * In these listings, position will match the slot because you are
                                     *   rendering everything (so the match will be automatic here).
                                     */
                                    SetStack(pair.Key, pair.Key, pair.Value.First, pair.Value.Second, pair.Value.Third);
                                }
                            }
                            else
                            {
                                uint offset = Offset;
                                for(int slot = 0; slot < PageSize; slot++)
                                {
                                    /**
                                     * Slot and key will differ - exactly by the amount of `offset`.
                                     * So we try getting an element by a calculated key here by adding
                                     *   slot to offset.
                                     */
                                    int position = (int)(slot + offset);
                                    Tuple<Sprite, string, object> element;
                                    if (elements.TryGetValue(position, out element))
                                    {
                                        SetStack(slot, position, element.First, element.Second, element.Third);
                                    }
                                    else
                                    {
                                        ClearStack(slot);
                                    }
                                }
                            }
                            AfterRefresh();
                        }

                        /// <summary>
                        ///   This method must be implemented. It must clear everything accordingly: stacks
                        ///     and whatever the UI needs to clear.
                        /// </summary>
                        public abstract void Clear();

                        /// <summary>
                        ///   This method must be implemented. It draws a particular stack in a particular
                        ///     slot for a particular original position.
                        /// </summary>
                        /// <param name="slot">The slot to render into. It will be constrained by <see cref="PageSize"/></param>
                        /// <param name="position">The source position</param>
                        /// <param name="icon">The stack's icon</param>
                        /// <param name="caption">The stack's caption</param>
                        /// <param name="quantity">The stackc's quantity</param>
                        protected abstract void SetStack(int slot, int position, Sprite icon, string caption, object quantity);

                        /// <summary>
                        ///   Clears a particular slot. No stack will be rendered there.
                        /// </summary>
                        /// <param name="slot">The slot to clear. It will be constrained by <see cref="PageSize"/></param>
                        protected abstract void ClearStack(int slot);

                        /// <summary>
                        ///   Additional custom logic that may be implemented to apply after refreshing
                        ///     our rendering.
                        /// </summary>
                        protected virtual void AfterRefresh() {}

                        /**
                         * This callback tells what happens when this sub-renderer is connected to a management
                         *   rendering strategy.
                         * 
                         * You can override it but, if you do, ensure you call base.Connected(sbRenderer) somewhere.
                         */

                        /// <summary>
                        ///   This method will never be used directly, but it is a callback that will clear everything
                        ///     and refresh again but according the new rendering strategy being attached (connected)
                        ///     to. Although this logic may be overridden, it is needed a call to <c>base.Connected</c>
                        ///     somewhere in the code.
                        /// </summary>
                        public virtual void Connected(InventorySingleSimpleRenderingManagementStrategy sbRenderer)
                        {
                            if (sourceRenderer != null)
                            {
                                sourceRenderer.RemoveSubRenderer(this);
                            }
                            sourceRenderer = sbRenderer;

                            // After a renderer was connected, clean and refresh everything inside.
                            if (elements != null)
                            {
                                elements.Clear();
                            }
                            else
                            {
                                elements = new SortedDictionary<int, Tuple<Sprite, string, object>>();
                            }
							IEnumerable<Tuple<int, BackPack.Types.Inventory.Stacks.Stack>> pairs = sourceRenderer.SingleInventory.StackPairs();
							foreach(Tuple<int, BackPack.Types.Inventory.Stacks.Stack> pair in pairs)
                            {
                                Dictionary<string, object> target = new Dictionary<string, object>();
                                pair.Second.MainRenderingStrategy.DumpRenderingData(target);
                                elements.Add(pair.First, new Tuple<Sprite, string, object>((Sprite)target["icon"], (string)target["caption"], target["quantity"]));
                            }
                            Refresh();
                        }

                        /// <summary>
                        ///   This method will never be used directly, but it is a callback that will clear everything
                        ///     because it will be disconnected from its former rendering strategy. Although this logic
                        ///     may be overridden, it is needed a call to <c>base.Connected</c> somewhere in the code.
                        /// </summary>
                        public virtual void Disconnected()
                        {
                            sourceRenderer = null;
                            Clear();
                        }

                        /**
                         * Calculates the slot to match the position, according to current page and size.
                         * Returns -1 if that slot is not visible for the current page/size.
                         * Otherwise, returns a number between 0 and (PageSize - 1), or returns the
                         *   input position as the slot if PageSize = 0;
                         */

                        /// <summary>
                        ///   Returns the slot index to use for certain visible position (according to
                        ///     paging settings).
                        /// </summary>
                        /// <returns>
                        ///   If the given position is not meant to be visible, it returns -1. Otherwise,
                        ///   it returns the index between 0 and <see cref="PageSize"/> -1, or the same
                        ///   position if <see cref="PageSize"/> is 0
                        /// </returns>
                        protected int SlotFor(int position)
                        {
                            if (PageSize == 0)
                            {
                                return position;
                            }

                            int offset = (int)Offset;
                            if (offset <= position && position < offset + PageSize)
                            {
                                return position - offset;
                            }

                            return -1;
                        }

                        /// <summary>
                        ///   Updates a single stack position. Intended to be called by the rendering 
                        ///     management strategy, this method will account only for visible items.
                        ///     See <see cref="SetStack(int, int, Sprite, string, object)"/> for more
                        ///     details.
                        /// </summary>
                        /// <param name="position">The position to update its data</param>
                        /// <param name="icon">The stack's icon</param>
                        /// <param name="caption">The stack's caption</param>
                        /// <param name="quantity">The stack's quantity</param>
                        public void UpdateStack(int position, Sprite icon, string caption, object quantity)
                        {
                            elements[position] = new Tuple<Sprite, string, object>(icon, caption, quantity);
                            int slot = SlotFor(position);
                            if (slot != -1)
                            {
                                SetStack(slot, position, icon, caption, quantity);
                                AfterRefresh();
                            }
                        }

                        /// <summary>
                        ///   Removes a single stack position. Intended to be called by the rendering
                        ///     management strategy, this method will account only for visible items.
                        ///     See <see cref="ClearStack(int)"/> for more details.
                        /// </summary>
                        /// <param name="position">The position to clear its data</param>
                        public void RemoveStack(int position)
                        {
                            elements.Remove(position);
                            int slot = SlotFor(position);
                            if (slot != -1)
                            {
                                ClearStack(slot);
                                AfterRefresh();
                            }
                        }

                        /**
                         * Clamps the page number. Returns whether the page number was
                         *   weird and was clamped, or not.
                         */
                        private bool ClampPage()
                        {
                            uint clampedPage = Values.Clamp(0, Page, MaxPage());
                            bool wasUnclamped = clampedPage != Page;
                            Page = clampedPage;
                            return wasUnclamped;
                        }

                        /// <summary>
                        ///   Moves one page forward, and updates the content accordingly. This method is meant
                        ///     to be invoked by the UI.
                        /// </summary>
                        /// <param name="justTest">
                        ///   If <c>true</c>, it doesn't actually perform the move but just tells whether it can
                        ///   move or not
                        /// </param>
                        /// <returns>Whether it could move</returns>
                        public bool Next(bool justTest = false)
                        {
                            bool wasUnclamped = ClampPage();
                            bool canIncrement = Page < MaxPage();

                            if (!justTest)
                            {
                                if (canIncrement)
                                {
                                    Page++;
                                }

                                if (canIncrement || wasUnclamped) Refresh();
                            }

                            return canIncrement;
                        }

                        /// <summary>
                        ///   Moves one page backward, and updates the content accordingly. This method is meant
                        ///     to be invoked by the UI.
                        /// </summary>
                        /// <param name="justTest">
                        ///   If <c>true</c>, it doesn't actually perform the move but just tells whether it can
                        ///   move or not
                        /// </param>
                        /// <returns>Whether it could move</returns>
                        public bool Prev(bool justTest = false)
                        {
                            bool wasUnclamped = ClampPage();
                            bool canDecrement = Page > 0;

                            if (!justTest)
                            {
                                if (canDecrement)
                                {
                                    Page--;
                                }

                                if (canDecrement || wasUnclamped) Refresh();
                            }

                            return canDecrement;
                        }

                        /// <summary>
                        ///   Chooses another page to render. Refreshes everything on success.
                        /// </summary>
                        /// <param name="page">
                        ///   The new page, which will be clamped between 0 and the maximum
                        ///   page to render as per the paging settings
                        /// </param>
                        /// <returns>Whether it changed the page, or is rendering the same one</returns>
                        public bool Go(uint page)
                        {
                            page = Values.Clamp(0, page, MaxPage());
                            bool wasUnclamped = ClampPage();
                            bool shouldChange = page != Page;

                            if (shouldChange)
                            {
                                Page = page;
                                Refresh();
                            }
                            else if (wasUnclamped)
                            {
                                Refresh();
                            }

                            return shouldChange;
                        }
                    }

                    /// <summary>
					///   Tells when trying to add a null <see cref="SingleInventorySubRenderer"/>
					///     when calling <see cref="AddSubRenderer(SingleInventorySubRenderer)"/>.
                    /// </summary>
                    public class InvalidSubRendererException : GMM.Types.Exception
                    {
                        public InvalidSubRendererException(string message) : base(message) { }
                    }

                    /// <summary>
					///   The initial list of <see cref="SingleInventorySubRenderer"/> instances
                    ///     to add to this rendering strategy.
                    /// </summary>
                    [SerializeField]
                    private List<SingleInventorySubRenderer> subRenderers = new List<SingleInventorySubRenderer>();
                    private HashSet<SingleInventorySubRenderer> subRenderersSet = new HashSet<SingleInventorySubRenderer>();

                    /// <summary>
                    ///   The <see cref="SingleSimpleInventory"/> this strategy is linked to.
                    /// </summary>
                    public SingleSimpleInventory SingleInventory
                    {
                        get; private set;
                    }

                    /// <summary>
                    ///   The max size of the container in the <see cref="SingleSimpleInventory"/>. This size will
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
						SingleInventory = GetComponent<SingleSimpleInventory>();
                    }

                    void Start()
                    {
                        foreach(SingleInventorySubRenderer subRenderer in subRenderers)
                        {
                            if (subRenderer == null) continue;
                            subRenderersSet.Add(subRenderer);
                            subRenderer.Connected(this);
                        }
                    }

                    void OnDestroy()
                    {
                        HashSet<SingleInventorySubRenderer> cloned = new HashSet<SingleInventorySubRenderer>(subRenderersSet);
                        subRenderersSet.Clear();
                        foreach (SingleInventorySubRenderer subRenderer in cloned)
                        {
                            subRenderer.Disconnected();
                        }
                    }

                    /// <summary>
                    ///   Adds a sub-renderer to this rendering management strategy. The sub-renderer will
                    ///     refresh with this renderer's data accordingly, and will be synchronized until
					///     it is removed by a call to <see cref="RemoveSubRenderer(SingleInventorySubRenderer)"/>.
                    /// </summary>
					/// <param name="subRenderer">The <see cref="SingleInventorySubRenderer"/> to add</param>
                    /// <returns>Whether it could be added, or it was already added</returns>
                    public bool AddSubRenderer(SingleInventorySubRenderer subRenderer)
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
                        subRenderer.Connected(this);
                        return true;
                    }

                    /// <summary>
                    ///   Removes a sub-renderer from this rendering management strategy. The sub-renderer will
                    ///     be cleared and removed.
                    /// </summary>
					/// <param name="subRenderer">The <see cref="SingleInventorySubRenderer"/> to remove</param>
                    /// <returns>Whether it could be removed, or it wasn't connected here on first place</returns>
                    public bool RemoveSubRenderer(SingleInventorySubRenderer subRenderer)
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
                        foreach(SingleInventorySubRenderer subRenderer in subRenderersSet)
                        {
                            subRenderer.Clear();
                        }
                    }

                    /// <summary>
                    ///   This method is invoked by the related inventory management strategy holder
                    ///     and will delegate everything in the underlying sub-renderers: updating
                    ///     a stack.
                    /// </summary>
                    protected override void StackWasUpdated(object containerPosition, int stackPosition, Sprite icon, string caption, object quantity)
                    {
                        foreach (SingleInventorySubRenderer subRenderer in subRenderersSet)
                        {
                            subRenderer.UpdateStack(stackPosition, icon, caption, quantity);
                        }
                    }

                    /// <summary>
                    ///   This method is invoked by the related inventory management strategy holder
                    ///     and will delegate everything in the underlying sub-renderers: removing
                    ///     a stack.
                    /// </summary>
                    protected override void StackWasRemoved(object containerPosition, int stackPosition)
                    {
                        foreach (SingleInventorySubRenderer subRenderer in subRenderersSet)
                        {
                            subRenderer.RemoveStack(stackPosition);
                        }
                    }
                }
            }
        }
    }
}
