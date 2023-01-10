using System.Text;

namespace doku_solver;

public class V2{
    
    private static string PATH = "grids";
    
    public static void Main(){
        DisplayMenu();
    }

    /// <summary>
    /// Displays the main menu, allowing the user to select a grid file and an algorithm to solve it.
    /// </summary>
    public static void DisplayMenu(){
        Dictionary<int, string> files = DisplayAvailableGrids();
        string input = AwaitInput("Welcome to the Doku Solver! Select grid file to solve :", "");
        int fileIndex;
        short[,] grid;
        if (int.TryParse(input, out fileIndex))
            grid = LoadTxt(files[fileIndex]);
        else
            grid = LoadTxt(input);
        DisplayAlgorithmChoice(grid);
    }

    /// <summary>
    /// Displays the algorithm choice menu and starts the selected algorithm to solve the grid.
    /// </summary>
    /// <param name="grid">The grid to be solved</param>
    public static void DisplayAlgorithmChoice(short[,] grid){
        Console.Clear();
        Console.WriteLine("Choose an algorithm :");
        Console.WriteLine("1. Brute Force");
        Console.WriteLine("2. Smart Brute Force");
        string input = AwaitInput("", "");
        int choice;
        if(!int.TryParse(input, out choice)){
            Console.WriteLine("Invalid choice");
            DisplayAlgorithmChoice(grid);
        }else{
            Console.Clear();
            bool breaked = false;
            DisplayGrid(grid);
            Console.WriteLine("--------------------");
            if (choice == 1){
                SolveBruteforce(grid, 100);
            }else if (choice == 2){
                SolveSmartBruteforce(grid, 100);
            }else{
                Console.WriteLine("Invalid choice");
                breaked = true;
                DisplayAlgorithmChoice(grid);
            }
            if(!breaked)
                DisplayGrid(grid);
        }
    }

    /// <summary>
    /// Displays the grid on the console.
    /// </summary>
    /// <param name="grid">The grid to be displayed</param>
    public static void DisplayGrid(short[,] grid){
        for (int i = 0; i < grid.GetLength(0); i++){
            for(int j = 0; j < grid.GetLength(1); j++){
                Console.Write(grid[i, j] + " ");
            }
            Console.WriteLine();
        }
    }
    
    /// <summary>
    /// Awaits user input and returns the input as a string.
    /// If the input is empty and a fallback value is provided, the fallback value is returned instead.
    /// </summary>
    /// <param name="inputMessage">The message to display to the user when prompting for input.</param>
    /// <param name="fallbackValue">The fallback value to return if the user input is empty.</param>
    /// <returns>The user's input, or the fallback value if the input is empty and a fallback value is provided.</returns>
    private static string AwaitInput(string inputMessage, string fallbackValue){
        if(inputMessage != "") Console.WriteLine(inputMessage);
        string input = Console.ReadLine();
        if (fallbackValue != "" && input == "")
            input = fallbackValue;
        return input;
    }

    /// <summary>
    /// Displays the available grids and returns a dictionary containing the grid number and its name.
    /// </summary>
    /// <returns>Dictionary containing the grid number and its name.</returns>
    private static Dictionary<int, string> DisplayAvailableGrids(){
        Dictionary<int, string> availableFiles = new Dictionary<int, string>();
        List<string> files = Directory.EnumerateFiles(PATH).ToList();
        for(int i = 0; i < files.Count; i++){
            files[i] = files[i].Replace(PATH + "\\", "");
            Console.WriteLine(i + 1 + " : " + files[i]);
            availableFiles.Add(i + 1, files[i]);
        }
        return availableFiles;
    }
    
    /// <summary>
    /// Loads a grid from a text file and returns it as a 2D short array.
    /// The grid should have square dimensions and be space-separated.
    /// </summary>
    /// <param name="fileName">The name of the file containing the grid.</param>
    /// <param name="path">The path to the file, defaults to "../../../grids/".</param>
    /// <returns>A 2D short array representing the grid from the input file.</returns>
    public static short[,] LoadTxt(string fileName){
        if (!Directory.Exists(PATH))
            Directory.CreateDirectory(PATH);
        string[] elements = SplitString(File.ReadAllText(PATH + "/" + fileName), ' ');
        short[] flattenArray = new short[elements.Length];
        for (int i = 0; i < elements.Length; i++)
            flattenArray[i] = short.Parse(elements[i]);
        int gridLength = (int) Math.Sqrt(flattenArray.Length);
        short[,] grid = new short[gridLength, gridLength];
        for (int i = 0; i < gridLength; i++)
            for (int j = 0; j < gridLength; j++)
                grid[i, j] = flattenArray[i * gridLength + j];
        return grid;
    }

    /// <summary>
    /// Splits an input string into an array of strings using a specified separator.
    /// </summary>
    /// <param name="input">The input string to be split.</param>
    /// <param name="separator">The separator to use when splitting the input string.</param>
    /// <returns>An array of strings resulting from splitting the input string using the specified separator.</returns>
    public static string[] SplitString(string input, char separator){
        List<string> elements = new List<string>();
        int startIndex = 0;
        for (int i = 0; i < input.Length; i++){
            if (input[i] == separator){
                elements.Add(SubString(input, startIndex, i - 1));
                startIndex = i + 1;
            }else if (i == input.Length - 1)
                elements.Add(SubString(input, startIndex, i));
        }
        string[] output = new string[elements.Count];
        for(int i = 0; i < elements.Count; i++)
            output[i] = elements[i];
        return output;
    }

    /// <summary>
    /// Returns a substring of the input string between two specified indices.
    /// </summary>
    /// <param name="input">The input string from which to extract the substring.</param>
    /// <param name="start">The starting index of the substring.</param>
    /// <param name="end">The ending index of the substring.</param>
    /// <returns>The substring of the input string between the specified indices.</returns>
    private static string SubString(string input, int start, int end){
        string output = "";
        for (int i = start; i <= end; i++)
            output += input[i];
        return output;
    }
    
    /// <summary>
    /// Solves a sudoku-like grid using a smart brute-force algorithm.
    /// </summary>
    /// <param name="grid">The grid to be solved, represented as a 2D short array.</param>
    /// <param name="fillPercentage">The fill percentage of the grid (e.g. 30 for 30%).</param>
    private static void SolveSmartBruteforce(short[,] grid, int fillPercentage) {
        // TODO
        List<short> possibilities = new List<short>();
        do{
            if (GetFillPercentage(grid) <= fillPercentage){
                for (int i = 0; i < grid.GetLength(0); i++) {
                    for (int j = 0; j < grid.GetLength(0); j++) {
                        if (grid[ i, j ] != 0)
                            continue;
                        possibilities.Clear();
                        for (short r = 1; r <= grid.GetLength(0); r++)
                            if (IsValidPlacement(grid, r, i, j))
                                possibilities.Add(r);
                        if (possibilities.Count == 1)
                            grid[ i, j ] = possibilities[ 0 ];
                    }
                }
            }
        } while (!IsSolved(grid));
    }
    
    /// <summary>
    /// Solves a sudoku-like grid using a brute-force algorithm.
    /// </summary>
    /// <param name="grid">The grid to be solved, represented as a 2D short array.</param>
    /// <param name="fillPercentage">The fill percentage of the grid (e.g. 30 for 30%).</param>
    private static void SolveBruteforce(short[,] grid, int fillPercentage) {
        // TODO
        Random random = new Random();
        int gridLength = grid.GetLength(0);
        do {
            for (int i = 0; i < gridLength; i++)
            for (int j = 0; j < gridLength; j++)
                if (grid[ i, j ] == 0)
                    grid[ i, j ] = (short) random.Next(1, gridLength + 1);
        } while (!IsSolved(grid));
    }
    
    /// <summary>
    /// Check if the grid is solved, by checking if all the slots have value and
    /// there is no contradiction of values
    /// </summary>
    /// <param name="tab">grid to check</param>
    /// <returns>true if the grid is solved, else false</returns>
    private static bool IsSolved(short[ , ] tab) {
        bool isSolved = true;
        for (int i = 0; i < tab.GetLength(0) && isSolved; i++){
            for (int j = 0; j < tab.GetLength(1) && isSolved; j++){
                if(tab[i, j] == 0) isSolved = false;
                if (GetSlotPossibilities(tab, i, j).Count != 0) isSolved = false;
            }
        }
        return isSolved;
    }

    /// <summary>
    /// Get the fill percentage of the grid
    /// </summary>
    /// <param name="grid">grid to check</param>
    /// <returns>fill percentage of the grid</returns>
    private static int GetFillPercentage(short[,] grid){
        int gridLength = grid.GetLength(0);
        int filledSlots = 0;
        for (int i = 0; i < gridLength; i++)
            for (int j = 0; j < gridLength; j++)
                if (grid[ i, j ] != 0)
                    filledSlots++;
        return (int) ((double) filledSlots / (gridLength * gridLength) * 100);
    }
    
    private static List<short> _rowPossibilities = new List<short>();
    private static List<short> _columnPossibilities = new List<short>();
    private static List<short> _sectionPossibilities = new List<short>();
    private static List<short> _possibilities = new List<short>();
    
    /// <summary>
    /// Get the possible numbers for a specific slot in a sudoku puzzle
    /// </summary>
    /// <param name="tab">The sudoku puzzle</param>
    /// <param name="row">The row of the slot</param>
    /// <param name="column">The column of the slot</param>
    /// <returns>A list of possible numbers</returns>
    private static List<short> GetSlotPossibilities(short[,] tab, int row, int column){
        _possibilities.Clear();
        GetRowPossibilities(tab, row);
        GetColumnPossibilities(tab, column);
        GetSectionPossibilities(tab, row, column);
        foreach (short val in _rowPossibilities)
            if(_columnPossibilities.Contains(val) && _sectionPossibilities.Contains(val))
                _possibilities.Add(val);
        return _possibilities;
    }
    
    /// <summary>
    /// Get the possible numbers for a specific column in a sudoku puzzle
    /// </summary>
    /// <param name="tab">The sudoku puzzle</param>
    /// <param name="column">The column of the slot</param>
    private static void GetColumnPossibilities(short[,] tab, int column){
        _columnPossibilities.Clear();
        for(short i = 1; i <= tab.GetLength(0); i++)
            _columnPossibilities.Add(i);
        for (int i = 0; i < tab.GetLength(0); i++)
            if(_columnPossibilities.Contains(tab[i, column]))
                _columnPossibilities.Remove(tab[i, column]);
    }
    
    /// <summary>
    /// Get the possible numbers for a specific row in a sudoku puzzle
    /// </summary>
    /// <param name="tab">The sudoku puzzle</param>
    /// <param name="row">The row of the slot</param>
    private static void GetRowPossibilities(short[,] tab, int row){
        _rowPossibilities.Clear();
        for(short i = 1; i <= tab.GetLength(1); i++)
            _rowPossibilities.Add(i);
        for (int i = 0; i < tab.GetLength(1); i++)
            if(_rowPossibilities.Contains(tab[row, i]))
                _rowPossibilities.Remove(tab[row, i]);
    }
    
    /// <summary>
    /// Get the possible numbers for a specific section in a sudoku puzzle
    /// </summary>
    /// <param name="tab">The sudoku puzzle</param>
    /// <param name="row">The row of the slot</param>
    /// <param name="column">The column of the slot</param>
    private static void GetSectionPossibilities(short[,] tab, int row, int column){
        _sectionPossibilities.Clear();
        for(short i = 1; i <= tab.GetLength(1); i++)
            _sectionPossibilities.Add(i);
        int squareSize = (int)Math.Sqrt(tab.GetLength(0));
        int squareRow = row / squareSize;
        int squareColumn = column / squareSize;
        for (int i = squareRow * squareSize; i < squareRow * squareSize + squareSize; i++)
        for (int j = squareColumn * squareSize; j < squareColumn * squareSize + squareSize; j++)
            if(_sectionPossibilities.Contains(tab[i, j]))
                _sectionPossibilities.Remove(tab[i, j]);
    }
    
    /// <summary>
    /// Check if a given number can be placed in a specific slot on the sudoku puzzle
    /// </summary>
    /// <param name="grid">The sudoku puzzle</param>
    /// <param name="number">The number to check</param>
    /// <param name="row">The row of the slot</param>
    /// <param name="column">The column of the slot</param>
    /// <returns>True if the number can be placed in the slot, otherwise false</returns>
    private static bool IsValidPlacement(short[,] grid, int number, int row, int column) {
        return !IsInRow(grid, number, row) &&
               !IsInColumn(grid, number, column) &&
               !IsInSection(grid, number, row, column);
    }
    
    /// <summary>
    /// Check if a given number is already in the specific row of a sudoku puzzle
    /// </summary>
    /// <param name="grid">The sudoku puzzle</param>
    /// <param name="number">The number to check</param>
    /// <param name="row">The row of the slot</param>
    /// <returns>True if the number is already in the row, otherwise false</returns>
    private static bool IsInRow(short[,] grid, int number, int row) {
        for (int i = 0; i < grid.GetLength(0); i++)
            if (grid[row, i] == number)
                return true;
        return false;
    }
    
    /// <summary>
    /// Check if a given number is already in the specific column of a sudoku puzzle
    /// </summary>
    /// <param name="grid">The sudoku puzzle</param>
    /// <param name="number">The number to check</param>
    /// <param name="column">The column of the slot</param>
    /// <returns>True if the number is already in the column, otherwise false</returns>
    private static bool IsInColumn(short[,] grid, int number, int column) {
        for (int i = 0; i < grid.GetLength(0); i++)
            if (grid[i, column] == number)
                return true;
        return false;
    }
    
    /// <summary>
    /// Check if a given number is already in the specific section of a sudoku puzzle
    /// </summary>
    /// <param name="grid">The sudoku puzzle</param>
    /// <param name="number">The number to check</param>
    /// <param name="row">The row of the slot</param>
    /// <param name="column">The column of the slot</param>
    /// <returns>True if the number is already in the section, otherwise false</returns>
    private static bool IsInSection(short[,] grid, int number, int row, int column) {
        int boxSize = (int) Math.Sqrt(grid.GetLength(0));
        int localRow = row - row % boxSize;
        int localColumn = column - column % boxSize;
        
        for (int i = localRow; i < localRow + boxSize; i++)
        for (int j = localColumn; j < localColumn + boxSize; j++)
            if (grid[i, j] == number)
                return true;
        return false;
    }
}