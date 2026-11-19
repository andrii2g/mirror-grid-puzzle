using MirrorGridPuzzle.Core;
using Xunit;

namespace MirrorGridPuzzle.Tests;

public sealed class GridLevelTests
{
    [Fact]
    public void FirstLightExitsAtTheFourByFourEdgeAndWinsWithOneRotation()
    {
        var level = GridLevels.All[0];
        var beam = Assert.Single(level.Beams);
        var mirror = Assert.Single(level.StartingMirrors);

        var start = Trace(level, beam, level.StartingMirrors);
        Assert.Equal(BeamOutcome.Exited, start.Outcome);
        Assert.Equal(new BeamState(new Cell(1, 3), Direction.North), start.Path[^1]);
        Assert.Equal(Direction.North, start.ExitDirection);

        var win = Trace(level, beam, [mirror with { Orientation = MirrorOrientation.Backslash }]);
        Assert.Equal(BeamOutcome.TargetHit, win.Outcome);
        Assert.Equal(new BeamState(beam.Target, Direction.South), win.Path[^1]);
    }

    [Fact]
    public void CrossedBeamsCanReachOneTargetBeforeReachingBoth()
    {
        var level = GridLevels.All[1];
        Assert.Equal(2, level.Beams.Count);
        Assert.All(level.Beams, beam =>
            Assert.Equal(BeamOutcome.Exited, Trace(level, beam, level.StartingMirrors).Outcome));

        var mirrors = level.StartingMirrors.ToArray();
        mirrors[0] = mirrors[0] with { Orientation = MirrorOrientation.Backslash };
        mirrors[1] = mirrors[1] with { Orientation = MirrorOrientation.Slash };

        var first = Trace(level, level.Beams[0], mirrors);
        var second = Trace(level, level.Beams[1], mirrors);
        Assert.Equal(BeamOutcome.TargetHit, first.Outcome);
        Assert.Equal(new BeamState(level.Beams[0].Target, Direction.West), first.Path[^1]);
        Assert.Equal(BeamOutcome.Exited, second.Outcome);
        Assert.Equal(new BeamState(new Cell(5, 4), Direction.South), second.Path[^1]);

        mirrors[2] = mirrors[2] with { Orientation = MirrorOrientation.Slash };
        second = Trace(level, level.Beams[1], mirrors);
        Assert.Equal(BeamOutcome.TargetHit, second.Outcome);
        Assert.Equal(new BeamState(level.Beams[1].Target, Direction.North), second.Path[^1]);
        Assert.Contains(first.Path, state => state.Cell == mirrors[0].Cell);
        Assert.Contains(second.Path, state => state.Cell == mirrors[0].Cell);
    }

    [Fact]
    public void CrossedBeamsHasExactlyOneConfigurationThatHitsBothTargets()
    {
        var level = GridLevels.All[1];
        var winningConfigurations = 0;

        for (var mask = 0; mask < 1 << level.StartingMirrors.Count; mask++)
        {
            var mirrors = level.StartingMirrors.Select((mirror, index) => mirror with
            {
                Orientation = (mask & (1 << index)) == 0 ? MirrorOrientation.Slash : MirrorOrientation.Backslash
            }).ToArray();
            if (level.Beams.All(beam => Trace(level, beam, mirrors).Outcome == BeamOutcome.TargetHit))
            {
                winningConfigurations++;
            }
        }

        Assert.Equal(1, winningConfigurations);
    }

    [Fact]
    public void ClosedCircuitKeepsTheOriginalSixBySixBoard()
    {
        var level = GridLevels.All[2];
        var beam = Assert.Single(level.Beams);

        Assert.Equal(PuzzleLevel.Size, level.Size);
        Assert.Equal(new BeamRoute(PuzzleLevel.Source, PuzzleLevel.LaunchDirection, PuzzleLevel.Target), beam);
        Assert.Equal(PuzzleLevel.StartingMirrors, level.StartingMirrors);
    }

    private static BeamTrace Trace(GridLevel level, BeamRoute beam, IReadOnlyList<Mirror> mirrors) =>
        BeamTracer.Trace(level.Size, beam.Source, beam.Direction, beam.Target, mirrors);
}
