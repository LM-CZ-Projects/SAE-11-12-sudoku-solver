using doku_solver.grid;

namespace doku_solver.doku.solvers.algorithms;

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