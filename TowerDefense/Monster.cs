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

enum CombatType { MELEE, RANGED, NUMBER_OF_COMBAT_TYPES }

namespace TowerDefense {
    /// <summary>
    /// A monster in the game world.
    /// </summary>
    class Monster : GameplayObject {
        
        /// <summary>
        /// Type of this monster.
        /// </summary>
        public MonsterType Type { get; set; }

        /// <summary>
        /// The type of fighter this monster is.
        /// </summary>
        public CombatType CombatType;

        /// <summary>
        /// Monster's speed, measured in tiles / second.
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// Pathfinder for this monster.
        /// </summary>
        private Pathfinder pf;

        /// <summary>
        /// Offset used to convert Pos when constructing from a tile coordinate.
        /// </summary>
        private Point TileToPointOffset { get => new Point(TileWidth / 2 - Width / 2, TileHeight / 2 - Height / 2); }

        /// <summary>
        /// Displacement for drawing this sprite, used when drawing melee attacks. Units of pixels.
        /// </summary>
        private Point MeleeDisplacement { get; set; }

        /// <summary>
        /// The box bounding the hitbox of this monster.
        /// </summary>
        public Rectangle BoundingBox { get => new Rectangle(CenterPoint.X - Width / 2, CenterPoint.Y - Height / 2, Width, Height); }
        
        /// <summary>
        /// Monster's target.
        /// </summary>
        public Tower Target { get; set; }

        /// <summary>
        /// Whether this monster is in attack range of its target tower.
        /// </summary>
        public Boolean IsTargetInRange { get => Target != null && Distance(CenterTile, pf.TargetPos) <= AttackRange; }

        /// <summary>
        /// Get the distance between this monster and its target, measured in units of tiles.
        /// </summary>
        /// <returns></returns>
        public int DistanceToTarget { get => pf.Path.Count; }

        /// <summary>
        /// Whether this monster has arrived at its target tile.
        /// </summary>
        public Boolean HasArrived { get => DistanceToTarget == 0; }
        
        /// <summary>
        /// Constructor for a new Monster.
        /// </summary>
        /// <param name="sprite">Sprite for this monster.</param>
        /// <param name="type">Type of monster.</param>
        /// <param name="pos">The tile position of this monster.</param>
        public Monster(CreatureSprite sprite, MonsterType type, Point pos) {
            Sprite = sprite;
            Pos = new Point(pos.X * TileWidth, pos.Y * TileHeight) + TileToPointOffset;
            Type = type;
            switch(Type) {
                case MonsterType.IMP:
                    CombatType = CombatType.MELEE;
                    Speed = 10;
                    MaxHealth = 10;
                    AttackDamage = 3;
                    AttackRange = SQRT2;
                    AttackRate = 1;
                    Width = 8;
                    Height = 18;
                    break;
            }

            CurrentHealth = MaxHealth;
            pf = new Pathfinder(pos, AttackRange);
            Target = pf.Target;

        }
        
        /// <summary>
        /// Draw this monster to its place on the screen.
        /// </summary>
        public override void Draw() {

            Graphics.DrawLine(BoundingBox.X, BoundingBox.Y, BoundingBox.Width, 1, Color.Green, WorldSpriteBatch);
            Graphics.DrawLine(BoundingBox.X, BoundingBox.Y + BoundingBox.Height, BoundingBox.Width, 1, Color.Green, WorldSpriteBatch);
            Graphics.DrawLine(BoundingBox.X, BoundingBox.Y, 1, BoundingBox.Height, Color.Green, WorldSpriteBatch);
            Graphics.DrawLine(BoundingBox.X + BoundingBox.Width, BoundingBox.Y, 1, BoundingBox.Height, Color.Green, WorldSpriteBatch);

            Sprite.Draw(CenterPoint + MeleeDisplacement, WorldSpriteBatch);

            if (CurrentHealth < MaxHealth) {
                Rectangle healthBarBox = new Rectangle(Pos + new Point(0, SpriteHeight + 2), new Point(SpriteWidth, 10));
                Graphics.DrawHealthBar(1.0 * CurrentHealth / MaxHealth, healthBarBox);
            }
            WorldSpriteBatch.Draw(Art.Pixel, new Rectangle(CenterPoint, new Point(1, 1)), Color.Blue);

        }

        /// <summary>
        /// Update this monster.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            if (Cooldown > 0) { // Handle cooldown before this monster can take any other actions.
                Cooldown = Math.Max(0, Cooldown - gameTime.ElapsedGameTime.TotalSeconds);
                if (CombatType == CombatType.MELEE) { // Give displacement for drawing "bash" attack animation
                    Point nextTilePos = new Point(pf.TargetPos.X * TileWidth, pf.TargetPos.Y * TileHeight) + new Point(TileWidth / 2, TileHeight / 2);
                    Vector2 dirVector = Vector2.Normalize((nextTilePos - CenterPoint).ToVector2());
                    if (Double.IsNaN(dirVector.X) || Double.IsNaN(dirVector.Y)) {
                        dirVector = Vector2.Zero;
                    }

                    ((CreatureSprite)Sprite).Update(gameTime, dirVector);
                    MeleeDisplacement = (new Vector2(dirVector.X * TileWidth, dirVector.Y * TileHeight) * (float)Cooldown * (float)AttackRate).ToPoint();
                }
            } else {
                if (!HasArrived) {
                    Move(gameTime);
                    MeleeDisplacement = new Point(0, 0);
                } else {
                    ((CreatureSprite)Sprite).Update(gameTime, Vector2.Zero);
                }

                if (HasArrived && IsTargetInRange && Target != null && Target.IsAlive) {
                    Attack();
                }

                if (Target != null && !Target.IsAlive) {
                    FindNewTarget();
                }
            }
        }

        /// <summary>
        /// Attacks this monster's target.
        /// Assumes that this.Target != null
        /// </summary>
        public override void Attack() {
            Target.TakeDamage(AttackDamage);
            Cooldown = (1.0 / AttackRate);
            if (CombatType == CombatType.RANGED) {
                Effects.Add(new Bolt(CenterPoint.ToVector2(), Target.CenterPoint.ToVector2(), Color.White, (float)(1.0 / AttackRate)));
            }
        }

        /// <summary>
        /// Have this monster find a new target.
        /// </summary>
        public void FindNewTarget() {
            pf = new Pathfinder(TilePos, AttackRange);
            Target = pf.Target;
        }

        /// <summary>
        /// Deals an amount of damage to this monster.
        /// </summary>
        /// <param name="damage"></param>
        public override void TakeDamage(int damage) {
            CurrentHealth = Math.Max(0, CurrentHealth - damage);
        }

        /// <summary>
        /// Draw the path this monster is currently on.
        /// </summary>
        public void DrawPath() {
            foreach (Tile t in pf.Path) {
                WorldSpriteBatch.Draw(Art.Pixel, new Rectangle(t.X * TileWidth, t.Y * TileHeight, TileWidth, TileHeight), Color.Crimson * 0.5f);
            }
        }

        /// <summary>
        /// Move this monster towards its next tile.  Assumes pf.Count > 0
        /// </summary>
        /// <param name="gameTime"></param>
        public void Move(GameTime gameTime) {
            Point nextTileCoord = pf.Path.First().Pos;
            Point nextTilePos = new Point(nextTileCoord.X * TileWidth, nextTileCoord.Y * TileHeight) + new Point(TileWidth / 2, TileHeight / 2);
            Vector2 dirVector = Vector2.Normalize((nextTilePos - CenterPoint).ToVector2());
            if (Double.IsNaN(dirVector.X) || Double.IsNaN(dirVector.Y)) {
                dirVector = Vector2.Zero;
            }

            Pos += (new Vector2(dirVector.X * TileWidth, dirVector.Y * TileHeight) * (float)Speed * (float)gameTime.ElapsedGameTime.TotalSeconds).ToPoint();

            // Remove this tile from the path if it has been reached
            if (nextTilePos - CenterPoint == new Point(0, 0)) {
                pf.Path.RemoveFirst();
            }

            // Update sprite with new position data.
            ((CreatureSprite)Sprite).Update(gameTime, dirVector);
        }
    }
}
