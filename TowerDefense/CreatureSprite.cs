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
    enum CreatureAnimationType { Idle, Left, Right, Up, Down, NumberOfAnimationTypes }

    /// <summary>
    /// Class that draws animated sprites for creatures.
    /// </summary>
    class CreatureSprite : AnimatedSprite {

        /// <summary>
        /// The type of animation currently playing.
        /// </summary>
        private CreatureAnimationType FrameType { get; set; }

        /// <summary>
        /// The currently playing sprite.
        /// </summary>
        public override Texture2D CurrentSprite { get => SpriteSheet[(int)FrameType][Frame]; }

        /// <summary>
        /// Constructor for a new AnimatedSprite using a spritesheet.
        /// </summary>
        /// <param name="spriteSheet">2D Array of sprites, built following the structure of AnimationType.</param>
        /// <param name="timePerFrame">The amount of time for each frame to remain visible (measured in seconds).</param>
        public CreatureSprite(Texture2D[][] spriteSheet, double timePerFrame = 0.2) {
            FrameType = CreatureAnimationType.Idle;
            SpriteSheet = spriteSheet;
            Frame = 0;
            TimePerFrame = timePerFrame;
        }

        /// <summary>
        /// Constructor for a new AnimatedSprite using another template as 
        /// </summary>
        /// <param name="template">The AnimatedSprite whose sprite data is to be copied.</param>
        /// <param name="timePerFrame">The amount of time for each frame to remain visible (measured in seconds).</param>
        public CreatureSprite(AnimatedSprite template, double timePerFrame = 0.2) {
            FrameType = CreatureAnimationType.Idle;
            SpriteSheet = template.SpriteSheet;
            Frame = 0;
            TimePerFrame = timePerFrame;
        }

        /// <summary>
        /// Update this sprite.  Depending on the animationType, will continue current animation cycle or begin a new one.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="dir">The direction this sprite is moving in.</param>
        public void Update(GameTime gameTime, Vector2 dir) {
            // Get the proper animation type
            CreatureAnimationType animationType = CreatureAnimationType.Idle;
            if (!dir.Equals(Vector2.Zero)) {
                if(Math.Abs(dir.X) > Math.Abs(dir.Y)) { // Movement is tending more towards X axis
                    if (dir.X < 0) {
                        animationType = CreatureAnimationType.Left;
                    } else {
                        animationType = CreatureAnimationType.Right;
                    }
                } else { // Movement is tending more towards Y axis
                    if (dir.Y < 0) {
                        animationType = CreatureAnimationType.Up;
                    } else {
                        animationType = CreatureAnimationType.Down;
                    }
                }
            }

            if(animationType == FrameType) {
                Frame = (Frame + (int)((ElapsedFrameTime + gameTime.ElapsedGameTime.TotalSeconds) / TimePerFrame)) % SpriteSheet[(int)FrameType].Count();
                ElapsedFrameTime = ElapsedFrameTime + gameTime.ElapsedGameTime.TotalSeconds;
                if(ElapsedFrameTime > TimePerFrame) {
                    ElapsedFrameTime = 0;
                }
            } else {
                FrameType = animationType;
                Frame = 0;
                ElapsedFrameTime = 0;
            }
        }

    }
}
