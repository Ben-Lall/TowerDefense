using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.Globals;


namespace TowerDefense {
    /// <summary>
    /// Class dedicated to loading and saving game elements.
    /// </summary>
    class SaveManager {

        public static String WorldSaveExtension { get => ".sav"; }
        public static String TempExtension { get => ".tmp"; }

        /// <summary>
        /// Save the world map to a file.
        /// </summary>
        public static void SaveMap(String name) {
            String fileName = name + WorldSaveExtension;
            String tempFileName = fileName + TempExtension;
            // Build a temporary file from the current world map.
            FileStream mapTemp = new FileStream(tempFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            SaveWorldHeader(mapTemp);
            SaveTiles(mapTemp);
            SaveTowers(mapTemp);

            // Save the temporary file to the real file and delete the temporary.
            mapTemp.Dispose();
            File.Copy(tempFileName, fileName, true);
            File.Delete(tempFileName);
        }

        /// <summary>
        /// Save and exit the currently open map.
        /// </summary>
        public static void SaveAndExit() {
            CurrentGameState = GameState.Title;
            SaveMap(WorldName);
            Monsters.Clear();
            Towers.Clear();
            DrawSet.Clear();
            Effects.Clear();
            Players.Clear();
        }

        /// <summary>
        /// Write the world map header to the filestream.
        /// </summary>
        /// <param name="f"></param>
        public static void SaveWorldHeader(FileStream f) {
            // Write the MapWidth and MapHeight
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(MapWidth >> 8);
            bytes[1] = (byte)MapWidth;
            bytes[2] = (byte)(MapHeight >> 8);
            bytes[3] = (byte)MapHeight;
            f.Write(bytes, 0, bytes.Count());
        }

        /// <summary>
        /// Write all tiles to the filestream.
        /// </summary>
        /// <param name="f"></param>
        public static void SaveTiles(FileStream f) {
            for (int y = 0; y < MapHeight; y++) {
                for (int x = 0; x < MapWidth; x++) {
                    f.Write(WorldMap.At(x, y).ToByteArray(), 0, Tile.TileDataSize);
                }
            }
        }

        /// <summary>
        /// Write the Towers to the filestream
        /// </summary>
        /// <param name="f"></param>
        public static void SaveTowers(FileStream f) {
            foreach(Tower t in Towers) {
                f.Write(t.ToByteArray(), 0, Tower.TowerDataSize);
            }
            f.Write(new byte[] { Byte.MaxValue }, 0, 1); // Write end of towers signifier
        }

        /// <summary>
        /// Load the map of the given name.
        /// </summary>
        public static void LoadMap(String name) {
            // Initialize ActivePlayer
            ActivePlayer = new Player(new Point(((MapWidth / 2) - 1) * TileWidth, ((MapHeight / 2) - 1) * TileHeight));
            UIPanels = ActivePlayer.UIElements;
            Camera = new Camera2d(ActivePlayer.Pos.ToVector2(), ScreenWidth, ScreenHeight);
            DrawSet.Add(ActivePlayer);

            SpawnRate = 6.0;
            SpawnCooldown = 0;
            HeatMap.Initialize();
            FileStream f = new FileStream(name, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            WorldMap.LoadFromFile(f);
            f.Dispose();
        }


        /// <summary>
        /// Check if there are any loadable worlds in the current directory.
        /// </summary>
        public static bool HasLoadableWorlds() {
            return Directory.EnumerateFiles(".", "*" + WorldSaveExtension).Any();
        }

        /// <summary>
        /// Return a sorted enumerable collection of strings of the names of loadable worlds.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<String> ListLoadableWorlds() {
            String[] files = Directory.GetFiles(".", "*" + WorldSaveExtension);
            Array.Sort(files, (x, y) => String.Compare(x, y));
            return files.Select(x => x.Substring(2));
        }

        /// <summary>
        /// Get the next default name for a world in this directory.
        /// </summary>
        /// <returns></returns>
        public static String NextDefaultWorldName() {
            string name = "world";
            int num = 1;
            foreach (String file in ListLoadableWorlds()) {
                if(file.Substring(0, file.IndexOf('.')).Equals(name + num)) {
                    num += 1;
                }
            }
            return name + num;
        }

    }
}
