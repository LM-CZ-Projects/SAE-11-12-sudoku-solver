namespace doku_solver.grid;

public class Loader{
    private readonly string[] _content;
    
    public Loader(string gridName){
        _content = File.ReadAllLines($"../../../grids/{gridName}.txt");
    }

    public int[,] GetGrid(){
        int[,] tab = new int[_content.Length, _content.Length];
        for(int i = 0; i < _content.Length; i++)
            for(int j = 0; j < _content.Length; j++)
                tab[i, j] = int.Parse(_content[i][j].ToString());
        return tab;
    }
    
}