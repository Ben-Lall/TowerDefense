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
        /// The Field
        /// </summary>
        static int[,] Field;

        /// <summary>
        /// The maximum heat differential.
        /// </summary>
        static int FieldMax;

        /// <summary>
        /// The color for a goal tile.
        /// </summary>
        static Color MaxColor = new Color(0, 174, 255);

        static int FieldWidth;
        static int FieldHeight;

        static int FieldPxWidth;
        static int FieldPxHeight;

        static int HeatTileWidth;
        static int HeatTileHeight;

        /// <summary>
        /// Scaling factor for converting world tiles to heat tiles over x.
        /// </summary>
        static int DivX { get => TileWidth / HeatTileWidth; }

        /// <summary>
        /// Scaling factor for converting world tiles to heat tiles over y.
        /// </summary>
        static int DivY { get => TileHeight / HeatTileHeight; }



        /// <summary>
        /// Initialize the heat map with blank values.
        /// </summary>
        public static void Initialize() {
            FieldPxWidth = MapWidth * TileWidth;
            FieldPxHeight = MapHeight * TileHeight;
            HeatTileWidth = TileWidth;
            HeatTileHeight = TileHeight;
            FieldWidth = MapWidth * TileWidth / HeatTileWidth;
            FieldHeight = MapHeight * TileHeight / HeatTileHeight;

            Field = new int[FieldHeight, FieldWidth];
            FieldMax = 0;
        }

        /// <summary>
        /// Update each heatmap
        /// </summary>
        public static void Update() {
            ResetFields();
            GenerateField(SetGoals());
        }

        /// <summary>
        /// Reset all heat maps to initial values.
        /// </summary>
        private static void ResetFields() {
            for (int y = 0; y < FieldHeight; y++) {
                for (int x = 0; x < FieldWidth; x++) {
                    Field[y, x] = -1;
                }
            }
        }

        /// <summary>
        /// Set goal tiles depending on the heatMap, and return a Queue containing them.
        /// </summary>
        private static Queue<Point> SetGoals() {
            Queue<Point> q = new Queue<Point>();
            foreach(Tower t in Towers) {
                if(t.Type == TowerType.HUB) {
                    for(int y = t.TileY; y < t.TileY + t.HeightTiles; y++) {
                        for(int x = t.TileX; x < t.TileX + t.WidthTiles; x++) {
                            for (int hy = 0; hy < TileHeight; hy += HeatTileHeight) {
                                for (int hx = 0; hx < TileWidth; hx += HeatTileWidth) {
                                    Field[y * DivY + hy, x * DivX + hx] = 0;
                                    q.Enqueue(new Point(x * DivX + hx, y * DivY + hy));
                                }
                            }
                        }
                    }
                }
            }
            return q;
        }

        /// <summary>
        /// Generate the heatmap for the given map.  Requires that goal points are set as 0.
        /// </summary>
        /// <param name="q">A queue initialized with the goal points.</param>
        private static void GenerateField(Queue<Point> q) {
            // Begin a BFS initialized with all the goal points.
            int heat = 0;
            while(q.Count > 0) {
                Point current = q.Dequeue();
                heat = Field[current.Y, current.X] + 1;

                // Apply heat to adjacent, unmarked tiles.
                if(current.Y > 0 && Field[current.Y - 1, current.X] == -1 && MapAt(current.X / DivX, (current.Y - 1) / DivY).IsEmpty()) {
                    Field[current.Y - 1, current.X] = heat;
                    q.Enqueue(new Point(current.X, current.Y - 1));
                }
                if (current.Y < FieldHeight - 1 && Field[current.Y + 1, current.X] == -1 && MapAt(current.X / DivX, (current.Y + 1) / DivY).IsEmpty()) {
                    Field[current.Y + 1, current.X] = heat;
                    q.Enqueue(new Point(current.X, current.Y + 1));
                }
                if (current.X > 0 && Field[current.Y, current.X - 1] == -1 && MapAt((current.X - 1) / DivX, current.Y / DivY).IsEmpty()) {
                    Field[current.Y, current.X - 1] = heat;
                    q.Enqueue(new Point(current.X - 1, current.Y));
                }
                if (current.X < FieldWidth - 1 && Field[current.Y, current.X + 1] == -1 && MapAt((current.X + 1) / DivX, current.Y / DivY).IsEmpty()) {
                    Field[current.Y, current.X + 1] = heat;
                    q.Enqueue(new Point(current.X + 1, current.Y));
                }
            }
            // Record the maximum difference in heat for this map
            FieldMax = heat - 1;
        }

        /// <summary>
        /// Overlay the heatmap to the screen.
        /// </summary>
        public static void Draw() {
            int max = FieldMax;
            if (max > 0) {
                for (int y = 0; y < FieldHeight; y += 1) {
                    for (int x = 0; x < FieldWidth; x += 1) {
                        Color color;
                        if (Field[y, x] == -1) {
                            color = Color.Red;
                        } else {
                            int R = MaxColor.R - Field[y, x] * MaxColor.R / max;
                            int G = MaxColor.G - Field[y, x] * MaxColor.G / max;
                            int B = MaxColor.B - Field[y, x] * MaxColor.B / max;
                            color = new Color(R, G, B);
                        }
                        WorldSpriteBatch.Draw(Art.Pixel, new Rectangle(x * HeatTileWidth, y * HeatTileHeight, HeatTileWidth, HeatTileHeight), color);

                        if(Field[y, x] != -1) {
                            String heatText = Field[y, x].ToString();
                            Vector2 heatTextSize = Art.Font.MeasureString(heatText);
                            WorldSpriteBatch.DrawString(Art.Font, heatText, new Vector2((x + 0.5f) * HeatTileWidth, (y + 0.5f) * HeatTileHeight) - heatTextSize / 2, Color.Black);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the heat map value of the given pixel point.
        /// </summary>
        /// <param name="p"></param>
        public static int HMapAt(Point p) {
            int x = Math.Min(p.X / HeatTileWidth, FieldWidth - 1);
            int y = Math.Min(p.Y / HeatTileHeight, FieldHeight - 1);
            return Field[y, x];
        }
    }
}
