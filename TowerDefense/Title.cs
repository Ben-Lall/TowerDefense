﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.Globals;


namespace TowerDefense {
    /// <summary>
    /// The title screen.
    /// </summary>
    static class Title {
        /// <summary>
        /// The list of UI Panels for the title screen.
        /// </summary>
        public static List<UIPanel> UIPanels { get; set; }

        /// <summary>
        /// Whether or not the title screen has been initialized.
        /// </summary>
        public static bool Initialized { get; set; }

        /// <summary>
        /// Initialize the title screen.
        /// </summary>
        public static void Initialize() {
            Initialized = true;
            UIPanels = new List<UIPanel>();

            //Create main title menu
            Button startButton = new Button("Load World", new Vector2(ScreenWidth / 2, 2 * ScreenHeight / 5), Art.MenuPanel, BeginPlay);
            List<Button> buttons = new List<Button>();
            buttons.Add(startButton);
            Rectangle bounds = new Rectangle(2 * ScreenWidth / 5, ScreenHeight / 3, ScreenWidth / 5, ScreenHeight / 3);
            UIPanels.Add(new UIPanel(Art.MenuPanel, bounds, buttons, UIType.Menu));
        }

        /// <summary>
        /// Draw the title screen.
        /// </summary>
        public static void Draw() {
            UISpriteBatch.Begin();
            foreach (UIPanel u in UIPanels) {
                u.Draw(UISpriteBatch);
            }
            UISpriteBatch.End();
        }

        /// <summary>
        /// Update the title screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Update(GameTime gameTime) {
            Input.HandleTitleInput();
        }

        /// <summary>
        /// Close the title screen and start playing the game.
        /// </summary>
        private static void BeginPlay() {
            CurrentGameState = GameState.Playing;
            Initialized = false;
            UIPanels.Clear();
        }
    }
}
