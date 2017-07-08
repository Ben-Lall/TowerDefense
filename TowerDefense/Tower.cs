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


        /* Setters and Getters */

        public Point Pos { get => pos; set => pos = value; }
        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public TowerTemplate Template { get => template; set => template = value; }
        public Texture2D Sprite { get => template.Sprite; set => template.Sprite = value; }
        public TowerType Type { get => template.Type; set => template.Type = value; }
    }
}
