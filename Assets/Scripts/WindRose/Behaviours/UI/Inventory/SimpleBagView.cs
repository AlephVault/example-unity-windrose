using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace WindRose
{
    namespace Behaviours
    {
        namespace UI
        {
            namespace Inventory
            {
                using Entities.Objects.Bags;

                /// <summary>
                ///   Simple bag views are a subclass of <see cref="InventorySimpleBagRenderingManagementStrategy.SimpleBagInventorySubRenderer"/>
                ///     that account for an internal array of items being visible: such items will be cleared or set (according to what actually
                ///     happens in the sub-renderer and renderer in general)
                /// </summary>
                [RequireComponent(typeof(Image))]
                public class SimpleBagView : InventorySimpleBagRenderingManagementStrategy.SimpleBagInventorySubRenderer
                {
                    /// <summary>
                    ///   An UI item that will know how to render and clear itself according to "simple" data.
                    /// </summary>
                    [RequireComponent(typeof(Image))]
                    public abstract class SimpleBagViewItem : MonoBehaviour
                    {
                        /**
                         * This class is the renderer of each item. Rendering an item like this
                         *   requires another Panel component (i.e. another image). This
                         *   element is direct child of SimpleBagView in the components
                         *   hierarchy on the scene.
                         */

                        public abstract void Clear();
                        public abstract void Set(int position, Sprite icon, string caption, object quantity);
                    }

                    /// <summary>
                    ///   Tells when this UI object cannot find, among its descendants,
                    ///     any behaviour being subclass of <see cref="SimpleBagViewItem"/>.
                    /// </summary>
                    public class NoSimpleBagViewItemException : Types.Exception
                    {
                        public NoSimpleBagViewItemException(string message) : base(message) {}
                    }

                    protected SimpleBagViewItem[] items;

                    protected virtual void Awake()
                    {
                        /**
                         * Get the slots from the children elements. Require at least one children.
                         */

                        items = GetComponentsInChildren<SimpleBagViewItem>();
                        PageSize = (uint)items.Length;
                        if (PageSize == 0)
                        {
                            Destroy(gameObject);
                            throw new NoSimpleBagViewItemException("At least one object must have a component of type SimpleBagViewItem (a descendant of)");
                        }
                    }

                    /// <summary>
                    ///   Forces every <see cref="SimpleBagViewItem"/> to <see cref="SimpleBagViewItem.Clear"/> themselves.
                    /// </summary>
                    public override void Clear()
                    {
                        foreach(SimpleBagViewItem item in items)
                        {
                            item.Clear();
                        }
                    }

                    /// <summary>
                    ///   Delegates the behaviour in the <see cref="SimpleBagViewItem"/> in the given slot by calling <see cref="SimpleBagViewItem.Clear"/>.
                    /// </summary>
                    /// <param name="slot"></param>
                    protected override void ClearStack(int slot)
                    {
                        items[slot].Clear();
                    }

                    /// <summary>
                    ///   Delegates the behaviour in the <see cref="SimpleBagViewItem"/> in the given slot by calling <see cref="SimpleBagViewItem.Set(int, Sprite, string, object)"/>.
                    /// </summary>
                    protected override void SetStack(int slot, int position, Sprite icon, string caption, object quantity)
                    {
                        items[slot].Set(position, icon, caption, quantity);
                    }

                    // Remember: AfterRefresh() is a method that can be overriden.
                }
            }
        }
    }
}
