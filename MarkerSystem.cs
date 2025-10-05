using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.IO;

namespace SimpleMapMarkers
{
    public class MarkerSystem : ModSystem
    {
        public static List<Marker> Markers = new List<Marker>();

        // Temporary storage for position when waiting for UI input
        public static Vector2? PendingMarkerPosition = null;

        public static void AddMarker(Vector2 worldPosition, int iconID, string name, bool isPublic = false)
        {
            string ownerName = Main.LocalPlayer.name;
            Marker newMarker = new Marker(worldPosition, iconID, name, isPublic, ownerName);
            Markers.Add(newMarker);

            // If multiplayer and public, sync to other clients
            if (Main.netMode == Terraria.ID.NetmodeID.MultiplayerClient && isPublic)
            {
                SendMarkerToServer(newMarker);
            }
        }

        // Overload for backwards compatibility
        public static void AddMarker(Vector2 worldPosition)
        {
            AddMarker(worldPosition, -2, "Unnamed Marker", false);
        }

        public static void RemoveLastMarker()
        {
            if (Markers.Count > 0)
                Markers.RemoveAt(Markers.Count - 1);
        }

        public static void RemoveMarker(int index)
        {
            if (index >= 0 && index < Markers.Count)
            {
                Marker marker = Markers[index];
                
                // Only allow removing your own markers or if you're the server
                if (Main.netMode == Terraria.ID.NetmodeID.SinglePlayer || 
                    marker.OwnerName == Main.LocalPlayer.name ||
                    Main.netMode == Terraria.ID.NetmodeID.Server)
                {
                    Main.NewText("Marker removed: " + marker.Name, Color.White);
                    
                    // If multiplayer and public, sync removal
                    if (Main.netMode == Terraria.ID.NetmodeID.MultiplayerClient && marker.IsPublic)
                    {
                        SendMarkerRemovalToServer(index);
                    }
                    
                    Markers.RemoveAt(index);
                }
                else
                {
                    Main.NewText("You can only remove your own markers!", Color.Red);
                }
            }
        }

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

        // Multiplayer: Send marker to server
        private static void SendMarkerToServer(Marker marker)
        {
            ModPacket packet = ModContent.GetInstance<SimpleMapMarkers>().GetPacket();
            packet.Write((byte)0); // Message type: Add Marker
            packet.Write(marker.Position.X);
            packet.Write(marker.Position.Y);
            packet.Write(marker.IconID);
            packet.Write(marker.Name);
            packet.Write(marker.OwnerName);
            packet.Send();
        }

        // Multiplayer: Send marker removal to server
        private static void SendMarkerRemovalToServer(int index)
        {
            ModPacket packet = ModContent.GetInstance<SimpleMapMarkers>().GetPacket();
            packet.Write((byte)1); // Message type: Remove Marker
            packet.Write(index);
            packet.Send();
        }

        // Save markers to world file (only save public markers in multiplayer)
        public override void SaveWorldData(TagCompound tag)
        {
            List<TagCompound> markerList = new List<TagCompound>();
            foreach (var marker in Markers)
            {
                // In multiplayer, only save public markers to world
                // Private markers are saved per-player
                if (Main.netMode != Terraria.ID.NetmodeID.Server || marker.IsPublic)
                {
                    markerList.Add(new TagCompound
                    {
                        ["X"] = marker.Position.X,
                        ["Y"] = marker.Position.Y,
                        ["IconID"] = marker.IconID,
                        ["Name"] = marker.Name,
                        ["IsPublic"] = marker.IsPublic,
                        ["OwnerName"] = marker.OwnerName
                    });
                }
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
                    bool isPublic = markerTag.ContainsKey("IsPublic") ? markerTag.GetBool("IsPublic") : false;
                    string ownerName = markerTag.ContainsKey("OwnerName") ? markerTag.GetString("OwnerName") : "";
                    
                    Markers.Add(new Marker(
                        new Vector2(markerTag.GetFloat("X"), markerTag.GetFloat("Y")),
                        markerTag.GetInt("IconID"),
                        markerTag.GetString("Name"),
                        isPublic,
                        ownerName
                    ));
                }
                ModContent.GetInstance<SimpleMapMarkers>().Logger.Info($"Loaded {Markers.Count} markers");
            }
        }

        public override void Unload()
        {
            PendingMarkerPosition = null;
        }
    }
}