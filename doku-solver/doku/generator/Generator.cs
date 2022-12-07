using System.Text;
using System.Text.Json;
using doku_solver.doku.solvers;
using doku_solver.doku.tools;
using doku_solver.grid;

namespace doku_solver.doku.generator;

public class Generator : Doku{

    public Grid GenerateUnsolved(int sectionSize){
        Grid baseGrid = GenerateSolvedGrid(sectionSize);
        while (baseGrid == null!)
            baseGrid = GenerateSolvedGrid(sectionSize);
        Grid grid;
        do{
            grid = new Grid(baseGrid);
            List<Position> removedPositions = new List<Position>();
            List<short> removedValues = new List<short>();
            Position toRemove = GetRandomPosition(sectionSize);
            if (grid.GetOnPosition(toRemove) != 0){
                removedValues.Add(grid.GetOnPosition(toRemove));
                removedPositions.Add(toRemove);
                grid.SetOnPosition(toRemove, 0);
            }
            while (IsDeletionValid(grid, removedPositions)){
                toRemove = GetRandomPosition(sectionSize);
                if (grid.GetOnPosition(toRemove) != 0){
                    removedValues.Add(grid.GetOnPosition(toRemove));
                    removedPositions.Add(toRemove);
                    grid.SetOnPosition(toRemove, 0);
                }
            }
            grid.SetOnPosition(removedPositions[^1], removedValues[^1]);
        } while (IsSolved(Algorithm.SlotPerSlot.Solve(grid, 100)));
        return grid;
    }

    private bool IsDeletionValid(Grid grid, List<Position> positions){
        bool is1Present = false;
        bool isLess2Present = true;
        foreach (Position position in positions){
            if (GetSlotPossibilities(grid.GetGrid(), position.Row, position.Column).Count == 1)
                is1Present = true;
            if (GetSlotPossibilities(grid.GetGrid(), position.Row, position.Column).Count >= 2 && isLess2Present)
                isLess2Present = false;
        }
        return is1Present && isLess2Present;
    }

    private Position GetRandomPosition(int sectionSize){
        Random random = new Random();
        return new Position(random.Next(sectionSize * sectionSize), random.Next(sectionSize * sectionSize));
    }

    public Grid GenerateSolvedGrid(int sectionSize){
        int gridSize = sectionSize * sectionSize;
        Grid baseGrid = new Grid(gridSize);
        GenerateBaseGrid(baseGrid, sectionSize);
        

        Algorithm backTrack = Algorithm.Backtrack;
        // int[,] solvedGrid = backTrack.Solve(grid, (int) Math.Pow(Factorial(sectionSize), 3)); // 5!^3
        Grid solvedGrid = backTrack.Solve(baseGrid, -1);
        // DokuSolver.DisplayGrid(grid);
        if (!IsFilled(solvedGrid)) return null!;
        return solvedGrid;
    }

    private long Factorial(int n){
        if (n <= 1) return 1;
        return n * Factorial(n - 1);
    }

    private List<Position> GetAvailablePositions(Grid grid){
        List<Position> availablePositions = new List<Position>();
        for (int i = 0; i < grid.GetLength(); i++)
            for (int j = 0; j < grid.GetLength(); j++){
                Position currentPosition = new Position(i, j);
                if (grid.GetOnPosition(currentPosition) != 0)
                    availablePositions.Add(new Position(i, j));
            }
        return availablePositions;
    }

    private void GenerateBaseGrid(Grid grid, int sectionSize){
        for(int i = 0; i < sectionSize; i++)
            MergeSection(grid, GenerateSection(sectionSize), i, i);
    }

    private void MergeSection(Grid grid, short[,] section, int sectionRow, int sectionColumn){
        int sectionSize = section.GetLength(0);
        for (int row = 0; row < sectionSize; row++)
            for (int column = 0; column < sectionSize; column++)
                grid.SetOnPosition(new Position(sectionRow * sectionSize + row, sectionColumn * sectionSize + column), section[row, column]);
    }
    
    private short[,] GenerateSection(int sectionSize){
        short[,] section = new short[sectionSize, sectionSize];
        List<int> numbers = Enumerable.Range(1, sectionSize * sectionSize).ToList();
        Random random = new Random();
        for (int i = 0; i < sectionSize; i++){
            for (int j = 0; j < sectionSize; j++){
                int index = random.Next(numbers.Count);
                section[i, j] = (short) numbers[index];
                numbers.RemoveAt(index);
            }
        }
        return section;
    }
}