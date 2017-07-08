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
        /// Constructor for a new template of a tower.
        /// </summary>
        /// <param name="type">The type of tower this tower is.</param>
        /// <param name="sprite">A sprite representing this tower.</param>
        public TowerTemplate(TowerType type, Texture2D sprite) {
            Type = type;
            Sprite = sprite;
        }

        /* Setters and Getters */

        public Texture2D Sprite { get => sprite; set => sprite = value; }
        internal TowerType Type { get => type; set => type = value; }
    }
}