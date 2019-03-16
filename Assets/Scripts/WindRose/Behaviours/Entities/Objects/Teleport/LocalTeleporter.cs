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
                /// <summary>
                ///   <para>
                ///     A local teleporter will ensure that an object that enters
                ///       (walks) into it, will be teleported to a related
                ///       <see cref="TeleportTarget"/>, located in the same
                ///       scene (perhaps in another map).
                ///   </para>
                ///   <para>
                ///     Teleportation will only by triggered when the object
                ///       FULLY WALKS into the teleporter. This is important to
                ///       remind when the entering object has dimensions greater
                ///       than (1, 1).
                ///   </para>
                ///   <para>
                ///     For two-sided teleporters, the object holding this component
                ///       may also hold <see cref="TeleportTarget"/>, and having
                ///       another object in similar conditions, they can specify
                ///       each other's <see cref="Target"/> and so have a bidirectional
                ///       path of teleportation.
                ///   </para>
                /// </summary>
                /// <remarks>
                ///   You may subclass this component to customize the
                ///     <see cref="CanTeleport(Positionable, TeleportTarget)"/> and
                ///     <see cref="DoTeleport(Action)"/> methods if you want to run
                ///     asynchronous code or add fade effects.
                /// </remarks>
                [RequireComponent(typeof(TriggerPlatform))]
                public class LocalTeleporter : MonoBehaviour
                {
                    /// <summary>
                    ///   The end side of this teleport.
                    /// </summary>
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

                                bool fullyContained = (x >= 0 && y >= 0 && x <= (thisWidth - objWidth) && y <= (thisHeight - objHeight));
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

                    /// <summary>
                    ///   If overriding this class, a condition may be set to tell whether an object will trigger
                    ///     the teleportation, or if such teleportation will be silently cancelled (not done), and
                    ///     the inner object will be inside the teleporter as if it was a regular step.
                    /// </summary>
                    /// <param name="objectToBeTeleported">The object intending to be teleported</param>
                    /// <param name="teleportTarget">The teleport target</param>
                    /// <returns>Whether the teleport can occur</returns>
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

                    /// <summary>
                    ///   This method can be overriden to provide an asynchronous teleportation! One example
                    ///     is producing a fade-out effect, then invoking <paramref name="teleport"/> callback,
                    ///     and then producing a fade-in. PLEASE REMIND: THE CALLBACK MUST BE INVOKED EXACTLY
                    ///     ONCE.
                    /// </summary>
                    /// <param name="teleport">The callback to invoke. IT MUST BE INVOKED SOMEWHERE IN THE IMPLEMENTATION.</param>
                    protected virtual void DoTeleport(Action teleport)
                    {
                        teleport();
                    }
                }
            }
        }
    }
}
