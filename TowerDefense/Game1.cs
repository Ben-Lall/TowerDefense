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
            // Set tile size to be 32x32
            TileWidth = 32;
            TileHeight = 32;
            base.Initialize();
            InitializeGlobals(Window);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Initialize SpriteBatches
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
            SortCollections();
            if (!Paused) {
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
        /// Sort all collections.  
        /// </summary>
        private void SortCollections() {
            DrawSet.Sort(DrawComparer);
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
            foreach (Tower t in Towers) {
                if (t.IsAlive) {
                    t.Update(gameTime);
                } else {
                    t.Remove();
                }
            }

            // Remove dead towers from the lists.
            Towers.RemoveAll(x => !x.IsAlive);
            DrawSet.RemoveAll(x => x.GetType() == typeof(Tower) && !((Tower)x).IsAlive);
        }

        /// <summary>
        /// Update Monsters.
        /// </summary>
        private void UpdateMonsters(GameTime gameTime) {
            // Remove dead monsters and out of bounds monsters.
            Monsters.RemoveAll(x => !x.IsAlive || !x.IsSimulated);
            DrawSet.RemoveAll(x => x.GetType() == typeof(Monster) && (!((Monster)x).IsAlive || !((Monster)x).IsSimulated));

            // Spawn new monsters if the cooldown has ended.
            if (SpawnCooldown == 0) {
                WorldMap.SpawnWave();
                SpawnCooldown = SpawnRate;
            } else {
                SpawnCooldown = Math.Max(0, SpawnCooldown - gameTime.ElapsedGameTime.TotalSeconds);
            }
            
            foreach (Monster m in Monsters) {
                m.Update(gameTime);
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
                    null, SamplerState.PointClamp, null, null, null, Camera.GetTransformation());

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
            ActivePlayer.DrawUI();
            DrawDebug();
            UISpriteBatch.End();

            base.Draw(gameTime);
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

            String viewportText = "Pos: " + ActivePlayer.CenterPoint.X + ", " + ActivePlayer.CenterPoint.Y + ")";
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
        public bool IsOnScreen(GameplayObject g) {
            Point a = g.Pos;
            Point b = new Point(g.SpriteWidth, g.SpriteHeight);
            return new Rectangle(Camera.Pos, new Point(ScreenWidth, ScreenHeight)).Intersects(new Rectangle(a, b));
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
                WorldMap.Draw();
            } else if (TileMode == Include.TileDrawMode.HEATMAP) {
                HeatMap.Draw();
            } else if(TileMode == Include.TileDrawMode.HEATMAP_NUMBERS) {
                HeatMap.Draw(true);
            }

            // If the player is currently in placement mode, highlight the selected tiles.
            if (IsPlacingTower && CursorIsOnMap()) {
                Point pos = GetAreaStartPoint();
                for (int y = pos.Y; y < pos.Y + PendingTowerTemplate.Height; y++) {
                    for (int x = pos.X; x < pos.X + PendingTowerTemplate.Width; x++) {
                        if (WorldMap.At(x, y).ObstructsTower()) {
                            WorldMap.At(x, y).Draw(Color.Red);
                        } else {
                            WorldMap.At(x, y).Draw(Color.Green);
                        }
                    }
                }
            }
            // Overlay grid
            if (GridToggle) {
                DrawGrid();
            }
        }
    }
}
