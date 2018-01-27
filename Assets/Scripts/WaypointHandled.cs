using System;
using System.Collections;
using UnityEngine;
using WindRose.Behaviours;
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
    private Coroutine currentCoroutine = null;
    private int currentStepIndex = 0;

    // Use this for initialization
    void Start()
    {
        positionable = GetComponent<Positionable>();
        oriented = GetComponent<Oriented>();
    }

    void OnAttached(object[] args)
    {
        if (waySteps.Length != 0)
        {
            currentCoroutine = StartCoroutine(PerformMovement());
        }
    }

    void OnDetached()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = null;
    }

    IEnumerator PerformMovement()
    {
        while(true)
        {
            WayStep currentStep = waySteps[currentStepIndex];

            // Waiting for delay
            yield return new WaitForSeconds(currentStep.delay);

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
