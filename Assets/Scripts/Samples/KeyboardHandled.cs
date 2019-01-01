using UnityEngine;
using WindRose.Behaviours.Objects;
using WindRose.Behaviours.Objects.CommandExchange.Talk;
using WindRose.Types;

[RequireComponent(typeof(Movable))]
[RequireComponent(typeof(TalkSender))]
public class KeyboardHandled : MonoBehaviour {
    private Movable movable;
    private Oriented oriented;
    private TalkSender talkSender;

	// Use this for initialization
	void Awake () {
        movable = GetComponent<Movable>();
        oriented = GetComponent<Oriented>();
        talkSender = GetComponent<TalkSender>();
	}
	
	// Update is called once per frame
	void Update () {
        bool upHeld = Input.GetKey(KeyCode.UpArrow);
        bool downHeld = Input.GetKey(KeyCode.DownArrow);
        bool leftHeld = Input.GetKey(KeyCode.LeftArrow);
        bool rightHeld = Input.GetKey(KeyCode.RightArrow);
        bool spacebarJustPressed = Input.GetKeyDown(KeyCode.Space);
        byte pressedKeys = 0;
        if (upHeld) pressedKeys++;
        if (downHeld) pressedKeys++;
        if (leftHeld) pressedKeys++;
        if (rightHeld) pressedKeys++;
        if (spacebarJustPressed) pressedKeys++;
        if (pressedKeys == 1)
        {
            if (upHeld)
            {
                if (oriented.orientation == Direction.UP)
                {
                    movable.StartMovement(Direction.UP);
                }
                else
                {
                    oriented.orientation = Direction.UP;
                }
            }
            else if (downHeld)
            {
                if (oriented.orientation == Direction.DOWN)
                {
                    movable.StartMovement(Direction.DOWN);
                }
                else
                {
                    oriented.orientation = Direction.DOWN;
                }
            }
            else if (leftHeld)
            {
                if (oriented.orientation == Direction.LEFT)
                {
                    movable.StartMovement(Direction.LEFT);
                }
                else
                {
                    oriented.orientation = Direction.LEFT;
                }
            }
            else if(rightHeld) // rightHeld
            {
                if (oriented.orientation == Direction.RIGHT)
                {
                    movable.StartMovement(Direction.RIGHT);
                }
                else
                {
                    oriented.orientation = Direction.RIGHT;
                }
            }
            if (spacebarJustPressed)
            {
                talkSender.Talk();
            }
        }
    }
}
