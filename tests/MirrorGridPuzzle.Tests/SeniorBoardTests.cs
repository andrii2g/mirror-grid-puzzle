using MirrorGridPuzzle.Core;
using Xunit;

namespace MirrorGridPuzzle.Tests;

public sealed class SeniorBoardTests
{
    [Fact]
    public void ThreeBeamsReachTheirTargetsOneAtATime()
    {
        var board = SeniorBoard.Definition;
        Assert.Equal(10, board.Size);
        Assert.Equal([5d, 30d, 65d], board.Beams.Select(beam => beam.LaunchAngle));

        Assert.Equal([false, false, false], TargetHits(board.StartingMirrors));
        Assert.Equal([true, false, false], TargetHits(WithAngles(30, 30, 55)));
        Assert.Equal([true, false, true], TargetHits(WithAngles(35, 30, 55)));
        Assert.Equal([true, true, true], TargetHits(WithAngles(35, 30, 60)));
    }

    [Fact]
    public void WinningBeamsAllReflectAtTheSameMirrorOnTheirWayToTheirOwnTargets()
    {
        var board = SeniorBoard.Definition;
        var mirrors = WithAngles(35, 30, 60);
        var sharedCell = new Cell(6, 5);

        Assert.All(board.Beams, beam =>
        {
            var trace = OpticalTracer.Trace(board, beam, mirrors);
            var reflections = trace.Path.Zip(trace.Path.Skip(1))
                .Where(steps => steps.First.IncomingAngle != steps.Second.IncomingAngle)
                .Select(steps => steps.First.Cell);

            Assert.Equal(BeamOutcome.TargetHit, trace.Outcome);
            Assert.Equal(beam.Target, trace.Path[^1].Cell);
            Assert.Contains(sharedCell, reflections);
        });
    }

    [Fact]
    public void OnlyOneConfigurationReachesAllThreeTargets()
    {
        var board = SeniorBoard.Definition;
        var winningConfigurations = new List<int[]>();

        foreach (var first in board.AllowedAngles)
        {
            foreach (var second in board.AllowedAngles)
            {
                foreach (var third in board.AllowedAngles)
                {
                    var mirrors = WithAngles(first, second, third);
                    if (board.Beams.All(beam => OpticalTracer.Trace(board, beam, mirrors).Outcome == BeamOutcome.TargetHit))
                    {
                        winningConfigurations.Add([first, second, third]);
                    }
                }
            }
        }

        Assert.Equal([35, 30, 60], Assert.Single(winningConfigurations));
    }

    private static IEnumerable<bool> TargetHits(IReadOnlyList<AngledMirror> mirrors)
    {
        var board = SeniorBoard.Definition;
        return board.Beams.Select(beam => OpticalTracer.Trace(board, beam, mirrors).Outcome == BeamOutcome.TargetHit);
    }

    private static AngledMirror[] WithAngles(params int[] angles) =>
        SeniorBoard.Definition.StartingMirrors.Select((mirror, index) => mirror with { Angle = angles[index] }).ToArray();
}
