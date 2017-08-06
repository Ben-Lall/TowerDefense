using static Include.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TowerDefense {
    /// <summary>
    /// A class dedicated to drawing simple and complex shapes.
    /// </summary>
    static class Graphics {
        /// <summary>
        /// Draw a line.
        /// </summary>
        /// <param name="x">Starting x position of this line</param>
        /// <param name="y">Starting y position of this line</param>
        /// <param name="width">Δx from the start to the end of this line.</param>
        /// <param name="height">Δy from start to the end of this line.</param>
        /// <param name="color">The color of this line.</param>
        public static void DrawLine(int x, int y, int width, int height, Color color) {
            Sprites.Draw(Art.Pixel, new Rectangle(x, y, width, height), color);
        }

        /// <summary>
        /// Draws a health bar given the max and current health values as well as a bounding rectangle.
        /// </summary>
        /// <param name="healthFraction">Fraction of health that is remaining</param>
        /// <param name="rectangle">A bounding rectangle in which the health bar will be rendered.</param>
        public static void DrawHealthBar(double healthFraction, Rectangle rectangle) {
            // Render black border around the health bar
            Sprites.Draw(Art.Pixel, rectangle, Color.Black);

            // Width of the health bar that will be filled
            int filledWidth = (int)Math.Ceiling((rectangle.Width - 2) * healthFraction);
            Rectangle filledBar = new Rectangle(rectangle.X + 1, rectangle.Y + 1, filledWidth, rectangle.Height - 2);

            // Render filled section of health bar
            Sprites.Draw(Art.Pixel, filledBar, Color.White);

            // Width of the health bar that is missing
            int missingWidth = rectangle.Width - filledWidth - 2;
            if (missingWidth != 0) {
                Rectangle missingBar = new Rectangle(rectangle.X + filledWidth + 1, rectangle.Y + 1, missingWidth, rectangle.Height - 2);
                // Render missing section of health bar
                Sprites.Draw(Art.Pixel, missingBar, Color.Red);
            }
        }

        /// <summary>
        /// Draw a circle centered on (x, y) with a radius of r.
        /// </summary>
        /// <param name="x">Center x coordinate.</param>
        /// <param name="y">Center y coordinate.</param>
        /// <param name="r">Circle radius.</param>
        public static void DrawCircle(int x, int y, int r) {
            for (int i = x - r; i <= x + r; i++) {
                int fx = (int)Math.Sqrt((r * r) - Math.Pow(i - x, 2));
                int y1 = y + fx;
                int y2 = y - fx;
                Sprites.Draw(Art.Pixel, new Rectangle(i, y1, 1, 1), Color.Blue);
                Sprites.Draw(Art.Pixel, new Rectangle(i, y2, 1, 1), Color.Blue);
            }
        }

        /// <summary>
        /// Draw a circle centered on p with a radius of r.
        /// </summary>
        /// <param name="p">Center coordinate.</param>
        /// <param name="r">Circle radius.</param>
        public static void DrawCircle(Point p, int r) {
            DrawCircle(p.X, p.Y, r);
        }

    }

    public class Line {
        public Vector2 A;
        public Vector2 B;
        public float Thickness;

        public Line(Vector2 a, Vector2 b, float thickness = 1) {
            A = a;
            B = b;
            Thickness = thickness;
        }

        public void Draw(Color color) {
            Vector2 tangent = B - A;
            float rotation = (float)Math.Atan2(tangent.Y, tangent.X);

            float ImageThickness = Art.BoltThickness;
            float thicknessScale = Thickness / ImageThickness;

            Vector2 capOrigin = new Vector2(Art.BoltCap.Width, Art.BoltCap.Height / 2f);
            Vector2 middleOrigin = new Vector2(0, Art.BoltLine.Height / 2f);
            Vector2 middleScale = new Vector2(tangent.Length(), thicknessScale);

            Sprites.Draw(Art.BoltLine, A - ViewportPx.ToVector2(), null, color, rotation, middleOrigin, middleScale, SpriteEffects.None, 0f);
            Sprites.Draw(Art.BoltCap, A - ViewportPx.ToVector2(), null, color, rotation, capOrigin, thicknessScale, SpriteEffects.None, 0f);
            Sprites.Draw(Art.BoltCap, B - ViewportPx.ToVector2(), null, color, rotation + MathHelper.Pi, capOrigin, thicknessScale, SpriteEffects.None, 0f);
        }

    }

    class Bolt {
        public List<Line> Segments = new List<Line>();

        public float Alpha { get; set; }
        public float FadeOutRate { get; set; }
        public float FadeOutTime { get; set; }
        public Color Tint { get; set; }

        public bool IsComplete { get { return Alpha <= 0; } }

        public Bolt(Vector2 source, Vector2 dest) : this(source, dest, new Color(0.9f, 0.8f, 1f), 0.5f) { }

        public Bolt(Vector2 source, Vector2 dest, Color color, float fadeOutTime) {
            Segments = CreateBolt(source, dest, 2);

            Tint = color;
            Alpha = 1f;
            FadeOutTime = fadeOutTime;
            FadeOutRate = ((float)((1.0 - (0.15 * fadeOutTime)) / (0.85 * fadeOutTime))) / 100;
        }

        public void Draw(SpriteBatch spriteBatch) {
            if (Alpha <= 0)
                return;

            foreach (var segment in Segments)
                segment.Draw(Tint * (Alpha * 0.6f));
        }

        public virtual void Update(GameTime gameTime) {
            // This update formula will allow the lightning bolt to "flash" more vibrantly for the first 15% of its lifetime, 
            // before fizzling out fast enough to disappear at the end.
            if (Alpha > 0.85f) {
                Alpha -= 0.01f * (gameTime.ElapsedGameTime.Milliseconds / 10);
            } else {
                Alpha -= FadeOutRate * (gameTime.ElapsedGameTime.Milliseconds / 10);
            }
        }

        protected static List<Line> CreateBolt(Vector2 source, Vector2 dest, float thickness) {
            var results = new List<Line>();
            Random Rand = new Random();
            Vector2 tangent = dest - source;
            Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
            float length = tangent.Length();

            List<float> positions = new List<float>();
            positions.Add(0);

            for (int i = 0; i < length / 4; i++)
                positions.Add((float)Rand.NextDouble());

            positions.Sort();

            const int Sway = 80;
            const float Jaggedness = 1.0f / Sway;

            Vector2 prevPoint = source;
            float prevDisplacement = 0;
            for (int i = 1; i < positions.Count; i++) {
                float pos = positions[i];

                // used to prevent sharp angles by ensuring very close positions also have small perpendicular variation.
                float scale = (length * Jaggedness) * (pos - positions[i - 1]);

                // defines an envelope. Points near the middle of the bolt can be further from the central line.
                float envelope = pos > 0.95f ? 20 * (1 - pos) : 1;

                float displacement = Rand.Next(-Sway, Sway);
                displacement -= (displacement - prevDisplacement) * (1 - scale);
                displacement *= envelope;

                Vector2 point = source + pos * tangent + displacement * normal;
                results.Add(new Line(prevPoint, point, thickness));
                prevPoint = point;
                prevDisplacement = displacement;
            }

            results.Add(new Line(prevPoint, dest, thickness));

            return results;
        }
    }
}
