namespace MirrorGridPuzzle.Core;

public static class BeamTracer
{
    public static BeamTrace Trace(IReadOnlyList<Mirror> mirrors) =>
        Trace(PuzzleLevel.Source, PuzzleLevel.LaunchDirection, PuzzleLevel.Target, mirrors);

    public static BeamTrace Trace(Cell source, Direction direction, Cell target, IReadOnlyList<Mirror> mirrors) =>
        Trace(PuzzleLevel.Size, source, direction, target, mirrors);

    public static BeamTrace Trace(int size, Cell source, Direction direction, Cell target, IReadOnlyList<Mirror> mirrors)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);
        ArgumentNullException.ThrowIfNull(mirrors);

        if (!IsOnBoard(source, size))
        {
            throw new ArgumentOutOfRangeException(nameof(source));
        }

        if (!IsOnBoard(target, size))
        {
            throw new ArgumentOutOfRangeException(nameof(target));
        }

        if (!Enum.IsDefined(direction))
        {
            throw new ArgumentOutOfRangeException(nameof(direction));
        }

        var orientations = mirrors.ToDictionary(mirror => mirror.Cell, mirror => mirror.Orientation);
        var visited = new HashSet<BeamState>();
        var path = new List<BeamState>();
        var cell = source;

        while (IsOnBoard(cell, size))
        {
            var state = new BeamState(cell, direction);
            // Keep the repeated state at the end so the path closes the loop.
            path.Add(state);

            if (cell == target)
            {
                return new BeamTrace(BeamOutcome.TargetHit, path.AsReadOnly());
            }

            if (!visited.Add(state))
            {
                return new BeamTrace(BeamOutcome.Looped, path.AsReadOnly());
            }

            if (orientations.TryGetValue(cell, out var orientation))
            {
                direction = Reflect(direction, orientation);
            }

            cell = Step(cell, direction);
        }

        return new BeamTrace(BeamOutcome.Exited, path.AsReadOnly()) { ExitDirection = direction };
    }

    private static bool IsOnBoard(Cell cell, int size) =>
        cell.Row >= 1 && cell.Row <= size && cell.Column >= 1 && cell.Column <= size;

    private static Cell Step(Cell cell, Direction direction) => direction switch
    {
        Direction.North => cell with { Row = cell.Row - 1 },
        Direction.East => cell with { Column = cell.Column + 1 },
        Direction.South => cell with { Row = cell.Row + 1 },
        Direction.West => cell with { Column = cell.Column - 1 },
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };

    private static Direction Reflect(Direction direction, MirrorOrientation orientation) => (orientation, direction) switch
    {
        (MirrorOrientation.Slash, Direction.North) => Direction.East,
        (MirrorOrientation.Slash, Direction.East) => Direction.North,
        (MirrorOrientation.Slash, Direction.South) => Direction.West,
        (MirrorOrientation.Slash, Direction.West) => Direction.South,
        (MirrorOrientation.Backslash, Direction.North) => Direction.West,
        (MirrorOrientation.Backslash, Direction.East) => Direction.South,
        (MirrorOrientation.Backslash, Direction.South) => Direction.East,
        (MirrorOrientation.Backslash, Direction.West) => Direction.North,
        _ => throw new ArgumentOutOfRangeException(nameof(orientation))
    };
}
