using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plutonium.GameData;

namespace Plutonium.MapEditor.UI.Tools
{
    public abstract class Tool
    {
        public virtual void OnMouseMove(Map map, Layer layer, Camera2D camera, Vector2 position, string selectedTileName)
        {

        }

        public virtual void OnMouseClick(Map map, Layer layer, Camera2D camera, Vector2 position, string selectedTileName)
        {

        }

        public virtual void OnMouseUp() { }

        public virtual void Draw(SpriteBatch spriteBatch, Camera2D camera, Vector2 mousePosition, Texture2D pixel)
        {

        }
    }
}
