using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KenKen
{
    public class CsvExporter : IKenKenExporter
    {
        public byte[] Export(KenKen kenken)
        {
            string[] ops = { "", "+", "-", "*", "/" };
            string final = Convert.ToString(kenken.Scale) + Environment.NewLine;
            for (int y = 0; y < kenken.Scale; y++)
            {
                for (int x = 0; x < kenken.Scale; x++)
                {
                    final += Convert.ToString(kenken[x, y]) + ",";
                }
                final += Environment.NewLine;
            }
            for(int i = 0; i < kenken.groups.Count; i++)
            {
                final += Convert.ToString(kenken.groups[i].result) + "," + Convert.ToString(ops[(int)kenken.groups[i].operation]);
                for(int j = 0; j < kenken.groups[i].coordinates.Count; j++)
                {
                    final += "," + Convert.ToString(kenken.groups[i].coordinates[j].x) + "/" + Convert.ToString(kenken.groups[i].coordinates[j].y);
                }
                final += Environment.NewLine;
            }
            return Encoding.ASCII.GetBytes(final);
        }

        public string GetFileExtension()
        {
            return "csv";
        }

        public string GetMimeType()
        {
            return "text/csv";
        }
    }
}
