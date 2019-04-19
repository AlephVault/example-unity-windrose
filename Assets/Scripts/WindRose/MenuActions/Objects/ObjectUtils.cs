using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WindRose
{
    namespace MenuActions
    {
        namespace Maps
        {
            using Support.Utils;

            /// <summary>
            ///   Menu actions to create an object inside an Objects layer.
            /// </summary>
            public static class ObjectUtils
            {
                private class CreateObjectWindow : EditorWindow
                {
                    private static int[] addStrategyOptions = new int[] { 0, 1, 2 };
                    private static string[] addStrategyLabels = new string[] { "Simple (includes Solidness and Layout)", "Layout", "Nothing (will be added manually later)" };
                    private static int[] addTriggerOptions = new int[] { 0, 1, 2 };
                    private static string[] addTriggerLabels = new string[] { "No trigger", "Live trigger", "Platform" };

                    public Transform selectedTransform;
                    // Basic properties.
                    private string objectName = "New Object";
                    private Vector2Int objectSize = new Vector2Int(1, 1);
                    // Optional behaviours for movement, animation, orientation.
                    private bool addOriented = false;
                    private bool addStatePicker = false;
                    private bool addMovable = false; // depends on addStatePicker
                    // Optional behaviours to send commands.
                    private bool addCommandSender = false;
                    private bool addTalkSender = false; // depends on addCommandSender.
                    // Optional behaviours for trigger type.
                    private int addTrigger = 0;
                    // Optional behaviours for when trigger type = activator.
                    private bool addCommandReceiver = false; // depends on addTrigger == 1.
                    private bool addTalkReceiver = false; // depends on addCommandReceiver.
                    // Object strategy setup.
                    private int addStrategy = 0;
                    // Bag (local inventory) setup.
                    private bool addBag = false;
                    private bool finiteBag = false; // depends on addBag.
                    private int bagSize = 10; // depends on finiteBag.
                    // TODO: perhaps include another *Utils class to create a teleporter?

                    private void OnGUI()
                    {
                        GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();
                        GUIStyle indentedStyle = MenuActionUtils.GetIndentedStyle();

                        // minSize = new Vector2(643, 250);
                        // maxSize = new Vector2(643, 300);

                        // General settings start here.

                        Rect contentRect = EditorGUILayout.BeginVertical();
                        titleContent = new GUIContent("Wind Rose - Creating a new object");
                        EditorGUILayout.LabelField("This wizard will create an object in the hierarchy of the current scene, under the selected objects layer in the hierarchy.", longLabelStyle);

                        // Object properties.

                        EditorGUILayout.LabelField("This is the name the game object will have when added to the hierarchy.", longLabelStyle);
                        objectName = MenuActionUtils.EnsureNonEmpty(EditorGUILayout.TextField("Object name", objectName), "New Object");

                        EditorGUILayout.LabelField("These are the object properties in the editor. Can be changed later.", longLabelStyle);
                        objectSize = EditorGUILayout.Vector2IntField("Map width (X) and height (Y) [1 to 32767]", objectSize);
                        objectSize = new Vector2Int(Values.Clamp(1, objectSize.x, 32767), Values.Clamp(1, objectSize.y, 32767));

                        addOriented = EditorGUILayout.ToggleLeft("Oriented (Provides orientation - useful if holding RoseAnimated visuals)", addOriented);
                        addStatePicker = EditorGUILayout.ToggleLeft("State Picker (Provides current stae - useful if holding MultiState visuals)", addStatePicker);
                        if (addStatePicker)
                        {
                            EditorGUILayout.BeginVertical(indentedStyle);
                            addMovable = EditorGUILayout.ToggleLeft("Movable (Adds a moving state, and actually performs movement when commanded to)", addMovable);
                            EditorGUILayout.EndVertical();
                        }
                        addCommandSender = EditorGUILayout.ToggleLeft("Close Command Sender (Provides feature to send a custom command to close objects)", addCommandSender);
                        if (addCommandSender)
                        {
                            EditorGUILayout.BeginVertical(indentedStyle);
                            addTalkSender = EditorGUILayout.ToggleLeft("Talk Sender (A particular close command sender that dispatches a talk command to NPCs)", addTalkSender);
                            EditorGUILayout.EndVertical();
                        }
                        addTrigger = EditorGUILayout.IntPopup("Trigge Type", addTrigger, addTriggerLabels, addTriggerOptions);
                        if (addTrigger == 1)
                        {
                            EditorGUILayout.BeginVertical(indentedStyle);
                            addCommandReceiver = EditorGUILayout.ToggleLeft("Command Receiver (Provides feature to NPCs to receive a custom command)", addCommandReceiver);
                            if (addCommandReceiver)
                            {
                                EditorGUILayout.BeginVertical(indentedStyle);
                                addTalkReceiver = EditorGUILayout.ToggleLeft("Talk Receiver (A particular command receiver that understands a talk command)", addTalkReceiver);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                        addStrategy = EditorGUILayout.IntPopup("Object Strategy", addStrategy, addStrategyLabels, addStrategyOptions);
                        addBag = EditorGUILayout.ToggleLeft("Bag (Particular simple inventory - Drawers will not be automatically added!)", addBag);
                        if (addBag)
                        {
                            EditorGUILayout.BeginVertical(indentedStyle);
                            EditorGUILayout.LabelField("Bags involve a specific set of strategies like:\n" +
                                                       "> simple spatial management strategy inside stack containers (finite or infinite, but slot-indexed)\n" +
                                                       "> Single-positioning strategy to locate stack containers (only one stack container: the bag)\n" +
                                                       "> Slot/Drop-styled rendering strategy\n" +
                                                       "All that contained inside a new inventory manager component. For the usage strategy, " +
                                                       "the NULL strategy will be added, which should be changed later or the items in the bag will have no logic.", longLabelStyle);
                            finiteBag = EditorGUILayout.ToggleLeft("Has a limited size", finiteBag);
                            if (finiteBag)
                            {
                                bagSize = EditorGUILayout.IntField("Bag size (>= 0)", bagSize);
                                if (bagSize < 1) bagSize = 1;
                            }
                            EditorGUILayout.EndVertical();
                        }
                        if (GUILayout.Button("Create Object")) Execute();
                        EditorGUILayout.EndVertical();

                        if (contentRect.size != Vector2.zero)
                        {
                            minSize = contentRect.max + contentRect.min;
                            maxSize = minSize;
                        }
                    }

                    private void Execute()
                    {
                        GameObject gameObject = new GameObject(objectName);
                        gameObject.transform.parent = selectedTransform;
                        gameObject.SetActive(false);
                        Layout.AddComponent<Behaviours.Entities.Objects.MapObject>(gameObject, new Dictionary<string, object>() {
                            { "width", (uint)objectSize.x },
                            { "height", (uint)objectSize.y }
                        });
                        if (addOriented)
                        {
                            Layout.AddComponent<Behaviours.Entities.Objects.Oriented>(gameObject);
                        }
                        if (addStatePicker)
                        {
                            Layout.AddComponent<Behaviours.Entities.Objects.StatePicker>(gameObject);
                            if (addMovable)
                            {
                                Layout.AddComponent<Behaviours.Entities.Objects.Movable>(gameObject);
                            }
                        }
                        if (addCommandSender)
                        {
                            Layout.AddComponent<Behaviours.Entities.Objects.CommandExchange.CloseCommandSender>(gameObject);
                            if (addTalkSender)
                            {
                                Layout.AddComponent<Behaviours.Entities.Objects.CommandExchange.Talk.TalkSender>(gameObject);
                            }
                        }
                        switch (addTrigger)
                        {
                            case 1:
                                Layout.AddComponent<BoxCollider2D>(gameObject);
                                Layout.AddComponent<Rigidbody2D>(gameObject);
                                Layout.AddComponent<Behaviours.Entities.Objects.TriggerLive>(gameObject);
                                if (addCommandReceiver)
                                {
                                    Layout.AddComponent<Behaviours.Entities.Objects.CommandExchange.CommandReceiver>(gameObject);
                                    if (addTalkReceiver)
                                    {
                                        Layout.AddComponent<Behaviours.Entities.Objects.CommandExchange.Talk.TalkReceiver>(gameObject);
                                    }
                                }
                                break;
                            case 2:
                                Layout.AddComponent<BoxCollider2D>(gameObject);
                                Layout.AddComponent<Behaviours.Entities.Objects.TriggerPlatform>(gameObject, new Dictionary<string, object>()
                                {
                                    { "innerMarginFactor", 0.25f }
                                });
                                break;
                        }
                        Behaviours.Entities.Objects.Strategies.ObjectStrategy mainStrategy = null;
                        switch(addStrategy)
                        {
                            case 0:
                                Layout.AddComponent<Behaviours.Entities.Objects.Strategies.Base.BaseObjectStrategy>(gameObject);
                                Layout.AddComponent<Behaviours.Entities.Objects.Strategies.Base.LayoutObjectStrategy>(gameObject);
                                Layout.AddComponent<Behaviours.Entities.Objects.Strategies.Solidness.SolidnessObjectStrategy>(gameObject);
                                mainStrategy = Layout.AddComponent<Behaviours.Entities.Objects.Strategies.Simple.SimpleObjectStrategy>(gameObject);
                                break;
                            case 1:
                                Layout.AddComponent<Behaviours.Entities.Objects.Strategies.Base.BaseObjectStrategy>(gameObject);
                                mainStrategy = Layout.AddComponent<Behaviours.Entities.Objects.Strategies.Base.LayoutObjectStrategy>(gameObject);
                                break;
                            default:
                                Debug.LogWarning("An object is being just created with no main strategy. This object will be destroyed on play if no main strategy is set.");
                                break;
                        }
                        // TODO: on the current strategy holder, set strategy=mainStrategy.
                        Behaviours.Entities.Objects.ObjectStrategyHolder currentHolder = gameObject.GetComponent<Behaviours.Entities.Objects.ObjectStrategyHolder>();
                        Layout.SetObjectFieldValues(currentHolder, new Dictionary<string, object>()
                        {
                            { "objectStrategy", mainStrategy }
                        });
                        if (addBag)
                        {
                            Behaviours.Inventory.ManagementStrategies.UsageStrategies.InventoryNullUsageManagementStrategy usageStrategy =
                                Layout.AddComponent<Behaviours.Inventory.ManagementStrategies.UsageStrategies.InventoryNullUsageManagementStrategy>(gameObject);
                            Layout.AddComponent<Behaviours.Inventory.InventoryManagementStrategyHolder>(gameObject, new Dictionary<string, object>() {
                                { "mainUsageStrategy", usageStrategy }
                            });
                            if (finiteBag)
                            {
                                Layout.AddComponent<Behaviours.Inventory.ManagementStrategies.SpatialStrategies.InventoryFiniteSimpleSpatialManagementStrategy>(gameObject, new Dictionary<string, object>()
                                {
                                    { "size", bagSize }
                                });
                            }
                            else
                            {
                                Layout.AddComponent<Behaviours.Inventory.ManagementStrategies.SpatialStrategies.InventoryInfiniteSimpleSpatialManagementStrategy>(gameObject);
                            }
                            Layout.AddComponent<Behaviours.Entities.Objects.Bags.SimpleBag>(gameObject);
                        }
                        gameObject.SetActive(true);
                        Close();
                    }
                }

                /// <summary>
                ///   This method is used in the menu action: GameObject > Wind Rose > Objects > Create Object.
                ///   It creates a <see cref="Behaviours.Entities.Objects.MapObject"/> under the selected objects
                ///     layer, in the scene editor.
                /// </summary>
                [MenuItem("GameObject/Wind Rose/Objects/Create Object", false, 11)]
                public static void CreateObject()
                {
                    CreateObjectWindow window = ScriptableObject.CreateInstance<CreateObjectWindow>();
                    window.position = new Rect(60, 180, 700, 468);
                    window.minSize = new Vector2(700, 244);
                    window.maxSize = new Vector2(700, 464);
                    window.selectedTransform = Selection.activeTransform;
                    // window.position = new Rect(new Vector2(57, 336), new Vector2(689, 138));
                    window.ShowUtility();
                }

                /// <summary>
                ///   Validates the menu item: GameObject > Wind Rose > Objects > Create Object.
                ///   It enables such menu option when an <see cref="Behaviours.World.Layers.Objects.ObjectsLayer"/>
                ///     is selected in the scene editor.
                /// </summary>
                [MenuItem("GameObject/Wind Rose/Objects/Create Object", true)]
                public static bool CanCreateObject()
                {
                    return Selection.activeTransform && Selection.activeTransform.GetComponent<Behaviours.World.Layers.Objects.ObjectsLayer>();
                }
            }
        }
    }
}