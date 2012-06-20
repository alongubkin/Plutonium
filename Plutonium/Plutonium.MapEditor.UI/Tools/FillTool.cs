using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plutonium.GameData;

namespace Plutonium.MapEditor.UI.Tools
{
    public class FillTool : Tool
    {
        public override void OnMouseClick(Map map, Layer layer, Camera2D camera, Vector2 position, string selectedTileName)
        {
            if (!string.IsNullOrEmpty(selectedTileName))
            {
                var orginal = new Vector2((int)((camera.Position.X + position.X) / 32), (int)((camera.Position.Y + position.Y) / 32));

                Tile target = null;
                layer.Tiles.TryGetValue(orginal, out target);


                FloodFill(layer, orginal, orginal, target, TileRepository.GetTile(selectedTileName));
            }
        }

        void FloodFill(Layer layer, Vector2 orginal, Vector2 node, Tile target, Tile replacement)
        {
            if (target == replacement)
                return;

            bool nodeExists = layer.Tiles.ContainsKey(node);

            if ((!nodeExists && target != null) || (nodeExists && target == null) || (nodeExists && layer.Tiles[node] != target))
                return;

            if (nodeExists)
                layer.Tiles[node] = replacement;
            else
                layer.Tiles.Add(node, replacement);

            if (node.X < orginal.X + 32 && node.X > orginal.X - 32)
            {
                FloodFill(layer, orginal, new Vector2(node.X - 1, node.Y), target, replacement);
                FloodFill(layer, orginal, new Vector2(node.X + 1, node.Y), target, replacement);
            }

            if (node.Y < orginal.Y + 32 && node.Y > orginal.Y - 32)
            {
                FloodFill(layer, orginal, new Vector2(node.X, node.Y - 1), target, replacement);
                FloodFill(layer, orginal, new Vector2(node.X, node.Y + 1), target, replacement);
            }
        }





    }
}