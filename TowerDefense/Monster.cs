using static Include.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Enum containing the different types of monster.
/// </summary>
enum MonsterType { IMP, NUMBER_OF_MONSTERS }

namespace TowerDefense {
    /// <summary>
    /// A monster in the game world.
    /// </summary>
    class Monster : Creature {
        /// <summary>
        /// Center pixel of this monster.
        /// </summary>
        override public Point CenterPoint { get => (Pos + new Point(SpriteWidth / 2, SpriteHeight / 2)); }
        
        /// <summary>
        /// Type of the monster.
        /// </summary>
        public MonsterType Type { get; set; }

        /// <summary>
        /// Monster's speed, measured in tiles / second.
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// Pathfinder for the monster.
        /// </summary>
        private Pathfinder pf;

        /// <summary>
        /// Offset used to convert Pos when constructing from a tile coordinate.
        /// </summary>
        private Point TileToPointOffset { get => new Point(TileWidth / 2 - SpriteWidth / 2, TileHeight / 2 - SpriteHeight / 2); }
        
        /// <summary>
        /// Monster's target.
        /// </summary>
        public Tower Target { get; set; }

        /// <summary>
        /// Whether the monster is in attack range of its target tower.
        /// </summary>
        public Boolean IsTargetInRange { get => Target != null && Distance(TilePos, Target.CenterTile) <= AttackRange; }
        
        /// <summary>
        /// Get the distance between the monster and its target, measured in units of tiles.
        /// </summary>
        /// <returns></returns>
        public int DistanceToTarget { get => pf.Path.Count; }

        /// <summary>
        /// Whether the monster has arrived at its target tile.
        /// </summary>
        public Boolean HasArrived { get => DistanceToTarget == 0; }
        
        /// <summary>
        /// Constructor for a new Monster.
        /// </summary>
        /// <param name="sprite">Sprite for this monster.</param>
        /// <param name="type">Type of monster.</param>
        /// <param name="pos">The tile position of this monster.</param>
        /// <param name="target">The target for this monster to reach.</param>
        /// <param name="maxHealth">The maximum health of this monster.</param>
        /// <param name="map">The world map.</param>
        public Monster(Texture2D sprite, MonsterType type, Point pos, int maxHealth) {
            Sprite = sprite;
            Type = type;
            Pos = new Point(pos.X * TileWidth, pos.Y * TileHeight) + TileToPointOffset;
            MaxHealth = maxHealth;
            CurrentHealth = MaxHealth;
            pf = new Pathfinder(pos);
            Target = pf.Target;
            Speed = 10;
            AttackDamage = 3;
            AttackRange = 8;
            AttackRate = 1;
        }
        
        /// <summary>
        /// Draw the monster at its current position.
        /// </summary>
        /// <param name="spritebatch"></param>
        public void Draw() {
            Sprites.Draw(Sprite, new Rectangle(Pos - ViewportPx, new Point(SpriteWidth, SpriteHeight)), Color.White);

            Rectangle healthBarBox = new Rectangle(Pos - ViewportPx + new Point(0, SpriteHeight + 2), new Point(SpriteWidth, 10));
            Graphics.DrawHealthBar(1.0 * CurrentHealth / MaxHealth, healthBarBox);
        }

        public void Update(GameTime gameTime) {
            if (!HasArrived) {
                Move(gameTime);
            }
            
            if (IsTargetInRange && Target != null && Target.IsAlive) {
                Cooldown = Math.Max(0, Cooldown - gameTime.ElapsedGameTime.TotalSeconds);
                if (Cooldown == 0) {
                    Attack();
                }
            }

            if (Target != null && !Target.IsAlive) {
                FindNewTarget();
            }
        }

        /// <summary>
        /// Attacks this monster's target.
        /// Assumes that this.Target != null
        /// </summary>
        public void Attack() {
            Target.TakeDamage(AttackDamage);
            Cooldown += (1.0 / AttackRate);
            Effects.Add(new Bolt(CenterPoint.ToVector2(), Target.CenterPoint.ToVector2(), Color.White, (float)(1.0 / AttackRate)));
        }

        /// <summary>
        /// Have this monster find a new target.
        /// </summary>
        public void FindNewTarget() {
            pf = new Pathfinder(TilePos);
            Target = pf.Target;
        }

        /// <summary>
        /// Deals an amount of damage to the monster.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage) {
            CurrentHealth = Math.Max(0, CurrentHealth - damage);
        }

        /// <summary>
        /// Draw the path this monster is currently on.
        /// </summary>
        public void DrawPath() {
            foreach (Tile t in pf.Path) {
                Sprites.Draw(Art.Pixel, new Rectangle(t.X * TileWidth, t.Y * TileHeight, TileWidth, TileHeight), Color.Crimson * 0.5f);
            }
        }

        /// <summary>
        /// Move this monster towards its next tile.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Move(GameTime gameTime) {
            if (pf.Path.Count > 0) {
                Point nextTileCoord = pf.Path.First().Pos;
                Point nextTilePos = new Point(nextTileCoord.X * TileWidth, nextTileCoord.Y * TileHeight) + TileToPointOffset;
                Vector2 dirVector = Vector2.Normalize((nextTilePos - Pos).ToVector2());
                Pos += new Point(
                    (int)(dirVector.X * gameTime.ElapsedGameTime.TotalSeconds * Speed * TileWidth),
                    (int)(dirVector.Y * gameTime.ElapsedGameTime.TotalSeconds * Speed * TileHeight));

                // Remove this tile from the path if it has been reached
                if (nextTilePos - Pos == new Point(0, 0)) {
                    pf.Path.RemoveFirst();
                }
            }
        }
    }
}
