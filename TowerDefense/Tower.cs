using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.Globals;

namespace TowerDefense {
    /// <summary>
    /// A tower gameplay object.
    /// </summary>
    class Tower : GameplayObject {

        /// <summary>
        /// Pixel coordinates from which attack visual effects originate.
        /// </summary>
        public override Point FirePoint { get => new Point(X + Width / 2, Y - 2 * Height / 3); }

        /// <summary>
        /// Template used to build this tower.
        /// </summary>
        public TowerTemplate Template { get; set; }

        /// <summary>
        /// Hitpoints of damage dealt by this tower's attacks.
        /// </summary>
        public override int AttackDamage { get => Template.AttackDamage; set => Template.AttackDamage = value; }

        /// <summary>
        /// Tower's attack range, measured in units of tileWidth.
        /// </summary>
        public override double AttackRange { get => Template.AttackRange; set => Template.AttackRange = value; }

        /// <summary>
        /// Frequency of this tower's attacks, measured in hertz.
        /// </summary>
        public override double AttackRate { get => Template.AttackRate; set => Template.AttackRate = value; }

        /// <summary>
        /// Tower's maximum possible health.
        /// </summary>
        public override int MaxHealth { get => Template.MaxHealth; set => Template.MaxHealth = value; }
        
        /// <summary>
        /// Type of the tower.
        /// </summary>
        public TowerType Type { get => Template.Type; set => Template.Type = value; }

        /// <summary>
        /// The width of this tower measured in units of tiles.
        /// </summary>
        public int WidthTiles { get => Template.Width; }

        /// <summary>
        /// The height of this tower measured in units of tiles.
        /// </summary>
        public int HeightTiles { get => Template.Height; }

        /// <summary>
        /// Constructor for a Tower, using a TowerTemplate
        /// </summary>
        /// <param name="template">Template used to construct this tower.</param>
        /// <param name="pos">Coordinate position of the top-left corner of this tower, in units of tiles.</param>
        public Tower(TowerTemplate template, Point pos) {
            Template = template;
            Sprite = Template.Sprite;
            TilePos = pos;
            Width = template.Width * TileWidth;
            Height = template.Height * TileHeight;
            MaxHealth = template.MaxHealth;
            CurrentHealth = MaxHealth;
        }

        /// <summary>
        /// Draw this tower to its position on the screen.
        /// </summary>
        public override void Draw() {
            if (Selected) {
                DrawBoundingBox();

                //TODO: add aura
            }

            // Draw this tower, with sprite positioning dependent on tower type.
            switch(Template.Type) {
                case TowerType.BOLT:
                    Sprite.Draw(new Point(CenterPoint.X, CenterPoint.Y - Height / 2), WorldSpriteBatch);
                    break;
                case TowerType.HUB:
                    Sprite.Draw(CenterPoint, WorldSpriteBatch);
                    break;
            }
            

            if (CurrentHealth < MaxHealth) {
                Rectangle healthBarBox = new Rectangle(CenterPoint + new Point(SpriteWidth / 6 + 1, SpriteHeight + 2), new Point(SpriteWidth * 2 / 3, 10));
                Graphics.DrawHealthBar(1.0 * CurrentHealth / MaxHealth, healthBarBox);
            }
        }

        /// <summary>
        /// Have this tower take an action.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            ((TowerSprite)Sprite).Update(gameTime, TowerAnimationType.IDLE);

            Cooldown = Math.Max(0, Cooldown - gameTime.ElapsedGameTime.TotalSeconds);
            if (Cooldown == 0) {
                Attack();
            }
        }

        /// <summary>
        /// Draw the firing range of this tower.
        /// </summary>
        public void DrawAttackRange() {
            Graphics.DrawCircle(CenterPoint, (int)(AttackRange * TileWidth), WorldSpriteBatch);
        }

        /// <summary>
        /// Have this tower attack a monster. The monster attacked depends on this tower's AI.
        /// </summary>
        public override void Attack() {
            //TODO: come up with various AI packs to determine tower firing strategies.  For now, pick the monster closest to its goal.
            float lowestDistance = MapWidth;
            Monster target = null;
            foreach(Monster m in Monsters) {
                if (Intersects(m.BoundingBox)) {
                      if(m.DistanceToTarget < lowestDistance) {
                        lowestDistance = m.DistanceToTarget;
                        target = m;
                    }
                }
            }

            if(target != null) {
                target.TakeDamage(AttackDamage);
                Cooldown += (1.0 / AttackRate);
                Effects.Add(new Bolt(FirePoint.ToVector2(), target.CenterPoint.ToVector2(), Color.White, (float)(1.0 / AttackRate)));
            }
        }

        /// <summary>
        /// Deals an amount of damage to this tower.
        /// </summary>
        /// <param name="damage"></param>
        public override void TakeDamage(int damage)
        {
            CurrentHealth = Math.Max(0, CurrentHealth - damage);
        }

        /// <summary>
        /// Remove this tower's data from the game world.
        /// </summary>
        public void Remove() {
            for(int y = TilePos.Y; y < TilePos.Y + HeightTiles; y++) {
                for (int x = TilePos.X; x < TilePos.X + WidthTiles; x++) {
                    MapAt(x, y).ContainsTower = false;
                }
            }
            HeatMap.Update();
        }

        /// <summary>
        /// Determine if this tower is placed on top of the given tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns>Return true if this tower overlaps the given tile. False otherwise.</returns>
        public bool ContainsTile(Tile tile) {
            return ContainsTile(tile.Pos);
        }

        /// <summary>
        /// Determine if this tower is placed on top of the given point.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>Return true if this tower overlaps the given point. False otherwise.</returns>
        public bool ContainsTile(Point p) {
            return (TileX <= p.X && p.X < TileX + WidthTiles && TileY <= p.Y && p.Y < TileY + HeightTiles);
        }
    }
}
