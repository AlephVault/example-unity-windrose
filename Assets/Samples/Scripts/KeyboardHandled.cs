using UnityEngine;
using AlephVault.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using AlephVault.Unity.WindRose.Authoring.Behaviours.Entities.Objects.CommandExchange.Talk;
using AlephVault.Unity.WindRose.Types;

[RequireComponent(typeof(TalkSender))]
public class KeyboardHandled : MonoBehaviour {
    private MapObject mapObject;
    private TalkSender talkSender;

	// Use this for initialization
	void Awake () {
        mapObject = GetComponent<MapObject>();
        talkSender = GetComponent<TalkSender>();
	}
	
	// Update is called once per frame
	void Update () {
        bool upHeld = Input.GetKey(KeyCode.UpArrow);
        bool downHeld = Input.GetKey(KeyCode.DownArrow);
        bool leftHeld = Input.GetKey(KeyCode.LeftArrow);
        bool rightHeld = Input.GetKey(KeyCode.RightArrow);
        bool fHeld = Input.GetKey(KeyCode.F);
        bool spacebarJustPressed = Input.GetKeyDown(KeyCode.Space);
        byte pressedKeys = 0;
        if (upHeld) pressedKeys++;
        if (downHeld) pressedKeys++;
        if (leftHeld) pressedKeys++;
        if (rightHeld) pressedKeys++;
        if (spacebarJustPressed) pressedKeys++;
        if (fHeld) pressedKeys++;
        if (pressedKeys == 1)
        {
            if (fHeld)
            {
                mapObject.FinishMovement();
            }
            if (upHeld)
            {
                if (mapObject.Orientation == Direction.UP)
                {
                    mapObject.StartMovement(Direction.UP);
                }
                else if (!mapObject.IsMoving)
                {
                    mapObject.Orientation = Direction.UP;
                }
            }
            else if (downHeld)
            {
                if (mapObject.Orientation == Direction.DOWN)
                {
                    mapObject.StartMovement(Direction.DOWN);
                }
                else if (!mapObject.IsMoving)
                {
                    mapObject.Orientation = Direction.DOWN;
                }
            }
            else if (leftHeld)
            {
                if (mapObject.Orientation == Direction.LEFT)
                {
                    mapObject.StartMovement(Direction.LEFT);
                }
                else if (!mapObject.IsMoving)
                {
                    mapObject.Orientation = Direction.LEFT;
                }
            }
            else if(rightHeld) // rightHeld
            {
                if (mapObject.Orientation == Direction.RIGHT)
                {
                    mapObject.StartMovement(Direction.RIGHT);
                }
                else if (!mapObject.IsMoving)
                {
                    mapObject.Orientation = Direction.RIGHT;
                }
            }
            if (spacebarJustPressed)
            {
                talkSender.Talk();
            }
        }
    }
}
