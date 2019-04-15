using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindRose.Behaviours.Entities.Objects;
using WindRose.Behaviours.Entities.Objects.Teleport;

[RequireComponent(typeof(TeleportTarget))]
public class Door : LocalTeleporter {
    protected override void DoTeleport(Action teleport, WindRose.Behaviours.Entities.Objects.MapObject objectToBeTeleported, TeleportTarget teleportTarget, WindRose.Behaviours.Entities.Objects.MapObject teleportTargetObject)
    {
        base.DoTeleport(teleport, objectToBeTeleported, teleportTarget, teleportTargetObject);
        Movable movable = objectToBeTeleported.GetComponent<Movable>();
        if (movable && teleportTarget.ForceOrientation)
        {
            movable.StartMovement(teleportTarget.NewOrientation);
        }
    }
}
