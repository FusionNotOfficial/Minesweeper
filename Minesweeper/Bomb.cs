using System;

namespace Minesweeper
{
    class Bomb
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Bomb(int x, int y)
        {
            X = x;
            Y = y;
        }
        public bool Compair(Bomb bomb)
        {
            return X == bomb.X && Y == bomb.Y;
        }
    }
}
