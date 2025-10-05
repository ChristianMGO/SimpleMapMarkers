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
            int hoveredMarkerIndex = -1;
            float closestDistance = 30f; // Max distance to be considered "hovering"

            for (int i = 0; i < MarkerSystem.Markers.Count; i++)
            {
                var marker = MarkerSystem.Markers[i];
                float tileX = marker.Position.X / 16f;
                float tileY = marker.Position.Y / 16f;

                Vector2 mapCenter = Main.mapFullscreenPos;
                float mapScale = Main.mapFullscreenScale;
                Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight);

                Vector2 markerOnMap = (new Vector2(tileX, tileY) - mapCenter) * mapScale + screenSize / 2f;

                // Check if mouse is hovering over this marker
                Vector2 mousePos = new Vector2(Main.mouseX, Main.mouseY);
                float distance = Vector2.Distance(mousePos, markerOnMap);
                
                bool isHovered = distance < closestDistance;
                if (isHovered && (hoveredMarkerIndex == -1 || distance < closestDistance))
                {
                    hoveredMarkerIndex = i;
                    closestDistance = distance;
                }

                // Get icon texture based on marker's IconID
                Texture2D markerTexture = GetMarkerIconTexture(marker.IconID);
                if (markerTexture == null)
                    continue;

                // Scale marker slightly larger when hovered
                float scale = isHovered ? 1.2f : 1f;
                
                // Scale down large textures to fit nicely on map
                if (markerTexture.Width > 32 || markerTexture.Height > 32)
                {
                    float textureScale = 32f / System.Math.Max(markerTexture.Width, markerTexture.Height);
                    scale *= textureScale;
                }

                Main.spriteBatch.Draw(
                    markerTexture,
                    markerOnMap,
                    null,
                    Color.White,
                    0f,
                    new Vector2(markerTexture.Width, markerTexture.Height) / 2f,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }

            // Draw tooltip for hovered marker
            if (hoveredMarkerIndex != -1)
            {
                var hoveredMarker = MarkerSystem.Markers[hoveredMarkerIndex];
                DrawMarkerTooltip(hoveredMarker);
            }
        }

        private Texture2D GetMarkerIconTexture(int iconID)
        {
            // Handle special icons
            if (iconID == -2) // Custom marker icon
            {
                try
                {
                    return ModContent.Request<Texture2D>("SimpleMapMarkers/Assets/Images/MapMarkers/marker_icon").Value;
                }
                catch
                {
                    return null;
                }
            }
            else if (iconID == -1) // House icon
            {
                try
                {
                    return Main.Assets.Request<Texture2D>(ItemIconRegistry.HouseIconPath).Value;
                }
                catch
                {
                    // Fallback to bed item
                    return Terraria.GameContent.TextureAssets.Item[Terraria.ID.ItemID.Bed].Value;
                }
            }
            
            // Regular item icons
            if (iconID > 0 && iconID < Terraria.ID.ItemID.Count)
            {
                try
                {
                    return Terraria.GameContent.TextureAssets.Item[iconID].Value;
                }
                catch
                {
                    return null;
                }
            }

            // Fallback - shouldn't reach here normally
            return null;
        }

        private void DrawMarkerTooltip(Marker marker)
        {
            // Get marker info
            string markerName = marker.Name;
            int tileX = (int)(marker.Position.X / 16f);
            int tileY = (int)(marker.Position.Y / 16f);
            string coordinates = $"({tileX}, {tileY})";

            // Build tooltip text
            string tooltipText = markerName + "\n" + coordinates;
            
            // Add owner name if it's a public marker
            if (marker.IsPublic && !string.IsNullOrEmpty(marker.OwnerName))
            {
                tooltipText += $"\nBy: {marker.OwnerName}";
            }

            // Use game's tooltip system
            Main.hoverItemName = tooltipText;
        }
    }
}