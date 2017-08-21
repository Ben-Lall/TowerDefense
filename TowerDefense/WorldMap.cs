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


        /// <summary>
        /// Initialize the world map.
        /// </summary>
        public static void Initialize() {
            Map = new Tile[MapWidth, MapHeight];
            GenerateMap();
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
