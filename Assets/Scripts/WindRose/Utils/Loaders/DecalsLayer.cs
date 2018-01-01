using System;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose.Utils.Loaders
{
    using Types;

    public class DecalsLayer : TilemapLayer
    {
        public class Decal
        {

        }

        public readonly List<Decal> Decals;

        public DecalsLayer(uint width, uint height, List<Decal> decals) : base(width, height)
        {
            Decals = decals;
        }

        public override void Process(Action<uint, uint, Texture2D, Rect> painter, Bitmask currentBlockMask)
        {
            // Implementar. Este en particular va a estar jodido para el contrato que le hice al método.
        }
    }
}
