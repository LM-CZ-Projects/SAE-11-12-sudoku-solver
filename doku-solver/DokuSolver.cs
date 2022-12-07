using doku_solver.doku;
using doku_solver.doku.solvers;
using doku_solver.grid;

namespace doku_solver;

public static class DokuSolver {
    public static void Main() {
        new Tester().TestAlgorithm(Algorithm.RandomBruteForce, true, 2);
    }
    
    public static void DisplayGrid(Grid grid) {
        Console.WriteLine("-----------------");
        grid.Cursor.Reset();
        while (grid.Cursor.HasNext()){
            if (grid.Cursor.GetPosition().Column == 0) Console.WriteLine();
            Console.Write(grid.GetOnCursor() + " ");
            grid.Cursor.Next();
        }
        Console.WriteLine(grid.GetOnCursor() + " ");
    }
}
