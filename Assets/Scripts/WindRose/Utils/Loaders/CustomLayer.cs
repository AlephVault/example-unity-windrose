using System;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose.Utils.Loaders
{
    using Types;

    public class CustomLayer : TilemapLayer
    {
        /* Palette entry involves a target color, a source texture, and a rect */
        public class PaletteSource
        {
            public readonly Texture2D Source;
            public readonly Rect SourceRect;
            public readonly bool? Blocking;

            public PaletteSource(Texture2D source, Rect sourceRect, bool? blocking)
            {
                Source = source;
                SourceRect = sourceRect;
                Blocking = blocking;
            }
        }

        /* Source texture for the custom layer. Each pixel will hold a paletted color */
        public readonly Texture2D Source;

        /* Palette entries. A color will match as key to a palette entry. */
        public readonly Dictionary<Color32, PaletteSource> Palette;

        /*
         * Tip: When importing the texture, ensure the type is DEFAULT and the filter is Point(No Filter)
         */
        public CustomLayer(uint width, uint height, Texture2D source, Dictionary<Color32, PaletteSource> palette) : base(width, height)
        {
            Source = source;
            Palette = palette;
            if (palette.ContainsKey(Color.black))
            {
                throw new ArgumentException("Cannot add an entry for black color", "palette");
            }
            if (source.width != width || source.height != height)
            {
                throw new ArgumentException("Source texture must have same dimensions as map", "source");
            }
        }

        public override void Process(Action<uint, uint, Texture2D, Rect> painter, Bitmask currentBlockMask)
        {
            Color32[] pixels = Source.GetPixels32();
            Textures.Flip(pixels, Source.width, Source.height);
            int index = 0;
            for (uint y = 0; y < Height; y++)
            {
                for(uint x = 0; x < Width; x++)
                {
                    Debug.Log("Color=" + pixels[index]);
                    PaletteSource entry;
                    if (Palette.TryGetValue(pixels[index], out entry))
                    {
                        Debug.Log("Painting ...");
                        painter(x, y, entry.Source, entry.SourceRect);
                        if (entry.Blocking != null)
                        {
                            currentBlockMask[x, y] = entry.Blocking.Value;
                        }
                    };
                    index++;
                }
            }
        }
    }
}
