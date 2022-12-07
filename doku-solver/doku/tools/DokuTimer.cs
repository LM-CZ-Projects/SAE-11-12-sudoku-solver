namespace doku_solver.doku.tools;

public class DokuTimer{
    
    private double startTime;
    private double endTime;
    
    public void Start(){
        startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() * 1D / 1000;
    }

    public void Stop(){
        endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() * 1D / 1000;
    }

    public double GetResult(){
        return Math.Round(endTime - startTime, 3);
    }
}