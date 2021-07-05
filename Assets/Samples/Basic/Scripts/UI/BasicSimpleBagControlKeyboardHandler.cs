using UnityEngine;
using AlephVault.Unity.Support.Authoring.Behaviours;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.UI.Inventory.StandardBag;

[RequireComponent(typeof(Throttler))]
[RequireComponent(typeof(BasicStandardBagControl))]
public class BasicSimpleBagControlKeyboardHandler : MonoBehaviour {

    [SerializeField]
    private KeyCode dropKey = KeyCode.D;

    [SerializeField]
    private KeyCode pickKey = KeyCode.A;

    private Throttler throttler;

    private BasicStandardBagControl bagView;

    void Awake()
    {
        throttler = GetComponent<Throttler>();
        bagView = GetComponent<BasicStandardBagControl>();
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
