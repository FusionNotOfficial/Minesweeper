using System;
using System.Collections.Generic;

namespace Minesweeper
{
    class Program
    {
        static int FIELD_SIZE = 11; // Optimal field size. You could write whatever you want
                                    // Just be patient for visual issues. Algorithm is correct
        static int BOMBS_AMOUNT = (FIELD_SIZE - 2) * 10 / 7; // You could set any positive number of bombs

        static int SCORE = (int)Math.Pow(FIELD_SIZE - 2, 2) - BOMBS_AMOUNT;
        static int BOMBS_LEFT = BOMBS_AMOUNT;
        static void Main(string[] args)
        {
            Cell[,] field = new Cell[FIELD_SIZE, FIELD_SIZE];
            Bomb[] bombs = new Bomb[BOMBS_AMOUNT];

            bool gameOver = false;

            SetupField(field);
            SetBombs(field, bombs);
            SetDigits(field);

            int choice;

            while (!gameOver && SCORE > 0)
            {
                Print(field);
                // ShowMeBombs(field); // For Debug only
                choice = SelectAction();
                ChoiceProcess(field, bombs, ref gameOver, choice);
            }
            if(gameOver)
            {
                ShowMeBombs(field);
                Print(field);
                Console.WriteLine("You lose!");
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("You Win!");
            }
            Console.ReadLine();
        }
        static void CheckMineLocation(Cell[,] field, Bomb[] bombs, int x, int y)
        {
            foreach(var bomb in bombs)
            {
                if (bomb.X == x && bomb.Y == y)
                {
                    (bomb.X, bomb.Y) = GetFirstEmptyCell(field);
                    field[bomb.X, bomb.Y].State = States.Bomb;
                    field[bomb.X, bomb.Y].Symbol = "*";
                    field[x, y].State = States.Cell;
                    field[x, y].Symbol = " ";
                    field[x, y].Visible = true;
                }
            }
        }
        static void ChoiceProcess(Cell[,] field, Bomb[] bombs, ref bool gameOver, int choice)
        {
            switch (choice)
            {
                case 1:
                    OpenCell(field, bombs, ref gameOver);
                    break;
                case 2:
                    SetFlag(field);
                    break;
                case 3:
                    RemoveFlag(field);
                    break;
            }
            Console.Clear();
        }
        static (int, int) GetFirstEmptyCell(Cell[,] field)
        {
            for(int i = 0; i < field.GetLength(0); ++i)
            {
                for (int j = 0; i < field.GetLength(1); ++j)
                {
                    if (field[i, j].State != States.Bomb)
                        return (i, j);
                }
            }
            return (0, 0);
        }
        static int SelectAction()
        {
            Console.WriteLine("Select action: ");
            Console.WriteLine("1. Open Cell\n2. Set flag\n3. Remove flag");
            Console.WriteLine($"Bombs: {BOMBS_LEFT}");
            return Convert.ToInt32(Console.ReadLine());
        }
        static void SetFlag(Cell[,] field)
        {
            int x = EnterCoordinate('X');
            int y = EnterCoordinate('Y');
            if (!field[y, x].Visible)
            {
                BOMBS_LEFT--;
                field[y, x].Symbol = "!";
                field[y, x].Visible = true;
                
            }
        }
        static void SetDigits(Cell[,] field)
        {
            for (int x = 1; x < field.GetLength(0) - 1; ++x)
                for (int y = 1; y < field.GetLength(1) - 1; ++y)
                    field[x, y].SetDigit(field, x, y);

        }
        static void SetBombs(Cell[,] cells, Bomb[] bombs)
        {
            Random rand = new Random();
            Bomb bomb;
            int x, y;
            bool exist = false;

            for (int i = 0; i < BOMBS_AMOUNT; ++i)
            {
                x = rand.Next(1, FIELD_SIZE - 1);
                y = rand.Next(1, FIELD_SIZE - 1);

                bomb = new Bomb(x, y);
                foreach (Bomb b in bombs)
                {
                    if (b is null)
                        break;
                    else
                    {
                        if (b.Compair(bomb))
                        {
                            exist = true;
                            --i;
                            break;
                        }
                        else
                            exist = false;
                    }
                }
                if (!exist)
                {
                    bombs[i] = bomb;
                    cells[x, y].State = States.Bomb;
                }
            }
        }
        static void RemoveFlag(Cell[,] field)
        {
            int x = EnterCoordinate('X');
            int y = EnterCoordinate('Y');
            if (field[y,x].Symbol == "!")
            {
                field[y, x].Visible = false;
                BOMBS_LEFT++;
            }
        }
        static void OpenCell(Cell[,] field, Bomb[] bombs, ref bool gameOver)
        {
            int x = EnterCoordinate('X');
            int y = EnterCoordinate('Y');

            foreach(var bomb in bombs)
                if (x == bomb.Y && y == bomb.X)
                    gameOver = true;
            SetVisibility(field, y, x);
        }
        static int EnterCoordinate(char symbol)
        {
            Console.Write($"Please enter coordinate {symbol}: ");
            int cord = Convert.ToInt32(Console.ReadLine());
            while (!(cord < FIELD_SIZE  - 1 && cord > 0))
            {
                cord = EnterCoordinate(symbol);
            }
            return cord;
        }
        static void ShowMeBombs(Cell[,] field)
        {
            for (int i = 1; i < field.GetLength(0) - 1; ++i)
            {
                for (int j = 1; j < field.GetLength(1) - 1; ++j)
                {
                    if (field[i, j].State == States.Bomb)
                    {
                        field[i, j].Visible = true;
                        field[i, j].Symbol = "*";
                    }
                }
            }
        }
        static void SetVisibility(Cell[,] field, int inputX, int inputY)
        {
            field[inputX, inputY].Visible = true;
            if (field[inputX, inputY].GetDigit() == 0)
            {
                SCORE--;
                AutomaticOpen(field, inputX, inputY);
            }
        }
        static void AutomaticOpen(Cell[,] field, int inputX, int inputY)
        {
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    int x = inputX + i;
                    int y = inputY + j;
                    if (x > 0 && x < FIELD_SIZE && y > 0 && y < FIELD_SIZE)
                    {
                        Cell tempCell = field[x, y];
                        if (tempCell.State != States.Bomb && !tempCell.Visible)
                            SetVisibility(field, x, y);
                    }
                }
            }
        }
        static void SetupField(Cell[,] field)
        {
            for (int i = 0; i < field.GetLength(0); ++i)
            {
                for (int j = 0; j < field.GetLength(1); ++j)
                {
                    if (i == 0 || i == FIELD_SIZE - 1 || j == 0 || j == FIELD_SIZE - 1)
                        field[i, j] = new Cell(true);
                    else
                        field[i, j] = new Cell();
                }
            }
        }
        static void Print(Cell[,] field)
        {
            Console.Write("  ");
            for (int i = 0; i < FIELD_SIZE - 1; ++i)
                Console.Write(i + " ");
            Console.WriteLine();
            for (int i = 0; i < field.GetLength(0); ++i)
            {
                if (i < FIELD_SIZE - 1)
                    Console.Write(i + " ");
                else
                    Console.Write("  ");
                for (int j = 0; j < field.GetLength(1); ++j)
                {
                    int digit = field[i, j].GetDigit();
                    if (field[i, j].State == States.Wall)
                        Console.Write(field[i, j].Symbol + " ");
                    else if (field[i, j].Visible && field[i, j].State != States.Wall && digit > 0)
                    {
                        if(field[i, j].Symbol == "!")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(field[i, j].Symbol + " ");
                            Console.ResetColor();
                        }
                        else if(field[i, j].State == States.Bomb)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("* ");
                            Console.ResetColor();
                        }
                        else
                        {
                            switch (digit)
                            {
                                case 1:
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    break;
                                case 2:
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    break;
                                case 3:
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    break;
                                case 4:
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    break;
                                case 5:
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    break;
                                case 6:
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    break;
                                case 7:
                                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                                    break;
                                case 8:
                                    Console.ForegroundColor = ConsoleColor.White;
                                    break;
                            }
                            Console.Write(field[i, j].Symbol + " ");
                            Console.ResetColor();
                        }
                    }
                    else if(field[i, j].Visible && digit == 0)
                        Console.Write("  ");
                    else
                        Console.Write("# ");
                }
                Console.WriteLine();
            }
        }
    }
}
