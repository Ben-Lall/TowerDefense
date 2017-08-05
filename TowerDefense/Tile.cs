using Microsoft.Xna.Framework;
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
        public TileType Type { get; set; }

        /// <summary>
        /// Boolean representing whether or not this tile contains a tower or part of a tower.
        /// </summary>
        public bool ContainsTower { get; set; }

        /// <summary>
        /// Coordinate position of this tile in the world map.
        /// </summary>
        public Point Pos { get; set; }

        public int X { get => Pos.X; }
        public int Y { get => Pos.Y; }

        /// <summary>
        /// Constructor for a tile
        /// </summary>
        /// <param name="type">TileType representing the type of tile this is.</param>
        /// <param name="xPos">X coorinate of this tile's position.</param>
        /// <param name="yPos">Y coordinate of this tile's position.</param>
        public Tile(TileType type, int x, int y) {
            Type = type;
            Pos = new Point(x, y);
            ContainsTower = false;
        }

        /// <summary>
        /// Return true if this tile is not obstructed, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty() {
            return Type == TileType.OPEN && !ContainsTower;
        }

        /// <summary>
        /// Return true if this tile will obstruct the building of a tower, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool ObstructsTower() {
            return ContainsTower || Type == TileType.WALL || Type == TileType.OPEN;
        }
    }
}