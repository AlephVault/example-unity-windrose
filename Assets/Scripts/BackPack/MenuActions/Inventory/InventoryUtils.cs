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
				// TODO change these ones later.
				private const Vector2 windowSize = new Vector2(360, 110);
				private const Vector2 windowPos = new Vector2(300, 200);

                public class CreateSingleInventoryViewWindow : EditorWindow
                {
					public Transform selectedTransform = null;

                    // Main container properties
                    private Color backgroundColor = Color.white;
                    private int gapSize = 4;
                    private int rows = 2;
                    private int columns = 5;

                    // Properties of the selection label
                    private int selectedItemLabelHeight = 20;
                    private int selectedItemLabelFontSize = 19;

                    // Properties of the pagination controls
                    private int pageLabelHeight = 20;
                    private int pageLabelFontSize = 20;
                    private ColorBlock prevPageButtonColor = MenuActionUtils.DefaultColors();
                    private ColorBlock nextPageButtonColor = MenuActionUtils.DefaultColors();

                    // Properties of the cell
                    private Color backgroundCellColor = new Color32(200, 200, 200, 255);
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
							"satisfies both requirements and so it can be connected to the view being created by this wizard)"
						);

						///////////////////////////////////////////////////////
						/// First, fix some values and derivate the other ones.
						///////////////////////////////////////////////////////

						bool customGapsForCellDisabled = !useCustomGapsForCells;

						if (customGapsForCellDisabled) {
							horizontalCellGapSize = gapSize;
							verticalCellGapSize = gapSize;
						}

						// Now the derivated values:
						int cellWidth = 2 * horizontalCellPadding + iconWidth;
						int cellHeight = 2 * verticalCellPadding + iconHeight;
						int gridWidth = columns * cellWidth + (columns - 1) * horizontalCellGapSize + 1;
						int gridHeight = rows * cellHeight + (rows - 1) * verticalCellGapSize + 1;
						int controlWidth = gridWidth + 2 * gapSize;
						int controlHeight = gridHeight + pageLabelHeight + selectedItemLabelHeight + 4 * gapSize;
						int buttonHeight = pageLabelHeight;
						int buttonWidth = pageLabelHeight;
						int pageLabelWidth = controlWidth - 4 * gapSize - 2 * buttonWidth;
						int selectedItemLabelWidth = gridWidth;

						//////////////////////////////////
						/// Now, the appropriate controls.
						//////////////////////////////////
                    }

                    private void Execute()
                    {

                    }
                }

				/// <summary>
				///   This method is used in the assets menu action: GameObject > Back Pack > Inventory > Create Basic Inventory.
				/// </summary>
				[MenuItem("GameObject/Back Pack/Inventory/Create Basic Inventory", false, 11)]
				public static void AddBag()
				{
					CreateSingleInventoryViewWindow window = ScriptableObject.CreateInstance<CreateSingleInventoryViewWindow>();
					window.selectedTransform = Selection.activeTransform;
					window.position = new Rect(windowPos, windowSize);
					window.ShowUtility();
				}

				[MenuItem("GameObject/Back Pack/Inventory/Create Basic Inventory", true)]
				public static bool CanAddBag()
				{
					Selection.activeTransform != null && Selection.activeTransform.GetComponent<Canvas>();
				}
            }
        }
    }
}
