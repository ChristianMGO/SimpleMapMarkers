using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace SimpleMapMarkers
{
    public class UICheckbox : UIElement
    {
        private bool _isChecked;
        private string _label;

        public bool IsChecked => _isChecked;

        public UICheckbox(string label, bool initialState = false)
        {
            _label = label;
            _isChecked = initialState;
        }

        public void SetChecked(bool value)
        {
            _isChecked = value;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            _isChecked = !_isChecked;
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.MenuTick);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle hitbox = GetDimensions().ToRectangle();

            // Draw checkbox background
            Color bgColor = _isChecked ? new Color(100, 200, 100) : new Color(35, 40, 83);
            Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;
            
            // Checkbox box (24x24)
            Rectangle checkboxRect = new Rectangle(hitbox.X, hitbox.Y, 24, 24);
            spriteBatch.Draw(pixel, checkboxRect, bgColor);

            // Draw border
            int borderWidth = 2;
            Color borderColor = Color.Black;
            spriteBatch.Draw(pixel, new Rectangle(checkboxRect.X, checkboxRect.Y, checkboxRect.Width, borderWidth), borderColor);
            spriteBatch.Draw(pixel, new Rectangle(checkboxRect.X, checkboxRect.Y + checkboxRect.Height - borderWidth, checkboxRect.Width, borderWidth), borderColor);
            spriteBatch.Draw(pixel, new Rectangle(checkboxRect.X, checkboxRect.Y, borderWidth, checkboxRect.Height), borderColor);
            spriteBatch.Draw(pixel, new Rectangle(checkboxRect.X + checkboxRect.Width - borderWidth, checkboxRect.Y, borderWidth, checkboxRect.Height), borderColor);

            // Draw checkmark if checked
            if (_isChecked)
            {
                Vector2 checkCenter = new Vector2(checkboxRect.X + 12, checkboxRect.Y + 12);
                Utils.DrawBorderString(spriteBatch, "✓", checkCenter, Color.White, 1.2f, 0.5f, 0.5f);
            }

            // Draw label text
            Vector2 labelPos = new Vector2(hitbox.X + 32, hitbox.Y + 4);
            Utils.DrawBorderString(spriteBatch, _label, labelPos, Color.White, 0.9f);
        }
    }
}