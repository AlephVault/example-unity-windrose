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
            using Support.Utils;

            /// <summary>
            ///   Menu actions to create inventory view components.
            /// </summary>
            public class InventoryUtils
            {
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
                    private int selectedItemLabelHeight = 20;
                    private int selectedItemLabelFontSize = 19;

                    // Properties of the pagination controls
                    private int pageLabelHeight = 20;
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
                    private int labelHeight = 11;
                    private int labelBottomMargin = 2;
                    private int labelFontSize = 11;

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
                        pageLabelFontSize = EditorGUILayout.IntField("Page Label Fond Size", pageLabelFontSize);
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
                        int cellWidth = EditorGUILayout.IntField("Final Width", 2 * horizontalCellPadding + iconWidth);
                        int cellHeight = EditorGUILayout.IntField("Final Height", 2 * verticalCellPadding + iconHeight);
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();

                        EditorGUILayout.Space();

                        EditorGUILayout.LabelField("Grid Styles");
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.BeginHorizontal();
                        int gridWidth = EditorGUILayout.IntField("Final Grid Width", columns * cellWidth + (columns - 1) * horizontalCellGapSize + 1);
                        int gridHeight = EditorGUILayout.IntField("Final Grid Height", rows * cellHeight + (rows - 1) * verticalCellGapSize + 1);
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();

                        EditorGUILayout.Space();

                        EditorGUILayout.LabelField("Final Header Controls' Styles");
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.BeginHorizontal();
                        int buttonWidth = EditorGUILayout.IntField("Final Button Width", pageLabelHeight);
                        int buttonHeight = EditorGUILayout.IntField("Final Button Height", pageLabelHeight);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        int pageLabelWidth = gridWidth - 2 * gapSize - 2 * buttonWidth;
                        pageLabelWidth = Values.Max(0, pageLabelWidth);
                        EditorGUILayout.Space();
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();

                        EditorGUILayout.Space();

                        EditorGUILayout.LabelField("Selected Item Label Styles");
                        EditorGUILayout.BeginHorizontal();
                        selectedItemLabelHeight = EditorGUILayout.IntField("Label Height", selectedItemLabelHeight);
                        EditorGUI.BeginDisabledGroup(true);
                        int selectedItemLabelWidth = gridWidth;
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();
                        selectedItemLabelFontSize = EditorGUILayout.IntField("Font Size", selectedItemLabelFontSize);

                        EditorGUILayout.Space();

                        EditorGUILayout.LabelField("Final Control Styles");
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginDisabledGroup(true);
                        int controlWidth = EditorGUILayout.IntField("Overall Width", gridWidth + 2 * gapSize);
                        int controlHeight = EditorGUILayout.IntField("Overall Height", gridHeight + pageLabelHeight + selectedItemLabelHeight + 4 * gapSize);
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();

                        if (GUILayout.Button("Create Inventory")) Execute();
                    }

                    private void Execute()
                    {
                        Close();
                    }
                }

				/// <summary>
				///   This method is used in the assets menu action: GameObject > Back Pack > Inventory > Create Basic Inventory.
				/// </summary>
				[MenuItem("GameObject/Back Pack/Inventory/Create Basic Inventory", false, 11)]
				public static void AddBasicInventory()
				{
					CreateBasicInventoryViewWindow window = ScriptableObject.CreateInstance<CreateBasicInventoryViewWindow>();
					window.selectedTransform = Selection.activeTransform;
					window.position = new Rect(new Vector2(139, 247), Vector2.zero);
					window.ShowUtility();
				}

				[MenuItem("GameObject/Back Pack/Inventory/Create Basic Inventory", true)]
				public static bool CanAddBasicInventory()
				{
					return Selection.activeTransform != null && Selection.activeTransform.GetComponent<Canvas>();
				}
            }
        }
    }
}
