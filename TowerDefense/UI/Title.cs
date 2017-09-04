using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.GameState;
using static Include.Globals;


namespace TowerDefense {
    /// <summary>
    /// The title screen.
    /// </summary>
    static class TitleState {
        /// <summary>
        /// The list of UI Panels for the title screen.
        /// </summary>
        public static Stack<UIPanel> ScreenList;

        /// <summary>
        /// The currently displaying screen of the menu.
        /// </summary>
        public static UIPanel CurrentScreen { get => ScreenList.Peek(); }

        /// <summary>
        /// Whether or not the title screen has been initialized.
        /// </summary>
        public static bool Initialized { get; set; }

        /// <summary>
        /// Whether or not the loading screen has been initialized.
        /// </summary>
        public static bool LoadingScreenInitialized { get; set; }

        static Rectangle DefaultPanelBounds { get => new Rectangle(2 * ScreenWidth / 5, ScreenHeight / 3, ScreenWidth / 5, ScreenHeight / 3); }
        static UIPanel DefaultUIPanel { get => new UIPanel(Art.MenuPanel, DefaultPanelBounds, UIType.Menu, DefaultPanelBounds.Height / 6); }

        /* Sample World data */

        /// <summary>
        /// The sample map.
        /// </summary>
        static SampleMap Sample;

        /// <summary>
        /// Initialize the title screen.
        /// </summary>
        public static void Initialize() {
            ScreenList = new Stack<UIPanel>();
            Initialized = true;
            WorldName = null;
            GeoType SampleType = WorldMap.RandomGeoType;
            Sample = new SampleMap();

            LoadTitleScreen();
        }

        /// <summary>
        /// Initialize a new loading screen.
        /// </summary>
        public static void InitializeLoadingScreen() {
            LoadingScreenInitialized = true;
            UIPanel screen = DefaultUIPanel;
            ScreenList.Push(screen);
            AddCancelButton();
        }

        /// <summary>
        /// Display the title screen.
        /// </summary>
        static void LoadTitleScreen() {
            UIPanel screen = DefaultUIPanel;
            ScreenList.Push(screen);
            screen.AddButton(new Button("Play", Art.MenuPanel, LoadPlayScreen));
        }

        /// <summary>
        /// Display the screen for starting a game.
        /// </summary>
        static void LoadPlayScreen() {
            UIPanel screen = DefaultUIPanel;
            ScreenList.Push(screen);
            screen.AddButton(new Button("Create New World", Art.MenuPanel, () => SaveManager.GenerateMap(SaveManager.NextDefaultWorldName(), 2000, 2000)));
            Color loadTextColor = SaveManager.HasLoadableWorlds() ? Color.Black : Color.Gray;
            screen.AddButton(new Button("Load World", loadTextColor, Art.MenuPanel, LoadWorldSelectScreen));
            AddBackButton();
        }

        /// <summary>
        /// Add a back button to the current screen.
        /// </summary>
        static void AddBackButton() {
            Button backButton = new Button("Back", Art.MenuPanel, DropScreen);
            Point offset = new Point(4, CurrentScreen.Bounds.Height - backButton.Height - 4);
            CurrentScreen.AddButton(backButton, offset);
        }

        /// <summary>
        /// Add a cancel button to the current screen.
        /// </summary>
        static void AddCancelButton() {
            Button cancelButton = new Button("Cancel", Art.MenuPanel, SaveManager.AbortLoad);
            Point offset = new Point(4, CurrentScreen.Bounds.Height - cancelButton.Height - 4);
            CurrentScreen.AddButton(cancelButton, offset);
        }

        /// <summary>
        /// Display a list of loadable worlds and provide buttons to load them.
        /// If there are no loadable worlds, do nothing.
        /// </summary>
        static void LoadWorldSelectScreen() {
            if(SaveManager.HasLoadableWorlds()) {
                UIPanel screen = DefaultUIPanel;
                ScreenList.Push(screen);
                foreach (String fileName in SaveManager.ListLoadableWorlds()) {
                    screen.AddButton(new Button(fileName.Substring(0, fileName.IndexOf('.')), Art.MenuPanel, () => BeginPlay(fileName)));
                }
            }
            AddBackButton();
        }

        /// <summary>
        /// Close the current screen and return to the previous one.
        /// </summary>
        public static void DropScreen() {
            if(CurrentGameState != GameStatus.Loading && ScreenList.Count > 1) {
                ScreenList.Pop();
            }
        }

        /// <summary>
        /// Draw the title screen.
        /// </summary>
        public static void Draw() {
            Sample.Draw();
            CurrentScreen.Draw(UISpriteBatch);
        }

        /// <summary>
        /// Draw the current loading screen.
        /// </summary>
        public static void DrawLoadingScreen() {
            Draw();
            DrawLoadingProgress();
        }

        /// <summary>
        /// Draw the loading bar of the current load progress.
        /// </summary>
        public static void DrawLoadingProgress() {
            // Draw the loading text
            int yDisplacement = ScreenHeight / 18;
            if (LoadText != null) {
                Vector2 stringSize = Art.Font.MeasureString(LoadText);
                Vector2 textPosition = DefaultPanelBounds.Center.ToVector2() - stringSize / 2 - new Vector2(0, yDisplacement);
                UISpriteBatch.DrawString(Art.Font, LoadText, textPosition, Color.Black);
            }
            // Draw the background bar
            int xOffset = ScreenWidth / 25;
            Point barStart = new Point(DefaultPanelBounds.X + xOffset, DefaultPanelBounds.Center.Y + yDisplacement);
            Point barSize = new Point(DefaultPanelBounds.Width - 2 * xOffset, yDisplacement);
            UISpriteBatch.Draw(Art.MenuPanel, new Rectangle(barStart, barSize), Color.White);

            // Draw the progress bar
            Point progressSize = (barSize.ToVector2() * new Vector2(LoadProgress, 1)).ToPoint();
            UISpriteBatch.Draw(Art.Pixel, new Rectangle(barStart, progressSize), Color.White);
        }

        /// <summary>
        /// Update the title screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Update(GameTime gameTime) {
            if(CurrentGameState != GameStatus.Loading && LoadingScreenInitialized) {
                LoadingScreenInitialized = false;
                DropScreen();
            }

            Sample.Update(gameTime);
            Input.HandleInput();
        }

        /// <summary>
        /// Close the title screen and load the chosen world.
        /// </summary>
        private static void BeginPlay(String worldName) {
            WorldName = worldName.Substring(0, worldName.IndexOf('.'));
            SaveManager.LoadMap(worldName);
        }
    }
}
