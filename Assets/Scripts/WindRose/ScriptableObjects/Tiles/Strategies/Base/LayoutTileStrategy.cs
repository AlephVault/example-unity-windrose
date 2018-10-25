using UnityEngine;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Tiles
        {
            namespace Strategies
            {
                namespace Base
                {
                    [CreateAssetMenu(fileName = "NewLayoutTileStrategy", menuName = "Wind Rose/Tile Strategies/Layout", order = 201)]
                    public class LayoutTileStrategy : TileStrategy
                    {
                        /**
                         * Layout strategies tell whether the cell
                         *   blocks the steps or allows walking.
                         */

                        [SerializeField]
                        private bool blocks;

                        public bool Blocks {
                            get {
                                Debug.Log("In Block property ...");
                                return blocks;
                            }
                        }
                    }
                }
            }
        }
    }
}
