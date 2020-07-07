using System;
using System.Collections;
using UnityEngine;
using WindRose.Behaviours.World;
using WindRose.Behaviours.Entities.Objects;
using WindRose.Types;
using System.Threading.Tasks;
using GMM.Utils;

[RequireComponent(typeof(Movable))]
class WaypointHandled : MonoBehaviour
{
    private Movable movable;
    private Oriented oriented;

    [Serializable]
    public struct WayStep
    {
        public Direction movementDirection;
        public bool onlyLook;
        public float delay;
    }

    [SerializeField]
    private WayStep[] waySteps;
    private bool isDead = false;
    private Coroutine currentCoroutine = null;
    private int currentStepIndex = 0;

    // Use this for initialization
    void Awake()
    {
        movable = GetComponent<Movable>();
        oriented = GetComponent<Oriented>();
        MapObject mapObject = GetComponent<MapObject>();
        mapObject.onAttached.AddListener(delegate (Map map)
        {
            isDead = false;
            if (waySteps.Length != 0)
            {
                PerformMovement();
            }
        });
        mapObject.onDetached.AddListener(delegate ()
        {
            isDead = true;
        });
    }

    private async void PerformMovement()
    {
        while(true)
        {
            WayStep currentStep = waySteps[currentStepIndex];

            // Waiting for delay
            float currentTime = 0;
            while (currentTime <= currentStep.delay)
            {
                await Tasks.Blink();
                currentTime += Time.deltaTime;
            }

            // If dead, aborting
            if (isDead)
            {
                break;
            }

            // Orienting the character to look in the same direction
            oriented.Orientation = currentStep.movementDirection;

            // Starting a movement
            if (!currentStep.onlyLook)
            {
                // Perform the movement until it is done.
                bool result = movable.StartMovement(currentStep.movementDirection, false);
                if (result)
                {
                    // Wait until the movement is done.
                    while (movable.Movement != null)
                    {
                        await Tasks.Blink();
                    }

                    // Move to the next frame.
                    currentStepIndex = (currentStepIndex + 1) % waySteps.Length;
                }
            }
            else
            {
                // Orientation was successful. Move to the next frame.
                currentStepIndex = (currentStepIndex + 1) % waySteps.Length;
            }
        }
    }

    void OnDestroy()
    {
        isDead = true;
    }
}
