using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KenKen
{
    public partial class Form1 : Form
    {
        KenKen kenken;

        public Form1()
        {
            InitializeComponent();
            kenken = new KenKenGenerator().Generate((int)numericUpDown1.Value, DateTime.Now.GetHashCode());
            pictureBox1.Image = new Bitmap(new MemoryStream(new PngExporter().Export(kenken)));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            kenken = new KenKenGenerator().Generate((int)numericUpDown1.Value, DateTime.Now.GetHashCode());
            pictureBox1.Image = new Bitmap(new MemoryStream(new PngExporter().Export(kenken)));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG Image|*.png";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                new Bitmap(new MemoryStream(new PngExporter().Export(kenken))).Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(new KenKenSolver().Solve(kenken))
            {
                pictureBox1.Image = new Bitmap(new MemoryStream(new PngExporter().Export(kenken)));
            }
            else
            {
                MessageBox.Show("Unsolvable!");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV File|*.csv";
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(sfd.FileName, new CsvExporter().Export(kenken));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV File|*.csv";
            if(ofd.ShowDialog()==DialogResult.OK)
            {
                kenken = new CsvImporter().Import(File.ReadAllBytes(ofd.FileName));
                pictureBox1.Image = new Bitmap(new MemoryStream(new PngExporter().Export(kenken)));
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (kenken.isValidSolved())
            {
                MessageBox.Show("KenKen is solved correctly!");
            }
            else
            {
                MessageBox.Show("There is at least one mistake!");
            }
        }
    }
}
