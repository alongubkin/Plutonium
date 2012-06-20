using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plutonium.GameData;
using Krypton.Lights;
using Krypton;

namespace Plutonium.MapEditor.UI.Tools
{
    public class LightTool : Tool
    {
        public override void OnMouseClick(Map map, Layer layer, Camera2D camera, Vector2 position, string selectedTileName)
        {
            bool add = true;

            foreach (Light2D light in MainForm.MapPanel.Krpyton.Lights)
            {
                var mouse = (new Vector2(camera.Position.X, camera.Position.Y) + position).ToPoint();
               
                for (int i = 0; i < MainForm.MapPanel._lightSymbolPixelData.Length; i++)
                {
                    int x = i % MainForm.MapPanel._lightSymbolTexture.Width;
                    int y = i / MainForm.MapPanel._lightSymbolTexture.Width;
                 //   System.Windows.Forms.MessageBox.Show(mouse.ToString() + " , " + x + (int)light.Position.X + " " + y + (int)light.Position.Y);
                    if (new Rectangle(x + (int)light.Position.X, y + (int)light.Position.Y, 16, 16).Contains(mouse))
                    {
                        add = false;
                        ((MainForm)MainForm.ActiveForm).OpenProperties(light);
                        MainForm.MapPanel.SelectedLight = light;
                    }
                }
            }

            if (add)
            {
                var light = new Light2D()
                {
                    Position = new Vector2(camera.Position.X + position.X, camera.Position.Y + position.Y),
                    Color = Color.White,
                    Texture = MapPanel.LightTexture,
                    Range = 100,
                    IsOn = true,
                    Intensity = 1.5f,
                    ShadowType = ShadowType.Occluded
                };

                MainForm.MapPanel.Krpyton.Lights.Add(light);
                ((MainForm)MainForm.ActiveForm).OpenProperties(light);
                MainForm.MapPanel.SelectedLight = light;
            }
        }

        public override void OnMouseMove(Map map, Layer layer, Camera2D camera, Vector2 position, string selectedTileName)
        {
            foreach (Light2D light in MainForm.MapPanel.Krpyton.Lights)
            {
                var mouse = (new Vector2(camera.Position.X, camera.Position.Y) + position).ToPoint();

                for (int i = 0; i < MainForm.MapPanel._lightSymbolPixelData.Length; i++)
                {
                    int x = i % MainForm.MapPanel._lightSymbolTexture.Width;
                    int y = i / MainForm.MapPanel._lightSymbolTexture.Width;

                    if (MainForm.MapPanel._movingLight == light || new Rectangle(x + (int)light.Position.X, y + (int)light.Position.Y, 1, 1)
                            .Contains(mouse))
                    {
                        if (MainForm.MapPanel._movingLight == null)
                            MainForm.MapPanel._movingLight = light;

                        light.Position = (new Vector2(camera.Position.X, camera.Position.Y) + position);
                    }
                }
            }

            base.OnMouseMove(map, layer, camera, position, selectedTileName);
        }
    }
}