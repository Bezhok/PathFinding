using System.Collections.Generic;
using Priority_Queue;
using UnityEditor.UIElements;
using UnityEngine;

namespace src
{
    public class PathFinder
    {
        private class Vector3M
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
        
        private SimplePriorityQueue<Vector3M> _priorityQueue = new SimplePriorityQueue<Vector3M>();
        private Dictionary<Vector3M, Vector3M> _cameFrom = new Dictionary<Vector3M, Vector3M>();
        private Dictionary<Vector3M, int> _pathCost = new Dictionary<Vector3M, int>();
        
        private GridManager _gridManager;
        
        public PathFinder(GridManager grid)
        {
            _gridManager = grid;
        }

        public void Find(in Vector3Int start, in Vector3Int goal)
        {
            Find(new Vector3M(start), new Vector3M(goal));
        }
        
        void Find(Vector3M start, Vector3M goal)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (_gridManager.Map[i, j].Content.Equals(CellContent.Empty))
                    {
                        _gridManager.Map[i, j].Renderer.material.color = Color.white;
                    }
                }
            }
            
            _priorityQueue.Clear();
            _cameFrom.Clear();
            _pathCost.Clear();
            
            _priorityQueue.Enqueue(start, 0);
            _pathCost[start] = 0;
            _cameFrom[start] = null;
            
            while (_priorityQueue.Count != 0)
            {
                var current = _priorityQueue.Dequeue();

                if (current.v.x == goal.v.x && current.v.y == goal.v.y)
                {
                    var prev = goal;
                    while (prev != null)
                    {
                        _gridManager.Map[prev.v.x, prev.v.y].Renderer.material.color = Color.yellow;
                        prev = _cameFrom[prev];
                    }
                    return;
                }

                foreach (var node in current.Neighbours(_gridManager))
                {
                    int newPathCost = _pathCost[current] + 1;

                    if (!_pathCost.ContainsKey(node) || newPathCost < _pathCost[node])
                    {
                        _gridManager.Map[node.v.x, node.v.y].Renderer.material.color = Color.magenta;
                        if (!_cameFrom.ContainsKey(node)) { _gridManager.Map[node.v.x, node.v.y].Renderer.material.color = Color.cyan;}
                        
                        int priority = newPathCost + Euristic(node, goal);
                        _priorityQueue.Enqueue(node, priority);
                        _pathCost[node] = newPathCost;
                        _cameFrom[node] = current;
                    }
                }
            }
        }

        private int Euristic(in Vector3M start, in Vector3M goal)
        {
            int dx = start.v.x - goal.v.x;
            int dy = start.v.y - goal.v.y;
            return dx * dx + dy * dy;
        }
    }
}