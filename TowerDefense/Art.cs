using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using static Include.Globals;


namespace TowerDefense {
    /// <summary>
    /// A static class dedicated to art assets and all associated information.
    /// </summary>
    static class Art {
        /* Fonts */
        public static SpriteFont Font;

        /* UI Textures */
        public static Texture2D MenuPanel;
        public static Texture2D TowerButton;
        public static Texture2D RecenterButton;

        /* World Textures */
        public static Texture2D TileSet;

        // Start indices for certain sprites types in the tilset
        public const int FieldStartIndex = 181;
        public const int DesertStartIndex = 448;
        public const int SwampStartIndex = 800;
        public const int CaveStartIndex = 995;
        public const int TundraStartIndex = 777;

        /// <summary>
        /// An array where each index refers to the most prevalent color of the texture of that ID in the TileSet.
        /// </summary>
        public static Color[] PrevalentColors;

        /** GameplayObject Textures **/

        /* Player Textures */
        public static CreatureSprite Player;

        /* Tower Textures */
        public static TowerSprite Tower;
        public static TowerSprite Hub;

        /* Monster Textures */
        public static CreatureSprite Imp;

        /* Effect Textures */
        public static Texture2D BoltLine;
        public static Texture2D BoltCap;
        public const float BoltThickness = 11;
        public static Texture2D Pixel;

        public static void LoadContent(ContentManager content, GraphicsDevice graphics) {
            /* Fonts */
            Font = content.Load<SpriteFont>("Font");
            /* UI Textures */
            MenuPanel = content.Load<Texture2D>("menu_panel");
            TowerButton = content.Load<Texture2D>("menu_panel");
            RecenterButton = content.Load<Texture2D>("recenter");

            /* World Textures */
            TileSet = content.Load<Texture2D>("tiles/terrain_atlas");
            LoadPrevalentColors();

            /* Player Textures */
            Player = LoadCreatureSprite(content, "player");

            /* Tower Textures */
            Tower = LoadTowerSprite(content, "bolt");
            Hub = LoadTowerSprite(content, "hub");
            /* Monster Textures */
            Imp = LoadCreatureSprite(content, "imp");

            /* Effect Textures */
            BoltLine = content.Load<Texture2D>("boltLine");
            BoltCap = content.Load<Texture2D>("boltCap");

            Pixel = new Texture2D(graphics, 1, 1);
            Pixel.SetData<Color>(new Color[] { Color.White });
        }

        /// <summary>
        /// Load the most prevalent colors of each tile from the tileset to an array of colors.
        /// </summary>
        private static void LoadPrevalentColors() {
            int ID = 0;
            PrevalentColors = new Color[(TileSet.Width / TileWidth) * (TileSet.Height / TileHeight)];
            for(int i = 0; i < TileSet.Height; i+= TileHeight) {
                for (int j = 0; j < TileSet.Width; j+= TileWidth) {
                    // Get texture's color data
                    Color[] buffer = new Color[TileWidth * TileHeight];
                    TileSet.GetData(0, new Rectangle(j, i, TileWidth, TileHeight), buffer, 0, TileWidth * TileHeight);
                    // Get totals of each non-transparent color's occurence.
                    Dictionary<Color, int> occurrences = new Dictionary<Color, int>();
                    foreach (Color c in buffer) {
                        if(!c.Equals(Color.Transparent)) {
                            if(occurrences.ContainsKey(c)) {
                                occurrences[c]++;
                            } else {
                                occurrences[c] = 1;
                            }
                        }
                    }

                    // Get the color that has the highest key (frequency).
                    if (occurrences.Count > 0) {
                        PrevalentColors[ID] = occurrences.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    } else {
                        PrevalentColors[ID] = Color.Black;
                    }
                    ID++;
                }
            }
        }


        /// <summary>
        /// Loads the spritesheet for a creature sprite from the given directory.
        /// </summary>
        /// <param name="folderName">The name of the immediately containing folder.</param>
        /// <returns></returns>
        private static TowerSprite LoadTowerSprite(ContentManager content, String folderName) {
            String folderDirectory = "towers/" + folderName;
            DirectoryInfo dir = new DirectoryInfo(content.RootDirectory + "/" + folderDirectory);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            Texture2D[][] spriteSheet = new Texture2D[(int)TowerAnimationType.NumberOfAnimationTypes][];
            FileInfo[] files = dir.GetFiles("*.*");

            spriteSheet[(int)TowerAnimationType.Idle] = new Texture2D[files.Count(f => Path.GetFileNameWithoutExtension(f.Name)[0] == 'i')];

            foreach (FileInfo file in files) {
                string fileName = Path.GetFileNameWithoutExtension(file.Name);
                switch (fileName[0]) {
                    case 'i':
                        spriteSheet[(int)TowerAnimationType.Idle][fileName[fileName.Length - 1] - '0' - 1] = content.Load<Texture2D>(folderDirectory + "/" + fileName);
                        break;
                }
            }

            return new TowerSprite(spriteSheet);
        }

        /// <summary>
        /// Loads the spritesheet for a creature sprite from the given directory.
        /// </summary>
        /// <param name="folderName">The name of the immediately containing folder.</param>
        /// <returns></returns>
        private static CreatureSprite LoadCreatureSprite(ContentManager content, String folderName) {
            String folderDirectory = "creatures/" + folderName;
            DirectoryInfo dir = new DirectoryInfo(content.RootDirectory + "/" + folderDirectory);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            Texture2D[][] spriteSheet = new Texture2D[(int)CreatureAnimationType.NumberOfAnimationTypes][];
            FileInfo[] files = dir.GetFiles("*.*");

            spriteSheet[(int)CreatureAnimationType.Idle] = new Texture2D[files.Count(f => Path.GetFileNameWithoutExtension(f.Name)[0] == 'i')];
            spriteSheet[(int)CreatureAnimationType.Left] = new Texture2D[files.Count(f => Path.GetFileNameWithoutExtension(f.Name)[0] == 'l')];
            spriteSheet[(int)CreatureAnimationType.Right] = new Texture2D[files.Count(f => Path.GetFileNameWithoutExtension(f.Name)[0] == 'r')];
            spriteSheet[(int)CreatureAnimationType.Up] = new Texture2D[files.Count(f => Path.GetFileNameWithoutExtension(f.Name)[0] == 'u')];
            spriteSheet[(int)CreatureAnimationType.Down] = new Texture2D[files.Count(f => Path.GetFileNameWithoutExtension(f.Name)[0] == 'd')];

            foreach (FileInfo file in files) {
                string fileName = Path.GetFileNameWithoutExtension(file.Name);
                switch(fileName[0]) {
                    case 'i':
                        spriteSheet[(int)CreatureAnimationType.Idle][fileName[fileName.Length - 1] - '0' - 1] = content.Load<Texture2D>(folderDirectory + "/" + fileName);
                        break;
                    case 'l':
                        spriteSheet[(int)CreatureAnimationType.Left][fileName[fileName.Length - 1] - '0' - 1] = content.Load<Texture2D>(folderDirectory + "/" + fileName);
                        break;
                    case 'r':
                        spriteSheet[(int)CreatureAnimationType.Right][fileName[fileName.Length - 1] - '0' - 1] = content.Load<Texture2D>(folderDirectory + "/" + fileName);
                        break;
                    case 'u':
                        spriteSheet[(int)CreatureAnimationType.Up][fileName[fileName.Length - 1] - '0' - 1] = content.Load<Texture2D>(folderDirectory + "/" + fileName);
                        break;
                    case 'd':
                        spriteSheet[(int)CreatureAnimationType.Down][fileName[fileName.Length - 1] - '0' - 1] = content.Load<Texture2D>(folderDirectory + "/" + fileName);
                        break;
                }
            }
            return new CreatureSprite(spriteSheet);
        }

        /// <summary>
        /// Given an ID, return region for the proper texture from the tileset.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Rectangle GetSourceRectangle(int ID) {
            int y = ID / (TileSet.Width / TileWidth);
            int x = ID % (TileSet.Width / TileWidth);
            return new Rectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight);
        }
    }
}
