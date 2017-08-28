using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TowerDefense;

namespace Include {

    /// <summary>
    /// A class containing global variables and helper methods.
    /// </summary>
    static class Globals {
        /* SpriteBatches */

        /// <summary>
        /// SpriteBatch containing details of drawing creatures and map features.
        /// </summary>
        public static SpriteBatch WorldSpriteBatch { get; set; }

        /// <summary>
        /// SpriteBatch containing details of drawing UI elements.
        /// </summary>
        public static SpriteBatch UISpriteBatch { get; set; }

        /// <summary>
        /// SpriteBatch containing details of Bolt Effects.
        /// </summary>
        public static SpriteBatch BoltSpriteBatch { get; set; }

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

        public static Point ScreenCenter { get => new Point(ScreenWidth / 2, ScreenHeight / 2); }

        public static int MaxScreenHeight { get => 1080; }
        public static int MaxScreenWidth { get => 1920; }

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

        /* Loading Screen information */

        /// <summary>
        /// The text to be displayed on the loading screen.
        /// </summary>
        public static String LoadText;

        /// <summary>
        /// The amount of progress the current load operation has made. Between 0 and 1 inclusive. 
        /// </summary>
        public static float LoadProgress;

        /// <summary>
        /// Whether or not the game has finished generating a world.
        /// </summary>
        public static bool DoneGenerating;

        /// <summary>
        /// Initialize globals.
        /// </summary>
        /// <param name="window"></param>
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
