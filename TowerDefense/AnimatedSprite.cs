using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense {
    /// <summary>
    /// Class that draws animated sprites.
    /// </summary>
    public abstract class AnimatedSprite {

        /// <summary>
        /// The spritesheet for this sprite.  Each row of the array corresponds to a different animation for this sprite.
        /// </summary>
        public Texture2D[][] SpriteSheet { get; set; }

        /// <summary>
        /// Amount of time each frame of animation should be visible.  Measured in seconds.
        /// </summary>
        public double TimePerFrame { get; set; }

        /// <summary>
        /// The amount of time the currently playing frame has been visible.
        /// </summary>
        protected double ElapsedFrameTime { get; set; }

        /// <summary>
        /// The index of the currently playing frame of FrameType.
        /// </summary>
        protected int Frame { get; set; }

        /// <summary>
        /// The currently playing sprite.
        /// </summary>
        public virtual Texture2D CurrentSprite { get; }

        /// <summary>
        /// The width of the currently playing sprite.
        /// </summary>
        public virtual int Width { get => CurrentSprite.Width; }

        /// <summary>
        /// The height of the currently playing sprite.
        /// </summary>
        public virtual int Height { get => CurrentSprite.Height; }

        /// <summary>
        /// Update this sprite.  Depending on the animationType, will continue current animation cycle or begin a new one.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="animationType">The type of animation cycle to perform.</param>
        public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// Draw this sprite at the given world coordinates, using the given SpriteBatch.
        /// </summary>
        /// <param name="x">x world coordinate.</param>
        /// <param name="y">y world coordinate.</param>
        /// <param name="spriteBatch"></param>
        /// <param name="width">The width for the sprite to be stretched to.  Leave empty or -1 for default.</param>
        /// <param name="height">The height for the sprite to be stretched to.  Leave empty or -1 for default.</param>
        public void Draw(int x, int y, SpriteBatch spriteBatch, int width = -1, int height = -1) {
            width = width == -1 ? CurrentSprite.Width : width;
            height = height == -1 ? CurrentSprite.Height : height;
            spriteBatch.Draw(CurrentSprite, new Rectangle(x - width / 2, y - height / 2, width, height), Color.White);
        }


        /// <summary>
        /// Draw this sprite at the given world coordinates, using the given SpriteBatch.
        /// </summary>
        /// <param name="p">World coordinate.</param>
        /// <param name="spriteBatch"></param>
        /// <param name="width">The width for the sprite to be stretched to.  Leave empty or -1 for default.</param>
        /// <param name="height">The height for the sprite to be stretched to.  Leave empty or -1 for default.</param>
        public void Draw(Point p, SpriteBatch spriteBatch, int width = -1, int height = -1) {
            Draw(p.X, p.Y, spriteBatch, width, height);
        }
    }
}
