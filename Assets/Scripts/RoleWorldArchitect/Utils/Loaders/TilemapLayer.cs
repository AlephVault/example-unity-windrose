using System;
using UnityEngine;

namespace RoleWorldArchitect.Utils.Loaders
{
    public abstract class TilemapLayer
    {
        public abstract void Process(RenderTexture target, Action<uint, uint> blockMaskSetter, Action<uint, uint> blockMaskClearer);
    }
}
