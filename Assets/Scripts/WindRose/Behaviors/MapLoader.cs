using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        using Utils.Loaders;
        using Types.Tilemaps.Loaders;

        public class MapLoader : MonoBehaviour
        {
            [System.Serializable]
            public class TilemapLayerSpec
            {
                /**
                 * We are specifying the type of layer: Filling, Biome.
                 * We will add more layer types later.
                 */

                [System.Serializable]
                public class RandomOptionSpec
                {
                    public Rect Region;
                    public uint Odds;
                }

                public enum BlockingMode { Blocking, Unblocking, DoesNotAffect }

                [System.Serializable]
                public class PaletteSource
                {
                    public Color32 Color;
                    public Texture2D Source;
                    public Rect SourceRect = new Rect(0, 0, 1, 1);
                    public BlockingMode Blocking;
                }

                [SerializeField]
                private bool EditorUnfolded = true;

                public enum LayerSpecType { Filling, Biome, Custom }
                public LayerSpecType LayerType = LayerSpecType.Filling;

                // These ones are for Filling Layer
                public Texture2D FillingSource;
                public Rect FillingSourceRect = new Rect(0, 0, 1, 1);
                public bool FillingBlocks = false;

                // These ones are for Biome Layer
                public Texture2D BiomeSource;
                public Texture2D BiomePresenceData;
                public bool BiomeExtendedPresence = false;
                public BlockingMode BiomeBlockingMode = BlockingMode.DoesNotAffect;
                public int BiomeOffsetX = 0;
                public int BiomeOffsetY = 0;

                // These ones are for Custom Layer
                public Texture2D CustomSource;
                public PaletteSource[] CustomPalette;

                // These ones are for both Filling Layer and Biome Layer.
                public Texture2D RandomSource;
                public RandomOptionSpec[] RandomOptions;
            }

            [SerializeField]
            private uint Width = 16;

            [SerializeField]
            private uint Height = 12;

            [SerializeField]
            private TilemapLayerSpec[] Layers = new TilemapLayerSpec[0];

            [SerializeField]
            private uint TileSize = 32;

            private RandomAlternatePicker CreatePicker(TilemapLayerSpec spec)
            {
                RandomAlternatePicker picker = null;
                if (spec.RandomSource)
                {
                    picker = new RandomAlternatePicker(
                        spec.RandomSource,
                        spec.RandomOptions.Select<TilemapLayerSpec.RandomOptionSpec, RandomAlternatePicker.RandomOption>((TilemapLayerSpec.RandomOptionSpec option) => {
                            return new RandomAlternatePicker.RandomOption(option.Region, option.Odds);
                        }).ToArray()
                    );
                }
                return picker;
            }

            private bool? ConvertBlockingMode(TilemapLayerSpec.BlockingMode mode)
            {
                bool? blockingMode = null;
                switch (mode)
                {
                    case TilemapLayerSpec.BlockingMode.Blocking:
                        blockingMode = true; break;
                    case TilemapLayerSpec.BlockingMode.Unblocking:
                        blockingMode = false; break;
                }
                return blockingMode;
            }

            private List<TilemapLayer> CreateLayers()
            {
                return Layers.Select<TilemapLayerSpec, TilemapLayer>((TilemapLayerSpec spec) =>
                {
                    switch (spec.LayerType)
                    {
                        case TilemapLayerSpec.LayerSpecType.Biome:
                            if (!spec.BiomeSource) return null;
                            return new BiomeLayer(Width, Height, spec.BiomeSource, spec.BiomePresenceData, spec.BiomeExtendedPresence,
                                                  ConvertBlockingMode(spec.BiomeBlockingMode), spec.BiomeOffsetX, spec.BiomeOffsetY, CreatePicker(spec));
                        case TilemapLayerSpec.LayerSpecType.Filling:
                            if (!spec.FillingSource) return null;
                            return new FillingLayer(Width, Height, spec.FillingSource, spec.FillingBlocks, spec.FillingSourceRect, CreatePicker(spec));
                        case TilemapLayerSpec.LayerSpecType.Custom:
                            Dictionary<Color32, CustomLayer.PaletteSource> palette = new Dictionary<Color32, CustomLayer.PaletteSource>();
                            foreach (TilemapLayerSpec.PaletteSource ps in spec.CustomPalette)
                            {
                                palette[ps.Color] = new CustomLayer.PaletteSource(ps.Source, ps.SourceRect, ConvertBlockingMode(ps.Blocking));
                            }
                            return new CustomLayer(Width, Height, spec.CustomSource, palette);
                        default:
                            return null;
                    }
                }).Where((TilemapLayer layer) => layer != null).ToList();
            }

            // Use this for initialization
            void Start()
            {
                new TilemapLoader(Width, Height, TileSize).Load(this.gameObject, CreateLayers());
            }
        }
    }
}