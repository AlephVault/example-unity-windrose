using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindRose.Behaviours.World.Layers.Drop;
using WindRose.ScriptableObjects.Inventory.Items;
using WindRose.ScriptableObjects.Inventory.Items.QuantifyingStrategies;

[RequireComponent(typeof(DropLayer))]
public class SampleMagicDropper : MonoBehaviour {
    private DropLayer dropLayer;
    private bool allowedToAct = true;
    private System.Random random = new System.Random();

    [SerializeField]
    private List<Item> chances = new List<Item>();

    [SerializeField]
    private KeyCode key;

    void Start()
    {
        dropLayer = GetComponent<DropLayer>();
    }

    void Update () {
        if (Input.GetKey(key))
        {
            dropARandomObject(dropLayer);
        }
    }

    IEnumerator Unthrottle()
    {
        yield return new WaitForSeconds(1f);
        allowedToAct = true;
    }

    void Throttled(Action doIt)
    {
        if (allowedToAct)
        {
            allowedToAct = false;
            doIt();
            StartCoroutine(Unthrottle());
        }
    }

    void dropARandomObject(DropLayer dropLayer)
    {
        Throttled(delegate ()
        {
            int index = random.Next(0, chances.Count);
            Item item = chances[index];
            WindRose.Types.Inventory.Stacks.Stack stack;
            if (item.QuantifyingStrategy is ItemUnstackedQuantifyingStrategy)
            {
                stack = item.Create(null, null);
            }
            else
            {
                stack = item.Create(25, null);
            }
            Vector2Int containerPosition = new Vector2Int(random.Next(0, (int)dropLayer.Map.Width), random.Next(0, (int)dropLayer.Map.Height));
            object finalStackPosition;
            bool pushed = dropLayer.Push(containerPosition, stack, out finalStackPosition);
        });
    }
}
