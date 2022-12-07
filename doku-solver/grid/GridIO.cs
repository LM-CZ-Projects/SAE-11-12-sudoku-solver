using System.Text;
using System.Text.Json;
using doku_solver.doku.generator;
using doku_solver.doku.tools;

namespace doku_solver.grid;

public class GridIO{
    
    protected readonly short[,] _grid;
    
    // Constructors \\
    
    public GridIO(int sectionSize){
        _grid = new short[sectionSize, sectionSize];
    }

    public GridIO(short[,] grid){
        _grid = CopyArray(grid);
    }
    
    // Grid Management Methods \\
    
    protected short[,] CopyArray(short[,] array){
        short[,] newArray = new short[array.GetLength(0), array.GetLength(1)];
        for(int i = 0; i < array.GetLength(0); i++)
            for(int j = 0; j < array.GetLength(1); j++)
                newArray[i, j] = array[i, j];
        return newArray;
    }
    
    public short[,] GetGrid(){
        return _grid;
    }
    
    // IO Methods \\
    
    public static void ExportJsonGrid(short[,] grid){
        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        string jsonSerialized = JsonSerializer.Serialize(grid, options);
        File.WriteAllText("../../../grids/export/{fileName}.json", jsonSerialized);
    }

    public static void GenerateExportJsonGrids(int count, int sectionSize, string fileName){
        List<Grid> grids = new List<Grid>();
        Generator generator = new Generator();
        Console.WriteLine("Generating grids...");
        for (int i = 0; i < count; i++)
            grids.Add(generator.GenerateUnsolved(sectionSize));
        Console.WriteLine("Exporting grids...");
        ExportJsonGrids(grids, fileName);
    }
    
    public static void ExportJsonGrids(List<Grid> grids, string fileName){
        List<short[]> flattenedGrids = new List<short[]>();
        foreach (var grid in grids){
            short[] flattenedGrid = new short[grid.GetLength() * grid.GetLength()];
            for (int i = 0; i < grid.GetLength(); i++)
                for (int j = 0; j < grid.GetLength(); j++)
                    flattenedGrid[i * grid.GetLength() + j] = grid.GetOnPosition(new Position(i, j));
            flattenedGrids.Add(flattenedGrid);
        }
        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        string jsonSerialized = JsonSerializer.Serialize(flattenedGrids, options);
        File.WriteAllText($"../../../grids/export/{fileName}.json", jsonSerialized);
    }
    
    public static void ExportCsvGrid(short[,] grid, string fileName){
        List<string> lines = new List<string>();
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < grid.GetLength(0); i++){
            for (int j = 0; j < grid.GetLength(1); j++){
                if(grid[i, j] == 0) builder.Append(" " + ";");
                else builder.Append(grid[i, j] + ";");
            }
            lines.Add(builder.ToString());
            builder.Clear();
        }
        String final = String.Join(Environment.NewLine, lines);
        File.WriteAllText($"../../../grids/export/{fileName}.csv", final);
    }

    public static short[,] ImportJsonGrid(string fileName){
        short[]? flattenedGrid = JsonSerializer.Deserialize<short[]>(File.ReadAllText($"../../../grids/export/{fileName}.json"));
        short[,] grid = new short[(int) Math.Sqrt(flattenedGrid!.Length), (int) Math.Sqrt(flattenedGrid.Length)];
        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
                grid[i, j] = flattenedGrid[i * grid.GetLength(0) + j];
        return grid;
    }
    
    public static List<Grid> ImportJsonGrids(string fileName){
        List<short[]>? flattenedGrids = JsonSerializer.Deserialize<List<short[]>>(File.ReadAllText($"../../../grids/export/{fileName}.json"));
        List<Grid> grids = new List<Grid>();
        foreach (var flattenedGrid in flattenedGrids!){
            short[,] grid = new short[(int) Math.Sqrt(flattenedGrid.Length), (int) Math.Sqrt(flattenedGrid.Length)];
            for (int i = 0; i < grid.GetLength(0); i++)
                for (int j = 0; j < grid.GetLength(1); j++)
                    grid[i, j] = flattenedGrid[i * grid.GetLength(0) + j];
            grids.Add(new Grid(grid));
        }
        return grids;
    }
}