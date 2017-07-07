using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense {
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

        public static int TileWidth {
            get {
                return tileWidth;
            }
            set {
                tileWidth = value;
            }
        }

        public static int TileHeight {
            get {
                return tileHeight;
            }
            set {
                tileHeight = value;
            }
        }

        public static int ViewportRowLength {
            get {
                return viewportRowLength;
            }
            set {
                viewportRowLength = value;
            }
        }

        public static int ViewportColumnLength {
            get {
                return viewportColumnLength;
            }
            set {
                viewportColumnLength = value;
            }
        }
    }




}
