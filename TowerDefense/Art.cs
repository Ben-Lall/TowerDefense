using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;

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

        private static TowerSprite LoadTowerSprite(ContentManager content, String folderName) {
            String folderDirectory = "towers/" + folderName;
            DirectoryInfo dir = new DirectoryInfo(content.RootDirectory + "/" + folderDirectory);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            Texture2D[][] spriteSheet = new Texture2D[(int)TowerAnimationType.NUMBER_OF_ANIMATION_TYPES][];
            FileInfo[] files = dir.GetFiles("*.*");

            spriteSheet[(int)TowerAnimationType.IDLE] = new Texture2D[files.Count(f => Path.GetFileNameWithoutExtension(f.Name)[0] == 'i')];

            foreach (FileInfo file in files) {
                string fileName = Path.GetFileNameWithoutExtension(file.Name);
                switch (fileName[0]) {
                    case 'i':
                        spriteSheet[(int)TowerAnimationType.IDLE][fileName[fileName.Length - 1] - '0' - 1] = content.Load<Texture2D>(folderDirectory + "/" + fileName);
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

            Texture2D[][] spriteSheet = new Texture2D[(int)CreatureAnimationType.NUMBER_OF_ANIMATION_TYPES][];
            FileInfo[] files = dir.GetFiles("*.*");

            spriteSheet[(int)CreatureAnimationType.IDLE] = new Texture2D[files.Count(f => Path.GetFileNameWithoutExtension(f.Name)[0] == 'i')];
            spriteSheet[(int)CreatureAnimationType.LEFT] = new Texture2D[files.Count(f => Path.GetFileNameWithoutExtension(f.Name)[0] == 'l')];
            spriteSheet[(int)CreatureAnimationType.RIGHT] = new Texture2D[files.Count(f => Path.GetFileNameWithoutExtension(f.Name)[0] == 'r')];
            spriteSheet[(int)CreatureAnimationType.UP] = new Texture2D[files.Count(f => Path.GetFileNameWithoutExtension(f.Name)[0] == 'u')];
            spriteSheet[(int)CreatureAnimationType.DOWN] = new Texture2D[files.Count(f => Path.GetFileNameWithoutExtension(f.Name)[0] == 'd')];

            foreach (FileInfo file in files) {
                string fileName = Path.GetFileNameWithoutExtension(file.Name);
                switch(fileName[0]) {
                    case 'i':
                        spriteSheet[(int)CreatureAnimationType.IDLE][fileName[fileName.Length - 1] - '0' - 1] = content.Load<Texture2D>(folderDirectory + "/" + fileName);
                        break;
                    case 'l':
                        spriteSheet[(int)CreatureAnimationType.LEFT][fileName[fileName.Length - 1] - '0' - 1] = content.Load<Texture2D>(folderDirectory + "/" + fileName);
                        break;
                    case 'r':
                        spriteSheet[(int)CreatureAnimationType.RIGHT][fileName[fileName.Length - 1] - '0' - 1] = content.Load<Texture2D>(folderDirectory + "/" + fileName);
                        break;
                    case 'u':
                        spriteSheet[(int)CreatureAnimationType.UP][fileName[fileName.Length - 1] - '0' - 1] = content.Load<Texture2D>(folderDirectory + "/" + fileName);
                        break;
                    case 'd':
                        spriteSheet[(int)CreatureAnimationType.DOWN][fileName[fileName.Length - 1] - '0' - 1] = content.Load<Texture2D>(folderDirectory + "/" + fileName);
                        break;
                }
            }
            return new CreatureSprite(spriteSheet);
        }
    }
}
