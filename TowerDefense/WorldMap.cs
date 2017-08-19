using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.Globals;


namespace TowerDefense {
    /// <summary>
    /// A Matrix of Tiles representing the world map.
    /// </summary>
    static class WorldMap {
        private static Tile[,] Map;

        /** AutoMap **/
        static Point AutoMapSize { get; set; }
        static Point AutoMapTileSize { get; set; }
        static Camera2d AutoMapCamera;

        static List<Button> AutoMapButtons { get; set; }

        /// <summary>
        /// Initialize the world map.
        /// </summary>
        public static void Initialize() {
            Map = new Tile[MapWidth, MapHeight];
            GenerateMap();
            // Automap settings
            AutoMapTileSize = new Point(16, 16);
            AutoMapSize = new Point(ScreenWidth / AutoMapTileSize.X, ScreenHeight / AutoMapTileSize.Y);
            AutoMapCamera = new Camera2d(ActivePlayer.Pos.ToVector2(), ScreenWidth * TileWidth / AutoMapTileSize.X, ScreenHeight * TileHeight / AutoMapTileSize.Y);
            AutoMapButtons = new List<Button>();

            // Create UI elements
            Point recenterPos = new Point(ScreenWidth - Art.RecenterButton.Width - 5, 5);
            AutoMapButtons.Add(new Button(new Rectangle(recenterPos, new Point(Art.RecenterButton.Width, Art.RecenterButton.Height)), Art.RecenterButton, () => AutoMapCamera.MoveTo(ActivePlayer.Pos.ToVector2())));
        }

        /// <summary>
        /// Get the tile at the given point.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Tile At(Point p) {
            Debug.Assert(p.X >= 0 && p.Y >= 0 && p.X < MapWidth && p.Y < MapHeight, "Violation of: p is within bounds.");
            return Map[p.Y, p.X];
        }

        /// <summary>
        /// Get the tile at the given coordinates.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Tile At(int x, int y) {
            Debug.Assert(x >= 0 && y >= 0 && x < MapWidth && y < MapHeight, "Violation of: (x, y) is within bounds.");
            return Map[y, x];
        }

        /// <summary>
        /// Generate a new world map.
        /// </summary>
        static void GenerateMap() {
            Random r = new Random();
            // Fill in the game map with open tiles.
            for (int y = 0; y < MapHeight; y++) {
                for (int x = 0; x < MapWidth; x++) {
                    int ID = 183;
                    double roll = r.NextDouble();
                    if (roll > 0.90) {
                        ID = 181;
                    } else if (roll > 0.75) {
                        ID = 182;
                    }
                    Map[y, x] = new Tile(TileType.OPEN, x, y, ID);
                }
            }
            AddTower(new Tower(HubTemplate, new Point(MapWidth / 2, MapHeight / 2)));
        }

        /// <summary>
        /// Draw the viewable portion of the map to the screen.
        /// </summary>
        public static void Draw() {
            for (int y = Camera.CameraTileStart.Y; y <= Camera.CameraTileEnd.Y; y++) {
                for (int x = Camera.CameraTileStart.X; x <= Camera.CameraTileEnd.X; x++) {
                    At(x, y).Draw(Color.White);
                }
            }
        }

        /// <summary>
        /// Overlay the AutoMap to the screen.
        /// </summary>
        public static void DrawAutoMap() {
            // Draw ground tiles to the auto map.
            for (int y = 0; y <= AutoMapSize.Y; y++) {
                for (int x = 0; x <= AutoMapSize.X; x++) {
                    Tile repTile = At(AutoMapCamera.CameraTileStart.X + x, AutoMapCamera.CameraTileStart.Y + y);
                    UISpriteBatch.Draw(Art.Pixel, new Rectangle(x * AutoMapTileSize.X, y * AutoMapTileSize.Y, AutoMapTileSize.X, AutoMapTileSize.Y), Art.PrevalentColors[repTile.SpriteId]);
                }
            }
            // Draw gameplay objects
            Rectangle AutoMapRegion = new Rectangle(AutoMapCamera.CameraStart, AutoMapCamera.CameraEnd);
            foreach(Tower t in Towers) {
                Point towerEnd = t.Pos + new Point(t.WidthTiles * TileWidth, t.HeightTiles * TileHeight);
                Rectangle towerRegion = new Rectangle(t.Pos, towerEnd);
                if(AutoMapRegion.Intersects(towerRegion)) {
                    for(int y = t.TilePos.Y; y < t.TilePos.Y + t.HeightTiles; y+= 1) {
                        for(int x = t.TilePos.X; x < t.TilePos.X + t.WidthTiles; x+= 1) {
                            if(y >= AutoMapCamera.CameraTileStart.Y && x >= AutoMapCamera.CameraTileStart.X) {
                                Point drawTile = new Point(x, y) - AutoMapCamera.CameraTileStart;
                                UISpriteBatch.Draw(Art.Pixel, new Rectangle(drawTile.X * AutoMapTileSize.X, drawTile.Y * AutoMapTileSize.Y, AutoMapTileSize.X, AutoMapTileSize.Y), Color.Gray);
                            }
                        }
                    }
                }
            }
            // Draw players last, with the active player absolutely last.
            Point drawPos = ActivePlayer.CenterTile - AutoMapCamera.CameraTileStart;
            UISpriteBatch.Draw(Art.Pixel, new Rectangle(drawPos.X * AutoMapTileSize.X, drawPos.Y * AutoMapTileSize.Y, AutoMapTileSize.X, AutoMapTileSize.Y), Color.Blue);

            // Draw Automap UI elements
            DrawAutoMapUI();
        }

        /// <summary>
        /// Draw AutoMap UI elements.
        /// </summary>
        private static void DrawAutoMapUI() {
            //Draw buttons
            foreach(Button b in AutoMapButtons) {
                b.Draw(UISpriteBatch);
            }
        }

        /// <summary>
        /// Toggle the interactivity of the map UI.
        /// </summary>
        public static void ToggleMapUI() {
            // Either let the list of currently interactable buttons be equal to this map's buttons, or clear the list of buttons
            // and allow the new UI to repopulate it.
            Buttons.Clear();
            if (MapOverlayToggle) {
                Buttons.AddRange(AutoMapButtons);
            }
        }

        /// <summary>
        /// Pan the camera in the given direction.
        /// </summary>
        /// <param name="direction">Vector giving the direction to pan the camera in.</param>
        public static void PanCamera(Vector2 direction) {
            AutoMapCamera.Move(new Vector2((float)Math.Round(direction.X) * AutoMapTileSize.X / 4, (float)Math.Round(direction.Y) * AutoMapTileSize.Y / 4));
        }

        /// <summary>
        /// Spawn a new wave of monsters.
        /// </summary>
        public static void SpawnWave() {
            Random r = new Random();
            int spawnAmt = r.Next(10, 15);

            // Spawn each enemy at a random tile.
            for (int i = 0; i < spawnAmt; i++) {
                int x = -1;
                int y = -1;
                if (r.NextDouble() <= 0.5) {
                    if (r.NextDouble() <= 0.5) {
                        x = r.Next(Camera.SpawnLeftStart.X, Camera.SpawnLeftEnd.X);
                    }
                    if (x < 0) {
                        x = r.Next(Camera.SpawnRightStart.X, Camera.SpawnRightEnd.X);
                    }
                    if (x >= MapWidth * TileWidth) {
                        x = r.Next(Camera.SpawnLeftStart.X, Camera.SpawnLeftEnd.X);
                    }
                    y = r.Next(Camera.CameraStart.Y, Camera.CameraEnd.Y);
                } else {
                    if (r.NextDouble() <= 0.5) {
                        y = r.Next(Camera.SpawnLeftStart.Y, Camera.SpawnLeftEnd.Y);
                    }
                    if (y < 0) {
                        y = r.Next(Camera.SpawnRightStart.Y, Camera.SpawnRightEnd.Y);
                    }
                    if (y >= MapHeight * TileHeight) {
                        y = r.Next(Camera.SpawnLeftStart.Y, Camera.SpawnLeftEnd.Y);
                    }
                    x = r.Next(Camera.CameraStart.X, Camera.CameraEnd.X);
                }
                Debug.Assert(y > 0 && x > 0);
                if (At(x / TileWidth, y / TileHeight).IsEmpty()) {
                    AddMonster(new Monster(new CreatureSprite(Art.Imp), MonsterType.IMP, new Point(x, y)));
                }
            }
        }
    }
}
