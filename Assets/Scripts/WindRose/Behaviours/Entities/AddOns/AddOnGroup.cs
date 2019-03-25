using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using WindRose.Behaviours.World;
using WindRose.Behaviours.World.Layers.Entities;
using WindRose.Types;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.AddOns
        {
            /// <summary>
            ///   <para>
            ///     Aside from map objects, this is another kind of entity. These objects
            ///       do not live on their own, but depend on a related map object.
            ///   </para>
            ///   <para>
            ///     Each map object will have two of these: one for "upper" and one for
            ///       "lower" add-ons, and these objects will hold several add-ons that,
            ///       in turn, are also ordered appropriately.
            ///   </para>
            /// </summary>
            [RequireComponent(typeof(SortingGroup))]
            public class AddOnGroup : Common.Entity
            {
                /// <summary>
                ///   The object this add-on is related to.
                /// </summary>
                [SerializeField]
                private Objects.Object relatedObject;

                /// <summary>
                ///   See <see cref="relatedObject"/>.
                /// </summary>
                public Objects.Object RelatedObject { get { return relatedObject; } }

                /// <summary>
                ///   The add-on type. Either an overlay or an underlay.
                /// </summary>
                public enum AddOnGroupType { Overlay, Underlay };

                /// <summary>
                ///   See <see cref="AddOnGroupType"/>.
                /// </summary>
                [SerializeField]
                private AddOnGroupType addOnGroupType = AddOnGroupType.Overlay;

                /// <summary>
                ///   The movement is the one in the related object.
                /// </summary>
                public override Direction? Movement { get { return relatedObject.Movement; } }

                /// <summary>
                ///   The parent map is the one in the related object.
                /// </summary>
                public override Map ParentMap { get { return relatedObject.ParentMap; } }

                /// <summary>
                ///   The logical X is the one in the related object.
                /// </summary>
                public override uint X { get { return relatedObject.X; } }

                /// <summary>
                ///   The logical Y is the one in the related object.
                /// </summary>
                public override uint Y { get { return relatedObject.Y; } }

                /* List of internal add-ons */
                private List<AddOn> orderedAddOns = new List<AddOn>();
                private HashSet<AddOn> addOns = new HashSet<AddOn>();

                /// <summary>
                ///   Adds an add-on that is not formerly present, and recomputes the
                ///     sort orders of all the add-ons.
                /// </summary>
                /// <param name="addOn">The add-on to add</param>
                public void Add(AddOn addOn)
                {
                    if (!addOns.Contains(addOn) && addOn)
                    {
                        orderedAddOns.Add(addOn);
                        addOn.transform.parent = transform;
                        addOns.Add(addOn);
                        ComputeAddOnsSortOrders();
                        addOn.Attached(this);
                    }
                }

                /// <summary>
                ///   Adds an add-on that is not formerly present, and recomputes the
                ///     sort orders of all the add-ons. The new add-on is pushed (this
                ///     means: added the closest possible to the main object).
                /// </summary>
                /// <param name="addOn">The add-on to add</param>
                public void Push(AddOn addOn)
                {
                    if (!addOns.Contains(addOn) && addOn)
                    {
                        if (addOnGroupType == AddOnGroupType.Underlay)
                        {
                            orderedAddOns.Add(addOn);
                        }
                        else
                        {
                            orderedAddOns.Insert(0, addOn);
                        }
                        addOns.Add(addOn);
                        addOn.transform.parent = transform;
                        ComputeAddOnsSortOrders();
                        addOn.Attached(this);
                    }
                }

                /// <summary>
                ///   Removes an add-on that is present, and recomputes the sort orders
                ///     of all the add-ons.
                /// </summary>
                /// <param name="addOn">The add-on to remove</param>
                public void Pop(AddOn addOn)
                {
                    if (addOns.Contains(addOn))
                    {
                        orderedAddOns.Remove(addOn);
                        addOns.Remove(addOn);
                        addOn.transform.parent = null;
                        ComputeAddOnsSortOrders();
                        addOn.Detached();
                    }
                }

                // Updates the sort orders in the innser add-ons.
                private void ComputeAddOnsSortOrders()
                {
                    for(int index = 0; index < addOns.Count; index++)
                    {
                        orderedAddOns[index].SortingOrder = index;
                    }
                }

                /// <summary>
                ///   Tells whether the whole add-ons group is paused.
                /// </summary>
                public bool Paused { get; private set; }

                /// <summary>
                ///   Invokes <see cref="AddOn.Pause(bool)"/> on each added add-on.
                /// </summary>
                /// <param name="fullFreeze">Whether to also pause animations or not</param>
                public override void Pause(bool fullFreeze)
                {
                    Paused = true;
                    foreach(AddOn addOn in orderedAddOns)
                    {
                        addOn.Pause(fullFreeze);
                    }
                }

                /// <summary>
                ///   Invokes <see cref="AddOn.Resume"/> on each added add-on.
                /// </summary>
                public override void Resume()
                {
                    foreach(AddOn addOn in addOns)
                    {
                        addOn.Resume();
                    }
                    Paused = false;
                }

                /// <summary>
                ///   Returns the appropriate sub-layer to use. Either the overlay sub-layer or the
                ///     underlay sub-layer, depending on <see cref="addOnGroupType"/>.
                /// </summary>
                /// <param name="layer">The entities layer to take the sub-layer from</param>
                /// <returns>The middle sub-layer</returns>
                protected override SortingSubLayer GetSubLayerFrom(EntitiesLayer layer)
                {
                    return addOnGroupType == AddOnGroupType.Overlay ? layer.OverlaysSubLayer : layer.UnderlaysSubLayer;
                }

                /// <summary>
                ///   Updates its position and executes the pipeline on each added add-on.
                /// </summary>
                protected override void UpdatePipeline()
                {
                    if (relatedObject && ParentMap)
                    {
                        transform.localPosition = RelatedObject.transform.localPosition;
                    }
                    if (!Paused)
                    {
                        foreach(AddOn addOn in addOns)
                        {
                            addOn.UpdatePipeline();
                        }
                    }
                }
            }
        }
    }
}
