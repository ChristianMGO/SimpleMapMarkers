using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SimpleMapMarkers
{
    public class IconButton : UIElement
    {
        private int _iconID;
        private string _iconName;
        private bool _isSelected;
        private bool _isHovered;

        public IconButton(int iconID, string iconName, bool isSelected)
        {
            _iconID = iconID;
            _iconName = iconName;
            _isSelected = isSelected;
        }

        public void SetSelected(bool selected)
        {
            _isSelected = selected;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            _isHovered = true;
            Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            _isHovered = false;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle hitbox = GetDimensions().ToRectangle();

            // Draw background
            Color bgColor = _isSelected ? new Color(255, 215, 0) : // Gold when selected
                           _isHovered ? new Color(255, 255, 100) :  // Light yellow when hovered
                           new Color(40, 60, 120);                   // Dark blue default

            Texture2D pixel = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, hitbox, bgColor);

            // Draw border
            int borderWidth = 2;
            Color borderColor = _isSelected ? Color.Gold : Color.Black;

            // Top, Bottom, Left, Right borders
            spriteBatch.Draw(pixel, new Rectangle(hitbox.X, hitbox.Y, hitbox.Width, borderWidth), borderColor);
            spriteBatch.Draw(pixel, new Rectangle(hitbox.X, hitbox.Y + hitbox.Height - borderWidth, hitbox.Width, borderWidth), borderColor);
            spriteBatch.Draw(pixel, new Rectangle(hitbox.X, hitbox.Y, borderWidth, hitbox.Height), borderColor);
            spriteBatch.Draw(pixel, new Rectangle(hitbox.X + hitbox.Width - borderWidth, hitbox.Y, borderWidth, hitbox.Height), borderColor);

            // Draw icon
            Texture2D iconTexture = GetIconTexture(_iconID);
            if (iconTexture != null)
            {
                // Center the icon in the button
                Vector2 iconPos = new Vector2(
                    hitbox.X + hitbox.Width / 2f,
                    hitbox.Y + hitbox.Height / 2f
                );

                // Get the frame for the item (handles animated items)
                Rectangle? frame = null;
                if (_iconID > 0 && Main.itemAnimations != null && Main.itemAnimations[_iconID] != null)
                {
                    frame = Main.itemAnimations[_iconID].GetFrame(iconTexture);
                }
                else
                {
                    frame = new Rectangle(0, 0, iconTexture.Width, iconTexture.Height);
                }

                float scale = 1f;
                // Scale down if icon is too large
                if (frame.Value.Width > 40 || frame.Value.Height > 40)
                {
                    float scaleX = 40f / frame.Value.Width;
                    float scaleY = 40f / frame.Value.Height;
                    scale = System.Math.Min(scaleX, scaleY);
                }

                spriteBatch.Draw(
                    iconTexture,
                    iconPos,
                    frame,
                    Color.White,
                    0f,
                    new Vector2(frame.Value.Width, frame.Value.Height) / 2f,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
            else
            {
                // Draw a red X if texture failed to load (for debugging)
                Vector2 center = new Vector2(hitbox.X + hitbox.Width / 2f, hitbox.Y + hitbox.Height / 2f);
                Utils.DrawBorderString(spriteBatch, "?", center, Color.Red);
            }

            // Show tooltip on hover
            if (_isHovered)
            {
                Main.hoverItemName = _iconName;
            }
        }

        private Texture2D GetIconTexture(int iconID)
        {
            // Handle special icons
            switch (iconID)
            {
                case -5: // Yellow custom marker icon
                    try
                    {
                        return ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(ItemIconRegistry.CustomMarkerYellowPath).Value;
                    }
                    catch
                    {
                        return null;
                    }
                    ;
                case -4: // Green custom marker icon
                    try
                    {
                        return ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(ItemIconRegistry.CustomMarkerGreenPath).Value;
                    }
                    catch
                    {
                        return null;
                    }
                    ;
                case -3: // Blue custom marker icon
                    try
                    {
                        return ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(ItemIconRegistry.CustomMarkerBluePath).Value;
                    }
                    catch
                    {
                        return null;
                    }
                    ;
                case -2: // Red custom marker icon
                    try
                    {
                        return ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(ItemIconRegistry.CustomMarkerRedPath).Value;
                    }
                    catch
                    {
                        return null;
                    }
                    ;

                case -1: // House icon
                    try
                    {
                        return Main.Assets.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(ItemIconRegistry.HouseIconPath).Value;
                    }
                    catch
                    {
                        return null;
                    }
                    ;

            }

            // Regular item icons
            if (iconID > 0 && iconID < ItemID.Count)
            {
                try
                {
                    return TextureAssets.Item[iconID].Value;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }
}