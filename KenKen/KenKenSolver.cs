using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KenKen
{
    public class KenKenSolver : IKenKenSolver
    {
        public bool Solve(KenKen kenken)
        {
            kenken.Sort();

            for(int i = 0; i < kenken.groups.Count; i++)
            {
                if (kenken.groups[i].coordinates.Count == 1) kenken[kenken.groups[i].coordinates[0].x, kenken.groups[i].coordinates[0].y] = kenken.groups[i].result;
            }

            KenKen original = (KenKen)kenken.Clone();
            List<(int x, int y, int[] possibleNums)> possibilities = new List<(int, int, int[])>();
            List<(int x, int y)> free = kenken.getFreePlaces();
            bool found = false;
            // try to logically infer
            do
            {
                possibilities.Clear();
                for (int i = 0; i < free.Count; i++)
                {
                    possibilities.Add((free[i].x, free[i].y, GetPossibleNumbers(kenken, free[i].x, free[i].y)));
                }
                found = false;
                for (int i = 0; i < possibilities.Count; i++)
                {
                    if (possibilities[i].possibleNums.Length == 1)
                    {
                        kenken[possibilities[i].x, possibilities[i].y] = possibilities[i].possibleNums[0];
                        free.RemoveAt(i);
                        possibilities.RemoveAt(i);
                        i--;
                        found = true;
                    }
                }
            } while (found);

            if (free.Count == 0) return true;

            // try to brute force

            for (int i = 0; i < possibilities[0].possibleNums.Length; i++)
            {
                KenKen newTry = (KenKen)kenken.Clone();
                newTry[possibilities[0].x, possibilities[0].y] = possibilities[0].possibleNums[i];
                bool success = Solve(newTry);
                if (success && newTry.isValidSolved())
                {
                    for (int j = 0; j < kenken.gameArea.Length; j++)
                    {
                        kenken.gameArea[j] = newTry.gameArea[j];
                    }
                    return true;
                }
            }

            for (int j = 0; j < kenken.gameArea.Length; j++)
            {
                kenken.gameArea[j] = original.gameArea[j];
            }

            return false;
        }

        private int[] GetPossibleNumbers(KenKen kenken, int x, int y)
        {
            List<int> nums = new List<int>();
            for (int i = 1; i <= kenken.Scale; i++)
            {
                if (fitsNumber(kenken, x, y, i)) nums.Add(i);
            }
            return nums.ToArray();
        }

        private bool fitsNumber(KenKen kenken, int x, int y, int number)
        {
            for (int xr = 0; xr < kenken.Scale; xr++)
            {
                if (kenken[xr, y] == number) return false;
            }

            for (int yc = 0; yc < kenken.Scale; yc++)
            {
                if (kenken[x, yc] == number) return false;
            }

            KenKen.Group group = kenken.getGroup(x, y);
            if(group.operation == KenKen.Group.Operation.None)
            {
                return number == group.result;
            }
            if(group.operation == KenKen.Group.Operation.Addition)
            {
                int res = 0;
                for(int i = 0; i < group.coordinates.Count; i++)
                {
                    int val = kenken[group.coordinates[i].x, group.coordinates[i].y];
                    if(val!= KenKen.EMPTYSLOT)
                    {
                        res += val;
                    }
                }
                return res + number <= group.result;
            }
            if(group.operation == KenKen.Group.Operation.Multiplication)
            {
                int res = 1;
                for(int i = 0; i < group.coordinates.Count; i++)
                {
                    int val = kenken[group.coordinates[i].x, group.coordinates[i].y];
                    if (val != KenKen.EMPTYSLOT)
                    {
                        res *= val;
                    }
                }
                return res * number <= group.result;
            }
            if(group.operation == KenKen.Group.Operation.Subtraction)
            {
                if (group.coordinates[0].x == x && group.coordinates[0].y == y)
                {
                    if (kenken[group.coordinates[1].x, group.coordinates[1].y] != KenKen.EMPTYSLOT)
                    {
                        return number - kenken[group.coordinates[1].x, group.coordinates[1].y] == group.result;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (kenken[group.coordinates[0].x, group.coordinates[0].y] != KenKen.EMPTYSLOT)
                    {
                        return kenken[group.coordinates[0].x, group.coordinates[0].y] - number == group.result;
                    }
                    else
                    {
                        return group.result + number <= kenken.Scale;
                    }
                }
            }
            if(group.operation == KenKen.Group.Operation.Division)
            {
                if (group.coordinates[0].x == x && group.coordinates[0].y == y)
                {
                    if (kenken[group.coordinates[1].x, group.coordinates[1].y] != KenKen.EMPTYSLOT)
                    {
                        return number / kenken[group.coordinates[1].x, group.coordinates[1].y] == group.result;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (kenken[group.coordinates[0].x, group.coordinates[0].y] != KenKen.EMPTYSLOT)
                    {
                        return kenken[group.coordinates[0].x, group.coordinates[0].y] / number == group.result;
                    }
                    else
                    {
                        return group.result * number <= kenken.Scale;
                    }
                }
            }

            return false;
        }
    }
}
