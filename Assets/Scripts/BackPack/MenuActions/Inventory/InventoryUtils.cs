using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace BackPack
{
    namespace MenuActions
    {
        namespace Inventory
        {
            using GMM.Utils;
            using Behaviours.UI.Inventory.Basic;

            /// <summary>
            ///   Menu actions to create inventory view components.
            /// </summary>
            public class InventoryUtils
            {
                private static Sprite background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
                private static Sprite sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                private static Sprite inputFieldBackground = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");

                public class CreateBasicInventoryViewWindow : EditorWindow
                {
					public Transform selectedTransform = null;

                    // TODO change these ones later.
                    private Vector2 windowSize = new Vector2(963, 689);

                    // Main container properties
                    private Color backgroundColor = Color.white;
                    private int gapSize = 4;
                    private int rows = 2;
                    private int columns = 5;

                    // Properties of labels
                    private int selectedItemLabelHeight = 40;
                    private int selectedItemLabelFontSize = 20;

                    // Properties of the pagination controls
                    private int pageLabelHeight = 40;
                    private int pageLabelFontSize = 20;
                    private ColorBlock prevPageButtonColor = MenuActionUtils.DefaultColors();
                    private ColorBlock nextPageButtonColor = MenuActionUtils.DefaultColors();

                    // Properties of the cell
                    private Color cellColor = new Color32(200, 200, 200, 255);
                    private bool useCustomGapsForCells = false;
                    private int verticalCellGapSize = 4;
                    private int horizontalCellGapSize = 4;
                    private int verticalCellPadding = 9;
                    private int horizontalCellPadding = 9;

                    // Properties of the cell's selection glow
                    private Color selectionGlowColor = new Color32(191, 159, 0, 63);

                    // Properties of the cell's icon
                    private int iconHeight = 32;
                    private int iconWidth = 32;

                    // Properties of the cell's label
                    private int labelWidth = 40;
                    private int labelHeight = 20;
                    private int labelBottomMargin = 2;
                    private int labelFontSize = 10;

                    // Derivated data:
                    int cellWidth, cellHeight;
                    int gridWidth, gridHeight;
                    int buttonWidth, buttonHeight;
                    int pageLabelWidth, selectedItemLabelWidth;
                    int controlWidth, controlHeight;

                    private void OnGUI()
                    {
						minSize = windowSize;
						maxSize = minSize;
						GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();

						titleContent = new GUIContent("Back Pack - Creating a new HUD simple & single inventory view");
						EditorGUILayout.LabelField(
							"This wizard will create a new view for a simple & single inventory under the selected HUD canvas. " +
							"There is a default implementation (called the 'basic single & simple' one) that will be used, and " +
							"is compatible with single-container inventories, and simple (icon, text, quantity) items, so the " +
							"inventory to connect must be compatible (this package provides the Single Simple Inventory which " +
							"satisfies both requirements and so it can be connected to the view being created by this wizard).",
                            longLabelStyle
						);

                        Debug.LogFormat("Size: {0}, Position: {1}", position.size, position.position);

                        EditorGUILayout.Space();

                        /// General Section Styles
                        EditorGUILayout.LabelField("General Styles", longLabelStyle);
                        EditorGUILayout.BeginHorizontal();
                        backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);
                        gapSize = EditorGUILayout.IntField("Gap Size", gapSize);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        rows = EditorGUILayout.IntField("Rows", rows);
                        columns = EditorGUILayout.IntField("Columns", columns);
                        EditorGUILayout.EndHorizontal();
                        // Fix some values (according to restrictions).
                        gapSize = Values.Max(0, gapSize);
                        rows = Values.Max(1, rows);
                        columns = Values.Max(1, columns);

                        EditorGUILayout.Space();

                        /// Header Styles
                        EditorGUILayout.LabelField("Header Controls' Styles", longLabelStyle);
                        EditorGUILayout.BeginHorizontal();
                        pageLabelHeight = EditorGUILayout.IntField("Page Label Height", pageLabelHeight);
                        pageLabelFontSize = EditorGUILayout.IntField("Page Label Font Size", pageLabelFontSize);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField("\"Previos\" Button");
                        prevPageButtonColor = MenuActionUtils.ColorsGUI(prevPageButtonColor);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField("\"Next\" Button");
                        nextPageButtonColor = MenuActionUtils.ColorsGUI(nextPageButtonColor);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        // Fix some values (according to restrictions).
                        pageLabelFontSize = Values.Max(1, pageLabelFontSize);
                        pageLabelHeight = Values.Max(1, pageLabelHeight);

                        EditorGUILayout.Space();

                        /// Cell Styles
                        EditorGUILayout.LabelField("Cell Styles", longLabelStyle);
                        EditorGUILayout.BeginHorizontal();
                        cellColor = EditorGUILayout.ColorField("Background Color", cellColor);
                        useCustomGapsForCells = EditorGUILayout.ToggleLeft("Use Custom Gaps", useCustomGapsForCells);
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.BeginDisabledGroup(!useCustomGapsForCells);
                        EditorGUILayout.BeginHorizontal();
                        horizontalCellGapSize = EditorGUILayout.IntField("Horizontal Gap", useCustomGapsForCells ? horizontalCellGapSize : gapSize);
                        verticalCellGapSize = EditorGUILayout.IntField("Vertical Gap", useCustomGapsForCells ? verticalCellGapSize : gapSize);
                        horizontalCellGapSize = Values.Max(0, horizontalCellGapSize);
                        verticalCellGapSize = Values.Max(0, verticalCellGapSize);
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.BeginHorizontal();
                        horizontalCellPadding = EditorGUILayout.IntField("Horizontal Padding", horizontalCellPadding);
                        verticalCellPadding = EditorGUILayout.IntField("Vertical Padding", verticalCellPadding);
                        horizontalCellPadding = Values.Max(0, horizontalCellPadding);
                        verticalCellPadding = Values.Max(0, verticalCellPadding);                       
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();

                        EditorGUILayout.LabelField("Cell Content Styles", longLabelStyle);
                        EditorGUILayout.BeginHorizontal();
                        iconWidth = EditorGUILayout.IntField("Icon Width", iconWidth);
                        iconHeight = EditorGUILayout.IntField("Icon Height", iconHeight);
                        iconWidth = Values.Max(0, iconWidth);
                        iconHeight = Values.Max(0, iconHeight);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        labelWidth = EditorGUILayout.IntField("Label Width", labelWidth);
                        labelHeight = EditorGUILayout.IntField("Label Height", labelHeight);
                        labelWidth = Values.Max(0, labelWidth);
                        labelHeight = Values.Max(0, labelHeight);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        labelFontSize = EditorGUILayout.IntField("Label Font Size", labelFontSize);
                        labelBottomMargin = EditorGUILayout.IntField("Bottom Margin", labelBottomMargin);
                        labelFontSize = Values.Max(0, labelFontSize);
                        labelBottomMargin = Values.Max(0, labelBottomMargin);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();

                        EditorGUILayout.LabelField("Final Cell Styles", longLabelStyle);
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.BeginHorizontal();
                        cellWidth = EditorGUILayout.IntField("Final Width", 2 * horizontalCellPadding + iconWidth);
                        cellHeight = EditorGUILayout.IntField("Final Height", 2 * verticalCellPadding + iconHeight);
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();

                        EditorGUILayout.Space();

                        EditorGUILayout.LabelField("Grid Styles");
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.BeginHorizontal();
                        gridWidth = EditorGUILayout.IntField("Final Grid Width", columns * cellWidth + (columns - 1) * horizontalCellGapSize + 1);
                        gridHeight = EditorGUILayout.IntField("Final Grid Height", rows * cellHeight + (rows - 1) * verticalCellGapSize + 1);
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();

                        EditorGUILayout.Space();

                        EditorGUILayout.LabelField("Final Header Controls' Styles");
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.BeginHorizontal();
                        buttonWidth = EditorGUILayout.IntField("Final Button Width", pageLabelHeight);
                        buttonHeight = EditorGUILayout.IntField("Final Button Height", pageLabelHeight);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        pageLabelWidth = gridWidth - 2 * gapSize - 2 * buttonWidth;
                        pageLabelWidth = Values.Max(0, pageLabelWidth);
                        EditorGUILayout.Space();
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();

                        EditorGUILayout.Space();

                        EditorGUILayout.LabelField("Selected Item Label Styles");
                        EditorGUILayout.BeginHorizontal();
                        selectedItemLabelHeight = EditorGUILayout.IntField("Label Height", selectedItemLabelHeight);
                        EditorGUI.BeginDisabledGroup(true);
                        selectedItemLabelWidth = gridWidth;
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();
                        selectedItemLabelFontSize = EditorGUILayout.IntField("Font Size", selectedItemLabelFontSize);

                        EditorGUILayout.Space();

                        EditorGUILayout.LabelField("Final Control Styles");
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginDisabledGroup(true);
                        controlWidth = EditorGUILayout.IntField("Overall Width", gridWidth + 2 * gapSize);
                        controlHeight = EditorGUILayout.IntField("Overall Height", gridHeight + pageLabelHeight + selectedItemLabelHeight + 4 * gapSize);
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();

                        if (GUILayout.Button("Create Inventory")) Execute();
                    }

                    private void MakeInventoryContainer(GameObject inventory)
                    {
                        Image inventoryBackground = inventory.AddComponent<Image>();
                        RectTransform inventoryRectTransform = inventory.GetComponent<RectTransform>();
                        inventory.AddComponent<BasicSingleSimpleInventoryView>();
                        Vector2 v01 = new Vector2(0, 1);
                        inventoryRectTransform.localScale = Vector3.one;
                        inventoryRectTransform.anchorMin = v01;
                        inventoryRectTransform.anchorMax = v01;
                        inventoryRectTransform.pivot = v01;
                        inventoryRectTransform.offsetMin = new Vector2(0, -controlHeight);
                        inventoryRectTransform.offsetMax = new Vector2(controlWidth, 0);
                        inventoryBackground.sprite = background;
                        inventoryBackground.type = Image.Type.Sliced;
                        inventoryBackground.color = backgroundColor;
                    }

                    private void MakePrevButtonControl(GameObject inventory)
                    {
                        Vector2 position = new Vector2(gapSize, 3 * gapSize + selectedItemLabelHeight + gridHeight);
                        Button button = MenuActionUtils.AddButton(inventory.GetComponent<RectTransform>(), position, new Vector2(buttonWidth, buttonHeight), "◀◀", "Prev", Color.black, prevPageButtonColor);
                        button.gameObject.AddComponent<BasicSingleSimpleInventoryViewPrevButton>();
                    }

                    private void MakeNextButtonControl(GameObject inventory)
                    {
                        Vector2 position = new Vector2(3 * gapSize + buttonWidth + pageLabelWidth, 3 * gapSize + selectedItemLabelHeight + gridHeight);
                        Button button = MenuActionUtils.AddButton(inventory.GetComponent<RectTransform>(), position, new Vector2(buttonWidth, buttonHeight), "▶▶", "Next", Color.black, nextPageButtonColor);
                        button.gameObject.AddComponent<BasicSingleSimpleInventoryViewNextButton>();
                    }

                    private void MakePageLabelControl(GameObject inventory)
                    {
                        GameObject pageLabelControl = new GameObject("PageLabel");
                        Text pageLabelText = pageLabelControl.AddComponent<Text>();
                        RectTransform pageLabelRectTransform = pageLabelControl.GetComponent<RectTransform>();
                        Vector2 position = new Vector2(2 * gapSize + buttonWidth, 3 * gapSize + selectedItemLabelHeight + gridHeight);
                        Vector2 size = new Vector2(pageLabelWidth, pageLabelHeight);
                        pageLabelRectTransform.SetParent(inventory.GetComponent<RectTransform>(), false);
                        pageLabelRectTransform.anchorMin = Vector2.zero;
                        pageLabelRectTransform.anchorMax = Vector2.zero;
                        pageLabelRectTransform.pivot = Vector2.zero;
                        pageLabelRectTransform.offsetMin = position;
                        pageLabelRectTransform.offsetMax = position + size;
                        pageLabelText.alignment = TextAnchor.MiddleCenter;
                        pageLabelText.fontSize = pageLabelFontSize;
                        pageLabelText.text = "(page) / (total)";
                        pageLabelText.color = Color.black;
                        pageLabelControl.AddComponent<BasicSingleSimpleInventoryViewPageLabel>();
                    }

                    private void MakeHeaderControls(GameObject inventory)
                    {
                        MakeNextButtonControl(inventory);
                        MakePageLabelControl(inventory);
                        MakePrevButtonControl(inventory);
                    }

                    private void MakeSelectionLabelControl(GameObject inventory)
                    {
                        GameObject selectedItemLabelControl = new GameObject("SelectedItemLabel");
                        Text selectedItemLabelText = selectedItemLabelControl.AddComponent<Text>();
                        RectTransform selectedItemLabelRectTransform = selectedItemLabelControl.GetComponent<RectTransform>();
                        Vector2 position = new Vector2(gapSize, gapSize);
                        Vector2 size = new Vector2(selectedItemLabelWidth, selectedItemLabelHeight);
                        selectedItemLabelRectTransform.SetParent(inventory.GetComponent<RectTransform>(), false);
                        selectedItemLabelRectTransform.anchorMin = Vector2.zero;
                        selectedItemLabelRectTransform.anchorMax = Vector2.zero;
                        selectedItemLabelRectTransform.pivot = Vector2.zero;
                        selectedItemLabelRectTransform.offsetMin = position;
                        selectedItemLabelRectTransform.offsetMax = position + size;
                        selectedItemLabelText.alignment = TextAnchor.MiddleCenter;
                        selectedItemLabelText.fontSize = selectedItemLabelFontSize;
                        selectedItemLabelText.text = "";
                        selectedItemLabelText.color = Color.black;
                        selectedItemLabelControl.AddComponent<BasicSingleSimpleInventoryViewSelectedItemLabel>();
                    }

                    private GameObject MakeGridControl(GameObject inventory)
                    {
                        GameObject gridControl = new GameObject("Slots");
                        GridLayoutGroup gridLayoutGroup = gridControl.AddComponent<GridLayoutGroup>();
                        gridLayoutGroup.cellSize = new Vector3(cellWidth, cellHeight);
                        gridLayoutGroup.spacing = new Vector3(horizontalCellGapSize, verticalCellGapSize);
                        RectTransform gridRectTransform = gridControl.GetComponent<RectTransform>();
                        Vector2 position = new Vector2(gapSize, 2 * gapSize + selectedItemLabelHeight);
                        Vector2 size = new Vector2(gridWidth, gridHeight);
                        gridRectTransform.SetParent(inventory.transform, false);
                        gridRectTransform.anchorMin = Vector2.zero;
                        gridRectTransform.anchorMax = Vector2.zero;
                        gridRectTransform.pivot = Vector2.zero;
                        gridRectTransform.offsetMin = position;
                        gridRectTransform.offsetMax = position + size;
                        return gridControl;
                    }

					private void AddIcon(GameObject slot)
					{
						GameObject icon = new GameObject("Icon");
						Image iconImage = icon.AddComponent<Image>();
						iconImage.enabled = false;
						RectTransform iconRectTransform = icon.GetComponent<RectTransform>();
						iconRectTransform.SetParent(slot.transform, false);
						iconRectTransform.pivot = Vector2.one / 2;
						iconRectTransform.anchorMin = Vector2.one / 2;
						iconRectTransform.anchorMax = Vector2.one / 2;
						iconRectTransform.localPosition = Vector3.zero;
						iconRectTransform.offsetMin = new Vector2(iconWidth, iconHeight) / -2;
						iconRectTransform.offsetMax = new Vector2(iconWidth, iconHeight) / 2;
						icon.AddComponent<BasicSingleSimpleInventoryViewItemIcon>();
					}

					private void AddSelectionGlow(GameObject slot)
					{
						GameObject selectionGlow = new GameObject("SelectionGlow");
						Image selectionGlowImage = selectionGlow.AddComponent<Image>();
						selectionGlowImage.sprite = sprite;
						selectionGlowImage.type = Image.Type.Sliced;
						selectionGlowImage.fillCenter = true;
						selectionGlowImage.color = selectionGlowColor;
						RectTransform selectionGlowRectTransform = selectionGlow.GetComponent<RectTransform>();
						selectionGlowRectTransform.SetParent(slot.transform, false);
						selectionGlowRectTransform.pivot = Vector2.one / 2;
						selectionGlowRectTransform.anchorMin = Vector2.one / 2;
						selectionGlowRectTransform.anchorMax = Vector2.one / 2;
						selectionGlowRectTransform.localPosition = Vector3.zero;
						selectionGlowRectTransform.offsetMin = new Vector2(cellWidth, cellHeight) / -2;
						selectionGlowRectTransform.offsetMax = new Vector2(cellWidth, cellHeight) / 2;
						selectionGlow.AddComponent<BasicSingleSimpleInventoryViewItemSelectionGlow>();
					}

					private void AddQuantityLabel(GameObject slot)
					{
						GameObject quantity = new GameObject("Quantity");
						Text quantityText = quantity.AddComponent<Text>();
						quantityText.fontSize = labelFontSize;
						quantityText.alignment = TextAnchor.MiddleCenter;
						quantityText.color = Color.black;
						RectTransform quantityRectTransform = quantity.GetComponent<RectTransform>();
						quantityRectTransform.pivot = new Vector2(0.5f, 0.0f);
						quantityRectTransform.anchorMin = new Vector2(0.5f, 0.0f);
						quantityRectTransform.anchorMax = new Vector2(0.5f, 0.0f);
						quantityRectTransform.offsetMin = new Vector2(-labelWidth/2, 0.0f);
						quantityRectTransform.offsetMax = new Vector2(labelWidth/2, labelHeight);
						quantityRectTransform.localPosition = new Vector3(0.0f, labelBottomMargin, 0.0f);
						quantityRectTransform.SetParent(slot.transform, false);
						quantity.AddComponent<BasicSingleSimpleInventoryViewItemQuantityLabel>();
					}

					private void MakeSlotControl(GameObject parentGrid, int row, int column)
                    {
						GameObject slot = new GameObject(string.Format("Slot[{0},{1}]", row, column));
						Image slotBackground = slot.AddComponent<Image>();
						RectTransform slotRectTransform = slot.GetComponent<RectTransform>();
						slotRectTransform.SetParent(parentGrid.transform, false);
						slotBackground.sprite = inputFieldBackground;
						slotBackground.type = Image.Type.Sliced;
						slotBackground.fillCenter = true;
						slotBackground.color = cellColor;
						AddIcon(slot);
						AddSelectionGlow(slot);
						AddQuantityLabel(slot);
						slot.AddComponent<BasicSingleSimpleInventoryViewItem>();
                    }

                    private void MakeBodyControls(GameObject inventory)
                    {
                        GameObject layout = MakeGridControl(inventory);
                        for(int currentRow = 0; currentRow < rows; currentRow++)
                        {
                            for(int currentColumn = 0; currentColumn < columns; currentColumn++)
                            {
                                MakeSlotControl(layout, currentRow, currentColumn);
                            }
                        }
                    }

                    private void Execute()
                    {
                        GameObject inventoryView = new GameObject("Basic Inventory View");
                        inventoryView.transform.parent = selectedTransform;
                        Canvas parentCanvas = selectedTransform.GetComponent<Canvas>();
                        MakeInventoryContainer(inventoryView);
                        MakeHeaderControls(inventoryView);
                        MakeSelectionLabelControl(inventoryView);
                        MakeBodyControls(inventoryView);
                        Undo.RegisterCreatedObjectUndo(inventoryView, "Create Basic Inventory View");
                        Close();
                    }
                }

				/// <summary>
				///   This method is used in the assets menu action: GameObject > Back Pack > Inventory > Create Basic Inventory View.
				/// </summary>
				[MenuItem("GameObject/Back Pack/Inventory/Create Basic Inventory View", false, 11)]
				public static void AddBasicInventory()
				{
					CreateBasicInventoryViewWindow window = ScriptableObject.CreateInstance<CreateBasicInventoryViewWindow>();
					window.selectedTransform = Selection.activeTransform;
					window.position = new Rect(new Vector2(139, 247), Vector2.zero);
					window.ShowUtility();
				}

				[MenuItem("GameObject/Back Pack/Inventory/Create Basic Inventory View", true)]
				public static bool CanAddBasicInventory()
				{
					return Selection.activeTransform != null && Selection.activeTransform.GetComponent<Canvas>();
				}
            }
        }
    }
}
