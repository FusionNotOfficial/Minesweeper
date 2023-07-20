using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public enum States
    {
        Cell,
        Bomb,
        Digit,
        Wall,
    }
    class Cell
    {
        public string Symbol { get; set; } = "#";
        public States State { get; set; } = States.Cell;
        public bool Visible = false;
        private int count = 0;
        public Cell() { }
        public Cell(bool iswall)
        {
            Symbol = "=";
            State = States.Wall;
            Visible = true;
        }

        public void SetDigit(Cell[,] field, int x, int y)
        {
            Visible = false;
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    var tempCell = field[x + i, y + j];
                    if (tempCell.State == States.Bomb)
                        count++;
                }
            }
            Symbol = count.ToString();
        }
        public int GetDigit()
        {
            return count > 0 ? count : 0;
        }
    }
}
