using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.Globals;


namespace TowerDefense {
    static class HeatMap {
        /// <summary>
        /// Matrix of heatmaps.  First index refers to the monster that will use it.
        /// </summary>
        static int[, ,] HeatMaps;

        /// <summary>
        /// The maximum heat differential for the monster at its index.
        /// </summary>
        static int[] HeatMapMaxes;

        /// <summary>
        /// The color for a goal tile.
        /// </summary>
        static Color MaxColor = new Color(0, 174, 255);

        /// <summary>
        /// Amount of time in seconds between heatmap calculations
        /// </summary>
        public static double HeatMapTimer;

        /// <summary>
        /// Amount of time remaining before recalculating heatmap.
        /// </summary>
        public static double HeatMapCooldown;

        static int HeatMapWidth;
        static int HeatMapHeight;

        static int HeatMapPxWidth;
        static int HeatMapPxHeight;

        static int HeatTileWidth;
        static int HeatTileHeight;


        /// <summary>
        /// Initialize the heat map with blank values.
        /// </summary>
        public static void InitializeHeatMaps() {
            HeatMapPxWidth = MapWidth * TileWidth;
            HeatMapPxHeight = MapHeight * TileHeight;
            HeatTileWidth = TileWidth;
            HeatTileHeight = TileHeight;
            HeatMapWidth = MapWidth * TileWidth / HeatTileWidth;
            HeatMapHeight = MapHeight * TileHeight / HeatTileHeight;

            HeatMapTimer = 2;
            HeatMapCooldown = 0;

            HeatMaps = new int[(int)MonsterType.NUMBER_OF_MONSTERS, HeatMapHeight, HeatMapWidth];
            HeatMapMaxes = new int[(int)MonsterType.NUMBER_OF_MONSTERS];
            for(int i = 0; i < HeatMapMaxes.Count(); i++) {
                HeatMapMaxes[i] = 1;
            }
        }

        /// <summary>
        /// Update each heatmap
        /// </summary>
        public static void Update(GameTime gameTime) {
            if (HeatMapCooldown == 0) {
                ResetHeatMaps();
                for (int m = 0; m < (int)MonsterType.NUMBER_OF_MONSTERS; m++) {
                    GenerateHeatMap(m, SetGoals(m));
                }
                HeatMapCooldown = HeatMapTimer;
            } else {
                HeatMapCooldown = Math.Max(0, HeatMapCooldown - gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        /// <summary>
        /// Reset all heat maps to initial values.
        /// </summary>
        private static void ResetHeatMaps() {
            for (int k = 0; k < (int)MonsterType.NUMBER_OF_MONSTERS; k++) {
                for (int y = 0; y < HeatMapHeight; y++) {
                    for (int x = 0; x < HeatMapWidth; x++) {
                        HeatMaps[k, y, x] = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Set goal tiles depending on the heatMap, and return a Queue containing them.
        /// </summary>
        /// <param name="heatMap"></param>
        private static Queue<Point> SetGoals(int heatMap) {
            Queue<Point> q = new Queue<Point>();
            if (heatMap == (int)MonsterType.IMP) {
                for(int y = 0; y < HeatMapPxHeight; y += HeatTileHeight) {
                    for(int x = 0; x < HeatMapPxWidth; x += HeatTileWidth) {
                        if(GetClosestTileDistance(new Point(x / TileWidth, y / TileHeight), TowerType.HUB) <= SQRT2) {
                            HeatMaps[heatMap, y / HeatTileHeight, x / HeatTileWidth] = 0;
                            q.Enqueue(new Point(x / HeatTileWidth, y / HeatTileHeight));
                        }
                    }
                }
            }
            return q;
        }

        /// <summary>
        /// Generate the heatmap for the given map.  Requires that goal points are set as 0.
        /// </summary>
        /// <param name="heatMap">The map to generate.</param>
        /// <param name="q">A queue initialized with the goal points.</param>
        private static void GenerateHeatMap(int heatMap, Queue<Point> q) {
            // Begin a BFS initialized with all the goal points.
            int heat = 0;
            // Get divisors for translating HeatMap dimensions to TileMap dimensions.
            int divX = TileWidth / HeatTileWidth;
            int divY = TileHeight / HeatTileHeight;
            while(q.Count > 0) {
                Point current = q.Dequeue();
                heat = HeatMaps[heatMap, current.Y, current.X] + 1;

                // Apply heat to adjacent, unmarked tiles.
                if(current.Y > 0 && HeatMaps[heatMap, current.Y - 1, current.X] == -1 && MapAt(current.X / divX, (current.Y - 1) / divY).IsEmpty()) {
                    HeatMaps[heatMap, current.Y - 1, current.X] = heat;
                    q.Enqueue(new Point(current.X, current.Y - 1));
                }
                if (current.Y < HeatMapHeight - 1 && HeatMaps[heatMap, current.Y + 1, current.X] == -1 && MapAt(current.X / divX, (current.Y + 1) / divY).IsEmpty()) {
                    HeatMaps[heatMap, current.Y + 1, current.X] = heat;
                    q.Enqueue(new Point(current.X, current.Y + 1));
                }
                if (current.X > 0 && HeatMaps[heatMap, current.Y, current.X - 1] == -1 && MapAt((current.X - 1) / divX, current.Y / divY).IsEmpty()) {
                    HeatMaps[heatMap, current.Y, current.X - 1] = heat;
                    q.Enqueue(new Point(current.X - 1, current.Y));
                }
                if (current.X < HeatMapWidth - 1 && HeatMaps[heatMap, current.Y, current.X + 1] == -1 && MapAt((current.X + 1) / divX, current.Y / divY).IsEmpty()) {
                    HeatMaps[heatMap, current.Y, current.X + 1] = heat;
                    q.Enqueue(new Point(current.X + 1, current.Y));
                }
            }
            // Record the maximum difference in heat for this map
            HeatMapMaxes[heatMap] = heat - 1;
        }

        /// <summary>
        /// Overlay the chosen heatmap to the screen.
        /// </summary>
        /// <param name="heatMap"></param>
        public static void Draw(int heatMap) {
            int max = HeatMapMaxes[heatMap];
            if (max > 0) {
                for (int y = 0; y < HeatMapHeight; y += 1) {
                    for (int x = 0; x < HeatMapWidth; x += 1) {
                        Color color;
                        if (HeatMaps[heatMap, y, x] == -1) {
                            color = Color.Red;
                        } else {
                            int R = MaxColor.R - HeatMaps[heatMap, y, x] * MaxColor.R / max;
                            int G = MaxColor.G - HeatMaps[heatMap, y, x] * MaxColor.G / max;
                            int B = MaxColor.B - HeatMaps[heatMap, y, x] * MaxColor.B / max;
                            color = new Color(R, G, B);
                        }
                        WorldSpriteBatch.Draw(Art.Pixel, new Rectangle(x * HeatTileWidth, y * HeatTileHeight, HeatTileWidth, HeatTileHeight), color);
                    }
                }
            }
        }
    }
}
