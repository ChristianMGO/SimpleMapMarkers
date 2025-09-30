using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SimpleMapMarkers
{
    public class MarkerSystem : ModSystem
    {
        public static List<Marker> Markers = new List<Marker>();

        // Temporary storage for position when waiting for UI input
        public static Vector2? PendingMarkerPosition = null;

        public static void AddMarker(Vector2 worldPosition, int iconID, string name)
        {
            Markers.Add(new Marker(worldPosition, iconID, name));
        }

        // Overload for backwards compatibility or quick adding with defaults
        public static void AddMarker(Vector2 worldPosition)
        {
            Markers.Add(new Marker(worldPosition, 1, "Unnamed Marker"));
        }

        public static void RemoveLastMarker()
        {
            if (Markers.Count > 0)
                Markers.RemoveAt(Markers.Count - 1);
        }

        // New method: Remove marker at specific index
        public static void RemoveMarker(int index)
        {
            if (index >= 0 && index < Markers.Count)
                Markers.RemoveAt(index);
        }

        // New method: Find nearest marker to a position
        public static int FindNearestMarkerIndex(Vector2 worldPosition, float maxDistance = 100f)
        {
            int nearestIndex = -1;
            float nearestDistance = maxDistance;

            for (int i = 0; i < Markers.Count; i++)
            {
                float distance = Vector2.Distance(Markers[i].Position, worldPosition);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }

        // Save markers to world file
        public override void SaveWorldData(TagCompound tag)
        {
            List<TagCompound> markerList = new List<TagCompound>();
            foreach (var marker in Markers)
            {
                markerList.Add(new TagCompound
                {
                    ["X"] = marker.Position.X,
                    ["Y"] = marker.Position.Y,
                    ["IconID"] = marker.IconID,
                    ["Name"] = marker.Name
                });
            }
            tag["Markers"] = markerList;
        }

        // Load markers from world file
        public override void LoadWorldData(TagCompound tag)
        {
            Markers.Clear();
            if (tag.ContainsKey("Markers"))
            {
                var markerList = tag.GetList<TagCompound>("Markers");
                foreach (var markerTag in markerList)
                {
                    Markers.Add(new Marker(
                        new Vector2(markerTag.GetFloat("X"), markerTag.GetFloat("Y")),
                        markerTag.GetInt("IconID"),
                        markerTag.GetString("Name")
                    ));
                }
                ModContent.GetInstance<SimpleMapMarkers>().Logger.Info($"Loaded {Markers.Count} markers");
            }
        }

        // Clear pending marker position when unloading
        public override void Unload()
        {
            PendingMarkerPosition = null;
        }
    }
}