using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SimpleMapMarkers
{
    public class MarkerCreationUI : UIState
    {
        private UIPanel mainPanel;
        private UIText titleText;
        private SimpleTextInput nameInputField;
        private UIPanel iconGridPanel;
        private UITextPanel<string> confirmButton;
        private UITextPanel<string> cancelButton;

        private int selectedIconID = 1;

        public override void OnInitialize()
        {
            // Main panel
            mainPanel = new UIPanel();
            mainPanel.Width.Set(500, 0f);
            mainPanel.Height.Set(400, 0f);
            mainPanel.HAlign = 0.5f;
            mainPanel.VAlign = 0.5f;
            mainPanel.BackgroundColor = new Color(73, 94, 171) * 0.9f;
            Append(mainPanel);

            // Title
            titleText = new UIText("Create Map Marker", 1.2f);
            titleText.Top.Set(15, 0f);
            titleText.HAlign = 0.5f;
            mainPanel.Append(titleText);

            // Name input label
            UIText nameLabel = new UIText("Marker Name:", 0.9f);
            nameLabel.Top.Set(60, 0f);
            nameLabel.Left.Set(15, 0f);
            mainPanel.Append(nameLabel);

            // Name input field using our custom SimpleTextInput
            nameInputField = new SimpleTextInput("Click to type...");
            nameInputField.Width.Set(-30, 1f);
            nameInputField.Height.Set(40, 0f);
            nameInputField.Top.Set(85, 0f);
            nameInputField.Left.Set(15, 0f);
            mainPanel.Append(nameInputField);

            // Icon selection label
            UIText iconLabel = new UIText("Select Icon:", 0.9f);
            iconLabel.Top.Set(140, 0f);
            iconLabel.Left.Set(15, 0f);
            mainPanel.Append(iconLabel);

            // Icon grid panel
            iconGridPanel = new UIPanel();
            iconGridPanel.Width.Set(-30, 1f);
            iconGridPanel.Height.Set(150, 0f);
            iconGridPanel.Top.Set(165, 0f);
            iconGridPanel.Left.Set(15, 0f);
            iconGridPanel.BackgroundColor = new Color(35, 40, 83);
            mainPanel.Append(iconGridPanel);

            CreateIconGrid();

            // Confirm button
            confirmButton = new UITextPanel<string>("Confirm");
            confirmButton.Width.Set(150, 0f);
            confirmButton.Height.Set(40, 0f);
            confirmButton.Top.Set(-55, 1f);
            confirmButton.Left.Set(15, 0f);
            confirmButton.OnLeftClick += ConfirmButtonClick;
            confirmButton.OnMouseOver += (evt, element) => 
            {
                confirmButton.BackgroundColor = new Color(50, 80, 50);
                Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.MenuTick);
            };
            confirmButton.OnMouseOut += (evt, element) => 
            {
                confirmButton.BackgroundColor = new Color(63, 82, 151) * 0.7f;
            };
            mainPanel.Append(confirmButton);

            // Cancel button
            cancelButton = new UITextPanel<string>("Cancel");
            cancelButton.Width.Set(150, 0f);
            cancelButton.Height.Set(40, 0f);
            cancelButton.Top.Set(-55, 1f);
            cancelButton.Left.Set(-165, 1f);
            cancelButton.OnLeftClick += CancelButtonClick;
            cancelButton.OnMouseOver += (evt, element) => 
            {
                cancelButton.BackgroundColor = new Color(80, 50, 50);
                Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.MenuTick);
            };
            cancelButton.OnMouseOut += (evt, element) => 
            {
                cancelButton.BackgroundColor = new Color(63, 82, 151) * 0.7f;
            };
            mainPanel.Append(cancelButton);
        }

        private void CreateIconGrid()
        {
            float xOffset = 10;
            float yOffset = 10;
            int iconsPerRow = 8;
            int iconIndex = 0;

            foreach (var kvp in ItemIconRegistry.ItemNames)
            {
                int iconID = kvp.Key;
                string iconName = kvp.Value;

                // Create icon button
                UIPanel iconButton = new UIPanel();
                iconButton.Width.Set(48, 0f);
                iconButton.Height.Set(48, 0f);
                iconButton.Left.Set(xOffset, 0f);
                iconButton.Top.Set(yOffset, 0f);
                iconButton.BackgroundColor = selectedIconID == iconID ? Color.Gold : Color.DarkBlue;
                
                // Store iconID in the button for click handling
                iconButton.OnLeftClick += (evt, element) => SelectIcon(iconID);
                iconButton.OnMouseOver += (evt, element) => 
                {
                    iconButton.BackgroundColor = Color.Yellow;
                };
                iconButton.OnMouseOut += (evt, element) => 
                {
                    iconButton.BackgroundColor = selectedIconID == iconID ? Color.Gold : Color.DarkBlue;
                };

                iconGridPanel.Append(iconButton);

                // Move to next position
                xOffset += 58;
                iconIndex++;
                
                if (iconIndex % iconsPerRow == 0)
                {
                    xOffset = 10;
                    yOffset += 58;
                }
            }
        }

        private void SelectIcon(int iconID)
        {
            selectedIconID = iconID;
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.MenuTick);
            
            // Refresh icon grid colors
            iconGridPanel.RemoveAllChildren();
            CreateIconGrid();
        }

        private void ConfirmButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (MarkerSystem.PendingMarkerPosition.HasValue)
            {
                // Get the marker name from the text field
                string markerName = nameInputField.CurrentText;
                
                // Create the marker with selected name and icon
                MarkerSystem.AddMarker(
                    MarkerSystem.PendingMarkerPosition.Value,
                    selectedIconID,
                    string.IsNullOrWhiteSpace(markerName) ? "Unnamed Marker" : markerName
                );

                // Clear pending position
                MarkerSystem.PendingMarkerPosition = null;

                // Play sound
                Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.MenuOpen);
            }

            // Close UI
            ModContent.GetInstance<UISystem>().HideMarkerCreationUI();
        }

        private void CancelButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            // Clear pending position
            MarkerSystem.PendingMarkerPosition = null;

            // Play sound
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.MenuClose);

            // Close UI
            ModContent.GetInstance<UISystem>().HideMarkerCreationUI();
        }

        public void OnShow()
        {
            nameInputField.SetText("");
            selectedIconID = 1;
            
            // Refresh icon grid
            iconGridPanel.RemoveAllChildren();
            CreateIconGrid();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // ESC to close
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape) && 
                !Main.oldKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                CancelButtonClick(null, null);
            }
        }
    }
}