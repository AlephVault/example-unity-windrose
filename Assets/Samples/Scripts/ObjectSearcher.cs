using AlephVault.Unity.WindRose.Authoring.Behaviours.World;
using AlephVault.Unity.WindRose.Authoring.Behaviours.World.Layers.Objects;
using UnityEngine;

public class ObjectSearcher : MonoBehaviour
{
    private Camera camera;
    private Map map;
    private Grid grid;
    private ObjectsLayer objects;

    private void Awake()
    {
        camera = Camera.main;
        map = GetComponent<Map>();
        objects = map.GetComponentInChildren<ObjectsLayer>();
        grid = objects.GetComponent<Grid>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = camera.ScreenToWorldPoint(mousePosition);
            Vector3 cell = grid.WorldToCell(worldPosition);
            Debug.Log("Searching objects...");
            foreach(var obj in objects.StrategyHolder.Search((ushort)(int)cell.x, (ushort)(int)cell.y))
            {
                Debug.Log($"Name: {obj.name}");
            }
        }
    }
}