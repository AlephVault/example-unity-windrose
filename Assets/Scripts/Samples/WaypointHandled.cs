using System;
using System.Collections;
using UnityEngine;
using WindRose.Behaviours.World;
using WindRose.Behaviours.Entities.Objects;
using WindRose.Types;

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
        Positionable positionable = GetComponent<Positionable>();
        positionable.onAttached.AddListener(delegate (Map map)
        {
            if (waySteps.Length != 0)
            {
                currentCoroutine = StartCoroutine(PerformMovement());
            }
        });
        positionable.onDetached.AddListener(delegate ()
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        });
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
                bool result = movable.StartMovement(currentStep.movementDirection, false);
                if (result)
                {
                    // Wait until the movement is done.
                    yield return new WaitUntil(() => movable.Movement == null);

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
