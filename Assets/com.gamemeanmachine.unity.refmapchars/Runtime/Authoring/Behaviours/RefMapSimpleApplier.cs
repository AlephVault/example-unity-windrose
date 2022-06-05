using AlephVault.Unity.SpriteUtils.Types;
using GameMeanMachine.Unity.RefMapChars.Types;
using GameMeanMachine.Unity.RefMapChars.Types.Traits;
using GameMeanMachine.Unity.RefMapChars.Types.Traits.Simple;


namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   A simple RefMAp applier. Aside from the base traits, it
            ///   adds a single cloth. How is the grid used, it is not
            ///   defined in this class but in children classes.
            /// </summary>
            public abstract class RefMapSimpleApplier : RefMapBaseApplier, IRefMapSimpleComposite,
                IApplier<ClothTrait>
            {
                /// <summary>
                ///   The cloth image.
                /// </summary>
                public RefMapSource Cloth => clothTrait?.Front;

                /// <summary>
                ///   The cloth trait.
                /// </summary>
                protected ClothTrait clothTrait;
                
                /// <summary>
                ///   The hash involves the 7 parts.
                /// </summary>
                public override string Hash()
                {
                    return $"{bodyTrait?.Hash ?? ""}:{hairTrait?.Hash ?? ""}:{necklaceTrait?.Hash ?? ""}:" +
                           $"{hatTrait?.Hash ?? ""}:{skilledHandItemTrait?.Hash ?? ""}:" +
                           $"{dumbHandItemTrait?.Hash ?? ""}:{clothTrait?.Hash ?? ""}";
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

                /// <summary>
                ///   Applies a full cloth trait. When passing null,
                ///   it clears the full cloth trait.
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                /// <param name="force">Whether to force the update or not</param>
                public void Use(ClothTrait appliance, bool force = true)
                {
                    clothTrait = appliance;
                    if (force) RefreshTexture();
                }
            }
        }
    }
}