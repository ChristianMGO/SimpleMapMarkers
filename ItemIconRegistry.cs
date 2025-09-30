using System.Collections.Generic;

namespace SimpleMapMarkers
{
    public static class ItemIconRegistry
    {
        // Special asset paths for non-item icons
        public const string ChestIconPath = "Terraria/Images/UI/Map/Map_Icon_Chest";
        public const string HouseIconPath = "Terraria/Images/UI/House_HousingQuery";

        // Dictionary of item IDs and display names
        public static readonly Dictionary<int, string> ItemNames = new Dictionary<int, string>
        {
            { 1, "Iron Pickaxe" },
            { 2, "Iron Broadsword" },
            { 3, "Iron Shortsword" },
            { 4, "Iron Hammer" },
            { 5, "Iron Axe" },
            { 6, "Iron Ore" },
            { 7, "Copper Ore" },
            { 8, "Gold Ore" },
            { 9, "Silver Ore" },
            { 10, "Copper Watch" },
            { -1, "Chest" }, // Special key for chest map icon
            { -2, "House" }, // Special key for house UI icon
            // Add more items/icons as needed
        };
    }
}