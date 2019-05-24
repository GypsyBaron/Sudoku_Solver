using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Sudoku_Solver
{

    class Program
    {
        static readonly int[,] sudokuTable = new int[9, 9]; // Why readonly IDK

        static void Main(string[] args)
        {
            PrepareForSolving(); // Prepare folders, files for sudoku solving
            Console.ReadLine();
        }

        static void PrepareForSolving()
        {
            string folderName = "", fileName = "";
            Console.WriteLine("Enter folder name: ");
            folderName = Console.ReadLine();
            if (Directory.Exists(@"C: \Users\Lenovo\Desktop\Sudoku\NeedToSolve\" + folderName))
            {
                DirectoryInfo d = new DirectoryInfo(@"C: \Users\Lenovo\Desktop\Sudoku\NeedToSolve\" + folderName); // Set a directory
                FileInfo[] fileArray = d.GetFiles("*.txt"); // Getting all txt files from that directory
                for (int i = 0; i < fileArray.Length; i++)
                {
                    Console.WriteLine();
                    Console.WriteLine("Sudoku (" + (i + 1) + " from " + fileArray.Length + ")");

                    fileName = fileArray[i].Name; // Get file name
                    ReadFile(folderName, fileName); // Read sudoku from file

                    if(GoodSudoku())
                    {
                        WriteRealNumbers(); // Writing 100% real numbers by formulas

                        if (!Solved()) Solve(); // Checking if sudoku if solved or not
                        ShowSudoku(folderName, fileName); // Write solved sudoku to file
                    }
                }
            }
            else Console.WriteLine("Directory doesn't exist.");
        }

        static void ReadFile(string folderName, string fileName) // Reading from file
        {
            string temp; // Temporary string line from file 
           
            StreamReader sr = new StreamReader(@"C: \Users\Lenovo\Desktop\Sudoku\NeedToSolve\" + folderName + @"\" + fileName);
            for (int i = 0; i < 9; i++)
            {
                temp = sr.ReadLine();
                for (int j = 0; j < 9; j++)
                    sudokuTable[i, j] = Convert.ToInt32(temp[j]) - 48;
            }
        }

        static void ShowSudoku(string folderName, string fileName) // Showing sudoku matrica
        {


            if (!Directory.Exists(@"C: \Users\Lenovo\Desktop\Sudoku\Solved\" + folderName))
            {
                Directory.CreateDirectory(@"C: \Users\Lenovo\Desktop\Sudoku\Solved\" + folderName);
                Console.WriteLine("Folder was created.");
            }
                
            string fileLocation = @"C: \Users\Lenovo\Desktop\Sudoku\Solved\" + folderName + @"\Solved." + fileName;

            StreamWriter sw = new StreamWriter(fileLocation);

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                    sw.Write(sudokuTable[i, j] + " ");
                sw.WriteLine();
            }

            sw.Close();
            Console.WriteLine("Sudoku " + fileName + " from folder " +  folderName + " was succesfully solved.");
        }

        static bool GoodSudoku()
        {
            int cornerY = 0, cornerX = 0; // Upper left corner coordinates

            for (int i = 0; i < 9; i++) // Checking if row is good
                for (int j = 0; j < 9; j++)
                    for (int k = j + 1; k < 9; k++)
                        if ((sudokuTable[i, j] != 0) && (sudokuTable[i, k] != 0) && (sudokuTable[i, j] == sudokuTable[i, k]))
                        {
                            Console.WriteLine("Mistake in " + (i + 1) + " row.");
                            return false;
                        }


            for (int i = 0; i < 9; i++) // Checking if column is good
                for (int j = 0; j < 9; j++)
                    for (int k = j + 1; k < 9; k++)
                        if (sudokuTable[j, i] != 0 && sudokuTable[j, i] == sudokuTable[k, i])
                        {
                            Console.WriteLine("Mistake in " + (i + 1) + " column.");
                            return false;
                        }


            for (int i = 0; i < 9; i++) // Checking 3x3 matrica
                for (int j = 0; j < 9; j++)
                {
                    cornerY = (i / 3) * 3;
                    cornerX = (j / 3) * 3;

                    for (int k = 0; k < 3; k++)
                        for (int l = 0; l < 3; l++)
                            if (sudokuTable[i, j] != 0 && sudokuTable[i, j] == sudokuTable[cornerY + k, cornerX + l] && (i != (cornerY + k) || j != (cornerX + l)))
                            {
                                Console.WriteLine("Mistake in " + (((i / 3) * 3) + (j / 3) + 1) + " 3x3 square.");
                                return false;
                            }
                }

            return true;
        }

        static void WriteRealNumbers() // Entering real values by simply Sudoku rules
        {
            int k = 1; // Number of changes through all circle. Set 1, because while circle should works atleast one time
            int n = 0; // How many values could be assigned to this cell
            int valueToWrite  = 0; // Value that could be writen in Sudoku
            while (k != 0)
            {
                k = 0;
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++)
                    {
                        n = 0;
                        for (int l = 0; l < 9; l++)
                        {
                            if (sudokuTable[i, j] == 0)
                            {
                                if (CheckPossibleValues(i, j, l + 1)) // Checking all possible values
                                {
                                    valueToWrite = l + 1;
                                    n++;
                                    if (n > 1) break;
                                }
                            }
                            else break;
                        }
                        if (n == 1) // If only one value could be in this cell
                        {
                            sudokuTable[i, j] = valueToWrite; // Entered a value into Sudoku
                            k++; // Plus 1 change
                        }
                    }
            }
            
        } 

        static bool CheckPossibleValues(int i, int j, int l) // Check every cell possible values
        {
            int cornerY = 0, cornerX = 0; // Upper left corner coordinates

            for (int x = 0; x < 9; x++) // Checking row if there is a match with l number;
                if (sudokuTable[i, x] == l) return false;

            for (int x = 0; x < 9; x++) // Checking column if there is a match with l number;
                if (sudokuTable[x, j] == l) return false;

            cornerY = (i / 3) * 3; // Row
            cornerX = (j / 3) * 3; // Column

            for (int y = 0; y < 3; y++) // Checking 3x3 matrica
                for (int x = 0; x < 3; x++)
                    if (sudokuTable[cornerY + y, cornerX + x] == l) return false;

            return true;
        }

        static bool Solved() // Check if sudoku is solved or not
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (sudokuTable[i, j] == 0)
                        return false;
            return true;
        }  

        static void Solve() // Brute force 
        {
            Console.WriteLine("Used brute force power to solve this hard Sudoku");
            int k = HowManyZero(), n = 0;
            int[] yCoordinates = new int[k]; // Cell with zero y coordinates
            int[] xCoordinates = new int[k]; // Cell with zero x coordinates
            int[] tempValue = new int[k]; // Cell temporary value (maybe good value)

            for (int i = 0; i < 9; i++) // Assign coordinates and value to arrays
                for (int j = 0; j < 9; j++)
                {
                    if (sudokuTable[i, j] == 0)
                    {
                        yCoordinates[n] = i;
                        xCoordinates[n] = j;
                        tempValue[n] = 0;
                        n++;
                    }
                }
            
            for (int i = 0; i < k; i++)
                for (int j = 0; j < 9; j++)
                {
                    // Check if you can write here higher number what was assigned before
                    if (sudokuTable[yCoordinates[i], xCoordinates[i]] < (j + 1) && CheckPossibleValues(yCoordinates[i], xCoordinates[i], j + 1))
                    {
                        sudokuTable[yCoordinates[i], xCoordinates[i]] = j + 1;
                        break;
                    }
                    // If not, assign 0 and go back
                    else if (j == 8)
                    {
                        sudokuTable[yCoordinates[i], xCoordinates[i]] = 0; 
                        i -= 2; // Goes back by one position
                        break;
                    }
                }
        }

        static int HowManyZero() // How many zero is in Sudoku
        {
            int k = 0;
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (sudokuTable[i, j] == 0)
                        k++;
            return k;
        } 
    }
}
