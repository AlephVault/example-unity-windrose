using System;
using AlephVault.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using AlephVault.Unity.WindRose.Types;
using UnityEngine;

[RequireComponent(typeof(MapObject))]
public class RelativeLookups : MonoBehaviour
{
    private MapObject obj;
    
    private void Awake()
    {
        obj = GetComponent<MapObject>();
    }

    private void LogPair(string text, Tuple<int, int> value)
    {
        Debug.Log($"Pos: {text}({value.Item1}, {value.Item2})");
    }
    
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            Debug.Log("Delta with offset mode");
            Debug.Log($"Pos: Center({obj.X}, {obj.Y})");
            LogPair("L", obj.Orientation.Delta(new Tuple<int, int>(-1, 0)));
            LogPair("F", obj.Orientation.Delta(new Tuple<int, int>(0, 1)));
            LogPair("FF", obj.Orientation.Delta(new Tuple<int, int>(0, 2)));
            LogPair("R", obj.Orientation.Delta(new Tuple<int, int>(1, 0)));
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
        {
            Debug.Log("Delta with steps mode");
            Debug.Log($"Pos: Center({obj.X}, {obj.Y})");
            LogPair("L", obj.Orientation.Delta(Direction.LEFT));
            LogPair("F", obj.Orientation.Delta(Direction.UP));
            LogPair("FF", obj.Orientation.Delta(Direction.UP, Direction.UP));
            LogPair("R", obj.Orientation.Delta(Direction.RIGHT));
        }
    }
}
