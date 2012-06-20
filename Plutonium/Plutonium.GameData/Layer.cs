using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Xml.Linq;
using Krypton;

namespace Plutonium.GameData
{
    public class Layer
    {
        public Layer()
        {
            Tiles = new Dictionary<Vector2, Tile>(); this.Shadows = new List<ShadowHull>();
        }

        public string Name
        {
            get;
            set;
        }

        public int Order
        {
            get;
            set;
        }

        public List<ShadowHull> Shadows
        {
            get;
            set;
        }

        public Dictionary<Vector2, Tile> Tiles
        {
            get;
            set;
        }

        public static Layer LoadFromXml(XElement element)
        {
            Layer layer = new Layer();
            layer.Name = element.Attribute("Name").Value;
            layer.Order = int.Parse(element.Attribute("Order").Value);

            foreach (var tile in element.Elements())
            {
                layer.Tiles.Add(new Vector2(float.Parse(tile.Attribute("X").Value), float.Parse(tile.Attribute("Y").Value)), 
                    TileRepository.GetTile(tile.Attribute("Name").Value));
            }

            return layer;
        }

        public XElement ToXml()
        {
            var layer = new XElement("Layer",
                new XAttribute("Name", Name),
                new XAttribute("Order", Order));

            foreach (var tile in Tiles)
            {
                layer.Add(new XElement("Tile", new XAttribute("X", tile.Key.X),
                                               new XAttribute("Y", tile.Key.Y),
                                               new XAttribute("Name", tile.Value.Name)));
            }

            return layer;
          
        }

        /*
        public static Layer Load(List<List<int>> matrix, Dictionary<int, Stream> provider)
        {
            var layer = new Layer();

            for (var y = 0; y < matrix.Count; y++)
            {
                for (var x = 0; x < matrix[y].Count; x++)
                {
                    if (matrix[y][x] != 0)
                    {
                        var texture = provider[matrix[y][x]];

                        layer.Tiles.Add(new Point(x, y), new Tile()
                        {
                            Id = matrix[y][x],
                            Texture = texture,
                        });
                    }
                }
            }           

            return layer;
        }

        public static Layer Load(string data, Dictionary<int, Stream> provider)
        {
            return Load(data.Split('\n').Where(p => !string.IsNullOrWhiteSpace(p)).Select(line => line.Trim().Split(' ').Select(item => int.Parse(item)).ToList()).ToList(), 
                provider);
        }*/
    }
}
