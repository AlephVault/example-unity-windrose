using GameMeanMachine.Unity.RefMapChars.Types;
using GameMeanMachine.Unity.RefMapChars.Types.Traits;
using GameMeanMachine.Unity.RefMapChars.Types.Traits.Standard;
using UnityEngine;


namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   A RefMap standard applier adds the 9 missing parts
            ///   to define the cloth traits.
            /// </summary>
            public abstract class RefMapStandardApplier : RefMapApplier,
                IApplier<BootsTrait>, IApplier<PantsTrait>, IApplier<ShirtTrait>,
                IApplier<ChestTrait>, IApplier<WaistTrait>, IApplier<ArmsTrait>,
                IApplier<LongShirtTrait>, IApplier<ShoulderTrait>, IApplier<CloakTrait>
            {
                /// <summary>
                ///   Whether the boots should go over the
                ///   pants or not.
                /// </summary>
                [SerializeField]
                protected bool bootsOverPants;

                /// <summary>
                ///   Whether the necklace should go over the
                ///   long shirt or not.
                /// </summary>
                [SerializeField]
                protected bool necklaceOverLongShirt;
                
                /// <summary>
                ///   The boots image.
                /// </summary>
                public override RefMapSource Boots => bootsTrait?.Front;

                /// <summary>
                ///   The pants image.
                /// </summary>
                public override RefMapSource Pants => pantsTrait?.Front;

                /// <summary>
                ///   The shirt image. Usually, with no arms.
                /// </summary>
                public override RefMapSource Shirt => shirtTrait?.Front;

                /// <summary>
                ///   The chest (light armor) image.
                /// </summary>
                public override RefMapSource Chest => chestTrait?.Front;

                /// <summary>
                ///   The waist image.
                /// </summary>
                public override RefMapSource Waist => waistTrait?.Front;

                /// <summary>
                ///   The arms image.
                /// </summary>
                public override RefMapSource Arms => armsTrait?.Front;

                /// <summary>
                ///   The long shirt image.
                /// </summary>
                public override RefMapSource LongShirt => longShirtTrait?.Front;

                /// <summary>
                ///   The shoulder image.
                /// </summary>
                public override RefMapSource Shoulder => shoulderTrait?.Front;

                /// <summary>
                ///   The cloak image.
                /// </summary>
                public override RefMapSource Cloak => cloakTrait?.Front;

                /// <summary>
                ///   Whether the boots should go over the
                ///   pants or not.
                /// </summary>
                public override bool BootsOverPants => bootsOverPants;

                /// <summary>
                ///   Whether the necklace should go over the
                ///   long shirt or not.
                /// </summary>
                public override bool NecklaceOverLongShirt => necklaceOverLongShirt;

                /// <summary>
                ///   The boots trait.
                /// </summary>
                protected BootsTrait bootsTrait;

                /// <summary>
                ///   The pants trait.
                /// </summary>
                protected PantsTrait pantsTrait;

                /// <summary>
                ///   The shirt trait. Usually with no arms.
                /// </summary>
                protected ShirtTrait shirtTrait;

                /// <summary>
                ///   The chest (light armor) trait.
                /// </summary>
                protected ChestTrait chestTrait;

                /// <summary>
                ///   The waist trait.
                /// </summary>
                protected WaistTrait waistTrait;

                /// <summary>
                ///   The arms trait.
                /// </summary>
                protected ArmsTrait armsTrait;

                /// <summary>
                ///   The long shirt trait.
                /// </summary>
                protected LongShirtTrait longShirtTrait;

                /// <summary>
                ///   The shoulder trait.
                /// </summary>
                protected ShoulderTrait shoulderTrait;

                /// <summary>
                ///   The cloak trait.
                /// </summary>
                protected CloakTrait cloakTrait;
                
                /// <summary>
                ///   The cloth hash involves the 9 cloth parts.
                /// </summary>
                protected override string ClothHash()
                {
                    return $"{bootsTrait?.Hash ?? ""}:{pantsTrait?.Hash ?? ""}:{shirtTrait?.Hash ?? ""}:" +
                           $"{chestTrait?.Hash ?? ""}:{waistTrait?.Hash ?? ""}:{armsTrait?.Hash ?? ""}:" +
                           $"{longShirtTrait?.Hash ?? ""}:{shoulderTrait?.Hash ?? ""}:{cloakTrait?.Hash ?? ""}";
                }

                /// <summary>
                ///   Applies a boots trait. When passing null, it clears
                ///   the boots trait.
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                /// <param name="force">Whether to force the update or not</param>
                public void Use(BootsTrait appliance, bool force = true)
                {
                    bootsTrait = appliance;
                    if (force) RefreshTexture();
                }

                /// <summary>
                ///   Applies a pants trait. When passing null, it clears
                ///   the pants trait.
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                /// <param name="force">Whether to force the update or not</param>
                public void Use(PantsTrait appliance, bool force = true)
                {
                    pantsTrait = appliance;
                    if (force) RefreshTexture();
                }

                /// <summary>
                ///   Applies a shirt trait. When passing null, it clears
                ///   the shirt trait.
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                /// <param name="force">Whether to force the update or not</param>
                public void Use(ShirtTrait appliance, bool force = true)
                {
                    shirtTrait = appliance;
                    if (force) RefreshTexture();
                }

                /// <summary>
                ///   Applies a chest trait. When passing null, it clears
                ///   the chest trait.
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                /// <param name="force">Whether to force the update or not</param>
                public void Use(ChestTrait appliance, bool force = true)
                {
                    chestTrait = appliance;
                    if (force) RefreshTexture();
                }

                /// <summary>
                ///   Applies a waist trait. When passing null, it clears
                ///   the waist trait.
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                /// <param name="force">Whether to force the update or not</param>
                public void Use(WaistTrait appliance, bool force = true)
                {
                    waistTrait = appliance;
                    if (force) RefreshTexture();
                }

                /// <summary>
                ///   Applies an arms trait. When passing null, it clears
                ///   the arms trait.
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                /// <param name="force">Whether to force the update or not</param>
                public void Use(ArmsTrait appliance, bool force = true)
                {
                    armsTrait = appliance;
                    if (force) RefreshTexture();
                }

                /// <summary>
                ///   Applies a long shirt trait. When passing null, it clears
                ///   the long shirt trait.
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                /// <param name="force">Whether to force the update or not</param>
                public void Use(LongShirtTrait appliance, bool force = true)
                {
                    longShirtTrait = appliance;
                    if (force) RefreshTexture();
                }

                /// <summary>
                ///   Applies a shoulder trait. When passing null, it clears
                ///   the shoulder trait.
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                /// <param name="force">Whether to force the update or not</param>
                public void Use(ShoulderTrait appliance, bool force = true)
                {
                    shoulderTrait = appliance;
                    if (force) RefreshTexture();
                }

                /// <summary>
                ///   Applies a cloak trait. When passing null, it clears
                ///   the cloak trait.
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                /// <param name="force">Whether to force the update or not</param>
                public void Use(CloakTrait appliance, bool force = true)
                {
                    cloakTrait = appliance;
                    if (force) RefreshTexture();
                }
            }
        }
    }
}