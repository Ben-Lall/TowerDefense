using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



/// <summary>
/// Enumerator representing the different types of possible tiles
/// </summary>
enum TileType { OPEN, LIMITED, WALL}

namespace TowerDefense {
    /// <summary>
    /// A Tile making up the map of the game world.  Each tile has a level of traversability as follows: 
    ///     Open:    Any creature, including the player, may move across this tile.
    ///     Limited: Only the player may cross this tile.
    ///     Wall:    None may cross this tile.
    ///     
    /// Each Tile also stores the coordinate position of its top-left corner.  The game screen is 160x90 units.
    /// </summary>
    class Tile {

        /// <summary>
        /// A TileType value representing the traversability of this tile.
        /// </summary>
        private TileType type;
        /// <summary>
        /// Integer representing the x position of the top-left corner of this tile.
        /// </summary>
        private int xPos;
        /// <summary>
        /// Integer representing the y position of the top-left corner of this tile.
        /// </summary>
        private int yPos;

        /// <summary>
        /// Constructor for a tile
        /// </summary>
        /// <param name="type">TileType representing the type of tile this is.</param>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        public Tile(TileType type, int xPos, int yPos) {
            this.type = type;
            this.xPos = xPos;
            this.yPos = yPos;
        }

        /* Setters and Getters */

        public TileType Type {
            get {
                return type;
            }
            set {
                type = value;
            }
        }

        public int XPos {
            get {
                return xPos;
            }
            set {
                xPos = value;
            }
        }

        public int YPos {
            get {
                return yPos;
            }
            set {
                yPos = value;
            }
        }

    }
}
