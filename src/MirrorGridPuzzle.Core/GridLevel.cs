namespace MirrorGridPuzzle.Core;

public sealed record BeamRoute(Cell Source, Direction Direction, Cell Target);

public sealed record GridLevel(
    string Name,
    int Size,
    IReadOnlyList<BeamRoute> Beams,
    IReadOnlyList<Mirror> StartingMirrors);

public static class GridLevels
{
    public static IReadOnlyList<GridLevel> All { get; } = Array.AsReadOnly<GridLevel>(
    [
        new("First Light", 4,
            Array.AsReadOnly<BeamRoute>([new(new Cell(2, 1), Direction.East, new Cell(4, 3))]),
            Array.AsReadOnly<Mirror>([new(new Cell(2, 3), MirrorOrientation.Slash)])),
        new("Crossed Beams", 5,
            Array.AsReadOnly<BeamRoute>(
            [
                new(new Cell(3, 1), Direction.East, new Cell(4, 1)),
                new(new Cell(1, 3), Direction.South, new Cell(1, 4))
            ]),
            Array.AsReadOnly<Mirror>(
            [
                new(new Cell(3, 3), MirrorOrientation.Slash),
                new(new Cell(4, 3), MirrorOrientation.Backslash),
                new(new Cell(3, 4), MirrorOrientation.Backslash)
            ])),
        new("Closed Circuit", PuzzleLevel.Size,
            Array.AsReadOnly<BeamRoute>([new(PuzzleLevel.Source, PuzzleLevel.LaunchDirection, PuzzleLevel.Target)]),
            PuzzleLevel.StartingMirrors)
    ]);
}
