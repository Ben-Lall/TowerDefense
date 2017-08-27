using static Include.GameState;
using static Include.Globals;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

/// <summary>
/// The different types of monster.
/// </summary>
enum MonsterType { Imp, NumberOfMonsters}

/// <summary>
/// The different combat styles a monster can have.
/// </summary>
enum CombatType { Melee, Ranged }

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
        /// Displacement for drawing this sprite, used when drawing melee attacks. Units of pixels.
        /// </summary>
        private Point MeleeDisplacement { get; set; }
        
        /// <summary>
        /// Monster's target.
        /// </summary>
        public GameplayObject Target { get; set; }

        /// <summary>
        /// Whether this monster has arrived at its target tile.
        /// </summary>
        public bool HasArrived { get => HeatMap.HasArrived(CenterPoint, (float)AttackRange) || Intersects(ActivePlayer.BoundingBox); }

        /// <summary>
        /// Whether this monster is currently working at attacking a tower.
        /// </summary>
        public bool TargettingTower { get; set; }

        /// <summary>
        /// Whether this monster is to be simulated.  In order for it to be despawned, it must be out of player range and have no tower to attack.
        /// </summary>
        public bool IsSimulated { get => base.IsInPlayerRange || TargettingTower; }

        /// <summary>
        /// The distance this monster is from a tile it can attack from.
        /// </summary>
        public float DistanceToTarget { get => Math.Max(0, HeatMap.HMapAt(CenterPoint) - (float)AttackRange); }
        
        /// <summary>
        /// Constructor for a new Monster.
        /// </summary>
        /// <param name="sprite">Sprite for this monster.</param>
        /// <param name="type">Type of monster.</param>
        /// <param name="pos">The pixel position of this monster.</param>
        public Monster(CreatureSprite sprite, MonsterType type, Point pos) {
            Sprite = sprite;
            Pos = pos;
            Type = type;
            switch(Type) {
                case MonsterType.Imp:
                    CombatType = CombatType.Melee;
                    Speed = 8.2;
                    MaxHealth = 10;
                    AttackDamage = 3;
                    AttackRange = 0.5;
                    AttackRate = 1;
                    Width = 8;
                    Height = 18;
                    break;
            }

            CurrentHealth = MaxHealth;
        }
        
        /// <summary>
        /// Draw this monster to its place on the screen.
        /// </summary>
        public override void Draw() {
            Sprite.Draw(CenterPoint + MeleeDisplacement, WorldSpriteBatch);
            if (CurrentHealth < MaxHealth) {
                Rectangle healthBarBox = new Rectangle(Pos + new Point(0, SpriteHeight + 2), new Point(SpriteWidth, 10));
                Graphics.DrawHealthBar(1.0 * CurrentHealth / MaxHealth, healthBarBox);
            }
        }

        /// <summary>
        /// Update this monster.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            Debug.Assert(CenterPoint.X >= 0 && CenterPoint.Y >= 0 && CenterPoint.X < MapWidth * TileWidth && CenterPoint.Y < MapHeight * TileHeight, "Violation of: Monster is dimensions of the map");
            if (Cooldown > 0) { // Handle cooldown before this monster can take any other actions.
                Cooldown = Math.Max(0, Cooldown - gameTime.ElapsedGameTime.TotalSeconds);
                if (CombatType == CombatType.Melee) { // Give displacement for drawing "bash" attack animation
                    Vector2 dirVector = HeatMap.GetDirVector(CenterPoint);

                    ((CreatureSprite)Sprite).Update(gameTime, dirVector);
                    MeleeDisplacement = (new Vector2(dirVector.X * TileWidth, dirVector.Y * TileHeight) * (float)Cooldown * (float)AttackRate).ToPoint();
                }
            } else {
                if (!HasArrived) {
                    Move(gameTime);
                } else {
                    ((CreatureSprite)Sprite).Update(gameTime, Vector2.Zero);
                    Attack();
                }
            }
        }

        /// <summary>
        /// Attacks this monster's target.
        /// Assumes that this.Target != null
        /// </summary>
        public override void Attack() {
            if(Target == null || Target != null && (!Target.IsAlive || Target.Equals(ActivePlayer))) {
                if (Intersects(ActivePlayer.BoundingBox)) {
                    Target = ActivePlayer;
                } else {
                    foreach (Tower t in Towers) {
                        if (Intersects(t.BoundingBox)) {
                            Target = t;
                            break;
                        }
                    }
                }
            }

            if (Target != null && Target.IsAlive) {
                Target.TakeDamage(AttackDamage);
                Cooldown = 1.0f / AttackRate;
                if (CombatType == CombatType.Ranged) {
                    Effects.Add(new Bolt(CenterPoint.ToVector2(), Target.CenterPoint.ToVector2(), Color.White, (float)(1.0 / AttackRate)));
                }
            }
        }

        /// <summary>
        /// Deals an amount of damage to this monster.
        /// </summary>
        /// <param name="damage"></param>
        public override void TakeDamage(int damage) {
            CurrentHealth = Math.Max(0, CurrentHealth - damage);
        }

        /// <summary>
        /// Move this monster towards its next tile.  Assumes !HasArrived.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Move(GameTime gameTime) {
            Vector2 dirVector = HeatMap.GetDirVector(CenterPoint);
            TargettingTower = !dirVector.Equals(Vector2.Zero);
            // If direction vector is zero, path towards nearest player.
            if(!TargettingTower) {
                dirVector = Vector2.Normalize((ActivePlayer.CenterPoint - CenterPoint).ToVector2());
                if (Double.IsNaN(dirVector.X))
                    dirVector = Vector2.Zero;
            }
            Pos += (new Vector2(dirVector.X * TileWidth, dirVector.Y * TileHeight) * (float)Speed * (float)gameTime.ElapsedGameTime.TotalSeconds).ToPoint();
            // Update sprite with new position data.
            ((CreatureSprite)Sprite).Update(gameTime, dirVector);
        }
    }
}
