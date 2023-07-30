using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KenKen
{
    public class KenKen : ICloneable
    {
        public const int EMPTYSLOT = -1;

        public int[] gameArea;

        public List<Group> groups;

        public readonly int Scale = 6;

        public KenKen()
        {
            Scale = 6;
            groups = new List<Group>();
            gameArea = new int[36];
            for (int i = 0; i < gameArea.Length; i++)
            {
                gameArea[i] = EMPTYSLOT;
            }
        }

        public KenKen(int scale)
        {
            Scale = scale;
            groups = new List<Group>();
            gameArea = new int[scale * scale];
            for (int i = 0; i < gameArea.Length; i++)
            {
                gameArea[i] = EMPTYSLOT;
            }
        }

        public int this[int x, int y]
        {
            get
            {
                return Get(x, y);
            }
            set
            {
                Set(x, y, value);
            }
        }

        public int Get(int x, int y)
        {
            return gameArea[x + y * Scale];
        }

        public void Set(int x, int y, int value)
        {
            gameArea[x + y * Scale] = value;
        }

        public bool isFree(int x, int y)
        {
            return Get(x, y) == EMPTYSLOT;
        }

        public List<(int x, int y)> getFreePlaces()
        {
            List<(int, int)> freePlaces = new List<(int, int)>();
            for (int i = 0; i < gameArea.Length; i++)
            {
                if (gameArea[i] == EMPTYSLOT) freePlaces.Add((i % Scale, i / Scale));
            }
            return freePlaces;
        }

        public bool isValidSolved()
        {
            // Init lists and arrays
            List<int>[] rows = new List<int>[9];
            List<int>[] columns = new List<int>[9];
            for (int i = 0; i < Scale; i++)
            {
                rows[i] = new List<int>();
                columns[i] = new List<int>();
            }

            // enumerate every place in the sudoku
            for (int y = 0; y < Scale; y++)
            {
                for (int x = 0; x < Scale; x++)
                {
                    // check validity
                    if (this[x, y] == EMPTYSLOT) return false;
                    if (rows[y].Contains(this[x, y])) return false; else rows[y].Add(this[x, y]);
                    if (columns[x].Contains(this[x, y])) return false; else columns[x].Add(this[x, y]);
                }
            }

            for(int i = 0; i < groups.Count; i++)
            {
                if (!groups[i].isValidSolved(this)) return false;
            }
            return true;
        }

        public bool isGroupAssigned(int x, int y)
        {
            for(int i = 0; i < groups.Count; i++)
            {
                if (groups[i].Contains(x, y)) return true;
            }
            return false;
        }

        public bool areAllPlacesGroupAssigned()
        {
            for(int x = 0; x < Scale; x++)
            {
                for(int y = 0; y < Scale; y++)
                {
                    if (!isGroupAssigned(x, y)) return false;
                }
            }
            return true;
        }

        public object Clone()
        {
            KenKen kenken = new KenKen(Scale);
            for(int i = 0; i < gameArea.Length; i++)
            {
                kenken.gameArea[i] = gameArea[i];
            }
            for(int i = 0; i < groups.Count; i++)
            {
                kenken.groups.Add((Group)groups[i].Clone());
            }
            return kenken;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is KenKen)) return false;
            KenKen kk = (KenKen)obj;
            for (int i = 0; i < gameArea.Length; i++)
            {
                if (kk.gameArea[i] != gameArea[i]) return false;
            }
            return true;
        }

        private delegate int TupleComparison((int x, int y) a, (int x, int y) b);
        
        public static void Sort(List<(int x, int y)> coordinates)
        {
            TupleComparison comparer = (a, b) =>
            {
                if (a.y < b.y) return -1;
                if (b.y < a.y) return 1;
                if (a.x < b.x) return -1;
                if (b.x < a.x) return 1;
                return 0;
            };
            coordinates.Sort(new Comparison<(int x, int y)>(comparer));
        }

        public class Group : ICloneable
        {
            public List<(int x, int y)> coordinates = new List<(int x, int y)>();

            public int result = 1;

            public Operation operation = Operation.Multiplication;

            public bool isValidSolved(KenKen kenken)
            {
                if(operation == Operation.Addition)
                {
                    int sum = 0;
                    for(int i = 0; i < coordinates.Count; i++)
                    {
                        int val = kenken[coordinates[i].x, coordinates[i].y];
                        if (val == KenKen.EMPTYSLOT) return false;
                        sum += val;
                    }
                    return sum == result;
                }
                if(operation == Operation.Multiplication)
                {
                    int product = 0;
                    for(int i = 0; i < coordinates.Count; i++)
                    {
                        int val = kenken[coordinates[i].x, coordinates[i].y];
                        if (val == KenKen.EMPTYSLOT) return false;
                        product *= val;
                    }
                    return product == result;
                }
                if(operation == Operation.None)
                {
                    return result == kenken[coordinates[0].x, coordinates[0].y];
                }
                Sort();
                if(operation == Operation.Subtraction)
                {
                    int val1 = kenken[coordinates[0].x, coordinates[0].y];
                    if (val1 == KenKen.EMPTYSLOT) return false;
                    int val2 = kenken[coordinates[1].x, coordinates[1].y];
                    if (val2 == KenKen.EMPTYSLOT) return false;
                    return val1 - val2 == result;
                }
                if(operation == Operation.Division)
                {
                    int val1 = kenken[coordinates[0].x, coordinates[0].y];
                    if (val1 == KenKen.EMPTYSLOT) return false;
                    int val2 = kenken[coordinates[1].x, coordinates[1].y];
                    if (val2 == KenKen.EMPTYSLOT) return false;
                    return val1 / val2 == result;
                }
                return false;
            }

            public void Sort()
            {
                KenKen.Sort(coordinates);
            }

            public bool Contains(int x, int y)
            {
                for(int i = 0; i < coordinates.Count; i++)
                {
                    if (coordinates[i].x == x && coordinates[i].y == y) return true;
                }
                return false;
            }

            public object Clone()
            {
                Group g = new Group();
                for(int i = 0; i < coordinates.Count; i++)
                {
                    g.coordinates.Add((coordinates[i].x, coordinates[i].y));
                }
                g.result = result;
                g.operation = operation;
                return g;
            }

            public enum Operation
            {
                Addition,
                Subtraction,
                Multiplication,
                Division,
                None
            }
        }
    }
}
