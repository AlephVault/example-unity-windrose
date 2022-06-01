using GameMeanMachine.Unity.RefMapChars.Types;
using GameMeanMachine.Unity.RefMapChars.Types.Traits;
using UnityEngine;


namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   A base RefMAp applier. This applier only makes use of body,
            ///   hair, hat, necklace, and hand tools. Clothes must be defined
            ///   later, in children classes. Also, what to do with the child
            ///   classes must be decided later (e.g. full-moving characters,
            ///   or only-pointing characters).
            /// </summary>
            public abstract class RefMapBaseApplier : MonoBehaviour, IRefMapBaseComposite,
                IApplier<BodyTrait>, IApplier<HairTrait>, IApplier<HatTrait>, IApplier<NecklaceTrait>,
                IApplier<SkilledHandToolTrait>, IApplier<DumbHandToolTrait>
            {
                /// <summary>
                ///   The body trait.
                /// </summary>
                public RefMapSource Body { get; private set; }
                
                /// <summary>
                ///   The hair trait. It does not include the tail.
                /// </summary>
                public RefMapSource Hair { get; private set; }

                /// <summary>
                ///   The hair tail trait. Not all hairs have tail.
                /// </summary>
                public RefMapSource HairTail { get; private set; }

                /// <summary>
                ///   The necklace trait.
                /// </summary>
                public RefMapSource Necklace { get; private set; }

                /// <summary>
                ///   The hat trait.
                /// </summary>
                public RefMapSource Hat { get; private set; }

                /// <summary>
                ///   The skilled hand item (e.g. weapon) trait.
                /// </summary>
                public RefMapSource SkilledHandItem { get; private set; }

                /// <summary>
                ///   The dumb hand item (e.g. shield) trait.
                /// </summary>
                public RefMapSource DumbHandItem { get; private set; }

                /// <summary>
                ///   A hash function that describes all the assigned
                ///   traits - not just those defined here.
                /// </summary>
                /// <returns>The hash of the current setting</returns>
                public abstract string Hash();

                /// <summary>
                ///   Applies a body trait. When passing null, it clears
                ///   the body trait.
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                public void Use(BodyTrait appliance)
                {
                    Body = appliance?.Front;
                    RefreshTexture();
                }

                /// <summary>
                ///   Applies a hair trait (which may include tail). When
                ///   passing null, it clears the hair trait (which may
                ///   include clearing also the tail, if present).
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                public void Use(HairTrait appliance)
                {
                    Hair = appliance?.Front;
                    HairTail = appliance?.Back;
                    RefreshTexture();
                }

                /// <summary>
                ///   Applies a hat trait. When passing null, it clears
                ///   the hat trait.
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                public void Use(HatTrait appliance)
                {
                    Hat = appliance?.Front;
                    RefreshTexture();
                }

                /// <summary>
                ///   Applies a necklace trait. When passing null, it clears
                ///   the necklace trait.
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                public void Use(NecklaceTrait appliance)
                {
                    Necklace = appliance?.Front;
                    RefreshTexture();
                }

                /// <summary>
                ///   Applies a skilled hand tool trait. When passing null,
                ///   it clears the skilled hand tool trait.
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                public void Use(SkilledHandToolTrait appliance)
                {
                    SkilledHandItem = appliance?.Front;
                    RefreshTexture();
                }

                /// <summary>
                ///   Applies a dumb hand tool trait. When passing null,
                ///   it clears the dumb hand tool trait.
                /// </summary>
                /// <param name="appliance">The appliance to set</param>
                public void Use(DumbHandToolTrait appliance)
                {
                    DumbHandItem = appliance?.Front;
                    RefreshTexture();
                }

                protected abstract void RefreshTexture();
            }
        }
    }
}