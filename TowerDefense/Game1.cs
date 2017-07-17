using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TowerDefense {
    /// <summary>
    /// This is the main type for the game.
    /// </summary>
    public class Game1 : Game {
        /* System */

        /// <summary>
        /// Graphics device.
        /// </summary>
        GraphicsDeviceManager graphics;

        /// <summary>
        /// SpriteBatch.
        /// </summary>
        SpriteBatch spriteBatch;

        /// <summary>
        /// The mouse's current state.
        /// </summary>
        MouseState mouseState;

        /* Graphics */

        /// <summary>
        /// Integer representing the width of the window, in pixels.
        /// </summary>
        int screenWidth;

        /// <summary>
        /// Integer representing the height of the window, in pixels.
        /// </summary>
        int screenHeight;

        /// <summary>
        /// Width in pixels of the menu panel.
        /// </summary>
        int menuPanelWidth;

        /// <summary>
        /// Height in pixels of the menu panel.
        /// </summary>
        int menuPanelHeight;

        /* Game World */

        /// <summary>
        /// 2D array representing the game map.  Read by map[i.j], where i refers to the row, and j refers to the column.
        /// </summary>
        Tile[,] map;

        /* Gameplay */

        /// <summary>
        /// List of templates of towers unlocked by the player
        /// </summary>
        List<TowerTemplate> ulTowers;

        /* UI */

        /// <summary>
        /// List of rectangles, where each rectangle represents the hit area of a menu button.
        /// </summary>
        List<Button> buttons;

        /// <summary>
        /// Boolean representing whether or not the player has selected a tower and is working on placing it.
        /// </summary>
        bool isPlacingTower;

        /// <summary>
        /// A Template of the tower whose placement is currently being deliberated, if any.
        /// </summary>
        TowerTemplate pendingTowerTemplate;

        /* Sprites */

        /// <summary>
        /// A baseline 1x1 texture used to draw lines.
        /// </summary>
        Texture2D pixel;

        /// <summary>
        /// A texture representing a tower.
        /// </summary>
        Texture2D tower;

        /// <summary>
        /// A texture for the tower selection panel.
        /// </summary>
        Texture2D menuPanel;

        /// <summary>
        /// A texture for the tower selection buttons.
        /// </summary>
        Texture2D towerButton;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            base.Initialize();

            // Set the screen resolution to be 16:9, as the game is largely balanced around this.
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            screenWidth = Window.ClientBounds.Width;
            screenHeight = Window.ClientBounds.Height;

            // Set the menu panel's height and width.
            menuPanelWidth = screenWidth / 8;
            menuPanelHeight = screenHeight;

            // Set the tile dimensions to 16px.  16 is a common factor of 720 and 1120, 1120 = 1280 * (7/8).
            Settings.TileWidth = 16;
            Settings.TileHeight = 16;

            // Set the number of tiles viewable on the screen, since the menu panel will obstruct the map slightly.
            Settings.ViewportRowLength = (screenWidth - menuPanelWidth) / Settings.TileWidth;
            Settings.ViewportColumnLength = screenHeight / Settings.TileHeight;

            // Initialize the gameplay objects
            map = new Tile[Settings.ViewportColumnLength, Settings.ViewportRowLength];

            // Initialize list of unlocked towers with the base bolt tower.
            ulTowers = new List<TowerTemplate>();
            ulTowers.Add(new TowerTemplate(TowerType.BOLT, tower));

            // Initialize list of menu buttons.
            buttons = new List<Button>();

            // Create sprites for drawing.
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
            // TEMP: load map (since ideally there'd be a title screen).
            LoadMap();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tower = Content.Load<Texture2D>("tower");
            menuPanel = Content.Load<Texture2D>("menu_panel");
            towerButton = Content.Load<Texture2D>("menu_panel");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /* Game logic functions */

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            // If a tower has been added/removed from the list, refresh the buttons list
            if (buttons.Count != ulTowers.Count) {
                RefreshButtonsList();
            }

            HandleInput();

            base.Update(gameTime);
        }

        /// <summary>
        /// Refresh the list of buttons to reflect the list of currently unlocked towers.
        /// </summary>
        private void RefreshButtonsList() {
            buttons.Clear();
            // Add a button for each tower in the list
            for (int i = 0; i < ulTowers.Count; i++) {
                Rectangle buttonBox = new Rectangle(screenWidth - menuPanelWidth + (menuPanelWidth / 4), (i * menuPanelHeight / 12) + (5 * i) + 5,
                                                    menuPanelWidth / 2, menuPanelHeight / 12);
                buttons.Add(new Button(buttonBox, towerButton, tower, null));
            }
        }

        /// <summary>
        /// Handle user input.
        /// </summary>
        private void HandleInput() {
            // Update mouseState
            mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed) {
                HandleLeftMouseClick(mouseState);
            } else if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                isPlacingTower = false;
                pendingTowerTemplate = null;
            }
                
        }

        /// <summary>
        /// Handle a left mouse click event.
        /// </summary>
        /// <param name="mouseState">The mouse's current state.</param>
        private void HandleLeftMouseClick(MouseState mouseState) {
            Button selectedButton = TowerButtonSelected(mouseState);
            if (selectedButton != null) {
                BeginTowerPlacement(ulTowers[buttons.IndexOf(selectedButton)]);
            } else if (isPlacingTower) {
                // TODO: Place tower if free space & resources.
            }

        }

        /// <summary>
        /// Begin tower placement.
        /// </summary>
        /// <param name="template">The template of the tower whose placement has begun.</param>
        private void BeginTowerPlacement(TowerTemplate template) {
            isPlacingTower = true;
            pendingTowerTemplate = template;
        }

        /// <summary>
        /// Return the button that the mouse is hovering over, or null if it isn't mousing over any buttons.
        /// </summary>
        /// <param name="mouseState">The mouse's current state.</param>
        /// <returns>The Button that the mouse is hovering over, or null if it isn't mousing over any buttons.</returns>
        private Button TowerButtonSelected(MouseState mouseState) {
            // Run quick check to see if mouse is within the boundaries of possible tower button placement.
            if (buttons.Count > 0 && mouseState.X >= buttons[0].X && mouseState.X <= buttons[0].X + buttons[0].Width) {
                // Then find a button with a matching Y coordinate (if any).
                foreach(Button b in buttons) {
                    if(mouseState.Y >= b.Y && mouseState.Y <= b.Y + b.Height) {
                        return b;
                    }
                }
                // If no such button was found, return null.
                return null;
            }
            // If no such button was found, return null.
            return null;
        }

        /* Drawing functions */

            /// <summary>
            /// This is called when the game should draw itself.
            /// </summary>
            /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            /* Draw gameplay elements */

            DrawMap();

            /* Draw UI elements */

            DrawMenuPanel();

            if(isPlacingTower) {
                DrawPendingTower();
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Draw a silhouette of the pending tower over the mouse's current location.  Draw it with a red tint if the target area is obstructed.
        /// </summary>
        protected void DrawPendingTower() {
            Point drawSize = new Point(pendingTowerTemplate.Width * Settings.TileWidth, pendingTowerTemplate.SpriteHeight);
            if (CursorIsOnMap()) {
                Point placementPos = GetCursorAreaPoint(pendingTowerTemplate.Width, pendingTowerTemplate.Height);

                // Highlight selected tiles
                for(int y = placementPos.Y; y < placementPos.Y + pendingTowerTemplate.Height; y++) {
                    for(int x = placementPos.X; x < placementPos.X + pendingTowerTemplate.Width; x++) {
                        Color tileColor = (map[y, x].Type == TileType.OPEN) ? Color.Green : Color.Red;
                        DrawTile(x, y, tileColor);
                    }
                }

                //Redraw the grid
                DrawGrid();

                //Draw the tower to snap to the selected tiles
                Point drawPos = new Point(placementPos.X * Settings.TileWidth, placementPos.Y * Settings.TileHeight);
                spriteBatch.Draw(pendingTowerTemplate.Sprite, new Rectangle(drawPos, drawSize), Color.White);

                //TODO: Check if the destination of this tower is obstructed, and change the tint accordingly
            } else {
                Point drawPos = new Point(mouseState.X, mouseState.Y) - new Point(pendingTowerTemplate.Width * Settings.TileWidth / 2, (2 * pendingTowerTemplate.SpriteHeight) / 3);
                spriteBatch.Draw(pendingTowerTemplate.Sprite, new Rectangle(drawPos, drawSize), Color.White);
            }
            
        }

        /// <summary>
        /// Returns a Point representing the coordinates of the top-left tile of the area highlighted by the cursor.
        /// </summary>
        /// <param name="width">Total width in units of tiles around the cursor</param>
        /// <param name="height">Total height in units of tiles around the cursor</param>
        /// <returns></returns>
        protected Point GetCursorAreaPoint(int width, int height) {
            Point cursorTilePos = PixelToClosestTile(new Point(mouseState.X, mouseState.Y));
            int x = Boundarize(cursorTilePos.X - width / 2, 0, Settings.ViewportRowLength - width);
            int y = Boundarize(cursorTilePos.Y - height / 2, 0, Settings.ViewportColumnLength - height);
            return new Point(x, y);  
        }

        /// <summary>
        /// Return target if it's within the boundaries, otherwise, return the boundary closest to tar.
        /// </summary>
        /// <param name="tar"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns>int i, i is between low and high (inclusive)</returns>
        protected int Boundarize(int tar, int low, int high) {
            return Math.Max(Math.Min(tar, high), low);
        }

        /// <summary>
        /// Return the map coordinates of the given pixel.
        /// </summary>
        /// <param name="pixel">Point containing the coordiantes of the pixel.</param>
        /// <returns></returns>
        protected Point PixelToTile(Point pixel) {
            return new Point(pixel.X / Settings.TileWidth, pixel.Y / Settings.TileHeight);
        }

        /// <summary>
        /// Return the map coordinates of the tile the given pixel is nearest to, rounded up.
        /// </summary>
        /// <param name="pixel">Point containing the coordiantes of the pixel.</param>
        /// <returns></returns>
        protected Point PixelToClosestTile(Point pixel) {
            int x = pixel.X % Settings.TileWidth;
            int y = pixel.Y % Settings.TileHeight;

            return PixelToTile(pixel + new Point(x, y));
        }

        /// <summary>
        /// Return true if the cursor is on the map, false otherwise.
        /// </summary>
        /// <returns></returns>
        protected bool CursorIsOnMap() {
            return mouseState.X < (screenWidth - menuPanelWidth);
        }

        /// <summary>
        /// Draw the menu panel to the correct position on the screen.
        /// </summary>
        protected void DrawMenuPanel() {
            int menuPanelX = screenWidth - menuPanelWidth;
            spriteBatch.Draw(menuPanel, new Rectangle(menuPanelX, 0, menuPanelWidth, menuPanelHeight), Color.White);

            /* Draw buttons for the panel. */
            foreach(Button b in buttons) {
                b.Draw(spriteBatch);
            }

        }

        /// <summary>
        /// Draw a line.
        /// </summary>
        /// <param name="x">Starting x position of this line</param>
        /// <param name="y">Starting y position of this line</param>
        /// <param name="width">Δx from the start to the end of this line.</param>
        /// <param name="height">Δy from start to the end of this line.</param>
        /// <param name="color">The color of this line.</param>
        protected void DrawLine(int x, int y, int width, int height, Color color) {
            spriteBatch.Draw(pixel, new Rectangle(x, y, width, height), color);
        }

        /// <summary>
        /// Draw the grid dividing tiles.
        /// </summary>
        protected void DrawGrid() {
            // Draw horizontal lines across the screen at each tile height
            for(int i = 0; i < screenHeight; i += Settings.TileHeight) {
                DrawLine(0, i, screenWidth - menuPanelWidth, 1, Color.Black);
            }

            // Draw vertical lines across the screen at each tile width
            for (int j = 0; j < screenWidth - menuPanelWidth; j += Settings.TileWidth) {
                DrawLine(j, 0, 1, screenHeight, Color.Black);
            }
        }

        /// <summary>
        /// Draw the grid of tiles and their colorations.
        /// </summary>
        protected void DrawMap() {
            //Shade in the limited tiles.
            for (int i = 0; i < Settings.ViewportColumnLength; i++) {
                for(int j = 0; j < Settings.ViewportRowLength; j++) {
                    if(map[i,j].Type == TileType.LIMITED) {
                        DrawTile(j, i, Color.Gray);
                    } else if(map[i, j].Type == TileType.OPEN) {
                        DrawTile(j, i, Color.White);
                    }
                }
            }

            // Overlay grid
            DrawGrid();
        }

        protected void DrawTile(int x, int y, Color color) {
            spriteBatch.Draw(pixel, new Rectangle(x * Settings.TileWidth, y * Settings.TileHeight, Settings.TileWidth, Settings.TileHeight), color);
        }

        /// <summary>
        /// Load in the next map.
        /// </summary>
        protected void LoadMap() {
            Random r = new Random();
            for(int i = 0; i < Settings.ViewportColumnLength; i++) {
                for(int j = 0; j < Settings.ViewportRowLength; j++) {
                    if(r.Next(100) <= 70) {
                        map[i, j] = new Tile(TileType.OPEN, j * Settings.TileWidth, i * Settings.TileHeight);
                    } else {
                        map[i, j] = new Tile(TileType.LIMITED, j * Settings.TileWidth, i * Settings.TileHeight);
                    }
                }
            }
        }
    }
}
