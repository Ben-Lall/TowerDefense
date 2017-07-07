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
enum TowerType { BOLT }

namespace TowerDefense {
    internal class Tower {

        /// <summary>
        /// The type of this tower.
        /// </summary>
        private TowerType type;

        /// <summary>
        /// The sprite of this tower.
        /// </summary>
        private Texture2D sprite;

        public Tower(TowerType type, Texture2D sprite) {
            Type = type;
            Sprite = sprite;
        }

        public Texture2D Sprite { get => sprite; set => sprite = value; }
        internal TowerType Type { get => type; set => type = value; }
    }
}