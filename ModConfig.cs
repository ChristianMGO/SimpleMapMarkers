using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SimpleMapMarkers.Configs
{
    public class SimpleMapMarkersConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.SimpleMapMarkers.Configs.SimpleMapMarkersConfig.GeneralSettings.Header")]
        [LabelKey("$Mods.SimpleMapMarkers.Configs.SimpleMapMarkersConfig.Background.Label")]
        [TooltipKey("$Mods.SimpleMapMarkers.Configs.SimpleMapMarkersConfig.Background.Tooltip")]
        [DefaultValue(true)]
        public bool TooltipBackground;
    }
}
