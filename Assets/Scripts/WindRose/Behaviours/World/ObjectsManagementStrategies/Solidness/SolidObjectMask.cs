using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace ObjectsManagementStrategies
            {
                namespace Solidness
                {
                    using Support.Utils;

                    /// <summary>
                    ///   SolidObjectMask is an abstraction of an object's solidness mask,
                    ///     which has [Width]x[Height] cells, and each cell may onle be one
                    ///     out of three values: Solid, Ghost, Hole. They alter the objects
                    ///     layer's solidness strategy's overall mask in the same way the
                    ///     individual statuses do, but this time per individual cell. This
                    ///     type has only meaning when the owner object uses a Mask solidness
                    ///     type. Otherwise this type has no use.
                    /// </summary>
                    [Serializable]
                    public class SolidObjectMask
                    {
                        /// <summary>
                        ///   A marker attribute telling that this particular property should be
                        ///     automatically clamped by the drawer.
                        /// </summary>
                        public class AutoClampedAttribute : Attribute
                        {
                        }

                        /// <summary>
                        ///   The actual underlying array of statuses.
                        /// </summary>
                        [SerializeField]
                        private SolidnessStatus[] cells;

                        /// <summary>
                        ///   The mask width.
                        /// </summary>
                        [SerializeField]
                        private uint width;

                        /// <summary>
                        ///   The mask height.
                        /// </summary>
                        [SerializeField]
                        private uint height;

                        /// <summary>
                        ///   The mask width.
                        /// </summary>
                        public uint Width { get { return width; } }

                        /// <summary>
                        ///   The mask height.
                        /// </summary>
                        public uint Height { get { return height; } }

                        /// <summary>
                        ///   Creates a zero-sized mask. This one exists as an empty value for serialization.
                        /// </summary>
                        public SolidObjectMask() : this(0, 0, null) {}

                        /// <summary>
                        ///   Creates a new solid mask with the given data.
                        /// </summary>
                        /// <param name="width">The mask's width</param>
                        /// <param name="height">The mask's height</param>
                        /// <param name="cells">The mask's cells. Cells being Mask are an error: they will be converted to Ghost</param>
                        public SolidObjectMask(uint width, uint height, SolidnessStatus[] cells)
                        {
                            if (width == 0 || height == 0)
                            {
                                this.width = 0;
                                this.height = 0;
                                this.cells = null;
                                return;
                            }

                            if (cells == null)
                            {
                                throw new ArgumentNullException("cells");
                            }

                            uint length = width * height;
                            if (length != cells.Length)
                            {
                                throw new ArgumentException("Width and height must multiply to the given array's length");
                            }

                            this.width = width;
                            this.height = height;
                            this.cells = new SolidnessStatus[length];
                            for (int index = 0; index < length; index++)
                            {
                                SolidnessStatus status = cells[index];
                                this.cells[index] = status == SolidnessStatus.Mask ? SolidnessStatus.Ghost : status;
                            }
                        }

                        /// <summary>
                        ///   Gets a single mask position in terms of inner (x, y) coordinates.
                        ///   The (0, 0) point refers the bottom-left corner of the object.
                        /// </summary>
                        /// <param name="x">The given x position to query</param>
                        /// <param name="y">The given y position to query</param>
                        /// <returns>The status at the given position</returns>
                        public SolidnessStatus this[uint x, uint y]
                        {
                            get
                            {
                                if (x >= width) throw new ArgumentOutOfRangeException("x");
                                if (y >= height) throw new ArgumentOutOfRangeException("y");
                                return cells[y * width + x];
                            }
                        }

                        /// <summary>
                        ///   Dumps the content of the array into a new array. This method should be used only
                        ///     on edition or under highly controlled scenarios which do not occur frequently
                        ///     because this will essentially be a performance killer if overused.
                        /// </summary>
                        /// <returns>A copy of the inner cells' statuses array</returns>
                        public SolidnessStatus[] Dump()
                        {
                            if (this.cells == null) return null;
                            int length = this.cells.Length;
                            SolidnessStatus[] cells = new SolidnessStatus[length];
                            for (int index = 0; index < length; index++)
                            {
                                SolidnessStatus status = this.cells[index];
                                cells[index] = status == SolidnessStatus.Mask ? SolidnessStatus.Ghost : status;
                            }
                            return cells;
                        }

                        /// <summary>
                        ///   Copies the current mask into a new size. If one of the dimensions grows and new
                        ///     cells appear, they will be filled by default with the Ghost status, and more
                        ///     precisely with the chosen type, if another one. A new mask will be returned,
                        ///     and the current one will be unaffected.
                        /// </summary>
                        /// <param name="width">The new width</param>
                        /// <param name="height">The new height</param>
                        /// <param name="fill">The value to use when filling new cells</param>
                        /// <returns>A new mask with the modified content</returns>
                        public SolidObjectMask Resized(uint width, uint height, SolidnessStatus fill = SolidnessStatus.Ghost)
                        {
                            if (width == 0 || height == 0)
                            {
                                return new SolidObjectMask();
                            }

                            return new SolidObjectMask(width, height, ResizeAndFill(this.cells, this.width, this.height, width, height, fill));
                        }

                        // Resizes the given source mask contents, given their dimensions, new dimensions and fill.
                        // A new mask contents array is returned. The original is unaffected.
                        private static SolidnessStatus[] ResizeAndFill(SolidnessStatus[] source, uint sourceWidth, uint sourceHeight, uint width, uint height, SolidnessStatus fill)
                        {
                            SolidnessStatus[] newCells = new SolidnessStatus[width * height];
                            uint targetIndex = 0;
                            uint sourceVOffset = 0;
                            uint minWidth = Values.Min(width, sourceWidth);
                            uint minHeight = Values.Min(height, sourceHeight);
                            for (int y = 0; y < minHeight; y++)
                            {
                                for (uint x = 0; x < minWidth; x++)
                                {
                                    SolidnessStatus status = source[sourceVOffset + x];
                                    newCells[targetIndex++] = status == SolidnessStatus.Mask ? SolidnessStatus.Ghost : status;
                                }
                                for (uint x = minWidth; x < width; x++)
                                {
                                    newCells[targetIndex++] = fill;
                                }
                                sourceVOffset += sourceWidth;
                            }
                            for (uint y = minHeight; y < height; y++)
                            {
                                for (uint x = 0; x < width; x++)
                                {
                                    newCells[targetIndex++] = fill;
                                }
                                sourceVOffset += sourceHeight;
                            }
                            return newCells;
                        }

                        /// <summary>
                        ///   Performs a resize of a given mask contents given its size, new size, and fill options. While the mask is 1-dimensional,
                        ///   its source width and height must also be specified to compute it appropriately.
                        /// </summary>
                        /// <param name="source">The mask contents to resize.</param>
                        /// <param name="sourceWidth">The width of the content.</param>
                        /// <param name="sourceHeight">The height of the content.</param>
                        /// <param name="width">The new width.</param>
                        /// <param name="height">The new height.</param>
                        /// <param name="fill">The fill for the new cells.</param>
                        /// <returns></returns>
                        public static SolidnessStatus[] Resized(SolidnessStatus[] source, uint sourceWidth, uint sourceHeight, uint width, uint height, SolidnessStatus fill)
                        {
                            if (width == 0 || height == 0)
                            {
                                return null;
                            }

                            if (sourceWidth * sourceHeight != source.Length)
                            {
                                throw new ArgumentException("Source dimensions do not match the source array");
                            }

                            return ResizeAndFill(source, sourceWidth, sourceHeight, width, height, fill);
                        }

                        /// <summary>
                        ///   Clones the mask into a given one.
                        /// </summary>
                        /// <returns>The cloned mask</returns>
                        public SolidObjectMask Clone()
                        {
                            return new SolidObjectMask(width, height, Dump());
                        }
                    }

                    /// <summary>
                    ///   The drawer for the mask only involves a button invoking a window for mask edition.
                    /// </summary>
                    [CustomPropertyDrawer(typeof(SolidObjectMask))]
                    public class SolidObjectMaskDrawer : PropertyDrawer
                    {
                        // Editor window for solid object masks.
                        private class SolidObjectMaskEditorWindow : EditorWindow
                        {
                            // Tells whether the dimensions have to be unchanged or
                            // may be changed. Dimensions will be unchangeable if the
                            // underlying object is a behaviour whose underlying game
                            // object is a WindRose MapObject.
                            private bool withFixedDimensions = false;

                            // This is the paint mode when clicking a (valid) cell.
                            // Also when not fixed dimensions, this is the value to
                            // fill new cells when resizing the mask.
                            private SolidnessStatus fillWith = SolidnessStatus.Ghost;

                            // Texture for solid cells.
                            private Texture solidCellImage;

                            // Texture for ghost cells.
                            private Texture ghostCellImage;

                            // Texture for hole cells.
                            private Texture holeCellImage;

                            // Texture for invalid cells.
                            private Texture invalidCellImage;

                            // The mask width.
                            private uint maskWidth = 1;

                            // The mask height.
                            private uint maskHeight = 1;

                            // The mask editor's X offset.
                            private uint offsetX = 0;

                            // The mask editor's Y offset.
                            private uint offsetY = 0;

                            // The current owner.
                            private UnityEngine.Object owner;

                            // The actual involved property.
                            private FieldInfo property;

                            // The dumped mask to be edited.
                            private SolidnessStatus[] contents;

                            // Adds a toggle state to the style depending on the toggle value.
                            // The result is the same style if the toggle value is false, while
                            // will have an active background if true.
                            private GUIStyle withToggle(GUIStyle style, bool toggle)
                            {
                                if (toggle)
                                {
                                    style = new GUIStyle(style);
                                    style.normal.background = style.active.background;
                                }
                                return style;
                            }

                            private void DimensionsUI()
                            {
                                EditorGUI.BeginDisabledGroup(withFixedDimensions);
                                EditorGUILayout.BeginHorizontal();
                                EditorGUI.BeginChangeCheck();
                                uint newMaskWidth = (uint)Values.Clamp(1, EditorGUILayout.LongField(new GUIContent("Width:"), maskWidth), 32767);
                                uint newMaskHeight = (uint)Values.Clamp(1, EditorGUILayout.LongField(new GUIContent("Height:"), maskHeight), 32767);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (maskHeight == 0 || maskWidth == 0)
                                    {
                                        contents = null;
                                    }
                                    else
                                    {
                                        contents = SolidObjectMask.Resized(contents, maskWidth, maskHeight, newMaskWidth, newMaskHeight, fillWith);
                                    }
                                    maskWidth = newMaskWidth;
                                    maskHeight = newMaskHeight;
                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUI.EndDisabledGroup();
                            }

                            private void PaletteUI()
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Paint (and fill) cells with mode:");
                                if (GUILayout.Button(new GUIContent("Solid", solidCellImage), withToggle(EditorStyles.miniButtonLeft, fillWith == SolidnessStatus.Solid), GUILayout.Height(16)))
                                {
                                    fillWith = SolidnessStatus.Solid;
                                }
                                if (GUILayout.Button(new GUIContent("Traversable", ghostCellImage), withToggle(EditorStyles.miniButtonMid, fillWith == SolidnessStatus.Ghost), GUILayout.Height(16)))
                                {
                                    fillWith = SolidnessStatus.Ghost;
                                }
                                if (GUILayout.Button(new GUIContent("Hole", holeCellImage), withToggle(EditorStyles.miniButtonRight, fillWith == SolidnessStatus.Hole), GUILayout.Height(16)))
                                {
                                    fillWith = SolidnessStatus.Hole;
                                }
                                EditorGUILayout.EndHorizontal();
                            }

                            // Given a cell's coordinate pair, returns its value and ensures it is
                            // solid, ghost or hole (filling cell with the value of fillWith if the
                            // value is not among them).
                            private SolidnessStatus GetCellContent(uint x, uint y)
                            {
                                Debug.LogFormat("Getting ({0}, {1}) ...", x, y);
                                uint index = y * maskWidth + x;
                                SolidnessStatus value = contents[index];
                                if (value != SolidnessStatus.Solid && value != SolidnessStatus.Ghost && value != SolidnessStatus.Hole)
                                {
                                    value = fillWith;
                                    contents[index] = value;
                                }
                                return value;
                            }

                            // Given a cell's coordinate pair, sets its content to a new status.
                            private void SetCellContent(uint x, uint y, SolidnessStatus status)
                            {
                                uint index = y * maskWidth + x;
                                contents[index] = status;
                            }

                            // Gets the appropriate image according to the state.
                            private Texture GetStatusImage(SolidnessStatus status)
                            {
                                switch(status)
                                {
                                    case SolidnessStatus.Solid:
                                        return solidCellImage;
                                    case SolidnessStatus.Ghost:
                                        return ghostCellImage;
                                    case SolidnessStatus.Hole:
                                        return holeCellImage;
                                    default:
                                        return null;
                                }
                            }

                            private void GridUI(Vector2 basePosition, uint maxX, uint maxY)
                            {
                                Vector2 size = Vector2.one * 32;
                                GUIStyle label = new GUIStyle(GUI.skin.label) { padding = new RectOffset(0, 0, 0, 0), margin = new RectOffset(0, 0, 0, 0) };
                                for(uint y = 0; y < 8; y++)
                                {
                                    uint mappedY = offsetY + 7 - y;
                                    if (mappedY >= maskHeight)
                                    {
                                        for (uint x = 0; x < 8; x++)
                                        {
                                            GUI.Label(new Rect(basePosition + new Vector2(x, y) * 32, size), invalidCellImage, label);
                                        }
                                    }
                                    else
                                    {
                                        for (uint x = 0; x < 8; x++)
                                        {
                                            uint mappedX = offsetX + x;
                                            if (mappedX >= maskWidth)
                                            {
                                                GUI.Label(new Rect(basePosition + new Vector2(x, y) * 32, size), invalidCellImage, label);
                                            }
                                            else
                                            {
                                                SolidnessStatus status = GetCellContent(mappedX, mappedY);
                                                Texture image = GetStatusImage(status);
                                                if (GUI.Button(new Rect(basePosition + new Vector2(x, y) * 32, size), new GUIContent(image), label))
                                                {
                                                    SetCellContent(mappedX, mappedY, fillWith);
                                                }
                                            }
                                        }
                                    }
                                }
                                // offsetX = (uint)GUILayout.HorizontalScrollbar(offsetX, 3, 0, maxX + 3, new GUIStyle() { fixedHeight = 8, fixedWidth = 256, margin = new RectOffset(8, 0, 0, 0) });
                            }

                            private void MainUI(uint maxX, uint maxY)
                            {
                                Rect mainRect = EditorGUILayout.BeginHorizontal();
                                Vector2 mainPosition = mainRect.position + new Vector2(16, 16);
                                EditorGUI.LabelField(
                                    new Rect(mainPosition + new Vector2(16, 0), new Vector2(256, 16)), string.Format("({0}, {1})", Values.Min(offsetX + 7, maskWidth - 1), Values.Min(offsetY + 7, maskHeight - 1)),
                                    new GUIStyle() { alignment = TextAnchor.MiddleRight }
                                );
                                EditorGUI.LabelField(
                                    new Rect(mainPosition + new Vector2(16, 288), new Vector2(256, 16)), string.Format("({0}, {1})", offsetX, offsetY)
                                );
                                EditorGUI.BeginDisabledGroup(maxX == 0);
                                offsetX = (uint)GUI.HorizontalScrollbar(new Rect(mainPosition + new Vector2(16, 272), new Vector2(256, 16)), offsetX, 1, 0, maxX, GUI.skin.horizontalScrollbar);
                                EditorGUI.EndDisabledGroup();
                                EditorGUI.BeginDisabledGroup(maxY == 0);
                                offsetY = (uint)GUI.VerticalScrollbar(new Rect(mainPosition + new Vector2(0, 16), new Vector2(16, 256)), offsetY, 1, 0, maxY, GUI.skin.verticalScrollbar);
                                EditorGUI.EndDisabledGroup();
                                GridUI(new Vector2(32, 32) + mainRect.position, maxX, maxY);
                            }

                            // Renders a grid to edit the mask with 3 states per cell: solid, ghost, hole.
                            private void OnGUI()
                            {
                                GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();
                                GUIStyle indentedStyle = MenuActionUtils.GetIndentedStyle();

                                EditorGUILayout.BeginVertical();
                                string message = "A solidness mask is being edited.";
                                if (withFixedDimensions)
                                {
                                    message += string.Format("\nNOTES: The mask dimensions are fixed to {0}x{1}.", maskWidth, maskHeight);
                                }
                                else
                                {
                                    message += "\nMask dimensions can be freely changed, although they are constrained between 1 and 32767.";
                                }
                                EditorGUILayout.LabelField(message, longLabelStyle);
                                DimensionsUI();
                                PaletteUI();
                                EditorGUILayout.EndVertical();
                                uint maxX = maskWidth >= 8 ? maskWidth - 8 : 0;
                                uint maxY = maskHeight >= 9 ? maskHeight - 8 : 0;
                                offsetX = Values.Min(maxX, offsetX);
                                offsetY = Values.Max(maxY, offsetY);
                                EditorGUILayout.LabelField("This grid is a display of 8x8 mask cells which may contain any state among: Solid, Traversable or Hole.\n" +
                                                           "Scrollbars will appear accordingly if the width or height is greater than 8.\n" +
                                                           "Cells will be invalidated accordingly when width or height is lower than 8.", longLabelStyle);
                                MainUI(maxX, maxY);
                                if (GUI.Button(new Rect(4, 448, 632, 16), "Update mask"))
                                {
                                    property.SetValue(owner, new SolidObjectMask(maskWidth, maskHeight, contents));
                                    Close();
                                }
                            }

                            // Given a sprite, makes its (cropped) texture.
                            private static Texture makeTexture(Sprite sprite)
                            {
                                var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                                var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                                        (int)sprite.textureRect.y,
                                                                        (int)sprite.textureRect.width,
                                                                        (int)sprite.textureRect.height);
                                croppedTexture.SetPixels(pixels);
                                croppedTexture.Apply();
                                return croppedTexture;
                            }

                            public static void EditMask(UnityEngine.Object owner, SolidObjectMask mask, FieldInfo property)
                            {
                                bool withFixedDimensions = false;
                                uint maskWidth = 1;
                                uint maskHeight = 1;
                                if (owner is MonoBehaviour && Attribute.IsDefined(property, typeof(SolidObjectMask.AutoClampedAttribute)))
                                {
                                    Entities.Objects.MapObject mapObject = ((MonoBehaviour)owner).GetComponent<Entities.Objects.MapObject>();
                                    if (mapObject)
                                    {
                                        withFixedDimensions = true;
                                        maskWidth = mapObject.Width;
                                        maskHeight = mapObject.Height;
                                    }
                                }
                                SolidObjectMaskEditorWindow window = ScriptableObject.CreateInstance<SolidObjectMaskEditorWindow>();
                                if (mask.Width != maskWidth || mask.Height != maskHeight)
                                {
                                    mask = mask.Resized(maskWidth, maskHeight, SolidnessStatus.Ghost);
                                }
                                window.contents = mask.Dump();
                                window.withFixedDimensions = withFixedDimensions;
                                window.maskHeight = maskHeight;
                                window.maskWidth = maskWidth;
                                window.owner = owner;
                                window.property = property;
                                window.minSize = new Vector2(640, 468);
                                window.maxSize = window.minSize;
                                foreach(Sprite sprite in AssetDatabase.LoadAllAssetRepresentationsAtPath("Assets/Graphics/EditorUI/solidness-cells.png").OfType<Sprite>())
                                {
                                    switch(sprite.name)
                                    {
                                        case "sc_solid":
                                            window.solidCellImage = makeTexture(sprite);
                                            break;
                                        case "sc_ghost":
                                            window.ghostCellImage = makeTexture(sprite);
                                            break;
                                        case "sc_hole":
                                            window.holeCellImage = makeTexture(sprite);
                                            break;
                                        case "sc_invalid":
                                            window.invalidCellImage = makeTexture(sprite);
                                            break;
                                    }
                                }
                                Debug.LogFormat("Images: solid={0} ghost={1} hole={2} invalid={3}", window.solidCellImage, window.ghostCellImage, window.holeCellImage, window.invalidCellImage);
                                window.titleContent = new GUIContent("Wind Rose - Editing a solid object mask");
                                window.ShowUtility();
                            }
                        }

                        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                        {
                            EditorGUI.BeginProperty(position, label, property);
                            var indent = EditorGUI.indentLevel;
                            EditorGUI.indentLevel = 0;
                            UnityEngine.Object owner = property.serializedObject.targetObject;
                            SolidObjectMask mask = (SolidObjectMask)fieldInfo.GetValue(owner);
                            if (GUI.Button(position, new GUIContent("Edit mask")))
                            {
                                SolidObjectMaskEditorWindow.EditMask(owner, mask, fieldInfo);
                            }
                            EditorGUI.indentLevel = indent;
                            EditorGUI.EndProperty();
                        }
                    }
                }
            }
        }
    }
}
