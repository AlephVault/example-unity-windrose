using System;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            public class TriggerActivator : TriggerLive
            {
                /**
                 * This helps us acting as a sending trigger. WindRose events dispatches will
                 *   be relevant in this behaviour. These events will be used from the counterpart
                 *   (i.e. a trigger zone, like platform or vision range).
                 * 
                 * This activator is strict part of WindRose objects and so will only consider
                 *   BoxCollider2D as available collider to be used. Also it will determine its
                 *   dimensions according to its positionable's Width and Height.
                 */

                protected override void SetupCollider(Collider2D collider2D)
                {
                    BoxCollider2D boxCollider2D = (BoxCollider2D)collider2D;
                    Positionable positionable = GetComponent<Positionable>();
                    // collision mask will have certain width and height
                    boxCollider2D.size = new Vector2(positionable.Width * positionable.GetCellWidth(), positionable.Height * positionable.GetCellHeight());
                    // and starting with those dimensions, we compute the offset as >>> and vvv
                    boxCollider2D.offset = new Vector2(boxCollider2D.size.x / 2, boxCollider2D.size.y / 2);
                }
            }
        }
    }
}