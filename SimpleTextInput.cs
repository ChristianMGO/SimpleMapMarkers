using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
        private static Asset<Texture2D> _backgroundTexture;
        private static Asset<Texture2D> _borderTexture;

        public string CurrentText => _currentText;

        public SimpleTextInput(string hintText = "")
        {
            _hintText = hintText;
            if (_backgroundTexture == null)
                _backgroundTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/PanelGrayscale");
            if (_borderTexture == null)
                _borderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder");
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
            
            _isFocused = true;
            
            // CRITICAL: Initialize text input system properly
            Main.chatRelease = false;
            Main.editSign = false;
            Main.editChest = false;
            Main.blockInput = true;
            Main.inputTextEnter = false;
            Main.inputTextEscape = false;
            
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.MenuTick);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Handle text input when focused
            if (_isFocused)
            {
                Main.blockInput = true;
                PlayerInput.WritingText = true;
                Main.instance.HandleIME();

                string newText = Main.GetInputText(_currentText);
                if (newText != _currentText)
                {
                    _currentText = newText;
                    if (_currentText.Length > _maxLength)
                        _currentText = _currentText.Substring(0, _maxLength);
                }

                // Press Enter to confirm and unfocus
                if (Main.inputTextEnter || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    _isFocused = false;
                    Main.blockInput = false;
                    PlayerInput.WritingText = false;
                    Main.inputTextEnter = false;
                }
                
                // Press Escape to cancel and unfocus
                if (Main.inputTextEscape || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                {
                    _isFocused = false;
                    Main.blockInput = false;
                    PlayerInput.WritingText = false;
                    Main.inputTextEscape = false;
                }

                // Click outside to unfocus
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Rectangle hitbox = GetDimensions().ToRectangle();
                    if (!hitbox.Contains(Main.MouseScreen.ToPoint()))
                    {
                        _isFocused = false;
                        Main.blockInput = false;
                        PlayerInput.WritingText = false;
                    }
                }
            }
            else
            {
                Main.blockInput = false;
                PlayerInput.WritingText = false;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle hitbox = GetDimensions().ToRectangle();

            // Draw simple rectangle background
            Color bgColor = _isFocused ? new Color(50, 60, 120) : new Color(35, 40, 83);
            
            // Draw background as filled rectangle
            Texture2D pixel = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/PanelGrayscale").Value;
            spriteBatch.Draw(pixel, hitbox, bgColor);

            // Draw border
            int borderWidth = 2;
            // Top
            spriteBatch.Draw(pixel, new Rectangle(hitbox.X, hitbox.Y, hitbox.Width, borderWidth), Color.Black);
            // Bottom
            spriteBatch.Draw(pixel, new Rectangle(hitbox.X, hitbox.Y + hitbox.Height - borderWidth, hitbox.Width, borderWidth), Color.Black);
            // Left
            spriteBatch.Draw(pixel, new Rectangle(hitbox.X, hitbox.Y, borderWidth, hitbox.Height), Color.Black);
            // Right
            spriteBatch.Draw(pixel, new Rectangle(hitbox.X + hitbox.Width - borderWidth, hitbox.Y, borderWidth, hitbox.Height), Color.Black);

            // Draw text
            string displayText = _currentText;
            if (string.IsNullOrEmpty(displayText) && !_isFocused)
            {
                displayText = _hintText;
            }

            if (_isFocused && Main.GameUpdateCount % 40 < 20)
                displayText += "|";

            Vector2 textPos = new Vector2(hitbox.X + 10, hitbox.Y + 12);
            Color textColor = string.IsNullOrEmpty(_currentText) && !_isFocused ? Color.Gray : Color.White;
            
            Utils.DrawBorderString(spriteBatch, displayText, textPos, textColor, 0.9f);
        }
    }
}