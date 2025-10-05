using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace SimpleMapMarkers
{
    public static class ItemIconRegistry
    {
        public const string CustomMarkerRedPath = "SimpleMapMarkers/Assets/Images/MapMarkers/marker_icon_red";
        public const string CustomMarkerGreenPath = "SimpleMapMarkers/Assets/Images/MapMarkers/marker_icon_green";
        public const string CustomMarkerBluePath = "SimpleMapMarkers/Assets/Images/MapMarkers/marker_icon_blue";
        public const string CustomMarkerYellowPath = "SimpleMapMarkers/Assets/Images/MapMarkers/marker_icon_yellow";
        // Special icon path for house (XNB files work - just remove .xnb extension)
        public const string HouseIconPath = "Images/UI/DisplaySlots_5";

        // Dictionary of item IDs and display names
        public static readonly Dictionary<int, string> ItemNames = new Dictionary<int, string>
        {
            // Special icons
            { -5, "Yellow Marker" },
            { -4, "Green Marker" },
            { -3, "Blue Marker" },
            { -2, "Red Marker" },
            { -1, "House" },

            // Common useful items for markers
            { ItemID.Chest, "Chest" },
            { ItemID.IronAnvil, "Anvil" },
            { ItemID.Bed, "Bed" },
            { ItemID.GoldPickaxe, "Pickaxe" },
            { ItemID.IronBar, "Iron" },
            { ItemID.CopperBar, "Copper" },
            { ItemID.GemLockDiamond, "Diamond" },
            { ItemID.GemLockAmethyst, "Amethyst" },
            { ItemID.GemLockTopaz, "Topaz" },
            { ItemID.GemLockAmber, "Amber" },
            { ItemID.GemLockRuby, "Ruby" },
            { ItemID.GemLockEmerald, "Emerald" },
            { ItemID.GemLockSapphire, "Sapphire" },
            { ItemID.LifeCrystal, "Life Crystal" },
            { ItemID.LifeFruit, "Life Fruit" },
            { ItemID.StrangePlant2, "Strange Plant"},
        };
    }
}