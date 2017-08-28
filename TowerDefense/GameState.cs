using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TowerDefense;
using static Include.Globals;


namespace Include {
    static class GameState {

        /* Input */

        /// <summary>
        /// The world position of the mouse.
        /// </summary>
        public static Vector2 WorldMousePos {
            get => Vector2.Transform(
            new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Matrix.Invert(ActivePlayer.Camera.Transform));
        }

        /* Graphics */

        /// <summary>
        /// Alias to the active player's camera.
        /// </summary>
        public static Camera2d Camera { get => ActivePlayer.Camera; }

        /// <summary>
        /// A set of towers / creatures, sorted by coordinate position, so as to be drawn in the correct order.
        /// </summary>
        public static List<GameplayObject> DrawSet { get; set; }

        /// <summary>
        /// Comparer used to sort objects by drawing order.
        /// </summary>
        public static readonly EnglishSort DrawComparer = new EnglishSort();

        /// <summary>
        /// Blendstate used to reduce additive blending on joints of lightning bolts.
        /// </summary>
        public static readonly BlendState MaxBlend = new BlendState() {
            AlphaBlendFunction = BlendFunction.Max,
            ColorBlendFunction = BlendFunction.Max,
            AlphaDestinationBlend = Blend.One,
            AlphaSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,
            ColorSourceBlend = Blend.One
        };

        /* Game World */

        /// <summary>
        /// The name of the currently loaded world.
        /// </summary>
        public static String WorldName { get; set; }

        /// <summary>
        /// Integer representing the width of the game map, measured in units of tiles.
        /// </summary>
        public static int MapWidth { get; set; }

        /// <summary>
        /// Integer representing the height of the game map, measured in units of tiles.
        /// </summary>
        public static int MapHeight { get; set; }

        /// <summary>
        /// The player being controlled by the client.
        /// </summary>
        public static Player ActivePlayer { get; set; }

        /// <summary>
        /// List of Towers currently on the game map.
        /// </summary>
        public static List<Tower> Towers { get; set; }

        /// <summary>
        /// List of monsters currently on the game map.
        /// </summary>
        public static List<Monster> Monsters { get; set; }

        /// <summary>
        /// List of players.
        /// </summary>
        public static List<Player> Players { get; set; }

        /// <summary>
        /// List of effects currently playing on the screen.
        /// </summary>
        public static List<Bolt> Effects { get; set; }

        /* Gameplay */

        /// <summary>
        /// Boolean representing if the game is currently Paused.
        /// </summary>
        public static bool Paused { get; set; }

        /// <summary>
        /// Whether the game is currently playing.
        /// </summary>
        public static bool Playing { get => CurrentGameState == GameStatus.Playing; }

        /// <summary>
        /// The amount of time between monster spawns.  Measured in units of seconds.
        /// </summary>
        public static double SpawnRate { get; set; }

        /// <summary>
        /// The amount of time remaining before a new monster spawnin  Measured in units of seconds.
        /// </summary>
        public static double SpawnCooldown { get; set; }

        /// <summary>
        /// Initialize a new gamestate.
        /// </summary>
        /// <param name="f">The file from which to load the gamestate.</param>
        public static void InitializeGameState(FileStream f, CancellationToken c) {
            CurrentGameState = GameStatus.Loading;
            if (c.IsCancellationRequested) { return; }
            // Initialize collections.
            Towers = new List<Tower>();
            Monsters = new List<Monster>();
            DrawSet = new List<GameplayObject>();
            Effects = new List<Bolt>();
            Players = new List<Player>();

            // Set world settings.
            SpawnRate = 6.0;
            SpawnCooldown = 0;


            if (c.IsCancellationRequested) { return; }
            // Load data from file.
            LoadProgress = 0;
            SaveManager.ReadHeader(f);
            HeatMap.Initialize();

            if (c.IsCancellationRequested) { return; }
            LoadText = "Loading World";
            WorldMap.LoadFromFile(f);

            if (c.IsCancellationRequested) { return; }
            LoadText = "Loading Towers";
            SaveManager.LoadTowers(f);

            if (c.IsCancellationRequested) { return; }
            CurrentGameState = GameStatus.Playing;
            
        }


        /// <summary>
        /// Save and exit the currently GameState.
        /// </summary>
        public static void SaveAndExit() {
            SaveManager.SaveMap(WorldName, MapWidth, MapHeight);
            ExitGame();
        }

        /// <summary>
        /// Close the current game, clearing all data involved.
        /// </summary>
        public static void ExitGame() {
            // Clear collections
            Monsters.Clear();
            Towers.Clear();
            DrawSet.Clear();
            Effects.Clear();
            Players.Clear();
            ActivePlayer = null;

            // Reset to title
            CurrentGameState = GameStatus.Title;
        }

        /// <summary>
        /// Handle a mouse click at the mouse's current world position.
        /// </summary>
        /// <param name="mouseState"></param>
        public static void HandleWorldClick() {
            Point mousePos = WorldMousePos.ToPoint();
            foreach(Tower t in Towers) {
                if(t.BoundingBox.Contains(mousePos)) {
                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift)) { // and shift was held
                        ActivePlayer.ToggleSelectedTower(t);
                    } else { // shift was not held
                        ActivePlayer.ClearSelectedTowers();
                        ActivePlayer.SelectedTowers.Add(t);
                    }
                    return;
                }
            }

            if (ActivePlayer.IsPlacingTower && CursorIsOnMap() && ValidTowerLocation()) {
                    PlacePendingTower();
            } else { // Actions that would deselect the selected towers on mouse click
                    ActivePlayer.ClearSelectedTowers();
            }
        }

        /// <summary>
        /// Assumes isPlacingTower = true, and CursorIsOnMap() = true.  Return true if the area the cursor is hovering over allows
        /// for enough space to place the currently selected tower. Returns false otherwise.
        /// </summary>
        /// <returns></returns>
        public static bool ValidTowerLocation() {
            //Return false if any of the tiles in the pending tower's selection area are obstructed, and true otherwise.
            Point pos = GetAreaStartPoint();
            for (int y = pos.Y; y < pos.Y + ActivePlayer.PendingTowerTemplate.Height; y++) {
                for (int x = pos.X; x < pos.X + ActivePlayer.PendingTowerTemplate.Width; x++) {
                    if (WorldMap.At(x, y).ObstructsTower()) {
                        return false;
                    }
                }
            }
            return true;
        }

        /** Update Helpers **/

        /// <summary>
        /// Begin tower placement.
        /// </summary>
        /// <param name="template">The template of the tower whose placement has begun.</param>
        public static void BeginTowerPlacement(TowerTemplate template) {
            ActivePlayer.PendingTowerTemplate = template;
        }

        /// <summary>
        /// Place the currently pending tower.
        /// </summary>
        public static void PlacePendingTower() {
            // TODO: Check for proper resources.
            Point pos = GetAreaStartPoint();
            // Place Tower
            Tower newTower = new Tower(ActivePlayer.PendingTowerTemplate, pos);
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

        /** Drawing Helpers **/

        /// <summary>
        /// Returns a Point representing the coordinates of the top-left tile of the area highlighted by the cursor, where the dimensions are
        /// the dimensions of the pending tower.
        /// Assumes isPlacingTower = true, and CursorIsOnMap() = true.
        /// </summary>
        /// <returns></returns>
        public static Point GetAreaStartPoint() {
            int width = ActivePlayer.PendingTowerTemplate.Width;
            int height = ActivePlayer.PendingTowerTemplate.Height;
            Point cursorTilePos = PixelToTile(WorldMousePos.ToPoint());
            int x = MathHelper.Clamp(cursorTilePos.X - width / 2, 0, MapWidth - width);
            int y = MathHelper.Clamp(cursorTilePos.Y - height / 2, 0, MapHeight - height);
            return new Point(x, y);
        }

        /// <summary>
        /// Return true if the cursor is on the map, false otherwise.
        /// Assumes CurrentGameState is Playing
        /// </summary>
        /// <returns></returns>
        public static bool CursorIsOnMap() {
            MouseState ms = Mouse.GetState();
            return 0 < ms.X && ms.X < ScreenWidth && ms.Y > 0 &&
                ms.Y < ScreenHeight &&
                ActivePlayer.UIElements.TrueForAll(x => !x.Contains(ms.Position));
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

    }
}
