using System.Collections.Generic;
using UnityEngine;

namespace src.Path
{
    public class Vector3M
    {
        public Vector3Int v;

        public Vector3M(int x, int y, int z)
        {
            v.x = x;
            v.y = y;
            v.z = z;
        }

        public Vector3M(in Vector3Int pos)
        {
            v = pos;
        }

        static private int[,] Mask =
        {
            {1, 0, 0},
            {-1, 0, 0},
            {0, 1, 0},
            {0, -1, 0}
        };

        public List<Vector3M> Neighbours(GridManager gridManager)
        {
            int maxX = gridManager.ColumnsNumber;
            int maxY = gridManager.RowsNumber;

            var neighbours = new List<Vector3M>();

            Vector3M vec;
            for (int i = 0; i < Mask.GetLength(0); i++)
            {
                vec = new Vector3M(v.x + Mask[i, 0], v.y + Mask[i, 1], v.z + Mask[i, 2]);
                if (gridManager.CanMakeMove(vec.v.x, vec.v.y, vec.v.z))
                    neighbours.Add(vec);
            }

            return neighbours;
        }

        public override bool Equals(object obj)
        {
            Vector3M vector3M = obj as Vector3M;

            if (vector3M == null)
            {
                return false;
            }

            return v.Equals(vector3M.v);
        }

        public override int GetHashCode()
        {
            return v.GetHashCode();
        }
    }
}