using System.Reflection;
using doku_solver.doku.solvers.algorithms;
using doku_solver.grid;

namespace doku_solver.doku.solvers;

public class Algorithm{

    public static readonly Algorithm SlotPerSlot = new(typeof(SlotPerSlot));
    public static readonly Algorithm BruteForce = new(typeof(BruteForce));
    public static readonly Algorithm Backtrack = new(typeof(BackTrack));
    public static readonly Algorithm OtherBackTrack = new(typeof(OtherBackTrack));
    public static readonly Algorithm RandomBruteForce = new(typeof(RandomBruteForce));

    private readonly Type _type;

    private Algorithm(Type type){
        _type = type;
    }

    private Solver GetClass(){
        return (Solver) Activator.CreateInstance(_type)! ?? throw new InvalidOperationException();
    }

    public Grid Solve(Grid grid, int maxIterations = 100){
        return GetClass().Solve(grid, maxIterations);
    }

    public static List<Algorithm> GetAlgorithms(){
        List<Algorithm> algorithms = new();
        foreach (FieldInfo fieldInfo in typeof(Algorithm).GetFields())
            if (fieldInfo.GetValue(null) is Algorithm algorithm)
                algorithms.Add(algorithm);
        return algorithms;
    }

}