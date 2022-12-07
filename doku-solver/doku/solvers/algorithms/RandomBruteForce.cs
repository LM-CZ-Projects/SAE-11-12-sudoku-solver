using doku_solver.grid;

namespace doku_solver.doku.solvers.algorithms; 

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