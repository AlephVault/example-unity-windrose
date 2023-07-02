using System;
using System.Collections;
using UnityEngine;
using AlephVault.Unity.WindRose.Authoring.Behaviours.World;
using AlephVault.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using AlephVault.Unity.WindRose.Types;
using System.Threading.Tasks;
using AlephVault.Unity.Support.Utils;

[RequireComponent(typeof(MapObject))]
public class WaypointHandled : MonoBehaviour
{
    private MapObject mapObject;

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
    private int currentStepIndex = 0;

    // Use this for initialization
    void Awake()
    {
        mapObject = GetComponent<MapObject>();
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
            mapObject.Orientation = currentStep.movementDirection;

            // Starting a movement
            if (!currentStep.onlyLook)
            {
                // Perform the movement until it is done.
                bool result = mapObject.StartMovement(currentStep.movementDirection, false);
                if (result)
                {
                    // Wait until the movement is done.
                    while (mapObject.Movement != null)
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
