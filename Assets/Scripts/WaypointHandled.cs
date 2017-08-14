using System;
using System.Collections;
using UnityEngine;
using WindRose.Behaviors;
using WindRose.Types;

[RequireComponent(typeof(Movable))]
class WaypointHandled : MonoBehaviour
{
    private Positionable positionable;
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

    // Use this for initialization
    void Start()
    {
        positionable = GetComponent<Positionable>();
        oriented = GetComponent<Oriented>();
    }

    void OnAttached()
    {
        if (waySteps.Length != 0)
        {
            StartCoroutine(PerformMovement());
        }
    }

    IEnumerator PerformMovement()
    {
        int currentStepIndex = 0;
        while(true)
        {
            WayStep currentStep = waySteps[currentStepIndex];

            // Waiting for delay
            if (currentStep.delay > 0)
            {
                yield return new WaitForSeconds(currentStep.delay);
            }

            // If dead, aborting
            if (isDead)
            {
                break;
            }

            // Orienting the character to look in the same direction
            oriented.orientation = currentStep.movementDirection;

            // Starting a movement
            if (!currentStep.onlyLook)
            {
                // Perform the movement until it is done.
                bool result = positionable.StartMovement(currentStep.movementDirection);
                if (result)
                {
                    // Wait until the movement is done.
                    yield return new WaitUntil(() => positionable.Movement == null);

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
