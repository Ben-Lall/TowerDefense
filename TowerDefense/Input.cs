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
        /// Toggle boolean for the rotation button.
        /// </summary>
        private static bool RotatePressed { get; set; }

        /// <summary>
        /// Toggle boolean for back button.
        /// </summary>
        private static bool BackPressed { get; set; }

        /// <summary>
        /// Toggle boolean for the tile mode button.
        /// </summary>
        private static bool TileModePressed { get; set; }

        /// <summary>
        /// Toggle boolean for the delete button.
        /// </summary>
        private static bool DeletePressed { get; set; }

        /// <summary>
        /// Previously recorded mouse wheel scroll.
        /// </summary>
        public static int PreviousMouseWheel { get; set; }

        /// <summary>
        /// Handle user input.
        /// </summary>
        public static void HandleInput() {
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
                Camera.Zoom -= 0.1f;
            }
            if (mouseState.ScrollWheelValue > PreviousMouseWheel) {
                Camera.Zoom += 0.1f;
            }
            PreviousMouseWheel = mouseState.ScrollWheelValue;


            /** Keyboard Handling **/
            // Back/cancel
            if (!BackPressed && Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                BackPressed = true;
                if (IsPlacingTower) {// Stop tower placement
                    IsPlacingTower = false;
                    PendingTowerTemplate = null;
                } else if (!IsPlacingTower) {
                    ClearTowerIllumination();
                } else if (!PausePressed) {
                    //TODO: Open menu
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

            // Map draw mode
            if (Keyboard.GetState().IsKeyDown(Keys.H) && !TileModePressed) {
                TileModePressed = true;
                TileMode = (Include.TileDrawMode)(((int)(TileMode) + 1) % (int)(Include.TileDrawMode.TOTAL_DRAW_MODES));
            }
            if (Keyboard.GetState().IsKeyUp(Keys.H) && TileModePressed) {
                TileModePressed = false;
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
            movement.Normalize();
            if (Double.IsNaN(movement.X))
                movement = Vector2.Zero;
            ActivePlayer.Direction = movement;

            // Rotation
            //if(Keyboard.GetState().IsKeyDown(Keys.E) && !RotatePressed) {
            //    Camera.Rotate((float)(Math.PI / 2));
            //    RotatePressed = true;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.Q) && !RotatePressed) {
            //    Camera.Rotate(-(float)(Math.PI / 2));
            //    RotatePressed = true;
            //}
            //if(Keyboard.GetState().IsKeyUp(Keys.E) && Keyboard.GetState().IsKeyUp(Keys.Q) && RotatePressed) {
            //    RotatePressed = false;
            //}

        }
        /// <summary>
        /// Handle a left mouse click event.
        /// </summary>
        /// <param name="mouseState">The mouse's current state.</param>
        private static void HandleLeftMouseClick(MouseState mouseState) {
            object selectedItem = GetClickedObject(mouseState);
            if (selectedItem != null) {
                if (selectedItem.GetType() == typeof(Button)) { // if a button was pressed
                    BeginTowerPlacement(UlTowers[Include.Globals.Buttons.IndexOf((Button)selectedItem)]);
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
            // Run quick check to see if mouse is within the boundaries of possible tower button placement.
            List<Button> buttons = Include.Globals.Buttons;
            if (buttons.Count > 0 && mouseState.X >= buttons[0].X && mouseState.X <= buttons[0].X + buttons[0].Width) {
                // Then find a button with a matching Y coordinate (if any).
                foreach (Button b in buttons) {
                    if (mouseState.Y >= b.Y && mouseState.Y <= b.Y + b.Height) {
                        return b;
                    }
                }
            }

            // Next, check if a tower was selected
            else if (CursorIsOnMap()) {
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