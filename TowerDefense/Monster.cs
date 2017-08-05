﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Enum containing the different types of monster.
/// </summary>
enum MonsterType { IMP, NUMBER_OF_MONSTERS }

namespace TowerDefense {
    /// <summary>
    /// A monster in the game world.
    /// </summary>
    class Monster {
        /// <summary>
        /// The sprite of this monster.
        /// </summary>
        public Texture2D Sprite { get; set; }

        public int SpriteWidth { get => Sprite.Width; }
        public int SpriteHeight { get => Sprite.Height; }

        /// <summary>
        /// Offset used to convert Pos when constructing from a tile coordinate.
        /// </summary>
        private Point TileToPointOffset { get => new Point(Settings.TileWidth / 2 - SpriteWidth / 2, Settings.TileHeight / 2 - SpriteHeight / 2); }

        /// <summary>
        /// The type of monster that this is.
        /// </summary>
        public MonsterType Type { get; set; }

        /// <summary>
        /// Coordinate position of the top-left pixel of this monster's sprite.
        /// </summary>
        public Point Pos { get; set; }

        public int X { get => Pos.X; set => Pos = new Point(value, Y); }
        public int Y { get => Pos.Y; set => Pos = new Point(X, value); }

        /// <summary>
        /// Center pixel of this monster.
        /// </summary>
        public Point CenterPoint { get => (Pos + new Point(SpriteWidth / 2, SpriteHeight / 2)); }

        /// <summary>
        /// This creature's maximum possible health.
        /// </summary>
        public int MaxHealth { get; set; }

        /// <summary>
        /// Integer representing this creature's current health.  Should always be > 0.
        /// </summary>
        public int CurrentHealth { get; set; }

        /// <summary>
        /// Whether or not this monster is alive.
        /// </summary>
        public bool IsAlive { get => CurrentHealth > 0; }

        /// <summary>
        /// This creature's speed, at a rate of tiles / second.
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// Pathfinder for this monster.
        /// </summary>
        private Pathfinder pf;

        /// <summary>
        /// Constructor for a new Monster.
        /// </summary>
        /// <param name="sprite">Sprite for this monster.</param>
        /// <param name="type">Type of monster.</param>
        /// <param name="pos">The tile position of this monster.</param>
        /// <param name="target">The target for this monster to reach.</param>
        /// <param name="maxHealth">The maximum health of this monster.</param>
        /// <param name="map">The world map.</param>
        public Monster(Texture2D sprite, MonsterType type, Point pos, Point target, int maxHealth, Tile[,] map) {
            Sprite = sprite;
            Type = type;
            Pos = new Point(pos.X * Settings.TileWidth, pos.Y * Settings.TileHeight) + TileToPointOffset;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            pf = new Pathfinder(pos, target, map);
            Speed = 10;
        }
        
        /// <summary>
        /// Draw the monster at its current position.
        /// </summary>
        /// <param name="spritebatch"></param>
        public void Draw(SpriteBatch spritebatch) {
            spritebatch.Draw(Sprite, new Rectangle(Pos - Globals.ViewportPx, new Point(SpriteWidth, SpriteHeight)), Color.White);
        }

        /// <summary>
        /// Get the distance between this monster and its target, measured in units of tiles.
        /// </summary>
        /// <returns></returns>
        public int DistanceToTarget() {
            return pf.Path.Count;
        }

        public void TakeDamage(int damage) {
            CurrentHealth = Math.Max(0, CurrentHealth - damage);
        }

        /// <summary>
        /// Draw the path this monster is currently on.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawPath(SpriteBatch spriteBatch) {
            foreach (Tile t in pf.Path) {
                spriteBatch.Draw(Art.Pixel, new Rectangle(t.X * Settings.TileWidth, t.Y * Settings.TileHeight, Settings.TileWidth, Settings.TileHeight), Color.Crimson * 0.5f);
            }
        }

        /// <summary>
        /// Move this monster towards its next tile.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Move(GameTime gameTime) {
            if (pf.Path.Count > 0) {
                Point nextTileCoord = pf.Path.First().Pos;
                Point nextTilePos = new Point(nextTileCoord.X * Settings.TileWidth, nextTileCoord.Y * Settings.TileHeight) + TileToPointOffset;
                Vector2 dirVector = Vector2.Normalize((nextTilePos - Pos).ToVector2());
                Pos += new Point(
                    (int)(dirVector.X * gameTime.ElapsedGameTime.TotalSeconds * Speed * Settings.TileWidth),
                    (int)(dirVector.Y * gameTime.ElapsedGameTime.TotalSeconds * Speed * Settings.TileHeight));

                // Remove this tile from the path if it has been reached
                if (nextTilePos - Pos == new Point(0, 0)) {
                    pf.Path.RemoveFirst();
                }
            }
        }
    }
}
