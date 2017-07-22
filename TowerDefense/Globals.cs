using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense {
    /// <summary>
    /// A class containing global variables and helper methods.
    /// </summary>
    static class Globals {
        /// <summary>
        /// Array containing every monster in the game, indexed by the MonsterType enumerator.
        /// </summary>
        private static Monster[] monsterCatalog;

        /** Tower Templates **/
        private static TowerTemplate boltTowerTemplate;
        private static TowerTemplate hubTemplate;

        /** Textures **/
        /* UI Textures */
        private static Texture2D menuPanelTex;
        private static Texture2D pixel;

        /* Tower Textures */
        private static Texture2D towerTex;
        private static Texture2D towerButtonTex;
        private static Texture2D hubTex;

        /* Monster Textures */
        private static Texture2D impTex;

        public static void InitializeGlobals(GraphicsDevice graphics) {
            MonsterCatalog = new Monster[(int)MonsterType.NUMBER_OF_MONSTERS];
            Pixel = new Texture2D(graphics, 1, 1);
            Pixel.SetData<Color>(new Color[] { Color.White });

            // Create Tower templates
            BoltTowerTemplate = new TowerTemplate(TowerType.BOLT, TowerTex);
            HubTemplate = new TowerTemplate(TowerType.HUB, HubTex);
        }

        /// <summary>
        /// Returns the Manhattan Distance between two points.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static int ManhattanDistance(Point p1, Point p2) {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }


        internal static Monster[] MonsterCatalog { get => monsterCatalog; set => monsterCatalog = value; }
        internal static TowerTemplate BoltTowerTemplate { get => boltTowerTemplate; set => boltTowerTemplate = value; }
        internal static TowerTemplate HubTemplate { get => hubTemplate; set => hubTemplate = value; }
        public static Texture2D MenuPanel { get => menuPanelTex; set => menuPanelTex = value; }
        public static Texture2D HubTex { get => hubTex; set => hubTex = value; }
        public static Texture2D TowerTex { get => towerTex; set => towerTex = value; }
        public static Texture2D TowerButton { get => towerButtonTex; set => towerButtonTex = value; }
        public static Texture2D Pixel { get => pixel; set => pixel = value; }
        public static Texture2D ImpTex { get => impTex; set => impTex = value; }

    }
}

