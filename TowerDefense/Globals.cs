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
        /* System */

        /// <summary>
        /// Graphics device.
        /// </summary>
        GraphicsDeviceManager graphics;

        /// <summary>
        /// SpriteBatch.
        /// </summary>
        SpriteBatch spriteBatch;

        /// <summary>
        /// A set of towers / creatures, sorted by coordinate position, so as to be drawn in the correct order.
        /// </summary>
        List<Object> drawSet;

        /* Input */

        /// <summary>
        /// The mouse's current state.
        /// </summary>
        MouseState mouseState;

        /// <summary>
        /// Toggle boolean for the pause button.
        /// </summary>
        bool pausePressed;

        /// <summary>
        /// Toggle boolean for the mouse button.
        /// </summary>
        bool mousePressed;

        /// <summary>
        /// Toggle boolean for back button.
        /// </summary>
        bool backPressed;

        /* Graphics */

        /// <summary>
        /// Integer representing the width of the window, in pixels.
        /// </summary>
        int screenWidth;

        /// <summary>
        /// Integer representing the height of the window, in pixels.
        /// </summary>
        int screenHeight;

        /// <summary>
        /// Width in pixels of the menu panel.
        /// </summary>
        int menuPanelWidth;

        /// <summary>
        /// Height in pixels of the menu panel.
        /// </summary>
        int menuPanelHeight;

        /* Game World */

        /// <summary>
        /// 2D array representing the game map.  Read by map[i.j], where i refers to the row, and j refers to the column.
        /// </summary>
        Tile[,] map;

        /* Gameplay */

        /// <summary>
        /// Boolean representing if the game is currently paused.
        /// </summary>
        bool paused;

        /// <summary>
        /// List of templates of towers unlocked by the player
        /// </summary>
        List<TowerTemplate> ulTowers;

        /// <summary>
        /// List of Towers currently on the game map.
        /// </summary>
        List<Tower> towers;

        /// <summary>
        /// The hub.
        /// </summary>
        Tower hub;

        /// <summary>
        /// List of monsters currently on the game map.
        /// </summary>
        List<Monster> monsters;

        /// <summary>
        /// Integer representing the current wave.  Always > 0.
        /// </summary>
        int currentWave;

        /* UI */

        /// <summary>
        /// List of rectangles, where each rectangle represents the hit area of a menu button.
        /// </summary>
        List<Button> buttons;

        /// <summary>
        /// Boolean representing whether or not the player has selected a tower and is working on placing it.
        /// </summary>
        bool isPlacingTower;

        /// <summary>
        /// A Template of the tower whose placement is currently being deliberated, if any.
        /// </summary>
        TowerTemplate pendingTowerTemplate;

        /// <summary>
        /// Comparer used to sort objects by drawing order.
        /// </summary>
        DrawComparer drawComparer = new DrawComparer();

        /// <summary>
        /// Blendstate used to reduce additive blending on joints of lightning bolts.
        /// </summary>
        private static readonly BlendState maxBlend = new BlendState()
        {
            AlphaBlendFunction = BlendFunction.Max,
            ColorBlendFunction = BlendFunction.Max,
            AlphaDestinationBlend = Blend.One,
            AlphaSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,
            ColorSourceBlend = Blend.One
        };

        



        /// <summary>
        /// Array containing every monster in the game, indexed by the MonsterType enumerator.
        /// </summary>
        public static Monster[] MonsterCatalog { get; set; }

        /// <summary>
        /// List of effects currently playing on the screen.
        /// </summary>
        public static List<Bolt> Effects { get; set; }

        /// <summary>
        /// Point representing the coordinates of the top-left corner of the viewport, measured in units of tiles.
        /// </summary>
        public static Point Viewport { get; set; }

        /// <summary>
        /// Point representing the coordinates of the top-left corner of the viewport, measured in units of pixels.
        /// </summary>
        public static Point ViewportPx { get => new Point(Viewport.X * Settings.TileWidth, Viewport.Y * Settings.TileHeight); }

        public static void InitializeGlobals() {
            MonsterCatalog = new Monster[(int)MonsterType.NUMBER_OF_MONSTERS];
            Effects = new List<Bolt>();

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
        
        internal static TowerTemplate BoltTowerTemplate { get => new TowerTemplate(TowerType.BOLT, Art.Tower); }
        internal static TowerTemplate HubTemplate { get => new TowerTemplate(TowerType.HUB, Art.Hub); }

        /** Constants **/
        public static double SQRT2 { get { return Math.Sqrt(2); } }
    }
}
