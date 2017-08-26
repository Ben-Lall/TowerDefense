using Microsoft.Xna.Framework;
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
        public static UIPanel CurrentScreen { get; set; }

        /// <summary>
        /// Whether or not the title screen has been initialized.
        /// </summary>
        public static bool Initialized { get; set; }

        static Rectangle DefaultPanelBounds { get => new Rectangle(2 * ScreenWidth / 5, ScreenHeight / 3, ScreenWidth / 5, ScreenHeight / 3); }
        static UIPanel DefaultUIPanel { get => new UIPanel(Art.MenuPanel, DefaultPanelBounds, UIType.Menu, DefaultPanelBounds.Height / 6); }

        static Rectangle SmallPanelBounds { get => new Rectangle(2 * ScreenWidth / 5, ScreenHeight / 3, ScreenWidth / 5, ScreenHeight / 3); }
        static UIPanel SmallUIPanel { get => new UIPanel(Art.MenuPanel, DefaultPanelBounds, UIType.Menu, DefaultPanelBounds.Height / 6); }

        /// <summary>
        /// Initialize the title screen.
        /// </summary>
        public static void Initialize() {
            Initialized = true;
            WorldName = null;
            DisplayTitleScreen();
        }



        /// <summary>
        /// Display the title screen.
        /// </summary>
        static void DisplayTitleScreen() {
            CurrentScreen = DefaultUIPanel;
            CurrentScreen.AddButton(new Button("Play", Art.MenuPanel, DisplayPlayScreen));
        }

        /// <summary>
        /// Display the screen for starting a game.
        /// </summary>
        static void DisplayPlayScreen() {
            CurrentScreen = DefaultUIPanel;
            CurrentScreen.AddButton(new Button("Create New World", Art.MenuPanel, () => WorldMap.GenerateMap(SaveManager.NextDefaultWorldName())));
            Color loadTextColor = SaveManager.HasLoadableWorlds() ? Color.Black : Color.Gray;
            CurrentScreen.AddButton(new Button("Load World", loadTextColor, Art.MenuPanel, DisplayLoadWorldScreen));
        }

        /// <summary>
        /// Display a list of loadable worlds and provide buttons to load them.
        /// If there are no loadable worlds, do nothing.
        /// </summary>
        static void DisplayLoadWorldScreen() {
            if(SaveManager.HasLoadableWorlds()) {
                CurrentScreen = DefaultUIPanel;
                foreach (String fileName in SaveManager.ListLoadableWorlds()) {
                    CurrentScreen.AddButton(new Button(fileName.Substring(0, fileName.IndexOf('.')), Art.MenuPanel, () => BeginPlay(fileName)));
                }
            }
        }

        /// <summary>
        /// Draw the title screen.
        /// </summary>
        public static void Draw() {
            UISpriteBatch.Begin();
            CurrentScreen.Draw(UISpriteBatch);
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
        /// Close the title screen and load the chosen world.
        /// </summary>
        private static void BeginPlay(String worldName) {
            CurrentGameState = GameState.Playing;
            Initialized = false;
            WorldName = worldName.Substring(0, worldName.IndexOf('.'));
            SaveManager.LoadMap(worldName);
        }
    }
}
