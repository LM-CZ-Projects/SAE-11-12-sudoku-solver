using doku_solver.doku.tools;
using doku_solver.grid;

namespace doku_solver.doku.solvers.algorithms;

public class SlotPerSlot : Solver{
    public override Grid Solve(Grid grid, int maxIterations){
        Grid targetGrid = new Grid(grid);
        int iterations = 0;
        while(!IsFilled(targetGrid) && iterations < maxIterations){
            while (targetGrid.Cursor.HasNext()){
                Position position = targetGrid.Cursor.GetPosition();
                // Console.WriteLine($"{position.Row}, {position.Column}");
                if (targetGrid.GetOnCursor() == 0){
                    List<short> possibilities = GetSlotPossibilities(targetGrid, position);
                    if (possibilities.Count == 1)
                        targetGrid.SetOnCursor(possibilities[0]);
                }
                targetGrid.Cursor.Next();
            }
            targetGrid.Cursor.Reset();
            iterations++;
        }
        return targetGrid;
    }
}