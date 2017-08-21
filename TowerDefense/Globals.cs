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
    /// Create a new map class for this along with a new map class!
    /// </summary>
    enum TileDrawMode { DEFAULT, HEATMAP, HEATMAP_NUMBERS, TOTAL_DRAW_MODES }

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
        public static SpriteBatch WorldSpriteBatch { get; set; }

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

        /// <summary>
        /// The world position of the mouse.
        /// </summary>
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
        public static List<GameplayObject> DrawSet { get; set; }

        /// <summary>
        /// List of effects currently playing on the screen.
        /// </summary>
        public static List<Bolt> Effects { get; set; }

        /// <summary>
        /// List of players.
        /// </summary>
        public static List<Player> Players { get; set; }

        /// <summary>
        /// The current tile drawing mode.
        /// </summary>
        public static TileDrawMode TileMode { get; set; }

        /// <summary>
        /// Whether or not the grid should be overlayed to the screen.
        /// </summary>
        public static bool GridToggle { get; set; }

        /* Game World */

        /// <summary>
        /// The player being controlled by the client.
        /// </summary>
        public static Player ActivePlayer { get; set; }

        /* Gameplay */

        /// <summary>
        /// Boolean representing if the game is currently Paused.
        /// </summary>
        public static bool Paused { get; set; }

        /// <summary>
        /// Boolean representing the presence of the map overlay.
        /// </summary>
        public static bool MapOverlayToggle { get; set; }

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
        /// The amount of time between monster spawns.  Measured in units of seconds.
        /// </summary>
        public static double SpawnRate { get; set; }

        /// <summary>
        /// The amount of time remaining before a new monster spawnin  Measured in units of seconds.
        /// </summary>
        public static double SpawnCooldown { get; set; }

        /* UI */

        /// <summary>
        /// List of Buttons.
        /// </summary>
        public static List<Button> Buttons { get; set; }

        /// <summary>
        /// A list of UI Panels, sorted in descending order of depth.  Alias to the ActivePlayer's UI.
        /// </summary>
        public static List<UIPanel> UIPanels { get; set; }

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

            // Set the map dimensions
            MapWidth = 2000;
            MapHeight = 2000;

            //Initialize collections
            Towers = new List<Tower>();
            Monsters = new List<Monster>();
            DrawSet = new List<GameplayObject>();
            Buttons = new List<Button>();
            Effects = new List<Bolt>();
            Players = new List<Player>();
            UlTowers = new List<TowerTemplate>();
            UlTowers.Add(BoltTowerTemplate);

            // Initialize Input
            Input.PreviousMouseWheel = MouseState.ScrollWheelValue;

            // Initialize ActivePlayer
            ActivePlayer = new Player(new Point(((MapWidth / 2) - 1) * TileWidth, ((MapHeight / 2) - 1) * TileHeight));
            UIPanels = ActivePlayer.UIElements;
            Camera = new Camera2d(ActivePlayer.Pos.ToVector2(), ScreenWidth, ScreenHeight);
            DrawSet.Add(ActivePlayer);

            // Initialize gameplay stuff.
            SpawnRate = 6.0;
            SpawnCooldown = 0;
            HeatMap.Initialize();
            WorldMap.Initialize();
            TileMode = TileDrawMode.DEFAULT;
            Paused = true;
        }

        /** General Helper Methods **/

        public static bool TileContainsMonster(Tile t) {
            foreach(Monster m in Monsters) {
                if (m.CenterTile.Equals(t.Pos)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the ceiling of a given vector2
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 Ceil(Vector2 v) {
            return new Vector2((float)Math.Ceiling(v.X), (float)Math.Ceiling(v.Y));
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
                    if (WorldMap.At(x, y).ObstructsTower()) {
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
                    WorldMap.At(tower.TilePos.X + x, tower.TilePos.Y + y).ContainsTower = true;
                }
            }
            HeatMap.AddTower(tower);
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
            Point cursorTilePos = PixelToTile(WorldMousePos.ToPoint());
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
                return 0 < MouseState.X && MouseState.X < ScreenWidth && MouseState.Y > 0 && MouseState.Y < ScreenHeight;
            }
            return 0 < MouseState.X && MouseState.X < (ScreenWidth - MenuPanelWidth) && MouseState.Y > 0 && MouseState.Y < ScreenWidth;
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

        /// <summary>
        /// Starting from the start point, find the closest tower of the matching type, and return the distance from its closest tile.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="target"></param>
        /// <returns>The closest tile position of the target type, if any exist.  Returns 0 otherwise.</returns>
        public static double GetClosestTileDistance(Point start, TowerType targetType) {
            if (Towers.Count > 0 && Towers.Exists(t => t.Type == TowerType.HUB)) { 
                double distance = int.MaxValue;
                foreach (Tower t in Towers) {
                    if (t.Type == targetType) {
                        for (int y = 0; y < t.HeightTiles; y++) {
                            for (int x = 0; x < t.WidthTiles; x++) {
                                Point p = t.TilePos + new Point(x, y);
                                double currentDistance = Distance(start, p);
                                if (currentDistance <= distance) {
                                    distance = currentDistance;
                                }
                            }
                        }
                    }
                }

                return distance;
            }
            return 0;
        }

        /** Collision **/
        
        public static TowerTemplate BoltTowerTemplate { get => new TowerTemplate(TowerType.BOLT); }
        public static TowerTemplate HubTemplate { get => new TowerTemplate(TowerType.HUB); }

    }
}
