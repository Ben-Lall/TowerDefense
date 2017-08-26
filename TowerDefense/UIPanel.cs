using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense {
    enum UIType { TowerPanel, Automap, Menu }

    class UIPanel {

        /// <summary>
        /// The body of this UI panel.
        /// </summary>
        public Rectangle Bounds { get; set; }

        /// <summary>
        /// The sprite of this UI panel.
        /// </summary>
        public Texture2D BodySprite {get; set;}

        /// <summary>
        /// A dictionary of buttons, where each button is paired with its offset position.
        /// </summary>
        public Dictionary<Button, Point> Buttons { get; set; }

        /// <summary>
        /// The depth of this UI panel.  Panels with a lower depth are drawn above ones with highter depths.
        /// Always >= 0.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// The visibility of this UI element.  Invisible UI elements are treated as if they do not exist.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// The type of UI this is.
        /// </summary>
        public UIType Type { get; set; }

        /// <summary>
        /// The y offset of the first button.
        /// </summary>
        int ButtonStartHeight { get; set; }

        /// <summary>
        /// The buffer space between buttons. Measured in units of pixels.
        /// </summary>
        static int Y_BUTTON_BUFFER = 5;

        /// <summary>
        /// Constructor for a new UI panel.
        /// </summary>
        /// <param name="sprite">The sprite for the background of this UI panel</param>
        /// <param name="bounds">Bounds of this UI panel</param>
        /// <param name="depth">The depth of this UI element. Esed to determine draw order.</param>
        public UIPanel(Texture2D sprite, Rectangle bounds, UIType type, int buttonStartHeight) {
            BodySprite = sprite;
            Bounds = bounds;
            Buttons = new Dictionary<Button, Point>();
            ButtonStartHeight = buttonStartHeight;
            Visible = true;
            Type = type;

            // Define depth based on the type of UI.
            switch(Type) {
                case UIType.TowerPanel:
                    Depth = 50;
                    break;
                case UIType.Menu:
                    Depth = 10;
                    break;
                default:
                    Depth = 30;
                    break;
            }
        }

        /// <summary>
        /// Parameterless constructor used for inherited classes.
        /// </summary>
        protected internal UIPanel() { }

        /// <summary>
        /// Draw this UI panel using the given SpriteBatch
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(BodySprite, Bounds, Color.White);
            // Draw buttons
            foreach(KeyValuePair<Button, Point> bPair in Buttons) {
                bPair.Key.Draw(spriteBatch, Bounds.Location + bPair.Value);
            }
        }

        /// <summary>
        /// Add a button to this UI panel, offset by the given point.
        /// </summary>
        /// <param name="b">The button to be added.</param>
        /// <param name="offset">The offset of this button.</param>
        public void AddButton(Button b, Point offset) {
            Buttons.Add(b, offset);
        }


        /// <summary>
        /// Add a button to this UI panel.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="height"></param>
        public void AddButton(Button b) {
            int yOffset = Buttons.Count == 0 ? ButtonStartHeight : Buttons.Last().Value.Y + Y_BUTTON_BUFFER;
            int xOffset = (Bounds.Center.X - b.Size.X / 2) - Bounds.X;
            Buttons.Add(b, new Point(xOffset, yOffset));
        }

        /// <summary>
        /// Determine if the given point is within the bounds of this UI Panel, as long as this UI Panel is visible.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains(Point p) {
            return Visible && Bounds.Contains(p);
        }

        /// <summary>
        /// Determine if the given object is a UIPanel or an inherited class.
        /// </summary>
        /// <returns></returns>
        public static bool IsUIElement(object o) {
            Type t = o.GetType();
            return t == typeof(UIPanel) || t == typeof(AutoMap);
        }

        /// <summary>
        /// Simulate a mouse click on the UI panel at the given location, and react appropriately.
        /// Assumes IsClicked() is true.
        /// </summary>
        /// <param name="p">The position of the mouse click.  Does not necessarily have to be within the bounds of this UI panel.</param>
        public void Click(Point p) {
            foreach (KeyValuePair<Button, Point> bPair in Buttons) {
                if (bPair.Key.Contains(p, Bounds.Location + bPair.Value)) {
                    bPair.Key.OnClick();
                    break;
                }
            }
        }

    }
}
