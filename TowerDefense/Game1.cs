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
            // Create a new SpriteBatch, which can be used to draw textures.
            Sprites = new SpriteBatch(GraphicsDevice);
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
            }

            Input.HandleInput();

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
            // Remove dead towers from the list.
            Towers.RemoveAll(x => !x.IsAlive);
            DrawSet.RemoveAll(x => x.GetType() == typeof(Tower) && !((Tower)x).IsAlive);

            foreach (Tower t in Towers) {
                t.Update(gameTime, Monsters);
            }
        }

        /// <summary>
        /// Update Monsters.  For now, this only entails movement.
        /// </summary>
        private void UpdateMonsters(GameTime gameTime) {
            // Remove dead monsters from the list.
            Monsters.RemoveAll(x => !x.IsAlive);
            DrawSet.RemoveAll(x => x.GetType() == typeof(Monster) && !((Monster)x).IsAlive);
            
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
            Sprites.Begin();

            /* Draw gameplay elements */

            DrawMap();
            DrawGameplayObjects();

            /* Draw UI elements */

            DrawUI();

            if (IsPlacingTower) {
                DrawPendingTower();
            }

            DrawDebug();

            // Change spriteBatch into the mode for drawing effects.
            Sprites.End();
            Sprites.Begin(SpriteSortMode.Texture, MaxBlend);
            foreach (Bolt e in Effects) { // TODO: replace with usage of DrawSet in DrawGameplayObjects()
                e.Draw(Sprites);
            }

            Sprites.End();
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
                Sprites.DrawString(Art.Font, PauseText, PausePosition, Color.Black);
            }

            String monsterText = "Monsters: " + Monsters.Count;
            Vector2 monsterTextSize = Art.Font.MeasureString(monsterText);
            Vector2 monsterTextPosition = new Vector2(ScreenWidth - 5, PausePosition.Y) - monsterTextSize;
            Sprites.DrawString(Art.Font, monsterText, monsterTextPosition, Color.Black);
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
                hoverTileText = "Hover: (" + PixelToTile(Include.Globals.MouseState.Position).X + ", " + PixelToTile(Include.Globals.MouseState.Position).Y + ")";
                hoverTextSize = Art.Font.MeasureString(hoverTileText);
                Sprites.DrawString(Art.Font, hoverTileText, new Vector2(ScreenWidth - 5, ScreenHeight - 5) - hoverTextSize, Color.Black);
            }

            String viewportText = "Viewport: " + ViewportX + ", " + ViewportY + ")";
            Vector2 viewportTextSize = Art.Font.MeasureString(viewportText);
            Sprites.DrawString(Art.Font, viewportText, new Vector2(ScreenWidth - 5, hoverTileText.Length == 0 ? ScreenHeight - 5 : ScreenHeight - 5 - hoverTextSize.Y) - viewportTextSize, Color.Black);
        }

        /// <summary>
        /// Draw each element in DrawSet.
        /// </summary>
        private void DrawGameplayObjects() {
            // Draw towers / creatures in the proper order.
            foreach (object obj in DrawSet) {
                if (IsOnScreen(obj)) {
                    if (obj.GetType() == typeof(Tower)) {
                        ((Tower)obj).Draw(Sprites);
                    } else if (obj.GetType() == typeof(Monster)) {
                        ((Monster)obj).Draw(Sprites);
                    }
                }
            }

            // Draw firing ranges of all selected towers
            foreach (Tower t in Towers) {
                if (t.Selected) {
                    t.DrawFiringRange(Sprites);
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
                projectedTower.Draw(Sprites);

                //TODO: Check if the destination of this tower is obstructed, and change the tint accordingly

                // Draw the firing range of this tower's projected position.
                projectedTower.DrawFiringRange(Sprites);

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
                a = ((Tower)o).PixelPos;
                b = new Point(((Tower)o).SpriteWidth, ((Tower)o).SpriteHeight);
            } else if (o.GetType() == typeof(Monster)) {
                a = ((Monster)o).Pos;
                b = new Point(((Monster)o).SpriteWidth, ((Monster)o).SpriteHeight);
            }
            return new Rectangle(ViewportPx, ViewPortDimensionsPx).Intersects(new Rectangle(a, b));
        }

        /// <summary>
        /// Draw the menu panel to the correct position on the screen.
        /// </summary>
        protected void DrawMenuPanel() {
            int menuPanelX = ScreenWidth - MenuPanelWidth;
            Sprites.Draw(Art.MenuPanel, new Rectangle(menuPanelX, 0, MenuPanelWidth, MenuPanelHeight), Color.White);

            /* Draw buttons for the panel. */
            foreach (Button b in Include.Globals.Buttons) {
                b.Draw(Sprites);
            }

        }

        /// <summary>
        /// Draw the grid dividing tiles.
        /// </summary>
        protected void DrawGrid() {
            // Draw horizontal lines across the screen at each tile height
            for (int i = 0; i < ScreenHeight; i += TileHeight) {
                Graphics.DrawLine(Sprites, 0, i, ScreenWidth, 1, Color.Black);
            }

            // Draw vertical lines across the screen at each tile width
            for (int j = 0; j < ScreenWidth; j += TileWidth) {
                Graphics.DrawLine(Sprites, j, 0, 1, ScreenHeight, Color.Black);
            }
        }

        /// <summary>
        /// Draw the grid of tiles and their colorations.
        /// </summary>
        protected void DrawMap() {
            // Shade in the tiles based on their type.
            for (int y = ViewportY; y < ViewCols + ViewportY; y++) {
                for (int x = ViewportX; x < ViewRows + ViewportX; x++) {
                    if (MapAt(x, y).Type == TileType.LIMITED) {
                        DrawTile(x, y, Color.DarkOliveGreen);
                    } else if (MapAt(x, y).Type == TileType.OPEN) {
                        DrawTile(x, y, Color.SandyBrown);
                    }
                }
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
            Sprites.Draw(Art.Pixel, new Rectangle((x * TileWidth) - ViewportPx.X, (y * TileHeight) - ViewportPx.Y, TileWidth, TileHeight), color);
        }

        /// <summary>
        /// Load in the next map.
        /// </summary>
        protected void LoadMap() {
            // Fill in the game map with limited tiles.
            for (int y = 0; y < MapHeight; y++) {
                for (int x = 0; x < MapWidth; x++) {
                    Map[y, x] = new Tile(TileType.LIMITED, x, y);
                }
            }

            // Draw a horizontal parabola-shaped pathway
            for (int x = 0; x < MapWidth; x++) {
                int fx = (int)Math.Sqrt(x / 3) + 1;

                for (int y = -1; y < 2; y++) {
                    MapAt(x, MapHeight / 2 + fx + y).Type = TileType.OPEN;
                }
                for (int y = -1; y < 2; y++) {
                    MapAt(x, MapHeight / 2 - fx - y).Type = TileType.OPEN;
                }
            }

            // Draw a horizontal line through the center
            for (int y = 0; y < MapHeight; y++) {
                for (int x = -1; x < 2; x++) {
                    MapAt(MapWidth / 2 + x, y).Type = TileType.OPEN;
                }
            }

            // Draw a 5x5 square in the center
            for (int y = -2; y <= 2; y++) {
                for (int x = -2; x <= 2; x++) {
                    MapAt(MapWidth / 2 + x, MapHeight / 2 + y).Type = TileType.OPEN;
                }
            }

            // Place hub in the center
            AddTower(new Tower(HubTemplate, new Point(MapWidth / 2 - 1, MapHeight / 2 - 1)));

            for (int y = -2; y <= 2; y++) {
                for (int x = -2; x <= 2; x++) {
                    MapAt(MapWidth / 2 + x + 4, MapHeight / 2 + y - 9).Type = TileType.OPEN;
                }
            }
            AddTower(new Tower(HubTemplate, new Point(53, 40)));

            CurrentWave = 0;

            SpawnWave();
        }

        public void SpawnWave() {
            CurrentWave++;
            Random r = new Random();
            int spawnAmt = (CurrentWave - 1) * 5 + 10 + r.Next(0, 5);

            // Get the set of valid tiles -- OPEN tiles on the boundary of the screen.
            List<Tile> spawnTiles = new List<Tile>();
            for (int i = 0; i < Math.Max(MapHeight, MapWidth); i++) {
                if (i < MapWidth) {
                    if (MapAt(i, 0).Type == TileType.OPEN) {
                        spawnTiles.Add(MapAt(i, 0));
                    }
                    if (MapAt(i, MapHeight - 1).Type == TileType.OPEN) {
                        spawnTiles.Add(MapAt(i, MapHeight - 1));
                    }
                }

                if (i < MapHeight) {
                    if (MapAt(0, i).Type == TileType.OPEN) {
                        spawnTiles.Add(MapAt(0, i));
                    }
                    if (MapAt(MapWidth - 1, i).Type == TileType.OPEN) {
                        spawnTiles.Add(MapAt(MapWidth - 1, i));
                    }
                }
            }

            // Spawn each enemy at a random tile.
            for (int i = 0; i < spawnAmt; i++) {
                Tile spawnTile = spawnTiles[r.Next(0, spawnTiles.Count - 1)];
                AddMonster(new Monster(Art.Imp, MonsterType.IMP, spawnTile.Pos, 10));
            }
        }
    }
}
