namespace MirrorGridPuzzle.Core;

public sealed record OpticalBeamRoute(Cell Source, double LaunchAngle, Cell Target);

public sealed record OpticalBoard(
    string Name,
    int Size,
    IReadOnlyList<OpticalBeamRoute> Beams,
    IReadOnlyList<AngledMirror> StartingMirrors,
    IReadOnlyList<int> AllowedAngles);

public static class MiddleBoard
{
    public static OpticalBoard Definition { get; } = new(
        OpticsLevel.Name,
        OpticsLevel.Size,
        Array.AsReadOnly<OpticalBeamRoute>(
        [
            new(OpticsLevel.Source, OpticsLevel.LaunchAngle, OpticsLevel.Target),
            new(new Cell(1, 2), 45, new Cell(7, 6))
        ]),
        OpticsLevel.StartingMirrors,
        OpticsLevel.AllowedAngles);
}
