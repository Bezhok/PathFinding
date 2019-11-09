using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using UnityEngine;

namespace src.Path
{
    public class PathFinder
    {
        private SimplePriorityQueue<Vector3M> _priorityQueue = new SimplePriorityQueue<Vector3M>();
        private Dictionary<Vector3M, Vector3M> _cameFrom = new Dictionary<Vector3M, Vector3M>();
        private Dictionary<Vector3M, int> _pathCost = new Dictionary<Vector3M, int>();

        private GridManager _gridManager;

        public PathFinder(GridManager grid)
        {
            _gridManager = grid;
        }

        public List<Vector3M> Find(in Vector3Int start, in Vector3Int goal)
        {
            return Find(new Vector3M(start), new Vector3M(goal));
        }

        List<Vector3M> Find(Vector3M start, Vector3M goal)
        {
            _priorityQueue.Clear();
            _cameFrom.Clear();
            _pathCost.Clear();

            _priorityQueue.Enqueue(start, 0);
            _pathCost[start] = 0;
            _cameFrom[start] = null;

            while (_priorityQueue.Any())
            {
                var current = _priorityQueue.Dequeue();

                if (current.v.x == goal.v.x && current.v.y == goal.v.y)
                {
                    var list = new List<Vector3M>();
                    var prev = goal;
                    while (prev != null)
                    {
//                        _gridManager.Map[prev.v.x, prev.v.y].Renderer.material.color = Color.yellow;
                        list.Add(prev);
                        prev = _cameFrom[prev];
                    }

                    return list;
                }

                foreach (var node in current.Neighbours(_gridManager))
                {
                    int newPathCost = _pathCost[current] + 1;

                    if (!_cameFrom.ContainsKey(node) || newPathCost < _pathCost[node])
                    {
                        _gridManager.Map[node.v.x, node.v.y].Renderer.material.color = Color.magenta;
                        if (!_cameFrom.ContainsKey(node))
                        {
                            _gridManager.Map[node.v.x, node.v.y].Renderer.material.color = Color.cyan;
                        }

                        int priority = newPathCost + Euristic(node, goal);
                        _priorityQueue.Enqueue(node, priority);
                        _pathCost[node] = newPathCost;
                        _cameFrom[node] = current;
                    }
                }
            }
            return null;
        }

        private int Euristic(in Vector3M start, in Vector3M goal)
        {
            int dx = start.v.x - goal.v.x;
            int dy = start.v.y - goal.v.y;
            return dx * dx + dy * dy;
        }
    }
}