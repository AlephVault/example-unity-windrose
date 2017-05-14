﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoleWorldArchitect
{
    namespace Behaviors
    {
        using Types.Tilemaps;

        public class Map : MonoBehaviour
        {
            /**
             * A map has nothing on its own but serves as a base marker
             *   for any object willing to act as a map. Children objects
             *   will be map layers, which will have access to this map's
             *   dimensions.
             * 
             * The map also provides dimensions for their cells/tiles, to
             *   be used by children objects.
             */

            [SerializeField]
            private uint width;

            [SerializeField]
            private uint height;

            [SerializeField]
            [TextArea(3, 10)]
            private string blockMask;

            [SerializeField]
            private char freeMarkingChar = '0';

            [SerializeField]
            private char blockMarkingChar = '1';

            [SerializeField]
            private uint maskApplicationOffsetX = 0;

            [SerializeField]
            private uint maskApplicationOffsetY = 0;

            private Tilemap internalTilemap;

            public Tilemap InternalTilemap { get { return internalTilemap; } }
            public uint Height { get { return height; } }
            public uint Width { get { return width; } }

            // Use this for initialization
            void Awake()
            {
                width = Utils.Values.Clamp<uint>(1, width, 100);
                height = Utils.Values.Clamp<uint>(1, height, 100);
                internalTilemap = new Tilemap(Width, Height, blockMask, freeMarkingChar, blockMarkingChar, maskApplicationOffsetX, maskApplicationOffsetY);
            }
        }
    }
}