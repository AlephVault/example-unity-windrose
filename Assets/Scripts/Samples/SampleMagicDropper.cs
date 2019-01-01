using Support.Behaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindRose.Behaviours.World.Layers.Drop;
using WindRose.ScriptableObjects.Inventory.Items;
using WindRose.ScriptableObjects.Inventory.Items.QuantifyingStrategies;

[RequireComponent(typeof(DropLayer))]
[RequireComponent(typeof(Throttler))]
public class SampleMagicDropper : MonoBehaviour {
    private DropLayer dropLayer;
    private bool allowedToAct = true;
    private System.Random random = new System.Random();

    [SerializeField]
    private List<Item> chances = new List<Item>();

    [SerializeField]
    private KeyCode key;

    private Throttler throttler;
    private int minX;
    private int maxX;
    private int minY;
    private int maxY;

    void Awake()
    {
        throttler = GetComponent<Throttler>();
    }

    void Start()
    {
        dropLayer = GetComponent<DropLayer>();
        uint mapMidX = dropLayer.Map.Width / 2;
        uint mapMidY = dropLayer.Map.Height / 2;
        minX = (int)mapMidX - 2;
        maxX = (int)mapMidX + 2;
        minY = (int)mapMidY - 2;
        maxY = (int)mapMidY + 2;
    }

    void Update () {
        if (Input.GetKey(key))
        {
            DropARandomObject(dropLayer);
        }
    }

    void Generate()
    {
        int index = random.Next(0, chances.Count);
        Item item = chances[index];
        WindRose.Types.Inventory.Stacks.Stack stack;
        if (item.QuantifyingStrategy is ItemUnstackedQuantifyingStrategy)
        {
            stack = item.Create(true, null);
        }
        else
        {
            stack = item.Create(25, null);
        }
        Vector2Int containerPosition = new Vector2Int(random.Next(minX, maxX), random.Next(minY, maxY));
        object finalStackPosition;
        bool pushed = dropLayer.Push(containerPosition, stack, out finalStackPosition);
    }

    void DropARandomObject(DropLayer dropLayer)
    {
        throttler.Throttled(delegate() {
            for(int i = 0; i < 16; i++)
            {
                Generate();
            }
        });
    }
}
