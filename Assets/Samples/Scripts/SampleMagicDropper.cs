using AlephVault.Unity.Support.Authoring.Behaviours;
using System.Collections.Generic;
using UnityEngine;
using AlephVault.Unity.WindRose.BackPack.Authoring.Behaviours.World.Layers.Drop;
using AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items;

[RequireComponent(typeof(DropLayer))]
[RequireComponent(typeof(Throttler))]
public class SampleMagicDropper : MonoBehaviour {
    private DropLayer dropLayer;
    private System.Random random = new System.Random();

    [SerializeField]
    private List<Item> chances = new List<Item>();

    [SerializeField]
    private KeyCode key;

    private Throttler throttler;
    private ushort minX;
    private ushort maxX;
    private ushort minY;
    private ushort maxY;

    void Awake()
    {
        throttler = GetComponent<Throttler>();
    }

    void Start()
    {
        dropLayer = GetComponent<DropLayer>();
        ushort mapMidX = (ushort)(dropLayer.Map.Width / 2);
        ushort mapMidY = (ushort)(dropLayer.Map.Height / 2);
        minX = (ushort)(mapMidX - 2);
        maxX = (ushort)(mapMidX + 2);
        minY = (ushort)(mapMidY - 2);
        maxY = (ushort)(mapMidY + 2);
    }

    void Update () {
        if (Input.GetKey(key))
        {
            DropARandomObject();
        }
    }

    void Generate()
    {
        int index = random.Next(0, chances.Count);
        Item item = chances[index];
        AlephVault.Unity.BackPack.Types.Inventory.Stacks.Stack stack;
        if (item.MaxStackQuantity == 100)
        {
            stack = item.Create(25, null);
        }
        else
        {
            stack = item.Create(1, null);
        }
        Vector2Int containerPosition = new Vector2Int(random.Next(minX, maxX), random.Next(minY, maxY));
        dropLayer.Push(containerPosition, stack, out object _);
    }

    void DropARandomObject()
    {
        throttler.Throttled(delegate() {
            for(int i = 0; i < 16; i++)
            {
                Generate();
            }
        });
    }
}
