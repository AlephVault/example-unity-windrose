using System;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            /// <summary>
            ///   <para>
            ///     This components sets its underlying collider (which will be a
            ///       <see cref="BoxCollider2D"/>) to the dimensions of the object
            ///       (considering underlying cell's width/height).
            ///   </para>
            ///   <para>
            ///     Live triggers are intended to be installed into moving objects,
            ///       and not in objects that intend to be like platforms. They 
            ///       ensure the collider is only active while the object is added
            ///       to a map.
            ///   </para>
            /// </summary>
            [RequireComponent(typeof(Rigidbody2D))]
            [RequireComponent(typeof(Positionable))]
            [RequireComponent(typeof(BoxCollider2D))]
            public class TriggerLive : TriggerHolder
            {
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

                /// <summary>
                ///   Gets its underlying <see cref="BoxCollider2D"/> as the involved collider.
                /// </summary>
                /// <returns>The collider</returns>
                protected override Collider2D GetCollider2D()
                {
                    return GetComponent<BoxCollider2D>();
                }

                /// <summary>
                ///   Sets up the collider to the dimensions of this object, with respect
                ///     to the dimensions and cell width/height in the objects layer.
                /// </summary>
                /// <param name="collider2D"></param>
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