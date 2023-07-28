using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KenKen
{
    public class PngExporter : IKenKenExporter
    {
        public Brush Background = Brushes.White;
        public Brush Foreground = Brushes.Black;

        public byte[] Export(KenKen kenken)
        {
            Pen bg = new Pen(Background, 2);
            Pen fg = new Pen(Foreground, 2);
            Pen bgb = new Pen(Background, 5);
            Pen fgb = new Pen(Foreground, 5);

            Bitmap bmp = new Bitmap(4 + kenken.Width * 55, 4 + kenken.Height * 55);
            Graphics g = Graphics.FromImage(bmp);
            
            g.FillRectangle(Background, 0, 0, bmp.Width, bmp.Height);

            g.DrawRectangle(fgb, 2, 2, bmp.Width - 5, bmp.Height - 5);

            for(int y = 2; y < bmp.Height; y += 55)
            {
                g.DrawLine(fg, 0, y, bmp.Width, y);
            }

            for(int x = 2; x < bmp.Width; x += 55)
            {
                g.DrawLine(fg, x, 0, x, bmp.Width);
            }

            Font smallFont = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular);

            for(int gr = 0; gr < kenken.groups.Count; gr++)
            {
                kenken.groups[gr].Sort();
                g.DrawString(Convert.ToString(kenken.groups[gr].result) + OperationToChar(kenken.groups[gr].operation), smallFont, Foreground, 5 + 55 * kenken.groups[gr].coordinates[0].x, 5 + 55 * kenken.groups[gr].coordinates[0].y);
                for(int cell = 0; cell < kenken.groups[gr].coordinates.Count; cell++)
                {
                    if (!kenken.groups[gr].Contains(kenken.groups[gr].coordinates[cell].x + 1, kenken.groups[gr].coordinates[cell].y))
                    {
                        g.DrawLine(fgb, 2 + 55 * (kenken.groups[gr].coordinates[cell].x + 1), 55 * (kenken.groups[gr].coordinates[cell].y), 2 + 55 * (kenken.groups[gr].coordinates[cell].x + 1), 4 + 55 * (kenken.groups[gr].coordinates[cell].y + 1));
                    }
                    if (!kenken.groups[gr].Contains(kenken.groups[gr].coordinates[cell].x - 1, kenken.groups[gr].coordinates[cell].y))
                    {
                        g.DrawLine(fgb, 2 + 55 * (kenken.groups[gr].coordinates[cell].x), 55 * (kenken.groups[gr].coordinates[cell].y), 2 + 55 * (kenken.groups[gr].coordinates[cell].x), 4 + 55 * (kenken.groups[gr].coordinates[cell].y + 1));
                    }
                    if (!kenken.groups[gr].Contains(kenken.groups[gr].coordinates[cell].x, kenken.groups[gr].coordinates[cell].y + 1))
                    {
                        g.DrawLine(fgb, 55 * (kenken.groups[gr].coordinates[cell].x), 2 + 55 * (kenken.groups[gr].coordinates[cell].y + 1), 4 + 55 * (kenken.groups[gr].coordinates[cell].x + 1), 2 + 55 * (kenken.groups[gr].coordinates[cell].y + 1));
                    }
                    if (!kenken.groups[gr].Contains(kenken.groups[gr].coordinates[cell].x, kenken.groups[gr].coordinates[cell].y - 1))
                    {
                        g.DrawLine(fgb, 55 * (kenken.groups[gr].coordinates[cell].x), 2 + 55 * (kenken.groups[gr].coordinates[cell].y), 4 + 55 * (kenken.groups[gr].coordinates[cell].x + 1), 2 + 55 * (kenken.groups[gr].coordinates[cell].y));
                    }
                }
            }

            Font bigFont = new Font(FontFamily.GenericSansSerif, 30, FontStyle.Bold);

            for(int y = 0; y < kenken.Height; y++)
            {
                for(int x = 0; x < kenken.Width; x++)
                {
                    if (kenken[x, y] != KenKen.EMPTYSLOT) g.DrawString(Convert.ToString(kenken[x, y]), bigFont, Foreground, 10 + 55 * x, 8 + 55 * y);
                }
            }

            g.Flush();
            g.Dispose();

            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            bmp.Dispose();

            return ms.ToArray();
        }

        private string OperationToChar(KenKen.Group.Operation operation)
        {
            switch(operation)
            {
                case KenKen.Group.Operation.Addition:
                    return "+";
                case KenKen.Group.Operation.Subtraction:
                    return "-";
                case KenKen.Group.Operation.Multiplication:
                    return "×";
                case KenKen.Group.Operation.Division:
                    return "÷";
            }
            return "?";
        }

        public string GetFileExtension()
        {
            return "png";
        }

        public string GetMimeType()
        {
            return "image/png";
        }
    }
}
