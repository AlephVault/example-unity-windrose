using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            /// <summary>
            ///   <para>
            ///     This behaviour configures a collision mask (giving it appropriate
            ///       dimensions and and making it a trigger).
            ///   </para>
            ///   <para>
            ///     Although the size and pivot are determined once, and this behaviour
            ///       does not touch the collider or handles the collisions, it will
            ///       provide a method named <see cref="RefreshDimensions"/> so other
            ///       users can make use of it when needed.
            ///   </para>
            /// </summary>
            [RequireComponent(typeof(Collider2D))]
            public abstract class TriggerHolder : MonoBehaviour
            {
                /// <summary>
                ///   The retrieved 2D collider.
                /// </summary>
                protected Collider2D collider2D;

                /// <summary>
                ///   This method must be implemented to retrieve the underlying component.
                ///   It will actually retrieve a collider component from this component,
                ///     but will differ on the type.
                /// </summary>
                /// <returns>The retrieved component</returns>
                protected abstract Collider2D GetCollider2D();

                /// <summary>
                ///   This method must be implemented to setup the (retrieved) component.
                /// </summary>
                /// <param name="collider2D">The component to setup</param>
                protected abstract void SetupCollider(Collider2D collider2D);

                /// <summary>
                ///   Refreshes the dimensions (essentially, invokes <see cref="SetupCollider(Collider2D)"/>
                ///     again).
                /// </summary>
                public void RefreshDimensions()
                {
                    SetupCollider(collider2D);
                }

                protected virtual void Awake()
                {
                    collider2D = GetCollider2D();
                }

                protected virtual void Start()
                {
                    collider2D.isTrigger = true;
                    SetupCollider(collider2D);
                }
            }
        }
    }
}