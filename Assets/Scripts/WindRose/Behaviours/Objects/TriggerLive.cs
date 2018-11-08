using System;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            [RequireComponent(typeof(Rigidbody2D))]
            [RequireComponent(typeof(Positionable))]
            [RequireComponent(typeof(BoxCollider2D))]
            public class TriggerLive : TriggerHolder
            {
                /**
                 * This abstract trigger has only the task of being a kinematic rigidbody, which is
                 *   required to trigger with subclasses from TriggerZone. This abstract class should
                 *   be a complement of TriggerZone.
                 * 
                 * Live triggers are deeply tied to WindRose, since they interact with positionables:
                 *   Live triggers are live map object, which could be not the case for other types
                 *   of colliders.
                 */

                private Rigidbody2D rigidbody2D;
                protected override void Awake()
                {
                    base.Awake();
                    collider2D.enabled = false;
                    Positionable positionable = GetComponent<Positionable>();
                    positionable.onAttached.AddListener(delegate (World.Map map)
                    {
                        collider2D.enabled = true;
                    });
                    positionable.onDetached.AddListener(delegate ()
                    {
                        collider2D.enabled = false;
                    });
                    rigidbody2D = GetComponent<Rigidbody2D>();
                }

                protected override Collider2D GetCollider2D()
                {
                    return GetComponent<BoxCollider2D>();
                }

                protected override void SetupCollider(Collider2D collider2D)
                {
                    BoxCollider2D boxCollider2D = (BoxCollider2D)collider2D;
                    Positionable positionable = GetComponent<Positionable>();
                    // collision mask will have certain width and height
                    boxCollider2D.size = new Vector2(positionable.Width * positionable.GetCellWidth(), positionable.Height * positionable.GetCellHeight());
                    // and starting with those dimensions, we compute the offset as >>> and vvv
                    boxCollider2D.offset = new Vector2(boxCollider2D.size.x / 2, boxCollider2D.size.y / 2);
                }

                protected override void Start()
                {
                    base.Start();
                    rigidbody2D.isKinematic = true;
                }
            }
        }
    }
}