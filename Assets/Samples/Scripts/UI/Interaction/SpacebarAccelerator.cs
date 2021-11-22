using UnityEngine;
using GameMeanMachine.Unity.GabTab.Authoring.Behaviours;

[RequireComponent(typeof(InteractiveInterface))]
public class SpacebarAccelerator : MonoBehaviour {

    private InteractiveInterface interactiveInterface;

	// Use this for initialization
	void Start () {
        interactiveInterface = GetComponent<InteractiveInterface>();
	}
	
	// Update is called once per frame
	void Update () {
        interactiveInterface.QuickTextMovement = Input.GetKey(KeyCode.Space);
	}
}
