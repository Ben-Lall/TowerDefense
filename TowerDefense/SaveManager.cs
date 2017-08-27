using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Include.GameState;
using static Include.Globals;

namespace TowerDefense {
    /// <summary>
    /// Class dedicated to loading and saving game elements.
    /// </summary>
    class SaveManager {

        public static String WorldSaveExtension { get => ".sav"; }
        public static String TempExtension { get => ".tmp"; }
        public static CancellationTokenSource CTS = new CancellationTokenSource();


        /// <summary>
        /// Create a new thread and begin map generation.
        /// </summary>
        /// <param name="worldName">The name of the world.</param>
        /// <param name="width">The width of the world, in units of tiles.</param>
        /// <param name="height">The height of the world, in units of tiles.</param>
        public static async void GenerateMap(String worldName, int width, int height) {
            await Task.Run(() => WorldMap.GenerateMap(worldName, width, height), CTS.Token);
        }

        /// <summary>
        /// Save the world map to a file.
        /// </summary>
        /// <param name="name">The name of the world.</param>
        /// <param name="width">The width of the world, in units of tiles.</param>
        /// <param name="height">The height of the world, in units of tiles.</param>
        public static void SaveMap(String name, int width, int height) {
            String fileName = name + WorldSaveExtension;
            String tempFileName = fileName + TempExtension;
            // Build a temporary file from the current world map.
            FileStream mapTemp = new FileStream(tempFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete);
            SaveWorldHeader(mapTemp, width, height);
            SaveTiles(mapTemp, width, height);
            SaveTowers(mapTemp);

            // Save the temporary file to the real file and delete the temporary.
            mapTemp.Dispose();
            File.Copy(tempFileName, fileName, true);
            File.Delete(tempFileName);
        }

        /// <summary>
        /// Write the world map header to the filestream.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="width">The width of the world, in units of tiles.</param>
        /// <param name="height">The height of the world, in units of tiles.</param>
        public static void SaveWorldHeader(FileStream f, int width, int height) {
            // Write the MapWidth and MapHeight
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(width >> 8);
            bytes[1] = (byte)width;
            bytes[2] = (byte)(height >> 8);
            bytes[3] = (byte)height;
            f.Write(bytes, 0, bytes.Count());
        }

        /// <summary>
        /// Write all tiles to the filestream.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="width">The width of the world, in units of tiles.</param>
        /// <param name="height">The height of the world, in units of tiles.</param>
        public static void SaveTiles(FileStream f, int width, int height) {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    f.Write(WorldMap.At(x, y).ToByteArray(), 0, Tile.TileDataSize);
                }
            }
        }

        /// <summary>
        /// Write the Towers to the filestream
        /// </summary>
        /// <param name="f"></param>
        public static void SaveTowers(FileStream f) {
            if (Towers != null) {
                foreach (Tower t in Towers) {
                    f.Write(t.ToByteArray(), 0, Tower.TowerDataSize);
                }
            }
            f.Write(new byte[] { Byte.MaxValue }, 0, 1); // Write end of towers signifier
        }

        /// <summary>
        /// Load the map of the given name.
        /// </summary>
        public static async void LoadMap(String name) {
            ActivePlayer = new Player(new Point(32000, 32000));
            FileStream f = new FileStream(name, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            await Task.Run(() => InitializeGameState(f), CTS.Token);
            DrawSet.Add(ActivePlayer);
            Players.Add(ActivePlayer);
        }

        /// <summary>
        /// Read and load in the data in the header.
        /// </summary>
        /// <param name="f"></param>
        public static void ReadHeader(FileStream f) {
            byte[] bytes = new byte[4];
            f.Read(bytes, 0, 4);
            MapWidth = bytes[0] << 8 | bytes[1];
            MapHeight = bytes[2] << 8 | bytes[3];
        }

        /// <summary>
        /// Read in the towers from the given filestream.
        /// </summary>
        /// <param name="f"></param>
        public static void LoadTowers(FileStream f) {
            byte[] bytes = new byte[Tower.TowerDataSize];
            f.Read(bytes, 0, 1);
            // Check that there are any towers to iterate through.
            if (bytes[0] != Byte.MaxValue) {
                f.Seek(-1, SeekOrigin.Current);
                f.Read(bytes, 0, Tower.TowerDataSize);
                // Iterate and add towers until the end of the towers section is reached.
                while (bytes[0] != Byte.MaxValue) {
                    AddTower(Tower.LoadFromByteArray(bytes));
                    f.Read(bytes, 0, Tower.TowerDataSize);
                }
            }
            LoadProgress += 0.1f;
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

        /// <summary>
        /// Clear the directory of any temporary files.
        /// </summary>
        static void ClearTempFiles() {
            String[] files = Directory.GetFiles(".", "*" + TempExtension);
            foreach(String filePath in files) {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Abort the currently running load operation.  The aborted operation may be
        /// a world generation operation or a world load operation.
        /// </summary>
        public static void AbortLoad() {
            CTS.Cancel();
            CurrentGameState = GameStatus.Title;
            ClearTempFiles();
        }
    }
}
