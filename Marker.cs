using Microsoft.Xna.Framework;

namespace SimpleMapMarkers
{
    public class Marker
    {
        public Vector2 Position { get; set; }
        public int IconID { get; set; }
        public string Name { get; set; }
        public bool IsPublic { get; set; } // New: For multiplayer
        public string OwnerName { get; set; } // New: Track who created it

        // Constructor
        public Marker(Vector2 position, int iconID, string name, bool isPublic = false, string ownerName = "")
        {
            Position = position;
            IconID = iconID;
            Name = name ?? "Unnamed Marker";
            IsPublic = isPublic;
            OwnerName = ownerName;
        }

        // Parameterless constructor for serialization
        public Marker()
        {
            Position = Vector2.Zero;
            IconID = -2; // Default to custom marker
            Name = "Unnamed Marker";
            IsPublic = false;
            OwnerName = "";
        }

        // Helper property to get tile position
        public Vector2 TilePosition => Position / 16f;
    }
}