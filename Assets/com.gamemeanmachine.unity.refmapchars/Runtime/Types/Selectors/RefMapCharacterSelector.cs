using System;
using System.Collections.ObjectModel;
using AlephVault.Unity.SpriteUtils.Types;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using GameMeanMachine.Unity.WindRose.SpriteUtils.Types;
using GameMeanMachine.Unity.WindRose.SpriteUtils.Types.Selectors;
using GameMeanMachine.Unity.WindRose.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Types
    {
        namespace Selectors
        {
            /// <summary>
            ///   The selection to spawn a moving / staying character animation
            ///   rose mapping.
            /// </summary>
            public class RefMapCharacterSelection : MultiRoseAnimatedSelection
            {
                public RefMapCharacterSelection(SpriteGrid sourceGrid, uint framesPerSecond) : base(
                    sourceGrid, 
                    new MultiSettings<RoseTuple<ReadOnlyCollection<Vector2Int>>>
                    {
                        { MapObject.IDLE_STATE, new RoseTuple<ReadOnlyCollection<Vector2Int>>(
                            Array.AsReadOnly(new []{ new Vector2Int(0, 3) }),
                            Array.AsReadOnly(new []{ new Vector2Int(0, 1) }),
                            Array.AsReadOnly(new []{ new Vector2Int(0, 2) }),
                            Array.AsReadOnly(new []{ new Vector2Int(0, 0) })
                          )},
                        { MapObject.MOVING_STATE, new RoseTuple<ReadOnlyCollection<Vector2Int>>(
                            Array.AsReadOnly(new []
                            {
                                new Vector2Int(0, 3), new Vector2Int(1, 3),
                                new Vector2Int(2, 3), new Vector2Int(3, 3)                                
                            }),
                            Array.AsReadOnly(new []
                            {
                                new Vector2Int(0, 1), new Vector2Int(1, 1),
                                new Vector2Int(2, 1), new Vector2Int(3, 1)
                            }),
                            Array.AsReadOnly(new []
                            {
                                new Vector2Int(0, 2), new Vector2Int(1, 2),
                                new Vector2Int(2, 2), new Vector2Int(3, 2)
                            }),
                            Array.AsReadOnly(new []
                            {
                                new Vector2Int(0, 0), new Vector2Int(1, 0),
                                new Vector2Int(2, 0), new Vector2Int(3, 0)
                            })
                          )}
                    },
                    framesPerSecond
                )
                {
                }
            }
        }
    }
}