using System;
using UnityEngine;

namespace RoleWorldArchitect.Utils.Loaders
{
    using Types;
    public abstract class TilemapLayer
    {
        /* Width of the current layer, expressed in cells. Must match the width of a map loader */
        public readonly uint Width;

        /* Height of the current layer, expressed in cells. Must match the width of a map loader */
        public readonly uint Height;

        public TilemapLayer(uint width, uint height)
        {
            Width = width;
            Height = height;
        }

        /* Children classes will have to override this method to paint on the texture and affect the block mask */
        public abstract void Process(Action<uint, uint, Texture2D, Rect> painter, Bitmask currentBlockMask);
    }
}
