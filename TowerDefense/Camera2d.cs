using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.Globals;


namespace TowerDefense {
    public class Camera2d {
        public float zoom; // Camera Zoom
        public Matrix Transform; // Matrix Transform
        public Vector2 pos; // Camera Position
        /// <summary>
        /// Pixel Position of the top-left corner of the camera.
        /// </summary>
        public Point Pos {
            get { return new Point(MathHelper.Clamp((int)pos.X - ScreenWidth / 2, 0, MapWidth * TileWidth - ScreenWidth), MathHelper.Clamp((int)pos.Y - ScreenHeight / 2, 0, MapHeight * TileHeight - ScreenHeight)); }
        }
        protected float Rotation { get; set; } // Camera Rotation
        public int ViewPortHeight { get => TileHeight * ScreenHeight; }
        public int ViewPortWidth { get => TileWidth * ScreenWidth; }
        public Point CameraStart { get => new Point(Pos.X, Pos.Y); }
        public Point CameraTileStart { get => new Point(CameraStart.X / TileWidth, CameraStart.Y / TileHeight); }
        public Point CameraHeatStart { get => new Point(CameraStart.X / HeatMap.HeatTileWidth, CameraStart.Y / HeatMap.HeatTileHeight); }
        public Point SpawnLeftStart {get => new Point((int)pos.X - Player.SpawnUpperBound, (int)pos.Y - Player.SpawnUpperBound); }
        public Point SpawnLeftEnd { get => new Point((int)pos.X - Player.SpawnLowerBound, (int)pos.Y - Player.SpawnLowerBound); }
        public Point CameraEnd { get => new Point(Math.Min(Pos.X + ScreenWidth, (MapWidth - 1) * TileWidth), Math.Min(Pos.Y + ScreenHeight, (MapHeight - 1) * TileHeight)); }
        public Point CameraTileEnd { get => new Point(CameraEnd.X / TileWidth, CameraEnd.Y / TileHeight); }
        public Point CameraHeatEnd { get => new Point(CameraEnd.X / HeatMap.HeatTileWidth, CameraEnd.Y / HeatMap.HeatTileHeight); }
        public Point SpawnRightStart { get => new Point((int)pos.X + Player.SpawnLowerBound, (int)pos.Y + Player.SpawnLowerBound); }
        public Point SpawnRightEnd { get => new Point((int)pos.X + Player.SpawnUpperBound, (int)pos.Y + Player.SpawnUpperBound); }



        /// <summary>
        /// Create a new camera centered at the given position.
        /// </summary>
        /// <param name="pos">Centerpoint of this camera</param>
        public Camera2d(Vector2 pos) {
            zoom = 1.0f;
            Rotation = 0.0f;
            this.pos = pos;
            Move(new Vector2(0, 0)); // Align the camera to the acceptable boundaries.
        }

        /// <summary>
        /// Translates the camera by a vector, while respecting camera boundaries.
        /// </summary>
        /// <param name="amount">Direction and magnitude of camera position change.</param>
        public void Move(Vector2 amount) {
            pos.X = (int)MathHelper.Clamp(pos.X + amount.X, (ScreenWidth * 0.5f) / Zoom, (MapWidth * TileWidth) - (ScreenWidth * 0.5f / Zoom));
            pos.Y = (int)MathHelper.Clamp(pos.Y + amount.Y, (ScreenHeight * 0.5f) / Zoom, (MapHeight * TileHeight) - (ScreenHeight * 0.5f / Zoom));
            pos.X = (int)MathHelper.Clamp(pos.X + amount.X, (ScreenWidth * 0.5f) / Zoom, (MapWidth * TileWidth) - (ScreenWidth * 0.5f / Zoom));
            pos.Y = (int)MathHelper.Clamp(pos.Y + amount.Y, (ScreenHeight * 0.5f) / Zoom, (MapHeight * TileHeight) - (ScreenHeight * 0.5f / Zoom));
        }

        /// <summary>
        /// Move the camera to the given location, while respecting camera boundaries.
        /// </summary>
        /// <param name="destination"></param>
        public void MoveTo(Vector2 destination) {
            pos = destination;
            Move(new Vector2(0, 0));
        }

        /// <summary>
        /// Rotate the camera by the given amount of radians.
        /// </summary>
        /// <param name="rotation"></param>
        public void Rotate(float rotation) {
            Rotation = (Rotation + rotation) % (float)(2 * Math.PI);
        }

        /// <summary>
        /// Generate and return a new transformation for this camera.
        /// </summary>
        /// <returns>The 3x3 Matrix representing the this camera's transform.</returns>
        public Matrix GetTransformation() {
            Transform =
            Matrix.CreateTranslation(new Vector3(-pos.X, -pos.Y, 0)) *
                                        Matrix.CreateRotationZ(Rotation) *
                                        Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                        Matrix.CreateTranslation(new Vector3(ScreenWidth * 0.5f, ScreenHeight * 0.5f, 0));
            return Transform;
        }

        public float Zoom {
            get { return zoom; }
            set { zoom = MathHelper.Clamp(value, 1.0f, 2.0f); }
        }
    }
}
