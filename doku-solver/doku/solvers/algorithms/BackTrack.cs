using doku_solver.doku.tools;
using doku_solver.grid;

namespace doku_solver.doku.solvers.algorithms;

public class BackTrack : Solver{

    private int _maxDeep;
    
    public override Grid Solve(Grid grid, int maxIterations){
        _maxDeep = maxIterations;
        Grid solvedGrid = new Grid(grid);
        Backtrack(solvedGrid, 0, 0);
        return solvedGrid;
    }

    private bool Backtrack(Grid grid, int row, int column){
        Position position = new Position(row, column);
        grid.Cursor.SetPosition(position);
        if (!grid.Cursor.HasNext()) return true;

        if (grid.GetOnPosition(position) != 0){
            Position nextPosition = new Cursor(grid.GetLength()).SetPosition(position).Next().GetPosition();
            return Backtrack(grid, nextPosition.Row, nextPosition.Column);
        }

        for (int i = 1; i <= grid.GetLength(); i++){
            if (!IsPresentForSlot(grid, position, i)){
                grid.SetOnPosition(position, i);
                Position nextPosition = new Cursor(grid.GetLength()).SetPosition(position).Next().GetPosition();
                if (Backtrack(grid, nextPosition.Row, nextPosition.Column)) return true;
            }
        }
        grid.SetOnPosition(position, 0);
        return false;
    }

    private bool IsPresentForSlot(Grid grid, int row, int column, int value){
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

    private bool IsPresentForSlot(Grid grid, Position position, int value){
        return IsPresentForSlot(grid, position.Row, position.Column, value);
    }
}