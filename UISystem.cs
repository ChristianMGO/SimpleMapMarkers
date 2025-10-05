using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SimpleMapMarkers
{
    public class UISystem : ModSystem
    {
        internal MarkerCreationUI MarkerCreationUI;
        private UserInterface _markerCreationInterface;

        public override void Load()
        {
            // Only create UI on client
            if (!Main.dedServ)
            {
                ModContent.GetInstance<SimpleMapMarkers>().Logger.Info("Creating MarkerCreationUI...");
                MarkerCreationUI = new MarkerCreationUI();
                ModContent.GetInstance<SimpleMapMarkers>().Logger.Info("Activating MarkerCreationUI...");
                MarkerCreationUI.Activate();
                ModContent.GetInstance<SimpleMapMarkers>().Logger.Info("Creating UserInterface...");
                _markerCreationInterface = new UserInterface();
                ModContent.GetInstance<SimpleMapMarkers>().Logger.Info("UISystem.Load() complete");
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // Block player input when marker creation UI is open
            if (_markerCreationInterface?.CurrentState != null)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (_markerCreationInterface?.CurrentState != null)
            {
                _markerCreationInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "SimpleMapMarkers: Marker Creation UI",
                    delegate
                    {
                        if (_markerCreationInterface?.CurrentState != null)
                        {
                            _markerCreationInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public void ShowMarkerCreationUI()
        {
            _markerCreationInterface?.SetState(MarkerCreationUI);
            MarkerCreationUI.OnShow();
        }

        public void HideMarkerCreationUI()
        {
            _markerCreationInterface?.SetState(null);
        }

        public override void Unload()
        {
            MarkerCreationUI = null;
            _markerCreationInterface = null;
        }
    }
}