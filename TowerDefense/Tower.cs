using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense {
    /// <summary>
    /// A tower gameplay object.
    /// </summary>
    class Tower {
        /// <summary>
        /// The template used to build this tower.
        /// </summary>
        public TowerTemplate Template { get; set; }

        /// <summary>
        /// The hitpoints of damage dealt by this tower's attacks.
        /// </summary>
        public int AttackDamage { get => Template.AttackDamage; set => Template.AttackDamage = value; }

        /// <summary>
        /// Frequency of this tower's attacks, measured in hertz.
        /// </summary>
        public double AttackRate { get => Template.AttackRate; set => Template.AttackRate = value; }

        /// <summary>
        /// This tower's attack range, measured in units of tileWidth.
        /// </summary>
        public double AttackRange { get => Template.AttackRange; set => Template.AttackRange = value; }
        
        /// <summary>
        /// The current time until this tower's next attack.
        /// </summary>
        public double Cooldown { get; set; }

        /// <summary>
        /// This tower's maximum possible health.
        /// </summary>
        public int MaxHealth { get => Template.MaxHealth; set => Template.MaxHealth = value; }

        /// <summary>
        /// Integer representing this tower's current health.  Should always be > 0.
        /// </summary>
        public int CurrentHealth { get; set; }

        /// <summary>
        /// Whether or not this tower is alive.
        /// </summary>
        public bool IsAlive { get => CurrentHealth > 0; }

        /// <summary>
        /// The type of this tower.
        /// </summary>
        public TowerType Type { get => Template.Type; set => Template.Type = value; }

        public Texture2D Sprite { get => Template.Sprite; set => Template.Sprite = value; }
        public int SpriteWidth { get => Sprite.Width; }
        public int SpriteHeight { get => Sprite.Height; }

        /// <summary>
        /// This tower's firing range, measured in units of pixels.
        /// </summary>
        public Point PixelRadius { get => new Point((int)(AttackRange * Settings.TileWidth), (int)(AttackRange * Settings.TileHeight)); }

        /// <summary>
        /// The tile coordinates of the top-left corner of the base of this tower.
        /// </summary>
        public Point Pos { get; set; }

        public int X { get => Pos.X; set => Pos = new Point(value, Y); }
        public int Y { get => Pos.Y; set => Pos = new Point(X, value); }

        /// <summary>
        /// The pixel coordinates of the top-left corner of the base of this tower.
        /// </summary>
        public Point PixelPos { get => new Point(Pos.X * Settings.TileWidth, Pos.Y * Settings.TileHeight); }
        
        public int PxX { get => PixelPos.X; }
        public int PxY { get => PixelPos.Y; }

        /// <summary>
        /// Boolean representing whether or not this tower has been selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// The width of the base of this tower, measured in units of tiles.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the base of this tower, measured in units of tiles.
        /// </summary>
        public int Height { get; set; }

        public Point CenterPoint { get => new Point(Settings.TileWidth * (X + Width / 2), Settings.TileHeight * (Y + Height / 2)); }
        public Point CenterTile { get => new Point(X + Width / 2, Y + Height / 2); }
        public Point FirePoint { get => new Point(Settings.TileWidth * (X + Width / 2), Settings.TileHeight * (Y - 2 * (Height) / 3)); }

        /// <summary>
        /// The pixel coordinate to where this tower should be drawn.
        /// </summary>
        public Point DrawPos { get => new Point(Pos.X * Settings.TileWidth - (SpriteWidth - Width * Settings.TileWidth) / 2, (Pos.Y * Settings.TileHeight) - SpriteHeight + Settings.TileHeight * Height); }

        /// <summary>
        /// Constructor for a Tower, using a TowerTemplate
        /// </summary>
        /// <param name="template">Template used to construct this tower.</param>
        /// <param name="pos">Coordinate position of the top-left corner of this tower, in units of tiles.</param>
        public Tower(TowerTemplate template, Point pos) {
            Template = template;
            Pos = pos;
            Width = template.Width;
            Height = template.Height;
            MaxHealth = template.MaxHealth;
            CurrentHealth = MaxHealth;
        }

        /// <summary>
        /// Draw this tower to its current position on the screen.
        /// </summary>
        /// <param name="spritebatch"></param>
        public void Draw(SpriteBatch spriteBatch) {
            if (Selected) {
                Graphics.DrawLine(spriteBatch, PxX - Globals.ViewportPx.X, PxY - Globals.ViewportPx.Y, Width * Settings.TileWidth, 1, Color.Green);
                Graphics.DrawLine(spriteBatch, PxX - Globals.ViewportPx.X, (Y + Height) * Settings.TileHeight - Globals.ViewportPx.Y, Width * Settings.TileWidth, 1, Color.Green);
                Graphics.DrawLine(spriteBatch, PxX - Globals.ViewportPx.X, PxY - Globals.ViewportPx.Y, 1, Height * Settings.TileHeight, Color.Green);
                Graphics.DrawLine(spriteBatch, (X + Width) * Settings.TileWidth - Globals.ViewportPx.X, PxY - Globals.ViewportPx.Y, 1, Height * Settings.TileHeight, Color.Green);

                //TODO: add aura
            }
            spriteBatch.Draw(Sprite, new Rectangle(DrawPos - Globals.ViewportPx, new Point(SpriteWidth, SpriteHeight)), Color.White);
        }

        /// <summary>
        /// Draw the firing range of this tower.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawFiringRange(SpriteBatch spriteBatch) {
            Graphics.DrawCircle(spriteBatch, CenterPoint - Globals.ViewportPx, (int)(AttackRange * Settings.TileWidth));
        }


        /// <summary>
        /// Have this tower take an action.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="monsters">List of monsters, in the event that it needs to fire.</param>
        public void Update(GameTime gameTime, List<Monster> monsters) {
            Cooldown = Math.Max(0, Cooldown - gameTime.ElapsedGameTime.TotalSeconds);
            if(Cooldown <= 0) {
                Attack(monsters);
            }
        }

        /// <summary>
        /// Given a list of monsters, attack one.  The monster attacked depends on this tower's AI.
        /// </summary>
        /// <param name="monsters">List of monsters.</param>
        public void Attack(List<Monster> monsters) {
            //TODO: come up with various AI packs to determine tower firing strategies.  For now, pick the monster closest to its goal.
            int lowestDistance = int.MaxValue;
            Monster target = null;
            foreach(Monster m in monsters) {
                if (Globals.Intersects(this, m)) {
                      if(m.DistanceToTarget < lowestDistance) {
                        lowestDistance = m.DistanceToTarget;
                        target = m;
                    }
                }
            }

            if(target != null) {
                target.TakeDamage(AttackDamage);
                Cooldown += (1.0 / AttackRate);
                Globals.effects.Add(new Bolt(FirePoint.ToVector2(), target.CenterPoint.ToVector2(), Color.White, (float)(1.0 / AttackRate)));
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
            return (X <= tile.X && tile.X <= X + Width && Y <= tile.Y && tile.Y <= Y + Height);
        }

        /// <summary>
        /// Determine if this tower is placed on top of the given point.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>Return true if this tower overlaps the given point. False otherwise.</returns>
        public bool ContainsTile(Point p) {
            return (X <= p.X && p.X < X + Width && Y <= p.Y && p.Y < Y + Height);
        }
    }
}
