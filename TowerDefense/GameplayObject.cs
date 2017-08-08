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
    abstract class GameplayObject {
        /// <summary>
        /// Pixel coordinates of this GameplayObject.
        /// </summary>
        public Point Pos { get; set; }
        public int X { get => Pos.X; set => Pos = new Point(value, Y); }
        public int Y { get => Pos.Y; set => Pos = new Point(X, value); }

        public int Width { get; set; }
        public int Height { get; set; }

        /// <summary>
        /// Pixel coordinates of the center of this GameplayObject.
        /// </summary>
        public Point CenterPoint { get => (Pos + new Point(Width / 2, Height / 2)); }

        /// <summary>
        /// Pixel coordinates from which attack visual effects originate.
        /// </summary>
        public virtual Point FirePoint { get => CenterPoint; }

        /// <summary>
        /// Tile coordinates of this GameplayObject.
        /// </summary>
        public Point TilePos { get => new Point(X / TileWidth, Y / TileHeight); set => Pos = new Point(value.X * TileWidth, value.Y * TileHeight); }

        public int TileX { get => TilePos.X; set => TilePos = new Point(value, TileY); }
        public int TileY { get => TilePos.Y; set => TilePos = new Point(TileX, value); }

        /// <summary>
        /// Tile coordinates of the center of this GameplayObject.
        /// </summary>
        public Point CenterTile { get => new Point(CenterPoint.X / TileWidth, CenterPoint.Y / TileHeight); }

        /// <summary>
        /// Hitpoints of damage dealt by this GameplayObject's attacks.
        /// </summary>
        public virtual int AttackDamage { get; set; }

        /// <summary>
        /// GameplayObject's attack range, measured in units of tileWidth.
        /// </summary>
        public virtual double AttackRange { get; set; }

        /// <summary>
        /// GameplayObject's attack range, measured in units of pixels.
        /// </summary>
        public Point PixelRange { get => new Point((int)(AttackRange * TileWidth), (int)(AttackRange * TileHeight)); }

        /// <summary>
        /// Frequency of this GameplayObject's attacks, measured in hertz.
        /// </summary>
        public virtual double AttackRate { get; set; }

        /// <summary>
        /// Current time until this GameplayObject's next attack, measured in seconds.
        /// </summary>
        public double Cooldown { get; set; }

        /// <summary>
        /// This GameplayObject's maximum possible health.
        /// </summary>
        public virtual int MaxHealth { get; set; }

        /// <summary>
        /// This GameplayObject's current health. Should always be >= 0.
        /// </summary>
        public int CurrentHealth { get; set; }

        /// <summary>
        /// Whether or not this GameplayObject is alive.
        /// </summary>
        public bool IsAlive { get => CurrentHealth > 0; }

        /// <summary>
        /// Whether or not this GameplayObject has been selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Sprite used for this GameplayObject.
        /// </summary>
        public AnimatedSprite Sprite { get; set; }

        public int SpriteWidth { get => Sprite.Width; }
        public int SpriteHeight { get => Sprite.Height; }

        /// <summary>
        /// Draw this GameplayObject at its current position.
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// Have this GameplayObject take an action.
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Attack a valid target.
        /// </summary>
        public abstract void Attack();

        /// <summary>
        /// Deals an amount of damage to this GameplayObject.
        /// </summary>
        /// <param name="damage"></param>
        public abstract void TakeDamage(int damage);

    }
}
