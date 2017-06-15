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

        public FillingLayer(uint width, uint height, Texture2D source) : this(width, height, source, new Rect(0, 0, 1, 1)) {}
        public FillingLayer(uint width, uint height, Texture2D source, Rect sourceRect) : base(width, height)
        {
            Source = source;
            SourceRect = sourceRect;
        }

        /**
         * The filling process involves iterating over the whole map, clearing the block mask, and painting the specified texture
         *   (using the specified rect!).
         */
        public override void Process(Action<uint, uint, Texture2D, Rect> painter, Action<uint, uint> blockMaskSetter,
                                     Action<uint, uint> blockMaskClearer, Action<uint, uint> blockMaskInverter)
        {
            for(uint y = 0; y < Height; y++)
            {
                for(uint x = 0; x < Width; x++)
                {
                    painter(x, y, Source, SourceRect);
                    blockMaskClearer(x, y);
                }
            }
        }
    }
}
