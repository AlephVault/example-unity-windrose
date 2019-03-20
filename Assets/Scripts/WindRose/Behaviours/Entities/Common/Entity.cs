using System;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Common
        {
            /// <summary>
            ///   Entities exist inside a map. There are two types of entities:
            ///   <list type="bullet">
            ///     <item>
            ///       <term>Objects</term>
            ///       <description>
            ///         They exist on their own, handle position, and movement.
            ///         They have ruling strategies to be allowed inside certain
            ///           types of maps.
            ///       </description>
            ///     </item>
            ///     <item>
            ///       <term>Add-On Groups</term>
            ///       <description>
            ///         Add-Ons are only visual. They exist "above" or "below" the
            ///           objects and depend on them to understand movement and
            ///           position. They are actually grouped into groups and are
            ///           related to a main Object.
            ///       </description>
            ///     </item>
            ///   </list>
            ///   Entities will know their map and position.
            /// </summary>
            public abstract class Entity : MonoBehaviour, Pausable.IPausable
            {
                /// <summary>
                ///   This method must be implemented to tell how will the
                ///     component react when it is commanded to pause, even
                ///     considering whether to also pause the animations.
                /// </summary>
                /// <param name="fullFreeze">Whether also pause the animations</param>
                public abstract void Pause(bool fullFreeze);

                /// <summary>
                ///   This method must be implemented to tell how will the
                ///     component react when it is commanded to resume. If
                ///     animations are frozen, this component must consider
                ///     they should also resume.
                /// </summary>
                public abstract void Resume();

                /// <summary>
                ///   Gets the parent map this entity is attached to.
                /// </summary>
                public abstract World.Map ParentMap { get; }

                /// <summary>
                ///   Gets the in-map X position.
                /// </summary>
                public abstract uint X { get; }

                /// <summary>
                ///   Gets the in-map Y position.
                /// </summary>
                public abstract uint Y { get; }

                /// <summary>
                ///   Gets the in-map movement.
                /// </summary>
                public abstract Types.Direction? Movement { get; }

                /// <summary>
                ///   Gets the appropriate sub-layer for this entity.
                /// </summary>
                /// <param name="layer">The entities layer to take the sub-layer from</param>
                /// <returns>The appropriate sub-layer</returns>
                protected abstract World.Layers.Entities.SortingSubLayer GetSubLayerFrom(World.Layers.Entities.EntitiesLayer layer);

                // Ensures the entity is appropriately positioned in the sub-layer's children for appropriate
                //   vertical sorting.
                protected void EnsureAppropriateVerticalSorting()
                {
                    transform.parent = GetSubLayerFrom(ParentMap.EntitiesLayer)[(int)ParentMap.Height - 1 - (int)Y].transform;
                }

                /// <summary>
                ///   The update pipeline is intended to be executed sequentially. This method must be
                ///     implemented to step-by-step control how the entity components must be updated.
                ///     Think of this method as a local implementation of Entity-Component system, but
                ///     right inside a single game object.
                /// </summary>
                protected abstract void UpdatePipeline();

                void Update()
                {
                    // Run the inner update (it will usually involve other components).
                    // Catch the null reference exception for when it is destroyed.
                    try
                    {
                        if (ParentMap)
                        {
                            EnsureAppropriateVerticalSorting();
                        }
                        UpdatePipeline();
                    }
                    catch (NullReferenceException) { }
                }
            }
        }
    }
}
