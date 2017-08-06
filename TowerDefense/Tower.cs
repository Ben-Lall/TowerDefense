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
    class Tower : Creature {
        /// <summary>
        /// Tile coordinates of the top-left corner of the base of the tower.
        /// </summary>
        override public Point TilePos { get; set; }

        /// <summary>
        /// Tile coordinates of the center of the tower.
        /// </summary>
        override public Point CenterTile { get => new Point(TileX + Width / 2, TileY + Height / 2); }

        /// <summary>
        /// Pixel coordinates of the top-left corner of the base of this tower.
        /// </summary>
        override public Point Pos { get => new Point(TilePos.X * TileWidth, TilePos.Y * TileHeight); }

        public int PxX { get => Pos.X; }
        public int PxY { get => Pos.Y; }

        /// <summary>
        /// Pixel coordinates of the center of the tower.
        /// </summary>
        override public Point CenterPoint { get => new Point(TileWidth * (TileX + Width / 2), TileHeight * (TileY + Height / 2)); }

        /// <summary>
        /// Pixel coordinates from which attack visual effects originate.
        /// </summary>
        override public Point FirePoint { get => new Point(TileWidth * (TileX + Width / 2), TileHeight * (TileY - 2 * (Height) / 3)); }

        /// <summary>
        /// Hitpoints of damage dealt by the tower's attacks.
        /// </summary>
        override public int AttackDamage { get => Template.AttackDamage; set => Template.AttackDamage = value; }

        /// <summary>
        /// Tower's attack range, measured in units of tileWidth.
        /// </summary>
        override public double AttackRange { get => Template.AttackRange; set => Template.AttackRange = value; }

        /// <summary>
        /// Frequency of this tower's attacks, measured in hertz.
        /// </summary>
        override public double AttackRate { get => Template.AttackRate; set => Template.AttackRate = value; }

        /// <summary>
        /// Tower's maximum possible health.
        /// </summary>
        override public int MaxHealth { get => Template.MaxHealth; set => Template.MaxHealth = value; }

        /// <summary>
        /// Template used to build the tower.
        /// </summary>
        public TowerTemplate Template { get; set; }
        
        /// <summary>
        /// Type of the tower.
        /// </summary>
        public TowerType Type { get => Template.Type; set => Template.Type = value; }

        /// <summary>
        /// Sprite used for the tower.
        /// </summary>
        override public Texture2D Sprite { get => Template.Sprite; set => Template.Sprite = value; }
        
        /// <summary>
        /// Width of the base of the tower, measured in units of tiles.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of the base of the tower, measured in units of tiles.
        /// </summary>
        public int Height { get; set; }
        
        /// <summary>
        /// Pixel coordinate to where the tower should be drawn.
        /// </summary>
        public Point DrawPos { get => new Point(TilePos.X * TileWidth - (SpriteWidth - Width * TileWidth) / 2, (TilePos.Y * TileHeight) - SpriteHeight + TileHeight * Height); }

        /// <summary>
        /// Constructor for a Tower, using a TowerTemplate
        /// </summary>
        /// <param name="template">Template used to construct this tower.</param>
        /// <param name="pos">Coordinate position of the top-left corner of this tower, in units of tiles.</param>
        public Tower(TowerTemplate template, Point pos) {
            
            Template = template;
            TilePos = pos;
            Width = template.Width;
            Height = template.Height;
            MaxHealth = template.MaxHealth;
            CurrentHealth = MaxHealth;
        }

        /// <summary>
        /// Draw the tower to its current position on the screen.
        /// </summary>
        public void Draw() {
            if (Selected) {
                Graphics.DrawLine(PxX, PxY, Width * TileWidth, 1, Color.Green, WorldSpriteBatch);
                Graphics.DrawLine(PxX, (TileY + Height) * TileHeight, Width * TileWidth, 1, Color.Green, WorldSpriteBatch);
                Graphics.DrawLine(PxX, PxY, 1, Height * TileHeight, Color.Green, WorldSpriteBatch);
                Graphics.DrawLine((TileX + Width) * TileWidth, PxY, 1, Height * TileHeight, Color.Green, WorldSpriteBatch);

                //TODO: add aura
            }
            WorldSpriteBatch.Draw(Sprite, new Rectangle(DrawPos, new Point(SpriteWidth, SpriteHeight)), Color.White);
            if (CurrentHealth < MaxHealth) {
                Rectangle healthBarBox = new Rectangle(DrawPos + new Point(SpriteWidth / 6 + 1, SpriteHeight + 2), new Point(SpriteWidth * 2 / 3, 10));
                Graphics.DrawHealthBar(1.0 * CurrentHealth / MaxHealth, healthBarBox);
            }
        }

        /// <summary>
        /// Draw the firing range of the tower.
        /// </summary>
        public void DrawAttackRange() {
            Graphics.DrawCircle(CenterPoint, (int)(AttackRange * TileWidth), WorldSpriteBatch);
        }


        /// <summary>
        /// Have the tower take an action.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="monsters">List of monsters, in the event that it needs to fire.</param>
        public void Update(GameTime gameTime) {
            Cooldown = Math.Max(0, Cooldown - gameTime.ElapsedGameTime.TotalSeconds);
            if(Cooldown == 0) {
                Attack(Monsters);
            }
        }

        /// <summary>
        /// Given a list of monsters, attack one.  The monster attacked depends on the tower's AI.
        /// </summary>
        /// <param name="monsters">List of monsters.</param>
        public void Attack(List<Monster> monsters) {
            //TODO: come up with various AI packs to determine tower firing strategies.  For now, pick the monster closest to its goal.
            int lowestDistance = int.MaxValue;
            Monster target = null;
            foreach(Monster m in monsters) {
                if (Intersects(this, m)) {
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
        /// Deals an amount of damage to the tower.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            CurrentHealth = Math.Max(0, CurrentHealth - damage);
        }

        /// <summary>
        /// Determine if this tower is placed on top of the given tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns>Return true if this tower overlaps the given tile. False otherwise.</returns>
        public bool ContainsTile(Tile tile) {
            return (TileX <= tile.X && tile.X <= TileX + Width && TileY <= tile.Y && tile.Y <= TileY + Height);
        }

        /// <summary>
        /// Determine if this tower is placed on top of the given point.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>Return true if this tower overlaps the given point. False otherwise.</returns>
        public bool ContainsTile(Point p) {
            return (TileX <= p.X && p.X < TileX + Width && TileY <= p.Y && p.Y < TileY + Height);
        }
    }
}
