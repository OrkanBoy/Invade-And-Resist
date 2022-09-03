using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

namespace Orkan
{
    public class PathFinding<T>
    {
        public const int STRAIGHT_COST = 10, DIAG_COST = 14;
        public readonly static (int x, int y, int cost)[] moves = {(0, 1, STRAIGHT_COST), (-1, 1, DIAG_COST), (-1, 0, STRAIGHT_COST), (-1, -1, DIAG_COST), (0, -1, STRAIGHT_COST), (1, -1, DIAG_COST), (1, 0, STRAIGHT_COST), (1, 1, DIAG_COST) };

        public Grid<Node> grid;
        public Grid<T> worldGrid;
        public Predicate<T> walkable = t => t == null;

        public PathFinding(Grid<T> worldGrid, Predicate<T> walkable = null)
        {
            grid = new Grid<Node>(worldGrid.width, worldGrid.height);
            if(walkable != null) this.walkable = walkable;
            this.worldGrid = worldGrid;
            RefreshGrid();
        }

        public void RefreshGrid() 
        {
            for (int x = 0; x < grid.width; x++)
                for (int y = 0; y < grid.height; y++)
                    grid[x, y] = new Node { x = x, y = y, walkable = walkable(worldGrid[x, y]), grid = grid };
        }

        public IEnumerable<(int, int)> FindPath(int startX, int startY, int endX, int endY) => FindPath_Backtracked(startX, startY, endX, endY).Reverse();

        private IEnumerable<(int, int)> FindPath_Backtracked(int startX, int startY, int endX, int endY) 
        {
            Node start = grid[startX, startY];
            start.gCost = 0;
            start.hCost = start.CalculateHCost(endX, endY);
            List<Node> explorable = new(){start};
            HashSet<Node> explored = new();

            Node current = start;
            while(explorable.Count > 0 && (current.x != endX || current.y != endY))
            {
                current = explorable[0];
                explorable.RemoveAt(0);
                foreach ((Node neighbour, int cost) in current.Neighbours)
                {
                    if (explored.Contains(neighbour)) continue;
                    int newgCost = current.gCost + cost;
                    int newhCost = neighbour.CalculateHCost(endX, endY);
                    if (neighbour.fCost > newgCost + newhCost)
                    {
                        neighbour.gCost = newgCost;
                        neighbour.hCost = newhCost;
                        neighbour.parent = current;
                    }
                    if(!explorable.Contains(neighbour)) explorable.Add(neighbour);
                }
                explored.Add(current);
                explorable.Sort( (r, l) =>
                {
                    if (r.fCost > l.fCost)
                        return 1;
                    if (r.fCost == l.fCost && r.hCost > l.hCost)
                        return 1;
                    if (r.gCost == l.gCost && r.hCost == l.hCost)
                        return 0;
                    return -1;
                });
                
            }
            Node end = explored.ArgMin(n => n.hCost);
            for (current = end; current != null; current = current.parent)
                yield return (current.x, current.y);
        }

        public IEnumerable<Vector2> FindPath(Vector2 start, Vector2 end) 
        {
            worldGrid.ToArrayPos(start, out int startX, out int startY);
            worldGrid.ToArrayPos(end, out int endX, out int endY);
            foreach ((int x, int y) in FindPath(startX, startY, endX, endY))
                yield return worldGrid.ToWorldPos(x, y);
        }

        public class Node
        {
            public Grid<Node> grid;
            public int x, y;
            public int gCost = 100000, hCost;
            public bool walkable;
            public Node parent;
            public int fCost => gCost + hCost;

            public IEnumerable<(Node, int)> Neighbours 
            {
                get 
                {
                    foreach ((int x, int y, int cost) move in moves)
                    {
                        int neighbourX = x + move.x;
                        int neighbourY = y + move.y;
                        if (grid.IsValidArrayPos(neighbourX, neighbourY))
                        {
                            Node neighbour = grid[neighbourX, neighbourY];
                            if (neighbour.walkable)
                                yield return (neighbour, move.cost);
                        }
                    }
                }
            }

            public int CalculateHCost(int endX, int endY) 
            {
                int dx = Mathf.Abs(endX - x), dy = Mathf.Abs(endY - y);
                return Mathf.Min(dx, dy) * DIAG_COST + Mathf.Abs(dx - dy) * STRAIGHT_COST;
            }

            public override string ToString() =>  $"({x}, {y}), g: {gCost}, h: {hCost}";
        }
    }
}
