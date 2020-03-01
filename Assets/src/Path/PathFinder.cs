using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using UnityEngine;

namespace src.Path
{
    public class PathFinder
    {
        private readonly Dictionary<Vector3M, Vector3M> _cameFrom = new Dictionary<Vector3M, Vector3M>();

        private readonly GridManager _gridManager;
        private readonly Dictionary<Vector3M, int> _pathCost = new Dictionary<Vector3M, int>();
        private readonly SimplePriorityQueue<Vector3M> _priorityQueue = new SimplePriorityQueue<Vector3M>();

        public PathFinder(GridManager grid)
        {
            _gridManager = grid;
        }

        public List<Vector3M> Find(in Vector3Int start, in Vector3Int goal)
        {
            return Find(new Vector3M(start), new Vector3M(goal));
        }

        private List<Vector3M> RestorePath(Vector3M goal)
        {
            var list = new List<Vector3M>();
            var prev = goal;
            while (prev != null)
            {
                list.Add(prev);
                prev = _cameFrom[prev];
            }

            return list;
        }

        private List<Vector3M> Find(Vector3M start, Vector3M goal)
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

                var isGoal = current.V.x == goal.V.x && current.V.y == goal.V.y;
                if (isGoal) return RestorePath(goal);

                foreach (var node in current.Neighbours(_gridManager))
                {
                    var newPathCost = _pathCost[current] + 1;

                    var isVisited = _cameFrom.ContainsKey(node);
                    if (!isVisited || newPathCost < _pathCost[node])
                    {
                        var priority = newPathCost + Euristic(node, goal);
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
            var dx = start.V.x - goal.V.x;
            var dy = start.V.y - goal.V.y;
            return dx * dx + dy * dy;
        }
    }
}