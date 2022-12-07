using doku_solver.grid;

namespace doku_solver.doku.solvers.algorithms; 

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