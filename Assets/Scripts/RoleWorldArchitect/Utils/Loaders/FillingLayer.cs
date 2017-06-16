using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoleWorldArchitect.Utils.Loaders
{
    class FillingLayer : TilemapLayer
    {
        /* Source texture for the filling layer */
        public readonly Texture2D Source;

        /* Rect representing the region (in UV coordinates! and being (0, 0) bottom-left corner) 
           of the texture being actually used. Defaults to Rect(0, 0, 1, 1) meaning the whole texture */
        public readonly Rect SourceRect;

        /* Whether we should block or release each position by default */
        public readonly bool Blocking;

        public FillingLayer(uint width, uint height, Texture2D source, bool blocking) : this(width, height, source, blocking, new Rect(0, 0, 1, 1)) {}
        public FillingLayer(uint width, uint height, Texture2D source, bool blocking, Rect sourceRect) : base(width, height)
        {
            Source = source;
            SourceRect = sourceRect;
            Blocking = blocking;
        }

        /**
         * The filling process involves iterating over the whole map, clearing the block mask, and painting the specified texture
         *   (using the specified rect!).
         */
        public override void Process(Action<uint, uint, Texture2D, Rect> painter, Action<uint, uint> blockMaskSetter,
                                     Action<uint, uint> blockMaskClearer, Action<uint, uint> blockMaskInverter)
        {
            Action<uint, uint> blockMaskModifier = Blocking ? blockMaskSetter : blockMaskClearer;
            for (uint y = 0; y < Height; y++)
            {
                for(uint x = 0; x < Width; x++)
                {
                    painter(x, y, Source, SourceRect);
                    blockMaskModifier(x, y);
                }
            }
        }
    }
}
