using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense {
    /// <summary>
    /// A class containing global variables and helper methods.
    /// </summary>
    static class Globals {
        /** Constants **/

        /// <summary>
        /// Array containing every monster in the game, indexed by the MonsterType enumerator.
        /// </summary>
        private static Monster[] monsterCatalog;

        /// <summary>
        /// List of effects currently playing on the screen.
        /// </summary>
        public static List<Bolt> effects;

        public static void InitializeGlobals() {
            MonsterCatalog = new Monster[(int)MonsterType.NUMBER_OF_MONSTERS];
            effects = new List<Bolt>();

        }

        /// <summary>
        /// Returns the Manhattan Distance between two points.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static int ManhattanDistance(Point p1, Point p2) {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }

        /// <summary>
        /// Get the Euclidean Distance between two points.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Distance(Point p1, Point p2) {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        /// <summary>
        /// Starting from the start point, find the closest tower of the matching type, and return its closest tile.
        /// Requires that the list of towers is non-empty, and is populated with least one tower of targetType.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Point GetClosestTilePos(Point start, TowerType targetType, List<Tower> towers) {
            Point closestCoord = new Point();
            double distance = Distance(start, towers.First().Pos);
            foreach(Tower t in towers) {
                if(t.Type == targetType) {
                    for(int y = 0; y < t.Height; y++) {
                        for(int x = 0; x < t.Width; x++) {
                            Point p = t.Pos + new Point(x, y);
                            double currentDistance = Distance(start, p);
                            if (currentDistance <= distance) {
                                distance = currentDistance;
                                closestCoord = p;
                            }
                        }
                    }
                }
            }

            return closestCoord;
        }

        /** Collision **/

        /// <summary>
        /// Check if the given monster intersects with the given tower's firing range.
        /// </summary>
        /// <param name="t">The Tower.</param>
        /// <param name="m">The Monster.</param>
        /// <returns>true if they intersect, false otherwise.</returns>
        public static bool Intersects(Tower t, Monster m) {
            // Tower range is interpreted as the ellipse ((x - t.CenterPoint.x)^2)/a^2 + ((y - t.CenterPoint.y)^2)/b^2 = 1
            // Where a = (t.PixelRadius.X)^2 and b = (t.PixelRadius.Y)^2
            // Check the 8 relevant points on a rectangle to see if it intersects.
            return (Math.Pow(m.X - t.CenterPoint.X, 2) / Math.Pow(t.PixelRadius.X, 2)) + (Math.Pow(m.Y - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRadius.Y, 2)) <= 1 ||
                   (Math.Pow(m.X + m.SpriteWidth / 2 - t.CenterPoint.X, 2) / Math.Pow(t.PixelRadius.X, 2)) + (Math.Pow(m.Y - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRadius.Y, 2)) <= 1 ||
                   (Math.Pow(m.X + m.SpriteWidth - t.CenterPoint.X, 2) / Math.Pow(t.PixelRadius.X, 2)) + (Math.Pow(m.Y - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRadius.Y, 2)) <= 1 ||
                   (Math.Pow(m.X - t.CenterPoint.X, 2) / Math.Pow(t.PixelRadius.X, 2)) + (Math.Pow(m.Y + m.SpriteHeight - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRadius.Y, 2)) <= 1 ||
                   (Math.Pow(m.X + m.SpriteWidth / 2 - t.CenterPoint.X, 2) / Math.Pow(t.PixelRadius.X, 2)) + (Math.Pow(m.Y + m.SpriteHeight - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRadius.Y, 2)) <= 1 ||
                   (Math.Pow(m.X + m.SpriteWidth - t.CenterPoint.X, 2) / Math.Pow(t.PixelRadius.X, 2)) + (Math.Pow(m.Y + m.SpriteHeight - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRadius.Y, 2)) <= 1 ||
                   (Math.Pow(m.X - t.CenterPoint.X, 2) / Math.Pow(t.PixelRadius.X, 2)) + (Math.Pow(m.Y + m.SpriteHeight / 2 - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRadius.Y, 2)) <= 1 ||
                   (Math.Pow(m.X + m.SpriteWidth - t.CenterPoint.X, 2) / Math.Pow(t.PixelRadius.X, 2)) + (Math.Pow(m.Y + m.SpriteHeight / 2 - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRadius.Y, 2)) <= 1;


        }


        internal static Monster[] MonsterCatalog { get => monsterCatalog; set => monsterCatalog = value; }
        internal static TowerTemplate BoltTowerTemplate { get => new TowerTemplate(TowerType.BOLT, Art.Tower); }
        internal static TowerTemplate HubTemplate { get => new TowerTemplate(TowerType.HUB, Art.Hub); }

        /** Constants **/
        public static double SQRT2 { get { return Math.Sqrt(2); } }
    }
}

