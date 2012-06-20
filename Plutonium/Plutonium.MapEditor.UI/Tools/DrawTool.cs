using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plutonium.GameData;
using MonitoredUndo;

namespace Plutonium.MapEditor.UI.Tools
{
    public class DrawTool : Tool
    {
        public override void OnMouseClick(Map map, Layer layer, Camera2D camera, Vector2 position, string selectedTileName)
        {
            DrawTile(map, layer, camera, position, selectedTileName);
        }

        public override void OnMouseMove(Map map, Layer layer, Camera2D camera, Vector2 position, string selectedTileName)
        {
            DrawTile(map, layer, camera, position, selectedTileName);
        }

        void DrawTile(Map map, Layer layer, Camera2D camera, Vector2 position, string selectedTileName)
        {
            if (!string.IsNullOrEmpty(selectedTileName))
            {
                var point = new Vector2((int)((camera.Position.X + position.X) / 32), (int)((camera.Position.Y + position.Y) / 32));

                if (layer.Tiles.ContainsKey(point))
                    layer.Tiles.Remove(point);

                layer.Tiles.Add(point, TileRepository.GetTile(selectedTileName));
                
                /*UndoService.Current[map].AddChange(new DelegateChange(null, new Action(() =>
                {
                    layer.Tiles.Remove(point);
                }), new Action(() =>
                {
                    layer.Tiles.Add(point, TileRepository.GetTile(selectedTileName));
                }), selectedTileName), "Draw a tile");*/
        
                /*MainForm.MapPanel.AddShadowForObject(Texture2D.FromStream(MainForm.MapPanel.GraphicsDevice, TileRepository.GetTile(selectedTileName).Texture),
                    point);*/
      
                base.OnMouseMove(map, layer, camera, position, selectedTileName);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Camera2D camera, Vector2 mousePosition, Texture2D pixel)
        {
            spriteBatch.Draw(pixel, new Rectangle((int)((camera.Position.X + mousePosition.X) / 32) * 32, (int)((camera.Position.Y + mousePosition.Y) / 32) * 32,32,32), Color.Orange);

            base.Draw(spriteBatch, camera, mousePosition, pixel);
        }
    }
}
