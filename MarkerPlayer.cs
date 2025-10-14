using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using Microsoft.Xna.Framework;

namespace SimpleMapMarkers
{
    public class MarkerPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (SimpleMapMarkers.AddMarkerKeybind.JustPressed)
            {
                // Store player's current position
                MarkerSystem.PendingMarkerPosition = Player.Center;

                // Open marker creation UI
                ModContent.GetInstance<UISystem>().ShowMarkerCreationUI();
            }

            // Toggle Admin Mode
            if (SimpleMapMarkers.ToggleAdminModeKeybind.JustPressed)
            {
                MarkerSystem.IsAdminMode = !MarkerSystem.IsAdminMode;

                if (MarkerSystem.IsAdminMode)
                {
                    Main.NewText("Admin Mode: ON - You can delete any public marker", Color.Red);
                    Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Roar);
                }
                else
                {
                    Main.NewText("Admin Mode: OFF", Color.Gray);
                    Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.MenuClose);
                }
            }
        }
    }
}