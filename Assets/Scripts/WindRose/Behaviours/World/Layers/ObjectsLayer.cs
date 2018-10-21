using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace Layers
            {
                [RequireComponent(typeof(Grid))]
                public class ObjectsLayer : MapLayer
                {
                    private Grid grid;

                    protected override void Awake()
                    {
                        base.Awake();
                        grid = GetComponent<Grid>();
                    }

                    protected override int GetSortingOrder()
                    {
                        return 20;
                    }

                    public float GetCellWidth()
                    {
                        return grid.cellSize.x;
                    }

                    public float GetCellHeight()
                    {
                        return grid.cellSize.y;
                    }
                }
            }
        }
    }
}
