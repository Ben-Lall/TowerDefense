﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense {
    /// <summary>
    /// A tower gameplay object.
    /// </summary>
    class Tower {
        /// <summary>
        /// The template used to build this tower.
        /// </summary>
        private TowerTemplate template;

        /// <summary>
        /// The tile coordinates of the top-left corner of the base of this tower.
        /// </summary>
        private Point pos;

        /// <summary>
        /// Boolean representing whether or not this tower has been selected.
        /// </summary>
        private bool selected;

        /// <summary>
        /// The width of the base of this tower, measured in units of tiles.
        /// </summary>
        private int width;

        /// <summary>
        /// The height of the base of this tower, measured in units of tiles.
        /// </summary>
        private int height;

        /// <summary>
        /// Constructor for a Tower, using a TowerTemplate
        /// </summary>
        /// <param name="template">Template used to construct this tower.</param>
        /// <param name="pos">Coordinate position of the top-left corner of this tower, in units of tiles.</param>
        public Tower(TowerTemplate template, Point pos) {
            Template = template;
            Pos = pos;
            Width = template.Width;
            Height = template.Height;
        }

        /// <summary>
        /// Draw this tower to its current position on the screen.
        /// </summary>
        /// <param name="spritebatch"></param>
        public void Draw(SpriteBatch spriteBatch) {
            if (Selected) {
                Illuminate(spriteBatch);
            }
            spriteBatch.Draw(Sprite, new Rectangle(DrawPos, new Point(SpriteWidth, SpriteHeight)), Color.White);
        }

        /// <summary>
        /// Illuminate this tower, giving it a glowing aura and lighting up the grid area it stands on.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Illuminate(SpriteBatch spriteBatch) {
            // Draw encompassing grid area.
            Drawing.DrawLine(spriteBatch, X * Settings.TileWidth, Y * Settings.TileHeight, Width * Settings.TileWidth, 1, Color.Green);
            Drawing.DrawLine(spriteBatch, X * Settings.TileWidth, (Y + Height) * Settings.TileHeight, Width * Settings.TileWidth, 1, Color.Green);
            Drawing.DrawLine(spriteBatch, X * Settings.TileWidth, Y * Settings.TileHeight, 1, Height * Settings.TileHeight, Color.Green);
            Drawing.DrawLine(spriteBatch, (X + Width) * Settings.TileWidth, Y * Settings.TileHeight, 1, Height * Settings.TileHeight, Color.Green);

            //TODO: add aura
        }

        /// <summary>
        /// Draw the firing range of this tower.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawFiringRange(SpriteBatch spriteBatch) {
            Drawing.DrawCircle(spriteBatch, Settings.TileWidth * (X + Width / 2), Settings.TileHeight * (Y + Height / 2), (int)(FireRadius * Settings.TileWidth));
        }

        /// <summary>
        /// Determine if this tower is placed on top of the given tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns>Return true if this tower overlaps the given tile. False otherwise.</returns>
        public bool ContainsTile(Tile tile) {
            return (X <= tile.X && tile.X <= X + Width && Y <= tile.Y && tile.Y <= Y + Height);
        }

        /// <summary>
        /// Determine if this tower is placed on top of the given point.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>Return true if this tower overlaps the given point. False otherwise.</returns>
        public bool ContainsTile(Point p) {
            return (X <= p.X && p.X < X + Width && Y <= p.Y && p.Y < Y + Height);
        }

        /* Setters and Getters */
        public Point Pos { get => pos; set => pos = value; }
        public int X { get => Pos.X; set => pos.X = value; }
        public int Y { get => Pos.Y; set => pos.Y = value; }
        public Point DrawPos { get => new Point(Pos.X * Settings.TileWidth - (SpriteWidth - Width * Settings.TileWidth) / 2, (Pos.Y * Settings.TileHeight) - SpriteHeight + Settings.TileHeight * Height); }
        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public TowerTemplate Template { get => template; set => template = value; }
        public int Damage { get => Template.Damage; set => Template.Damage = value; }
        public double FireRate { get => Template.FireRate; set => Template.FireRate = value; }
        public double FireRadius { get => Template.FireRadius; set => Template.FireRadius = value; }
        public Texture2D Sprite { get => template.Sprite; set => template.Sprite = value; }
        public TowerType Type { get => template.Type; set => template.Type = value; }
        public int SpriteWidth { get => Sprite.Width; }
        public int SpriteHeight { get => Sprite.Height; }
        public bool Selected { get => selected; set => selected = value; }
    }
}
