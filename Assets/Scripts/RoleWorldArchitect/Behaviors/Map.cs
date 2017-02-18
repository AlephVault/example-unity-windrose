using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoleWorldArchitect
{
    namespace Behaviors
    {
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

            private const uint TILE_WIDTH = 32;
            private const uint TILE_HEIGHT = TILE_WIDTH;

            public uint Height { get { return height; } }
            public uint Width { get { return width; } }
            public uint TileWidth { get { return TILE_WIDTH; } }
            public uint TileHeight { get { return TILE_HEIGHT; } }

            // Use this for initialization
            void Awake()
            {
                width = Utils.Values.Clamp<uint>(1, width, 100);
                height = Utils.Values.Clamp<uint>(1, height, 100);
            }
        }
    }
}