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
            for (int y = 0; y < MapHeight; y++) {
                for (int x = 0; x < MapWidth; x++) {
                    mapTemp.Write(WorldMap.At(x, y).ToByteArray(), 0, Tile.TileDataSize);
                }
            }

            // Save the temporary file to the real file and delete the temporary.
            mapTemp.Dispose();
            File.Copy(tempFileName, fileName, true);
            File.Delete(tempFileName);
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
            WorldMap.LoadFromFile(new FileStream(name, FileMode.Open, FileAccess.ReadWrite, FileShare.None));
        }


        /// <summary>
        /// Check if there are any loadable worlds in the current directory.
        /// </summary>
        public static bool HasLoadableWorlds() {
            return Directory.EnumerateFiles(".", "*" + WorldSaveExtension).Any();
        }

        /// <summary>
        /// Return a sorted array of strings of the names of loadable worlds.
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
