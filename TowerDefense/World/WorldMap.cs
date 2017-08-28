using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Include.GameState;
using static Include.Globals;



namespace TowerDefense {
    /// <summary>
    /// Types of world geography.
    /// </summary>
    enum GeoType { None, Field, Swamp, Desert, Tundra, Cave, NumberOfGeoTypes};

    /// <summary>
    /// Draw modes for the world map.
    /// </summary>
    enum TileDrawMode { Default, HeatMap, HeatMapNumbers, TotalDrawModes }

    /// <summary>
    /// A Matrix of Tiles representing the world map.
    /// </summary>
    static class WorldMap {
        /// <summary>
        /// The representation of the map, as a 2d array of Tiles.
        /// </summary>
        private static Tile[,] Map;

        /// <summary>
        /// Gives a random valid GeoType.
        /// </summary>
        public static GeoType RandomGeoType {get => (GeoType)(new Random().Next(1, (int)(GeoType.NumberOfGeoTypes - 1))); }

        /// <summary>
        /// Get the tile at the given point.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Tile At(Point p) {
            return Map[p.Y, p.X];
        }

        /// <summary>
        /// Get the tile at the given coordinates.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Tile At(int x, int y) {
            return Map[y, x];
        }

        /// <summary>
        /// Generate a new world map, and save it with the given name.
        /// </summary>
        /// <param name="name">The name of the world.</param>
        /// <param name="width">The width of the world, in units of tiles.</param>
        /// <param name="height">The height of the world, in units of tiles.</param>
        /// <param name="c">Cancellation token for async operation.</param>
        public static void GenerateMap(String name, int width, int height, CancellationToken c) {
            CurrentGameState = GameStatus.Loading;
            Map = new Tile[height, width];
            Random r = new Random();

            // Create a voronoi diagram to determine biome areas.
            GeoType[,] voronoi = new GeoType[height, width];

            if (c.IsCancellationRequested) { return; }

            // Create a set of seeds.
            Point[] seeds = new Point[(int)GeoType.NumberOfGeoTypes];
            seeds[(int)GeoType.Field] = new Point(width / 2, height / 2);
            seeds[(int)GeoType.Swamp] = new Point(3 * width / 4, 3 * height / 4);
            seeds[(int)GeoType.Desert] = new Point(width / 4, height / 4);
            seeds[(int)GeoType.Tundra] = new Point(width / 4, 3 * height / 4);
            seeds[(int)GeoType.Cave] = new Point(3 * width / 4, height / 4);


            // Initialize voronoi
            LoadText = "Building biomes boundaries";
            LoadProgress = 0;
            for (int y = 0; y < height; y++) {
                if (c.IsCancellationRequested) { return; }

                for (int x = 0; x < width; x++) {
                    voronoi[y, x] = GetClosestSeed(new Point(x, y), seeds);
                }
                LoadProgress += (1.0f / height) / 2;
            }

            // Set tiles.
            if (!c.IsCancellationRequested) {
                LoadText = "Building Tilemap";
                for (int y = 0; y < height; y++) {
                    if (c.IsCancellationRequested) { return; }

                    for (int x = 0; x < width; x++) {
                        int ID = GetSpriteIDFromGeoType(voronoi[y, x], r);
                        Map[y, x] = new Tile(TileType.Open, x, y, voronoi[y, x], ID);
                    }
                    LoadProgress += (1.0f / height) / 2;
                }
            }

            if (!c.IsCancellationRequested) {
                LoadText = "Saving map";
                SaveManager.SaveMap(name, width, height);

                CurrentGameState = GameStatus.Title;
                LoadText = null;
                LoadProgress = 0;
                DoneGenerating = true;
            }
        }

        /// <summary>
        /// Load the world map from the given filestream.
        /// </summary>
        /// <param name="f"></param>
        public static void LoadFromFile(FileStream f) {
            float operationWorth = 0.9f;
            Map = new Tile[MapHeight, MapWidth];

            byte[] bytes = new byte[Tile.TileDataSize];
            for (int y = 0; y < MapHeight; y++) {
                for (int x = 0; x < MapWidth; x++) {
                    f.Read(bytes, 0, Tile.TileDataSize);
                    Map[y, x] = Tile.LoadFromByteArray(x, y, bytes);
                }
                LoadProgress += operationWorth * ( 1.0f / MapHeight);
            }
        }

        /// <summary>
        /// Given a geography type, return a random sprite ID for that type.
        /// </summary>
        /// <param name="gt">Geography type.</param>
        /// <param name="r">Random number generator.  Passed in to reduce this function's overhead.</param>
        /// <returns></returns>
        public static int GetSpriteIDFromGeoType(GeoType gt, Random r) {
            double rand = r.NextDouble();
            int offset = 0;

            switch(gt) {
                case GeoType.Field:
                    if (rand >= 0.90) {
                        offset = 2;
                    } else if (rand >= 0.75) {
                        offset = 1;
                    }
                    return Art.FieldStartIndex + offset;
                case GeoType.Swamp:
                    if (rand >= 0.98) {
                        offset = 4;
                    } else if (rand >= 0.92) {
                        offset = 2;
                    } else if (rand >= 0.70 ) {
                        offset = 1;
                    }
                    return Art.SwampStartIndex + offset;
                case GeoType.Desert:
                    if (rand >= 0.90) {
                        offset = 2;
                    } else if (rand >= 0.75) {
                        offset = 1;
                    }
                    return Art.DesertStartIndex + offset;
                case GeoType.Cave:
                    if (rand >= 0.98) {
                        offset = 2;
                    } else if (rand >= 0.80) {
                        offset = 1;
                    }
                    return Art.CaveStartIndex + offset;
                case GeoType.Tundra:
                    if (rand >= 0.90) {
                        offset = 2;
                    } else if (rand >= 0.75) {
                        offset = 1;
                    }
                    return Art.TundraStartIndex + offset;
                default:
                    Debug.Fail("Error: Tried to find sprite for unsupported geography type.");
                    return 0;
            }
        }

        /// <summary>
        /// Find the seed closest to the given coordinate and return its geography type.
        /// </summary>
        /// <param name="p">The point being investigated.</param>
        /// <param name="seeds">Array of seeds, where each seed's GeoType is represented by its index.</param>
        /// <returns></returns>
        static GeoType GetClosestSeed(Point p, Point[] seeds) {
            double closestDistance = int.MaxValue;
            int geoType = 0;
            for(int i = 1; i < seeds.Count(); i++) {
                double d = Distance(p, seeds[i]);
                if (d < closestDistance) {
                    closestDistance = d;
                    geoType = i;
                }
            }

            Debug.Assert(geoType != 0, "Could not find valid geography type for tile at " + p.ToString());
            return (GeoType)geoType;
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
                    AddMonster(new Monster(new CreatureSprite(Art.Imp), MonsterType.Imp, new Point(x, y)));
                }
            }
        }
    }
}
