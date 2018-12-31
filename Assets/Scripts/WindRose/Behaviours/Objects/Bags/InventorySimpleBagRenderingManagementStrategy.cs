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
                public class InventorySimpleBagRenderingManagementStrategy : InventorySimpleRenderingManagementStrategy
                {
                    /**
                     * This rendering strategy renders a simple bag. A simple bag will have
                     *   just one position, but will have perhaps multiple "viewers". THESE
                     *   CLASSES ARE NOT JUST RENDERERS: THEY ARE INTENDED TO WORK AS THE UI
                     *   OF THE INVENTORY BEING DISPLAYED.
                     */
                    public abstract class SimpleBagInventorySubRenderer : MonoBehaviour
                    {
                        // The rendering strategy currently bound to
                        private InventorySimpleBagRenderingManagementStrategy parentRenderer;

                        public abstract void Clear();
                        public virtual void Connected(InventorySimpleBagRenderingManagementStrategy sbRenderer)
                        {
                            if (parentRenderer != null)
                            {
                                parentRenderer.RemoveSubRenderer(this);
                            }
                            parentRenderer = sbRenderer;
                            /* PLEASE NOTE: Perhaps you'd like to extend this method - just ensure this one gets called as `base` */
                        }
                        public virtual void Disconnected()
                        {
                            Clear();
                            /* PLEASE NOTE: Perhaps you'd like to extend this method - just ensure this one gets called as `base` */
                        }
                        public abstract void RefreshStack(int stackPosition, Sprite icon, string caption, object quantity);
                        public abstract void RemoveStack(int stackPosition);
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
                            subRenderer.RefreshStack(stackPosition, icon, caption, quantity);
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
