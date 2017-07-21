using Microsoft.Xna.Framework;
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
    class Monster {
        /// <summary>
        /// The sprite of this monster.
        /// </summary>
        private Texture2D sprite;

        /// <summary>
        /// The type of monster that this template represents.
        /// </summary>
        private MonsterType type;

        /// <summary>
        /// Coordinate position of the top-left pixel of this monster's sprite.
        /// </summary>
        private Point pos;

        /// <summary>
        /// Integer representing this creature's maximum possible health.
        /// </summary>
        private int maxHealth;

        /// <summary>
        /// Integer representing this creature's current health.  Should always be > 0.
        /// </summary>
        private int currentHealth;

        public Monster(Texture2D sprite, MonsterType type, Point pos, int maxHealth) {
            Sprite = sprite;
            Type = type;
            Pos = pos;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public void Draw(SpriteBatch spritebatch) {
            spritebatch.Draw(Sprite, new Rectangle(Pos, new Point(SpriteWidth, SpriteHeight)), Color.White);
        }

        public Texture2D Sprite { get => sprite; set => sprite = value; }
        public Point Pos { get => pos; set => pos = value; }
        public int SpriteWidth { get => Sprite.Width; }
        public int SpriteHeight { get => Sprite.Height; }
        public int MaxHealth { get => maxHealth; set => maxHealth = value; }
        public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
        internal MonsterType Type { get => type; set => type = value; }
    }
}
