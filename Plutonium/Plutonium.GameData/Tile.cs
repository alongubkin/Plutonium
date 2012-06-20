using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Plutonium.GameData
{
    public class Tile
    {
        public string Name
        {
            get;
            set;
        }

        public Stream Texture
        {
            get;
            set;
        }
    }

    public static class TileRepository
    {
        static Dictionary<string, Tile> _tiles;

        static TileRepository()
        {
            _tiles = new Dictionary<string, Tile>();
        }

        public static void Load(string path)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                foreach (var file in Directory.GetFiles(directory))
                {
                    var name = Path.Combine(Path.GetFileNameWithoutExtension(directory) + "/", Path.GetFileNameWithoutExtension(file));

                    _tiles.Add(name, new Tile()
                    {
                        Name = name,
                        Texture = File.OpenRead(file)
                    });
                }
            }
        }

        public static Tile GetTile(string name)
        {
            return _tiles[name];
        }

        public static IEnumerable<Tile> GetTiles(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return _tiles.Select(p => p.Value);

            return _tiles.Where(p => p.Key.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0).Select(p => p.Value);
        }
    }
}
