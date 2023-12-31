﻿using UnityEngine;

[RequireComponent(typeof(AlephVault.Unity.WindRose.GabTab.Authoring.Behaviours.UI.HUD))]
public class SampleCharacterSwapping : MonoBehaviour
{
    /**
     * Delay for transitions.
     */
    [SerializeField]
    private float delay = 0;

    /**
     * Index of objects to rotate among.
     */
    [SerializeField]
    private AlephVault.Unity.WindRose.Authoring.Behaviours.Entities.Objects.MapObject[] targets;

    private AlephVault.Unity.WindRose.GabTab.Authoring.Behaviours.UI.HUD hud;

    private int currentTarget = 0;

    private void Start()
    {
        hud = GetComponent<AlephVault.Unity.WindRose.GabTab.Authoring.Behaviours.UI.HUD>();
        TrackTarget();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentTarget = (currentTarget + 1);
            if (currentTarget == targets.Length)
            {
                currentTarget = 0;
            }
            TrackTarget();
        }
    }

    private async void TrackTarget()
    {
        if (targets.Length != 0)
        {
            await hud.Focus(targets[currentTarget], delay, true);
        }
    }
}
