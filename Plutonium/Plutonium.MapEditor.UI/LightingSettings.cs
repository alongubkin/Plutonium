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
    public partial class LightingSettings : Form
    {
        public LightingSettings()
        {
            InitializeComponent();
        }

        public Map Map
        {
            get;
            set;
        }
     
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Map.Bluriness = (float)trackBar1.Value;
            MainForm.MapPanel.Krpyton.Bluriness = Map.Bluriness;
        }

        private void LightingSettings_Load(object sender, EventArgs e)
        {
            if (Map.AmbientColor != null)
            {
                var color = Color.FromArgb(Map.AmbientColor.R, Map.AmbientColor.G, Map.AmbientColor.B);
                textBox1.Text = ColorTranslator.ToHtml(color);
            }

            trackBar1.Value = (int)Map.Bluriness;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var color = new ColorDialog();
            
            if (!string.IsNullOrWhiteSpace(textBox1.Text))
                color.Color = ColorTranslator.FromHtml(textBox1.Text);

            if (color.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = ColorTranslator.ToHtml(color.Color);
                Map.AmbientColor = new Microsoft.Xna.Framework.Color(color.Color.R, color.Color.G, color.Color.B);
                MainForm.MapPanel.Krpyton.AmbientColor = Map.AmbientColor;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
