using UnityEngine;

public class TurnEvent
{
    public TurnYield myTurnYield { get; } = new TurnYield();

    public bool myIsTurnDone => !myTurnYield.keepWaiting;

    public void SignalDone() => myTurnYield.SignalDone();

    public void ResetState() => myTurnYield.ResetState();
}

public class TurnYield : CustomYieldInstruction
{
    public override bool keepWaiting => !myIsTurnDone;

    private bool myIsTurnDone = false;

    public void SignalDone() => myIsTurnDone = true;

    public void ResetState() => myIsTurnDone = false;
}
