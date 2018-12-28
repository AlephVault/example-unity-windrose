using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindRose.Types.Inventory.Stacks;
using WindRose.Behaviours.Objects;
using WindRose.Behaviours.World.Layers.Drop;
using WindRose.ScriptableObjects.Inventory.Items;
using WindRose.ScriptableObjects.Inventory.Items.QuantifyingStrategies;

[RequireComponent(typeof(Positionable))]
public class SampleMagicDropper : MonoBehaviour {
    private Positionable positionable;
    private bool allowedToAct = true;
    private System.Random random = new System.Random();

    [SerializeField]
    private List<Item> chances = new List<Item>();

    void Start()
    {
        positionable = GetComponent<Positionable>();
    }

    void Update () {
        if (!positionable.Paused && positionable.ParentMap != null && allowedToAct)
        {
            DropLayer dropLayer = positionable.ParentMap.DropLayer;
            if (dropLayer)
            {
                if (Input.GetKey(KeyCode.D))
                {
                    dropARandomObject(dropLayer);
                    
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    undropTopObject(dropLayer);
                }
            }
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
            Vector2Int containerPosition = new Vector2Int((int)positionable.X, (int)positionable.Y);
            object finalStackPosition;
            bool pushed = dropLayer.Push(containerPosition, stack, out finalStackPosition);
            if (pushed)
            {
                Debug.Log(string.Format("Could push at {0} with a final position of {1}", containerPosition, finalStackPosition));
            }
        });
    }

    void undropTopObject(DropLayer dropLayer)
    {
        Throttled(delegate ()
        {

        });
    }
}
