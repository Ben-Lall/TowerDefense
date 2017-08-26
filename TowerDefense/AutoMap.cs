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

        /// <summary>
        /// Boolean representing if the camera has been panned.  Used to determine where the map should be drawn to upon next opening.
        /// </summary>
        bool Panning;

        public AutoMap(Vector2 startPos) {
            Type = UIType.Automap;
            Depth = 0;
            Visible = false;
            Bounds = new Rectangle(0, 0, ScreenWidth, ScreenHeight);

            AutoMapTileSize = new Point(16, 16);
            AutoMapSize = new Point(ScreenWidth / AutoMapTileSize.X, ScreenHeight / AutoMapTileSize.Y);
            AutoMapCamera = new Camera2d(startPos, ScreenWidth * TileWidth / AutoMapTileSize.X, ScreenHeight * TileHeight / AutoMapTileSize.Y);
            Buttons = new Dictionary<Button, Point>();

            // Create UI elements
            Point recenterPos = new Point(ScreenWidth - Art.RecenterButton.Width - 5, 5);
            Buttons.Add(new Button(new Point(Art.RecenterButton.Width, Art.RecenterButton.Height), Art.RecenterButton, Recenter), recenterPos);
        }

        /// <summary>
        /// Update this automap.
        /// </summary>
        public void Update() {
            if (!Panning) {
                AutoMapCamera.MoveTo(ActivePlayer.Pos.ToVector2());
            }
        }

        /// <summary>
        /// Overlay this AutoMap to the screen.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch) {
            // Draw ground tiles to the auto map.
            for (int i = AutoMapCamera.CameraTileStart.Y; i <= AutoMapCamera.CameraTileEnd.Y; i++) {
                for (int j = AutoMapCamera.CameraTileStart.X; j <= AutoMapCamera.CameraTileEnd.X; j++) {
                    int y = i - AutoMapCamera.CameraTileStart.Y;
                    int x = j - AutoMapCamera.CameraTileStart.X;
                    Tile repTile = WorldMap.At(j, i);
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
            foreach (KeyValuePair<Button, Point> bPair in Buttons) {
                bPair.Key.Draw(UISpriteBatch, Bounds.Location + bPair.Value);
            }
        }

        /// <summary>
        /// Pan the camera in the given direction.
        /// </summary>
        /// <param name="direction">Vector giving the direction to pan the camera in.</param>
        public void PanCamera(Vector2 direction) {
            if(!direction.Equals(Vector2.Zero)) {
                Panning = true;
                AutoMapCamera.Move(new Vector2((float)Math.Round(direction.X) * AutoMapTileSize.X / 4, (float)Math.Round(direction.Y) * AutoMapTileSize.Y / 4));
            }
        }

        /// <summary>
        /// Set panning to off.
        /// </summary>
        public void Recenter() {
            Panning = false;
        }
    }
}
