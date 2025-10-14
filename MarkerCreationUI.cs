using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
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
        private UICheckbox publicCheckbox;

        private int selectedIconID = -2; // Default to custom marker icon

        public override void OnInitialize()
        {
            ModContent.GetInstance<SimpleMapMarkers>().Logger.Info("=== OnInitialize STARTED ===");
            ModContent.GetInstance<SimpleMapMarkers>().Logger.Info($"NetMode: {Main.netMode}");
            // Main panel
            mainPanel = new UIPanel();
            mainPanel.Width.Set(500, 0f);
            mainPanel.Height.Set(470, 0f);
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

            // Icon grid panel - taller to fit all icons
            iconGridPanel = new UIPanel();
            iconGridPanel.Width.Set(-30, 1f);
            iconGridPanel.Height.Set(200, 0f); // Increased from 150 to 180
            iconGridPanel.Top.Set(165, 0f);
            iconGridPanel.Left.Set(15, 0f);
            iconGridPanel.BackgroundColor = new Color(35, 40, 83);
            mainPanel.Append(iconGridPanel);

            CreateIconGrid();


            // Public checkbox (show when multiplayer is active - either as client or when hosting)
            if (Main.maxPlayers > 1 || Main.netMode != Terraria.ID.NetmodeID.SinglePlayer)
            {
                ModContent.GetInstance<SimpleMapMarkers>().Logger.Info($"Creating checkbox. MaxPlayers: {Main.maxPlayers}, NetMode: {Main.netMode}");

                publicCheckbox = new UICheckbox("Public Marker", false);
                publicCheckbox.Width.Set(150, 0f);
                publicCheckbox.Height.Set(24, 0f);
                publicCheckbox.Top.Set(371, 0f);
                publicCheckbox.Left.Set(15, 0f);
                mainPanel.Append(publicCheckbox);

                ModContent.GetInstance<SimpleMapMarkers>().Logger.Info("Checkbox created and appended");
            }
            else
            {
                ModContent.GetInstance<SimpleMapMarkers>().Logger.Info($"Singleplayer mode. MaxPlayers: {Main.maxPlayers}, NetMode: {Main.netMode}");
            }

            // Confirm button
            confirmButton = new UITextPanel<string>("Confirm");
            confirmButton.Width.Set(150, 0f);
            confirmButton.Height.Set(40, 0f);
            confirmButton.Top.Set(-43, 1f);
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
            cancelButton.Top.Set(-43, 1f);
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

            ModContent.GetInstance<SimpleMapMarkers>().Logger.Info("=== OnInitialize FINISHED ===");
        }

        private void CreateIconGrid()
        {
            iconGridPanel.RemoveAllChildren();

            ModContent.GetInstance<SimpleMapMarkers>().Logger.Info($"Creating icon grid with {ItemIconRegistry.ItemNames.Count} icons");

            float xOffset = 5;
            float yOffset = 5;
            int iconsPerRow = 7;
            int iconIndex = 0;

            foreach (var kvp in ItemIconRegistry.ItemNames)
            {
                int iconID = kvp.Key;
                string iconName = kvp.Value;

                // Create icon button
                IconButton iconButton = new IconButton(iconID, iconName, selectedIconID == iconID);
                iconButton.Width.Set(48, 0f);
                iconButton.Height.Set(48, 0f);
                iconButton.Left.Set(xOffset, 0f);
                iconButton.Top.Set(yOffset, 0f);

                // Store iconID in the button for click handling
                iconButton.OnLeftClick += (evt, element) => SelectIcon(iconID);

                iconGridPanel.Append(iconButton);

                ModContent.GetInstance<SimpleMapMarkers>().Logger.Info($"Added icon {iconName} (ID: {iconID}) at position ({xOffset}, {yOffset})");

                // Move to next position
                xOffset += 58;
                iconIndex++;

                // Wrap to next row after iconsPerRow icons
                if (iconIndex % iconsPerRow == 0)
                {
                    xOffset = 5;
                    yOffset += 58;
                }
            }

            iconGridPanel.Recalculate();
            ModContent.GetInstance<SimpleMapMarkers>().Logger.Info($"Icon grid created with {iconIndex} icons total");
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
            // CRITICAL: Reset input state before doing anything
            nameInputField.Unfocus();

            if (MarkerSystem.PendingMarkerPosition.HasValue)
            {
                // Get the marker name from the text field
                string markerName = nameInputField.CurrentText;

                // Get isPublic state from checkbox (false if in singleplayer where checkbox doesn't exist)
                bool isPublic = publicCheckbox != null && publicCheckbox.IsChecked;
                if (selectedIconID == 1)
                {
                    // Ensure the selected icon is not the "None" icon
                    selectedIconID = -2; // Default to custom red marker if "None" was selected
                }
                // Create the marker with selected name, icon, and public/private setting
                MarkerSystem.AddMarker(
                    MarkerSystem.PendingMarkerPosition.Value,
                    selectedIconID,
                    string.IsNullOrWhiteSpace(markerName) ? "Unnamed Marker" : markerName,
                    isPublic
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
            // CRITICAL: Reset input state before doing anything
            nameInputField.Unfocus();

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
                nameInputField.Unfocus();
                CancelButtonClick(null, null);
            }
        }
    }
}