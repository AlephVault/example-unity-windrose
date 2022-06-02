using AlephVault.Unity.SpriteUtils.Types;
using GameMeanMachine.Unity.RefMapChars.Types;


namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   A RefMap applier uses all the parts of a RefMap
            ///   character, but does not provide a particular way
            ///   to set those extra components appropriately. Still
            ///   it provides a mean to render and use everything.
            /// </summary>
            public abstract class RefMapApplier : RefMapBaseApplier, IRefMapComposite
            {
                /// <summary>
                ///   The boots image.
                /// </summary>
                public abstract RefMapSource Boots { get; }

                /// <summary>
                ///   The pants image.
                /// </summary>
                public abstract RefMapSource Pants { get; }

                /// <summary>
                ///   The shirt image.
                /// </summary>
                public abstract RefMapSource Shirt { get; }

                /// <summary>
                ///   The chest image.
                /// </summary>
                public abstract RefMapSource Chest { get; }

                /// <summary>
                ///   The waist image.
                /// </summary>
                public abstract RefMapSource Waist { get; }

                /// <summary>
                ///   The arms image.
                /// </summary>
                public abstract RefMapSource Arms { get; }

                /// <summary>
                ///   The long shirt image.
                /// </summary>
                public abstract RefMapSource LongShirt { get; }

                /// <summary>
                ///   The shoulder image.
                /// </summary>
                public abstract RefMapSource Shoulder { get; }
                
                /// <summary>
                ///   The cloak image.
                /// </summary>
                public abstract RefMapSource Cloak { get; }
                
                /// <summary>
                ///   Whether the boots should be rendered above the pants.
                /// </summary>
                public abstract bool BootsOverPants { get; }
                
                /// <summary>
                ///   Whether the necklace should be rendered above the long shirt.
                /// </summary>
                public abstract bool NecklaceOverLongShirt { get; }

                /// <summary>
                ///   The cloth hash computes the hash for the 7 cloth parts.
                /// </summary>
                /// <returns></returns>
                protected abstract string ClothHash();
                
                /// <summary>
                ///   The hash involves the 13 parts.
                /// </summary>
                public override string Hash()
                {
                    return $"{bodyTrait?.Hash ?? ""}:{hairTrait?.Hash ?? ""}:{necklaceTrait?.Hash ?? ""}:" +
                           $"{hatTrait?.Hash ?? ""}:{skilledHandItemTrait?.Hash ?? ""}:" +
                           $"{dumbHandItemTrait?.Hash ?? ""}:{ClothHash()}:{BootsOverPants}:{NecklaceOverLongShirt}";
                }

                /// <summary>
                ///   Gets the grid, and uses it.
                /// </summary>
                protected override void RefreshTexture()
                {
                    UseGrid(cache.Get(this));
                }

                /// <summary>
                ///   Using the grid depends on the selection type
                ///   to make use of, and that depends on the type
                ///   of visual to tie this behaviour to.
                /// </summary>
                /// <param name="grid">The grid to use</param>
                protected abstract void UseGrid(SpriteGrid grid);
            }
        }
    }
}