using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Plutonium.GameData;

namespace Plutonium.MapEditor.UI
{
    public partial class LayersManagementForm : Form
    {
        public LayersManagementForm()
        {
            InitializeComponent();
        }

        public Map Map
        {
            get;
            set;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Map.Layers.SingleOrDefault(p => p.Name == textBox1.Text) == null)
            {
                listBox1.Items.Add(textBox1.Text);

                var order = 0;
                var lastLayer = Map.Layers.OrderBy(p => p.Order).LastOrDefault();

                if (lastLayer != null)
                {
                    order = lastLayer.Order + 1;
                }

                Map.Layers.Add(new Layer()
                {
                    Name = textBox1.Text,
                    Order = order
                });
            }
            else
            {
                MessageBox.Show("Layer with this name already exists.", "Plutonium", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LayersManagementForm_Load(object sender, EventArgs e)
        {
            foreach (var layer in Map.Layers)
            {
                listBox1.Items.Add(layer.Name);
            }
        }
    }
}
