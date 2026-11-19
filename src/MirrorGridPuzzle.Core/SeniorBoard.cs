namespace MirrorGridPuzzle.Core;

public static class SeniorBoard
{
    public const string Name = "Shared Focus";

    private static readonly IReadOnlyList<int> AllowedAngles =
        Array.AsReadOnly<int>([25, 30, 35, 40, 45, 50, 55, 60, 65]);

    public static IReadOnlyList<OpticalBoard> Presets { get; } = Array.AsReadOnly<OpticalBoard>(
    [
        CreatePreset("Central Crossing",
            [
                new(new Cell(6, 4), 5, new Cell(7, 6)),
                new(new Cell(4, 3), 30, new Cell(4, 9)),
                new(new Cell(4, 4), 65, new Cell(10, 8)),
            ],
            [
                new(new Cell(6, 7), 30),
                new(new Cell(6, 5), 35),
                new(new Cell(5, 5), 55),
            ]),
        CreatePreset("Northern Relay",
            [
                new(new Cell(2, 7), 5, new Cell(7, 9)),
                new(new Cell(1, 4), 30, new Cell(8, 9)),
                new(new Cell(3, 3), 65, new Cell(9, 10)),
            ],
            [
                new(new Cell(2, 6), 65),
                new(new Cell(2, 8), 60),
                new(new Cell(9, 6), 30),
            ]),
        CreatePreset("Far Corner",
            [
                new(new Cell(3, 2), 5, new Cell(7, 9)),
                new(new Cell(4, 1), 30, new Cell(9, 9)),
                new(new Cell(5, 3), 65, new Cell(10, 9)),
            ],
            [
                new(new Cell(8, 8), 35),
                new(new Cell(4, 10), 55),
                new(new Cell(7, 4), 35),
            ]),
    ]);

    public static OpticalBoard Definition => Presets[0];

    public static OpticalBoard PickPreset() => Presets[Random.Shared.Next(Presets.Count)];

    private static OpticalBoard CreatePreset(string name, OpticalBeamRoute[] beams, AngledMirror[] mirrors) =>
        new(name, 10, Array.AsReadOnly(beams), Array.AsReadOnly(mirrors), AllowedAngles);
}
