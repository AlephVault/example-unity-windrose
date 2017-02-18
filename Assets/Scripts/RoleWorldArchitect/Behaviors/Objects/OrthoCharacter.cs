using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class OrthoCharacter : MonoBehaviour {

    public enum OrthoCharacterSizeType {
        VX, XP
    }

    private const int SPRITE_WIDTH = 32;
    private const int SPRITE_VX_HEIGHT = 32;
    private const int SPRITE_XP_HEIGHT = 48;

    [SerializeField]
    private OrthoCharacterSizeType sizeType = OrthoCharacterSizeType.VX;

    [SerializeField]
    private Sprite[] downMovement;

    [SerializeField]
    private Sprite[] upMovement;

    [SerializeField]
    private Sprite[] leftMovement;

    [SerializeField]
    private Sprite[] rightMovement;

    [SerializeField]
    private uint downStoppedIndex = 1;

    [SerializeField]
    private uint upStoppedIndex = 1;

    [SerializeField]
    private uint leftStoppedIndex = 1;

    [SerializeField]
    private uint rightStoppedIndex = 1;

    public uint movementSpeed = 64;
    public uint spriteFramesPerSecond = 4;
    public bool moving = false;
    //public OrthoDirection direction = OrthoDirection.DOWN;

    private float currentTime = 0;
    private int currentFrame = 0;
    private Rigidbody2D body;
    private SpriteRenderer renderer;

    // Use this for initialization
    void Awake () {
        body = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        FullCheck();
	}
	
	// Update is called once per frame
	void Update () {
		if (moving) {
            float fraction = (float) (1.0 / spriteFramesPerSecond);
            currentTime += Time.deltaTime;
            Debug.Log("Current time: " + currentTime);
            if (currentTime > fraction) {
                currentTime -= fraction;
                currentFrame++;
                Debug.Log("Current frame: " + currentFrame);
            }
            //if (direction == OrthoDirection.UP) { body.velocity = new Vector2(0, movementSpeed); currentFrame %= upMovement.Length; renderer.sprite = upMovement[currentFrame]; }
            //if (direction == OrthoDirection.DOWN) { body.velocity = new Vector2(0, -movementSpeed); currentFrame %= downMovement.Length; renderer.sprite = downMovement[currentFrame]; }
            //if (direction == OrthoDirection.LEFT) { body.velocity = new Vector2(-movementSpeed, 0); currentFrame %= leftMovement.Length; renderer.sprite = leftMovement[currentFrame]; }
            //if (direction == OrthoDirection.RIGHT) { body.velocity = new Vector2(movementSpeed, 0); currentFrame %= rightMovement.Length; renderer.sprite = rightMovement[currentFrame]; }

        } else {
            currentFrame = 0;
            body.velocity = Vector2.zero;
            //if (direction == OrthoDirection.UP) { renderer.sprite = upMovement[upStoppedIndex]; }
            //if (direction == OrthoDirection.DOWN) { renderer.sprite = downMovement[downStoppedIndex]; }
            //if (direction == OrthoDirection.LEFT) { renderer.sprite = leftMovement[leftStoppedIndex]; }
            //if (direction == OrthoDirection.RIGHT) { renderer.sprite = rightMovement[rightStoppedIndex]; }
        }
    }

    void FullCheck() {
        ReplaceData(sizeType,
                    leftMovement, leftStoppedIndex,
                    rightMovement, rightStoppedIndex,
                    upMovement, upStoppedIndex,
                    downMovement, downStoppedIndex);
    }

    void SetDownSprites(Sprite[] movementSpriteList, uint stoppedSpriteIndex) {
        ReplaceData(sizeType,
                    leftMovement, leftStoppedIndex,
                    rightMovement, rightStoppedIndex,
                    upMovement, upStoppedIndex,
                    CloneList(movementSpriteList), stoppedSpriteIndex);
    }

    void SetUpSprites(Sprite[] movementSpriteList, uint stoppedSpriteIndex) {
        ReplaceData(sizeType,
                    leftMovement, leftStoppedIndex,
                    rightMovement, rightStoppedIndex,
                    CloneList(movementSpriteList), stoppedSpriteIndex,
                    downMovement, downStoppedIndex);
    }

    void SetLeftSprites(Sprite[] movementSpriteList, uint stoppedSpriteIndex) {
        ReplaceData(sizeType,
                    CloneList(movementSpriteList), stoppedSpriteIndex,
                    rightMovement, rightStoppedIndex,
                    upMovement, upStoppedIndex,
                    downMovement, downStoppedIndex);
    }

    void SetRightSprites(Sprite[] movementSpriteList, uint stoppedSpriteIndex) {
        ReplaceData(sizeType,
                    leftMovement, leftStoppedIndex,
                    CloneList(movementSpriteList), stoppedSpriteIndex,
                    upMovement, upStoppedIndex,
                    downMovement, downStoppedIndex);
    }

    void SetWholeSpriteData(OrthoCharacterSizeType newSizeType,
                            Sprite[] leftMovementSpriteList, uint leftStoppedSpriteIndex,
                            Sprite[] rightMovementSpriteList, uint rightStoppedSpriteIndex,
                            Sprite[] upMovementSpriteList, uint upStoppedSpriteIndex,
                            Sprite[] downMovementSpriteList, uint downStoppedSpriteIndex) {
        ReplaceData(newSizeType,
                    CloneList(leftMovementSpriteList), leftStoppedSpriteIndex,
                    CloneList(rightMovementSpriteList), rightStoppedSpriteIndex,
                    CloneList(upMovementSpriteList), upStoppedSpriteIndex,
                    CloneList(downMovementSpriteList), downStoppedSpriteIndex);
    }

    private Sprite[] CloneList(Sprite[] list) {
        return (list != null) ? ((Sprite[])list.Clone()) : null;
    }

    private void ReplaceData(OrthoCharacterSizeType newSizeType,
                             Sprite[] leftMovementSpriteList, uint leftStoppedSpriteIndex,
                             Sprite[] rightMovementSpriteList, uint rightStoppedSpriteIndex,
                             Sprite[] upMovementSpriteList, uint upStoppedSpriteIndex,
                             Sprite[] downMovementSpriteList, uint downStoppedSpriteIndex) {
        try {
            leftStoppedSpriteIndex = CheckValueSpriteListAndStopIndex(leftMovementSpriteList, leftStoppedSpriteIndex, newSizeType, "left");
            rightStoppedSpriteIndex = CheckValueSpriteListAndStopIndex(rightMovementSpriteList, rightStoppedSpriteIndex, newSizeType, "right");
            upStoppedSpriteIndex = CheckValueSpriteListAndStopIndex(upMovementSpriteList, upStoppedSpriteIndex, newSizeType, "up");
            downStoppedSpriteIndex = CheckValueSpriteListAndStopIndex(downMovementSpriteList, downStoppedSpriteIndex, newSizeType, "down");

            leftStoppedIndex = leftStoppedSpriteIndex;
            rightStoppedIndex = rightStoppedSpriteIndex;
            upStoppedIndex = upStoppedSpriteIndex;
            downStoppedIndex = downStoppedSpriteIndex;
            leftMovement = leftMovementSpriteList;
            rightMovement = rightMovementSpriteList;
            upMovement = upMovementSpriteList;
            downMovement = downMovementSpriteList;
            sizeType = newSizeType;
            gameObject.SetActive(true);
            Debug.Log("Activated");
        }
        catch (System.Exception e) {
            gameObject.SetActive(false);
            Debug.Log("Deactivated due to errors");
        }
    }

    private uint CheckValueSpriteListAndStopIndex(Sprite[] movementSpriteList, uint stoppedSpriteIndex, OrthoCharacterSizeType sizeType, string direction) {
        if (movementSpriteList == null || !movementSpriteList.IsFixedSize || movementSpriteList.Length == 0) {
            throw new System.Exception("A movement sprite list for " + direction + " direction cannot be empty, dynamic-size, or null");
        }
        foreach (Sprite sprite in movementSpriteList) {
            if (sprite == null || sprite.bounds.size.x != SPRITE_WIDTH) {
                throw new System.Exception("No sprite in " + direction + "-list can be null or have a width different than 32 units");
            }
            if (sizeType == OrthoCharacterSizeType.VX) {
                if (sprite.bounds.size.x != SPRITE_VX_HEIGHT) {
                    throw new System.Exception("No sprite in " + direction + "-list can be null or have a width different than 32 units for a VX orthogonal character");
                }
            } else {
                if (sprite.bounds.size.x != SPRITE_XP_HEIGHT) {
                    throw new System.Exception("No sprite in " + direction + "-list can be null or have a width different than 48 units for an XP orthogonal character");
                }
            }
        };
        uint length = (uint) movementSpriteList.Length;
        return (length < stoppedSpriteIndex) ? length : stoppedSpriteIndex;
    }

}
