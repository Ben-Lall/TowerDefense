using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense {
    /// <summary>
    /// A static data structure containing a list of settings set by and calculated from the user's settings.
    /// </summary>
    static class Settings {
        /// <summary>
        /// Integer representing the width of all tiles.
        /// </summary>
        private static int tileWidth;

        /// <summary>
        /// Integer representing the height of all tiles.
        /// </summary>
        private static int tileHeight;

        /// <summary>
        /// Integer representing the width of the viewport, measured in units of tiles.
        /// </summary>
        private static int viewportRowLength;

        /// <summary>
        /// Integer representing the height of the viewport, measured in units of tiles.
        /// </summary>
        private static int viewportColumnLength;


        /* Setters and Getters */

        public static int TileWidth { get => tileWidth; set => tileWidth = value; }
        public static int TileHeight { get => tileHeight; set => tileHeight = value; }
        public static int ViewportRowLength { get => viewportRowLength; set => viewportRowLength = value; }
        public static int ViewportColumnLength { get => viewportColumnLength; set => viewportColumnLength = value; }

    }
}