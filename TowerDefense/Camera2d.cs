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
        public Matrix transform; // Matrix Transform
        public Vector2 pos; // Camera Position
        protected float Rotation { get; set; } // Camera Rotation

        /// <summary>
        /// Create a new camera centered at the given position.
        /// </summary>
        /// <param name="pos">Centerpoint of this camera</param>
        public Camera2d(Vector2 pos) {
            zoom = 1.0f;
            Rotation = 0.0f;
            this.pos = pos;
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount) {
            pos.X = (int)MathHelper.Clamp(pos.X + amount.X, (ScreenWidth * 0.5f) / Zoom, (MapWidth * TileWidth) - (ScreenWidth * 0.5f / Zoom));
            pos.Y = (int)MathHelper.Clamp(pos.Y + amount.Y, (ScreenHeight * 0.5f) / Zoom, (MapHeight * TileHeight) -( ScreenHeight * 0.5f / Zoom));
        }

        /// <summary>
        /// Generate and return a new transformation for this camera.
        /// </summary>
        /// <returns>The 3x3 Matrix representing the this camera's transform.</returns>
        public Matrix get_transformation() {
            transform =
              Matrix.CreateTranslation(new Vector3(-pos.X, -pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(ScreenWidth * 0.5f, ScreenHeight * 0.5f, 0));
            return transform;
        }

        /// <summary>
        /// Position of the top-left corner of the camera.
        /// </summary>
        public Vector2 Pos {
            get { return pos - new Vector2(ScreenWidth / 2, ScreenHeight / 2); }
        }

        public float Zoom {
            get { return zoom; }
            set { zoom = MathHelper.Clamp(value, 1.0f, 2.0f); }
        }
    }
}
