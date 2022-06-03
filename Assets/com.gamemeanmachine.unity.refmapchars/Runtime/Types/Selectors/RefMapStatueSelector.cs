using AlephVault.Unity.SpriteUtils.Types;
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
            ///   The selection to spawn a staying-only statue rose.
            /// </summary>
            public class RefMapStatueSelection : RoseSpritedSelection
            {
                public RefMapStatueSelection(SpriteGrid sourceGrid) : base(sourceGrid, new RoseTuple<Vector2Int>(
                    new Vector2Int(0, 0), new Vector2Int(0, 1), 
                    new Vector2Int(0, 2), new Vector2Int(0, 3))
                )
                {
                }
            }
        }
    }
}