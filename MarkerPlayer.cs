using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;

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
            
            if (SimpleMapMarkers.RemoveMarkerKeybind.JustPressed)
            {
                MarkerSystem.RemoveLastMarker();
            }
        }
    }
}