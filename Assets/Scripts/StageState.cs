using System.Collections;
public class StageState
{
    public bool myIsRunning => myIsPlayerAlive && !myIsStageWon;

    public bool myIsStageWon { get; set; } = false;
    public bool myIsPlayerAlive { get; set; } = true;
}
