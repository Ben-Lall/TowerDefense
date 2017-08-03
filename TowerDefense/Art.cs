using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TowerDefense {
    /// <summary>
    /// A static class dedicated to art assets and all associated information.
    /// </summary>
    static class Art {
        /* Fonts */
        public static SpriteFont Font;

        /* UI Textures */
        public static Texture2D MenuPanel;
        public static Texture2D TowerButton;

        /* Tower Textures */
        public static Texture2D Tower;
        public static Texture2D Hub;

        /* Monster Textures */
        public static Texture2D Imp;

        /* Effect Textures */
        public static Texture2D BoltLine;
        public static Texture2D BoltCap;
        public const float BoltThickness = 11;
        public static Texture2D Pixel;

        internal static void LoadContent(ContentManager content, GraphicsDevice graphics) {
            /* Fonts */
            Font = content.Load<SpriteFont>("Font");
            /* UI Textures */
            MenuPanel = content.Load<Texture2D>("menu_panel");
            TowerButton = content.Load<Texture2D>("menu_panel");
            /* Tower Textures */
            Tower = content.Load<Texture2D>("torreMagica");
            Hub = content.Load<Texture2D>("hub");
            /* Monster Textures */
            Imp = content.Load<Texture2D>("imp");

            /* Effect Textures */
            BoltLine = content.Load<Texture2D>("boltLine");
            BoltCap = content.Load<Texture2D>("boltCap");

            Pixel = new Texture2D(graphics, 1, 1);
            Pixel.SetData<Color>(new Color[] { Color.White });
        }

    }
}
