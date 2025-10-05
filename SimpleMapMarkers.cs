using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria;
using System.IO;
using Microsoft.Xna.Framework;

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
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    int iconID = reader.ReadInt32();
                    string name = reader.ReadString();
                    string ownerName = reader.ReadString();

                    if (Main.netMode == Terraria.ID.NetmodeID.Server)
                    {
                        // Server: Add marker and broadcast to all clients
                        Marker newMarker = new Marker(new Vector2(x, y), iconID, name, true, ownerName);
                        MarkerSystem.Markers.Add(newMarker);

                        // Broadcast to all other clients
                        ModPacket packet = GetPacket();
                        packet.Write((byte)0);
                        packet.Write(x);
                        packet.Write(y);
                        packet.Write(iconID);
                        packet.Write(name);
                        packet.Write(ownerName);
                        packet.Send(-1, whoAmI); // Send to everyone except sender
                    }
                    else
                    {
                        // Client: Receive marker from server
                        Marker newMarker = new Marker(new Vector2(x, y), iconID, name, true, ownerName);
                        MarkerSystem.Markers.Add(newMarker);
                        Main.NewText($"Public marker added by {ownerName}: {name}", Color.Yellow);
                    }
                    break;

                case 1: // Remove Marker
                    int index = reader.ReadInt32();

                    if (Main.netMode == Terraria.ID.NetmodeID.Server)
                    {
                        // Server: Remove marker and broadcast
                        if (index >= 0 && index < MarkerSystem.Markers.Count)
                        {
                            MarkerSystem.Markers.RemoveAt(index);

                            // Broadcast removal to all clients
                            ModPacket packet = GetPacket();
                            packet.Write((byte)1);
                            packet.Write(index);
                            packet.Send(-1, whoAmI);
                        }
                    }
                    else
                    {
                        // Client: Receive removal from server
                        if (index >= 0 && index < MarkerSystem.Markers.Count)
                        {
                            MarkerSystem.Markers.RemoveAt(index);
                        }
                    }
                    break;
            }
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
                    }
                    catch
                    {
                        Logger.Warn($"Failed to preload house icon at {ItemIconRegistry.HouseIconPath}");
                    }
                }
                else if (iconID == -2)
                {
                    // Preload custom marker icon
                    try
                    {
                        ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(ItemIconRegistry.CustomMarkerRedPath);
                    }
                    catch
                    {
                        Logger.Warn($"Failed to preload custom marker icon at {ItemIconRegistry.CustomMarkerRedPath}");
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