using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Plutonium.GameData;
using Plutonium.MapEditor.UI.Tools;
using System.IO;
using Krypton;
using Krypton.Lights;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;

namespace Plutonium.MapEditor.UI
{
    public partial class MapPanel : GraphicsDeviceControl 
    {
        public static Texture2D LightTexture;

        SpriteBatch _spriteBatch;
        Texture2D _pixel;
        Vector2 _currentMousePosition;

        KryptonEngine _krypton;
        ContentManager contentManager;

        public Texture2D _lightSymbolTexture;
        public Color[] _lightSymbolPixelData;
        public Light2D _movingLight = null;

        Texture2D _selectionTexture;

        public Light2D SelectedLight
        {
            get;
            set;
        }

        bool mouseDown = false;
      
        public Dictionary<string, Texture2D> _cache;

        static MapPanel()
        {
            
        }

        public MapPanel()
            : base()
        {
            contentManager = new ContentManager(this.Services, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/"));
            Map = new Map();
        }

        public Camera2D Camera { get; set; }
        public KryptonEngine Krpyton { get { return _krypton; } }

        public Map Map
        {
            get;
            set;
        }

        public Layer ActiveLayer
        {
            get;
            set;
        }

        public Tool ActiveTool
        {
            get;
            set;
        }

        public bool LightsEnabled
        {
            get;
            set;
        }

        public string SelectedTileName
        {
            get;
            set;
        }

        public bool IsGridEnabled
        {
            get;
            set;
        }

        protected override void Initialize()
        {
            try
            {
                _spriteBatch = new SpriteBatch(GraphicsDevice);

                Camera = new Camera2D();
                Camera.Position = new Vector3(500, 500, 0);
             /*   Camera.Initialize();
                Camera.FocusPosition = new Vector2(0, 0); ;
                */
                _pixel = new Texture2D(GraphicsDevice, 1, 1);
                _pixel.SetData<Color>(new Color[] { Color.White });

                _cache = new Dictionary<string, Texture2D>();

                GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                _krypton = new KryptonEngine();
                _krypton.GraphicsDevice = GraphicsDevice;
                _krypton.mEffect = contentManager.Load<Effect>("KryptonEffect");
                _krypton.Initialize();

                _krypton.LoadContent();

                LightTexture = LightTextureBuilder.CreatePointLight(GraphicsDevice, 1024);
                //_krypton.
               // _krypton.AmbientColor = Color.DarkGreen;
                _krypton.CullMode = CullMode.None;
                _krypton.SpriteBatchCompatablityEnabled = true;
                
                _lightSymbolTexture = contentManager.Load<Texture2D>("light_bulb");
                _lightSymbolPixelData = GetPixelData(_lightSymbolTexture);

                _selectionTexture = contentManager.Load<Texture2D>("selection");

                System.Windows.Forms.Application.Idle += delegate { Invalidate(); };

                this.MouseMove += new System.Windows.Forms.MouseEventHandler(OnMouseMove);
                this.MouseDown += new System.Windows.Forms.MouseEventHandler(OnMouseDown);
                this.MouseUp += new System.Windows.Forms.MouseEventHandler(OnMouseUp);
                this.MouseClick += new System.Windows.Forms.MouseEventHandler(MapPanel_MouseClick);
            }
            catch( Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);

            }
        }

        void MapPanel_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ActiveTool.OnMouseClick(Map, ActiveLayer, Camera, _currentMousePosition, SelectedTileName);
        }

        void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = false;
            ActiveTool.OnMouseUp();
        }

        void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = true;
        }

        void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _currentMousePosition = new Vector2(e.X, e.Y);

            if (mouseDown)
            {
                ActiveTool.OnMouseMove(Map, ActiveLayer, Camera, _currentMousePosition, SelectedTileName);
            }
        }

        protected override void Draw()
        {
            Camera.Update();

            if (LightsEnabled)
            {
               _krypton.Matrix = Camera.View;
                this._krypton.LightMapPrepare();
            }

            GraphicsDevice.Clear(new Color(240,240,240));

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Camera.View);

            foreach (var layer in Map.Layers.Where(p => p.Order <= ActiveLayer.Order).OrderBy(p => p.Order))
            {
                DrawLayer(layer);
            }

            _spriteBatch.End();

            if (LightsEnabled)
                this._krypton.Draw();

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Camera.View);
            
            ActiveTool.Draw(_spriteBatch, Camera, _currentMousePosition, _pixel);

            if (!mouseDown && _movingLight != null)
                _movingLight = null;
            
            foreach (Light2D light in _krypton.Lights)
            {
                if (light == SelectedLight)
                    _spriteBatch.Draw(_selectionTexture, light.Position + new Vector2(1, -1), Color.White);

                _spriteBatch.Draw(_lightSymbolTexture, light.Position/* - new Vector2(_lightSymbolTexture.Width / 2, _lightSymbolTexture.Height / 2)*/, Color.White);
            }

            _spriteBatch.End();
        }

        void DrawLayer(Layer layer)
        {
            foreach (var tile in layer.Tiles)
            {
                if (!_cache.ContainsKey(tile.Value.Name))
                {
                    var texture = Texture2D.FromStream(GraphicsDevice, tile.Value.Texture);
                    _cache.Add(tile.Value.Name, texture);
                }

                _spriteBatch.Draw(_cache[tile.Value.Name],
                    new Vector2(tile.Key.X * 32 + 1, tile.Key.Y * 32), Color.White);
            }
        }
       
        public void AddShadowForObject(Texture2D texture, Vector2 position)
        {
            uint[] data = new uint[texture.Width * texture.Height];
            texture.GetData<uint>(data);

            foreach (var poly in EarclipDecomposer.ConvexPartition(PolygonTools.CreatePolygon(data, texture.Width)))
            {
                if (poly.Count >= 3)
                {
                    var array = poly.ToArray();
                    
                    var hull = ShadowHull.CreateConvex(ref array);
                    hull.Position = position;

                    _krypton.Hulls.Add(hull);
                }
            }
        }

        private Color[] GetPixelData(Texture2D texture)
        {
            Color[] pixels = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(pixels);
            return pixels;
        }
    }
}
