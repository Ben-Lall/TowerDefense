using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Include.Globals;

namespace TowerDefense {
    /// <summary>
    /// A small single-biome map used as the title's scrolling background.
    /// </summary>
    class SampleMap  {
        /// <summary>
        /// The type of the current sample map.
        /// </summary>
        GeoType SampleType { get; set; }

        /// <summary>
        /// The Tiles that make up this map.
        /// </summary>
        Tile[,] Map { get; set; }

        int MapHeight { get; set; }
        int MapWidth { get; set; }

        /// <summary>
        /// The camera that scrolls over the map.
        /// </summary>
        Camera2d Camera { get; set; }

        /// <summary>
        /// The scroll speed of this camera, measured at a rate of tiles / second.
        /// </summary>
        float ScrollSpeed { get => 5f; }

        /* Fade information */

        // Fade out

        /// <summary>
        /// The amount of time (in seconds) until the camera reaches its maxmimum X value.
        /// </summary>
        float TimeUntilEnd { get => (((float)Camera.CameraMaxX - Camera.CameraStart.X) / TileWidth) / ScrollSpeed; }

        /// <summary>
        /// The time threshold at which to begin fading the background.
        /// </summary>
        float FadeOutThreshold { get => 4; }

        /// <summary>
        /// The current fade percentage.
        /// </summary>
        float FadeOutPercent { get => (FadeOutThreshold - TimeUntilEnd) / FadeOutThreshold; }

        /// <summary>
        /// Whether the fade out has begun.
        /// </summary>
        bool StartFadeOut { get => FadeOutPercent >= 0; }

        /// <summary>
        /// Whether the end of the map has been reached.
        /// </summary>
        bool EndReached { get => FadeOutPercent == 1; }

        // Fade in

        /// <summary>
        /// The amount of time in seconds for which the fade in lasts.
        /// </summary>
        float FadeInDuration { get => 2; } 

        /// <summary>
        /// The x position at which the fade in should be over.
        /// </summary>
        float CeaseFadeX { get => (FadeInDuration * ScrollSpeed) * TileWidth; }

        /// <summary>
        /// The amount of time until the fade in ceases.
        /// </summary>
        float TimeUntilStopFade { get => ((CeaseFadeX - Camera.Pos.X) / TileWidth) / ScrollSpeed; }

        /// <summary>
        /// The current fade in percent.
        /// </summary>
        float FadeInPercent { get => 1f - ((FadeInDuration - TimeUntilStopFade) / FadeInDuration); }

        /// <summary>
        /// Whether the fade in has begun.
        /// </summary>
        bool StartFadeIn { get => FadeInPercent <= 1; }

        /// <summary>
        /// Create a new SampleMap.
        /// </summary>
        void Initialize() {
            SampleType = WorldMap.RandomGeoType;
            MapHeight = MaxScreenHeight / TileHeight;
            MapWidth = 200;
            GenerateMap();
            Camera = new Camera2d(new Vector2(40, MaxScreenHeight / 2), ScreenWidth, ScreenHeight, MapWidth, MapHeight);
            Camera.Move(new Vector2(0, 0));
        }

        /// <summary>
        /// Create a new SampleMap.
        /// </summary>
        public SampleMap() {
            Initialize();
        }

        /// <summary>
        /// Generate a new map filled with various tiles of the current SampleType.
        /// </summary>
        void GenerateMap() {
            Map = new Tile[MapHeight, MapWidth];
            Random r = new Random();
            for (int y = 0; y < MapHeight; y++) {
                for (int x = 0; x < MapWidth; x++) {
                    int ID = WorldMap.GetSpriteIDFromGeoType(SampleType, r);
                    Map[y, x] = new Tile(TileType.Open, x, y, SampleType, ID);
                }
            }
        }

        /// <summary>
        /// Update this SampleMap.
        /// </summary>
        public void Update(GameTime gameTime) {
            Camera.Move(new Vector2(ScrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * TileWidth, 0));
            if(EndReached) {
                Initialize();
            }
        }

        /// <summary>
        /// Draw this SampleMap.
        /// </summary>
        public void Draw() {
            WorldSpriteBatch.Begin(SpriteSortMode.Deferred,
                null, SamplerState.PointClamp, null, null, null, Camera.GetTransformation());

            for (int y = Camera.CameraTileStart.Y; y <= Camera.CameraTileEnd.Y; y++) {
                for (int x = Camera.CameraTileStart.X; x <= Camera.CameraTileEnd.X; x++) {
                    Map[y, x].Draw(Color.White);
                }
            }

            if(StartFadeOut) {
                UISpriteBatch.Draw(Art.Pixel, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White * FadeOutPercent);
            } else if(StartFadeIn) {
                UISpriteBatch.Draw(Art.Pixel, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White * FadeInPercent);
            }
            WorldSpriteBatch.End();
        }
    }
}
