using System.Text.Json;

namespace doku_solver;

/// <summary>
/// Input class of the program
/// </summary>
public class DokuSolver{
    /// <summary>
    /// Input method of the program
    /// </summary>
    public static void Main(){
        Menu.DisplayMain();
    }
}

/// <summary>
/// Class used for menu display and input
/// </summary>
static class Menu{

    /// <summary>
    /// Display main menu
    /// </summary>
    public static void DisplayMain(){
        Console.WriteLine("Welcome to the Doku Solver!");
        Console.WriteLine("1 - Solve a Sudoku Grid");
        Console.WriteLine("2 - Leave the program");
        int input = int.Parse(AwaitInput(null, "0"));
        switch (input){
            case 1:
                Console.Clear();
                DisplayFile();
                break;
            case 2:
                Environment.Exit(0);
                break;
            default:
                Console.Clear();
                Console.WriteLine("Invalid input");
                DisplayMain();
                break;
        }
    }

    /// <summary>
    /// Ask a file to the user
    /// </summary>
    private static void DisplayFile(){
        Console.WriteLine($"Please enter the path to the file (current path : {Environment.CurrentDirectory})");
        Console.WriteLine("1 - Back to main menu");
        string stringInput = AwaitInput();
        try{
            int input = int.Parse(stringInput);
            switch (input){
                case 1:
                    Console.Clear();
                    DisplayMain();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Invalid input");
                    DisplayFile();
                    break;
            }
        }
        catch (Exception e){
            Grid? grid = null;
            if (stringInput.EndsWith(".json"))
                grid = new Loader().LoadJson(stringInput);
            else if(stringInput.EndsWith(".txt"))
                grid = new Loader().LoadTxt(stringInput);
            if (grid != null){
                Console.Clear();
                DisplayAlgorithm(grid);
            }else{
                // Console.Clear();
                Console.WriteLine("Invalid file");
                DisplayFile();
            }
        }
    }
    
    /// <summary>
    /// Ask the user an algorithm to use
    /// </summary>
    /// <param name="grid">Loaded grid</param>
    private static void DisplayAlgorithm(Grid grid){
        Console.WriteLine("Please enter the algorithm you want to use");
        Console.WriteLine("1 - Backtrack");
        Console.WriteLine("2 - Slot per slot");
        Console.WriteLine("3 - Bruteforce");
        Console.WriteLine("4 - Back to main menu");
        int input = int.Parse(AwaitInput(null, "4"));
        Solver solver;
        switch (input){
            case 1:
                solver = new Algorithm.BackTrack();
                break;
            case 2:
                solver = new Algorithm.SmartBruteforce();
                break;
            case 3:
                solver = new Algorithm.BruteForce();
                break;
            default:
                return;
        }
        Console.Clear();
        int fillPercentage = int.Parse(AwaitInput("Enter the fill percentage to reach :", "100"));
        Grid solvedGrid = solver.Solve(grid, fillPercentage);
        Console.Clear();
        Console.WriteLine("Base grid :");
        DisplayGrid(grid);
        Console.WriteLine("Solved grid :");
        DisplayGrid(solvedGrid);
    }

    /// <summary>
    /// Display a grid to the console
    /// </summary>
    /// <param name="grid">Grid object</param>
    public static void DisplayGrid(Grid grid){
        grid.GetCursor().Set(0, 0);
        do{
            Console.Write(grid.GetOnCursor());
            Console.Write(" ");
            grid.GetCursor().Next();
            if(grid.GetCursor().GetColumn() == 0)
                Console.WriteLine();
        } while (grid.GetCursor().HasNext());
        Console.Write(grid.GetOnCursor());
        Console.WriteLine();
    }
    
    /// <summary>
    /// Await input from the user
    /// </summary>
    /// <param name="inputMessage">Display a message before the ReadLine</param>
    /// <param name="fallbackValue">Value if ReadLine fail</param>
    /// <returns>String printed by the user</returns>
    private static string AwaitInput(string? inputMessage = null, string fallbackValue = ""){
        if(inputMessage != null) Console.WriteLine(inputMessage);
        string input = Console.ReadLine() ?? fallbackValue;
        if (input == ""){
            input = fallbackValue;
        }
        return input;
    }
}

/// <summary>
/// Util to load a grid from a file
/// </summary>
class Loader{
    /// <summary>
    /// Load a grid from a json file
    /// </summary>
    /// <param name="fileName">File name without extension</param>
    /// <param name="path">File path</param>
    /// <returns>Grid object</returns>
    public Grid LoadJson(string fileName, string? path = "../../../grids/export/"){
        short[]? flattenedGrid = JsonSerializer.Deserialize<short[]>(File.ReadAllText($"{path}{fileName}"));
        short[,] grid = new short[(int) Math.Sqrt(flattenedGrid!.Length), (int) Math.Sqrt(flattenedGrid.Length)];
        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
                grid[i, j] = flattenedGrid[i * grid.GetLength(0) + j];
        return new Grid(grid);
    }

    /// <summary>
    /// Load a grid from a csv file
    /// </summary>
    /// <param name="fileName">File name without extension</param>
    /// <param name="path">File path</param>
    /// <returns>Grid object</returns>
    public Grid LoadCsv(string fileName, string? path = "../../../grids/"){
        return null!;
    }

    /// <summary>
    /// Load a grid from a txt file
    /// </summary>
    /// <param name="fileName">File name without extension</param>
    /// <param name="path">File path</param>
    /// <returns>Grid object</returns>
    public Grid LoadTxt(string fileName, string? path = "../../../grids/"){
        Console.WriteLine(path + fileName);
        short[] flattenArray = File.ReadAllLines(path + fileName)[0].Split(" ").Select(short.Parse).ToArray();
        int gridLength = (int) Math.Sqrt(flattenArray.Length);
        short[,] grid = new short[gridLength, gridLength];
        for (int i = 0; i < gridLength; i++)
            for (int j = 0; j < gridLength; j++)
                grid[i, j] = flattenArray[i * gridLength + j];
        return new Grid(grid);
    }
}

/// <summary>
/// Represents a two-dimensional grid of short values, along with a cursor to navigate the grid.
/// </summary>
public class Grid{

    private short[,] arrayGrid;
    private Cursor cursor;
    
    /// <summary>
    /// Constructs a new grid with the given size and initializes the cursor to the top-left position of the grid.
    /// </summary>
    /// <param name="gridSize">The size of the grid (i.e., the number of rows and columns).</param>
    public Grid(short gridSize){
        arrayGrid = new short[gridSize, gridSize];
        cursor = new Cursor(gridSize);
    }
    
    /// <summary>
    /// Constructs a new grid with the given values and initializes the cursor to the top-left position of the grid.
    /// </summary>
    /// <param name="inputGrid">The values to be placed in the grid.</param>
    public Grid(short[,] inputGrid){
        arrayGrid = CopyGrid(inputGrid);
        cursor = new Cursor((short)arrayGrid.GetLength(0));
    }
    
    /// <summary>
    /// Constructs a new grid with the same values and cursor position as the given grid.
    /// </summary>
    /// <param name="grid">The grid to be copied.</param>
    public Grid(Grid grid){
        arrayGrid = CopyGrid(grid.GetGrid());
        cursor = new Cursor((short)arrayGrid.GetLength(0));
    }

    /// <summary>
    /// Makes a copy of the given grid.
    /// </summary>
    /// <param name="oldArray">The grid to be copied.</param>
    /// <returns>A copy of the given grid.</returns>
    public short[,] CopyGrid(short[,] oldArray){
        short[,] copy = new short[oldArray.GetLength(0), oldArray.GetLength(1)];
        for(short i = 0; i < oldArray.GetLength(0); i++)
            for(short j = 0; j < oldArray.GetLength(1); j++)
                copy[i,j] = oldArray[i,j];
        return copy;
    }
    
    /// <summary>
    /// Returns the current values in the grid.
    /// </summary>
    /// <returns>A two-dimensional array representing the current values in the grid.</returns>
    public short[,] GetGrid(){
        return arrayGrid;
    }
    
    /// <summary>
    /// Returns the current cursor position.
    /// </summary>
    /// <returns>An instance of the Cursor class representing the current cursor position.</returns>
    public Cursor GetCursor(){
        return cursor;
    }

    /// <summary>
    /// Sets the value at the current cursor position.
    /// </summary>
    /// <param name="value">The value to be set at the current cursor position.</param>
    public void SetOnCursor(short value){
        arrayGrid[cursor.GetRow(), cursor.GetColumn()] = value;
    }
    
    /// <summary>
    /// Returns the value at the current cursor position.
    /// </summary>
    /// <returns>The value at the current cursor position.</returns>
    public short GetOnCursor(){
        return arrayGrid[cursor.GetRow(), cursor.GetColumn()];
    }
    
    /// <summary>
    /// Sets the value at the specified cursor position.
    /// </summary>
    /// <param name="_cursor">The cursor position at which the value will be set.</param>
    /// <param name="value">The value to be set at the specified cursor position.</param>
    public void SetOnCursor(Cursor _cursor, short value){
        arrayGrid[_cursor.GetRow(), _cursor.GetColumn()] = value;
    }
    
    /// <summary>
    /// Returns the value at the specified cursor position.
    /// </summary>
    /// <param name="_cursor">The cursor position to be accessed.</param>
    /// <returns>The value at the specified cursor position.</returns>
    public short GetOnCursor(Cursor _cursor){
        return arrayGrid[_cursor.GetRow(), _cursor.GetColumn()];
    }
    
    /// <summary>
    /// Sets the value at the specified position in the grid.
    /// </summary>
    /// <param name="row">The row of the position to be accessed.</param>
    /// <param name="column">The column of the position to be accessed.</param>
    /// <param name="value">The value to be set at the specified position.</param>
    public void SetOnPosition(short row, short column, short value){
        arrayGrid[row, column] = value;
    }
    
    /// <summary>
    /// Returns the value at the specified position in the grid.
    /// </summary>
    /// <param name="row">The row of the position to be accessed.</param>
    /// <param name="column">The column of the position to be accessed.</param>
    /// <returns>The value at the specified position in the grid.</returns>
    public short GetOnPosition(short row, short column){
        return arrayGrid[row, column];
    }

    /// <summary>
    /// Returns the size of the grid (i.e., the number of rows and columns).
    /// </summary>
    /// <returns>The size of the grid.</returns>
    public short GetLength(){
        return (short)arrayGrid.GetLength(0);
    }

    public int GetFillPercentage(){
        int filledCount = 0;
        for (int i = 0; i < arrayGrid.GetLength(0); i++)
            for (int j = 0; j < arrayGrid.GetLength(1); j++)
                if (arrayGrid[i, j] != 0)
                    filledCount++;
        return (int)((double)filledCount / (arrayGrid.GetLength(0) * arrayGrid.GetLength(1)) * 100);
    }
}

/// <summary>
/// Represents a cursor for navigating a two-dimensional grid of short values.
/// </summary>
public class Cursor{
    private short row;
    private short column;
    private short gridSize;
    
    /// <summary>
    /// Constructs a new cursor with the given position and grid size.
    /// </summary>
    /// <param name="row">The row of the cursor position.</param>
    /// <param name="column">The column of the cursor position.</param>
    /// <param name="gridSize">The size of the grid (i.e., the number of rows and columns).</param>
    public Cursor(short row, short column, short gridSize){
        this.row = row;
        this.column = column;
        this.gridSize = gridSize;
    }

    /// <summary>
    /// Constructs a new cursor with the same position and grid size as the given cursor.
    /// </summary>
    /// <param name="cursor">The cursor to be copied.</param>
    public Cursor(Cursor cursor){
        row = cursor.GetRow();
        column = cursor.GetColumn();
        gridSize = cursor.GetGridSize();
    }

    private short GetGridSize(){
        return gridSize;
    }

    /// <summary>
    /// Constructs a new cursor with the given grid size and initializes the position to the top-left corner of the grid.
    /// </summary>
    /// <param name="gridSize">The size of the grid (i.e., the number of rows and columns).</param>
    public Cursor(short gridSize){
        row = 0;
        column = 0;
        this.gridSize = gridSize;
    }
    
    /// <summary>
    /// Returns the row of the current cursor position.
    /// </summary>
    /// <returns>The row of the current cursor position.</returns>
    public short GetRow(){
        return row;
    }
    
    /// <summary>
    /// Returns the column of the current cursor position.
    /// </summary>
    /// <returns>The column of the current cursor position.</returns>
    public short GetColumn(){
        return column;
    }
    
    /// <summary>
    /// Sets the row of the current cursor position.
    /// </summary>
    /// <param name="posX">The new row of the cursor position.</param>
    public void SetRow(short posX){
        row = posX;
    }
    
    /// <summary>
    /// Sets the column of the current cursor position.
    /// </summary>
    /// <param name="posY">The new column of the cursor position.</param>
    public void SetColumn(short posY){
        column = posY;
    }

    /// <summary>
    /// Sets the row and column of the current cursor position.
    /// </summary>
    /// <param name="posX">The new row of the cursor position.</param>
    /// <param name="posY">The new column of the cursor position.</param>
    public void Set(short posX, short posY){
        row = posX;
        column = posY;
    }

    /// <summary>
    /// Moves the cursor to the next position in the grid, wrapping around to the first column of the next row if necessary.
    /// </summary>
    /// <returns>The updated cursor position.</returns>
    public Cursor Next(){
        column++;
        if (column != gridSize) return this;
        row++;
        column = 0;
        return this;
    }

    /// <summary>
    /// Moves the cursor to the previous position in the grid, wrapping around to the last column of the previous row if necessary.
    /// </summary>
    /// <returns>The updated cursor position.</returns>
    public Cursor Previous(){
        column--;
        if (column != -1) return this;
        column = (short) (gridSize - 1);
        row--;
        return this;
    }
    
    /// <summary>
    /// Determines whether the cursor has a position after the current one in the grid.
    /// </summary>
    /// <returns>True if the cursor has a position after the current one in the grid, false otherwise.</returns>
    public bool HasNext(){
        return row != gridSize - 1 || column != gridSize - 1;
    }
    
    /// <summary>
    /// Determines whether the cursor has a position before the current one in the grid.
    /// </summary>
    /// <returns>True if the cursor has a position before the current one in the grid, false otherwise.</returns>
    public bool HasPrevious(){
        return row != 0 || column != 0;
    }
    
    public bool IsOutOfBounds(){
        return row < 0 || row >= gridSize || column < 0 || column >= gridSize;
    }
}

/// <summary>
/// Contains various algorithms for solving Sudoku puzzles.
/// </summary>
public static class Algorithm{
    /// <summary>
    /// A backtracking algorithm for solving Sudoku puzzles.
    /// </summary>
    public class BackTrack : Solver{
        /// <summary>
        /// Solves the given Sudoku puzzle using a backtracking algorithm.
        /// </summary>
        /// <param name="grid">The Sudoku puzzle to be solved.</param>
        /// <param name="fillPercentage">The fill percentage to reach before stopping</param>
        /// <returns>The solved Sudoku puzzle.</returns>
        public override Grid Solve(Grid grid, int fillPercentage){
            Grid solvedGrid = new Grid(grid);
            return Backtrack(solvedGrid, 0, 0, fillPercentage)!;
        }
        
        /// <summary>
        /// Recursive helper function for the backtracking algorithm.
        /// </summary>
        /// <param name="grid">The Sudoku puzzle being solved.</param>
        /// <param name="row">The current row of the cursor position.</param>
        /// <param name="column">The current column of the cursor position.</param>
        /// <param name="fillPercentage">The fill percentage to reach before stopping.</param>
        /// <returns>The solved Sudoku puzzle, or null if the puzzle has not been solved yet.</returns>
        private Grid? Backtrack(Grid grid, short row, short column, int fillPercentage){
            Cursor cursor = new Cursor((short) grid.GetGrid().GetLength(0));
            cursor.Set(row, column);

            if (cursor.IsOutOfBounds()) return grid;

            if (grid.GetFillPercentage() >= fillPercentage) return grid;
            
            if (grid.GetOnPosition(row, column) != 0){
                Cursor nextCursor = new Cursor(cursor).Next();
                return Backtrack(grid, nextCursor.GetRow(), nextCursor.GetColumn(), fillPercentage);
            }

            for (short i = 1; i <= grid.GetLength(); i++){
                if (!IsPresentForSlot(grid, row, column, i)){
                    grid.SetOnPosition(row, column, i);
                    Cursor nextCursor = new Cursor(cursor).Next();
                    Grid? solvedGrid = Backtrack(grid, nextCursor.GetRow(), nextCursor.GetColumn(), fillPercentage);
                    if (solvedGrid != null) return solvedGrid;
                    grid.SetOnPosition(row, column, 0);
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the given value is present in the same row, column, or box as the given position in the grid.
        /// </summary>
        /// <param name="grid">The Sudoku puzzle being solved.</param>
        /// <param name="row">The row of the given position.</param>
        /// <param name="column">The column of the given position.</param>
        /// <param name="value">The value to be checked.</param>
        /// <returns>True if the value is present in the same row, column, or box as the given position, false otherwise.</returns>
        private bool IsPresentForSlot(Grid grid, short row, short column, int value){
            for(int i = 0; i < grid.GetLength(); i++){
                if (grid.GetGrid()[row, i] == value) return true;
                if (grid.GetGrid()[i, column] == value) return true;
            }
            int sqrt = (int)Math.Sqrt(grid.GetLength());
            int boxRowStart = row - row % sqrt;
            int boxColStart = column - column % sqrt;
            for (int r = boxRowStart; r < boxRowStart + sqrt; r++)
                for (int c = boxColStart; c < boxColStart + sqrt; c++)
                    if (grid.GetGrid()[r, c] == value) return true;
            return false;
        }
    }
    
    /// <summary>
    /// A brute force algorithm for solving Sudoku puzzles.
    /// </summary>
    public class SmartBruteforce : Solver {
        /// <summary>
        /// Solves the given Sudoku puzzle using a slot per slot algorithm.
        /// </summary>
        /// <param name="grid">The Sudoku puzzle to be solved.</param>
        /// <param name="fillPercentage">The fill percentage to reach before stopping</param>
        /// <returns>The solved Sudoku puzzle.</returns>
        public override Grid Solve(Grid grid, int fillPercentage) {
            Grid solvedGrid = new Grid(grid);
            List<short> possibilities = new List<short>();
            short[ , ] tab = solvedGrid.GetGrid();
            do {
                if (solvedGrid.GetFillPercentage() >= fillPercentage) return solvedGrid;
                for (int i = 0; i < solvedGrid.GetLength(); i++) {
                    for (int j = 0; j < solvedGrid.GetLength(); j++) {
                        if (tab[ i, j ] != 0)
                            continue;
                        possibilities.Clear();
                        for (short r = 1; r <= solvedGrid.GetLength(); r++)
                            if (IsValidPlacement(tab, r, i, j))
                                possibilities.Add(r);
                        if (possibilities.Count == 1)
                            tab[ i, j ] = possibilities[ 0 ];
                    }
                }
            } while (!IsSolved(solvedGrid));
            return solvedGrid;
        }
    }

    public class BruteForce : Solver {
        public override Grid Solve(Grid grid, int fillPercentage) {
            Grid solvedGrid = new Grid(grid);
            Random random = new Random();
            short[ , ] tab = solvedGrid.GetGrid();
            do {
                for (int i = 0; i < solvedGrid.GetLength(); i++)
                    for (int j = 0; j < solvedGrid.GetLength(); j++)
                        if (tab[ i, j ] == 0)
                            tab[ i, j ] = (short) random.Next(1, solvedGrid.GetLength() + 1);
            } while (!IsSolved(solvedGrid));
        
            return solvedGrid;
        }
    }
}

/// <summary>
/// Abstract base class for Sudoku solver algorithms.
/// </summary>
public abstract class Solver : Doku{
    /// <summary>
    /// Solves the given Sudoku puzzle using a specific algorithm.
    /// </summary>
    /// <param name="grid">The Sudoku puzzle to be solved.</param>
    /// <param name="fillPercentage">The fill percentage the algorithm need to reach</param>
    /// <returns>The solved Sudoku puzzle.</returns>
    public abstract Grid Solve(Grid grid, int fillPercentage);
    
    /// <summary>
    /// Determines whether the given number is present in the specified row of the given Sudoku puzzle.
    /// </summary>
    /// <param name="grid">The Sudoku puzzle being checked.</param>
    /// <param name="number">The number to be checked for.</param>
    /// <param name="row">The row to be checked.</param>
    /// <returns>True if the number is present in the specified row, false otherwise.</returns>
    protected bool IsInRow(short[,] grid, int number, int row) {
        for (int i = 0; i < grid.GetLength(0); i++)
            if (grid[row, i] == number)
                return true;
        return false;
    }

    /// <summary>
    /// Determines whether the given number is present in the specified column of the given Sudoku puzzle.
    /// </summary>
    /// <param name="grid">The Sudoku puzzle being checked.</param>
    /// <param name="number">The number to be checked for.</param>
    /// <param name="column">The column to be checked.</param>
    /// <returns>True if the number is present in the specified column, false otherwise.</returns>
    protected bool IsInColumn(short[,] grid, int number, int column) {
        for (int i = 0; i < grid.GetLength(0); i++)
            if (grid[i, column] == number)
                return true;
        return false;
    }

    /// <summary>
    /// Determines whether the given number is present in the same row, column, or box as the specified position in the given Sudoku puzzle.
    /// </summary>
    /// <param name="grid">The Sudoku puzzle being checked.</param>
    /// <param name="number">The number to be checked for.</param>
    /// <param name="row">The row of the specified position.</param>
    /// <param name="column">The column of the specified position.</param>
    /// <returns>True if the number is present in the same row, column, or box as the specified position, false otherwise.</returns>
    protected bool IsInSection(short[,] grid, int number, int row, int column) {
        int boxSize = (int) Math.Sqrt(grid.GetLength(0));
        int localRow = row - row % boxSize;
        int localColumn = column - column % boxSize;
        
        for (int i = localRow; i < localRow + boxSize; i++)
        for (int j = localColumn; j < localColumn + boxSize; j++)
            if (grid[i, j] == number)
                return true;
        return false;
    }

    /// <summary>
    /// Determines whether the given number can be placed at the specified position in the given Sudoku puzzle.
    /// </summary>
    /// <param name="grid">The Sudoku puzzle being checked.</param>
    /// <param name="number">The number to be placed.</param>
    /// <param name="row">The row of the specified position.</param>
    /// <param name="column">The column of the specified position.</param>
    /// <returns>True if the number can be placed at the specified position, false otherwise.</returns>
    protected bool IsValidPlacement(short[,] grid, int number, int row, int column) {
        return !IsInRow(grid, number, row) &&
               !IsInColumn(grid, number, column) &&
               !IsInSection(grid, number, row, column);
    }
}

public class Doku{
    /// <summary>
    /// Makes a copy of the given 2D array.
    /// </summary>
    /// <param name="tab">The 2D array to be copied.</param>
    /// <returns>A copy of the given 2D array.</returns>
    protected int[,] Copy(int[,] tab){
        int[,] nTab = new int[tab.GetLength(0), tab.GetLength(1)];
        for (int i = 0; i < tab.GetLength(0); i++)
            for (int j = 0; j < tab.GetLength(1); j++)
                nTab[i, j] = tab[i, j];
        return nTab;
    }

    /// <summary>
    /// Determines whether the given Sudoku puzzle is filled (i.e. all of its positions are filled with numbers).
    /// </summary>
    /// <param name="tab">The Sudoku puzzle being checked.</param>
    /// <returns>True if the puzzle is filled, false otherwise.</returns>
    protected bool IsFilled(short[,] tab){
        bool isFilled = true;
        for (int i = 0; i < tab.GetLength(0) && isFilled; i++)
            for (int j = 0; j < tab.GetLength(1) && isFilled; j++)
                if(tab[i, j] == 0) isFilled = false;
                // if (GetSlotPossibilities(tab, i, j).Count != 0) isSolved = false;
        return isFilled;
    }
    
    /// <summary>
    /// Determines whether the given Sudoku puzzle is filled (i.e. all of its positions are filled with numbers).
    /// </summary>
    /// <param name="grid">The Sudoku puzzle being checked.</param>
    /// <returns>True if the puzzle is filled, false otherwise.</returns>
    protected bool IsFilled(Grid grid){
        return IsFilled(grid.GetGrid());
    }

    /// <summary>
    /// Determines whether the given Sudoku puzzle is solved (i.e. all of its positions are filled with numbers and the puzzle is valid).
    /// </summary>
    /// <param name="tab">The Sudoku puzzle being checked.</param>
    /// <returns>True if the puzzle is solved, false otherwise.</returns>
    protected bool IsSolved(short[ , ] tab) {
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
    /// Determines whether the given Sudoku puzzle is solved (i.e. all of its positions are filled with numbers and the puzzle is valid).
    /// </summary>
    /// <param name="grid">The Sudoku puzzle being checked.</param>
    /// <returns>True if the puzzle is solved, false otherwise.</returns>
    protected bool IsSolved(Grid grid) {
        return IsSolved(grid.GetGrid());
    }
    
    private readonly List<short> _rowPossibilities = new();
    private readonly List<short> _columnPossibilities = new();
    private readonly List<short> _sectionPossibilities = new();
    private readonly List<short> _possibilities = new();
    
    /// <summary>
    /// Gets a list of the numbers that can be placed in the given position of the given Sudoku puzzle.
    /// </summary>
    /// <param name="tab">The Sudoku puzzle being checked.</param>
    /// <param name="row">The row of the specified position.</param>
    /// <param name="column">The column of the specified position.</param>
    /// <returns>A list of numbers that can be placed in the specified position.</returns>
    protected List<short> GetSlotPossibilities(short[,] tab, int row, int column){
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
    /// Gets a list of the numbers that can be placed in the given position of the given Sudoku puzzle.
    /// </summary>
    /// <param name="grid">The Sudoku puzzle being checked.</param>
    /// <param name="row">The row of the specified position.</param>
    /// <param name="column">The column of the specified position.</param>
    /// <returns>A list of numbers that can be placed in the specified position.</returns>
    protected List<short> GetSlotPossibilities(Grid grid, short row, short column){
        return GetSlotPossibilities(grid.GetGrid(), row, column);
    }
    
    /// <summary>
    /// Gets a list of the numbers that are not present in the given column of the given Sudoku puzzle.
    /// </summary>
    /// <param name="tab">The Sudoku puzzle being checked.</param>
    /// <param name="column">The column being checked.</param>
    private void GetColumnPossibilities(short[,] tab, int column){
        _columnPossibilities.Clear();
        for(short i = 1; i <= tab.GetLength(0); i++)
            _columnPossibilities.Add(i);
        for (int i = 0; i < tab.GetLength(0); i++)
            if(_columnPossibilities.Contains(tab[i, column]))
                _columnPossibilities.Remove(tab[i, column]);
    }
    
    /// <summary>
    /// Gets a list of the numbers that are not present in the given row of the given Sudoku puzzle.
    /// </summary>
    /// <param name="tab">The Sudoku puzzle being checked.</param>
    /// <param name="row">The row being checked.</param>
    private void GetRowPossibilities(short[,] tab, int row){
        _rowPossibilities.Clear();
        for(short i = 1; i <= tab.GetLength(1); i++)
            _rowPossibilities.Add(i);
        for (int i = 0; i < tab.GetLength(1); i++)
            if(_rowPossibilities.Contains(tab[row, i]))
                _rowPossibilities.Remove(tab[row, i]);
    }
    
    /// <summary>
    /// Gets a list of the numbers that are not present in the same section as the given position in the given Sudoku puzzle.
    /// </summary>
    /// <param name="tab">The Sudoku puzzle being checked.</param>
    /// <param name="row">The row of the specified position.</param>
    /// <param name="column">The column of the specified position.</param>
    private void GetSectionPossibilities(short[,] tab, int row, int column){
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
}

/// <summary>
/// A class for timing actions.
/// </summary>
public class DokuTimer{
    
    private double startTime;
    private double endTime;
    
    /// <summary>
    /// Starts the timer.
    /// </summary>
    public void Start(){
        startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() * 1D / 1000;
    }

    /// <summary>
    /// Stops the timer.
    /// </summary>
    public void Stop(){
        endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() * 1D / 1000;
    }

    /// <summary>
    /// Gets the elapsed time between the start and end times of the timer.
    /// </summary>
    /// <returns>The elapsed time, in seconds, between the start and end times of the timer.</returns>
    public double GetResult(){
        return Math.Round(endTime - startTime, 3);
    }
}