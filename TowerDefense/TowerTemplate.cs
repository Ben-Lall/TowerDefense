using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Enum listing the types of towers available:
/// <para>Bolt: Fires a simple projectile</para>
/// </summary>
enum TowerType { BOLT, HUB }

namespace TowerDefense {
    /// <summary>
    /// A template that can be used to build a tower.
    /// </summary>
    internal class TowerTemplate {

        /// <summary>
        /// The type of this tower.
        /// </summary>
        public TowerType Type { get; set; }

        /// <summary>
        /// The sprite of this tower.
        /// </summary>
        public TowerSprite Sprite { get; set; }

        public int SpriteWidth { get => Sprite.Width; }
        public int SpriteHeight { get => Sprite.Height; }

        /// <summary>
        /// The hitpoints of damage dealt by this tower's attacks.
        /// </summary>
        public int AttackDamage { get; set; }

        /// <summary>
        /// Frequency of this tower's attacks, measured in hertz.
        /// </summary>
        public double AttackRate { get; set; }

        /// <summary>
        /// This tower's attack range, measured in units of tileWidth.
        /// </summary>
        public double AttackRange { get; set; }

        /// <summary>
        /// This tower's maximum possible health.
        /// </summary>
        public int MaxHealth { get; set; }

        /// <summary>
        /// The width of the base of this tower, measured in units of tiles.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the base of this tower, measured in units of tiles.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Constructor for a new template of a tower.
        /// </summary>
        /// <param name="type">The type of tower this tower is.</param>
        /// <param name="sprite">A sprite representing this tower.</param>
        public TowerTemplate(TowerType type) {
            Type = type;
            
            switch(type) {
                case TowerType.BOLT:
                    Sprite = new TowerSprite(Art.Tower);
                    Width = 1;
                    Height = 1;
                    AttackDamage = 8;
                    AttackRate = 1.5;
                    AttackRange = 6;
                    MaxHealth = 100;
                    break;
                case TowerType.HUB:
                    Sprite = new TowerSprite(Art.Hub);
                    Width = 2;
                    Height = 2;
                    MaxHealth = 3000;
                    break;
                default:
                    Width = 2;
                    Height = 2;
                    break;
            }
        }
    }
}
