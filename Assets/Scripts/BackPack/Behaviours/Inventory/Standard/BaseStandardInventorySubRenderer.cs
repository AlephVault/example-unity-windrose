using System;
using UnityEngine;

namespace BackPack
{
    namespace Behaviours
    {
        namespace Inventory
        {
            namespace Standard
            {
                using GMM.Utils;
                using System.Collections.Generic;
                using System.Linq;

                /// <summary>
                ///   A sub-renderer is, basically, a view than can be connected
                ///     to an <see cref="InventoryStandardRenderingManagementStrategy"/>.
                ///   It is not just a way to render items, but also a way to
                ///     interact with them by -e.g.- pagination: different sub
                ///     renderers may show different pages, but they will render
                ///     the same underlying items.
                /// </summary>
                public abstract class BaseStandardInventorySubRenderer : MonoBehaviour, InventoryStandardRenderingManagementStrategy.InventorySubRenderer
                {
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
                    public void Refresh()
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
                                SetStack(pair.Key, pair.Key, pair.Value.Item1, pair.Value.Item2, pair.Value.Item3);
                            }
                        }
                        else
                        {
                            uint offset = Offset;
                            for (int slot = 0; slot < PageSize; slot++)
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
                                    SetStack(slot, position, element.Item1, element.Item2, element.Item3);
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
                    protected virtual void AfterRefresh() { }

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
                    public virtual void Connected()
                    {
                        // After a renderer was connected, clean and refresh everything inside.
                        if (elements != null)
                        {
                            elements.Clear();
                        }
                        else
                        {
                            elements = new SortedDictionary<int, Tuple<Sprite, string, object>>();
                        }
                    }

                    /// <summary>
                    ///   This method will never be used directly, but it is a callback that will clear everything
                    ///     because it will be disconnected from its former rendering strategy. Although this logic
                    ///     may be overridden, it is needed a call to <c>base.Connected</c> somewhere in the code.
                    /// </summary>
                    public virtual void Disconnected()
                    {
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
            }
        }
    }
}
