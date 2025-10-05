using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;

namespace SimpleMapMarkers
{
    public class SimpleTextInput : UIElement
    {
        private string _currentText = "";
        private int _maxLength = 30;
        private bool _isFocused = false;
        private string _hintText = "";

        public string CurrentText => _currentText;

        public SimpleTextInput(string hintText = "")
        {
            _hintText = hintText;
        }

        public void SetText(string text)
        {
            _currentText = text ?? "";
            if (_currentText.Length > _maxLength)
                _currentText = _currentText.Substring(0, _maxLength);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            Focus();
        }

        private void Focus()
        {
            _isFocused = true;
            Main.clrInput();
            Main.blockInput = true;
            Main.drawingPlayerChat = true;
            Main.chatRelease = false;
        }

        public void Unfocus()
        {
            _isFocused = false;
            Main.blockInput = false;
            Main.drawingPlayerChat = false;
            Main.chatRelease = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_isFocused)
            {
                PlayerInput.WritingText = true;
                Main.instance.HandleIME();

                // Get input text
                string newString = Main.GetInputText(_currentText);
                if (!newString.Equals(_currentText))
                {
                    _currentText = newString;
                    if (_currentText.Length > _maxLength)
                    {
                        _currentText = _currentText.Substring(0, _maxLength);
                    }
                }

                // Unfocus on click outside
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Rectangle hitbox = GetDimensions().ToRectangle();
                    if (!hitbox.Contains(Main.MouseScreen.ToPoint()))
                    {
                        Unfocus();
                    }
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle hitbox = GetDimensions().ToRectangle();

            // Draw simple white/light background using TextureAssets
            Color bgColor = _isFocused ? Color.White : new Color(240, 240, 240);

            // Use a 1x1 white texture from game assets
            Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, hitbox, bgColor);

            // Draw border
            int borderWidth = 2;
            Color borderColor = _isFocused ? Color.Black : Color.Gray;

            // Top border
            spriteBatch.Draw(pixel, new Rectangle(hitbox.X, hitbox.Y, hitbox.Width, borderWidth), borderColor);
            // Bottom border
            spriteBatch.Draw(pixel, new Rectangle(hitbox.X, hitbox.Y + hitbox.Height - borderWidth, hitbox.Width, borderWidth), borderColor);
            // Left border
            spriteBatch.Draw(pixel, new Rectangle(hitbox.X, hitbox.Y, borderWidth, hitbox.Height), borderColor);
            // Right border
            spriteBatch.Draw(pixel, new Rectangle(hitbox.X + hitbox.Width - borderWidth, hitbox.Y, borderWidth, hitbox.Height), borderColor);

            // Draw text
            string displayText = _currentText;
            Color textColor = Color.Black;

            if (string.IsNullOrEmpty(displayText))
            {
                if (_isFocused)
                {
                    // Show just blinking cursor when focused and empty
                    displayText = Main.GameUpdateCount % 40 < 20 ? "|" : "";
                    textColor = Color.Black;
                }
                else
                {
                    displayText = _hintText;
                    textColor = Color.Gray;
                }
            }
            else
            {
                // Show cursor after text when focused and has content
                if (_isFocused && Main.GameUpdateCount % 40 < 20)
                    displayText += "|";
                textColor = Color.Black;
            }

            Vector2 textPos = new Vector2(hitbox.X + 10, hitbox.Y + 12);

            Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, displayText, textPos.X, textPos.Y, textColor, Color.Transparent, Vector2.Zero, 0.9f);
        }
    }
}