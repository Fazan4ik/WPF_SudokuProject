using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.Net.Http.Json;
using System.Windows.Media;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;


namespace SudokuGeneration
{

    public interface SudokuGeneratorStrategy // це паттерн стратегія
    {
        int[,] GenerateGrid();
    }
    class SudokuGenerator : SudokuGeneratorStrategy // Це генерація судоку
    {
        private int BoardSize = 9;
        private const int SubgridSize = 3;
        private const int MaxIterations = 100;
        private int difficult;
        Random random = new Random();
        private int[,] board;

        public SudokuGenerator()
        {
            board = new int[BoardSize, BoardSize];
        }
        public SudokuGenerator(int sudokuDifficult)
        {
            difficult = sudokuDifficult;
            board = new int[BoardSize, BoardSize];
        }


        public int[,] GenerateGrid() // Це генерація судоку з рівнем важкості
        {
            if (difficult == 1)
            {
                FillDiagonal();
                FillRemaining(0, BoardSize);
            }
            if (difficult == 2)
            {
                FillDiagonal();
            }
            return board;
        }

        private void FillDiagonal() // Заповняємо поле по діагоналі бо так легше
        {
            for (int i = 0; i < BoardSize; i += SubgridSize)
            {
                FillSubgrid(i, i);
            }
        }

        private void FillSubgrid(int row, int col) // рандом цифр
        {
            int[] values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            Shuffle(values);

            for (int i = 0; i < SubgridSize; i++)
            {
                for (int j = 0; j < SubgridSize; j++)
                {
                    board[row + i, col + j] = values[i * SubgridSize + j];
                }
            }
        }

        private void FillRemaining(int row, int col) // Заповняємо клітинки геніальним методом
        {
            if (col >= BoardSize && row < BoardSize - 1)
            {
                row++;
                col = 0;
            }

            if (row >= BoardSize && col >= BoardSize)
            {
                return;
            }

            if (row < SubgridSize)
            {
                if (col < SubgridSize)
                {
                    col = SubgridSize;
                }
            }
            else if (row < BoardSize - SubgridSize)
            {
                if (col == (int)(row / SubgridSize) * SubgridSize)
                {
                    col += SubgridSize;
                }
            }
            else
            {
                if (col == BoardSize - SubgridSize)
                {
                    row++;
                    col = 0;
                    if (row >= BoardSize)
                    {
                        return;
                    }
                }
            }
            for (int value = 1; value <= BoardSize; value++)
            {
                if (IsValid(row, col, value))
                {
                    board[row, col] = value;
                    if (col < BoardSize - 1)
                    {
                        FillRemaining(row, col + 1);
                    }
                    else
                    {
                        FillRemaining(row + 1, 0);
                    }

                    if (row >= BoardSize || col >= BoardSize)
                    {
                        return;
                    }
                }
            }
        }

        private bool IsValid(int row, int col, int value) // перевірка заповняємості на корректність
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (board[row, i] == value || board[i, col] == value)
                {
                    return false;
                }
            }

            int subgridRow = (row / SubgridSize) * SubgridSize;
            int subgridCol = (col / SubgridSize) * SubgridSize;

            for (int i = 0; i < SubgridSize; i++)
            {
                for (int j = 0; j < SubgridSize; j++)
                {
                    if (board[subgridRow + i, subgridCol + j] == value)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        private void Shuffle<T>(T[] array) // генерація
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                T temp = array[j];
                array[j] = array[i];
                array[i] = temp;
            }
        }
    }

    public class Sudoku // Цей класс зв'язан з судоку
    {
        private int[,] grid;
        private Button[,] sudokuButtons;



        public Sudoku(SudokuGeneratorStrategy strategy, Grid sudoku) // генерація через стратегію
        {
            do
            {
                grid = strategy.GenerateGrid();
            } while (IsSolved());
            ShowSudokuGrid(sudoku);
        }

        public bool Win()
        {
            if (IsSolved())
            {
                return true;
            }
            return false;
        }

        private bool IsSolved() // перевірка на вірність судоку
        {
            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(0); col++)
                {
                    if (grid[row, col] == 0)
                    {
                        return false;
                    }
                }
            }
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                if (!IsValidRow(i))
                {
                    return false;
                }
                if (!IsValidColumn(i))
                {
                    return false;
                }
                if (grid.GetLength(0) == 9)
                {
                    if (!IsValidSquare(i / 3 * 3, i % 3 * 3))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsValidRow(int row) // Перевірка правильності рядка
        {
            bool[] usedValues = new bool[grid.GetLength(0)];

            for (int col = 0; col < grid.GetLength(0); col++)
            {
                int value = grid[row, col];

                if (value != 0)
                {
                    if (usedValues[value - 1])
                    {
                        return false;
                    }

                    usedValues[value - 1] = true;
                }
            }
            return true;
        }

        private bool IsValidColumn(int col) // Перевірка правильності стовпця
        {
            bool[] usedValues = new bool[grid.GetLength(0)];

            for (int row = 0; row < grid.GetLength(0); row++)
            {
                int value = grid[row, col];
                if (value != 0)
                {
                    if (usedValues[value - 1])
                    {
                        return false;
                    }
                    usedValues[value - 1] = true;
                }
            }
            return true;
        }

        private bool IsValidSquare(int startRow, int startCol) // Перевіряємо квадрат 3х3
        {
            bool[] usedValues = new bool[9];

            for (int row = startRow; row < startRow + 3; row++)
            {
                for (int col = startCol; col < startCol + 3; col++)
                {
                    int value = grid[row, col];

                    if (value != 0)
                    {
                        if (usedValues[value - 1])
                        {
                            return false;
                        }
                        usedValues[value - 1] = true;
                    }
                }
            }


            return true;
        }

        private bool CanPlaceNumber(int row, int col, int value) // Перевірка на можливість вставки цифри
        {
            if (value < 1 || value > grid.GetLength(0))
            {
                if (value == 0)
                {
                    if (grid[row, col] == 0)
                    {
                        return false;
                    }
                    return true;
                }

                return false;
            }
            for (int c = 0; c < grid.GetLength(0); c++)
            {
                if (grid[row, c] == value)
                {
                    return false;
                }
            }
            for (int r = 0; r < grid.GetLength(0); r++)
            {
                if (grid[r, col] == value)
                {
                    return false;
                }
            }

            int squareRow = row / 3 * 3;
            int squareCol = col / 3 * 3;
            for (int r = squareRow; r < squareRow + 3; r++)
            {
                for (int c = squareCol; c < squareCol + 3; c++)
                {
                    if (grid[r, c] == value)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void ShowSudokuGrid(Grid sudokuGrid)
        {
            sudokuGrid.Children.Clear();
            sudokuGrid.RowDefinitions.Clear();
            sudokuGrid.ColumnDefinitions.Clear();

            sudokuButtons = new Button[9, 9];
            Random random = new Random();
            double defaultMargin = 1;
            double additionalMargin = defaultMargin * 10;

            for (int i = 0; i < 9; i++)
            {
                RowDefinition row = new RowDefinition();
                sudokuGrid.RowDefinitions.Add(row);

                for (int j = 0; j < 9; j++)
                {
                    ColumnDefinition column = new ColumnDefinition();
                    sudokuGrid.ColumnDefinitions.Add(column);

                    Button button = new Button();
                    button.Name = $"Button_{i}_{j}";
                    button.Width = 50;
                    button.Height = 50;
                    button.Margin = new Thickness(defaultMargin);
                    button.Click += SudokuButton_Click;
                    button.SetValue(Grid.RowProperty, i);
                    button.SetValue(Grid.ColumnProperty, j);
                    button.Content = grid[i, j] == 0 ? "-" : grid[i, j].ToString();

                    if ((j) % 3 == 0)
                        button.Margin = new Thickness(button.Margin.Left + additionalMargin, button.Margin.Top, button.Margin.Right, button.Margin.Bottom);

                    if ((i) % 3 == 0)
                        button.Margin = new Thickness(button.Margin.Left, button.Margin.Top + additionalMargin, button.Margin.Right, button.Margin.Bottom);

                    sudokuGrid.Children.Add(button);
                    sudokuButtons[i, j] = button;
                }
            }
        }


        private void SudokuButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string buttonName = button.Name;
            int row = int.Parse(buttonName.Split('_')[1]);
            int col = int.Parse(buttonName.Split('_')[2]);

            string input = Interaction.InputBox("Введіть текст", "Зміна значення кнопки", "");
            string cleanedInput = Regex.Replace(input, "[^0-9-]", "");

            if (int.TryParse(cleanedInput, out int number))
            {
                if (CanPlaceNumber(row, col, number))
                {
                    if (number == 0)
                    {
                        button.Content = "-";
                    }
                    else
                    {
                        button.Content = number.ToString();

                    }
                    grid[row, col] = number;
                }
                else
                {
                    MessageBox.Show($"Число {cleanedInput} не підходить для вводу", "Повідомлення", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Недопустиме значення.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
