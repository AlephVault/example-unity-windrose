using UnityEngine;
using GMM.Behaviours;
using WindRose.Behaviours.UI.Inventory.SimpleBag;

[RequireComponent(typeof(Throttler))]
[RequireComponent(typeof(BasicSimpleBagView))]
public class BasicSimpleBagControl : MonoBehaviour {

    [SerializeField]
    private KeyCode dropKey = KeyCode.D;

    [SerializeField]
    private KeyCode pickKey = KeyCode.A;

    private Throttler throttler;

    private BasicSimpleBagView bagView;

    void Awake()
    {
        throttler = GetComponent<Throttler>();
        bagView = GetComponent<BasicSimpleBagView>();
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
