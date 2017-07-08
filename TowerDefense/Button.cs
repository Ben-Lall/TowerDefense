using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

enum ButtonType {TOWER};

namespace TowerDefense {
    /// <summary>
    /// A menu button.  Has a visual appearance, and executes an action when pressed.
    /// </summary>
    class Button {

        /// <summary>
        /// Rectangle representing the hit detection area of this button.
        /// </summary>
        private Rectangle hitBox;

        /// <summary>
        /// Texture for the button background.
        /// </summary>
        private Texture2D background;

        /// <summary>
        /// Texture for contents of this button.  May be null.
        /// </summary>
        private Texture2D contents;

        /// <summary>
        /// The action the button should perform upon being pressed.
        /// </summary>
        private Action pressAction;

        private void createNewRep(Rectangle shape, Texture2D background, Texture2D contents, Action action) {
            HitBox = shape;
            Background = background;
            Contents = contents;
            pressAction = action;
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
        public Button(Rectangle shape, Texture2D background, Texture2D contents, Action action) {
            createNewRep(shape, background, contents, action);
        }

        public void Draw(SpriteBatch spriteBatch) {

            // Draw the background of the button
            spriteBatch.Draw(Background, HitBox, Color.White);

            // Draw the contents of the button, if any
            if (Contents != null) {
                int towerWidth = HitBox.Width / 3;
                int towerHeight = (HitBox.Height * 2) / 3;
                int towerY = HitBox.Y + ((HitBox.Height - towerHeight) / 2);
                int towerX = HitBox.X + ((HitBox.Width - towerWidth) / 2);

                spriteBatch.Draw(Contents, new Rectangle(towerX, towerY, towerWidth, towerHeight), Color.White);
            }
        }


        /* Setters and getters */

        public Rectangle HitBox { get => hitBox; set => hitBox = value; }
        public Texture2D Background { get => background; set => background = value; }
        public Texture2D Contents { get => contents; set => contents = value; }
        public Action PressAction { get => pressAction; set => pressAction = value; }
        public int X { get => hitBox.X; set => hitBox.X = value; }
        public int Y { get => hitBox.Y; set => hitBox.Y = value; }
        public int Width { get => hitBox.Width; set => hitBox.Width = value; }
        public int Height { get => hitBox.Height; set => hitBox.Height = value; }
    }
}
