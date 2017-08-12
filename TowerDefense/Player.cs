using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.Globals;


namespace TowerDefense {
    class Player : GameplayObject {

        /// <summary>
        /// Monster's speed, measured in tiles / second.
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// This player's current direction vector.
        /// </summary>
        public Vector2 Direction { get; set; }

        /// <summary>
        /// Number of pixels minimum distance for spawning a new monster.
        /// </summary>
        public static int SpawnLowerBound { get => 648; }

        /// <summary>
        /// Number of pixels maxmimum distance for spawning a new monster.
        /// </summary>
        public static int SpawnUpperBound { get => 720; }

        /// <summary>
        /// Create a new player at the position.
        /// </summary>
        /// <param name="pos"></param>
        public Player(Point pos) {
            Sprite = new CreatureSprite(Art.Player);
            Pos = pos;
            MaxHealth = 100;
            CurrentHealth = MaxHealth;
            Speed = 20;
        }

        /// <summary>
        /// Draw this player.
        /// </summary>
        public override void Draw() {
            Sprite.Draw(CenterPoint, WorldSpriteBatch);
        }

        /// <summary>
        /// Update this player
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            Sprite.Update(gameTime);
            Move(gameTime);
            ((CreatureSprite)Sprite).Update(gameTime, Direction);
        }

        /// <summary>
        /// Move the player in the direction of the given direction vector.
        /// </summary>
        /// <param name="dir"></param>
        public void Move(GameTime gameTime) {
            Pos += (new Vector2(Direction.X * TileWidth, Direction.Y * TileHeight) * (float)Speed * (float)gameTime.ElapsedGameTime.TotalSeconds).ToPoint();
            Camera.MoveTo(CenterPoint.ToVector2());
        }

        public override void Attack() {
            throw new NotImplementedException();
        }

        public override void TakeDamage(int damage) {
            throw new NotImplementedException();
        }
    }
}
