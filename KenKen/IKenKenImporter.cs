using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KenKen
{
    public interface IKenKenImporter
    {
        KenKen Import(byte[] bytes);
        string GetMimeType();
        string GetFileExtension();
    }
}
