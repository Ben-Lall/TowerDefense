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
        /// This player's AutoMap.
        /// </summary>
        private AutoMap aMap;

        /// <summary>
        /// Whether or not this player's map has been toggled.
        /// </summary>
        public bool MapOverlayToggled { get => aMap.Visible; }

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
            // The towerPanel
            int menuPanelWidth = ScreenWidth / 8;
            Point buttonSize = new Point(menuPanelWidth / 2, ScreenHeight / 12);
            UIPanel towerPanel = new UIPanel(Art.MenuPanel, new Rectangle(ScreenWidth - menuPanelWidth, 0, menuPanelWidth, ScreenHeight), UIType.TowerPanel, 4);
            towerPanel.AddButton(new Button(buttonSize, Art.TowerButton, Art.Tower, () => BeginTowerPlacement(UlTowers[0])));
            UIElements.Add(towerPanel);
            // The AutoMap
            aMap = new AutoMap(Pos.ToVector2());
            UIElements.Add(aMap);
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
        /// Draw this player's UI to the screen.
        /// </summary>
        public void DrawUI() {
            if(MapOverlayToggled) {
                aMap.Draw(UISpriteBatch);
            } else {
                foreach(UIPanel u in UIElements) {
                    if(u.Visible) {
                        u.Draw(UISpriteBatch);
                    }
                }
            }
            DrawUIText();
        }

        /// <summary>
        /// Draw the UI text to the screen.
        /// </summary>
        private void DrawUIText() {
            String PauseText = "Paused!";
            Vector2 PauseTextSize = Art.Font.MeasureString(PauseText);
            Vector2 PausePosition = new Vector2(ScreenWidth - 5, ScreenHeight - 45) - PauseTextSize;
            if (Paused) {
                UISpriteBatch.DrawString(Art.Font, PauseText, PausePosition, Color.Black);
            }

            String monsterText = "Monsters: " + Monsters.Count;
            Vector2 monsterTextSize = Art.Font.MeasureString(monsterText);
            Vector2 monsterTextPosition = new Vector2(ScreenWidth - 5, PausePosition.Y) - monsterTextSize;
            UISpriteBatch.DrawString(Art.Font, monsterText, monsterTextPosition, Color.Black);
        }

        /// <summary>
        /// Update this player
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            UpdateUI();
            SortCollections();
            aMap.Update();
            // Using the input direction, move the map or the player depending on what's active.
            if (MapOverlayToggled) {
                aMap.PanCamera(Direction);
                ((CreatureSprite)Sprite).Update(gameTime, Vector2.Zero);
            } else {
                ((CreatureSprite)Sprite).Update(gameTime, Direction);
                Move(gameTime);
            }
        }

        /// <summary>
        /// Update the UI to reflect the player settings.
        /// </summary>
        private void UpdateUI() {
            foreach(UIPanel u in UIElements) {
                if(u.Type == UIType.TowerPanel) {
                    u.Visible = !MapOverlayToggled && !IsPlacingTower;
                }
            }
        }

        /// <summary>
        /// Sort all the collections contained within this player as needed.
        /// </summary>
        private void SortCollections() {
            UIPanels.Sort((x, y) => x.Depth.CompareTo(y.Depth));
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

        /// <summary>
        /// Take damage.
        /// </summary>
        /// <param name="damage"></param>
        public override void TakeDamage(int damage) {
            CurrentHealth = Math.Max(0, CurrentHealth - damage);
        }

        /// <summary>
        /// Toggle this player's AutoMap visibility.
        /// </summary>
        public void ToggleMapUI() {
            aMap.Visible = !aMap.Visible;
        }
    }
}
