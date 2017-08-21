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
        /// List of UI elements that need to be rendered for this player.  Sorted in descending order of depth.
        /// </summary>
        public List<UIPanel> UIElements;

        /// <summary>
        /// Create a new player at the position.
        /// </summary>
        /// <param name="pos"></param>
        public Player(Point pos) {
            Sprite = new CreatureSprite(Art.Player);
            Pos = pos;
            MaxHealth = 100;
            CurrentHealth = MaxHealth;
            Speed = 8;

            // Personal UI elements
            UIElements = new List<UIPanel>();
            Rectangle buttonBox = new Rectangle(ScreenWidth - MenuPanelWidth + (MenuPanelWidth / 4), 5, MenuPanelWidth / 2, MenuPanelHeight / 12);
            UIPanel towerPanel = new UIPanel(Art.MenuPanel, new Rectangle(ScreenWidth - MenuPanelWidth, 0, MenuPanelWidth, MenuPanelHeight), null, UIType.TOWERPANEL);
            towerPanel.AddButton(new Button(buttonBox, Art.TowerButton, Art.Tower, () => BeginTowerPlacement(UlTowers[0])));
            UIElements.Add(towerPanel);
        }

        /// <summary>
        /// Draw this player.
        /// </summary>
        public override void Draw() {
            Sprite.Draw(CenterPoint, WorldSpriteBatch);
            if (CurrentHealth < MaxHealth) {
                Rectangle healthBarBox = new Rectangle(CenterPoint + new Point(SpriteWidth / 6 + 1, SpriteHeight + 2), new Point(SpriteWidth * 2 / 3, 10));
                Graphics.DrawHealthBar(1.0 * CurrentHealth / MaxHealth, healthBarBox);
            }
        }

        /// <summary>
        /// Update this player
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            ((CreatureSprite)Sprite).Update(gameTime, Direction);
            Move(gameTime);
        }

        /// <summary>
        /// Move the player in the direction of the given direction vector.
        /// </summary>
        /// <param name="dir"></param>
        public void Move(GameTime gameTime) {
            Pos += (new Vector2(Direction.X * TileWidth, Direction.Y * TileHeight) * (float)Speed * (float)gameTime.ElapsedGameTime.TotalSeconds).ToPoint();
            Camera.MoveTo(CenterPoint.ToVector2());
            Direction = Vector2.Zero;
        }

        public override void Attack() {
            throw new NotImplementedException();
        }

        public override void TakeDamage(int damage) {
            CurrentHealth = Math.Max(0, CurrentHealth - damage);
        }
    }
}
