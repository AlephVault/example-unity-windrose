using UnityEngine;
using WindRose.Behaviours;
using WindRose.Types;

[RequireComponent(typeof(Movable))]
public class KeyboardHandled : MonoBehaviour {
    private Movable movable;
    private Oriented oriented;

	// Use this for initialization
	void Awake () {
        movable = GetComponent<Movable>();
        oriented = GetComponent<Oriented>();
	}
	
	// Update is called once per frame
	void Update () {
        bool upHeld = Input.GetKey(KeyCode.UpArrow);
        bool downHeld = Input.GetKey(KeyCode.DownArrow);
        bool leftHeld = Input.GetKey(KeyCode.LeftArrow);
        bool rightHeld = Input.GetKey(KeyCode.RightArrow);
        byte pressedKeys = 0;
        if (upHeld) pressedKeys++;
        if (downHeld) pressedKeys++;
        if (leftHeld) pressedKeys++;
        if (rightHeld) pressedKeys++;
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
            else // rightHeld
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
        }
    }
}
