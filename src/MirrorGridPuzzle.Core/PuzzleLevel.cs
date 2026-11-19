namespace MirrorGridPuzzle.Core;

public readonly record struct Cell(int Row, int Column);

public enum Direction
{
    North,
    East,
    South,
    West
}

public enum MirrorOrientation
{
    Slash,
    Backslash
}

public sealed record Mirror(Cell Cell, MirrorOrientation Orientation);

public static class PuzzleLevel
{
    public const int Size = 6;

    public static Cell Source { get; } = new(2, 3);

    public static Cell Target { get; } = new(4, 1);

    public static Direction LaunchDirection => Direction.East;

    // The list order identifies mirrors A through E.
    public static IReadOnlyList<Mirror> StartingMirrors { get; } = Array.AsReadOnly<Mirror>(
    [
        new(new Cell(2, 5), MirrorOrientation.Backslash),
        new(new Cell(5, 5), MirrorOrientation.Slash),
        new(new Cell(5, 2), MirrorOrientation.Slash),
        new(new Cell(2, 2), MirrorOrientation.Slash),
        new(new Cell(2, 1), MirrorOrientation.Slash)
    ]);
}
