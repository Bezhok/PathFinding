using System.Collections.Generic;
using UnityEngine;

namespace src.Path
{
    public class Vector3M
    {
        private static readonly int[,] NeighbourMask =
        {
            {1, 0, 0},
            {-1, 0, 0},
            {0, 1, 0},
            {0, -1, 0}
        };

        public Vector3Int V;

        public Vector3M(int x, int y, int z)
        {
            V.x = x;
            V.y = y;
            V.z = z;
        }

        public Vector3M(in Vector3Int pos)
        {
            V = pos;
        }

        public List<Vector3M> Neighbours(GridManager gridManager)
        {
            var neighbours = new List<Vector3M>();

            for (int i = 0; i < NeighbourMask.GetLength(0); i++)
            {
                var vec = new Vector3M(V.x + NeighbourMask[i, 0], V.y + NeighbourMask[i, 1], V.z + NeighbourMask[i, 2]);
                if (gridManager.CanMakeMove(vec.V.x, vec.V.y, vec.V.z))
                    neighbours.Add(vec);
            }

            return neighbours;
        }

        public override bool Equals(object obj)
        {
            var vector3M = obj as Vector3M;

            if (vector3M == null) return false;

            return V.Equals(vector3M.V);
        }

        public override int GetHashCode()
        {
            return V.GetHashCode();
        }
    }
}