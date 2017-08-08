using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense {
    /// <summary>
    /// Enumerator detailing the types of animations.
    /// </summary>
    enum TowerAnimationType { IDLE, NUMBER_OF_ANIMATION_TYPES }

    /// <summary>
    /// Class that draws animated sprites for creatures.
    /// </summary>
    class TowerSprite : AnimatedSprite {

        /// <summary>
        /// The type of animation currently playing.
        /// </summary>
        private TowerAnimationType FrameType { get; set; }

        /// <summary>
        /// The currently playing sprite.
        /// </summary>
        public override Texture2D CurrentSprite { get => SpriteSheet[(int)FrameType][Frame]; }

        /// <summary>
        /// Constructor for a new AnimatedSprite using a spritesheet.
        /// </summary>
        /// <param name="spriteSheet">2D Array of sprites, built following the structure of AnimationType.</param>
        /// <param name="timePerFrame">The amount of time for each frame to remain visible (measured in seconds).</param>
        public TowerSprite(Texture2D[][] spriteSheet, double timePerFrame = 0.5) {
            SpriteSheet = spriteSheet;
            Frame = 0;
            TimePerFrame = timePerFrame;
        }

        /// <summary>
        /// Constructor for a new AnimatedSprite using another template as 
        /// </summary>
        /// <param name="template">The AnimatedSprite whose sprite data is to be copied.</param>
        /// <param name="timePerFrame">The amount of time for each frame to remain visible (measured in seconds).</param>
        public TowerSprite(AnimatedSprite template, double timePerFrame = 0.5) {
            SpriteSheet = (Texture2D[][])template.SpriteSheet.Clone();
            Frame = 0;
            TimePerFrame = timePerFrame;
        }

        /// <summary>
        /// Update this sprite.  Depending on the animationType, will continue current animation cycle or begin a new one.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="animationType">The type of animation cycle to perform.</param>
        public void Update(GameTime gameTime, TowerAnimationType animationType) {
            if (animationType == FrameType) {
                Frame = (Frame + (int)((ElapsedFrameTime + gameTime.ElapsedGameTime.TotalSeconds) / TimePerFrame)) % SpriteSheet[(int)FrameType].Count();
                ElapsedFrameTime = (ElapsedFrameTime + gameTime.ElapsedGameTime.TotalSeconds) % TimePerFrame;
            } else {
                FrameType = animationType;
                Frame = 0;
                ElapsedFrameTime = 0;
            }
        }
    }
}
