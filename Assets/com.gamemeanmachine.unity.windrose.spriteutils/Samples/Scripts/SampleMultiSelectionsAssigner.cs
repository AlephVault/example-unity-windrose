using System;
using System.Collections.ObjectModel;
using System.Linq;
using AlephVault.Unity.SpriteUtils.Types;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using GameMeanMachine.Unity.WindRose.SpriteUtils.Authoring.Behaviours;
using GameMeanMachine.Unity.WindRose.SpriteUtils.Types;
using GameMeanMachine.Unity.WindRose.SpriteUtils.Types.Selectors;
using GameMeanMachine.Unity.WindRose.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Samples
    {
        public class SampleMultiSelectionsAssigner : MonoBehaviour
        {
            [SerializeField]
            private Texture2D[] textures;

            [SerializeField]
            private SpritedSelectionApplier spritedApplier;

            [SerializeField]
            private RoseSpritedSelectionApplier roseSpritedApplier;

            [SerializeField]
            private AnimatedSelectionApplier animatedApplier;

            [SerializeField]
            private RoseAnimatedSelectionApplier roseAnimatedApplier;

            [SerializeField]
            private MultiSpritedSelectionApplier multiSpritedApplier;

            [SerializeField]
            private MultiRoseSpritedSelectionApplier multiRoseSpritedApplier;

            [SerializeField]
            private MultiAnimatedSelectionApplier multiAnimatedApplier;

            [SerializeField]
            private MultiRoseAnimatedSelectionApplier multiRoseAnimatedApplier;
            
            private IdentifiedSpriteGridPool<int> spriteGridPool;

            void Awake()
            {
                spriteGridPool = new IdentifiedSpriteGridPool<int>(3);
            }
            
            // Update is called once per frame
            void Update()
            {
                bool found = false;
                for(KeyCode key = KeyCode.Alpha0; key <= KeyCode.Alpha9; key++)
                {
                    if (Input.GetKeyDown(key))
                    {
                        UseGrid(key - KeyCode.Alpha0);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    if (Input.GetKeyDown(KeyCode.Minus))
                    {
                        ReleaseSelections();
                    }
                }
            }

            void ReleaseSelections()
            {
                if (spritedApplier) spritedApplier.ReleaseSelection();
                if (roseSpritedApplier) roseSpritedApplier.ReleaseSelection();
                if (animatedApplier) animatedApplier.ReleaseSelection();
                if (roseAnimatedApplier) roseAnimatedApplier.ReleaseSelection();
                if (multiSpritedApplier) multiSpritedApplier.ReleaseSelection();
                if (multiRoseSpritedApplier) multiRoseSpritedApplier.ReleaseSelection();
                if (multiAnimatedApplier) multiAnimatedApplier.ReleaseSelection();
                if (multiRoseAnimatedApplier) multiRoseAnimatedApplier.ReleaseSelection();
            }

            void UseGrid(int index)
            {
                IdentifiedSpriteGrid<int> spriteGrid = spriteGridPool.Get(index, () =>
                {
                    return new Tuple<Texture2D, Rect?, uint, uint, float, Action, Action>(
                        textures[index], null, 32, 32, 32, null, null
                    );
                });
                spritedApplier.UseSelection(new SpritedSelection(spriteGrid, new Vector2Int(2, 0)));
                roseSpritedApplier.UseSelection(new RoseSpritedSelection(spriteGrid, new RoseTuple<Vector2Int>(
                    new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(3, 0), new Vector2Int(2, 0)
                )));
                animatedApplier.UseSelection(new AnimatedSelection(spriteGrid, Array.AsReadOnly(new []
                {
                    new Vector2Int(2, 0), new Vector2Int(2, 1)
                }), 2));
                roseAnimatedApplier.UseSelection(new RoseAnimatedSelection(spriteGrid, new RoseTuple<ReadOnlyCollection<Vector2Int>>(
                    Array.AsReadOnly(new []
                    {
                        new Vector2Int(0, 0), new Vector2Int(0, 1)
                    }),
                    Array.AsReadOnly(new []
                    {
                        new Vector2Int(1, 0), new Vector2Int(1, 1)
                    }),
                    Array.AsReadOnly(new []
                    {
                        new Vector2Int(3, 0), new Vector2Int(3, 1)
                    }),
                    Array.AsReadOnly(new []
                    {
                        new Vector2Int(2, 0), new Vector2Int(2, 1)
                    })
                ), 2));
                multiSpritedApplier.UseSelection(new MultiSpritedSelection(spriteGrid, new MultiSettings<Vector2Int>
                {
                    { "", new Vector2Int(2, 0) },
                    { "alt", new Vector2Int(2, 2) },
                }));
                multiRoseSpritedApplier.UseSelection(new MultiRoseSpritedSelection(spriteGrid, new MultiSettings<RoseTuple<Vector2Int>>
                {
                    { "", new RoseTuple<Vector2Int>(
                        new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(3, 0), new Vector2Int(2, 0)
                    )},
                    { "alt", new RoseTuple<Vector2Int>(
                        new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(3, 2), new Vector2Int(2, 2)
                    )},
                }));
                multiAnimatedApplier.UseSelection(new MultiAnimatedSelection(spriteGrid, new MultiSettings<ReadOnlyCollection<Vector2Int>>
                {
                    { "", Array.AsReadOnly(new [] {
                        new Vector2Int(2, 0), new Vector2Int(2, 1)
                    })},
                    { "alt", Array.AsReadOnly(new [] {
                        new Vector2Int(2, 2), new Vector2Int(2, 3)
                    })},
                }, 2));
                multiRoseAnimatedApplier.UseSelection(new MultiRoseAnimatedSelection(spriteGrid, new MultiSettings<RoseTuple<ReadOnlyCollection<Vector2Int>>>
                {
                    { "", new RoseTuple<ReadOnlyCollection<Vector2Int>>(
                        Array.AsReadOnly(new []
                        {
                            new Vector2Int(0, 0), new Vector2Int(0, 1)
                        }),
                        Array.AsReadOnly(new []
                        {
                            new Vector2Int(1, 0), new Vector2Int(1, 1)
                        }),
                        Array.AsReadOnly(new []
                        {
                            new Vector2Int(3, 0), new Vector2Int(3, 1)
                        }),
                        Array.AsReadOnly(new []
                        {
                            new Vector2Int(2, 0), new Vector2Int(2, 1)
                        })
                    )},
                    { "alt", new RoseTuple<ReadOnlyCollection<Vector2Int>>(
                        Array.AsReadOnly(new []
                        {
                            new Vector2Int(0, 2), new Vector2Int(0, 3)
                        }),
                        Array.AsReadOnly(new []
                        {
                            new Vector2Int(1, 2), new Vector2Int(1, 3)
                        }),
                        Array.AsReadOnly(new []
                        {
                            new Vector2Int(3, 2), new Vector2Int(3, 3)
                        }),
                        Array.AsReadOnly(new []
                        {
                            new Vector2Int(2, 2), new Vector2Int(2, 3)
                        })
                    )}
                }, 2));
            }
        }
    }
}
