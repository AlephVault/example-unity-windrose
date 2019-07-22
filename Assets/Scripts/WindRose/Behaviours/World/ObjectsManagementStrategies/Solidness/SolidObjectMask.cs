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
                        private SolidnessStatus fillWith = SolidnessStatus.Ghost;
                        private SerializedProperty widthProperty;
                        private SerializedProperty heightProperty;
                        private SerializedProperty cellsProperty;
                        private Texture2D invalidSquare;
                        private Texture2D ghostSquare;
                        private Texture2D holeSquare;
                        private Texture2D solidSquare;
                        private uint scrollX = 0;
                        private uint scrollY = 0;
                        private Entities.Objects.MapObject clampAgainst = null;
                        private bool initialized = false;
                        private SerializedProperty[,] cellElementProperties;

                        private Texture2D MakeSolidIcon(Color color, int height = 0)
                        {
                            if (height <= 0) height = (int)EditorGUIUtility.singleLineHeight - 2;
                            int size = height * height;
                            Color[] content = new Color[size];
                            for (int index = 0; index < size; index++) content[index] = color;
                            Texture2D texture = new Texture2D(height, height, TextureFormat.ARGB32, false);
                            texture.SetPixels(content);
                            texture.Apply();
                            return texture;
                        }

                        private void Initialize(SerializedProperty property)
                        {
                            if (!initialized)
                            {
                                widthProperty = property.FindPropertyRelative("width");
                                heightProperty = property.FindPropertyRelative("height");
                                cellsProperty = property.FindPropertyRelative("cells");
                                invalidSquare = MakeSolidIcon(Color.black);
                                ghostSquare = MakeSolidIcon(new Color(0, 0.5f, 0, 1));
                                holeSquare = MakeSolidIcon(new Color(0.5f, 0, 0, 1));
                                solidSquare = MakeSolidIcon(Color.grey);
                                bool withClampingAttribute = Attribute.IsDefined(fieldInfo, typeof(SolidObjectMask.AutoClampedAttribute));
                                bool ownerIsBehaviour = property.serializedObject.targetObject is MonoBehaviour;
                                if (withClampingAttribute && ownerIsBehaviour)
                                {
                                    clampAgainst = (property.serializedObject.targetObject as MonoBehaviour).GetComponent<Entities.Objects.MapObject>();
                                }
                                int width = widthProperty.intValue;
                                int height = heightProperty.intValue;
                                if (cellsProperty.arraySize != width * height)
                                {
                                    cellsProperty.arraySize = width * height;
                                }
                                cellElementProperties = new SerializedProperty[widthProperty.intValue, heightProperty.intValue];
                                int index = 0;
                                for (uint y = 0; y < height; y++)
                                {
                                    for (uint x = 0; x < width; x++)
                                    {
                                        cellElementProperties[x, y] = cellsProperty.GetArrayElementAtIndex((int)index++);
                                    }
                                }
                                initialized = true;
                            }
                        }

                        private void Resize(uint oldWidth, uint oldHeight, uint newWidth, uint newHeight)
                        {
                            SolidnessStatus[] statuses = (SolidnessStatus[])Enum.GetValues(typeof(SolidnessStatus));
                            SolidnessStatus[] oldStatuses = new SolidnessStatus[oldWidth * oldHeight];
                            uint index = 0;
                            for (uint y = 0; y < oldHeight; y++)
                            {
                                for (uint x = 0; x < oldWidth; x++)
                                {
                                    oldStatuses[index] = statuses[cellElementProperties[x, y].enumValueIndex];
                                }
                            }
                            SolidnessStatus[] newStatuses = SolidObjectMask.Resized(oldStatuses, oldWidth, oldHeight, newWidth, newHeight, fillWith);
                            if (newStatuses == null)
                            {
                                cellElementProperties = new SerializedProperty[0, 0];
                                cellsProperty.arraySize = 0;
                            }
                            else
                            {
                                cellsProperty.arraySize = (int)(newWidth * newHeight);
                                index = 0;
                                for (uint y = 0; y < newHeight; y++)
                                {
                                    for (uint x = 0; x < newWidth; x++)
                                    {
                                        cellElementProperties[x, y] = cellsProperty.GetArrayElementAtIndex((int)index++);
                                        cellElementProperties[x, y].enumValueIndex = Array.IndexOf(statuses, fillWith);
                                    }
                                }
                            }
                        }

                        // Gets the appropriate image according to the state.
                        private Texture GetStatusImage(SolidnessStatus status)
                        {
                            switch (status)
                            {
                                case SolidnessStatus.Solid:
                                    return solidSquare;
                                case SolidnessStatus.Ghost:
                                    return ghostSquare;
                                case SolidnessStatus.Hole:
                                    return holeSquare;
                                default:
                                    return null;
                            }
                        }

                        private void RenderGrid(Vector2 basePosition, uint width, uint height, float squareSize)
                        {
                            // Names are overriden here to use appropriately sized squares.
                            Texture2D invalidSquare = MakeSolidIcon(Color.black, (int)squareSize);
                            Texture2D ghostSquare = MakeSolidIcon(new Color(0, 0.5f, 0, 1), (int)squareSize);
                            Texture2D holeSquare = MakeSolidIcon(new Color(0.5f, 0, 0, 1), (int)squareSize);
                            Texture2D solidSquare = MakeSolidIcon(Color.grey, (int)squareSize);

                            Vector2 size = Vector2.one * (squareSize - 1);
                            GUIStyle label = new GUIStyle(GUI.skin.label) { padding = new RectOffset(0, 0, 0, 0), margin = new RectOffset(0, 0, 0, 0) };
                            SolidnessStatus[] statuses = (SolidnessStatus[])Enum.GetValues(typeof(SolidnessStatus));

                            for (uint y = 0; y < 8; y++)
                            {
                                uint mappedY = scrollY + 7 - y;
                                if (mappedY >= height)
                                {
                                    for (uint x = 0; x < 8; x++)
                                    {
                                        GUI.Label(new Rect(basePosition + new Vector2(x, y) * squareSize, size), invalidSquare, label);
                                    }
                                }
                                else
                                {
                                    for (uint x = 0; x < 8; x++)
                                    {
                                        Vector2 offset = new Vector2(x, y) * squareSize;
                                        offset.x = (int)offset.x;
                                        offset.y = (int)offset.y;
                                        uint mappedX = scrollX + x;
                                        if (mappedX >= width)
                                        {
                                            GUI.Label(new Rect(basePosition + offset, size), invalidSquare, label);
                                        }
                                        else
                                        {
                                            SolidnessStatus status = statuses[cellElementProperties[mappedX, mappedY].enumValueIndex];
                                            if (status == SolidnessStatus.Mask)
                                            {
                                                status = fillWith;
                                            }
                                            Texture2D image = null;
                                            switch (status)
                                            {
                                                case SolidnessStatus.Solid:
                                                    image = solidSquare;
                                                    break;
                                                case SolidnessStatus.Ghost:
                                                    image = ghostSquare;
                                                    break;
                                                case SolidnessStatus.Hole:
                                                    image = holeSquare;
                                                    break;
                                            }
                                            if (GUI.Button(new Rect(basePosition + offset, size), new GUIContent(image), label))
                                            {
                                                cellElementProperties[mappedX, mappedY].enumValueIndex = Array.IndexOf(Enum.GetValues(typeof(SolidnessStatus)), fillWith);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        private float GetCurrentWidth()
                        {
                            // The value "15" has to be tested against 2017.3.
                            return EditorGUIUtility.currentViewWidth - 15 * EditorGUI.indentLevel;
                        }

                        private Vector2 Height2Vector(float height)
                        {
                            return new Vector2(0, height);
                        }

                        private void FillButton(Rect position, Texture2D image, string text, SolidnessStatus status, GUIStyle baseStyle)
                        {
                            GUIStyle style = baseStyle;
                            if (fillWith == status)
                            {
                                style = new GUIStyle(style);
                                style.normal.background = style.active.background;
                            }
                            GUIContent content = new GUIContent(text, image);
                            if (GUI.Button(position, content, style))
                            {
                                fillWith = status;
                            }
                        }

                        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                        {
                            Initialize(property);
                            float availableWidth = position.width;
                            float slHeight = EditorGUIUtility.singleLineHeight;
                            float propHeight = 0;
                            Vector2 xyPos = position.position;
                            Vector2 xySpacing = Height2Vector(EditorGUIUtility.standardVerticalSpacing);
                            Vector2 xySLHeight = Height2Vector(slHeight);
                            EditorGUI.BeginProperty(position, label, property);
                            EditorGUI.BeginDisabledGroup(clampAgainst != null);
                            // Keep current dimensions
                            uint oldWidth = (uint)widthProperty.intValue;
                            uint oldHeight = (uint)heightProperty.intValue;
                            // Width
                            propHeight = EditorGUI.GetPropertyHeight(widthProperty);
                            EditorGUI.PropertyField(new Rect(xyPos, new Vector2(position.width, propHeight)), widthProperty, true);
                            xyPos += xySpacing + Height2Vector(propHeight);
                            // Height
                            propHeight = EditorGUI.GetPropertyHeight(heightProperty);
                            EditorGUI.PropertyField(new Rect(xyPos, new Vector2(position.width, propHeight)), heightProperty, true);
                            xyPos += xySpacing + Height2Vector(propHeight);
                            EditorGUI.EndDisabledGroup();
                            // Clamp current dimensions
                            widthProperty.intValue = Values.Clamp(1, widthProperty.intValue, 32767);
                            heightProperty.intValue = Values.Clamp(1, heightProperty.intValue, 32767);
                            if (clampAgainst)
                            {
                                widthProperty.intValue = (int)clampAgainst.Width;
                                heightProperty.intValue = (int)clampAgainst.Height;
                            }
                            // Compare dimensions and perhaps resize
                            uint newWidth = (uint)widthProperty.intValue;
                            uint newHeight = (uint)heightProperty.intValue;
                            if (oldWidth != newWidth || oldHeight != newHeight)
                            {
                                Resize(oldWidth, oldHeight, newWidth, newHeight);
                            }
                            else if (cellsProperty.arraySize != (oldWidth * oldHeight))
                            {
                                // This will occur typically on first GUI iteration only.
                                cellsProperty.arraySize = (int)(newWidth * newHeight);
                                int index = 0;
                                for(uint x = 0; x < newWidth; x++)
                                {
                                    for(uint y = 0; y < newHeight; y++)
                                    {
                                        Debug.Log("cellsProperty=" + cellsProperty);
                                        cellElementProperties[x, y] = cellsProperty.GetArrayElementAtIndex(index);
                                        index++;
                                    }
                                }
                            }
                            // Clamp scrolling coordinates to {1, .., new width - 8}
                            //                            and {1, .., new height - 8}
                            uint maxX = newWidth - 8;
                            uint maxY = newHeight - 8;
                            scrollX = Values.Clamp(0, scrollX, maxX);
                            scrollY = Values.Clamp(0, scrollY, maxY);
                            // Grid (and scrollbars)
                            float squareSize = (availableWidth - slHeight) / 8;
                            EditorGUI.BeginDisabledGroup(maxX == 0);
                            scrollX = (uint)GUI.HorizontalScrollbar(new Rect(xyPos + new Vector2(slHeight, availableWidth - slHeight), new Vector2(availableWidth - slHeight, slHeight)), scrollX, 1, 0, maxX + 1, GUI.skin.horizontalScrollbar);
                            EditorGUI.EndDisabledGroup();
                            EditorGUI.BeginDisabledGroup(maxY == 0);
                            scrollY = (uint)GUI.VerticalScrollbar(new Rect(xyPos, new Vector2(slHeight, availableWidth - slHeight)), scrollY, 1, maxY + 1, 0, GUI.skin.verticalScrollbar);
                            EditorGUI.EndDisabledGroup();
                            RenderGrid(xyPos + new Vector2(slHeight, 0), newWidth, newHeight, squareSize);
                            xyPos += xySpacing + Height2Vector(position.width);
                            // Position (x, y) -> (xf, yf)
                            EditorGUI.LabelField(new Rect(xyPos, new Vector2(position.width, slHeight)), string.Format(
                                "Left-Down: ({0}, {1}) - Right-Up: ({2}, {3})", scrollX, scrollY, Values.Min(scrollX + 7, maxX), Values.Min(scrollY + 7, maxY)
                            ));
                            xyPos += xySpacing + xySLHeight;
                            // Buttons
                            float width3 = position.width / 3;
                            FillButton(new Rect(xyPos.x, xyPos.y, width3, slHeight), solidSquare, "Solid", SolidnessStatus.Solid, EditorStyles.miniButtonLeft);
                            FillButton(new Rect(xyPos.x + width3, xyPos.y, width3, slHeight), ghostSquare, "Ghost", SolidnessStatus.Ghost, EditorStyles.miniButtonMid);
                            FillButton(new Rect(xyPos.x + 2 * width3, xyPos.y, width3, slHeight), holeSquare, "Hole", SolidnessStatus.Hole, EditorStyles.miniButtonRight);
                            xyPos += xySpacing + xySLHeight;
                            EditorGUI.EndProperty();
                        }

                        /// <summary>
                        ///   Allows caching the same drawer for the same mask property instance.
                        /// </summary>
                        /// <param name="property">The property to cache for</param>
                        /// <returns>true</returns>
                        public override bool CanCacheInspectorGUI(SerializedProperty property)
                        {
                            return true;
                        }

                        /// <summary>
                        ///   Property height for 5 fields: 4 having standard size, and 1 having the height
                        ///   being the same as the GUI width.
                        /// </summary>
                        /// <param name="property">The property being calculated for</param>
                        /// <param name="label">The property label</param>
                        /// <returns>The height involving all the 5 fields</returns>
                        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                        {
                            Initialize(property);
                            return 2 * EditorGUIUtility.standardVerticalSpacing + GetCurrentWidth() +
                                   EditorGUI.GetPropertyHeight(widthProperty) + EditorGUI.GetPropertyHeight(heightProperty);
                            // Possible bug: Why I don't need to add the two instances of standard single-line size
                            //               and their corresponding standard vertical spacing?
                        }
                    }
                }
            }
        }
    }
}
