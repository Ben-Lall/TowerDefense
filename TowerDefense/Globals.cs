using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense;

namespace Include {
    /// <summary>
    /// A class containing global variables and helper methods.
    /// </summary>
    static class Globals {
        /* System */

        /// <summary>
        /// Graphics device.
        /// </summary>
        public static GraphicsDeviceManager Graphics { get; set; }

        /// <summary>
        /// The game window.
        /// </summary>
        public static GameWindow Window { get; set; }

        /* SpriteBatches */

        /// <summary>
        /// SpriteBatch containing details of drawing creatures and map features.
        /// </summary>
        public static SpriteBatch WorldSpriteBatch{ get; set; }

        /// <summary>
        /// SpriteBatch containing details of drawing UI elements.
        /// </summary>
        public static SpriteBatch UISpriteBatch { get; set; }

        /// <summary>
        /// SpriteBatch containing details of Bolt Effects.
        /// </summary>
        public static SpriteBatch BoltSpriteBatch { get; set; }

        /* Input */

        /// <summary>
        /// The mouse's current state.
        /// </summary>
        public static MouseState MouseState { get; set; }

        public static Vector2 WorldMousePos { get => Vector2.Transform(
        new Vector2(MouseState.X, MouseState.Y), Matrix.Invert(Camera.Transform));
        }

        /* Graphics */

        /// <summary>
        /// Integer representing the width of the window, in pixels.
        /// </summary>
        public static int ScreenWidth { get => Window.ClientBounds.Width; }

        /// <summary>
        /// Integer representing the height of the window, in pixels.
        /// </summary>
        public static int ScreenHeight { get => Window.ClientBounds.Height; }

        /// <summary>
        /// Integer representing the width of all tiles.
        /// </summary>
        public static int TileWidth { get; set; }

        /// <summary>
        /// Integer representing the height of all tiles.
        /// </summary>
        public static int TileHeight { get; set; }

        /// <summary>
        /// Integer representing the width of the game map, measured in units of tiles.
        /// </summary>
        public static int MapWidth { get; set; }

        /// <summary>
        /// Integer representing the height of the game map, measured in units of tiles.
        /// </summary>
        public static int MapHeight { get; set; }

        /// <summary>
        /// Point representing the coordinates of the top-left corner of the viewport, measured in units of tiles.
        /// </summary>
        public static Camera2d Camera { get; set; }

        /// <summary>
        /// Integer representing the width of the viewport, measured in units of tiles.
        /// </summary>
        public static int ViewRows { get; set; }

        /// <summary>
        /// Integer representing the height of the viewport, measured in units of tiles.
        /// </summary>
        public static int ViewCols { get; set; }

        /// <summary>
        /// Integer representing the width of the viewport, measured in units of pixels.
        /// </summary>
        public static int ViewRowsPx { get => ViewRows * TileWidth; }

        /// <summary>
        /// Integer representing the height of the viewport, measured in units of pixels.
        /// </summary>
        public static int ViewColsPx { get => ViewCols * TileHeight; }

        /// <summary>
        /// The maximum possible Y value for the viewport.
        /// </summary>
        public static int MaxViewportY { get => MapHeight - ViewCols; }

        /// <summary>
        /// The maximum possible X value for the viewport.
        /// </summary>
        public static int MaxViewportX { get => MapWidth - ViewRows; }

        /// <summary>
        /// The dimensions of the viewport, measured in units of tiles.
        /// </summary>
        public static Point ViewPortDimensions { get => new Point(ViewRows, ViewCols); }

        /// <summary>
        /// The dimensions of the viewport, measured in units of pixels.
        /// </summary>
        public static Point ViewPortDimensionsPx { get => new Point(ViewRowsPx, ViewColsPx); }

        /// <summary>
        /// Width in pixels of the menu panel.
        /// </summary>
        public static int MenuPanelWidth { get; set; }

        /// <summary>
        /// Height in pixels of the menu panel.
        /// </summary>
        public static int MenuPanelHeight { get; set; }

        /// <summary>
        /// A set of towers / creatures, sorted by coordinate position, so as to be drawn in the correct order.
        /// </summary>
        public static List<Object> DrawSet { get; set; }

        /// <summary>
        /// List of effects currently playing on the screen.
        /// </summary>
        public static List<Bolt> Effects { get; set; }

        /* Game World */

        /// <summary>
        /// 2D array representing the game map.  Read by map[i.j], where i refers to the row, and j refers to the column.
        /// </summary>
        public static Tile[,] Map { get; set; }

        /* Gameplay */

        /// <summary>
        /// Boolean representing if the game is currently Paused.
        /// </summary>
        public static bool Paused { get; set; }

        /// <summary>
        /// List of templates of towers unlocked by the player
        /// </summary>
        public static List <TowerTemplate> UlTowers { get; set; }

        /// <summary>
        /// List of Towers currently on the game map.
        /// </summary>
        public static List<Tower> Towers { get; set; }

        /// <summary>
        /// List of monsters currently on the game map.
        /// </summary>
        public static List<Monster> Monsters { get; set; }

        /// <summary>
        /// Integer representing the current wave.  Always > 0.
        /// </summary>
        public static int CurrentWave { get; set; }

        /* UI */

        /// <summary>
        /// List of Buttons.
        /// </summary>
        public static List<Button> Buttons { get; set; }

        /// <summary>
        /// Boolean representing whether or not the player has selected a tower and is working on placing it.
        /// </summary>
        public static bool IsPlacingTower { get; set; }

        /// <summary>
        /// A Template of the tower whose placement is currently being deliberated, if any.
        /// </summary>
        public static TowerTemplate PendingTowerTemplate { get; set; }

        /// <summary>
        /// Comparer used to sort objects by drawing order.
        /// </summary>
        public static readonly EnglishSort DrawComparer = new EnglishSort();

        /// <summary>
        /// Blendstate used to reduce additive blending on joints of lightning bolts.
        /// </summary>
        public static readonly BlendState MaxBlend = new BlendState()
        {
            AlphaBlendFunction = BlendFunction.Max,
            ColorBlendFunction = BlendFunction.Max,
            AlphaDestinationBlend = Blend.One,
            AlphaSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,
            ColorSourceBlend = Blend.One
        };

        /** Constants **/
        public static double SQRT2 { get { return Math.Sqrt(2); } }
        
        public static void InitializeGlobals(GameWindow window) {

            // Set the screen resolution to be 16:9, as the game is largely balanced around this.
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();

            Window = window;

            // Set the menu panel's height and width.
            MenuPanelWidth = ScreenWidth / 8;
            MenuPanelHeight = ScreenHeight;

            // Set the tile dimensions to 16px.  16 is a common factor of 720 and 1120: 1120 = 1280 * (7/8).
            TileWidth = 16;
            TileHeight = 16;

            // Set the number of tiles viewable on the screen
            ViewRows = ScreenWidth / TileWidth;
            ViewCols = ScreenHeight / TileHeight;

            // Set the map dimensions
            MapWidth = Math.Max(ViewRows, 100);
            MapHeight = Math.Max(ViewCols, 100);

            // Set up camera
            Camera = new Camera2d(new Vector2(ScreenWidth / 2, ScreenHeight / 2));

            // Initialize the gameplay objects.
            Map = new Tile[MapWidth, MapHeight];

            //Initialize collections
            Towers = new List<Tower>();
            Monsters = new List<Monster>();
            DrawSet = new List<Object>();
            Buttons = new List<Button>();
            Effects = new List<Bolt>();
            UlTowers = new List<TowerTemplate>();
            UlTowers.Add(BoltTowerTemplate);

            // Initialize Input
            Input.PreviousMouseWheel = MouseState.ScrollWheelValue;
        }

        /** General Helper Methods **/

        /// <summary>
        /// Returns the tile at the given coordianates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>map[y, x]</returns>
        public static Tile MapAt(int x, int y) {
            return Map[y, x];
        }

        /// <summary>
        /// Assumes isPlacingTower = true, and CursorIsOnMap() = true.  Return true if the area the cursor is hovering over allows
        /// for enough space to place the currently selected tower. Returns false otherwise.
        /// </summary>
        /// <returns></returns>
        public static bool ValidTowerLocation() {
            //Return false if any of the tiles in the pending tower's selection area are obstructed, and true otherwise.
            Point pos = GetAreaStartPoint();
            for (int y = pos.Y; y < pos.Y + PendingTowerTemplate.Height; y++) {
                for (int x = pos.X; x < pos.X + PendingTowerTemplate.Width; x++) {
                    if (MapAt(x, y).ObstructsTower()) {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Get the tower at the given position.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>The tower at the given position, or null if there isn't one.</returns>
        public static Tower TowerAt(Point p) {
            foreach(Tower t in Towers) {
                if(t.ContainsTile(p)){
                    return t;
                }
            }
            return null;
        }

        /// <summary>
        /// Check if a tower of the given type is on the map.
        /// </summary>
        /// <param name="tt"></param>
        /// <returns></returns>
        public static bool TowerIsOnMap(TowerType tt) {
            foreach(Tower t in Towers) {
                if(t.Type == tt) {
                    return true;
                }
            }
            return false;
        }

        /** Update Helpers **/

        /// <summary>
        /// Begin tower placement.
        /// </summary>
        /// <param name="template">The template of the tower whose placement has begun.</param>
        public static void BeginTowerPlacement(TowerTemplate template) {
            IsPlacingTower = true;
            PendingTowerTemplate = template;
        }

        /// <summary>
        /// Place the currently pending tower.
        /// </summary>
        public static void PlacePendingTower() {
            // TODO: Check for proper resources.
            Point pos = GetAreaStartPoint();
            // Place Tower
            Tower newTower = new Tower(PendingTowerTemplate, pos);
            AddTower(newTower);
        }

        /// <summary>
        /// Adds the given tower to the game world.
        /// </summary>
        /// <param name="tower">The tower to be added.</param>
        public static void AddTower(Tower tower) {
            Towers.Add(tower);
            DrawSet.Add(tower);

            // Mark each of its tiles as TOWER
            for (int y = 0; y < tower.HeightTiles; y++) {
                for (int x = 0; x < tower.WidthTiles; x++) {
                    MapAt(tower.TilePos.X + x, tower.TilePos.Y + y).ContainsTower = true;
                }
            }
        }

        /// <summary>
        /// Adds the given monster to the game world.
        /// </summary>
        /// <param name="monster">The monster to be added.</param>
        public static void AddMonster(Monster monster) {
            Monsters.Add(monster);
            DrawSet.Add(monster);
        }

        /// <summary>
        /// Clears all tower illumination.
        /// </summary>
        public static void ClearTowerIllumination() {
            foreach (Tower t in Towers) {
                t.Selected = false;
            }
        }

        /** Drawing Helpers **/

        /// <summary>
        /// Returns a Point representing the coordinates of the top-left tile of the area highlighted by the cursor, where the dimensions are
        /// the dimensions of the pending tower.
        /// Assumes isPlacingTower = true, and CursorIsOnMap() = true.
        /// </summary>
        /// <returns></returns>
        public static Point GetAreaStartPoint() {
            int width = PendingTowerTemplate.Width;
            int height = PendingTowerTemplate.Height;
            Point cursorTilePos = PixelToClosestTile(WorldMousePos.ToPoint());
            int x = MathHelper.Clamp(cursorTilePos.X - width / 2, 0, MapWidth - width);
            int y = MathHelper.Clamp(cursorTilePos.Y - height / 2, 0, MapHeight - height);
            return new Point(x, y);
        }

        /// <summary>
        /// Return true if the cursor is on the map, false otherwise.
        /// </summary>
        /// <returns></returns>
        public static bool CursorIsOnMap() {
            if (IsPlacingTower) {
                return 0 < MouseState.X && MouseState.X < ScreenWidth && MouseState.Y > 0 && MouseState.Y < ViewColsPx;
            }
            return 0 < MouseState.X && MouseState.X < (ScreenWidth - MenuPanelWidth) && MouseState.Y > 0 && MouseState.Y < ViewColsPx;
        }

        /// <summary>
        /// Return the map coordinates of the given pixel.
        /// </summary>
        /// <param name="pixel">Point containing the coordinates of the pixel.</param>
        /// <returns></returns>
        public static Point PixelToTile(Point pixel) {
            return new Point(pixel.X / TileWidth, pixel.Y / TileHeight);
        }

        /// <summary>
        /// Return the map coordinates of the tile the given pixel is nearest to, rounded up.
        /// </summary>
        /// <param name="pixel">Point containing the coordinates of the pixel.</param>
        /// <returns></returns>
        public static Point PixelToClosestTile(Point pixel) {
            int x = pixel.X % TileWidth;
            int y = pixel.Y % TileHeight;

            return PixelToTile(pixel + new Point(x, y));
        }

        /** Pathing Helpers **/

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
        /// Requires that the list of towers is non-empty, and is populated with at least one tower of targetType.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="target"></param>
        /// <returns>The closest tile position of the target type, if any exist.  Returns start otherwise.</returns>
        public static Point GetClosestTilePos(Point start, TowerType targetType) {
            Point closestCoord = new Point();
            bool closestFound = false;
            double distance = int.MaxValue;
            foreach(Tower t in Towers) {
                if(t.Type == targetType) {
                    for(int y = 0; y < t.HeightTiles; y++) {
                        for(int x = 0; x < t.WidthTiles; x++) {
                            Point p = t.TilePos + new Point(x, y);
                            double currentDistance = Distance(start, p);
                            if (currentDistance <= distance) {
                                closestFound = true;
                                distance = currentDistance;
                                closestCoord = p;
                            }
                        }
                    }
                }
            }

            if(closestFound) {
                return closestCoord;
            } else {
                return start;
            }
            
        }

        /** Collision **/

        /// <summary>
        /// Check if the given monster intersects with the given tower's firing range.
        /// </summary>
        /// <param name="t">The tower.</param>
        /// <param name="m">The monster.</param>
        /// <returns>true if they intersect, false otherwise.</returns>
        public static bool Intersects(Tower t, Monster m) {
            // Tower range is interpreted as the ellipse ((x - t.CenterPoint.x)^2)/a^2 + ((y - t.CenterPoint.y)^2)/b^2 = 1
            // Where a = (t.PixelRange.X)^2 and b = (t.PixelRange.Y)^2
            // Check the 8 relevant points on a rectangle to see if it intersects.
            return (Math.Pow(m.X - t.CenterPoint.X, 2) / Math.Pow(t.PixelRange.X, 2)) + (Math.Pow(m.Y - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRange.Y, 2)) <= 1 ||
                   (Math.Pow(m.X + m.Width / 2 - t.CenterPoint.X, 2) / Math.Pow(t.PixelRange.X, 2)) + (Math.Pow(m.Y - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRange.Y, 2)) <= 1 ||
                   (Math.Pow(m.X + m.Width - t.CenterPoint.X, 2) / Math.Pow(t.PixelRange.X, 2)) + (Math.Pow(m.Y - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRange.Y, 2)) <= 1 ||
                   (Math.Pow(m.X - t.CenterPoint.X, 2) / Math.Pow(t.PixelRange.X, 2)) + (Math.Pow(m.Y + m.Height - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRange.Y, 2)) <= 1 ||
                   (Math.Pow(m.X + m.Width / 2 - t.CenterPoint.X, 2) / Math.Pow(t.PixelRange.X, 2)) + (Math.Pow(m.Y + m.Height - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRange.Y, 2)) <= 1 ||
                   (Math.Pow(m.X + m.Width - t.CenterPoint.X, 2) / Math.Pow(t.PixelRange.X, 2)) + (Math.Pow(m.Y + m.Height - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRange.Y, 2)) <= 1 ||
                   (Math.Pow(m.X - t.CenterPoint.X, 2) / Math.Pow(t.PixelRange.X, 2)) + (Math.Pow(m.Y + m.Height / 2 - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRange.Y, 2)) <= 1 ||
                   (Math.Pow(m.X + m.Width - t.CenterPoint.X, 2) / Math.Pow(t.PixelRange.X, 2)) + (Math.Pow(m.Y + m.Height / 2 - t.CenterPoint.Y, 2) / Math.Pow(t.PixelRange.Y, 2)) <= 1;


        }
        
        public static TowerTemplate BoltTowerTemplate { get => new TowerTemplate(TowerType.BOLT); }
        public static TowerTemplate HubTemplate { get => new TowerTemplate(TowerType.HUB); }

    }
}
