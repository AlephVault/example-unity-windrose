using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            namespace Teleport
            {
                [RequireComponent(typeof(TriggerPlatform))]
                public class LocalTeleporter : MonoBehaviour
                {
                    /**
                     * A local teleporter performs a teleport in the same scene.
                     * 
                     * However, since a scene may have several distinct maps, a
                     *   local teleporter will be able to teleport to a different
                     *   map. To make this possible, one field will exist:
                     *   
                     *   1. Teleport target. This target may belong to the same or
                     *        a different map, and will be in a valid position (as
                     *        and positionable object is!).
                     *   2. An orientation the teleported object (if orientable) will
                     *        have.
                     *   
                     *   Under the hoods, a teleporter is just a call to Attach
                     *     on the activator object that enters (walking) to the
                     *     teleporter, appropriately, to the map and coordinates
                     *     the teleport target will refer. This involves:
                     *   
                     *   1. The activator must be seen "walking" into to this teleporter.
                     *   2. The activator must be completely contained by this teleporter.
                     *   3. Satisfied those conditions, a teleport will occur to the target
                     *        position. The teleportation method may be overriden, and the
                     *        default one is instantaneous.
                     *   4. The teleportation may force a new orientation of the teleported
                     *        object (if the object is actually orientable).
                     */

                    public TeleportTarget Target;
                    
                    // Use this for initialization
                    private void Start()
                    {
                        TriggerPlatform platform = GetComponent<TriggerPlatform>();
                        platform.onMapTriggerWalked.AddListener(OnWalkedIntoTeleporter);
                    }

                    private void OnWalkedIntoTeleporter(Positionable objectToBeTeleported, Positionable thisTeleporter, int x, int y)
                    {
                        if (enabled && Target)
                        {
                            Positionable tgPositionable = Target.GetComponent<Positionable>();
                            if (tgPositionable.ParentMap)
                            {
                                uint thisWidth = thisTeleporter.Width;
                                uint thisHeight = thisTeleporter.Height;
                                uint objWidth = objectToBeTeleported.Width;
                                uint objHeight = objectToBeTeleported.Height;
                                uint tgWidth = tgPositionable.Width;
                                uint tgHeight = tgPositionable.Height;
                                bool fullyContained = (x >= 0 && y >= 0 && x < (thisWidth - objWidth) && y < (thisHeight - objHeight));
                                bool matchingTarget = (tgWidth >= objWidth && tgHeight >= objHeight && tgWidth % 2 == objWidth % 2 && tgHeight % 2 == objHeight % 2);

                                if (fullyContained && matchingTarget && CanTeleport(objectToBeTeleported, Target))
                                {
                                    DoTeleport(delegate ()
                                    {
                                        ObjectTeleportOperation(objectToBeTeleported, Target, tgPositionable);
                                    });
                                }
                            }
                        }
                    }

                    /**
                     * Perhaps you'd like to override this one to add custom checks before teleporting?
                     */
                    protected virtual bool CanTeleport(Positionable objectToBeTeleported, TeleportTarget teleportTarget)
                    {
                        return true;
                    }

                    /**
                     * This method will only be invoked in the context of a callback. This only has the use to work as the internal
                     *   callback of a process that can be deferred by the user (DoTeleport).
                     */
                    private void ObjectTeleportOperation(Positionable objectToBeTeleported, TeleportTarget teleportTarget, Positionable teleportTargetPositionable)
                    {
                        uint tgX = teleportTargetPositionable.X;
                        uint tgY = teleportTargetPositionable.Y;
                        uint tgWidth = teleportTargetPositionable.Width;
                        uint tgHeight = teleportTargetPositionable.Height;
                        uint x = tgX + (tgWidth - objectToBeTeleported.Width) / 2;
                        uint y = tgY + (tgHeight - objectToBeTeleported.Height) / 2;

                        // Choose between an in-map teleport or a full-force-attach to a new map.
                        if (teleportTargetPositionable.ParentMap != objectToBeTeleported.ParentMap)
                        {
                            objectToBeTeleported.Attach(teleportTargetPositionable.ParentMap, x, y, true);
                        }
                        else
                        {
                            objectToBeTeleported.Teleport(x, y);
                        }
                        // Also set the orientation of the object being teleported.
                        Oriented objectToBeOriented = objectToBeTeleported.GetComponent<Oriented>();
                        if (objectToBeOriented && teleportTarget.ForceOrientation)
                        {
                            objectToBeOriented.orientation = teleportTarget.NewOrientation;
                        }
                    }

                    /**
                     * Perhaps you'd like to override this one to add custom behaviour? (e.g. animations)
                     */
                    protected virtual void DoTeleport(Action teleport)
                    {
                        teleport();
                    }
                }
            }
        }
    }
}
