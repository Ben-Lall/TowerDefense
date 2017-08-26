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
        /// A list of the buttons this UI panel holds.
        /// </summary>
        public List<Button> Buttons { get; set; }

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
        /// Constructor for a new UI panel.
        /// </summary>
        /// <param name="sprite">The sprite for the background of this UI panel</param>
        /// <param name="bounds">Bounds of this UI panel</param>
        /// <param name="buttons">The buttons contained within this panel.  If null, an empty will be created.</param>
        /// <param name="depth">The depth of this UI element. Esed to determine draw order.</param>
        public UIPanel(Texture2D sprite, Rectangle bounds, List<Button> buttons, UIType type) {
            BodySprite = sprite;
            Bounds = bounds;
            Buttons = buttons;
            if(Buttons == null) {
                Buttons = new List<Button>();
            }
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
            foreach(Button b in Buttons) {
                b.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Add a button to this UI panel.
        /// </summary>
        /// <param name="b">The button to be added.</param>
        public void AddButton(Button b) {
            Buttons.Add(b);
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
            foreach(Button b in Buttons) {
                if(b.Contains(p)) {
                    b.OnClick();
                    break;
                }
            }
        }

    }
}
