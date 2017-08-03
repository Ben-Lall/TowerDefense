using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        /// A set of towers / creatures, sorted by coordinate position, so as to be drawn in the correct order.
        /// </summary>
        List<Object> drawSet;

        /* Input */

        /// <summary>
        /// The mouse's current state.
        /// </summary>
        MouseState mouseState;

        /// <summary>
        /// Toggle boolean for the pause button.
        /// </summary>
        bool pausePressed;

        /// <summary>
        /// Toggle boolean for the mouse button.
        /// </summary>
        bool mousePressed;

        /// <summary>
        /// Toggle boolean for back button.
        /// </summary>
        bool backPressed;

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
        /// Boolean representing if the game is currently paused.
        /// </summary>
        bool paused;

        /// <summary>
        /// List of templates of towers unlocked by the player
        /// </summary>
        List<TowerTemplate> ulTowers;

        /// <summary>
        /// List of Towers currently on the game map.
        /// </summary>
        List<Tower> towers;

        /// <summary>
        /// The hub.
        /// </summary>
        Tower hub;

        /// <summary>
        /// List of monsters currently on the game map.
        /// </summary>
        List<Monster> monsters;

        /// <summary>
        /// Integer representing the current wave.  Always > 0.
        /// </summary>
        int currentWave;

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


        DrawComparer drawComparer = new DrawComparer();

        private static readonly BlendState maxBlend = new BlendState() {
            AlphaBlendFunction = BlendFunction.Max,
            ColorBlendFunction = BlendFunction.Max,
            AlphaDestinationBlend = Blend.One,
            AlphaSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,
            ColorSourceBlend = Blend.One
        };

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

            // Initialize the gameplay objects.
            map = new Tile[Settings.ViewportColumnLength, Settings.ViewportRowLength];

            //Initialize collections
            towers = new List<Tower>();
            monsters = new List<Monster>();
            drawSet = new List<Object>();
            buttons = new List<Button>();
            
            Globals.InitializeGlobals();

            ulTowers = new List<TowerTemplate>();
            ulTowers.Add(Globals.BoltTowerTemplate);

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
            Art.LoadContent(Content, GraphicsDevice);

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
            drawSet.Sort(drawComparer);
            if (!paused) {
                if (buttons.Count != ulTowers.Count) {
                    RefreshButtonsList();
                }

                UpdateTowers(gameTime);
                UpdateMonsters(gameTime);
                UpdateEffects(gameTime);
            }

            HandleInput();

            base.Update(gameTime);
        }

        /// <summary>
        /// Update effects.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateEffects(GameTime gameTime) {
            Globals.effects.RemoveAll(x => x.IsComplete);
            foreach(LightningBolt e in Globals.effects) {
                e.Update();
            }
        }

        /// <summary>
        /// Update towers. This calls each tower and has them execute their automatic abilities.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateTowers(GameTime gameTime) {
            foreach (Tower t in towers) {
               t.Update(gameTime, monsters);
            }
        }

        /// <summary>
        /// Update monsters.  For now, this only entails movement.
        /// </summary>
        private void UpdateMonsters(GameTime gameTime) {
            // Remove dead monsters from the list.
            monsters.RemoveAll(x => !x.IsAlive);
            drawSet.RemoveAll(x => x.GetType() == typeof(Monster) && !((Monster)x).IsAlive);

            foreach (Monster m in monsters) {
                m.Move(gameTime);
            }
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
                buttons.Add(new Button(buttonBox, Art.TowerButton, Art.Tower, null));
            }
        }

        /// <summary>
        /// Handle user input.
        /// </summary>
        private void HandleInput() {
            // Update mouseState
            mouseState = Mouse.GetState();

            /** Mouse handling **/
            if (mouseState.LeftButton == ButtonState.Pressed && !mousePressed) {
                HandleLeftMouseClick(mouseState);
                mousePressed = true;
            } else if(mouseState.LeftButton == ButtonState.Released) {
                mousePressed = false;
            }

            /** Keyboard Handling **/
            if (!backPressed && (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))) {
                backPressed = true;
                if (isPlacingTower) {// Stop tower placement
                    isPlacingTower = false;
                    pendingTowerTemplate = null;
                } else if (!isPlacingTower) {
                    ClearTowerIllumination();
                } else if (!pausePressed) {
                    //TODO: Open menu
                }
            } else if(Keyboard.GetState().IsKeyUp(Keys.Escape)) {
                backPressed = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !pausePressed) {
                // Toggle pause if menu is not open
                paused = !paused;
                pausePressed = true;
            } else if (Keyboard.GetState().IsKeyUp(Keys.Space) && pausePressed) {
                pausePressed = false;
            }

        }

        /// <summary>
        /// Handle a left mouse click event.
        /// </summary>
        /// <param name="mouseState">The mouse's current state.</param>
        private void HandleLeftMouseClick(MouseState mouseState) {
            object selectedItem = GetClickedObject(mouseState);
            if (selectedItem != null) {
                if (selectedItem.GetType() == typeof(Button)) { // if a button was pressed
                    BeginTowerPlacement(ulTowers[buttons.IndexOf((Button)selectedItem)]);
                } else if (selectedItem.GetType() == typeof(Tower)) { // if a tower was clicked
                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift)) { // and shift was held
                        ((Tower)selectedItem).Selected = !((Tower)selectedItem).Selected;
                    } else { // shift was not held
                        ClearTowerIllumination();
                        ((Tower)selectedItem).Selected = true;
                    }
                } 
            } else if (isPlacingTower && CursorIsOnMap() && ValidTowerLocation()) {
                PlacePendingTower();
            } else { // Actions that would deselect the selected towers on mouse click
                ClearTowerIllumination();
            }
        }

        /// <summary>
        /// Clears all tower illumination.
        /// </summary>
        private void ClearTowerIllumination() {
            foreach (Tower t in towers) {
                t.Selected = false;
            }
        }

        /// <summary>
        /// Place the currently pending tower.
        /// </summary>
        private void PlacePendingTower() {
            // TODO: Check for proper resources.
            Point pos = GetAreaStartPoint();
            // Place Tower
            Tower newTower = new Tower(pendingTowerTemplate, pos);
            AddTower(newTower);
        }

        /// <summary>
        /// Adds the given tower to the game world.
        /// </summary>
        /// <param name="tower">The tower to be added.</param>
        private void AddTower(Tower tower) {
            towers.Add(tower);
            drawSet.Add(tower);

            // Mark each of its tiles as TOWER
            for (int y = 0; y < tower.Height; y++) {
                for (int x = 0; x < tower.Width; x++) {
                    Map(tower.Pos.X + x, tower.Pos.Y + y).ContainsTower = true;
                }
            }
        }

        /// <summary>
        /// Adds the given monster to the game world.
        /// </summary>
        /// <param name="monster">The monster to be added.</param>
        private void AddMonster(Monster monster) {
            monsters.Add(monster);
            drawSet.Add(monster);
        }

        /// <summary>
        /// Assumes isPlacingTower = true, and CursorIsOnMap() = true.  Return true if the area the cursor is hovering over allows
        /// for enough space to place the currently selected tower. Returns false otherwise.
        /// </summary>
        /// <returns></returns>
        protected bool ValidTowerLocation() {
            //Return false if any of the tiles in the pending tower's selection area are obstructed, and true otherwise.
            Point pos = GetAreaStartPoint();
            for(int y = pos.Y; y < pos.Y + pendingTowerTemplate.Height; y++) {
                for(int x = pos.X; x < pos.X + pendingTowerTemplate.Width; x++) {
                    if(Map(x, y).ObstructsTower()) {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Returns a Point representing the coordinates of the top-left tile of the area highlighted by the cursor, where the dimensions are
        /// the dimensions of the pending tower.
        /// </summary>
        /// <returns></returns>
        protected Point GetAreaStartPoint() {
            int width = pendingTowerTemplate.Width;
            int height = pendingTowerTemplate.Height;
            Point cursorTilePos = PixelToClosestTile(new Point(mouseState.X, mouseState.Y));
            int x = Boundarize(cursorTilePos.X - width / 2, 0, Settings.ViewportRowLength - width);
            int y = Boundarize(cursorTilePos.Y - height / 2, 0, Settings.ViewportColumnLength - height);
            return new Point(x, y);
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
        /// Get the object that the mouse is hovering over.
        /// </summary>
        /// <param name="mouseState">The mouse's current state.</param>
        /// <returns>The object that the mouse is hovering over, or null if it isn't mousing over any object.</returns>
        private object GetClickedObject(MouseState mouseState) {
            // Run quick check to see if mouse is within the boundaries of possible tower button placement.
            if (buttons.Count > 0 && mouseState.X >= buttons[0].X && mouseState.X <= buttons[0].X + buttons[0].Width) {
                // Then find a button with a matching Y coordinate (if any).
                foreach(Button b in buttons) {
                    if(mouseState.Y >= b.Y && mouseState.Y <= b.Y + b.Height) {
                        return b;
                    }
                }
            }

            // Next, check if a tower was selected
            else if (CursorIsOnMap()) {
                Point clickedTile = PixelToTile(mouseState.Position);
                foreach(Tower t in towers) {
                    if(t.ContainsTile(clickedTile)) {
                        return t;
                    }
                }
            }

            return null;
        }

        /* Graphics functions */

            /// <summary>
            /// This is called when the game should draw itself.
            /// </summary>
            /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            /* Draw gameplay elements */

            DrawMap();
            DrawGameplayObjects();

            /* Draw UI elements */

            DrawUI();

            if(isPlacingTower) {
                DrawPendingTower();
            }

            DrawDebug();

            // Change spriteBatch into the mode for drawing effects.
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Texture, maxBlend);
            foreach (LightningBolt e in Globals.effects) { // TODO: replace with usage of drawSet in DrawGameplayObjects()
                e.Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawUI() {
            DrawMenuPanel();
            String pauseText = "Paused!";
            Vector2 pauseTextSize = Art.Font.MeasureString(pauseText);
            Vector2 pausePosition = new Vector2(screenWidth - 5, screenHeight - 25) - pauseTextSize;
            if (paused) {
                spriteBatch.DrawString(Art.Font, pauseText, pausePosition, Color.Black);
            }

            String monsterText = "Monsters: " + monsters.Count;
            Vector2 monsterTextSize = Art.Font.MeasureString(monsterText);
            Vector2 monsterTextPosition = new Vector2(screenWidth - 5, pausePosition.Y) - monsterTextSize;
            spriteBatch.DrawString(Art.Font, monsterText, monsterTextPosition, Color.Black);

        }

        /// <summary>
        /// Draw debug information to the screen.
        /// </summary>
        private void DrawDebug() {
            // Draw hover information
            if(CursorIsOnMap()) {
                // Draw currently hovered tile coordinates
                String hoverTileText = "Hover: (" + (mouseState.X / Settings.TileWidth) + ", " + (mouseState.Y / Settings.TileHeight) + ")";
                Vector2 hoverSize = Art.Font.MeasureString(hoverTileText);
                spriteBatch.DrawString(Art.Font, hoverTileText, new Vector2(screenWidth - 5, screenHeight - 5) - hoverSize, Color.Black);
            }
        }

        /// <summary>
        /// Draw each element in drawSet.
        /// </summary>
        private void DrawGameplayObjects() {
            // Draw towers / creatures in the proper order.
            foreach (object obj in drawSet) {
                if (obj.GetType() == typeof(Tower)) {
                    ((Tower)obj).Draw(spriteBatch);
                } else if (obj.GetType() == typeof(Monster)) {
                    ((Monster)obj).Draw(spriteBatch);
                }
            }

            // Draw firing ranges of all selected towers
            foreach(Tower t in towers) {
                if (t.Selected) {
                    t.DrawFiringRange(spriteBatch);
                }
            }
        }

        /// <summary>
        /// Draw a silhouette of the pending tower over the mouse's current location.  Draw it with a red tint if the target area is obstructed.
        /// </summary>
        protected void DrawPendingTower() {
            Point drawSize = new Point(pendingTowerTemplate.SpriteWidth, pendingTowerTemplate.SpriteHeight);
            if (CursorIsOnMap()) { // Draw the tower so that it snaps to the hovered grid position.
                Point placementPos = GetAreaStartPoint();

                //Draw the tower to snap to the selected tiles
                Tower projectedTower = new Tower(pendingTowerTemplate, placementPos);
                spriteBatch.Draw(pendingTowerTemplate.Sprite, new Rectangle(projectedTower.DrawPos, drawSize), Color.White);

                //TODO: Check if the destination of this tower is obstructed, and change the tint accordingly

                // Draw the firing range of this tower's projected position.
                projectedTower.DrawFiringRange(spriteBatch);

            } else { // Draw the tower so that it follows the cursor
                Point drawPos = new Point(mouseState.X, mouseState.Y) - new Point(pendingTowerTemplate.SpriteWidth  / 2, (2 * pendingTowerTemplate.SpriteHeight) / 3);
                spriteBatch.Draw(pendingTowerTemplate.Sprite, new Rectangle(drawPos, drawSize), Color.White);
            }
            
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
            return 0 < mouseState.X && mouseState.X < (screenWidth - menuPanelWidth) && mouseState.Y > 0 && mouseState.Y < Settings.ViewportColumnLength * Settings.TileHeight;
        }

        /// <summary>
        /// Draw the menu panel to the correct position on the screen.
        /// </summary>
        protected void DrawMenuPanel() {
            int menuPanelX = screenWidth - menuPanelWidth;
            spriteBatch.Draw(Art.MenuPanel, new Rectangle(menuPanelX, 0, menuPanelWidth, menuPanelHeight), Color.White);

            /* Draw buttons for the panel. */
            foreach(Button b in buttons) {
                b.Draw(spriteBatch);
            }

        }

        /// <summary>
        /// Draw the grid dividing tiles.
        /// </summary>
        protected void DrawGrid() {
            // Draw horizontal lines across the screen at each tile height
            for(int i = 0; i < screenHeight; i += Settings.TileHeight) {
                Graphics.DrawLine(spriteBatch, 0, i, screenWidth - menuPanelWidth, 1, Color.Black);
            }

            // Draw vertical lines across the screen at each tile width
            for (int j = 0; j < screenWidth - menuPanelWidth; j += Settings.TileWidth) {
                Graphics.DrawLine(spriteBatch, j, 0, 1, screenHeight, Color.Black);
            }
        }

        /// <summary>
        /// Draw the grid of tiles and their colorations.
        /// </summary>
        protected void DrawMap() {
            // Shade in the tiles based on their type.
            for (int y = 0; y < Settings.ViewportColumnLength; y++) {
                for(int x = 0; x < Settings.ViewportRowLength; x++) {
                    if(Map(x, y).Type == TileType.LIMITED) {
                        DrawTile(x, y, Color.DarkOliveGreen);
                    } else if(Map(x, y).Type == TileType.OPEN) {
                        DrawTile(x, y, Color.SandyBrown);
                    }
                }
            }

            // If the player is currently in placement mode, highlight the selected tiles.
            if (isPlacingTower && CursorIsOnMap()) {
                Point pos = GetAreaStartPoint();
                for (int y = pos.Y; y < pos.Y + pendingTowerTemplate.Height; y++) {
                    for (int x = pos.X; x < pos.X + pendingTowerTemplate.Width; x++) {
                        if (Map(x, y).ObstructsTower()) {
                            DrawTile(x, y, Color.Red);
                        } else {
                            DrawTile(x, y, Color.Green);
                        }
                    }
                }
            }
            // Overlay grid
            DrawGrid();
        }

        protected void DrawTile(int x, int y, Color color) {
            spriteBatch.Draw(Art.Pixel, new Rectangle(x * Settings.TileWidth, y * Settings.TileHeight, Settings.TileWidth, Settings.TileHeight), color);
        }

        /// <summary>
        /// Load in the next map.
        /// </summary>
        protected void LoadMap() {
            // Fill in the game map with limited tiles.
            for(int y = 0; y < Settings.ViewportColumnLength; y++) {
                for(int x = 0; x < Settings.ViewportRowLength; x++) {
                    map[y, x] = new Tile(TileType.LIMITED, x, y);
                }
            }

            // Draw a horizontal parabola-shaped pathway
            for (int x = 0; x < Settings.ViewportRowLength; x++) {
                int fx = (int)Math.Sqrt(x / 3) + 1;

                for(int y = -1; y < 2; y++) {
                    Map(x, Settings.ViewportColumnLength / 2 + fx + y).Type = TileType.OPEN;
                }
                for(int y = -1; y < 2; y++) {
                    Map(x, Settings.ViewportColumnLength / 2 - fx - y).Type = TileType.OPEN;
                }
            }

            // Draw a horizontal line through the center
            for (int y = 0; y < Settings.ViewportColumnLength; y++) {
                for (int x = -1; x < 2; x++) {
                    Map(Settings.ViewportRowLength / 2 + x, y).Type = TileType.OPEN;
                }
            }

            // Draw a 5x5 square in the center
            for(int y = -2; y <= 2; y++) {
                for(int x = -2; x <= 2; x++) {
                    Map(Settings.ViewportRowLength / 2 + x, Settings.ViewportColumnLength / 2 + y).Type = TileType.OPEN;
                }
            }

            // Place hub in the center
            hub = new Tower(Globals.HubTemplate, new Point(Settings.ViewportRowLength / 2 - 1, Settings.ViewportColumnLength / 2 - 1));
            AddTower(hub);

            currentWave = 0;

            SpawnWave();
        }

        public void SpawnWave() {
            currentWave++;
            Random r = new Random();
            int spawnAmt = (currentWave - 1) * 5 + 10 + r.Next(0, 5);

            // Get the set of valid tiles -- OPEN tiles on the boundary of the screen.
            List<Tile> spawnTiles = new List<Tile>();
            for(int i = 0; i < Math.Max(Settings.ViewportRowLength, Settings.ViewportRowLength); i++) {
                if (i < Settings.ViewportRowLength) {
                    if (Map(i, 0).Type == TileType.OPEN) {
                        spawnTiles.Add(Map(i, 0));
                    }
                    if (Map(i, Settings.ViewportColumnLength - 1).Type == TileType.OPEN) {
                        spawnTiles.Add(Map(i, Settings.ViewportColumnLength - 1));
                    }
                }

                if (i < Settings.ViewportColumnLength) {
                    if (Map(0, i).Type == TileType.OPEN) {
                        spawnTiles.Add(Map(0, i));
                    }
                    if (Map(Settings.ViewportRowLength - 1, i).Type == TileType.OPEN) {
                        spawnTiles.Add(Map(Settings.ViewportRowLength - 1, i));
                    }
                }
            }

            // Spawn each enemy at a random tile.
            for(int i = 0; i < spawnAmt; i++) {
                Tile spawnTile = spawnTiles[r.Next(0, spawnTiles.Count - 1)];
                AddMonster(new Monster(Art.Imp, MonsterType.IMP, spawnTile.Pos, Globals.GetClosestTilePos(spawnTile.Pos, TowerType.HUB, towers), 10, map));
            }
        }

        /// <summary>
        /// Returns the tile at the given coordianates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>map[y, x]</returns>
        private Tile Map(int x, int y) {
            return map[y, x];
        }
    }
}
