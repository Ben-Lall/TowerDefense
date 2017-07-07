﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TowerDefense {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

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
        /// List of towers unlocked by the player
        /// </summary>
        List<Tower> ulTowers;

        /* UI */

        /// <summary>
        /// List of rectangles, where each rectangle represents the hit area of a menu button.
        /// </summary>
        List<Button> buttons;

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

            // Set the menu panel's height and width
            menuPanelWidth = screenWidth / 8;
            menuPanelHeight = screenHeight;

            // Set the tile dimensions to 16px.  16 is a common factor of 720 and 1120, 1120 = 1280 * (7/8)
            Settings.TileWidth = 16;
            Settings.TileHeight = 16;

            // Set the number of tiles viewable on the screen, since the menu panel will obstruct the map slightly
            Settings.ViewportRowLength = (screenWidth - menuPanelWidth) / Settings.TileWidth;
            Settings.ViewportColumnLength = screenHeight / Settings.TileHeight;

            // Initialize the gameplay objects
            map = new Tile[Settings.ViewportColumnLength, Settings.ViewportRowLength];

            // Initialize list of unlocked towers with the base bolt tower
            ulTowers = new List<Tower>();
            ulTowers.Add(new Tower(TowerType.BOLT, tower));
            ulTowers.Add(new Tower(TowerType.BOLT, tower));
            ulTowers.Add(new Tower(TowerType.BOLT, tower));
            ulTowers.Add(new Tower(TowerType.BOLT, tower));
            ulTowers.Add(new Tower(TowerType.BOLT, tower));
            ulTowers.Add(new Tower(TowerType.BOLT, tower));

            buttons = new List<Button>();
            
            // Create sprites for drawing
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
            // TEMP: load map (since ideally there'd be a title screen)
            loadMap();

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

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // If a tower has been added/removed from the list, refresh the buttons list
            if(buttons.Count != ulTowers.Count) {
                buttons.Clear();
                // Add a button for each tower in the list
                for(int i = 0; i < ulTowers.Count; i++) {
                    Rectangle buttonBox = new Rectangle(screenWidth - menuPanelWidth + (menuPanelWidth / 4), (i * menuPanelHeight / 12) + (5 * i) + 5,
                                                        menuPanelWidth / 2, menuPanelHeight / 12);
                    buttons.Add(new Button(buttonBox, towerButton, tower, null));
                }
            }

            MouseState newState = Mouse.GetState();

            // Find the tile currently hovered over

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            /* Draw gameplay elements */

            drawTiles();

            /* Draw UI elements */

            drawMenuPanel();

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Draw the menu panel to the correct position on the screen.
        /// </summary>
        protected void drawMenuPanel() {
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
        protected void drawLine(int x, int y, int width, int height, Color color) {
            spriteBatch.Draw(pixel, new Rectangle(x, y, width, height), color);
        }

        /// <summary>
        /// Draw the grid dividing tiles.
        /// </summary>
        protected void drawGrid() {
            // Draw horizontal lines across the screen at each tile height
            for(int i = 0; i < screenHeight; i += Settings.TileHeight) {
                drawLine(0, i, screenWidth - menuPanelWidth, 1, Color.Black);
            }

            // Draw vertical lines across the screen at each tile width
            for (int j = 0; j < screenWidth - menuPanelWidth; j += Settings.TileWidth) {
                drawLine(j, 0, 1, screenHeight, Color.Black);
            }
        }

        /// <summary>
        /// Draw the grid of tiles and their colorations.
        /// </summary>
        protected void drawTiles() {
            
            //Shade in the limited tiles.
            for (int i = 0; i < Settings.ViewportColumnLength; i++) {
                for(int j = 0; j < Settings.ViewportRowLength; j++) {
                    if(map[i,j].Type == TileType.LIMITED) {
                        spriteBatch.Draw(pixel, new Rectangle(j * Settings.TileWidth, i * Settings.TileHeight, Settings.TileWidth, Settings.TileHeight), Color.Gray);
                    } else if(map[i, j].Type == TileType.OPEN) {
                        spriteBatch.Draw(pixel, new Rectangle(j * Settings.TileWidth, i * Settings.TileHeight, Settings.TileWidth, Settings.TileHeight), Color.White);
                    }
                }
            }

            // Overlay grid
            drawGrid();
        }

        /// <summary>
        /// Load in the next map.
        /// </summary>
        protected void loadMap() {
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
