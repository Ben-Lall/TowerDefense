using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.GameState;
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
        /// The camera fixed upon this player.
        /// </summary>
        public Camera2d Camera { get; set; }

        /// <summary>
        /// The render mode for this player's tiles.
        /// </summary>
        public TileDrawMode TileMode { get; set; }

        /// <summary>
        /// The template of the tower being placed by the active player, if any.
        /// </summary>
        public TowerTemplate PendingTowerTemplate { get; set; }

        /// <summary>
        /// Boolean representing whether or not the active player has selected a tower and is working on placing it.
        /// </summary>
        public bool IsPlacingTower { get => PendingTowerTemplate != null; }

        /// <summary>
        /// Whether or not the grid should be overlayed to the screen.
        /// </summary>
        public bool GridToggle { get; set; }

        /// <summary>
        /// A list of the player's placable towers.
        /// </summary>
        public List<TowerTemplate> UlTowers;

        /// <summary>
        /// List of UI elements that need to be rendered for this player.  Sorted in descending order of depth.
        /// </summary>
        public List<UIPanel> UIElements;

        /// <summary>
        /// List of towers selected by this player.
        /// </summary>
        public List<Tower> SelectedTowers;

        /// <summary>
        /// This player's AutoMap.
        /// </summary>
        private AutoMap aMap;

        /// <summary>
        /// This player's escape menu.
        /// </summary>
        private UIPanel Menu;

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
            Camera = new Camera2d(Pos.ToVector2(), ScreenWidth, ScreenHeight);
            SelectedTowers = new List<Tower>();

            UlTowers = new List<TowerTemplate>();
            UlTowers.Add(BoltTowerTemplate);

            TileMode = TileDrawMode.Default;
            GridToggle = false;


            BuildDefaultUI();
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
            UpdateUI();
            UpdateSelectedTowers();
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

        /* UI Methods */

        /// <summary>
        /// Update the UI to reflect the player settings.
        /// </summary>
        private void UpdateUI() {
            foreach (UIPanel u in UIElements) {
                if (u.Type == UIType.TowerPanel) {
                    u.Visible = !MapOverlayToggled && !IsPlacingTower;
                }
            }
        }

        /// <summary>
        /// Draw this player's UI to the screen.
        /// </summary>
        public void DrawUI() {
            if (MapOverlayToggled) {
                aMap.Draw(UISpriteBatch);
            } else {
                foreach (UIPanel u in UIElements) {
                    if (u.Visible) {
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
        /// Build the default UI elements for this player. 
        /// </summary>
        void BuildDefaultUI() {

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

            // The menu
            int width = ScreenWidth / 6;
            int height = ScreenHeight / 6;
            Menu = new UIPanel(Art.MenuPanel, new Rectangle(ScreenWidth / 2 - width / 2, ScreenHeight / 2 - height / 2, width, height), UIType.Menu, 8);
            Menu.AddButton(new Button("Save & Exit", Art.MenuPanel, () => SaveAndExit()));
            UIElements.Add(Menu);
            Menu.Visible = false;
        }

        /// <summary>
        /// Toggle this player's AutoMap visibility.
        /// </summary>
        public void ToggleMapUI() {
            aMap.Visible = !aMap.Visible;
        }

        /// <summary>
        /// Toggle this player's menu.
        /// </summary>
        public void ToggleMenu() {
            Menu.Visible = !Menu.Visible;
        }

        /* Tower Selection Methods */

        /// <summary>
        /// Update the selected towers list.
        /// </summary>
        public void UpdateSelectedTowers() {
            SelectedTowers.RemoveAll(x => !x.IsAlive);
        }

        /// <summary>
        /// Clear this player's list of selected towers.
        /// </summary>
        public void ClearSelectedTowers() {
            SelectedTowers.Clear();
        }

        /// <summary>
        /// Toggle the selection of the given tower.
        /// </summary>
        /// <param name="t"></param>
        public void ToggleSelectedTower(Tower t) {
            if(SelectedTowers.Contains(t)) {
                SelectedTowers.Remove(t);
            } else {
                SelectedTowers.Add(t);
            }
        }

        /// <summary>
        /// Check if there are any towers selected.
        /// </summary>
        public bool HasSelectedTowers() {
            return SelectedTowers.Count > 0;
        }

        /// <summary>
        /// Remove all selected towers.
        /// </summary>
        public void RemoveSelectedTowers() {
            foreach (Tower t in SelectedTowers) {
                t.CurrentHealth = 0;
            }
        }

        /* Utility Methods */

        /// <summary>
        /// Sort all the collections contained within this player as needed.
        /// </summary>
        private void SortCollections() {
            UIElements.Sort((x, y) => x.Depth.CompareTo(y.Depth));
        }
    }
}
