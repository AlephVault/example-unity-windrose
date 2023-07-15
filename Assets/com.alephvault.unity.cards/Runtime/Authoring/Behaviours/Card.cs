using AlephVault.Unity.Cards.Authoring.ScriptableObjects;
using AlephVault.Unity.Support.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace AlephVault.Unity.Cards
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   Each card is a UI image but linked to a repository. There
            ///   are twp types of card layouts: Static layouts, where the
            ///   layouts are determined by the rules, and dynamic layouts,
            ///   where the layouts are totally determined by the user, or
            ///   to some extent. In the first layout, the cards are already
            ///   pre-allocated. In the second layout, cards can be added
            ///   dynamically.
            /// </summary>
            [RequireComponent(typeof(Image))]
            public class Card : MonoBehaviour
            {
                // The underlying image.
                private Image image;

                /// <summary>
                ///   The deck this card belongs to. Either as a prefab or
                ///   inserted in a scene, this value must be set a priori.
                /// </summary>
                [SerializeField]
                private Deck deck;

                /// <summary>
                ///   See <see cref="deck" />.
                /// </summary>
                public Deck Deck;

                /// <summary>
                ///   The background. It must be a non-negative index in the
                ///   deck.
                /// </summary>
                [SerializeField]
                private int background;

                /// <summary>
                ///   See <see cref="background" />.
                /// </summary>
                public int Background
                {
                    get => background;
                    set
                    {
                        int old = background;
                        background = Values.Clamp(0, value, Deck.Backgrounds.Count - 1);
                        if (old != background && !FacingUp)
                        {
                            image.sprite = deck.Backgrounds.Count > 0 ? deck.Backgrounds[background] : null;
                        }
                    }
                }

                /// <summary>
                ///   The face. It must be a non-negative index in the deck.
                /// </summary>
                [SerializeField]
                private int face;

                /// <summary>
                ///   See <see cref="face" />.
                /// </summary>
                public int Face
                {
                    get => face;
                    set
                    {
                        int old = face;
                        face = Values.Clamp(0, face, Deck.Cards.Count - 1);
                        if (old != face && FacingUp)
                        {
                            image.sprite = deck.Cards.Count > 0 ? deck.Cards[face] : null;
                        }
                    }
                }

                /// <summary>
                ///   The flip status (<c>true</c> means the card looks up).
                /// </summary>
                public bool FacingUp { get; private set; }

                private void Awake()
                {
                    image = GetComponent<Image>();
                    image.sprite = deck.Backgrounds.Count > 0 ? deck.Backgrounds[0] : null;
                }

                public async void FaceUp(bool animated = false)
                {
                    if (FacingUp) return;
                    FacingUp = true;
                    if (!animated)
                    {
                        image.sprite = deck.Cards.Count > 0 ? deck.Cards[face] : null;
                    }
                    else
                    {
                        // TODO implement loop.
                    }

                }

                public async void FaceDown(bool animated = false)
                {
                    if (!FacingUp) return;
                    FacingUp = true;
                    if (!animated)
                    {
                        image.sprite = deck.Backgrounds.Count > 0 ? deck.Backgrounds[background] : null;
                    }
                    else
                    {
                        // TODO implement loop.
                    }
                }
            }
        }
    }
}
