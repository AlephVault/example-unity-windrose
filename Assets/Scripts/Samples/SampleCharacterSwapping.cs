using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using CamJam.Behaviours;

[RequireComponent(typeof(WindRose.Behaviours.UI.HUD))]
class SampleCharacterSwapping : MonoBehaviour
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
    private WindRose.Behaviours.Entities.Objects.MapObject[] targets;

    private WindRose.Behaviours.UI.HUD hud;

    private int currentTarget = 0;

    private void Start()
    {
        hud = GetComponent<WindRose.Behaviours.UI.HUD>();
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

    private void TrackTarget()
    {
        if (targets.Length != 0)
        {
            hud.Focus(targets[currentTarget], delay, true);
        }
    }
}
