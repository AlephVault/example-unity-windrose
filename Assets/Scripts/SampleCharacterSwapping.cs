using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using CamJam.Behaviours;

[RequireComponent(typeof(StalkerEye))]
class SampleCharacterSwapping : MonoBehaviour
{
    /**
     * Speed for transitions.
     */
    [SerializeField]
    private float speed = 0;

    /**
     * Index of objects to rotate among.
     */
    [SerializeField]
    private GameObject[] targets;

    private StalkerEye stalkerEye;

    private int currentTarget = 0;

    private void Start()
    {
        stalkerEye = GetComponent<StalkerEye>();
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
            stalkerEye.Seek(targets[currentTarget], speed, true, delegate ()
            {
                Debug.Log("Stalking target " + currentTarget + "...");
            });
        }
    }
}
