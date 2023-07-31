using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KenKen
{
    public interface IKenKenSolver
    {
        bool Solve(KenKen kenken);
    }
}
