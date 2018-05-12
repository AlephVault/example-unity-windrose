using UnityEngine;
using Support.Utils;

namespace WindRose
{
    namespace Behaviours
    {
        using Types;
        using Types.States;

        [ExecuteInEditMode]
        [RequireComponent(typeof(Pausable))]
        public class Positionable : MonoBehaviour
        {
            /**
             * A positionable object updates its position and solidness status
             *   to its holding layer.
             * 
             * It will have behaviors like walking and teleporting.
             */

            /* *********************** Initial data *********************** */

            [SerializeField]
            private uint width = 1;

            [SerializeField]
            private uint height = 1;

            [SerializeField]
            private uint initialX = 0;

            [SerializeField]
            private uint initialY = 0;

            [SerializeField]
            private SolidnessStatus initialSolidness = SolidnessStatus.Solid;

            /* *********************** Additional data *********************** */

            private Map parentMap = null;
            private MapState.MapObjectState mapObjectState = null;
            private bool paused = false;

            /* *********************** Public properties *********************** */

            public Map ParentMap { get { return parentMap; } }
            public uint Width { get { return width; } } // Referencing directly allows us to query the width without a map assigned yet.
            public uint Height { get { return width; } } // Referencing directly allows us to query the height without a map assigned yet.
            public uint X { get { return mapObjectState.X; } }
            public uint Y { get { return mapObjectState.Y; } }
            public uint Xf { get { return mapObjectState.Xf; } }
            public uint Yf { get { return mapObjectState.Yf; } }
            public Direction? Movement { get { return mapObjectState.Movement; } }
            public SolidnessStatus Solidness { get { return mapObjectState != null ? mapObjectState.Solidness : initialSolidness; } }
            
            void Start()
            {
                Initialize();
            }

            #if UNITY_EDITOR
            // This is lame since the function will still exist when running in the editor mode.
            // Although it will not exist when running the game once deployed, when testing this app
            //   in the editor with a lot of Positionable objects, it will slow down somewhat since
            //   there will a lot of calls to this Update method just checking the condition (which
            //   will always return false). This is a crap I cannot get rid of, until Unity allows
            //   a difference between Unity Editor being run, and Unity Editor in design time.
            private void Update()
            {
                if (!Application.isPlaying)
                {
                    transform.localPosition = new Vector3(initialX * Map.GAME_UNITS_PER_TILE_UNITS, - (int)initialY * Map.GAME_UNITS_PER_TILE_UNITS, transform.localPosition.z);
                }
            }
            #endif

            void OnDestroy()
            {
                Detach();
            }

            void OnAttached(object[] args)
            {
                parentMap = ((MapState)(args[0])).RelatedMap;
            }

            void OnDetached()
            {
                parentMap = null;
            }

            public void Initialize()
            {
                if (!Application.isPlaying) return;

                if (mapObjectState != null)
                {
                    return;
                }

                try
                {
                    // perhaps it will not be added now because the Map component is not yet initialized! (e.g. this method being called from Start())
                    // however, when the Map becomes ready, this method will be called, again, by the map itself, which will exist.
                    parentMap = Layout.RequireComponentInParent<Map>(this);
                    mapObjectState = new MapState.MapObjectState(this, initialX, initialY, width, height, initialSolidness);
                    mapObjectState.Attach(parentMap.InternalMapState);
                }
                catch (Layout.MissingComponentInParentException)
                {
                    // nothing - diaper
                }
            }

            public void Detach()
            {
                // There are some times at startup when the MapState object may be null.
                // That's why we run the conditional.
                //
                // For the general cases, Detach will find a mapObjectState attached.
                if (mapObjectState != null) mapObjectState.Detach();
            }

            public void Attach(Map map, uint? x = null, uint? y = null, bool force = false)
            {
                if (force) Detach();
                mapObjectState.Attach(map != null ? map.InternalMapState : null, x, y);
            }

            public void Teleport(uint? x, uint? y)
            {
                if (mapObjectState != null && !paused) mapObjectState.Teleport(x, y);
            }

            public void SetSolidness(SolidnessStatus newSolidness)
            {
                if (mapObjectState != null && !paused) mapObjectState.SetSolidness(newSolidness);
            }

            public bool StartMovement(Direction movementDirection)
            {
                return mapObjectState != null && !paused && mapObjectState.StartMovement(movementDirection);
            }

            public bool FinishMovement()
            {
                return mapObjectState != null && !paused && mapObjectState.FinishMovement();
            }

            public bool CancelMovement()
            {
                return mapObjectState != null && !paused && mapObjectState.CancelMovement();
            }

            void Pause(bool fullFreeze)
            {
                paused = true;
            }

            void Resume()
            {
                paused = false;
            }
        }
    }
}