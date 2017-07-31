using System;
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

        public void Draw(SpriteBatch spritebatch) {
            spritebatch.Draw(Sprite, new Rectangle(DrawPos, new Point(SpriteWidth, SpriteHeight)), Color.White);
        }


        /* Setters and Getters */
        public Point Pos { get => pos; set => pos = value; }
        public int X { get => Pos.X; set => pos.X = value; }
        public int Y { get => Pos.Y; set => pos.Y = value; }
        public Point DrawPos { get => new Point(Pos.X * Settings.TileWidth - (SpriteWidth - Width * Settings.TileWidth) / 2, (Pos.Y * Settings.TileHeight) - SpriteHeight + Settings.TileHeight * Height); }
        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public TowerTemplate Template { get => template; set => template = value; }
        public Texture2D Sprite { get => template.Sprite; set => template.Sprite = value; }
        public TowerType Type { get => template.Type; set => template.Type = value; }
        public int SpriteWidth { get => Sprite.Width; }
        public int SpriteHeight { get => Sprite.Height; }
    }
}
