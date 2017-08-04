using Microsoft.Xna.Framework;
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
        public static int TileWidth { get; set; }

        /// <summary>
        /// Integer representing the height of all tiles.
        /// </summary>
        public static int TileHeight { get; set; }

        /// <summary>
        /// Integer representing the width of the game map, measured in units of tiles.
        /// </summary>
        public static int MapWidth { get; set; }

        /// <summary>
        /// Integer representing the height of the game map, measured in units of tiles.
        /// </summary>
        public static int MapHeight { get; set; }

        /// <summary>
        /// Integer representing the width of the viewport, measured in units of tiles.
        /// </summary>
        public static int ViewRows { get; set; }

        /// <summary>
        /// Integer representing the height of the viewport, measured in units of tiles.
        /// </summary>
        public static int ViewCols { get; set; }

        /// <summary>
        /// Integer representing the width of the viewport, measured in units of pixels.
        /// </summary>
        public static int ViewRowsPx { get => ViewRows * TileWidth; }

        /// <summary>
        /// Integer representing the height of the viewport, measured in units of pixels.
        /// </summary>
        public static int ViewColsPx { get => ViewCols * TileHeight; }

        /// <summary>
        /// The maximum possible Y value for the viewport.
        /// </summary>
        public static int MaxViewportY { get => MapHeight - ViewCols; }

        /// <summary>
        /// The maximum possible X value for the viewport.
        /// </summary>
        public static int MaxViewportX { get => MapWidth - ViewRows; }

        /// <summary>
        /// The dimensions of the viewport, measured in units of tiles.
        /// </summary>
        public static Point ViewPortDimensions { get => new Point(ViewRows, ViewCols); }

        /// <summary>
        /// The dimensions of the viewport, measured in units of pixels.
        /// </summary>
        public static Point ViewPortDimensionsPx { get => new Point(ViewRowsPx, ViewColsPx); }
    }
}