using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.Globals;
using System.Diagnostics;

namespace TowerDefense {
    /// <summary>
    /// A menu button.  Has a visual appearance, and executes an action when pressed.
    /// </summary>
    class Button {

        /// <summary>
        /// The rectangular bounds of this button.
        /// </summary>
        public Rectangle Bounds { get; set; }

        public int X { get => Bounds.X; set => Bounds = new Rectangle(value, Y, Width, Height); }
        public int Y { get => Bounds.Y; set => Bounds = new Rectangle(X, value, Width, Height); }
        public int Width { get => Bounds.Width; }
        public int Height { get => Bounds.Height; }

        /// <summary>
        /// Texture for the button background.
        /// </summary>
        public Texture2D Background { get; set; }

        /// <summary>
        /// The sprite filling the body of this button.
        /// </summary>
        public AnimatedSprite Contents { get; set; }

        /// <summary>
        /// The text filling the body of this button.
        /// </summary>
        public StringBuilder Text { get; set; }

        /// <summary>
        /// The action the button should perform upon being pressed.
        /// </summary>
        public Action OnClick;


        /// <summary>
        /// Create a new button.  May only have text or a sprite for its contents.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="background"></param>
        /// <param name="contents"></param>
        /// <param name="text"></param>
        /// <param name="action"></param>
        private void createNewRep(Rectangle bounds, Texture2D background, AnimatedSprite contents, String text, Action action) {
            Debug.Assert((contents == null || text == null), "A button cannot have both a string and an image for its contents!");
            Bounds = bounds;
            Background = background;
            Contents = contents;
            Text = text == null ? null : new StringBuilder(text);
            OnClick = action;
        }

        /// <summary>
        /// Create a button consisting only of a texture background.
        /// </summary>
        /// <param name="bounds">Rectangle representing the hitbox of this button.</param>
        /// <param name="background">Texture representing the background contents of this button.</param>
        /// <param name="action">Action referring to the function this button will call upon being pressed.</param>
        public Button(Rectangle bounds, Texture2D background, Action action) {
            createNewRep(bounds, background, null, null, action);
        }

        /// <summary>
        /// Create a new button with a second sprite filling its body.
        /// </summary>
        /// <param name="bounds">Rectangle representing the hitbox of this button.</param>
        /// <param name="background">Texture representing the background contents of this button.</param>
        /// <param name="contents">Texture representing the interior content of this button.</param>
        /// <param name="action">Action referring to the function this button will call upon being pressed.</param>
        public Button(Rectangle bounds, Texture2D background, AnimatedSprite contents, Action action) {
            createNewRep(bounds, background, contents, null, action);
        }

        /// <summary>
        /// Create a new button with text filling its body.
        /// </summary>
        /// <param name="shape">Rectangle representing the hitbox of this button.</param>
        /// <param name="background">Texture representing the background contents of this button.</param>
        /// <param name="text">Text filling the body of this button.</param>
        /// <param name="action">Action referring to the function this button will call upon being pressed.</param>
        public Button(Rectangle shape, Texture2D background, String text, Action action) {
            createNewRep(shape, background, null, text, action);
        }

        /// <summary>
        /// Create a new button from a string of text, so that the button will fit it.
        /// </summary>
        /// <param name="text">Text filling the body of this button.</param>
        /// <param name="centerPoint">The point this button should be centered around.</param>
        /// <param name="background">Texture representing the background contents of this button.</param>
        /// <param name="action">Action referring to the function this button will call upon being pressed.</param>
        public Button(String text, Vector2 centerPoint, Texture2D background, Action action) {
            const int BUFFER_SIZE = 4; // buffer space between the text and the edge of the button, measured in pixels.
            Vector2 ButtonSize = Art.Font.MeasureString(text) + new Vector2(BUFFER_SIZE * 2, BUFFER_SIZE * 2);
            Rectangle bounds = new Rectangle((centerPoint - ButtonSize / 2).ToPoint(), ButtonSize.ToPoint());
            createNewRep(bounds, background, null, text, action);
        }

        /// <summary>
        /// Draw this button using the given SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch) {
            Debug.Assert((Contents == null || Text == null), "A button cannot have both a string and an image for its contents!");
            // Draw the background of the button
            spriteBatch.Draw(Background, Bounds, Color.White);

            // Draw the contents of the button, if any
            if (Contents != null) {
                int contentsWidth = Bounds.Width / 3;
                int contentsHeight = (Bounds.Height * 2) / 3;
                int towerY = Bounds.Y + Bounds.Height / 2;
                int towerX = Bounds.X + Bounds.Width / 2;

                Contents.Draw(towerX, towerY, spriteBatch, contentsWidth, contentsHeight);
            } else if(Text != null) {
                Vector2 textSize = Art.Font.MeasureString(Text);
                // Resize text to fit this button, if necessary
                Vector2 Scale = new Vector2(1, 1);
                if(textSize.X > Bounds.Width) {
                    Scale.X = Bounds.Width / textSize.X;
                }
                if(textSize.Y > Bounds.Height) {
                    Scale.Y = Bounds.Height / textSize.Y;
                }

                Vector2 Pos = Bounds.Center.ToVector2() - (textSize / 2);
                spriteBatch.DrawString(Art.Font, Text, Pos, Color.Black, 0, Vector2.Zero, Scale, SpriteEffects.None, 1f);
            }
        }

        /// <summary>
        /// Check if the given point is contained within the bounds of this button.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains(Point p) {
            return Bounds.Contains(p);
        }
    }
}
