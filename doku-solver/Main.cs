using System.Text.Json;

namespace doku_solver;

public class DokuSolver{
    public static void Main(){
        new Menu();
    }
}

class Menu{

    public Menu(){
        DisplayMain();
    }

    private void DisplayMain(){
        Console.WriteLine("Welcome to the Doku Solver!");
        Console.WriteLine("1 - Solve a Sudoku Grid");
        Console.WriteLine("2 - Leave the program");
        int input = int.Parse(AwaitInput(null));
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

    private void DisplayFile(){
        Console.WriteLine($"Please enter the path to the file (current path : {Environment.CurrentDirectory})");
        Console.WriteLine("1 - Back to main menu");
        string stringInput = AwaitInput(null);
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
            if (grid != null){
                Console.Clear();
                DisplayAlgorithm(stringInput);
            }else{
                Console.Clear();
                Console.WriteLine("Invalid file");
                DisplayFile();
            }
        }
        
    }
    private void DisplayAlgorithm(string gridFile){
        
    }

    /// <summary>
    /// Display a grid to the console
    /// </summary>
    /// <param name="grid"></param>
    public static void DisplayGrid(Grid grid){
        
    }
    
    /// <summary>
    /// Await input from the user
    /// </summary>
    /// <param name="inputMessage">Display a message before the ReadLine</param>
    /// <returns>String printed by the user</returns>
    private string AwaitInput(string? inputMessage){
        if(inputMessage != null) Console.WriteLine(inputMessage);
        return Console.ReadLine() ?? string.Empty;
    }
}

class Loader{
    public Grid LoadJson(string fileName){
        short[]? flattenedGrid = JsonSerializer.Deserialize<short[]>(File.ReadAllText($"../../../grids/export/{fileName}.json"));
        short[,] grid = new short[(int) Math.Sqrt(flattenedGrid!.Length), (int) Math.Sqrt(flattenedGrid.Length)];
        for (int i = 0; i < grid.GetLength(0); i++)
        for (int j = 0; j < grid.GetLength(1); j++)
            grid[i, j] = flattenedGrid[i * grid.GetLength(0) + j];
        return new Grid(grid);
    }

    public Grid LoadCsv(string fileName){
        return null;
    }

    public Grid LoadtTxt(string fileName){
        short[] flattenArray = File.ReadAllLines(fileName)[0].Split(" ").Select(short.Parse).ToArray();
        int gridLength = (int) Math.Sqrt(flattenArray.Length);
        short[,] grid = new short[gridLength, gridLength];
        for (int i = 0; i < grid.GetLength(0); i++)
            grid[i % gridLength, i / gridLength] = flattenArray[i];
        return new Grid(grid);
    }
}


public class Grid{

    private short[,] arrayGrid;
    private Cursor cursor;
    
    public Grid(short gridSize){
        arrayGrid = new short[gridSize, gridSize];
        cursor = new Cursor(gridSize);
    }
    
    public Grid(short[,] inputGrid){
        arrayGrid = CopyGrid(inputGrid);
        cursor = new Cursor((short)arrayGrid.GetLength(0));
    }
    
    public Grid(Grid grid){
        arrayGrid = CopyGrid(grid.GetGrid());
        cursor = new Cursor((short)arrayGrid.GetLength(0));
    }

    public short[,] CopyGrid(short[,] oldArray){
        short[,] copy = new short[oldArray.GetLength(0), oldArray.GetLength(1)];
        for(short i = 0; i < oldArray.GetLength(0); i++){
            for(short j = 0; j < oldArray.GetLength(1); j++){
                copy[i,j] = oldArray[i,j];
            }
        }
        return copy;
    }
    
    public short[,] GetGrid(){
        return arrayGrid;
    }
    
    public Cursor GetCursor(){
        return cursor;
    }

    public void SetOnCursor(short value){
        arrayGrid[cursor.GetRow(), cursor.GetColumn()] = value;
    }
    
    public short GetOnCursor(){
        return arrayGrid[cursor.GetRow(), cursor.GetColumn()];
    }
    
    public void SetOnCursor(Cursor _cursor, short value){
        arrayGrid[_cursor.GetRow(), _cursor.GetColumn()] = value;
    }
    
    public short GetOnCursor(Cursor _cursor){
        return arrayGrid[_cursor.GetRow(), _cursor.GetColumn()];
    }
    
    public void SetOnPosition(short row, short column, short value){
        arrayGrid[row, column] = value;
    }
    
    public short GetOnPosition(short row, short column){
        return arrayGrid[row, column];
    }

    public short GetLength(){
        return (short)arrayGrid.GetLength(0);
    }
}

public class Cursor{
    private short row;
    private short column;
    private short gridSize;
    
    public Cursor(short row, short column, short gridSize){
        this.row = row;
        this.column = column;
        this.gridSize = gridSize;
    }

    public Cursor(Cursor cursor){
        row = cursor.row;
        column = cursor.column;
        gridSize = cursor.gridSize;
    }

    public Cursor(short gridSize){
        row = 0;
        column = 0;
        this.gridSize = gridSize;
    }
    
    public short GetRow(){
        return row;
    }
    
    public short GetColumn(){
        return column;
    }
    
    public void SetRow(short posX){
        row = posX;
    }
    
    public void SetColumn(short posY){
        column = posY;
    }

    public void Set(short posX, short posY){
        row = posX;
        column = posY;
    }

    public Cursor Next(){
        column++;
        if (column != gridSize) return this;
        row++;
        column = 0;
        return this;
    }

    public Cursor Previous(){
        column--;
        if (column != -1) return this;
        column = (short) (gridSize - 1);
        row--;
        return this;
    }
    
    public bool HasNext(){
        return row != gridSize - 1 || column != gridSize - 1;
    }
    
    public bool HasPrevious(){
        return row != 0 || column != 0;
    }
}

public static class Algorithm{
    public class BackTrack : Solver{
        public override Grid Solve(Grid grid, int maxIterations){
            Grid solvedGrid = new Grid(grid);
            Backtrack(solvedGrid, 0, 0);
            return solvedGrid;
        }

        private bool Backtrack(Grid grid, short row, short column){
            grid.GetCursor().Set(row, column);
            if (!grid.GetCursor().HasNext()) return true;

            if (grid.GetOnPosition(row, column) != 0){
                Cursor nextCursor = new Cursor(grid.GetCursor()).Next();
                return Backtrack(grid, nextCursor.GetRow(), nextCursor.GetColumn());
            }

            for (short i = 1; i <= grid.GetLength(); i++){
                if (!IsPresentForSlot(grid, row, column, i)){
                    grid.SetOnPosition(row, column, i);
                    Cursor nextCursor = new Cursor(grid.GetCursor()).Next();
                    if (Backtrack(grid, nextCursor.GetRow(), nextCursor.GetColumn())) return true;
                }
            }
            grid.SetOnPosition(row, column, 0);
            return false;
        }

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
    
    public class BruteForce : Solver {
        public override Grid Solve(Grid grid, int maxIterations) {
            List<short> possibilities = new List<short>();
            short[ , ] tab = grid.GetGrid();

            do {
                for (int i = 0; i < grid.GetLength(); i++) {
                    for (int j = 0; j < grid.GetLength(); j++) {
                        if (tab[ i, j ] != 0)
                            continue;
                    
                        possibilities.Clear();

                        for (short r = 1; r <= grid.GetLength(); r++)
                            if (IsValidPlacement(tab, r, i, j))
                                possibilities.Add(r);

                        if (possibilities.Count == 1)
                            tab[ i, j ] = possibilities[ 0 ];
                    }
                }
            } while (!IsSolved(grid));
        
            return grid;
        }
    }
    
    public class OtherBackTrack: Solver {
        private int GRID_SIZE;
        public override Grid Solve(Grid grid, int maxIterations) {
            GRID_SIZE = grid.GetLength();
            SmartSolve(grid.GetGrid());
            return grid;
        }

        private bool SmartSolve(short[,] grid) {
            for (int i = 0; i < GRID_SIZE; i++) {
                for (int j = 0; j < GRID_SIZE; j++) {
                    if (grid[i, j] == 0) {
                        for (int t = 1; t <= GRID_SIZE; t++) {
                            if (IsValidPlacement(grid, t, i, j)) {
                                grid[i, j] = (short) t;
                                if (SmartSolve(grid)) return true;
                                grid[i, j] = 0;
                            }
                        }

                        return false;
                    }
                }
            }

            return true;
        }
    }
    
    public class RandomBruteForce : Solver {
        public override Grid Solve(Grid grid, int maxIterations) {
            Random random = new Random();
            short[ , ] tab = grid.GetGrid();

            do {
                for (int i = 0; i < grid.GetLength(); i++)
                for (int j = 0; j < grid.GetLength(); j++)
                    if (tab[ i, j ] == 0)
                        tab[ i, j ] = (short) random.Next(1, grid.GetLength() + 1);
            } while (!IsSolved(grid));
        
            return grid;
        }
    }
    
    public class SlotPerSlot : Solver{
        public override Grid Solve(Grid grid, int maxIterations){
            Grid targetGrid = new Grid(grid);
            int iterations = 0;
            while(!IsFilled(targetGrid) && iterations < maxIterations){
                while (targetGrid.GetCursor().HasNext()){
                    if (targetGrid.GetOnCursor() == 0){
                        List<short> possibilities = GetSlotPossibilities(targetGrid, targetGrid.GetCursor().GetRow(), targetGrid.GetCursor().GetColumn());
                        if (possibilities.Count == 1)
                            targetGrid.SetOnCursor(possibilities[0]);
                    }
                    targetGrid.GetCursor().Next();
                }
                targetGrid.GetCursor().Set(0, 0);
                iterations++;
            }
            return targetGrid;
        }
    }
}

public abstract class Solver : Doku{
    public abstract Grid Solve(Grid grid, int maxIterations);
    
    protected bool IsInRow(short[,] grid, int number, int row) {
        for (int i = 0; i < grid.GetLength(0); i++)
            if (grid[row, i] == number)
                return true;
        return false;
    }

    protected bool IsInColumn(short[,] grid, int number, int column) {
        for (int i = 0; i < grid.GetLength(0); i++)
            if (grid[i, column] == number)
                return true;
        return false;
    }

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

    protected bool IsValidPlacement(short[,] grid, int number, int row, int column) {
        return !IsInRow(grid, number, row) &&
               !IsInColumn(grid, number, column) &&
               !IsInSection(grid, number, row, column);
    }
}

public class Doku{
    protected int[,] Copy(int[,] tab){
        int[,] nTab = new int[tab.GetLength(0), tab.GetLength(1)];
        for (int i = 0; i < tab.GetLength(0); i++)
            for (int j = 0; j < tab.GetLength(1); j++)
                nTab[i, j] = tab[i, j];
        return nTab;
    }

    protected bool IsFilled(short[,] tab){
        bool isFilled = true;
        for (int i = 0; i < tab.GetLength(0) && isFilled; i++)
            for (int j = 0; j < tab.GetLength(1) && isFilled; j++)
                if(tab[i, j] == 0) isFilled = false;
                // if (GetSlotPossibilities(tab, i, j).Count != 0) isSolved = false;
        return isFilled;
    }
    
    protected bool IsFilled(Grid grid){
        return IsFilled(grid.GetGrid());
    }

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

    protected bool IsSolved(Grid grid) {
        return IsSolved(grid.GetGrid());
    }
    
    private readonly List<short> _rowPossibilities = new();
    private readonly List<short> _columnPossibilities = new();
    private readonly List<short> _sectionPossibilities = new();
    private readonly List<short> _possibilities = new();
    
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
    
    protected List<short> GetSlotPossibilities(Grid grid, short row, short column){
        return GetSlotPossibilities(grid.GetGrid(), row, column);
    }
    private void GetColumnPossibilities(short[,] tab, int column){
        _columnPossibilities.Clear();
        for(short i = 1; i <= tab.GetLength(0); i++)
            _columnPossibilities.Add(i);
        for (int i = 0; i < tab.GetLength(0); i++)
            if(_columnPossibilities.Contains(tab[i, column]))
                _columnPossibilities.Remove(tab[i, column]);
    }
    
    private void GetRowPossibilities(short[,] tab, int row){
        _rowPossibilities.Clear();
        for(short i = 1; i <= tab.GetLength(1); i++)
            _rowPossibilities.Add(i);
        for (int i = 0; i < tab.GetLength(1); i++)
            if(_rowPossibilities.Contains(tab[row, i]))
                _rowPossibilities.Remove(tab[row, i]);
    }
    
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

public class DokuTimer{
    
    private double startTime;
    private double endTime;
    
    public void Start(){
        startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() * 1D / 1000;
    }

    public void Stop(){
        endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() * 1D / 1000;
    }

    public double GetResult(){
        return Math.Round(endTime - startTime, 3);
    }
}