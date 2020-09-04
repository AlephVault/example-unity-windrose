using UnityEngine;
using Mirror;
using NetRose.Behaviours.Entities.Objects;
using NetRose.Behaviours.UI.Inventory;
using BackPack.Behaviours.Inventory.Standard;
using BackPack.Behaviours.UI.Inventory.Basic;
using WindRose.Behaviours.Entities.Objects.Bags;
using NetRose.Behaviours;
using NetRose.Behaviours.UI;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(NetworkedMapObjectFollower))]
[RequireComponent(typeof(NetworkedStandardInventoryView))]
public class SamplePlayer : NetworkBehaviour
{
    // The integrated follower.
    private NetworkedMapObjectFollower follower;

    // The integrated single inventory view.
    private NetworkedStandardInventoryView inventoryView;

    // The inventory of the followed object (if any).
    private StandardInventory inventory;

    // The bag of the followed object (if any).
    private StandardBag bag;

    private void Awake()
    {
        inventoryView = GetComponent<NetworkedStandardInventoryView>();
        follower = GetComponent<NetworkedMapObjectFollower>();
        follower.onTargetChanged.AddListener(delegate (NetworkedMapObject oldObject, NetworkedMapObject newObject)
        {
            if (oldObject)
            {
                oldObject.MapObject.onMovementStarted.RemoveListener(OnMovementStarted);
                inventory = oldObject.GetComponent<StandardInventory>();
                if (inventory)
                {
                    inventory.RenderingStrategy.Broadcaster.RemoveListener(inventoryView);
                }
            }
            if (newObject)
            {
                newObject.MapObject.onMovementStarted.AddListener(OnMovementStarted);
                inventory = newObject.GetComponent<StandardInventory>();
                if (inventory)
                {
                    inventory.RenderingStrategy.Broadcaster.AddListener(inventoryView);
                }
            }

            if (inventory)
            {
                bag = inventory.GetComponent<StandardBag>();
            }
            else
            {
                bag = null;
            }
        });
    }

    private void OnMovementStarted(WindRose.Types.Direction direction)
    {
        follower.Target.MapObject.Orientation = direction;
    }

    /// <summary>
    ///   On clients, a camera will be searched (<see cref="Camera.main" />), and
    ///     also a <see cref="BasicStandardInventoryView"/> will be searched, via
    ///     the tag: "Inventory".
    /// </summary>
    public override void OnStartClient()
    {
        follower.camera = Camera.main;
        GameObject basicViewObj = GameObject.FindGameObjectWithTag("Inventory");
        if (basicViewObj)
        {
            BasicStandardInventoryView basicView = basicViewObj.GetComponent<BasicStandardInventoryView>();
            if (basicView) inventoryView.Broadcaster.AddListener(basicView);
        }
    }

    [Command]
    public void Pick()
    {
        int? pos = null;
        if (bag) bag.Pick(out pos);
    }

    [Command]
    public void Drop(int position)
    {
        if (bag) bag.Drop(position);
    }

    [Command]
    public void Right()
    {
        if (follower.Target)
        {
            follower.Target.MapObject.StartMovement(WindRose.Types.Direction.RIGHT);
        }
    }

    [Command]
    public void Up()
    {
        if (follower.Target)
        {
            follower.Target.MapObject.StartMovement(WindRose.Types.Direction.UP);
        }
    }

    [Command]
    public void Left()
    {
        if (follower.Target)
        {
            follower.Target.MapObject.StartMovement(WindRose.Types.Direction.LEFT);
        }
    }

    [Command]
    public void Down()
    {
        if (follower.Target)
        {
            follower.Target.MapObject.StartMovement(WindRose.Types.Direction.DOWN);
        }
    }
}
