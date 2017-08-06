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
        public float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 _pos; // Camera Position
        protected float Rotation { get; set; } // Camera Rotation

        /// <summary>
        /// Create a new camera centered at the given position.
        /// </summary>
        /// <param name="pos">Centerpoint of this camera</param>
        public Camera2d(Vector2 pos) {
            _zoom = 1.0f;
            Rotation = 0.0f;
            _pos = pos;
        }

        // Sets and gets zoom
        public float Zoom {
            get { return _zoom; }
            set { _zoom = MathHelper.Clamp(value, 1.0f, 2.0f); }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount) {
            _pos.X = (int)MathHelper.Clamp(_pos.X + amount.X, (ScreenWidth * 0.5f) / Zoom, (MapWidth * TileWidth) - (ScreenWidth * 0.5f / Zoom));
            _pos.Y = (int)MathHelper.Clamp(_pos.Y + amount.Y, (ScreenHeight * 0.5f) / Zoom, (MapHeight * TileHeight) -( ScreenHeight * 0.5f / Zoom));
        }

        /// <summary>
        /// Position of the top-left corner of the camera.
        /// </summary>
        public Vector2 Pos {
            get { return _pos - new Vector2(ScreenWidth / 2, ScreenHeight / 2 ); }
        }

        public Matrix get_transformation() {
            _transform =
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(ScreenWidth * 0.5f, ScreenHeight * 0.5f, 0));
            return _transform;
        }
    }
}
