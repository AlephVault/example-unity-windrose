using System;
using UnityEngine;

namespace RoleWorldArchitect.Utils.Loaders
{
    using Types.Tilemaps.Loaders;

    class FillingLayer : TilemapLayer
    {
        /* Source texture for the filling layer */
        public readonly Texture2D Source;

        /* Rect representing the region (in UV coordinates! and being (0, 0) bottom-left corner) 
           of the texture being actually used. Defaults to Rect(0, 0, 1, 1) meaning the whole texture */
        public readonly Rect SourceRect;

        /* Whether we should block or release each position by default */
        public readonly bool Blocking;

        /* Alternates picker, if available */
        public readonly RandomAlternatePicker OtherTilesPicker;

        public FillingLayer(uint width, uint height, Texture2D source, bool blocking, RandomAlternatePicker picker = null) : this(width, height, source, blocking, new Rect(0, 0, 1, 1), picker) {}
        public FillingLayer(uint width, uint height, Texture2D source, bool blocking, Rect sourceRect, RandomAlternatePicker picker = null) : base(width, height)
        {
            Source = source;
            SourceRect = sourceRect;
            Blocking = blocking;
            OtherTilesPicker = picker;
        }

        /**
         * The filling process involves iterating over the whole map, clearing or setting the block mask, and painting the specified texture
         *   (using the specified rect!), or an alternative if the odds were in favor.
         */
        public override void Process(Action<uint, uint, Texture2D, Rect> painter, Action<uint, uint> blockMaskSetter,
                                     Action<uint, uint> blockMaskClearer, Action<uint, uint> blockMaskInverter)
        {
            Action<uint, uint> blockMaskModifier = Blocking ? blockMaskSetter : blockMaskClearer;
            for (uint y = 0; y < Height; y++)
            {
                for(uint x = 0; x < Width; x++)
                {
                    Rect? picked = (OtherTilesPicker != null) ? OtherTilesPicker.Pick() : null;
                    painter(x, y, picked != null ? OtherTilesPicker.Source : Source, picked != null ? picked.Value : SourceRect);
                    blockMaskModifier(x, y);
                }
            }
        }
    }
}
