namespace MirrorGridPuzzle.Core;

public static class SeniorBoard
{
    public static OpticalBoard Definition { get; } = new(
        "Shared Focus",
        10,
        Array.AsReadOnly<OpticalBeamRoute>(
        [
            new(new Cell(6, 4), 5, new Cell(7, 6)),
            new(new Cell(4, 3), 30, new Cell(4, 9)),
            new(new Cell(4, 4), 65, new Cell(10, 8)),
        ]),
        Array.AsReadOnly<AngledMirror>(
        [
            new(new Cell(6, 7), 30),
            new(new Cell(6, 5), 35),
            new(new Cell(5, 5), 55),
        ]),
        Array.AsReadOnly<int>([25, 30, 35, 40, 45, 50, 55, 60, 65]));
}
