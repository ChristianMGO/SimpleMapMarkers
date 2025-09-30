using Terraria.ModLoader;

namespace SimpleMapMarkers
{
    public class SimpleMapMarkers : Mod
    {
        public static ModKeybind AddMarkerKeybind;
        public static ModKeybind AddMarkerRightClickKeybind;
        public static ModKeybind RemoveMarkerKeybind;

        public override void Load()
        {
            AddMarkerKeybind = KeybindLoader.RegisterKeybind(this, "Add Map Marker", "P");
            AddMarkerRightClickKeybind = KeybindLoader.RegisterKeybind(this, "Add Map Marker in Fullscreen map", "Mouse2");
            RemoveMarkerKeybind = KeybindLoader.RegisterKeybind(this, "Remove Last Marker", "O");
        }

        public override void Unload()
        {
            AddMarkerKeybind = null;
            AddMarkerRightClickKeybind = null;
            RemoveMarkerKeybind = null;
        }
    }
}