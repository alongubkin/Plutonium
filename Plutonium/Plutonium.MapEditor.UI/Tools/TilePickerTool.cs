using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plutonium.GameData;

namespace Plutonium.MapEditor.UI.Tools
{
    class TilePickerTool : Tool 
    {
        MainForm _form;

        public TilePickerTool(MainForm form)
        {
            _form = form;
        }

        public override void OnMouseClick(Map map, Layer layer, Camera2D camera, Vector2 position, string selectedTileName)
        {
            PickTile(layer, camera, position);
        }

        void PickTile(Layer layer, Camera2D camera, Vector2 position)
        {
            var point = new Vector2((int)((camera.Position.X + position.X) / 32), (int)((camera.Position.Y + position.Y) / 32));

            if (layer.Tiles.ContainsKey(point))
            {
                _form.SelectTile(layer.Tiles[point].Name);
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
