using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.Globals;

namespace TowerDefense {
    static class Input {
        /// <summary>
        /// Toggle boolean for the Pause button.
        /// </summary>
        private static bool PausePressed { get; set; }

        /// <summary>
        /// Toggle boolean for the mouse button.
        /// </summary>
        private static bool MousePressed { get; set; }

        /// <summary>
        /// Toggle boolean for the grid toggle button.
        /// </summary>
        private static bool GridPressed { get; set; }

        /// <summary>
        /// Toggle boolean for back button.
        /// </summary>
        private static bool BackPressed { get; set; }

        /// <summary>
        /// Toggle boolean for the tile mode button.
        /// </summary>
        private static bool TileModePressed { get; set; }

        /// <summary>
        /// Toggle boolean for the map toggle.
        /// </summary>
        private static bool MapButtonPressed { get; set; }

        /// <summary>
        /// Toggle boolean for the delete button.
        /// </summary>
        private static bool DeletePressed { get; set; }

        /// <summary>
        /// Previously recorded mouse wheel scroll.
        /// </summary>
        public static int PreviousMouseWheel { get; set; }

        /// <summary>
        /// Handle user input during the gamestate where the title is displayed.
        /// </summary>
        public static void HandleTitleInput() {
            // Update mouseState
            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && !MousePressed) {
                HandleLeftMouseClick(mouseState);
                MousePressed = true;
            } else if (mouseState.LeftButton == ButtonState.Released) {
                MousePressed = false;
            }
        }


        /// <summary>
        /// Handle user input during the gamestate where the game is playing.
        /// </summary>
        public static void HandleGameInput() {
            // Update mouseState
            MouseState mouseState = Mouse.GetState();

            /** Mouse handling **/
            if (mouseState.LeftButton == ButtonState.Pressed && !MousePressed) {
                HandleLeftMouseClick(mouseState);
                MousePressed = true;
            } else if (mouseState.LeftButton == ButtonState.Released) {
                MousePressed = false;
            }
            // Scroll Wheel
            if(mouseState.ScrollWheelValue < PreviousMouseWheel) {
                Camera.Zoom -= 0.2f;
            }
            if (mouseState.ScrollWheelValue > PreviousMouseWheel) {
                Camera.Zoom += 0.2f;
            }
            PreviousMouseWheel = mouseState.ScrollWheelValue;


            /** Keyboard Handling **/
            // Back/cancel
            if (!BackPressed && Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                BackPressed = true;
                if (ActivePlayer.MapOverlayToggled) {
                    ActivePlayer.ToggleMapUI();
                } else if (IsPlacingTower) {// Stop tower placement
                    IsPlacingTower = false;
                    PendingTowerTemplate = null;
                } else if (!IsPlacingTower) {
                    ClearTowerIllumination();
                }
            } else if (Keyboard.GetState().IsKeyUp(Keys.Escape)) {
                BackPressed = false;
            }

            // Pause
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !PausePressed) {
                // Toggle pause if menu is not open
                Paused = !Paused;
                PausePressed = true;
            } else if (Keyboard.GetState().IsKeyUp(Keys.Space) && PausePressed) {
                PausePressed = false;
            }

            // Grid overlay toggle
            if (Keyboard.GetState().IsKeyDown(Keys.G) && !GridPressed) {
                // Toggle grid overlay.
                GridToggle = !GridToggle;
                GridPressed = true;
            } else if (Keyboard.GetState().IsKeyUp(Keys.G) && GridPressed) {
                GridPressed = false;
            }

            // Tile draw mode
            if (Keyboard.GetState().IsKeyDown(Keys.H) && !TileModePressed) {
                TileModePressed = true;
                TileMode = (Include.TileDrawMode)(((int)(TileMode) + 1) % (int)(Include.TileDrawMode.TotalDrawModes));
            }
            if (Keyboard.GetState().IsKeyUp(Keys.H) && TileModePressed) {
                TileModePressed = false;
            }

            // Map overlay toggle
            if (Keyboard.GetState().IsKeyDown(Keys.M) && !MapButtonPressed) {
                ActivePlayer.ToggleMapUI();
                MapButtonPressed = true;
            } else if (Keyboard.GetState().IsKeyUp(Keys.M) && MapButtonPressed) {
                MapButtonPressed = false;
            }

            // Delete button
            if (Keyboard.GetState().IsKeyDown(Keys.Delete) && !DeletePressed) {
                DeletePressed = true;
                foreach(Tower t in Towers) {
                    if(t.Selected) {
                        t.CurrentHealth = 0;
                    }
                }
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Delete) && DeletePressed) {
                DeletePressed = false;
            }

            // Movement keys
            int movementMul = 1;
            if(Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift)) {
                movementMul = 10;
            }

            Vector2 movement = Vector2.Zero;
            if (Keyboard.GetState().IsKeyDown(Keys.W)) {
                movement.Y--;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) {
                movement.Y++;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A)) {
                movement.X--;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) {
                movement.X++;
            }
            // Normalize vector
            if (movement.X != 0 && movement.Y != 0) {
                movement.Normalize();
            }
            movement *= movementMul;
            // Move Change the player's active direction.
            ActivePlayer.Direction = movement;
        }

        /// <summary>
        /// Handle a left mouse click event.
        /// </summary>
        /// <param name="mouseState">The mouse's current state.</param>
        private static void HandleLeftMouseClick(MouseState mouseState) {
            object selectedItem = GetClickedObject(mouseState);
            if (selectedItem != null) {
                if (UIPanel.IsUIElement(selectedItem)) { // if a UIPanel was pressed
                    ((UIPanel)selectedItem).Click(mouseState.Position);
                } else if (selectedItem.GetType() == typeof(Tower)) { // if a tower was clicked
                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift)) { // and shift was held
                        ((Tower)selectedItem).Selected = !((Tower)selectedItem).Selected;
                    } else { // shift was not held
                        ClearTowerIllumination();
                        ((Tower)selectedItem).Selected = true;
                    }
                }
            } else if (IsPlacingTower && CursorIsOnMap() && ValidTowerLocation()) {
                PlacePendingTower();
            } else { // Actions that would deselect the selected towers on mouse click
                ClearTowerIllumination();
            }
        }

        /// <summary>
        /// Get the object that the mouse is hovering over.
        /// </summary>
        /// <param name="mouseState">The mouse's current state.</param>
        /// <returns>The object that the mouse is hovering over, or null if it isn't mousing over any object.</returns>
        private static object GetClickedObject(MouseState mouseState) {
            // Check if a UI element was clicked.
            List<UIPanel> UI = new List<UIPanel>();
            if(CurrentGameState == GameState.Title) {
                UI = Title.UIPanels;
            } else {
                UI = UIPanels;
            }
            foreach (UIPanel u in UI) {
                if (u.Contains(mouseState.Position)) {
                    return u;
                }
            }

            // Next, check if a tower was selected
            if (CurrentGameState == GameState.Playing && CursorIsOnMap()) {
                Point clickedTile = PixelToTile(WorldMousePos.ToPoint());
                foreach (Tower t in Towers) {
                    if (t.ContainsTile(clickedTile)) {
                        return t;
                    }
                }
            }
            return null;
        }
    }
}    