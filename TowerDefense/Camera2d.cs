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
        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation

        public Camera2d(Vector2 pos) {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _pos = pos;
        }

        // Sets and gets zoom
        public float Zoom {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public float Rotation {
            get { return _rotation; }
            set { _rotation = value; }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount) {
            _pos.X = MathHelper.Clamp(_pos.X + amount.X, ScreenWidth / 2, (MapWidth * TileWidth) - ScreenWidth + ScreenWidth / 2);
            _pos.Y = MathHelper.Clamp(_pos.Y + amount.Y, ScreenHeight / 2, (MapHeight * TileHeight) - ScreenHeight + ScreenHeight / 2);
        }

        /// <summary>
        /// Position of the top-left corner of the camera.
        /// </summary>
        public Vector2 Pos {
            get { return _pos - new Vector2(ScreenWidth / 2, ScreenHeight / 2 ); }
        }



        public Matrix get_transformation(GraphicsDevice graphicsDevice) {
            _transform =
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(ScreenWidth * 0.5f, ScreenHeight * 0.5f, 0));
            return _transform;
        }
    }
}
