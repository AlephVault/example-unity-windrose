using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            [RequireComponent(typeof(Collider2D))]
            public abstract class TriggerHolder : MonoBehaviour
            {
                /**
                 * This behaviour configures a collision mask in the collider2D
                 *   component, and turns it into a trigger.
                 * 
                 * The size and pivot are determined once*, and this behaviour does
                 *   nothing else regarding the collisions, like trying to detect
                 *   them or handling events.
                 * 
                 * (* However the user is provided of a method named RefreshDimensions
                 *    to be called when they need to refresh, again, the collider's
                 *    dimensions using the same logic)
                 * 
                 * This works regardless the object has a rigidbody or not, since this
                 *   only provides a way to update the box collider's position.
                 */

                protected Collider2D collider2D;
                protected abstract Collider2D GetCollider2D();
                protected abstract void SetupCollider(Collider2D collider2D);

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

                void Pause(bool fullFreeze)
                {
                    collider2D.enabled = false;
                }

                void Resume()
                {
                    collider2D.enabled = true;
                }
            }
        }
    }
}