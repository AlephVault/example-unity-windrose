using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoleWorldArchitect
{
    namespace Behaviors
    {
        public class ObjectLayer : MonoBehaviour
        {
            public class ObjectNotAddedException : Types.Exception
            {
                public readonly Positionable target;

                public ObjectNotAddedException(Positionable positionable) { target = positionable; }
                public ObjectNotAddedException(string message, Positionable positionable) : base(message) { target = positionable; }
                public ObjectNotAddedException(string message, Positionable positionable, System.Exception inner) : base(message, inner) { target = positionable; }
            }

            public class ObjectAlreadyAddedException : Types.Exception
            {
                public readonly Positionable target;

                public ObjectAlreadyAddedException(Positionable positionable) { target = positionable; }
                public ObjectAlreadyAddedException(string message, Positionable positionable) : base(message) { target = positionable; }
                public ObjectAlreadyAddedException(string message, Positionable positionable, System.Exception inner) : base(message, inner) { target = positionable; }
            }

            private class ObjectSpatialData
            {
                public uint x;
                public uint y;
                public uint width;
                public uint height;
                public Positionable.SolidnessStatus solidness;
                // Perhaps it will not be used if the object it not Movable.
                public Types.Direction? currentMovement;

                public ObjectSpatialData(uint x, uint y, uint width, uint height, Positionable.SolidnessStatus solidness)
                {
                    this.x = x;
                    this.y = y;
                    this.width = width;
                    this.height = height;
                    this.solidness = solidness;
                    this.currentMovement = null;
                }
            }

            private Map map;
            private uint mapWidth;
            private uint mapHeight;
            private uint tileWidth;
            private uint tileHeight;

            [SerializeField]
            [TextArea(5, 10)]
            private string mask;

            [SerializeField]
            private char freeMarkingChar = '0';

            [SerializeField]
            private char blockMarkingChar = '1';

            [SerializeField]
            private uint maskApplicationOffsetX = 0;

            [SerializeField]
            private uint maskApplicationOffsetY = 0;

            private Types.BlockMask blockMask;
            private Types.SolidMask solidMask;
            private Dictionary<Positionable, ObjectSpatialData> objects = new Dictionary<Positionable, ObjectSpatialData>();

            public uint MapWidth { get { return mapWidth; } }
            public uint MapHeight { get { return mapHeight; } }
            public uint TileWidth { get { return tileWidth; } }
            public uint TileHeight { get { return tileHeight; } }

            void Awake()
            {
                map = Utils.Layout.RequireComponentInParent<Map>(this);
                mapWidth = map.Width;
                mapHeight = map.Height;
                tileWidth = map.TileWidth;
                tileHeight = map.TileHeight;

                if (mask == null)
                {
                    mask = "";
                }

                blockMask = new Types.BlockMask(mapWidth, mapHeight, Types.BlockMask.PadMask(
                    mask, mapWidth, mapHeight, freeMarkingChar, blockMarkingChar, maskApplicationOffsetX, maskApplicationOffsetY
                ));
                solidMask = new Types.SolidMask(mapWidth, mapHeight);
            }

            /**
             * To be implemented:
             * 
             * Installing a Positionable with values (x,y,w,h,solidness):
             * # Method called from a Positionable behavior when awakened.
             * - It is an error if the Positionable is already in the dictionary.
             * - If it is not solid, checks whether (x,y,w,h) are valid.
             *   Otherwise, try incrementing the positions in (x,y,w,h) (the check is also performed!).
             * - Adds it to the dictionary, with an appropriately created ObjectSpatialData instance.
             * 
             * Uninstalling a Positionable:
             * # Method called from a Positionable behavior when related GameObject instance is destroyed.
             * - It is an error if the Positionable is not in the dictionary.
             * - If it is solid, decrement the positions in (x,y,w,f) (dimensional check is performed).
             * - Remove it from the dictionary.
             * 
             * Start movement of a Positionable in a direction
             * Cancel movement of a Positionable in a direction
             * Complete movement of a Positionable in a direction
             * Change X of a Positionable (also cancels a movement, if one started)
             * Change Y of a Positionable (also cancels a movement, if one started)
             * Change X,Y of a Positionable (also cancels a movement, if one started)
             * Change solidness of a Positionable
             */

            public void Add(Positionable positionable, uint x, uint y, uint width, uint height, Positionable.SolidnessStatus solidness)
            {
                if (Added(positionable))
                {
                    throw new ObjectAlreadyAddedException(positionable);
                }
                if (solidness == Positionable.SolidnessStatus.Ghost)
                {
                    solidMask.CheckDimensions(x, y, width, height);
                }
                else
                {
                    solidMask.IncSquare(x, y, width, height);
                }
                objects.Add(positionable, new ObjectSpatialData(x, y, width, height, solidness));
            }

            public void Remove(Positionable positionable)
            {
                CheckAdded(positionable);
                ObjectSpatialData data = objects[positionable];
                if (data.solidness != Positionable.SolidnessStatus.Ghost)
                {
                    solidMask.DecSquare(data.x, data.y, data.width, data.height);
                }
                objects.Remove(positionable);
            }

            public bool Added(Positionable positionable)
            {
                return objects.ContainsKey(positionable);
            }

            private void CheckAdded(Positionable positionable)
            {
                if (!Added(positionable))
                {
                    throw new ObjectNotAddedException(positionable);
                }
            }

            public void SetX(Positionable positionable, uint x)
            {
                SetXY(positionable, x, objects[positionable].y);
            }

            public void SetY(Positionable positionable, uint y)
            {
                SetXY(positionable, objects[positionable].x, y);
            }

            public void SetXY(Positionable positionable, uint x, uint y)
            {
                CheckAdded(positionable);
                ObjectSpatialData data = objects[positionable];
                if (x != data.x || y != data.y)
                {
                    CancelMovement(positionable);
                    if (data.solidness != Positionable.SolidnessStatus.Ghost)
                    {
                        // Increment the square it will be located at
                        solidMask.IncSquare(x, y, data.width, data.height);
                        // Decrementing the square it is located at
                        solidMask.DecSquare(data.x, data.y, data.width, data.height);
                    }
                    else
                    {
                        solidMask.CheckDimensions(x, y, data.width, data.height);
                    }
                    data.x = x;
                    data.y = y;
                }
            }

            public void SetSolidness(Positionable positionable, Positionable.SolidnessStatus solidness)
            {
                CheckAdded(positionable);
                ObjectSpatialData data = objects[positionable];
                if (data.solidness != Positionable.SolidnessStatus.Ghost && solidness != Positionable.SolidnessStatus.Ghost)
                {

                }
                else
                {

                }
            }

            public void AllocateMovement(Positionable positionable, Types.Direction direction)
            {
                CheckAdded(positionable);
                // TODO allocate the respective movement
            }

            public void CancelMovement(Positionable positionable)
            {
                CheckAdded(positionable);
            }

            public void ConfirmMovement(Positionable positionable)
            {
                CheckAdded(positionable);
            }

            public bool CanMove(Positionable positionable, Types.Direction direction)
            {
                CheckAdded(positionable);
            }

            public uint GetX(Positionable positionable)
            {
                return objects[positionable].x;
            }

            public uint GetY(Positionable positionable)
            {
                return objects[positionable].y;
            }

            public uint GetWidth(Positionable positionable)
            {
                return objects[positionable].width;
            }

            public uint GetHeight(Positionable positionable)
            {
                return objects[positionable].height;
            }

            public Positionable.SolidnessStatus GetSolidness(Positionable positionable)
            {
                return objects[positionable].solidness;
            }

            public Types.Direction? GetCurrentMovement(Positionable positionable)
            {
                return objects[positionable].currentMovement;
            }
        }
    }
}