using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WindRose.Behaviours.World;

public class Pausable : MonoBehaviour {

    private Map map;

    private void Update()
    {
        if (!map)
        {
            map = GetComponent<Map>();
        }

        if (map)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                map.Pause(true);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                map.Pause(false);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                map.Resume();
            }
        }
    }

}
