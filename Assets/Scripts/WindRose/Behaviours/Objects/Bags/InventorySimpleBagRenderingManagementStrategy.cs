using Support.Types;
using Support.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using WindRose.Behaviours.Inventory;
using WindRose.Behaviours.Inventory.ManagementStrategies.RenderingStrategies;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            namespace Bags
            {
                [RequireComponent(typeof(SimpleBag))]
                public class InventorySimpleBagRenderingManagementStrategy : InventorySimpleRenderingManagementStrategy
                {
                    /**
                     * This rendering strategy renders a simple bag. A simple bag will have
                     *   just one position, but will have perhaps multiple "viewers". THESE
                     *   CLASSES ARE NOT JUST RENDERERS: THEY ARE INTENDED TO WORK AS THE UI
                     *   OF THE INVENTORY BEING DISPLAYED.
                     * 
                     * Sub-renderers may imply pagination. This means: Pagination can be
                     *   configured into them via several means. Always considering:
                     *   1. Page Size of 0 means no pagination.
                     *   2. Page Offset of N will mean an actual offset of N * (Page Size).
                     * 
                     * Paging *may* change later (if child components are defined so), but
                     *   there is no guarantee here.
                     */
                    public abstract class SimpleBagInventorySubRenderer : MonoBehaviour
                    {
                        /**
                         * This is a reference to the only rendering strategy one of this
                         *   subrenderers can be bound to. If you attach this component to
                         *   another renderer, the former renderer will disconnect from
                         *   this object.
                         */
                        private InventorySimpleBagRenderingManagementStrategy sourceRenderer;

                        /**
                         * To make this work, the sub-renderers will need to know the data to
                         *   render.
                         */
                        private SortedDictionary<int, Tuple<Sprite, string, object>> elements;

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
                        public SimpleBag SourceSimpleBag
                        {
                            get { return sourceRenderer != null ? sourceRenderer.SimpleBag : null; }
                        }
                        public uint PageSize { get; protected set; }
                        public uint Page { get; protected set; }
                        public bool Paginates { get { return PageSize > 0; } }
                        public uint Offset { get { return PageSize * Page; } }
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

                        /**
                         * This method refreshes (actually: re-renders) all the elements being
                         *   held as data. In order to this method work appropriately, data must
                         *   be up-to-date in `elements` collection.
                         * 
                         * When there is no paging, everything is being rendered. In this case,
                         *   refreshing involves:
                         * 1. Clearing everything, because you don't know the capacity beforehand.
                         * 2. Iterating over each element, and refreshing it.
                         * 
                         * A different case is when you have a paging: You know the capacity, so
                         *   you will know how to treat each element in the capacity (e.g. inventory
                         *   slots in regular layouts).
                         */
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
                                uint limit = offset + PageSize;
                                for(int slot = 0; slot <= PageSize; slot++)
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
                            RefreshExtra();
                        }

                        /**
                         * Clears all the displays. Useful when you have been commanded to clear your display,
                         *   or when you are refreshing everything in a non-paginated layout.
                         */
                        public abstract void Clear();

                        /**
                         * Renders a particular stack (by its data) element inside a particular slot (by its index).
                         */
                        protected abstract void SetStack(int slot, int position, Sprite icon, string caption, object quantity);

                        /**
                         * Clears a particular slot (by its index) - no stack will be displayed there.
                         */
                        protected abstract void ClearStack(int slot);

                        /**
                         * Sets more data you'd like (e.g. page number).
                         */
                        protected virtual void RefreshExtra() {}

                        /**
                         * This callback tells what happens when this sub-renderer is connected to a management
                         *   rendering strategy.
                         * 
                         * You can override it but, if you do, ensure you call base.Connected(sbRenderer) somewhere.
                         */
                        public virtual void Connected(InventorySimpleBagRenderingManagementStrategy sbRenderer)
                        {
                            if (sourceRenderer != null)
                            {
                                sourceRenderer.RemoveSubRenderer(this);
                            }
                            sourceRenderer = sbRenderer;
                        }

                        /**
                         * This callback tells what happens when this sub-renderer is disconnected from the management
                         *   strategy.
                         * 
                         * You can override it but, if you do, ensure toy call base.Disconnected() somewhere.
                         */
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
                        private int SlotFor(int position)
                        {
                            if (PageSize == 0)
                            {
                                return position;
                            }

                            int offset = (int)Offset;
                            if (offset <= position && position <= offset + PageSize)
                            {
                                return position - offset;
                            }

                            return -1;
                        }

                        /**
                         * Updates the content of a stack (i.e. updates rendering value for a position).
                         */
                        public void UpdateStack(int position, Sprite icon, string caption, object quantity)
                        {
                            elements[position] = new Tuple<Sprite, string, object>(icon, caption, quantity);
                            int slot = SlotFor(position);
                            if (slot != -1) SetStack(slot, position, icon, caption, quantity);
                        }

                        /**
                         * Removes the content of a stack (i.e. clear rendering value for a position).
                         */
                        public void RemoveStack(int position)
                        {
                            elements.Remove(position);
                            int slot = SlotFor(position);
                            if (slot != -1) ClearStack(slot);
                        }

                        /**
                         * Calculates the maximum allowed page to display.
                         * It will be 0 for infinite, but may be nonzero for regular paging.
                         */
                        private uint MaxPage()
                        {
                            if (PageSize == 0) return 0;

                            return (uint)elements.Last().Key / PageSize;
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

                        /**
                         * Moves to the next page (and refreshes).
                         * If the second argument is true, the move is not actually
                         *   performed, but just the test whether it could move.
                         */
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

                        /**
                         * Goes to the previous page (and refreshes).
                         * If the second argument is true, the move is not actually
                         *   performed, but just the test whether it could move.
                         */
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

                        /**
                         * This method is quite different: it goes to another page.
                         * If the page is lower than 0 or greater than MaxPage, it
                         *   is clamped to that value.
                         * Returns whether the change occurs or not (perhaps even the
                         *   same page is refreshed).
                         */
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

                    /**
                     * Exception to tell when the sub-renderer is null.
                     */
                    public class InvalidSubRendererException : Types.Exception
                    {
                        public InvalidSubRendererException(string message) : base(message) { }
                    }

                    /**
                     * This renderer will have sub-renderers for it to work appropriately: inventory may be
                     *   watched from different simultaneous sides. Those sides will be instance of this new
                     *   subclass: SimpleBagInventorySubRenderer.
                     */
                    [SerializeField]
                    private List<SimpleBagInventorySubRenderer> subRenderers = new List<SimpleBagInventorySubRenderer>();
                    private HashSet<SimpleBagInventorySubRenderer> subRenderersSet = new HashSet<SimpleBagInventorySubRenderer>();

                    public SimpleBag SimpleBag
                    {
                        get; private set;
                    }

                    /**
                     * This class will account for max size, since it will be related to a Simple Spatial
                     *   Strategy that could either be finite (size > 0) or infinite (size == 0). However,
                     *   while this renderer is not attached to any object (which will occur for a split
                     *   second, say), the max size is -1.
                     */
                    public int MaxSize
                    {
                        get; private set;
                    }

                    void Awake()
                    {
                        MaxSize = spatialStrategy.GetSize();
                        SimpleBag = GetComponent<SimpleBag>();
                    }

                    void Start()
                    {
                        foreach(SimpleBagInventorySubRenderer subRenderer in subRenderers)
                        {
                            if (subRenderer == null) continue;
                            subRenderersSet.Add(subRenderer);
                            subRenderer.Connected(this);
                        }
                    }

                    void OnDestroy()
                    {
                        foreach (SimpleBagInventorySubRenderer subRenderer in subRenderersSet)
                        {
                            subRenderersSet.Remove(subRenderer);
                            subRenderer.Disconnected();
                        }
                    }

                    /**
                     * Two methods here to handle the different sub-renderers. Remember:
                     *   one sub-renderer will only watch one single simple bag at a time.
                     */

                    public bool AddSubRenderer(SimpleBagInventorySubRenderer subRenderer)
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

                    public bool RemoveSubRenderer(SimpleBagInventorySubRenderer subRenderer)
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

                    public override void EverythingWasCleared()
                    {
                        foreach(SimpleBagInventorySubRenderer subRenderer in subRenderersSet)
                        {
                            subRenderer.Clear();
                        }
                    }

                    protected override void StackWasUpdated(object containerPosition, int stackPosition, Sprite icon, string caption, object quantity)
                    {
                        foreach (SimpleBagInventorySubRenderer subRenderer in subRenderersSet)
                        {
                            subRenderer.UpdateStack(stackPosition, icon, caption, quantity);
                        }
                    }

                    protected override void StackWasRemoved(object containerPosition, int stackPosition)
                    {
                        foreach (SimpleBagInventorySubRenderer subRenderer in subRenderersSet)
                        {
                            subRenderer.RemoveStack(stackPosition);
                        }
                    }
                }
            }
        }
    }
}
