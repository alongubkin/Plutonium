using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Plutonium.GameData;
using System.IO;
using System.Threading;
using Plutonium.MapEditor.UI.Tools;
using MonitoredUndo;

namespace Plutonium.MapEditor.UI
{
    public partial class MainForm : Form
    {
        public static MapPanel MapPanel;

        Dictionary<ToolStripButton, Tool> _tools;

        public MainForm()
        {
            InitializeComponent();

            _tools = new Dictionary<ToolStripButton, Tool>();
            _tools.Add(dragToolButton, new DragTool());
            _tools.Add(drawToolButton, new DrawTool());
            _tools.Add(eraserToolButton, new EraserTool());
            _tools.Add(tilePickerToolButton, new TilePickerTool(this));
            _tools.Add(fillToolButton, new FillTool());
            _tools.Add(lightToolButton, new LightTool());
         
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;

            tilesListView.MultiSelect = false;
            textBox1.Width = tilesListView.Width - textBox1.Location.X;
            comboBox1.Width = tilesListView.Width - comboBox1.Location.X;

            MapPanel = mapPanel1;

            mapPanel1.Map = new Map();
            mapPanel1.ActiveTool = _tools[drawToolButton];

            mapPanel1.Map.Layers.Add(new Layer()
            {
                Name = "Base",
                Order = 0
            });

            mapPanel1.Krpyton.Lights = mapPanel1.Map.Lights;
           // mapPanel1.Krpyton.Hulls = mapPanel1.Map.Shadows;

            mapPanel1.ActiveLayer = mapPanel1.Map.Layers[0];

            UndoService.Current[mapPanel1.Map].UndoStackChanged += new EventHandler(MainForm_UndoStackChanged);
            UndoService.Current[mapPanel1.Map].RedoStackChanged += new EventHandler(MainForm_RedoStackChanged);
            
            
            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Objects");
            TileRepository.Load(directory);

            foreach (var category in Directory.GetDirectories(directory))
            {
                this.comboBox1.Items.Add(Path.GetFileNameWithoutExtension(category));
            }
        }

        void MainForm_RedoStackChanged(object sender, EventArgs e)
        {
            redoToolStripMenuItem.Enabled = UndoService.Current[mapPanel1.Map].CanRedo;

            foreach (var change in UndoService.Current[mapPanel1.Map].RedoStack)
            {
                redoToolStripMenuItem.DropDown.Items.Add(change.Description);
            }
        }

        void MainForm_UndoStackChanged(object sender, EventArgs e)
        {
            undoToolStripMenuItem.Enabled = UndoService.Current[mapPanel1.Map].CanUndo;

            foreach (var change in UndoService.Current[mapPanel1.Map].UndoStack)
            {
                undoToolStripMenuItem.DropDown.Items.Add(change.Description);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenMap();
        }

        public void SelectTile(string name)
        {
            var category = name.Split('/')[0];

            if (comboBox1.Items.Contains(category))
            {
                comboBox1.SelectedItem = category;

                foreach (ListViewItem item in tilesListView.Items)
                {
                    if (item.ImageKey == name)
                    {
                        tilesListView.SelectedItems.Clear();
                        item.Selected = true;
                    }
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            OpenMap();
        }

        void OpenMap()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Plutonium Maps (*.pmap)|*.xml";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    mapPanel1.Map = Map.Load(dialog.FileName, MapPanel.LightTexture);
                    MapPanel.Krpyton.Lights = mapPanel1.Map.Lights;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshListView();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tilesListView.SelectedItems.Count > 0)
            {
                mapPanel1.SelectedTileName = tilesListView.SelectedItems[0].ImageKey;
            }
        }

        private void toolStripButton5_DropDownOpening(object sender, EventArgs e)
        {
            var items = toolStripButton5.DropDown.Items;
            items.Clear();

            foreach (var layer in mapPanel1.Map.Layers)
            {
                items.Add(layer.Name);
            }
        }

        private void toolStripSeparator3_Click(object sender, EventArgs e)
        {
            var form = new LayersManagementForm();
            form.Map = mapPanel1.Map;

            form.ShowDialog();
        }


        private void toolStripButton5_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var layer = mapPanel1.Map.Layers.SingleOrDefault(p => p.Name == e.ClickedItem.Text);

            if (layer != null)
            {
                activeLayerLabel.Text = "Active Layer: " + layer.Name;
                mapPanel1.ActiveLayer = layer;
            }
        }

        private void ToolButton_Click(object sender, EventArgs e)
        {
            foreach (var tool in _tools)
            {
                if (tool.Key == sender)
                {
                    tool.Key.Checked = true;
                    mapPanel1.ActiveTool = tool.Value;

                    if (tool.Value is DrawTool || tool.Value is EraserTool)
                        mapPanel1.Cursor = Cursors.Default;
                    else
                        mapPanel1.Cursor = PInvoke.CreateCursor((Bitmap)tool.Key.Image, 5, 5);

                    if (!(tool.Value is LightTool))
                    {
                        CloseProperties();

                        mapPanel1.SelectedLight = null;
                    }
                }
                else
                {
                    tool.Key.Checked = false;
                }
            }
        }

        private void mapPanel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            mapPanel1.LightsEnabled = toolStripButton9.Checked;
        }

        public void OpenProperties(Krypton.Lights.Light2D light)
        {
            splitContainer3.Panel2Collapsed = false;
            propertyGrid1.SelectedObject = light;
        }

        public void CloseProperties()
        {
            splitContainer3.Panel2Collapsed = true;
            propertyGrid1.SelectedObject = null;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && MapPanel.SelectedLight != null)
            {
                MapPanel.Krpyton.Lights.Remove(MapPanel.SelectedLight);
                MapPanel.SelectedLight = null;

                CloseProperties();
            }
        }

        private void toolStripButton7_Click_1(object sender, EventArgs e)
        {
            var form = new LightingSettings();
            form.Map = mapPanel1.Map;

            form.ShowDialog();
        }

        void NewMap()
        {
            CloseProperties();

            mapPanel1.SelectedLight = null;
            mapPanel1.SelectedTileName = null;

            mapPanel1.Map = new Map();

            mapPanel1.Map.Layers.Add(new Layer()
            {
                Name = "Base",
                Order = 0
            });

            mapPanel1.Krpyton.Lights = mapPanel1.Map.Lights;
           // mapPanel1.Krpyton.Hulls = mapPanel1.Map.Shadows;

            mapPanel1.ActiveLayer = mapPanel1.Map.Layers[0];
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            NewMap();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length >= 3 || (string.IsNullOrWhiteSpace(textBox1.Text) && (comboBox1.SelectedItem != null || !string.IsNullOrWhiteSpace(comboBox1.SelectedText))))
                RefreshListView();
        }

        void RefreshListView()
        {
            var query = TileRepository.GetTiles(textBox1.Text).AsQueryable();

            if (comboBox1.SelectedItem != null)
            {
                query = query.Where(p => p.Name.Split('/')[0] == comboBox1.SelectedItem.ToString());
            }
            else if (!string.IsNullOrWhiteSpace(comboBox1.SelectedText))
            {
                query = query.Where(p => p.Name.Split('/')[0].IndexOf(comboBox1.SelectedText, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            new Thread(new ThreadStart(() =>
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    tilesListView.Clear();
                    imageList1.Images.Clear();

                    foreach (var tile in query.ToList())
                    {
                        imageList1.Images.Add(tile.Name, Image.FromStream(tile.Texture));
                        tilesListView.Items.Add(Path.GetFileNameWithoutExtension(tile.Name), tile.Name);
                    }
                }));
            })).Start();
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            UndoService.Current[mapPanel1.Map].Undo();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            UndoService.Current[mapPanel1.Map].Redo();

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SaveMap();
        }

        void SaveMap()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Plutonium Maps (*.pmap)|*.xml";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    mapPanel1.Map.Save(dialog.FileName);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMap();
        }

        private void newMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewMap();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            textBox1.Width = tilesListView.Width - textBox1.Location.X;
            comboBox1.Width = tilesListView.Width - comboBox1.Location.X;
        }
    }

}
