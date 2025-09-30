using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Map;
using Terraria.ModLoader;

namespace SimpleMapMarkers
{
    public class MarkerMapLayer : ModMapLayer
    {
        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            if (!Main.mapFullscreen)
                return;

            // Handle right-click marker placement
            HandleRightClickPlacement();

            // Draw existing markers
            DrawMarkers();
        }

        private void HandleRightClickPlacement()
        {
            // Check if the right-click keybind is pressed
            if (SimpleMapMarkers.AddMarkerRightClickKeybind.JustPressed)
            {
                // Get mouse position in screen space
                Vector2 mouseScreen = new Vector2(Main.mouseX, Main.mouseY);

                // Convert to world position
                Vector2 worldPosition = ScreenToWorldPosition(mouseScreen);

                // Store position for UI to use later
                MarkerSystem.PendingMarkerPosition = worldPosition;

                // Close the fullscreen map so UI is visible
                Main.mapFullscreen = false;

                // Open marker creation UI
                ModContent.GetInstance<UISystem>().ShowMarkerCreationUI();

                // Optional: Play a sound or give feedback
                Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.MenuTick);
            }
        }

        private Vector2 ScreenToWorldPosition(Vector2 screenPosition)
        {
            // Get map center position (in tiles)
            Vector2 mapCenter = Main.mapFullscreenPos;
            
            // Get map scale
            float mapScale = Main.mapFullscreenScale;
            
            // Get screen dimensions (already accounts for UI scale internally)
            Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight);
            Vector2 screenCenter = screenSize / 2f;

            // Convert screen position to position relative to screen center
            Vector2 relativeToCenter = screenPosition - screenCenter;

            // Convert to tile position relative to map center
            Vector2 tileRelativeToCenter = relativeToCenter / mapScale;

            // Get absolute tile position
            Vector2 tilePosition = mapCenter + tileRelativeToCenter;

            // Convert tile position to world position (pixels)
            Vector2 worldPosition = tilePosition * 16f;

            return worldPosition;
        }

        private void DrawMarkers()
        {
            Texture2D markerTexture = null;
            try
            {
                markerTexture = ModContent.Request<Texture2D>("SimpleMapMarkers/Assets/Images/MapMarkers/marker_icon").Value;
            }
            catch
            {
                return; // Don't draw if texture failed to load
            }

            if (markerTexture == null)
                return;

            foreach (var marker in MarkerSystem.Markers)
            {
                float tileX = marker.Position.X / 16f;
                float tileY = marker.Position.Y / 16f;

                Vector2 mapCenter = Main.mapFullscreenPos;
                float mapScale = Main.mapFullscreenScale;
                Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight);

                Vector2 markerOnMap = (new Vector2(tileX, tileY) - mapCenter) * mapScale + screenSize / 2f;

                Main.spriteBatch.Draw(
                    markerTexture,
                    markerOnMap,
                    null,
                    Color.White,
                    0f,
                    new Vector2(markerTexture.Width, markerTexture.Height) / 2f,
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}