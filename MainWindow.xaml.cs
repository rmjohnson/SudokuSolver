using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public SudokuGrid sGrid = new SudokuGrid();
        public MainWindow()
        {
            InitializeComponent();
            TextBox tmpBox;
            int bottomBorder;
            int topBorder;
            int leftBorder;
            int rightBorder;
            RowDefinition row;
            ColumnDefinition col;

            for (int i = 0; i < sGrid.numRows(); i++)
            {
                row = new RowDefinition();
                row.Height = new GridLength(1, GridUnitType.Star);
                col = new ColumnDefinition();
                col.Width = new GridLength(1, GridUnitType.Star);
                SudokuGrid.RowDefinitions.Add(row);
                SudokuGrid.ColumnDefinitions.Add(col);
                for (int j = 0; j < sGrid.numCols(); j++)
                {
                    tmpBox = new TextBox();
                    tmpBox.Name = "b" + i + j;
                    tmpBox.BorderBrush = new SolidColorBrush(Colors.Black);
                    topBorder = (i % 3 == 0) ? 5 : 1;
                    bottomBorder = (i == sGrid.numRows() - 1) ? 5 : 1;
                    leftBorder = (j % 3 == 0) ? 5 : 1;
                    rightBorder = (j == sGrid.numCols() - 1) ? 5 : 1;
                    tmpBox.BorderThickness = new Thickness(leftBorder, topBorder, rightBorder, bottomBorder);
                    sGrid.setBox(tmpBox, i, j);
                    Grid.SetRow(sGrid.getBox(i, j), i);
                    Grid.SetColumn(sGrid.getBox(i, j), j);
                    SudokuGrid.Children.Add(sGrid.getBox(i, j));
                }
            }
        }

        private void example(object sender, RoutedEventArgs e)
        {
            sGrid.example();
        }
        private void reset(object sender, RoutedEventArgs e)
        {
            sGrid.reset();
        }

        private void solve(object sender, RoutedEventArgs e)
        {
            //MessageBoxResult result = MessageBox.Show(this, "DO IT YOURSELF!");
            sGrid.solve();
        }
    }

    public class PossibleNums
    {
        bool[] nums;
        int knownValue;
        public PossibleNums()
        {
            nums = new bool[] { true, true, true, true, true, true, true, true, true };
            knownValue = -1;
        }
        public void setImpossible(int num)
        {
            nums[num - 1] = false;
            if (knownValue == -1)
                setValue(findValue());
        }
        public void setPossible(int num)
        {
            nums[num - 1] = true;
        }
        public void setValue(int num)
        {
            if (num == -2) //Reset flag
            {
                knownValue = -1;
                nums = new bool[] { true, true, true, true, true, true, true, true, true };
            }
            else if (knownValue == -1)
            {
                knownValue = num;
                if (knownValue != -1)
                {
                    for (int i = 0; i < nums.Length; i++)
                    {
                        nums[i] = false;
                    }
                    nums[num - 1] = true;
                }
            }
        }
        public bool isPossible(int num)
        {
            return nums[num - 1];
        }
        public string possibles()
        {
            string possibles = "";
            for (int i = 0; i < nums.Length; i++)
            {
                if (nums[i])
                    possibles += (i + 1) + ",";
            }
            return possibles;
        }
        public int getValue()
        {
            return knownValue;
        }
        public int findValue()
        {
            int possibleValue = -1;
            for (int i = 0; i < nums.Length; i++)
            {
                if (nums[i])
                {
                    if (possibleValue == -1)
                    {
                        possibleValue = i + 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            return possibleValue;
        }
    }

    public class SudokuGrid
    {
        TextBox[,] grid;
        PossibleNums[,] possibles;
        bool isSolved;
        public SudokuGrid()
        {
            grid = new TextBox[9, 9];
            isSolved = false;
            possibles = new PossibleNums[9, 9];
            //Fill the possibles array because C# is dumb =(
            for (int i = 0; i < possibles.GetLength(0); i++)
            {
                for (int j = 0; j < possibles.GetLength(1); j++)
                {
                    possibles[i, j] = new PossibleNums();
                }
            }
        }
        public void setBox(TextBox txtbox, int i, int j)
        {
            grid[i, j] = txtbox;
            if (txtbox.Text != "")
                setValue(i,j,Convert.ToInt32(txtbox.Text));
        }
        public void checkChanges()
        {
            bool tmpSolved = true;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j].Text != "")
                    {
                        setValue(i,j,Convert.ToInt32(grid[i, j].Text));
                    }
                    else
                    {
                        tmpSolved = false;
                    }
                }
            }
            isSolved = tmpSolved;

        }
        public TextBox getBox(int i, int j)
        {
            return grid[i, j];
        }
        public int numRows()
        {
            return grid.GetLength(0);
        }
        public int numCols()
        {
            return grid.GetLength(1);
        }
        public void example()
        {
            int[,] exampleGrid = {{4,-1,-1,-1,1,-1,-1,-1,-1},
                                  {-1,-1,-1,3,-1,9,-1,4,-1},
                                  {-1,7,-1,-1,-1,5,-1,-1,9},
                                  {-1,-1,-1,-1,6,-1,-1,2,1},
                                  {-1,-1,4,-1,7,-1,6,-1,-1},
                                  {1,9,-1,-1,5,-1,-1,-1,-1},
                                  {9,-1,-1,4,-1,-1,-1,7,-1},
                                  {-1,3,-1,6,-1,8,-1,-1,-1},
                                  {-1,-1,-1,-1,3,-1,-1,-1,6}};

            for (int i = 0; i < exampleGrid.GetLength(0); i++)
            {
                for (int j = 0; j < exampleGrid.GetLength(1); j++)
                {
                    possibles[i, j].setValue(exampleGrid[i,j]);
                }
            }
            displayKnownValues();
        }
        public void reset()
        {
            for (int i = 0; i < possibles.GetLength(0); i++)
            {
                for (int j = 0; j < possibles.GetLength(1); j++)
                {
                    possibles[i,j].setValue(-2);
                }
            }
            isSolved = false;
            displayKnownValues();   
        }
        public void solve()
        {
            int tries = 0;
            while (!isSolved)
            {
                if (tries > 100)
                {
                    MessageBox.Show("IMPOSSIBLE!");
                    break;
                }
                checkChanges();
                checkOnlyPossible();
                checkColumns();
                checkRows();
                checkSquares();
                displayKnownValues();
                tries++;
            }
            /*
            checkChanges();
            checkColumns();
            checkRows();
            checkSquares();
            checkOnlyPossible();
            displayKnownValues();
             */
        }
        public void displayKnownValues()
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (possibles[i, j].getValue() != -1)
                    {
                        grid[i, j].Text = possibles[i, j].getValue().ToString();
                    }
                    else
                    {
                        grid[i, j].Text = "";
                    }
                }
            }
        }
        public void setValue(int i, int j, int value)
        {
            possibles[i, j].setValue(value);
            checkRows();
            checkColumns();
            checkSquares();
            checkOnlyPossible();
        }
        public void checkRows()
        {
            PossibleNums[] row;
            int knownValue;
            for (int i = 0; i < possibles.GetLength(0); i++)
            {
                row = new PossibleNums[possibles.GetLength(1)];
                for (int j = 0; j < possibles.GetLength(1); j++)
                {
                    row[j] = possibles[i, j];
                }
                //Now loop through the row
                for (int j = 0; j < row.Length; j++)
                {
                    if (row[j].getValue() != -1)
                    {
                        knownValue = row[j].getValue();
                        for (int k = 0; k < row.Length; k++)
                        {
                            if (k != j)
                            {
                                possibles[i, k].setImpossible(knownValue);
                            }
                        }
                    }
                }
            }
        }
        public void checkColumns()
        {
            PossibleNums[] col;
            int knownValue;
            for (int i = 0; i < possibles.GetLength(0); i++)
            {
                col = new PossibleNums[possibles.GetLength(0)];
                for (int j = 0; j < possibles.GetLength(0); j++)
                {
                    col[j] = possibles[j, i];
                }
                //Now loop through the column
                for (int j = 0; j < col.Length; j++)
                {
                    if (col[j].getValue() != -1)
                    {
                        knownValue = col[j].getValue();
                        for (int k = 0; k < col.Length; k++)
                        {
                            if (k != j)
                            {
                                possibles[k, i].setImpossible(knownValue);
                            }
                        }
                    }
                }
            }
        }
        public void checkSquares()
        {
            PossibleNums[,] square;
            int knownValue;
            for (int i = 0; i < 9; i++)
            {
                square = new PossibleNums[3, 3];
                for (int j = 0; j < square.GetLength(0); j++)
                {
                    for (int k = 0; k < square.GetLength(1); k++)
                    {
                        square[j, k] = possibles[(i / 3) * 3 + j, (i % 3) * 3 + k];
                    }
                }
                //Now loop through the square
                for (int j = 0; j < square.GetLength(0); j++)
                {
                    for (int k = 0; k < square.GetLength(1); k++)
                    {
                        if (square[j, k].getValue() != -1)
                        {
                            knownValue = square[j, k].getValue();
                            for (int l = 0; l < square.GetLength(0); l++)
                            {
                                for (int m = 0; m < square.GetLength(1); m++)
                                {
                                    if (possibles[(i / 3) * 3 + l, (i % 3) * 3 + m].getValue() == -1)
                                        possibles[(i / 3) * 3 + l, (i % 3) * 3 + m].setImpossible(knownValue);
                                }
                            }
                        }

                    }
                }
            }
        }

        public void checkOnlyPossible()
        {
            int onlyPossible;
            PossibleNums[] row;
            PossibleNums[] col;
            PossibleNums[,] square;
            for (int i = 0; i < possibles.GetLength(0); i++)
            {
                for (int j = 0; j < possibles.GetLength(1); j++)
                {
                    if (possibles[i, j].getValue() == -1)
                    {
                        //Get the row
                        row = new PossibleNums[possibles.GetLength(1)];
                        for (int k = 0; k < row.Length; k++)
                        {
                            row[k] = possibles[i, k];
                        }
                        //Check the row
                        for (int k = 1; k <= possibles.GetLength(0); k++) //Loop through possible numbers
                        {
                            if (possibles[i, j].isPossible(k) && possibles[i, j].getValue() == -1)
                            {
                                onlyPossible = k;
                                //Check row
                                //Loop through the rest of the row to check if it's possible somewhere else
                                for (int l = 0; l < possibles.GetLength(0) && onlyPossible != -1; l++)
                                {
                                    if (l != j)
                                    {
                                        if (row[l].isPossible(onlyPossible))
                                        {
                                            onlyPossible = -1;
                                        }
                                    }
                                }
                                if (onlyPossible == k)
                                {
                                    setValue(i,j,onlyPossible);
                                }
                            }

                        }
                    }
                }
            }
            for (int i = 0; i < possibles.GetLength(0); i++)
            {
                for (int j = 0; j < possibles.GetLength(1); j++)
                {
                    //Get the column
                    col = new PossibleNums[possibles.GetLength(0)];
                    for (int k = 0; k < col.Length; k++)
                    {
                        col[k] = possibles[k, j];
                    }
                    for (int k = 1; k <= possibles.GetLength(0); k++) //Loop through possible numbers
                    {
                        if (possibles[i, j].isPossible(k) && possibles[i, j].getValue() == -1)
                        {
                            onlyPossible = k;
                            //Check column
                            //Loop through the rest of the col to check if it's possible somewhere else
                            for (int l = 0; l < possibles.GetLength(0) && onlyPossible != -1; l++)
                            {
                                if (l != i)
                                {
                                    if (col[l].isPossible(onlyPossible))
                                    {
                                        onlyPossible = -1;
                                    }
                                }
                            }
                            if (onlyPossible == k)
                            {
                                setValue(i,j,onlyPossible);
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < possibles.GetLength(0); i++)
            {
                for (int j = 0; j < possibles.GetLength(1); j++)
                {
                    //Get the square
                    square = new PossibleNums[3, 3];
                    for (int k = 0; k < square.GetLength(0); k++)
                    {
                        for (int l = 0; l < square.GetLength(1); l++)
                        {
                            square[k, l] = possibles[(i / 3) * 3 + k, (j / 3) * 3 + l];
                        }
                    }
                    for (int k = 1; k <= possibles.GetLength(0); k++) //Loop through possible numbers
                    {
                        if (possibles[i, j].isPossible(k) && possibles[i, j].getValue() == -1)
                        {
                            onlyPossible = k;
                            //Check square
                            //Loop through the rest of the square to check if it's possible somewhere else
                            for (int l = 0; l < square.GetLength(0) && onlyPossible != -1; l++)
                            {
                                for (int m = 0; m < square.GetLength(1) && onlyPossible != -1; m++)
                                {
                                    if (l != i && m != j)
                                    {
                                        if (square[l, m].isPossible(onlyPossible))
                                        {
                                            onlyPossible = -1;
                                        }
                                    }
                                }
                            }
                            if (onlyPossible == k)
                            {
                                setValue(i,j,onlyPossible);
                            }
                        }
                    }
                }
            }
        }
    }
}
