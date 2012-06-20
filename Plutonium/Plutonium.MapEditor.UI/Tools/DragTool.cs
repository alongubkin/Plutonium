using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plutonium.GameData;

namespace Plutonium.MapEditor.UI.Tools
{
    public class DragTool : Tool
    {
        float _lastX = -1;
        float _lastY = -1;

        public override void OnMouseMove(Map map, GameData.Layer layer, Camera2D camera, Microsoft.Xna.Framework.Vector2 position, string selectedTileName)
        {
            if (_lastX != -1 && _lastY != -1)
            {
                var direction = Vector3.Zero;

                direction.X = position.X - _lastX;
                direction.Y = position.Y - _lastY;

                var delta = camera.Position - direction;

                if (delta.X > 0 && delta.Y > 0)
                    camera.Position = delta;
            }

            _lastX = position.X;
            _lastY = position.Y;

            base.OnMouseMove(map, layer, camera, position, selectedTileName);
        }

        public override void OnMouseUp()
        {
            _lastX = -1;
            _lastY = -1;

            base.OnMouseUp();
        }
    }
}
