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
        private TowerType type;

        /// <summary>
        /// The sprite of this tower.
        /// </summary>
        private Texture2D sprite;

        /// <summary>
        /// The hitpoints of damage this tower deals.
        /// </summary>
        private int damage;

        /// <summary>
        /// This tower's fire rate, at a rate of rounds per second (RPS).
        /// </summary>
        private double fireRate;

        /// <summary>
        /// This tower's firing range, measured in units of tileWidth.
        /// </summary>
        private double fireRadius;

        /// <summary>
        /// The width of the base of this tower, measured in units of tiles.
        /// </summary>
        private int width;

        /// <summary>
        /// The height of the base of this tower, measured in units of tiles.
        /// </summary>
        private int height;

        /// <summary>
        /// Constructor for a new template of a tower.
        /// </summary>
        /// <param name="type">The type of tower this tower is.</param>
        /// <param name="sprite">A sprite representing this tower.</param>
        public TowerTemplate(TowerType type, Texture2D sprite) {
            Type = type;
            Sprite = sprite;
            
            switch(type) {
                case TowerType.BOLT:
                    Width = 2;
                    Height = 2;
                    Damage = 8;
                    FireRate = 1.5;
                    FireRadius = 6;
                    break;
                case TowerType.HUB:
                    Width = 3;
                    Height = 3;
                    break;
                default:
                    Width = 2;
                    Height = 2;
                    break;
            }
        }

        /* Setters and Getters */

        public Texture2D Sprite { get => sprite; set => sprite = value; }
        internal TowerType Type { get => type; set => type = value; }
        public int Damage { get => damage; set => damage = value; }
        public double FireRate { get => fireRate; set => fireRate = value; }
        public double FireRadius { get => fireRadius; set => fireRadius = value; }
        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public int SpriteWidth { get => Sprite.Width; }
        public int SpriteHeight { get => Sprite.Height; }

    }
}