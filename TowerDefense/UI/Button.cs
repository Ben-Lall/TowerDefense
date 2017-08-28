using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.GameState;
using System.Diagnostics;

namespace TowerDefense {
    /// <summary>
    /// A menu button.  Has a visual appearance, and executes an action when pressed.
    /// </summary>
    class Button {

        /// <summary>
        /// The size of this button.
        /// </summary>
        public Point Size { get; set; }

        public int Width { get => Size.X; }
        public int Height { get => Size.Y; }

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
        /// The color of the text of this button.
        /// </summary>
        public Color TextColor { get; set; }
        static Color DefaultTextColor { get => Color.Black; }

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
        private void CreateButton(Point size, Texture2D background, Action action) {
            Size = size;
            Background = background;
            OnClick = action;
        }

        /// <summary>
        /// Create a new button whose size is determined by the given String.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textColor"></param>
        /// <param name="background"></param>
        /// <param name="action"></param>
        private void CreateButtonFromText(String text, Color textColor, Texture2D background, Action action) {
            const int BUFFER_SIZE = 4; // buffer space between the text and the edge of the button, measured in pixels.
            Vector2 buttonSize = Art.Font.MeasureString(text) + new Vector2(BUFFER_SIZE * 2, BUFFER_SIZE * 2);
            CreateButton(buttonSize.ToPoint(), background, action);
            Text = new StringBuilder(text);
            TextColor = textColor;
        }

        /// <summary>
        /// Create a button consisting only of a texture background.
        /// </summary>
        /// <param name="size">The size of this button.</param>
        /// <param name="background">Texture representing the background contents of this button.</param>
        /// <param name="action">Action referring to the function this button will call upon being pressed.</param>
        public Button(Point size, Texture2D background, Action action) {
            CreateButton(size, background, action);
        }

        /// <summary>
        /// Create a new button with a second sprite filling its body.
        /// </summary>
        /// <param name="size">The size of this button.</param>
        /// <param name="background">Texture representing the background contents of this button.</param>
        /// <param name="contents">Texture representing the interior content of this button.</param>
        /// <param name="action">Action referring to the function this button will call upon being pressed.</param>
        public Button(Point size, Texture2D background, AnimatedSprite contents, Action action) {
            CreateButton(size, background, action);
            Contents = contents;
        }

        /// <summary>
        /// Create a new button with text filling its body.
        /// </summary>
        /// <param name="shape">Rectangle representing the hitbox of this button.</param>
        /// <param name="background">Texture representing the background contents of this button.</param>
        /// <param name="text">Text filling the body of this button.</param>
        /// <param name="action">Action referring to the function this button will call upon being pressed.</param>
        public Button(Point size, Texture2D background, String text, Action action) {
            CreateButton(size, background, action);
            Text = new StringBuilder(text);
        }

        /// <summary>
        /// Create a new button from a string of text, so that the button will fit it.
        /// </summary>
        /// <param name="text">Text filling the body of this button.</param>
        /// <param name="background">Texture representing the background contents of this button.</param>
        /// <param name="action">Action referring to the function this button will call upon being pressed.</param>
        public Button(String text, Texture2D background, Action action) {
            CreateButtonFromText(text, DefaultTextColor, background, action);
        }

        /// <summary>
        /// Create a new button from a string of text, so that the button will fit it.  The text color can also be specified.
        /// </summary>
        /// <param name="text">Text filling the body of this button.</param>
        /// <param name="textColor">Color for this button's text.</param>
        /// <param name="background">Texture representing the background contents of this button.</param>
        /// <param name="action">Action referring to the function this button will call upon being pressed.</param>
        public Button(String text, Color textColor, Texture2D background, Action action) {
            CreateButtonFromText(text, textColor, background, action);
        }

        /// <summary>
        /// Draw this button using the given SpriteBatch to the given position.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch, Point p) {
            Debug.Assert((Contents == null || Text == null), "A button cannot have both a string and an image for its contents!");
            // Draw the background of the button
            Rectangle bounds = new Rectangle(p, Size);
            spriteBatch.Draw(Background, bounds, Color.White);

            // Draw the contents of the button, if any
            if (Contents != null) {
                int contentsWidth = bounds.Width / 3;
                int contentsHeight = (bounds.Height * 2) / 3;
                int towerY = bounds.Y + bounds.Height / 2;
                int towerX = bounds.X + bounds.Width / 2;

                Contents.Draw(towerX, towerY, spriteBatch, contentsWidth, contentsHeight);
            } else if(Text != null) {
                Vector2 textSize = Art.Font.MeasureString(Text);
                // Resize text to fit this button, if necessary
                Vector2 Scale = new Vector2(1, 1);
                if(textSize.X > bounds.Width) {
                    Scale.X = bounds.Width / textSize.X;
                }
                if(textSize.Y > bounds.Height) {
                    Scale.Y = bounds.Height / textSize.Y;
                }

                Vector2 Pos = bounds.Center.ToVector2() - (textSize / 2);
                spriteBatch.DrawString(Art.Font, Text, Pos, TextColor, 0, Vector2.Zero, Scale, SpriteEffects.None, 1f);
            }
        }

        /// <summary>
        /// Check if the given point is contained within the bounds of this button.
        /// </summary>
        /// <param name="p">the point to be checked</param>
        /// <param name="startPos">The start point of this button.</param>
        /// <returns></returns>
        public bool Contains(Point p, Point startPos) {
            return new Rectangle(startPos, Size).Contains(p);
        }
    }
}
