using UnityEngine;
using Mirror;
using NetRose.Behaviours.Entities.Objects;
using NetRose.Behaviours.UI.Inventory;
using BackPack.Behaviours.Inventory.Standard;
using WindRose.Behaviours.Entities.Objects.Bags;
using NetRose.Behaviours;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(NetworkedStandardInventoryView))]
public class SamplePlayer : NetworkBehaviour
{
    // A sample player keeps several references.
    // In server:
    // 1. The MapObject being tracked.
    // 2. The inventory, of such object, being tracked.
    //    Even better if it is a bag as well, since it
    //    can run Drop/Pick as well. An inventory
    //    listener will be added to this player object
    //    to do that. Only one inventory will be managed.
    // And will follow such MapObject:
    // i. One-to-one object's transform's position.
    // ii. When the object changes scene, the player
    //     will also change scene but via the only
    //     NetworkedSceneLayout instance methods.
    // In client:
    // 1. The player will find the main camera by its
    //    Camera.main method, and also will find the
    //    main UI method as a Basic Standard Inventory
    //    View item with a tag: "MainInventoryView".
    //    Such view, if found, will connect to the
    //    client-side of the networked inventory.
    // 2. The main camera will always follow the player.

    // The underlying identity.
    private NetworkIdentity identity;

    // The integrated single inventory view.
    private NetworkedStandardInventoryView inventoryView;

    // The main camera (cached for speed).
    private Camera mainCamera;

    private void Awake()
    {
        identity = GetComponent<NetworkIdentity>();
        inventoryView = GetComponent<NetworkedStandardInventoryView>();
    }

    // The object being tracked.
    [SyncVar]
    private NetworkIdentity mapObject;

    /// <summary>
    ///   The object being tracked. This stands for the
    ///     object receiving the commands, in the end,
    ///     that are issued from the client. If no object
    ///     is tied, then the whole sample player does
    ///     nothing.
    /// </summary>
    public NetworkedMapObject MapObject
    {
        get
        {
            return mapObject.GetComponent<NetworkedMapObject>();
        }
        set
        {
            mapObject = value ? value.GetComponent<NetworkIdentity>() : null;
        }
    }

    // Updating the object involves following the related
    // map object's position, rotation and scene.
    private void Update()
    {
        if (isServer)
        {
            // Follow the object's position and scene.
            if (mapObject)
            {
                if (gameObject.scene != mapObject.gameObject.scene)
                {
                    NetworkedSceneLayout.Instance.MovePlayer(identity, mapObject.gameObject.scene);
                }
                transform.position = mapObject.transform.position;
                transform.rotation = mapObject.transform.rotation;
            }
            else if (gameObject.scene != NetworkedSceneLayout.Instance.gameObject.scene)
            {
                // Move the player to the main scene.
                NetworkedSceneLayout.Instance.MovePlayer(identity, NetworkedSceneLayout.Instance.gameObject.scene);
            }
        }

        if (isClient)
        {
            // TODO:
            // I must ensure HUD.Focus() is called on a HUD in the client, so the map object is stalked.
            // Also the HUD will contain an inventory, that must be connected accordingly.
        }
    }
}
