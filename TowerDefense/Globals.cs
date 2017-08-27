using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense;

namespace Include {

    /// <summary>
    /// A class containing global variables and helper methods.
    /// </summary>
    static class Globals {
        /* System */

        /// <summary>
        /// Graphics device.
        /// </summary>
        public static GraphicsDeviceManager Graphics { get; set; }

        /// <summary>
        /// The game window.
        /// </summary>
        public static GameWindow Window;

        /// <summary>
        /// Integer representing the width of the window, in pixels.
        /// </summary>
        public static int ScreenWidth { get => Window.ClientBounds.Width; }

        /// <summary>
        /// Integer representing the height of the window, in pixels.
        /// </summary>
        public static int ScreenHeight { get => Window.ClientBounds.Height; }

        /* Graphics */

        /// <summary>
        /// Integer representing the width of all tiles.
        /// </summary>
        public static int TileWidth { get; set; }

        /// <summary>
        /// Integer representing the height of all tiles.
        /// </summary>
        public static int TileHeight { get; set; }

        /// <summary>
        /// The current game state.
        /// </summary>
        public static GameStatus CurrentGameState;

        public static void Initialize(GameWindow window) {
            Window = window;
            Input.PreviousMouseWheel = Mouse.GetState().ScrollWheelValue;

            // Set tile size to be 32x32
            TileWidth = 32;
            TileHeight = 32;

            // Set the screen resolution to be 16:9.
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();

        }



        /** Constants **/
        public static double SQRT2 { get { return Math.Sqrt(2); } }
        public static TowerTemplate BoltTowerTemplate { get => new TowerTemplate(TowerType.Bolt); }
        public static TowerTemplate HubTemplate { get => new TowerTemplate(TowerType.Hub); }
    }
}
