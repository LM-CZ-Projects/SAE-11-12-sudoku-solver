using doku_solver.doku.tools;
using doku_solver.grid;

namespace doku_solver.doku;

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
    
    protected List<short> GetSlotPossibilities(Grid grid, Position position){
        return GetSlotPossibilities(grid.GetGrid(), position.Row, position.Column);
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