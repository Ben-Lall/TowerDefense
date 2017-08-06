using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// A path made of 2D Vectors that used to illustrate a pathway between two points.  Typically used for monster pathfinding, and generated using the A* search algorithm.
/// TODO: Take List of towers instead of Point target, and have each node recalculate its heuristic value to the new closest hub.  The current implementation works because there is only
/// 1 hub on the map, if there are more then the initial target could potentially be sub-par.
/// </summary>
namespace TowerDefense {

    class Pathfinder {

        /// <summary>
        /// Priority Queue
        /// </summary>
        private SortedSet<SearchNode>  pq;

        /// <summary>
        /// Search node that serves as the head of the path to the target
        /// </summary>
        public LinkedList<Tile> Path { get; set; }

        public Pathfinder(Point start, Point target, Tile[,] map) {
            // Initialize the priority queue
            pq = new SortedSet<SearchNode>(new AStarComparer());
            pq.Add(new SearchNode(map[start.Y, start.X], null, Globals.Distance(start, target)));

            // A* search
            while (!GoalTest(pq.First(), target)) {
                SearchNode s = pq.First();
                pq.Remove(s);
                AddAdjacencies(s, target, map);
            }

            GeneratePath(pq.First());
        }

        /// <summary>
        /// Generates a linked list path of tiles to follow from the current tile.
        /// </summary>
        /// <param name="tail">The current tail of a SearchNode trail.</param>
        private void GeneratePath(SearchNode tail) {
            Path = new LinkedList<Tile>();
            SearchNode current = tail;
            while(current.parent != null) {
                Path.AddFirst(current.tile);
                current = current.parent;
            }
        }

        /// <summary>
        /// Goal test for the A* search.  Returns true if the tile is adjacent or diagonal to the target tile.
        /// </summary>
        /// <param name="s">The search node to be tested.</param>
        /// <param name="target">The coordinates of the goal tile.</param>
        /// <returns></returns>
        private bool GoalTest(SearchNode s, Point target) {
            return Globals.Distance(s.tile.Pos, target) <= Globals.SQRT2;
        }

        /// <summary>
        /// Add to the given priority queue all valid search nodes adjacent to the given one.
        /// </summary>
        /// <param name="s">The "source" search node that all entered search nodes will be adjacent to.</param>
        /// <param name="target">The coordinates of the goal tile.</param>
        /// <param name="map">Game map.</param>
        private void AddAdjacencies(SearchNode s, Point target, Tile[,] map) {
            // Check and add the 6 tiles above and below the given tile
            for(int i = s.tile.X == 0 ? 0 : -1; i < (s.tile.X == Settings.MapWidth - 1 ? 1 : 2); i++) {
                if(s.tile.Y > 0) {
                    Tile t = map[s.tile.Y - 1, s.tile.X + i];
                    if (t.IsEmpty()) {
                        pq.Add(new SearchNode(t, s, Globals.Distance(t.Pos, target)));
                    }
                }
                if(s.tile.Y < Settings.MapHeight - 1) {
                    Tile t = map[s.tile.Y + 1, s.tile.X + i];
                    if (t.IsEmpty()) {
                        pq.Add(new SearchNode(t, s, Globals.Distance(t.Pos, target)));
                    }
                }
            }

            // Check and add the two tiles on either side of the given tile
            if(s.tile.X > 0) {
                Tile t = map[s.tile.Y, s.tile.X - 1];
                if(t.IsEmpty()) {
                    pq.Add(new SearchNode(t, s, Globals.Distance(t.Pos, target)));
                }
            }
            if (s.tile.X < Settings.MapWidth - 1) {
                Tile t = map[s.tile.Y, s.tile.X + 1];
                if (t.IsEmpty()) {
                    pq.Add(new SearchNode(t, s, Globals.Distance(t.Pos, target)));
                }
            }
        }
    }

    /// <summary>
    /// Search node for the A* search.  Each node represents a tile, but also stores priority.
    /// </summary>
    class SearchNode {
        /// <summary>
        /// Integer representing the priority of this node. g(x) + h(x), where:
        /// g(x) = total cost to travel to this node
        /// h(x) = heuristic cost to travel to the target node (Euclidean distance)
        /// </summary>
        internal double priority;

        /// <summary>
        /// The cost in to travel to this node.  In terms of the "real world", this is the total Euclidian distance.
        /// </summary>
        internal double cost;

        /// <summary>
        /// The tile on the world map that this node represents.
        /// </summary>
        internal Tile tile;

        /// <summary>
        /// The parent of this node.  In terms of the "real-world", this is the SearchNode representing the tile traversed directly before this one.
        /// </summary>
        internal SearchNode parent;

        /// <summary>
        /// Constructor for a new SearchNode.
        /// </summary>
        /// <param name="tile">The Tile this node represents</param>
        /// <param name="parent">The node preceding this one</param>
        /// <param name="h">The result of the heuristic function for this search</param>
        internal SearchNode(Tile tile, SearchNode parent, double h) {
            this.tile = tile;
            this.parent = parent;
            if (parent != null) {
                cost = parent.cost + Globals.Distance(parent.tile.Pos, tile.Pos);
            } else {
                cost = 0;
            }
            priority = cost + h;
        }
    }

    class AStarComparer : IComparer<SearchNode> {
        public int Compare(SearchNode s1, SearchNode s2) {
            if(s1.priority < s2.priority) {
                return -1;
            } else if(s1.priority > s2.priority) {
                return 1;
            } else { // s1.priority == s2.priority
                if(s1.cost < s2.cost) {
                    return -1;
                } else if(s1.cost > s2.cost) {
                    return 1;
                } else { // s1.cost == s2.cost
                    return 0;
                }
            }
        }
    }
}
