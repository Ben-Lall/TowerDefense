using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.Globals;


enum ButtonType {TOWER};

namespace TowerDefense {
    /// <summary>
    /// A menu button.  Has a visual appearance, and executes an action when pressed.
    /// </summary>
    class Button {

        /// <summary>
        /// Rectangle representing the hit detection area of this button.
        /// </summary>
        public Rectangle HitBox { get; set; }

        public int X { get => HitBox.X; set => HitBox = new Rectangle(value, Y, Width, Height); }
        public int Y { get => HitBox.Y; set => HitBox = new Rectangle(X, value, Width, Height); }
        public int Width { get => HitBox.Width; set => HitBox = new Rectangle(X, Y, value, Height); }
        public int Height { get => HitBox.Height; set => HitBox = new Rectangle(X, Y, Width, value); }

        /// <summary>
        /// Texture for the button background.
        /// </summary>
        public Texture2D Background { get; set; }

        /// <summary>
        /// Texture for contents of this button.  May be null.
        /// </summary>
        public AnimatedSprite Contents { get; set; }

        /// <summary>
        /// The action the button should perform upon being pressed.
        /// </summary>
        public Action OnClick;

        /// <summary>
        /// Create a new button.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="background"></param>
        /// <param name="contents"></param>
        /// <param name="action"></param>
        private void createNewRep(Rectangle shape, Texture2D background, AnimatedSprite contents, Action action) {
            HitBox = shape;
            Background = background;
            Contents = contents;
            OnClick = action;
        }

        /// <summary>
        /// Create a new button without any contents.
        /// </summary>
        /// <param name="pos">A point representing the top-left corner of this button.</param>
        /// <param name="shape">Rectangle representing the hitbox of this button.</param>
        /// <param name="background">Texture representing the background contents of this button.</param>
        /// <param name="action">Action referring to the function this button will call upon being pressed.</param>
        public Button(Rectangle shape, Texture2D background, Action action) {
            createNewRep(shape, background, null, action);
        }

        /// <summary>
        /// Create a new button.
        /// </summary>
        /// <param name="pos">A point representing the top-left corner of this button.</param>
        /// <param name="shape">Rectangle representing the hitbox of this button.</param>
        /// <param name="background">Texture representing the background contents of this button.</param>
        /// <param name="contents">Texture representing the interior content of this button.</param>
        /// <param name="action">Action referring to the function this button will call upon being pressed.</param>
        public Button(Rectangle shape, Texture2D background, AnimatedSprite contents, Action action) {
            createNewRep(shape, background, contents, action);
        }

        /// <summary>
        /// Draw this button using the given SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch) {
            // Draw the background of the button
            spriteBatch.Draw(Background, HitBox, Color.White);

            // Draw the contents of the button, if any
            if (Contents != null) {
                int contentsWidth = HitBox.Width / 3;
                int contentsHeight = (HitBox.Height * 2) / 3;
                int towerY = HitBox.Y + HitBox.Height / 2;
                int towerX = HitBox.X + HitBox.Width / 2;

                Contents.Draw(towerX, towerY, spriteBatch, contentsWidth, contentsHeight);
            }
        }

        /// <summary>
        /// Check if the given point is contained within the bounds of this button.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains(Point p) {
            return HitBox.Contains(p);
        }
    }
}
