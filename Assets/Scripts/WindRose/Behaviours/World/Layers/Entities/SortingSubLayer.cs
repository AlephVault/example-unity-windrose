using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace Layers
            {
                namespace Objects
                {
                    using Support.Utils;
                    using Support.Behaviours;

                    /// <summary>
                    ///   <para>
                    ///     Sorting sub-layers depend on the map and create several children
                    ///       holding a <see cref="SortingGroup"/> component each one. Since
                    ///       the number of sorting groups being created depends on the map
                    ///       this object belongs to, this implies this object is required
                    ///       to be inside a map (it will be usually bound to a layer), but
                    ///       it will actually be inside a map's <see cref="EntitiesLayer"/>
                    ///       (actually, this component will be attached to a newly
                    ///       instantiated object by an <see cref="EntitiesLayer"/>).
                    ///   </para>
                    ///   <para>
                    ///     Related to those groups, this class provides methods to get any
                    ///       of these groups (e.g. to be used as a parent of another object).
                    ///       Such group objects will be ready right on Awake.
                    ///   </para>
                    /// </summary>
                    [RequireComponent(typeof(Normalized))]
                    [RequireComponent(typeof(SortingGroup))]
                    public class SortingSubLayer : MonoBehaviour
                    {
                        private SortingGroup[] sortingGroups;

                        void Awake()
                        {
                            EntitiesLayer entitiesLayer = Layout.RequireComponentInParent<EntitiesLayer>(this);
                            uint height = Layout.RequireComponentInParent<Map>(entitiesLayer).Height;
                            sortingGroups = new SortingGroup[height];
                            for(uint index = 0; index < height; index++)
                            {
                                GameObject newGameObject = new GameObject(string.Format("SubLayer{0}", index));
                                newGameObject.transform.parent = transform;
                                newGameObject.transform.localPosition = Vector3.zero;
                                newGameObject.transform.localScale = Vector3.one;
                                newGameObject.transform.localRotation = Quaternion.identity;
                                SortingGroup sortingGroup = newGameObject.AddComponent<SortingGroup>();
                                sortingGroup.sortingLayerID = 0;
                                sortingGroup.sortingOrder = (int)index;
                                sortingGroups[index] = sortingGroup;
                            }
                        }

                        /// <summary>
                        ///   Gets a sorting group given an index. Other layers may make use of this
                        ///     one to actually place their objects accordingly.
                        /// </summary>
                        /// <param name="index">The index to lookup</param>
                        /// <returns>The involved sorting group</returns>
                        public SortingGroup this[int index]
                        {
                            get
                            {
                                return sortingGroups[index];
                            }
                        } 
                    }
                }
            }
        }
    }
}
