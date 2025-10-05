using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.GameContent;
using Terraria;
using System.IO;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel;
using System.Linq;

namespace SimpleMapMarkers
{
    public class SimpleMapMarkers : Mod
    {
        public static ModKeybind AddMarkerKeybind;
        public static ModKeybind AddMarkerRightClickKeybind;
        public static ModKeybind RemoveMarkerKeybind;

        public override void Load()
        {
            AddMarkerKeybind = KeybindLoader.RegisterKeybind(this, "Add Marker at Player Position", "P");
            AddMarkerRightClickKeybind = KeybindLoader.RegisterKeybind(this, "Add Map Marker in Fullscreen map", "Mouse2");
            RemoveMarkerKeybind = KeybindLoader.RegisterKeybind(this, "Remove Hovered Marker", "Mouse3");

            // Preload all icon textures
            if (!Main.dedServ)
            {
                PreloadIconTextures();
            }
        }

        // Handle network packets for multiplayer marker sync
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte messageType = reader.ReadByte();

            switch (messageType)
            {
                case 0: // Add Marker
                    int id = reader.ReadInt32();
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    int iconID = reader.ReadInt32();
                    string name = reader.ReadString();
                    string ownerName = reader.ReadString();

                    Marker newMarker = new Marker
                    {
                        ID = id,
                        Position = new Vector2(x, y),
                        IconID = iconID,
                        Name = name,
                        IsPublic = true,
                        OwnerName = ownerName
                    };

                    if (Main.netMode == Terraria.ID.NetmodeID.Server)
                    {
                        MarkerSystem.Markers.Add(newMarker);

                        ModPacket packet = GetPacket();
                        packet.Write((byte)0);
                        packet.Write(id);
                        packet.Write(x);
                        packet.Write(y);
                        packet.Write(iconID);
                        packet.Write(name);
                        packet.Write(ownerName);
                        packet.Send(-1, whoAmI);
                    }
                    else
                    {
                        MarkerSystem.Markers.Add(newMarker);
                        Main.NewText($"Public marker added by {ownerName}: {name}", Color.Yellow);
                    }
                    break;

                case 1: // Remove Marker
                    int markerID = reader.ReadInt32(); // Read ID instead of index

                    if (Main.netMode == Terraria.ID.NetmodeID.Server)
                    {
                        int index = MarkerSystem.Markers.FindIndex(m => m.ID == markerID);
                        if (index >= 0)
                        {
                            MarkerSystem.Markers.RemoveAt(index);

                            ModPacket packet = GetPacket();
                            packet.Write((byte)1);
                            packet.Write(markerID); // Send ID
                            packet.Send(-1, whoAmI);
                        }
                    }
                    else
                    {
                        int index = MarkerSystem.Markers.FindIndex(m => m.ID == markerID);
                        if (index >= 0)
                        {
                            MarkerSystem.Markers.RemoveAt(index);
                        }
                    }
                    break;

                case 2: // Request full sync (sent by joining client)
                    if (Main.netMode == Terraria.ID.NetmodeID.Server)
                    {
                        Logger.Info($"Client {whoAmI} requested marker sync");
                        SyncAllMarkersToClient(whoAmI);
                    }
                    break;
                case 3: // Full sync response (sent by server to client)
                    int markerCount = reader.ReadInt32();
                    Logger.Info($"Receiving {markerCount} markers from server");

                    // Clear existing markers
                    MarkerSystem.Markers.Clear();

                    // Read all markers
                    for (int i = 0; i < markerCount; i++)
                    {
                        Marker marker = new Marker
                        {
                            ID = reader.ReadInt32(),
                            Position = new Vector2(reader.ReadSingle(), reader.ReadSingle()),
                            IconID = reader.ReadInt32(),
                            Name = reader.ReadString(),
                            IsPublic = reader.ReadBoolean(),
                            OwnerName = reader.ReadString()
                        };
                        MarkerSystem.Markers.Add(marker);
                    }

                    Main.NewText($"Synced {markerCount} markers from server", Color.Green);
                    break;
            }
        }

        private void SyncAllMarkersToClient(int clientWhoAmI)
        {
            // Only send public markers
            var publicMarkers = MarkerSystem.Markers.Where(m => m.IsPublic).ToList();

            ModPacket packet = GetPacket();
            packet.Write((byte)3); // Full sync message
            packet.Write(publicMarkers.Count);

            foreach (var marker in publicMarkers)
            {
                packet.Write(marker.ID);
                packet.Write(marker.Position.X);
                packet.Write(marker.Position.Y);
                packet.Write(marker.IconID);
                packet.Write(marker.Name);
                packet.Write(marker.IsPublic);
                packet.Write(marker.OwnerName);
            }

            packet.Send(clientWhoAmI);
            Logger.Info($"Sent {publicMarkers.Count} markers to client {clientWhoAmI}");
        }
        private void PreloadIconTextures()
        {
            // Force load all item textures used in ItemIconRegistry
            foreach (var iconID in ItemIconRegistry.ItemNames.Keys)
            {
                if (iconID > 0)
                {
                    // Force the texture to load
                    Main.instance.LoadItem(iconID);
                }
                else if (iconID == -1)
                {
                    // Preload house icon
                    try
                    {
                        Main.Assets.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(ItemIconRegistry.HouseIconPath);
                        Logger.Info($"Successfully preloaded house icon from {ItemIconRegistry.HouseIconPath}");
                    }
                    catch
                    {
                        Logger.Warn($"Failed to preload house icon at {ItemIconRegistry.HouseIconPath}");
                    }
                }
                else if (iconID == -2)
                {
                    try
                    {
                        var texture = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(ItemIconRegistry.CustomMarkerRedPath);
                        Logger.Info($"Successfully preloaded Red marker from {ItemIconRegistry.CustomMarkerRedPath}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"Failed to preload Red marker at {ItemIconRegistry.CustomMarkerRedPath}: {ex.Message}");
                    }
                }
                else if (iconID == -3)
                {
                    try
                    {
                        var texture = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(ItemIconRegistry.CustomMarkerBluePath);
                        Logger.Info($"Successfully preloaded Blue marker from {ItemIconRegistry.CustomMarkerBluePath}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"Failed to preload Blue marker at {ItemIconRegistry.CustomMarkerBluePath}: {ex.Message}");
                    }
                }
                else if (iconID == -4)
                {
                    try
                    {
                        var texture = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(ItemIconRegistry.CustomMarkerGreenPath);
                        Logger.Info($"Successfully preloaded Green marker from {ItemIconRegistry.CustomMarkerGreenPath}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"Failed to preload Green marker at {ItemIconRegistry.CustomMarkerGreenPath}: {ex.Message}");
                    }
                }
                else if (iconID == -5)
                {
                    try
                    {
                        var texture = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(ItemIconRegistry.CustomMarkerYellowPath);
                        Logger.Info($"Successfully preloaded Yellow marker from {ItemIconRegistry.CustomMarkerYellowPath}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"Failed to preload Yellow marker at {ItemIconRegistry.CustomMarkerYellowPath}: {ex.Message}");
                    }
                }
            }

            Logger.Info($"Preloaded {ItemIconRegistry.ItemNames.Count} icon textures");
        }

        public override void Unload()
        {
            AddMarkerKeybind = null;
            AddMarkerRightClickKeybind = null;
            RemoveMarkerKeybind = null;
        }
    }
}