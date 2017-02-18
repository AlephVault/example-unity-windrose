using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoleWorldArchitect
{
    namespace Behaviors
    {
        public class BlockLayer : MonoBehaviour
        {
            /**
             * This class will keep track of positions no object or character can step into.
             */

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

            private Map map;
            private uint mapWidth;
            private uint mapHeight;
            private Types.BlockMask blockMask;

            void Awake()
            {
                map = Utils.Layout.RequireComponentInParent<Map>(this);
                mapWidth = map.Width;
                mapHeight = map.Height;

                if (mask == null)
                {
                    mask = "";
                }

                blockMask = new Types.BlockMask(mapWidth, mapHeight, Types.BlockMask.PadMask(
                    mask, mapWidth, mapHeight, freeMarkingChar, blockMarkingChar, maskApplicationOffsetX, maskApplicationOffsetY
                ));
            }

            bool IsBlocked(uint x, uint y)
            {
                return blockMask.GetBit(x, y);
            }

            bool IsRowBlocked(uint xi, uint xf, uint y)
            {
                return blockMask.GetRow(xi, xf, y, Types.BlockMask.CheckType.ANY_BLOCKED);
            }

            bool IsColumnBlocked(uint x, uint yi, uint yf)
            {
                return blockMask.GetColumn(x, yi, yf, Types.BlockMask.CheckType.ANY_BLOCKED);
            }

            bool IsSquareBlocked(uint xi, uint yi, uint xf, uint yf)
            {
                return blockMask.GetSquare(xi, yi, xf, yf, Types.BlockMask.CheckType.ANY_BLOCKED);
            }
        }
    }
}