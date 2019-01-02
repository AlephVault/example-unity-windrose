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
                using Objects.Bags;

                [RequireComponent(typeof(Image))]
                public class SimpleBagView : InventorySimpleBagRenderingManagementStrategy.SimpleBagInventorySubRenderer
                {
                    /**
                     * This component has sub-items that know how to render themselves.
                     * It is an error to not have appropriate children to render the content.
                     * 
                     * This means: this component will be PAGINATED, and pagination will be
                     *   set on Awake().
                     *   
                     * This inventory view will be mounted on a Panel component (basically,
                     *   a Panel is an image) inside a Canvas.
                     */

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

                    /**
                     * Clearing this involves clearing each slot. Also, setting the page to 0.
                     */
                    public override void Clear()
                    {
                        foreach(SimpleBagViewItem item in items)
                        {
                            item.Clear();
                        }
                    }

                    /**
                     * Delegating call to ClearStack on the appropriate slot.
                     */
                    protected override void ClearStack(int slot)
                    {
                        items[slot].Clear();
                    }

                    /**
                     * Delegating call to SetStack on the appropriate slot.
                     */
                    protected override void SetStack(int slot, int position, Sprite icon, string caption, object quantity)
                    {
                        Debug.Log(string.Format("Setting slot {0} to position {1}, icon {2}, caption '{3}' and quantity {4}", slot, position, icon, caption, quantity));
                        items[slot].Set(position, icon, caption, quantity);
                    }

                    // Remember: AfterRefresh() is a method that can be overriden.
                }
            }
        }
    }
}
