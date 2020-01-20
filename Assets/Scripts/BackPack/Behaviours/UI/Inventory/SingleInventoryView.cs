using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BackPack
{
    namespace Behaviours
    {
        namespace UI
        {
            namespace Inventory
            {
				using Behaviours.Inventory.Single;

                /// <summary>
                ///   Single views are a subclass of <see cref="InventorySingleRenderingManagementStrategy.SingleInventorySubRenderer"/>
                ///     that account for an internal array of items being visible: such items will be cleared or set (according to what actually
                ///     happens in the sub-renderer and renderer in general)
                /// </summary>
                [RequireComponent(typeof(Image))]
                public class SingleInventoryView : InventorySingleRenderingManagementStrategy.SingleInventorySubRenderer
                {
                    /// <summary>
                    ///   An UI item that will know how to render and clear itself according to "simple" data.
                    /// </summary>
                    [RequireComponent(typeof(Image))]
                    public abstract class SingleInventoryViewItem : MonoBehaviour
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
                    ///     any behaviour being subclass of <see cref="SingleInventoryViewItem"/>.
                    /// </summary>
                    public class NoSingleInventoryViewItemException : Support.Types.Exception
                    {
                        public NoSingleInventoryViewItemException(string message) : base(message) {}
                    }

					protected SingleInventoryViewItem[] items;

                    protected virtual void Awake()
                    {
                        /**
                         * Get the slots from the children elements. Require at least one children.
                         */

						items = GetComponentsInChildren<SingleInventoryViewItem>();
                        PageSize = (uint)items.Length;
                        if (PageSize == 0)
                        {
                            Destroy(gameObject);
                            throw new NoSingleInventoryViewItemException("At least one object must have a component of type SingleInventoryViewItem (a descendant of)");
                        }
                    }

                    /// <summary>
                    ///   Forces every <see cref="SingleInventoryViewItem"/> to <see cref="SingleInventoryViewItem.Clear"/> themselves.
                    /// </summary>
                    public override void Clear()
                    {
						foreach(SingleInventoryViewItem item in items)
                        {
                            item.Clear();
                        }
                    }

                    /// <summary>
                    ///   Delegates the behaviour in the <see cref="SingleInventoryViewItem"/> in the given slot by calling <see cref="SingleInventoryViewItem.Clear"/>.
                    /// </summary>
                    /// <param name="slot"></param>
                    protected override void ClearStack(int slot)
                    {
                        items[slot].Clear();
                    }

                    /// <summary>
                    ///   Delegates the behaviour in the <see cref="SingleInventoryViewItem"/> in the given slot by calling <see cref="SingleInventoryViewItem.Set(int, Sprite, string, object)"/>.
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
