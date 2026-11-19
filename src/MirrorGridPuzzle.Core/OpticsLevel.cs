namespace MirrorGridPuzzle.Core;

public sealed record AngledMirror(Cell Cell, int Angle);

public static class OpticsLevel
{
    public const int Size = 7;
    public const double MirrorHalfLength = 0.38;
    public const double TargetRadius = 0.22;

    public static string Name => "Off Axis";

    public static Cell Source { get; } = new(4, 2);

    public static Cell Target { get; } = new(1, 5);

    public static double LaunchAngle => 0;

    public static IReadOnlyList<int> AllowedAngles { get; } = Array.AsReadOnly<int>(
        [15, 30, 45, 60, 90, 105, 120, 135, 150, 165]);

    public static IReadOnlyList<AngledMirror> StartingMirrors { get; } = Array.AsReadOnly<AngledMirror>(
    [
        new(new Cell(4, 4), 15),
        new(new Cell(6, 7), 105),
        new(new Cell(7, 5), 45),
    ]);
}
