using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KenKen
{
    public interface IKenKenGenerator
    {
        KenKen Generate();
        KenKen Generate(int seed);
        KenKen Generate(int scale, int seed);
    }
}
