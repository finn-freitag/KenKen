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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KenKen kenken = new KenKenGenerator().Generate();
            pictureBox1.Image = new Bitmap(new MemoryStream(new PngExporter().Export(kenken)));
        }
    }
}
