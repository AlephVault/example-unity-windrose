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
                public class CreateSingleInventoryViewWindow : EditorWindow
                {
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
                    private bool useCustomGapsForCells;
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
                        
                    }

                    private void Execute()
                    {

                    }
                }
            }
        }
    }
}
