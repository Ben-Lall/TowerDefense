using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TowerDefense {
    static class Graphics {
        /// <summary>
        /// Draw a line.
        /// </summary>
        /// <param name="x">Starting x position of this line</param>
        /// <param name="y">Starting y position of this line</param>
        /// <param name="width">Δx from the start to the end of this line.</param>
        /// <param name="height">Δy from start to the end of this line.</param>
        /// <param name="color">The color of this line.</param>
        public static void DrawLine(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color) {
            spriteBatch.Draw(Globals.Pixel, new Rectangle(x, y, width, height), color);
        }

        /// <summary>
        /// Draw a circle centered on (x, y) with a radius of r.
        /// </summary>
        /// <param name="x">Center x coordinate.</param>
        /// <param name="y">Center y coordinate.</param>
        /// <param name="r">Circle radius.</param>
        public static void DrawCircle(SpriteBatch spriteBatch, int x, int y, int r) {
            for (int i = x - r; i <= x + r; i++) {
                int fx = (int)Math.Sqrt((r * r) - Math.Pow(i - x, 2));
                int y1 = y + fx;
                int y2 = y - fx;
                spriteBatch.Draw(Globals.Pixel, new Rectangle(i, y1, 1, 1), Color.Blue);
                spriteBatch.Draw(Globals.Pixel, new Rectangle(i, y2, 1, 1), Color.Blue);
            }
        }

        /// <summary>
        /// Draw a jagged bolt from the start point to the end point.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void DrawBolt(Point start, Point end) {
            int boltLength = Settings.TileHeight;
            Point boltEnd;

        }

        /// <summary>
        /// Draw a circle centered on p with a radius of r.
        /// </summary>
        /// <param name="p">Center coordinate.</param>
        /// <param name="r">Circle radius.</param>
        public static void DrawCircle(SpriteBatch spriteBatch, Point p, int r) {
            DrawCircle(spriteBatch, p.X, p.Y, r);
        }

    }



}
