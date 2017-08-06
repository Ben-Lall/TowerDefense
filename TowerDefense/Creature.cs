using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.Globals;

namespace TowerDefense
{
    /// <summary>
    /// Base abstract class for monsters and towers.
    /// </summary>
    abstract class Creature {
        /// <summary>
        /// Pixel coordinates of the creature.
        /// </summary>
        virtual public Point Pos { get; set; }

        virtual public int X { get => Pos.X; set => Pos = new Point(value, Y); }
        virtual public int Y { get => Pos.Y; set => Pos = new Point(X, value); }

        /// <summary>
        /// Pixel coordinates of the center of the creature.
        /// </summary>
        virtual public Point CenterPoint { get => Pos; }

        /// <summary>
        /// Pixel coordinates from which attack visual effects originate.
        /// </summary>
        virtual public Point FirePoint { get => CenterPoint; }

        /// <summary>
        /// Tile coordinates of the creature.
        /// </summary>
        virtual public Point TilePos { get => new Point(X / TileWidth, Y / TileHeight); set => Pos = new Point(value.X * TileWidth, value.Y * TileHeight); }

        public int TileX { get => TilePos.X; set => TilePos = new Point(value, TileY); }
        public int TileY { get => TilePos.Y; set => TilePos = new Point(TileX, value); }

        /// <summary>
        /// Tile coordinates of the center of the creature.
        /// </summary>
        virtual public Point CenterTile { get => TilePos; }

        /// <summary>
        /// Hitpoints of damage dealt by the monster's attacks.
        /// </summary>
        virtual public int AttackDamage { get; set; }

        /// <summary>
        /// Creature's attack range, measured in units of tileWidth.
        /// </summary>
        virtual public double AttackRange { get; set; }

        /// <summary>
        /// Creature's attack range, measured in units of pixels.
        /// </summary>
        public Point PixelRange { get => new Point((int)(AttackRange * TileWidth), (int)(AttackRange * TileHeight)); }

        /// <summary>
        /// Frequency of the creature's attacks, measured in hertz.
        /// </summary>
        virtual public double AttackRate { get; set; }

        /// <summary>
        /// Current time until the creature's next attack.
        /// </summary>
        public double Cooldown { get; set; }

        /// <summary>
        /// Creature's maximum possible health.
        /// </summary>
        virtual public int MaxHealth { get; set; }

        /// <summary>
        /// Integer representing the creature's current health. Should always be > 0.
        /// </summary>
        public int CurrentHealth { get; set; }

        /// <summary>
        /// Whether or not the creature is alive.
        /// </summary>
        public bool IsAlive { get => CurrentHealth > 0; }

        /// <summary>
        /// Whether or not the creature has been selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Sprite used for the creature.
        /// </summary>
        virtual public Texture2D Sprite { get; set; }

        public int SpriteWidth { get => Sprite.Width; }
        public int SpriteHeight { get => Sprite.Height; }

    }
}
