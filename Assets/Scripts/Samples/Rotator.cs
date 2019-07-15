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
    private static SolidObjectMask mask1 = new SolidObjectMask(2, 2, new SolidnessStatus[] {
        SolidnessStatus.Solid, SolidnessStatus.Ghost,
        SolidnessStatus.Solid, SolidnessStatus.Ghost
    });
    private static SolidObjectMask mask2 = new SolidObjectMask(2, 2, new SolidnessStatus[] {
        SolidnessStatus.Solid, SolidnessStatus.Solid,
        SolidnessStatus.Ghost, SolidnessStatus.Ghost
    });
    private static SolidObjectMask mask3 = new SolidObjectMask(2, 2, new SolidnessStatus[] {
        SolidnessStatus.Ghost, SolidnessStatus.Solid,
        SolidnessStatus.Ghost, SolidnessStatus.Solid
    });
    private static SolidObjectMask mask4 = new SolidObjectMask(2, 2, new SolidnessStatus[] {
        SolidnessStatus.Ghost, SolidnessStatus.Ghost,
        SolidnessStatus.Solid, SolidnessStatus.Solid
    });

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
