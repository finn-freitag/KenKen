using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KenKen
{
    public class CsvImporter : IKenKenImporter
    {
        public string GetFileExtension()
        {
            return "csv";
        }

        public string GetMimeType()
        {
            return "text/csv";
        }

        public KenKen Import(byte[] bytes)
        {
            string[] ops = { "", "+", "-", "*", "/" };
            string raw = Encoding.ASCII.GetString(bytes);
            string[] lines = raw.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            KenKen kenken = new KenKen(Convert.ToInt32(lines[0]));
            for(int y = 0; y < kenken.Scale; y++)
            {
                string[] parts = lines[y + 1].Split(',');
                for(int x = 0; x < kenken.Scale; x++)
                {
                    kenken[x, y] = Convert.ToInt32(parts[x]);
                }
            }
            for (int i = kenken.Scale + 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(',');
                KenKen.Group group = new KenKen.Group();
                group.result = Convert.ToInt32(parts[0]);
                group.operation = (KenKen.Group.Operation)ops.ToList().IndexOf(parts[1]);
                for(int j = 2; j < parts.Length; j++)
                {
                    string[] coordinate = parts[j].Split('/');
                    group.coordinates.Add((Convert.ToInt32(coordinate[0]), Convert.ToInt32(coordinate[1])));
                }
                kenken.groups.Add(group);
            }
            return kenken;
        }
    }
}
