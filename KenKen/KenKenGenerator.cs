using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KenKen
{
    public class KenKenGenerator : IKenKenGenerator
    {
        public KenKen Generate()
        {
            return Generate(6, DateTime.Now.GetHashCode());
        }

        public KenKen Generate(int seed)
        {
            return Generate(6, seed);
        }

        public KenKen Generate(int scale, int seed)
        {
            Random r = new Random(seed);
            KenKen kenken = GenerateValidBasePattern(scale, r);
            for(int y = 0; y < scale; y++)
            {
                for(int x = 0; x < scale; x++)
                {
                    if (!kenken.isGroupAssigned(x, y)) kenken.groups.Add(generateGroup(kenken, x, y, r));
                }
            }
            for (int i = 0; i < kenken.gameArea.Length; i++)
            {
                kenken.gameArea[i] = KenKen.EMPTYSLOT;
            }
            kenken.Sort();
            return kenken;
        }

        private KenKen.Group generateGroup(KenKen kenken, int x, int y, Random r)
        {
            int groupSize = r.Next(3) + 1;
            if (groupSize == 1) groupSize++; // There should be twice as many 2 sized groups than 3 and 4 sized groups.
            (int x, int y) lastPos = (x, y);
            KenKen.Group group = new KenKen.Group();
            group.coordinates.Add((x, y));
            for(int i = 1; i < groupSize; i++)
            {
                Direction direction = (Direction)r.Next(4);
                (int x, int y) pos = getFromDirection(lastPos.x, lastPos.y, direction);
                int counter = 0;
                while (!isValid(kenken, pos.x, pos.y) || kenken.isGroupAssigned(pos.x, pos.y) || group.Contains(pos.x, pos.y))
                {
                    direction = (Direction)r.Next(4);
                    pos = getFromDirection(lastPos.x, lastPos.y, direction);
                    if (counter == 8)
                    {
                        for(int j = 0; j < 4; j++)
                        {
                            pos = getFromDirection(lastPos.x, lastPos.y, (Direction)j);
                            if (isValid(kenken, pos.x, pos.y) && !kenken.isGroupAssigned(pos.x, pos.y) && !group.Contains(pos.x, pos.y)) break;
                        }
                        break;
                    }
                    counter++;
                }
                if (isValid(kenken, pos.x, pos.y) && !kenken.isGroupAssigned(pos.x, pos.y) && !group.Contains(pos.x, pos.y))
                {
                    group.coordinates.Add((pos.x, pos.y));
                    lastPos = pos;
                }
                else break;
            }
            if (group.coordinates.Count == 1)
            {
                group.result = kenken[group.coordinates[0].x, group.coordinates[0].y];
                group.operation = KenKen.Group.Operation.Addition;
                return group;
            }
            group.Sort();
            List<KenKen.Group.Operation> possibleOperators = new List<KenKen.Group.Operation>();
            possibleOperators.Add(KenKen.Group.Operation.Addition);
            possibleOperators.Add(KenKen.Group.Operation.Multiplication);
            if (group.coordinates.Count == 2 && kenken[group.coordinates[0].x, group.coordinates[0].y] >= kenken[group.coordinates[1].x, group.coordinates[1].y]) possibleOperators.Add(KenKen.Group.Operation.Subtraction);
            if (group.coordinates.Count == 2 && (double)kenken[group.coordinates[0].x, group.coordinates[0].y] / kenken[group.coordinates[1].x, group.coordinates[1].y] % 1 == 0) possibleOperators.Add(KenKen.Group.Operation.Division);
            KenKen.Group.Operation operation = possibleOperators[r.Next(possibleOperators.Count)];
            group.operation = operation;
            int res = 0;
            if (operation == KenKen.Group.Operation.Addition)
            {
                for(int i = 0; i < group.coordinates.Count; i++)
                {
                    res += kenken[group.coordinates[i].x, group.coordinates[i].y];
                }
            }
            if(operation == KenKen.Group.Operation.Multiplication)
            {
                res = 1;
                for (int i = 0; i < group.coordinates.Count; i++)
                {
                    res *= kenken[group.coordinates[i].x, group.coordinates[i].y];
                }
            }
            if(operation == KenKen.Group.Operation.Subtraction)
            {
                res = kenken[group.coordinates[0].x, group.coordinates[0].y] - kenken[group.coordinates[1].x, group.coordinates[1].y];
            }
            if(operation == KenKen.Group.Operation.Division)
            {
                res = kenken[group.coordinates[0].x, group.coordinates[0].y] / kenken[group.coordinates[1].x, group.coordinates[1].y];
            }
            group.result = res;
            return group;
        }

        private bool isValid(KenKen kenken, int x, int y)
        {
            return x >= 0 && y >= 0 && x < kenken.Scale && y < kenken.Scale;
        }

        private (int x, int y) getFromDirection(int x, int y, Direction direction)
        {
            if (direction == Direction.Up) return (x, y - 1);
            if (direction == Direction.Right) return (x + 1, y);
            if (direction == Direction.Down) return (x, y + 1);
            if (direction == Direction.Left) return (x - 1, y);
            return (-1, -1);
        }

        public KenKen GenerateValidBasePattern(int scale, Random r)
        {
            KenKen kenken = Generate(new KenKen(scale), r);
            while (!kenken.isValidSolved()) kenken = Generate(new KenKen(scale), r);
            return kenken;
        }

        private KenKen Generate(KenKen kenken, Random r)
        {
            List<(int x, int y, int[] possibleNums)> possibilities = new List<(int, int, int[])>();
            List<(int x, int y)> free = kenken.getFreePlaces();
            bool found = false;
            // try to logically infer
            do
            {
                possibilities.Clear();
                for (int i = 0; i < free.Count; i++)
                {
                    int[] possNums = GetPossibleNumbers(kenken, free[i].x, free[i].y);
                    if (possNums.Length == 0) return Generate(new KenKen(kenken.Scale), r);
                    possibilities.Add((free[i].x, free[i].y, possNums));
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

            if (free.Count == 0) return kenken;

            // try to brute force

            var possibility = possibilities[r.Next(possibilities.Count)];
            if (possibility.possibleNums.Length == 0) return Generate(new KenKen(kenken.Scale), r);
            kenken[possibility.x, possibility.y] = possibility.possibleNums[r.Next(possibility.possibleNums.Length)];

            return Generate(kenken, r);
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

            return true;
        }

        public enum Direction : int
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3
        }
    }
}
