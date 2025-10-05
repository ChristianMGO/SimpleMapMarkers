using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using SimpleMapMarkers.Configs;

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
            var config = ModContent.GetInstance<SimpleMapMarkersConfig>();
            bool showBackground = config.TooltipBackground;

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

                    if (SimpleMapMarkers.RemoveMarkerKeybind.JustPressed)
                    {
                        ModContent.GetInstance<SimpleMapMarkers>().Logger.Info($"Attempting to remove marker at index {i}: {MarkerSystem.Markers[i].Name}");
                        MarkerSystem.RemoveMarker(MarkerSystem.Markers[i].ID);
                        Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.MenuClose);
                        return;
                    }
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
                if (showBackground)
                {
                    Terraria.ModLoader.UI.UICommon.TooltipMouseText(GetMarkerTooltipText(hoveredMarker)); // Ensure tooltip is drawn
                }
                else
                {
                    Main.instance.MouseText(GetMarkerTooltipText(hoveredMarker)); // Draw without background
                }
            }
        }

        private Texture2D GetMarkerIconTexture(int iconID)
        {
            // Handle special icons
            switch (iconID)
            {
                case -5: // Yellow custom marker icon
                    return ModContent.Request<Texture2D>(ItemIconRegistry.CustomMarkerYellowPath).Value;
                case -4: // Green custom marker icon
                    return ModContent.Request<Texture2D>(ItemIconRegistry.CustomMarkerGreenPath).Value;
                case -3: // Blue custom marker icon
                    return ModContent.Request<Texture2D>(ItemIconRegistry.CustomMarkerBluePath).Value;
                case -2: // Red custom marker icon
                    return ModContent.Request<Texture2D>(ItemIconRegistry.CustomMarkerRedPath).Value;
                case -1: // House icon
                    return Main.Assets.Request<Texture2D>(ItemIconRegistry.HouseIconPath).Value;
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

        private string GetMarkerTooltipText(Marker marker)
        {
            string markerName = marker.Name;

            string tooltipText = markerName;

            if (marker.IsPublic && !string.IsNullOrEmpty(marker.OwnerName))
            {
                tooltipText += $"\nBy: {marker.OwnerName}";
            }
            return tooltipText;
        }
    }
}