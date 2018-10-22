using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Ceilings
        {
            using World.Layers;

            [RequireComponent(typeof(Tilemap))]
            [RequireComponent(typeof(TilemapRenderer))]
            public class Ceiling : MonoBehaviour
            {
                /**
                 * This class defined a ceiling and provides means to
                 *   show it, hide it, or make it translucent.
                 */

                private TilemapRenderer tilemapRenderer;
                private Grid parentGrid;

                public enum DisplayMode { HIDDEN, TRANSLUCENT, VISIBLE }

                public DisplayMode displayMode;

                [SerializeField]
                [Range(0, 1)]
                private float opacityInTranslucentMode;

                [SerializeField]
                private string materialColorVariable = "_Color";

                public float DisplayModeOpacity
                {
                    get { return opacityInTranslucentMode; }
                    set { opacityInTranslucentMode = Support.Utils.Values.Clamp(0, value, 1); }
                }

                private void Awake()
                {
                    CeilingLayer ceilingLayer = Support.Utils.Layout.RequireComponentInParent<CeilingLayer>(this);
                    parentGrid = ceilingLayer.GetComponent<Grid>();
                    tilemapRenderer = GetComponent<TilemapRenderer>();
                }

                private void Start()
                {
                    // Rounding position and setting relative z to 0.
                    transform.localPosition = parentGrid.CellToLocal(parentGrid.LocalToCell(new Vector3(transform.localPosition.x, transform.localPosition.y, 0)));
                }

                private void Update()
                {
                    try
                    {
                        Color color = tilemapRenderer.material.GetColor(materialColorVariable);
                        switch(displayMode)
                        {
                            case DisplayMode.HIDDEN:
                                color.a = 0;
                                break;
                            case DisplayMode.VISIBLE:
                                color.a = 1;
                                break;
                            case DisplayMode.TRANSLUCENT:
                                color.a = opacityInTranslucentMode;
                                break;
                        }
                        tilemapRenderer.material.SetColor(materialColorVariable, color);
                    }
                    catch(Exception)
                    {
                        // Diaper - nothing will be done here.
                    }
                }
            }
        }
    }
}