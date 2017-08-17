using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.Globals;


namespace TowerDefense {
    static class HeatMap {
        /// <summary>
        /// The Field
        /// </summary>
        static float[,] Field;

        /// <summary>
        /// The maximum heat differential.
        /// </summary>
        static float FieldMax { get => (float)SQRT2 * Player.SpawnUpperBound / TileWidth; }

        /// <summary>
        /// The color for a goal tile.
        /// </summary>
        static Color MaxColor = new Color(0, 174, 255);

        static int FieldWidth;
        static int FieldHeight;

        static int FieldPxWidth;
        static int FieldPxHeight;

        /// <summary>
        /// The width in units of pixels of a heatmap tile.
        /// </summary>
        public static int HeatTileWidth;
        /// <summary>
        /// The height in units of pixels of a heatmap tile.
        /// </summary>
        public static int HeatTileHeight;

        /// <summary>
        /// Scaling factor for converting world tiles to heat tiles over x.
        /// </summary>
        static int DivX { get => TileWidth / HeatTileWidth; }

        /// <summary>
        /// Scaling factor for converting world tiles to heat tiles over y.
        /// </summary>
        static int DivY { get => TileHeight / HeatTileHeight; }

        static float HeatIncrement { get => 1.0f / DivX; }

        /// <summary>
        /// Initialize the heat map with blank values.
        /// </summary>
        public static void Initialize() {
            HeatTileWidth = TileWidth / 2;
            HeatTileHeight = TileHeight / 2;
            FieldWidth = MapWidth * TileWidth / HeatTileWidth;
            FieldHeight = MapHeight * TileHeight / HeatTileHeight;
            FieldPxWidth = FieldWidth * HeatTileWidth;
            FieldPxHeight = FieldHeight * HeatTileHeight;

            Field = new float[FieldHeight, FieldWidth];
            ResetField();
        }

        public static void AddTower(Tower t) {
            // Set all cells associated with the base of this tower to 0 and enqueue them.
            Queue<Point> q = new Queue<Point>();
            for (int y = t.TileY; y < t.TileY + t.HeightTiles; y++) {
                for (int x = t.TileX; x < t.TileX + t.WidthTiles; x++) {
                    for (int hy = 0; hy < DivY; hy += 1) {
                        for (int hx = 0; hx < DivX; hx += 1) {
                            Field[y * DivY + hy, x * DivX + hx] = 0;
                            q.Enqueue(new Point(x * DivX + hx, y * DivY + hy));
                        }
                    }
                }
            }
            GenerateField(q);
        }

        /// <summary>
        /// Remove a tower's heat influence from the heatmap.
        /// </summary>
        /// <param name="t"></param>
        public static void RemoveTower(Tower t) {
            // Enqueue all cells associated with the base of this tower.
            Queue<Point> q = new Queue<Point>();
            for (int y = t.TileY; y < t.TileY + t.HeightTiles; y++) {
                for (int x = t.TileX; x < t.TileX + t.WidthTiles; x++) {
                    for (int hy = 0; hy < DivY; hy += 1) {
                        for (int hx = 0; hx < DivX; hx += 1) {
                            q.Enqueue(new Point(x * DivX + hx, y * DivY + hy));
                        }
                    }
                }
            }
            RemoveField(q);
        }

        /// <summary>
        /// Reset Field to initial values.
        /// </summary>
        private static void ResetField() {
            for (int y = 0; y < FieldHeight; y++) {
                for (int x = 0; x < FieldWidth; x++) {
                    Field[y, x] = FieldMax;
                }
            }
        }

        /// <summary>
        /// Generate the heatmap.  Requires that goal points are set.
        /// </summary>
        /// <param name="q">A queue initialized with the goal points.</param>
        private static void GenerateField(Queue<Point> q) {
            // Begin a BFS initialized with all the goal points.
            float heat = 0;
            while(q.Count > 0) {
                Point current = q.Dequeue();
                heat = Field[current.Y, current.X] + HeatIncrement;
                if (heat <= FieldMax ) { // Extend heatmap out to some number of tiles.
                    // Apply heat to adjacent tiles whose current heat is greater than the projected heat.
                    if (current.Y > 0 && Field[current.Y - 1, current.X] > heat && WorldMap.At(current.X / DivX, (current.Y - 1) / DivY).IsEmpty()) {
                        Field[current.Y - 1, current.X] = heat;
                        q.Enqueue(new Point(current.X, current.Y - 1));
                    }
                    if (current.Y < FieldHeight - 1 && Field[current.Y + 1, current.X] > heat && WorldMap.At(current.X / DivX, (current.Y + 1) / DivY).IsEmpty()) {
                        Field[current.Y + 1, current.X] = heat;
                        q.Enqueue(new Point(current.X, current.Y + 1));
                    }
                    if (current.X > 0 && Field[current.Y, current.X - 1] > heat && WorldMap.At((current.X - 1) / DivX, current.Y / DivY).IsEmpty()) {
                        Field[current.Y, current.X - 1] = heat;
                        q.Enqueue(new Point(current.X - 1, current.Y));
                    }
                    if (current.X < FieldWidth - 1 && Field[current.Y, current.X + 1] > heat && WorldMap.At((current.X + 1) / DivX, current.Y / DivY).IsEmpty()) {
                        Field[current.Y, current.X + 1] = heat;
                        q.Enqueue(new Point(current.X + 1, current.Y));
                    }
                }
            }
        }

        /// <summary>
        /// Remove a chunk of the heatmap starting at the points given by the queue, then fills the gap left behind.
        /// </summary>
        /// <param name="q">A queue initialized with the goal points.</param>
        private static void RemoveField(Queue<Point> q) {
            // Begin a BFS initialized with all the goal points.
            Queue<Point> generateQ = new Queue<Point>();
            float heat = 0;
            while (q.Count > 0) {
                Point current = q.Dequeue();
                heat = Field[current.Y, current.X];
                Field[current.Y, current.X] = FieldMax;
                // Enqueue adjacent tiles to be removed if their heat is higher than this one's.  Otherwise, enqueue it into generateQ.
                if (current.Y > 0) {
                    if (Field[current.Y - 1, current.X] > heat && Field[current.Y - 1, current.X] != FieldMax) {
                        q.Enqueue(new Point(current.X, current.Y - 1));
                    } else {
                        generateQ.Enqueue(new Point(current.X, current.Y - 1));
                    }
                }
                if (current.Y < FieldHeight - 1) {
                    if (Field[current.Y + 1, current.X] > heat && Field[current.Y + 1, current.X] != FieldMax) {
                        q.Enqueue(new Point(current.X, current.Y + 1));
                    } else {
                        generateQ.Enqueue(new Point(current.X, current.Y + 1));
                    }
                }
                if (current.X > 0) {
                    if (Field[current.Y, current.X - 1] > heat && Field[current.Y, current.X - 1] != FieldMax) {
                        q.Enqueue(new Point(current.X - 1, current.Y));
                    } else {
                        generateQ.Enqueue(new Point(current.X - 1, current.Y));
                    }
                }
                if (current.X < FieldWidth - 1) {
                    if (Field[current.Y, current.X + 1] > heat && Field[current.Y, current.X + 1] != FieldMax) {
                        q.Enqueue(new Point(current.X + 1, current.Y));
                    } else {
                        generateQ.Enqueue(new Point(current.X + 1, current.Y));
                    }
                }
            }
            // Generate a new field starting at the frontier left behind by this removal brushfire.
            GenerateField(generateQ);
        }

        /// <summary>
        /// Draw an approximation of the heatmap to the screen. Each regular tile will be filled as one color.
        /// </summary>
        /// <param name="numbers">Whether or not the numbers should be drawn.</param>
        public static void Draw(bool numbers=false) {
            if (FieldMax > 0) {
                for (int y = Camera.CameraHeatStart.Y; y <= Camera.CameraHeatEnd.Y; y += 1) {
                    for (int x = Camera.CameraHeatStart.X; x <= Camera.CameraHeatEnd.X; x += 1) {
                        DrawTile(x, y);
                        if (numbers) {
                            DrawTileValue(x, y);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw the HeatMap tile at the HeatMap coordinates specified.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void DrawTile(int x, int y) {
            Color color;
            if (Field[y, x] == -1) {
                color = Color.Red;
            } else {
                int R = (int)(MaxColor.R - Field[y, x] * MaxColor.R / FieldMax);
                int G = (int)(MaxColor.G - Field[y, x] * MaxColor.G / FieldMax);
                int B = (int)(MaxColor.B - Field[y, x] * MaxColor.B / FieldMax);
                color = new Color(R, G, B);
            }
            WorldSpriteBatch.Draw(Art.Pixel, new Rectangle(x * HeatTileWidth, y * HeatTileHeight, HeatTileWidth, HeatTileHeight), color);
        }

        /// <summary>
        /// Draw the value of the HeatMap tile at the HeatMap coordinates specified.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void DrawTileValue(int x, int y) {
            if (Field[y, x] != -1) {
                String heatText = Math.Ceiling(Field[y, x]).ToString();
                Vector2 heatTextSize = Art.Font.MeasureString(heatText);
                WorldSpriteBatch.DrawString(Art.Font, heatText, new Vector2((x + 0.5f) * HeatTileWidth, (y + 0.5f) * HeatTileHeight) - heatTextSize / 2, Color.Black);
            }
        }

        /// <summary>
        /// Get the heat map value of the given pixel point.
        /// </summary>
        /// <param name="p"></param>
        public static float HMapAt(Point p) {
            Debug.Assert(p.X >= 0 && p.Y >= 0, "p is within bounds of the map.");
            int x = Math.Min(p.X / HeatTileWidth, FieldWidth - 1);
            int y = Math.Min(p.Y / HeatTileHeight, FieldHeight - 1);
            return Field[y, x];
        }

        /// <summary>
        /// Update the heat map at the given pixel point
        /// </summary>
        /// <param name="p"></param>
        public static void UpdateHMapAt(Point p, float value) {
            int x = Math.Min(p.X / HeatTileWidth, FieldWidth - 1);
            int y = Math.Min(p.Y / HeatTileHeight, FieldHeight - 1);
            Field[y, x] = value;
        }

        /// <summary>
        /// Given a pixel point, return a direction vector.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Vector2 GetDirVector(Point p) {
            // Get field vector
            float left = HMapAt(new Point(Math.Max(0, p.X - HeatTileWidth), p.Y));
            float right = HMapAt(new Point(Math.Min(FieldPxWidth, p.X + HeatTileWidth), p.Y));
            float up = HMapAt(new Point(p.X, Math.Max(0, p.Y - HeatTileHeight)));
            float down = HMapAt(new Point(p.X, Math.Min(FieldPxHeight, p.Y + HeatTileHeight)));

            Vector2 dir = Vector2.Normalize(new Vector2(left - right, up - down));
            if (Double.IsNaN(dir.X))
                return Vector2.Zero;
            return dir;
        }

        /// <summary>
        /// Check if a monster with the given firing range has arrived if it is at Point p.
        /// </summary>
        /// <param name="p">The point of the monster.</param>
        /// <param name="range">The firing range of the monster.</param>
        /// <returns></returns>
        public static bool HasArrived(Point p, float range) {
            float val = HMapAt(p);
            // return true if on a tile closer than firing range, or on a tile that may be greater than this, but due to the discrete nature of the heatmap, is still valid.
            return val != -1 && range >= val;
        }
    }
}
