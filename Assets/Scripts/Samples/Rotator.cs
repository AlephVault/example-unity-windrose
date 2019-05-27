using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WindRose.Behaviours.World.ObjectsManagementStrategies.Solidness;
using WindRose.Behaviours.Entities.Objects;
using WindRose.Behaviours.Entities.Objects.Strategies.Simple;
using WindRose.Behaviours.Entities.Objects.Strategies.Solidness;

[RequireComponent(typeof(MapObject))]
[RequireComponent(typeof(ObjectStrategyHolder))]
[RequireComponent(typeof(SimpleObjectStrategy))]
[RequireComponent(typeof(SpriteRenderer))]
public class Rotator : MonoBehaviour {
    private static SolidnessStatus[,] mask1 = new SolidnessStatus[,]
    {
        // SG
        // SG
        { SolidnessStatus.Solid, SolidnessStatus.Solid },
        { SolidnessStatus.Ghost, SolidnessStatus.Ghost },
    };
    private static SolidnessStatus[,] mask4 = new SolidnessStatus[,]
    {
        // SS
        // GG
        { SolidnessStatus.Ghost, SolidnessStatus.Solid },
        { SolidnessStatus.Ghost, SolidnessStatus.Solid },
    };
    private static SolidnessStatus[,] mask3 = new SolidnessStatus[,]
    {
        // GS
        // GS
        { SolidnessStatus.Ghost, SolidnessStatus.Ghost },
        { SolidnessStatus.Solid, SolidnessStatus.Solid },
    };
    private static SolidnessStatus[,] mask2 = new SolidnessStatus[,]
    {
        // GG
        // SS
        { SolidnessStatus.Solid, SolidnessStatus.Ghost },
        { SolidnessStatus.Solid, SolidnessStatus.Ghost },
    };

    [SerializeField]
    private Sprite sprite1;
    [SerializeField]
    private Sprite sprite2;
    [SerializeField]
    private Sprite sprite3;
    [SerializeField]
    private Sprite sprite4;

    private int index = 0;
    private float accumulatedTime = 0;
    private SpriteRenderer renderer;
    private SolidnessObjectStrategy solidnessStrategy;

    private void Rotate(int index)
    {
        switch(index)
        {
            case 0:
                renderer.sprite = sprite1;
                solidnessStrategy.Mask = mask1;
                break;
            case 1:
                renderer.sprite = sprite2;
                solidnessStrategy.Mask = mask2;
                break;
            case 2:
                renderer.sprite = sprite3;
                solidnessStrategy.Mask = mask3;
                break;
            case 3:
                renderer.sprite = sprite4;
                solidnessStrategy.Mask = mask4;
                break;
        }
    }

    // Use this for initialization
    void Start () {
        if (GetComponent<ObjectStrategyHolder>().ObjectStrategy != GetComponent<SimpleObjectStrategy>())
        {
            Destroy(gameObject);
        }
        renderer = GetComponent<SpriteRenderer>();
        solidnessStrategy = GetComponent<SolidnessObjectStrategy>();
        solidnessStrategy.Solidness = SolidnessStatus.Mask;
	}
	
	// Update is called once per frame
	void Update () {
        accumulatedTime += Time.deltaTime;
        if (accumulatedTime > 3f)
        {
            accumulatedTime -= 3f;
            index++;
            if (index == 4) index = 0;
            Rotate(index);
        }
	}
}
