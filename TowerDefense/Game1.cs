using static Include.Globals;
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

        public Game1() {
            Include.Globals.Graphics = new GraphicsDeviceManager(this);
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

            InitializeGlobals(Window);
            // TEMP: load map (since ideally there'd be a title screen).
            LoadMap();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            /// Initialize SpriteBatches
            WorldSpriteBatch = new SpriteBatch(GraphicsDevice);
            UISpriteBatch = new SpriteBatch(GraphicsDevice);
            BoltSpriteBatch = new SpriteBatch(GraphicsDevice);

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
            Include.Globals.MouseState = Mouse.GetState();
            DrawSet.Sort(DrawComparer);
            if (!Paused) {
                // If a tower has been added/removed from the list, refresh the buttons list
                if (Include.Globals.Buttons.Count != UlTowers.Count) {
                    RefreshButtonsList();
                }

                UpdateTowers(gameTime);
                UpdateMonsters(gameTime);
                UpdateEffects(gameTime);
                ActivePlayer.Update(gameTime);
            }

            if (IsActive) {
                Input.HandleInput();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Update effects.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateEffects(GameTime gameTime) {
            Effects.RemoveAll(x => x.IsComplete);
            foreach (Bolt e in Effects) {
                e.Update(gameTime);
            }
        }

        /// <summary>
        /// Update towers. This calls each tower and has them execute their automatic abilities.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateTowers(GameTime gameTime) {
            // Remove dead towers and update living ones.
            bool removed = false;
            foreach (Tower t in Towers) {
                if (t.IsAlive) {
                    t.Update(gameTime);
                } else {
                    t.Remove();
                    removed = true;
                }
            }

            // Remove dead towers from the lists.
            Towers.RemoveAll(x => !x.IsAlive);
            DrawSet.RemoveAll(x => x.GetType() == typeof(Tower) && !((Tower)x).IsAlive);
            if(removed) {
                HeatMap.Update();
            }
        }

        /// <summary>
        /// Update Monsters.  For now, this only entails movement.
        /// </summary>
        private void UpdateMonsters(GameTime gameTime) {
            // Remove dead monsters from the list.
            Monsters.RemoveAll(x => !x.IsAlive);
            DrawSet.RemoveAll(x => x.GetType() == typeof(Monster) && !((Monster)x).IsAlive);

            // Spawn new monsters if the cooldown has ended.
            if(SpawnCooldown == 0) {
                SpawnWave();
                SpawnCooldown = SpawnRate;
            } else {
                SpawnCooldown = Math.Max(0, SpawnCooldown - gameTime.ElapsedGameTime.TotalSeconds);
            }
            
            foreach (Monster m in Monsters) {
                m.Update(gameTime);
            }
        }

        /// <summary>
        /// Refresh the list of buttons to reflect the list of currently unlocked towers.
        /// </summary>
        private void RefreshButtonsList() {
            Include.Globals.Buttons.Clear();
            // Add a button for each tower in the list
            for (int i = 0; i < UlTowers.Count; i++) {
                Rectangle buttonBox = new Rectangle(ScreenWidth - MenuPanelWidth + (MenuPanelWidth / 4), (i * MenuPanelHeight / 12) + (5 * i) + 5,
                                                    MenuPanelWidth / 2, MenuPanelHeight / 12);
                Include.Globals.Buttons.Add(new Button(buttonBox, Art.TowerButton, Art.Tower, null));
            }
        }

        /* Graphics functions */

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Blue);

            /* Draw world elements */

            WorldSpriteBatch.Begin(SpriteSortMode.Deferred,
                    null, null, null, null, null, Camera.GetTransformation());

            DrawMap();
            double time = gameTime.ElapsedGameTime.TotalSeconds;
            DrawGameplayObjects();

            if (IsPlacingTower) {
                DrawPendingTower();
            }
            WorldSpriteBatch.End();

            /* Draw effect elements */
            BoltSpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Camera.Transform);
            foreach (Bolt e in Effects) {
                e.Draw(BoltSpriteBatch);
            }
            BoltSpriteBatch.End();

            /* Draw UI elements */
            UISpriteBatch.Begin();
            DrawUI();
            DrawDebug();
            UISpriteBatch.End();


            base.Draw(gameTime);
        }

        /// <summary>
        /// Draw the UI to the screen.
        /// </summary>
        private void DrawUI() {
            if (!IsPlacingTower) {
                DrawMenuPanel();
            }
            DrawUIText();
        }

        /// <summary>
        /// Draw the UI text to the screen.
        /// </summary>
        private void DrawUIText() {
            String PauseText = "Paused!";
            Vector2 PauseTextSize = Art.Font.MeasureString(PauseText);
            Vector2 PausePosition = new Vector2(ScreenWidth - 5, ScreenHeight - 45) - PauseTextSize;
            if (Paused) {
                UISpriteBatch.DrawString(Art.Font, PauseText, PausePosition, Color.Black);
            }

            String monsterText = "Monsters: " + Monsters.Count;
            Vector2 monsterTextSize = Art.Font.MeasureString(monsterText);
            Vector2 monsterTextPosition = new Vector2(ScreenWidth - 5, PausePosition.Y) - monsterTextSize;
            UISpriteBatch.DrawString(Art.Font, monsterText, monsterTextPosition, Color.Black);
        }

        /// <summary>
        /// Draw debug information to the screen.
        /// </summary>
        private void DrawDebug() {
            String hoverTileText = "";
            Vector2 hoverTextSize = new Vector2();
            // Draw hover information
            if (CursorIsOnMap()) {
                // Draw currently hovered tile coordinates
                hoverTileText = "Hover: (" + PixelToTile(WorldMousePos.ToPoint()).X + ", " + PixelToTile(WorldMousePos.ToPoint()).Y + ")";
                hoverTextSize = Art.Font.MeasureString(hoverTileText);
                UISpriteBatch.DrawString(Art.Font, hoverTileText, new Vector2(ScreenWidth - 5, ScreenHeight - 5) - hoverTextSize, Color.Black);
            }

            String viewportText = "Camera: " + Camera.Pos.X + ", " + Camera.Pos.Y + ")";
            Vector2 viewportTextSize = Art.Font.MeasureString(viewportText);
            UISpriteBatch.DrawString(Art.Font, viewportText, new Vector2(ScreenWidth - 5, hoverTileText.Length == 0 ? ScreenHeight - 5 : ScreenHeight - 5 - hoverTextSize.Y) - viewportTextSize, Color.Black);
        }

        /// <summary>
        /// Draw each element in DrawSet.
        /// </summary>
        private void DrawGameplayObjects() {
            // Gameplay Objects in the proper order.
            foreach (GameplayObject g in DrawSet) {
                g.Draw();
            }

            // Draw firing ranges of all selected towers
            foreach (Tower t in Towers) {
                if (t.Selected) {
                    t.DrawAttackRange();
                }
            }
        }

        /// <summary>
        /// Draw a silhouette of the pending tower over the mouse's current location.  Draw it with a red tint if the target area is obstructed.
        /// </summary>
        protected void DrawPendingTower() {
            Point drawSize = new Point(PendingTowerTemplate.SpriteWidth, PendingTowerTemplate.SpriteHeight);
            if (CursorIsOnMap()) { // Draw the tower so that it snaps to the hovered grid position.
                Point placementPos = GetAreaStartPoint();

                //Draw the tower to snap to the selected tiles
                Tower projectedTower = new Tower(PendingTowerTemplate, placementPos);
                projectedTower.Draw();

                //TODO: Check if the destination of this tower is obstructed, and change the tint accordingly

                // Draw the firing range of this tower's projected position.
                projectedTower.DrawAttackRange();

            }
        }

        /// <summary>
        /// Checks if the given object is within the borders of the viewport.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        protected bool IsOnScreen(object o) {
            Point a = new Point();
            Point b = new Point();
            if (o.GetType() == typeof(Tower)) {
                a = ((Tower)o).Pos;
                b = new Point(((Tower)o).SpriteWidth, ((Tower)o).SpriteHeight);
            } else if (o.GetType() == typeof(Monster)) {
                a = ((Monster)o).Pos;
                b = new Point(((Monster)o).SpriteWidth, ((Monster)o).SpriteHeight);
            }
            return new Rectangle(Camera.Pos, new Point(ScreenWidth, ScreenHeight)).Intersects(new Rectangle(a, b));
        }

        /// <summary>
        /// Draw the menu panel to the correct position on the screen.
        /// </summary>
        protected void DrawMenuPanel() {
            int menuPanelX = ScreenWidth - MenuPanelWidth;
            UISpriteBatch.Draw(Art.MenuPanel, new Rectangle(menuPanelX, 0, MenuPanelWidth, MenuPanelHeight), Color.White);

            /* Draw buttons for the panel. */
            foreach (Button b in Include.Globals.Buttons) {
                b.Draw(UISpriteBatch);
            }

        }

        /// <summary>
        /// Draw the grid dividing tiles.
        /// </summary>
        protected void DrawGrid() {
            // Draw horizontal lines across the screen at each tile height
            for (int i = 0; i < MapHeight; i += 1) {
                Graphics.DrawLine(0, i * TileHeight, MapWidth * TileHeight, 1, Color.Black, WorldSpriteBatch);
            }

            // Draw vertical lines across the screen at each tile width
            for (int j = 0; j < MapWidth; j += 1) {
                Graphics.DrawLine(j * TileWidth, 0, 1, MapHeight * TileHeight, Color.Black, WorldSpriteBatch);
            }
        }

        /// <summary>
        /// Draw the grid of tiles and their colorations.
        /// </summary>
        protected void DrawMap() {
            // Shade in the tiles within the camera's viewport based on tile draw mode.
            if (TileMode == Include.TileDrawMode.DEFAULT) {
                for (int y = Camera.CameraTileStart.Y; y <= Camera.CameraTileEnd.Y; y++) {
                    for (int x = Camera.CameraTileStart.X; x <= Camera.CameraTileEnd.X; x++) {
                        if (MapAt(x, y).Type == TileType.LIMITED) {
                            DrawTile(x, y, Color.DarkOliveGreen);
                        } else if (MapAt(x, y).Type == TileType.OPEN) {
                            DrawTile(x, y, Color.SandyBrown);
                        }
                    }
                }
            } else if (TileMode == Include.TileDrawMode.HEATMAP) {
                HeatMap.Draw();
            }

            // If the player is currently in placement mode, highlight the selected tiles.
            if (IsPlacingTower && CursorIsOnMap()) {
                Point pos = GetAreaStartPoint();
                for (int y = pos.Y; y < pos.Y + PendingTowerTemplate.Height; y++) {
                    for (int x = pos.X; x < pos.X + PendingTowerTemplate.Width; x++) {
                        if (MapAt(x, y).ObstructsTower()) {
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

        /// <summary>
        /// Draw a tile to the game screen.
        /// </summary>
        /// <param name="x">x position.</param>
        /// <param name="y">y position.</param>
        /// <param name="color">The color</param>
        protected void DrawTile(int x, int y, Color color) {
            WorldSpriteBatch.Draw(Art.Pixel, new Rectangle((x * TileWidth), (y * TileHeight), TileWidth, TileHeight), color);
        }

        /// <summary>
        /// Load in the next map.
        /// </summary>
        protected void LoadMap() {
            // Fill in the game map with open tiles.
            for (int y = 0; y < MapHeight; y++) {
                for (int x = 0; x < MapWidth; x++) {
                    Map[y, x] = new Tile(TileType.OPEN, x, y);
                }
            }

            AddTower(new Tower(HubTemplate, new Point(500, 500)));
            HeatMap.Update();

            SpawnWave();
        }

        public void SpawnWave() {
            Random r = new Random();
            int spawnAmt = 25 + r.Next(5, 20);

            // Spawn each enemy at a random tile.
            for (int i = 0; i < spawnAmt; i++) {
                int x = -1;
                int y = -1;
                if (r.NextDouble() <= 0.5) {
                    if (r.NextDouble() <= 0.5) {
                        x = r.Next(Camera.SpawnLeftStart.X, Camera.SpawnLeftEnd.X);
                    }
                    if (x < 0) {
                        x = r.Next(Camera.SpawnRightStart.X, Camera.SpawnRightEnd.X);
                    }
                    if (x >= MapWidth * TileWidth) {
                        x = r.Next(Camera.SpawnLeftStart.X, Camera.SpawnLeftEnd.X);
                    }
                    y = r.Next(Camera.CameraStart.Y, Camera.CameraEnd.Y);
                } else {
                    if (r.NextDouble() <= 0.5) {
                        y = r.Next(Camera.SpawnLeftStart.Y, Camera.SpawnLeftEnd.Y);
                    }
                    if (y < 0) {
                        y = r.Next(Camera.SpawnRightStart.Y, Camera.SpawnRightEnd.Y);
                    }
                    if (y >= MapHeight * TileHeight) {
                        y = r.Next(Camera.SpawnLeftStart.Y, Camera.SpawnLeftEnd.Y);
                    }
                    x = r.Next(Camera.CameraStart.X, Camera.CameraEnd.X);
                }
                if (MapAt(x / TileWidth, y / TileHeight).IsEmpty()) {
                    AddMonster(new Monster(new CreatureSprite(Art.Imp), MonsterType.IMP, new Point(x, y)));
                }
            }
        }
    }
}
