using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plutonium.GameData;

namespace Plutonium.MapEditor.UI.Tools
{
    public class EraserTool : Tool
    {
        public override void OnMouseClick(Map map, Layer layer, Camera2D camera, Vector2 position, string selectedTileName)
        {
            Erase(layer, camera, position, selectedTileName);
        }

        public override void OnMouseMove(Map map, Layer layer, Camera2D camera, Vector2 position, string selectedTileName)
        {
            Erase(layer, camera, position, selectedTileName);
            base.OnMouseMove(map, layer, camera, position, selectedTileName);
        }

        void Erase(Layer layer, Camera2D camera, Vector2 position, string selectedTileName)
        {
            if (!string.IsNullOrEmpty(selectedTileName))
            {
                var point = new Vector2((int)((camera.Position.X + position.X) / 32), (int)((camera.Position.Y + position.Y) / 32));

                if (layer.Tiles.ContainsKey(point))
                    layer.Tiles.Remove(point);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Camera2D camera, Vector2 mousePosition, Texture2D pixel)
        {
            spriteBatch.Draw(pixel, new Rectangle((int)((camera.Position.X + mousePosition.X) / 32) * 32 + 1, (int)((camera.Position.Y + mousePosition.Y) / 32) * 32, 32, 32),
                Color.Orange);

            base.Draw(spriteBatch, camera, mousePosition, pixel);
        }
    }
}
