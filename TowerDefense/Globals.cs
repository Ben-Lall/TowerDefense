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

        /** Tower Templates **/
        private static TowerTemplate boltTowerTemplate;
        private static TowerTemplate hubTemplate;

        /** Textures **/
        /** Fonts **/
        private static SpriteFont font;

        /* UI Textures */
        private static Texture2D menuPanelTex;
        private static Texture2D pixel;

        /* Tower Textures */
        private static Texture2D towerTex;
        private static Texture2D towerButtonTex;
        private static Texture2D hubTex;

        /* Monster Textures */
        private static Texture2D impTex;

        public static void InitializeGlobals(GraphicsDevice graphics) {
            MonsterCatalog = new Monster[(int)MonsterType.NUMBER_OF_MONSTERS];
            Pixel = new Texture2D(graphics, 1, 1);
            Pixel.SetData<Color>(new Color[] { Color.White });
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
            // Check that the extremities of the circle are outside of the boundaries of the rectangle.
            return !((t.Pos.X - t.FireRadius) * Settings.TileWidth > m.X + m.SpriteWidth ||
                   (t.Pos.X + t.FireRadius) * Settings.TileWidth < m.X ||
                   (t.Pos.Y - t.FireRadius) * Settings.TileWidth > m.Y + m.SpriteHeight ||
                   (t.Pos.Y + t.FireRadius) * Settings.TileWidth < m.Y);
        }


        internal static Monster[] MonsterCatalog { get => monsterCatalog; set => monsterCatalog = value; }
        internal static TowerTemplate BoltTowerTemplate { get => new TowerTemplate(TowerType.BOLT, TowerTex); }
        internal static TowerTemplate HubTemplate { get => new TowerTemplate(TowerType.HUB, HubTex); }
        public static SpriteFont Font { get => font; set => font = value; }
        public static Texture2D MenuPanel { get => menuPanelTex; set => menuPanelTex = value; }
        public static Texture2D HubTex { get => hubTex; set => hubTex = value; }
        public static Texture2D TowerTex { get => towerTex; set => towerTex = value; }
        public static Texture2D TowerButton { get => towerButtonTex; set => towerButtonTex = value; }
        public static Texture2D Pixel { get => pixel; set => pixel = value; }
        public static Texture2D ImpTex { get => impTex; set => impTex = value; }

        /** Constants **/
        public static double SQRT2 { get { return Math.Sqrt(2); } }
    }
}

