using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindRose.Behaviours.Entities.Objects;
using WindRose.Behaviours.Entities.Objects.Teleport;

[RequireComponent(typeof(TeleportTarget))]
public class Door : LocalTeleporter {
    protected override void DoTeleport(Action teleport, MapObject objectToBeTeleported, TeleportTarget teleportTarget, MapObject teleportTargetObject)
    {
        base.DoTeleport(teleport, objectToBeTeleported, teleportTarget, teleportTargetObject);
        if (teleportTarget.ForceOrientation)
        {
            objectToBeTeleported.StartMovement(teleportTarget.NewOrientation);
        }
    }
}
