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
            KenKen kenken = new KenKen(6, 4);
            kenken[0, 0] = 3;
            kenken[1, 1] = 6;
            KenKen.Group g1 = new KenKen.Group();
            g1.result = 24;
            g1.operation = KenKen.Group.Operation.Multiplication;
            g1.coordinates.Add((0, 0));
            g1.coordinates.Add((0, 1));
            g1.coordinates.Add((1, 1));
            g1.coordinates.Add((1, 2));
            kenken.groups.Add(g1);
            pictureBox1.Image = new Bitmap(new MemoryStream(new PngExporter().Export(kenken)));
        }
    }
}
