namespace MirrorGridPuzzle.Core;

public enum BeamOutcome
{
    Exited,
    Looped,
    TargetHit,
    TraceLimit
}

public readonly record struct BeamState(Cell Cell, Direction Direction);

public sealed record BeamTrace(BeamOutcome Outcome, IReadOnlyList<BeamState> Path)
{
    public Direction? ExitDirection { get; init; }
}
