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
        static float[,] Field;

        /// <summary>
        /// The maximum heat differential.
        /// </summary>
        static float FieldMax;

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



        /// <summary>
        /// Initialize the heat map with blank values.
        /// </summary>
        public static void Initialize() {
            FieldPxWidth = FieldWidth * HeatTileWidth;
            FieldPxHeight = FieldHeight * HeatTileHeight;
            HeatTileWidth = 8;
            HeatTileHeight = 8;
            FieldWidth = MapWidth * TileWidth / HeatTileWidth;
            FieldHeight = MapHeight * TileHeight / HeatTileHeight;

            Field = new float[FieldHeight, FieldWidth];
            FieldMax = 0;
        }

        /// <summary>
        /// Update the heatmap
        /// </summary>
        public static void Update() {
            ResetField();
            GenerateField(SetGoals());
        }

        /// <summary>
        /// Reset all heat maps to initial values.
        /// </summary>
        private static void ResetField() {
            for (int y = 0; y < FieldHeight; y++) {
                for (int x = 0; x < FieldWidth; x++) {
                    Field[y, x] = -1;
                }
            }
        }

        /// <summary>
        /// Set goal tiles, and return a Queue containing them.
        /// </summary>
        private static Queue<Point> SetGoals() {
            Queue<Point> q = new Queue<Point>();
            foreach(Tower t in Towers) {
                for(int y = t.TileY; y < t.TileY + t.HeightTiles; y++) {
                    for(int x = t.TileX; x < t.TileX + t.WidthTiles; x++) {
                        for (int hy = 0; hy < DivY; hy += 1) {
                            for (int hx = 0; hx < DivX; hx += 1) {
                                Field[y * DivY + hy, x * DivX + hx] = 0;
                                q.Enqueue(new Point(x * DivX + hx, y * DivY + hy));
                            }
                        }
                    }
                }
            }
            return q;
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
                heat = Field[current.Y, current.X] + (1.0f / DivX);
                if (heat <= DivX * Player.SpawnUpperBound / TileWidth ) {
                    // Apply heat to adjacent, unmarked tiles.
                    if (current.Y > 0 && Field[current.Y - 1, current.X] == -1 && MapAt(current.X / DivX, (current.Y - 1) / DivY).IsEmpty()) {
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
            }
            // Record the maximum difference in heat for this map
            FieldMax = heat - 1;
        }

        /// <summary>
        /// Draw the heatmap to the screen.
        /// </summary>
        public static void Draw() {
            float max = FieldMax;
            if (max > 0) {
                for (int y = Camera.CameraHeatStart.Y; y <= Camera.CameraHeatEnd.Y; y += 1) {
                    for (int x = Camera.CameraHeatStart.X; x <= Camera.CameraHeatEnd.X; x += 1) {
                        Color color;
                        if (Field[y, x] == -1) {
                            color = Color.Red;
                        } else {
                            int R = (int)(MaxColor.R - Field[y, x] * MaxColor.R / max);
                            int G = (int)(MaxColor.G - Field[y, x] * MaxColor.G / max);
                            int B = (int)(MaxColor.B - Field[y, x] * MaxColor.B / max);
                            color = new Color(R, G, B);
                        }
                        WorldSpriteBatch.Draw(Art.Pixel, new Rectangle(x * HeatTileWidth, y * HeatTileHeight, HeatTileWidth, HeatTileHeight), color);
                    }
                }
            }
        }

        /// <summary>
        /// Get the heat map value of the given pixel point.
        /// </summary>
        /// <param name="p"></param>
        public static float HMapAt(Point p) {
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
            float left = HMapAt(p - new Point(HeatTileWidth, 0));
            float right = HMapAt(p + new Point(HeatTileWidth, 0));
            float up = HMapAt(p - new Point(0, HeatTileHeight));
            float down = HMapAt(p + new Point(0, HeatTileHeight));
            if (left == -1) { left = Math.Max(HMapAt(p), right); }
            if (right == -1) { right = Math.Max(HMapAt(p), left); }
            if (up == -1) { up = Math.Max(HMapAt(p), down); }
            if (down == -1) { down = Math.Max(HMapAt(p), up); }

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
