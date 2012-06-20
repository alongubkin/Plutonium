using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Plutonium.AnimationBuilder
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Gif Images (*.gif)|*.gif";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    listBox1.Items.Add(dialog.FileName);
                }
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (listBox1.SelectedItem != null)
                {
                    listBox1.Items.Remove(listBox1.SelectedItem);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "Png Image (*.png)|*.png";

                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Bitmap[,] matrix = new Bitmap[200, 200];

                        int y = 0;
                        int maxFrames = 0;

                        foreach (string fileName in listBox1.Items)
                        {
                            var image = Image.FromFile(fileName);
                            var dimension = new FrameDimension(image.FrameDimensionsList[0]);

                            int frameCount = image.GetFrameCount(dimension);

                            for (int frame = 0; frame < frameCount; frame++)
                            {
                                image.SelectActiveFrame(dimension, frame);
                                matrix[y, frame] = new Bitmap(image);
                            }

                            maxFrames = Math.Max(maxFrames, frameCount);
                            y++;
                        }

                        var final = new Bitmap(maxFrames * matrix[0, 0].Width, y * matrix[0, 0].Height);

                        using (var fx = Graphics.FromImage(final))
                        {
                            for (int g = 0; g < y; g++)
                            {
                                for (int x = 0; x < 200; x++)
                                {
                                    if (matrix[g, x] != null)
                                    {
                                        MessageBox.Show((x * matrix[g, x].Width).ToString() + " " + x);
                                        fx.DrawImage((Image)matrix[g, x], new Point(x * matrix[g, x].Width, g * matrix[g, x].Height));
                                    }
                                }
                            }
                            


                            fx.Flush();
                            
                        }

                        final.Save(dialog.FileName, ImageFormat.Png);

                    }
                }
            }
        }
    }
}
