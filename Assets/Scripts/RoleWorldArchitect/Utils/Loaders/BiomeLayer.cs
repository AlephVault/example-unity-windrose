using System;
using UnityEngine;

namespace RoleWorldArchitect.Utils.Loaders
{
    using Types.Tilemaps.Loaders;

    class BiomeLayer : TilemapLayer
    {
        /* Source texture for the filling layer */
        public readonly Texture2D Source;

        /*
         * This is an array of biome presence. This array is (Width+1)x(Height+1), when the
         *   map array is Width x Height. Actually this array represents the "corners" for
         *   a presence computation for the underlying Width x Height array. In this sense,
         *   computation in the underlying array for position [x, y] will be based on the
         *   corners in this PresenceDataL [x, y], [x, y+1], [x+1, y], [x+1, y+1].
         * 
         * The result of the computation will have two stages (perhaps we could add more in
         *   a future!):
         *   
         *   * The texture section to use, computed as a binary number if converting to
         *     bits the presences ([x, y])([x+1, y])([x, y+1])([x+1, y+1]).
         *   * The presence. If extendedPresence is true, it will be computed as an OR
         *     of the presence on each corner (i.e. the presence will be marked for
         *     every cell except for the empty one). Otherwise, it will be computed as an
         *     AND of the presence of each corner (i.e. the presence will only be marked
         *     for "central" biome cells).
         *   * When a presence was marked, we should tell whether the presence marks a
         *     position as blocked, or marks a position as free, or does nothing.
         *     This is determined by the value of presenceBlockingMode: true means block,
         *     false means clear, and null does nothing (this latter case is ideal for
         *     stuff like grass or dirt).
         */
        public readonly string PresenceData;
        private readonly bool[,] ParsedPresenceData;
        public readonly bool ExtendedPresence;
        public readonly bool? PresenceBlockingMode;
        public readonly char PresenceMarkingChar;
        public readonly char AbsenceMarkingChar;

        /* Alternates picker (used only for the center (15) tile), if available */
        public readonly RandomAlternatePicker OtherTilesPicker;

        /* Mappings of source rects - Biomes should be (2^n)x(2^n) size, n >= 2 */
        private static float S = 0.25f;
        private static float P0 = 0f;
        private static float P1 = S;
        private static float P2 = 2 * S;
        private static float P3 = 3 * S;
        private static readonly Rect[] SourceRects = new Rect[]{
            new Rect(P2, P3, S, S), new Rect(P0, P2, S, S), new Rect(P2, P2, S, S), new Rect(P1, P2, S, S),
            new Rect(P0, P0, S, S), new Rect(P0, P1, S, S), new Rect(P0, P3, S, S), new Rect(P3, P0, S, S),
            new Rect(P2, P0, S, S), new Rect(P1, P3, S, S), new Rect(P2, P1, S, S), new Rect(P3, P2, S, S),
            new Rect(P1, P0, S, S), new Rect(P3, P1, S, S), new Rect(P3, P3, S, S), new Rect(P1, P1, S, S)
        };

        public BiomeLayer(uint width, uint height, Texture2D source, string presenceData, bool extendedPresence, bool? presenceBlockingMode = null,
                          RandomAlternatePicker picker = null, char presenceMarkingChar = '1', char absenceMarkingChar = '0') : base(width, height)
        {
            Source = source;
            PresenceData = presenceData;
            ExtendedPresence = extendedPresence;
            PresenceBlockingMode = presenceBlockingMode;
            PresenceMarkingChar = presenceMarkingChar;
            AbsenceMarkingChar = absenceMarkingChar;
            ParsedPresenceData = ParsePresenceData(presenceData);
            OtherTilesPicker = picker;
        }

        private bool[,] ParsePresenceData(string presenceData)
        {
            if (presenceData == null)
            {
                throw new ArgumentNullException("presenceData");
            }
            string[] rows = presenceData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            if ((uint) rows.Length != Height + 1)
            {
                throw new ArgumentException("The presence data string must have " + (Height + 1) + " rows (the actual height, + 1)", "presenceData");
            }
            bool[,] result = new bool[Width + 1, Height + 1];
            uint x, y = 0;
            foreach(string row in rows)
            {
                if ((uint) row.Length != Width + 1)
                {
                    throw new ArgumentException("Each row in presence data string must have " + (Width + 1) + " characters (the actual width, + 1)", "presenceData");
                }
                x = 0;
                foreach(char cell in row)
                {
                    if (cell == PresenceMarkingChar)
                    {
                        result[x, y] = true;
                    }
                    else if (cell == AbsenceMarkingChar)
                    {
                        result[x, y] = false;
                    }
                    else
                    {
                        throw new ArgumentException("Each character in the presence data string must be either " + PresenceMarkingChar + " or " + AbsenceMarkingChar);
                    }
                    x++;
                }
                y++;
            }
            return new bool[0, 0];
        }

        /**
         * The biome-making process involves iterating over the whole map, computing the tile index based on its tile mask,
         *   computing the block mask, and generating the texture (perhaps using random tiles for central tile).
         */
        public override void Process(Action<uint, uint, Texture2D, Rect> painter, Action<uint, uint> blockMaskSetter,
                             Action<uint, uint> blockMaskClearer, Action<uint, uint> blockMaskInverter)
        {
            Action<uint, uint> blockMaskModifier;
            if (PresenceBlockingMode == true)
            {
                blockMaskModifier = blockMaskSetter;
            }
            else if (PresenceBlockingMode == false)
            {
                blockMaskModifier = blockMaskClearer;
            }
            else
            {
                blockMaskModifier = (uint x, uint y) => {};
            }
            for (uint y = 0; y < Height; y++)
            {
                for (uint x = 0; x < Width; x++)
                {
                    // Right now this does not mess with biomatic markers.
                    // Just the block mask and the texture.
                    int presenceIndex = 0;
                    if (ParsedPresenceData[x, y]) { presenceIndex += 8; }
                    if (ParsedPresenceData[x+1, y]) { presenceIndex += 4; }
                    if (ParsedPresenceData[x, y+1]) { presenceIndex += 2; }
                    if (ParsedPresenceData[x+1, y+1]) { presenceIndex += 1; }
                    Rect? picked = (presenceIndex == 15 && OtherTilesPicker != null) ? OtherTilesPicker.Pick() : null;
                    Rect section = picked != null ? picked.Value : SourceRects[presenceIndex];
                    painter(x, y, picked != null ? OtherTilesPicker.Source : Source, section);
                    if (ExtendedPresence ? (presenceIndex != 0) : (presenceIndex == 15))
                    {
                        blockMaskModifier(x, y);
                    }
                }
            }
        }
    }
}
