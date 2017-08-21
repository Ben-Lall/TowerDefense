using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Include.Globals;

namespace TowerDefense {
    class AutoMap : UIPanel {

        Point AutoMapSize { get; set; }
        Point AutoMapTileSize { get; set; }
        Camera2d AutoMapCamera;

        public AutoMap(Vector2 startPos) {
            Type = UIType.AUTOMAP;
            Depth = 0;
            Visible = false;
            Bounds = new Rectangle(0, 0, ScreenWidth, ScreenHeight);

            AutoMapTileSize = new Point(16, 16);
            AutoMapSize = new Point(ScreenWidth / AutoMapTileSize.X, ScreenHeight / AutoMapTileSize.Y);
            AutoMapCamera = new Camera2d(startPos, ScreenWidth * TileWidth / AutoMapTileSize.X, ScreenHeight * TileHeight / AutoMapTileSize.Y);
            Buttons = new List<Button>();

            // Create UI elements
            Point recenterPos = new Point(ScreenWidth - Art.RecenterButton.Width - 5, 5);
            Buttons.Add(new Button(new Rectangle(recenterPos, new Point(Art.RecenterButton.Width, Art.RecenterButton.Height)), Art.RecenterButton, () => AutoMapCamera.MoveTo(ActivePlayer.Pos.ToVector2())));
        }

        /// <summary>
        /// Overlay the AutoMap to the screen.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch) {
            // Draw ground tiles to the auto map.
            for (int y = 0; y <= AutoMapSize.Y; y++) {
                for (int x = 0; x <= AutoMapSize.X; x++) {
                    Tile repTile = WorldMap.At(AutoMapCamera.CameraTileStart.X + x, AutoMapCamera.CameraTileStart.Y + y);
                    spriteBatch.Draw(Art.Pixel, new Rectangle(x * AutoMapTileSize.X, y * AutoMapTileSize.Y, AutoMapTileSize.X, AutoMapTileSize.Y), Art.PrevalentColors[repTile.SpriteId]);
                }
            }
            // Draw gameplay objects
            Rectangle AutoMapRegion = new Rectangle(AutoMapCamera.CameraStart, AutoMapCamera.CameraEnd);
            foreach (Tower t in Towers) {
                Point towerEnd = t.Pos + new Point(t.WidthTiles * TileWidth, t.HeightTiles * TileHeight);
                Rectangle towerRegion = new Rectangle(t.Pos, towerEnd);
                if (AutoMapRegion.Intersects(towerRegion)) {
                    for (int y = t.TilePos.Y; y < t.TilePos.Y + t.HeightTiles; y += 1) {
                        for (int x = t.TilePos.X; x < t.TilePos.X + t.WidthTiles; x += 1) {
                            if (y >= AutoMapCamera.CameraTileStart.Y && x >= AutoMapCamera.CameraTileStart.X) {
                                Point drawTile = new Point(x, y) - AutoMapCamera.CameraTileStart;
                                spriteBatch.Draw(Art.Pixel, new Rectangle(drawTile.X * AutoMapTileSize.X, drawTile.Y * AutoMapTileSize.Y, AutoMapTileSize.X, AutoMapTileSize.Y), Color.Gray);
                            }
                        }
                    }
                }
            }
            // Draw players last, with the active player absolutely last.
            Point drawPos = ActivePlayer.CenterTile - AutoMapCamera.CameraTileStart;
            spriteBatch.Draw(Art.Pixel, new Rectangle(drawPos.X * AutoMapTileSize.X, drawPos.Y * AutoMapTileSize.Y, AutoMapTileSize.X, AutoMapTileSize.Y), Color.Blue);

            // Draw Automap UI elements
            DrawAutoMapUI();
        }

        /// <summary>
        /// Draw AutoMap UI elements.
        /// </summary>
        private void DrawAutoMapUI() {
            //Draw buttons
            foreach (Button b in Buttons) {
                b.Draw(UISpriteBatch);
            }
        }

        /// <summary>
        /// Pan the camera in the given direction.
        /// </summary>
        /// <param name="direction">Vector giving the direction to pan the camera in.</param>
        public void PanCamera(Vector2 direction) {
            AutoMapCamera.Move(new Vector2((float)Math.Round(direction.X) * AutoMapTileSize.X / 4, (float)Math.Round(direction.Y) * AutoMapTileSize.Y / 4));
        }
    }
}
