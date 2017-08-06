using static Include.Globals;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// A path made of 2D Vectors that used to illustrate a pathway between two points.  Typically used for monster pathfinding, and generated using the A* search algorithm.
/// TODO: Take List of towers instead of Point target, and have each node recalculate its hueristic value to the new closest hub.  The current implementation works because there is only
/// 1 hub on the map, if there are more then the initial target could potentially be sub-par.
/// </summary>
namespace TowerDefense {

    class Pathfinder {
        /// <summary>
        /// Amount of iterations before giving up on this monster.
        /// </summary>
        private int SearchCutoff = 20000;

        /// <summary>
        /// Priority Queue
        /// </summary>
        private SortedSet<SearchNode>  pq;

        /// <summary>
        /// Search node that serves as the head of the path to the target
        /// </summary>
        public LinkedList<Tile> Path { get; set; }

        public Pathfinder(Point start) {
            // Initialize the priority queue
            pq = new SortedSet<SearchNode>(new AStarComparer());
            pq.Add(new SearchNode(MapAt(start.X, start.Y), null));

            // A* search
            int c = 0;
            while (c < SearchCutoff && !GoalTest(pq.First())) {
                c++;
                SearchNode s = pq.First();
                pq.Remove(s);
                AddAdjacencies(s);
            }

            if (c < SearchCutoff) {
                GeneratePath(pq.First());
            } else {
                Path = new LinkedList<Tile>();
                Path.AddFirst(MapAt(start.X, start.Y));
            }
        }

        /// <summary>
        /// Generates a linked list path of tiles to follow from the current tile.
        /// </summary>
        /// <param name="tail">The current tail of a SearchNode trail.</param>
        private void GeneratePath(SearchNode tail) {
            Path = new LinkedList<Tile>();
            SearchNode current = tail;
            while(current.Parent != null) {
                Path.AddFirst(current.Tile);
                current = current.Parent;
            }
        }

        /// <summary>
        /// Goal test for the A* search.  Returns true if the tile is adjacent or diagonal to the target tile.
        /// </summary>
        /// <param name="s">The search node to be tested.</param>
        /// <param name="target">The coordinates of the goal tile.</param>
        /// <returns></returns>
        private bool GoalTest(SearchNode s) {
            return Distance(s.Tile.Pos, GetClosestTilePos(s.Tile.Pos, TowerType.HUB)) <= SQRT2;
        }

        /// <summary>
        /// Add to the given priority queue all valid search nodes adjacent to the given one.
        /// </summary>
        /// <param name="s">The "source" search node that all entered search nodes will be adjacent to.</param>
        /// <param name="target">The coordinates of the goal tile.</param>
        /// <param name="map">Game map.</param>
        private void AddAdjacencies(SearchNode s) {
            bool tr = false, tl = false, bl = false, br = false;
            // Check and add the four tiles adjacent to this search node, while adding diagonals if properly linked.
            if (s.Tile.X > 0) {
                Tile t = MapAt(s.Tile.X - 1, s.Tile.Y);
                if(t.IsEmpty()) {
                    pq.Add(new SearchNode(t, s));
                    // Check and add bottom left and top left
                    if (s.Tile.Y > 0) {
                        tl = true;
                        t = MapAt(s.Tile.X - 1, s.Tile.Y - 1);
                        if (t.IsEmpty()) {
                            pq.Add(new SearchNode(t, s));
                        }
                    }
                    if (s.Tile.Y < MapHeight - 1) {
                        bl = true;
                        t = MapAt(s.Tile.X - 1, s.Tile.Y + 1);
                        if (t.IsEmpty()) {
                            pq.Add(new SearchNode(t, s));
                        }
                    }
                }
            }
            if (s.Tile.X < MapWidth - 1) {
                Tile t = MapAt(s.Tile.X + 1, s.Tile.Y);
                if (t.IsEmpty()) {
                    pq.Add(new SearchNode(t, s));
                    // Check and add bottom right and top right
                    if (s.Tile.Y > 0) {
                        tr = true;
                        t = MapAt(s.Tile.X + 1, s.Tile.Y - 1);
                        if (t.IsEmpty()) {
                            pq.Add(new SearchNode(t, s));
                        }
                    }
                    if (s.Tile.Y < MapHeight - 1) {
                        br = true;
                        t = MapAt(s.Tile.X + 1, s.Tile.Y + 1);
                        if (t.IsEmpty()) {
                            pq.Add(new SearchNode(t, s));
                        }
                    }
                }
            }
            if (s.Tile.Y > 0) {
                Tile t = MapAt(s.Tile.X, s.Tile.Y - 1);
                if (t.IsEmpty()) {
                    pq.Add(new SearchNode(t, s));
                    // Check and add top left and top right
                    if (s.Tile.X > 0 && !tl) {
                        t = MapAt(s.Tile.X - 1, s.Tile.Y - 1);
                        if (t.IsEmpty()) {
                            pq.Add(new SearchNode(t, s));
                        }
                    }
                    if (s.Tile.X < MapWidth - 1 && !tr) {
                        t = MapAt(s.Tile.X + 1, s.Tile.Y - 1);
                        if (t.IsEmpty()) {
                            pq.Add(new SearchNode(t, s));
                        }
                    }
                }
            }
            if (s.Tile.Y < MapHeight - 1) {
                Tile t = MapAt(s.Tile.X, s.Tile.Y + 1);
                if (t.IsEmpty()) {
                    pq.Add(new SearchNode(t, s));
                    // Check and add bottom left and bottom right
                    if (s.Tile.X > 0 && !bl) {
                        t = MapAt(s.Tile.X - 1, s.Tile.Y + 1);
                        if (t.IsEmpty()) {
                            pq.Add(new SearchNode(t, s));
                        }
                    }
                    if (s.Tile.X < MapWidth - 1 && !br) {
                        t = MapAt(s.Tile.X + 1, s.Tile.Y + 1);
                        if (t.IsEmpty()) {
                            pq.Add(new SearchNode(t, s));
                        }
                    }
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
        internal double Priority;

        /// <summary>
        /// The cost in to travel to this node.  In terms of the "real world", this is the total Euclidian distance.
        /// </summary>
        internal double Cost;

        /// <summary>
        /// The tile on the world map that this node represents.
        /// </summary>
        internal Tile Tile;

        /// <summary>
        /// The parent of this node.  In terms of the "real-world", this is the SearchNode representing the tile traversed directly before this one.
        /// </summary>
        internal SearchNode Parent;

        /// <summary>
        /// Constructor for a new SearchNode.
        /// </summary>
        /// <param name="tile">The Tile this node represents</param>
        /// <param name="parent">The node preceding this one</param>
        internal SearchNode(Tile tile, SearchNode parent) {
            Tile = tile;
            Parent = parent;
            if (parent != null) {
                Cost = parent.Cost + Distance(parent.Tile.Pos, tile.Pos);
            } else {
                Cost = 0;
            }
            Priority = Cost + Distance(Tile.Pos, GetClosestTilePos(Tile.Pos, TowerType.HUB));
        }
    }

    class AStarComparer : IComparer<SearchNode> {
        public int Compare(SearchNode s1, SearchNode s2) {
            if(s1.Priority < s2.Priority) {
                return -1;
            } else if(s1.Priority > s2.Priority) {
                return 1;
            } else { // s1.Priority == s2.Priority
                if (s1.Cost < s2.Cost) {
                    return -1;
                } else if(s1.Cost > s2.Cost) {
                    return 1;
                } else { // s1.Cost == s2.Cost
                    return 0;
                }
            }
        }
    }
}
