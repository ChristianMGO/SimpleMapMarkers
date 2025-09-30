using Microsoft.Xna.Framework;

namespace SimpleMapMarkers
{
    public class Marker
    {
        public Vector2 Position { get; set; }
        public int IconID { get; set; }
        public string Name { get; set; }

        // Constructor
        public Marker(Vector2 position, int iconID, string name)
        {
            Position = position;
            IconID = iconID;
            Name = name ?? "Unnamed Marker"; // Default name if null
        }

        // Parameterless constructor for serialization
        public Marker()
        {
            Position = Vector2.Zero;
            IconID = 1; // Default to first item in registry
            Name = "Unnamed Marker";
        }

        // Helper property to get tile position
        public Vector2 TilePosition => Position / 16f;
    }
}