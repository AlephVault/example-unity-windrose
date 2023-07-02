using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlephVault.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using AlephVault.Unity.WindRose.Authoring.Behaviours.Entities.Objects.Teleport;

[RequireComponent(typeof(SimpleTeleportTarget))]
public class Door : SimpleTeleporter {
    protected override void DoTeleport(Action teleport, MapObject objectToBeTeleported, SimpleTeleportTarget teleportTarget, MapObject teleportTargetObject)
    {
        base.DoTeleport(teleport, objectToBeTeleported, teleportTarget, teleportTargetObject);
        if (teleportTarget.ForceOrientation)
        {
            objectToBeTeleported.StartMovement(teleportTarget.NewOrientation);
        }
    }
}
