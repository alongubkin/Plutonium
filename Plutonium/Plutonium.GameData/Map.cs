using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Krypton;
using Krypton.Lights;

namespace Plutonium.GameData
{
    public partial class Map
    {
        public Map()
        {
            this.Layers = new List<Layer>();
            this.Lights = new List<ILight2D>();
        }

        public Color AmbientColor
        {
            get;
            set;
        }

        public float Bluriness
        {
            get;
            set;
        }
        
        public List<ILight2D> Lights
        {
            get;
            set;
        }

        public List<Layer> Layers
        {
            get;
            set;
        }

        public static Map Load(string path, Texture2D lightTexture)
        {
            using (var file = File.Open(path, FileMode.Open))
            {
                return Map.Load(file, lightTexture);
            }
        }

        public static Map Load(Stream stream, Texture2D lightTexture)
        {
            var map = new Map();
            var xml = XDocument.Load(stream).Element("Map");

            var ambientColorElement = xml.Element("AmbientColor");
            map.AmbientColor = new Color(int.Parse(ambientColorElement.Attribute("R").Value), int.Parse(ambientColorElement.Attribute("G").Value), int.Parse(ambientColorElement.Attribute("B").Value));
            map.Bluriness = float.Parse(xml.Element("Bluriness").Value);

            foreach (var light in xml.Element("Lights").Elements("Light"))
            {
                map.Lights.Add(new Light2D()
                {
                    Angle = float.Parse(light.Attribute("Angle").Value),
                    Texture = lightTexture,
                    Position = new Vector2(float.Parse(light.Attribute("X").Value), float.Parse(light.Attribute("Y").Value)),
                    IsOn = bool.Parse(light.Attribute("IsOn").Value),
                    ShadowType = (ShadowType)Enum.Parse(typeof(ShadowType), light.Attribute("ShadowType").Value),
                    Range = float.Parse(light.Attribute("Range").Value),
                    Fov = float.Parse(light.Attribute("Fov").Value),
                    Intensity = float.Parse(light.Attribute("Intensity").Value),
                    Color = new Color(int.Parse(light.Attribute("R").Value), int.Parse(light.Attribute("G").Value), int.Parse(light.Attribute("B").Value))
                });
            }

            foreach (var layer in xml.Element("Layers").Elements("Layer"))
            {
                map.Layers.Add(Layer.LoadFromXml(layer));
            }

            return map;
        }

        public void Save(string path)
        {
            using (var file = File.Create(path))
            {
                Save(file);
            }
        }

        public void Save(Stream stream)
        {
            var map = new XElement("Map",
                new XElement("AmbientColor",
                    new XAttribute("R", AmbientColor.R),
                    new XAttribute("G", AmbientColor.G),
                    new XAttribute("B", AmbientColor.B)),
                new XElement("Bluriness", Bluriness));

            var lightsElement = new XElement("Lights");

            foreach (Light2D light in Lights)
            {
                lightsElement.Add(new XElement("Light",
                    new XAttribute("Angle", light.Angle),
                    new XAttribute("X", light.Position.X),
                    new XAttribute("Y", light.Position.Y),
                    new XAttribute("IsOn", light.IsOn),
                    new XAttribute("ShadowType", light.ShadowType.ToString()),
                    new XAttribute("Range", light.Range),
                    new XAttribute("Fov", light.Fov),
                    new XAttribute("Intensity", light.Intensity),
                    new XAttribute("R", light.Color.R),
                    new XAttribute("G", light.Color.G),
                    new XAttribute("B", light.Color.B)));
            }

            map.Add(lightsElement);

            var layersElement = new XElement("Layers");

            foreach (var layer in Layers)
            {
                layersElement.Add(layer.ToXml());
            }

            map.Add(layersElement);

            map.Save(stream);
        }

    }
}
