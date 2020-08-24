﻿using UnityEngine;
using GMM.Behaviours;
using WindRose.Behaviours.UI.Inventory.StandardBag;

[RequireComponent(typeof(Throttler))]
[RequireComponent(typeof(WindRose.Behaviours.UI.Inventory.StandardBag.BasicStandardInventoryControl))]
public class BasicSimpleBagControlKeyboardHandler : MonoBehaviour {

    [SerializeField]
    private KeyCode dropKey = KeyCode.D;

    [SerializeField]
    private KeyCode pickKey = KeyCode.A;

    private Throttler throttler;

    private WindRose.Behaviours.UI.Inventory.StandardBag.BasicStandardInventoryControl bagView;

    void Awake()
    {
        throttler = GetComponent<Throttler>();
        bagView = GetComponent<WindRose.Behaviours.UI.Inventory.StandardBag.BasicStandardInventoryControl>();
    }

    void Update()
    {
        if (Input.GetKey(dropKey))
        {
            throttler.Throttled(bagView.DropSelected);
        }
        else if (Input.GetKey(pickKey))
        {
            throttler.Throttled(bagView.Pick);
        }
    }

    
}
