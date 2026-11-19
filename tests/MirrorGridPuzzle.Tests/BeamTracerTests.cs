using MirrorGridPuzzle.Core;
using Xunit;

namespace MirrorGridPuzzle.Tests;

public sealed class BeamTracerTests
{
    [Fact]
    public void StartingBoardExitsBelowTheBottomEdge()
    {
        var trace = BeamTracer.Trace(PuzzleLevel.StartingMirrors);

        Assert.Equal(BeamOutcome.Exited, trace.Outcome);
        Assert.Equal(new BeamState(new Cell(2, 3), Direction.East), trace.Path[0]);
        Assert.Equal(new BeamState(new Cell(6, 2), Direction.South), trace.Path[^1]);
        Assert.Equal(Direction.South, trace.ExitDirection);
    }

    [Fact]
    public void RotatingCClosesALoopAtTheSource()
    {
        var trace = BeamTracer.Trace(WithRotatedMirrors(2));
        var beforeClosing = trace.Path.SkipLast(1);

        Assert.Equal(BeamOutcome.Looped, trace.Outcome);
        Assert.Equal(new BeamState(new Cell(2, 3), Direction.East), trace.Path[^1]);
        Assert.Equal(trace.Path[0], trace.Path[^1]);
        Assert.Equal(beforeClosing.Distinct(), beforeClosing);
        Assert.Null(trace.ExitDirection);
    }

    [Fact]
    public void RotatingCAndDReachesTheTargetViaAllFiveMirrors()
    {
        var trace = BeamTracer.Trace(WithRotatedMirrors(2, 3));
        var mirrorCells = PuzzleLevel.StartingMirrors.Select(mirror => mirror.Cell).ToArray();
        var visitedMirrors = trace.Path.Select(state => state.Cell).Where(mirrorCells.Contains);

        Assert.Equal(BeamOutcome.TargetHit, trace.Outcome);
        Assert.Equal(new BeamState(new Cell(4, 1), Direction.South), trace.Path[^1]);
        Assert.Equal(mirrorCells, visitedMirrors);
        Assert.Null(trace.ExitDirection);
    }

    [Fact]
    public void ExactlyOneOfThe32ConfigurationsWins()
    {
        var outcomes = Enumerable.Range(0, 32).Select(configuration =>
        {
            var mirrors = PuzzleLevel.StartingMirrors.Select((mirror, index) => mirror with
            {
                Orientation = (configuration & (1 << index)) == 0
                    ? MirrorOrientation.Slash
                    : MirrorOrientation.Backslash,
            }).ToArray();

            return BeamTracer.Trace(mirrors).Outcome;
        }).ToArray();

        Assert.Equal(1, outcomes.Count(outcome => outcome == BeamOutcome.TargetHit));
        Assert.Equal(2, outcomes.Count(outcome => outcome == BeamOutcome.Looped));
        Assert.Equal(29, outcomes.Count(outcome => outcome == BeamOutcome.Exited));
    }

    [Fact]
    public void CrossingTheSameCellInAnotherDirectionContinuesTheTrace()
    {
        Mirror[] mirrors =
        [
            new(new Cell(3, 5), MirrorOrientation.Backslash),
            new(new Cell(5, 5), MirrorOrientation.Slash),
            new(new Cell(5, 3), MirrorOrientation.Backslash),
        ];

        var trace = BeamTracer.Trace(new Cell(3, 1), Direction.East, new Cell(1, 3), mirrors);
        var crossing = new Cell(3, 3);

        Assert.Equal(BeamOutcome.TargetHit, trace.Outcome);
        Assert.Contains(new BeamState(crossing, Direction.East), trace.Path);
        Assert.Contains(new BeamState(crossing, Direction.North), trace.Path);
    }

    [Fact]
    public void ExitDirectionUsesTheReflectionAtAnEdgeMirror()
    {
        var source = new Cell(1, 2);
        var edge = new Cell(1, 3);
        var trace = BeamTracer.Trace(source, Direction.East, new Cell(6, 6),
            [new Mirror(edge, MirrorOrientation.Slash)]);

        Assert.Equal(BeamOutcome.Exited, trace.Outcome);
        Assert.Equal<BeamState>([new(source, Direction.East), new(edge, Direction.East)], trace.Path);
        Assert.Equal(Direction.North, trace.ExitDirection);
    }

    [Theory]
    [InlineData(MirrorOrientation.Slash, Direction.North, Direction.East, 4, 3, 3, 4)]
    [InlineData(MirrorOrientation.Slash, Direction.East, Direction.North, 3, 2, 2, 3)]
    [InlineData(MirrorOrientation.Slash, Direction.South, Direction.West, 2, 3, 3, 2)]
    [InlineData(MirrorOrientation.Slash, Direction.West, Direction.South, 3, 4, 4, 3)]
    [InlineData(MirrorOrientation.Backslash, Direction.North, Direction.West, 4, 3, 3, 2)]
    [InlineData(MirrorOrientation.Backslash, Direction.East, Direction.South, 3, 2, 4, 3)]
    [InlineData(MirrorOrientation.Backslash, Direction.South, Direction.East, 2, 3, 3, 4)]
    [InlineData(MirrorOrientation.Backslash, Direction.West, Direction.North, 3, 4, 2, 3)]
    public void EachDiagonalReflectsEveryCardinalDirection(
        MirrorOrientation orientation, Direction incoming, Direction reflected,
        int sourceRow, int sourceColumn, int targetRow, int targetColumn)
    {
        var source = new Cell(sourceRow, sourceColumn);
        var mirror = new Cell(3, 3);
        var target = new Cell(targetRow, targetColumn);

        var trace = BeamTracer.Trace(source, incoming, target, [new Mirror(mirror, orientation)]);

        Assert.Equal(BeamOutcome.TargetHit, trace.Outcome);
        Assert.Equal<BeamState>(
            [new(source, incoming), new(mirror, incoming), new(target, reflected)], trace.Path);
    }

    private static Mirror[] WithRotatedMirrors(params int[] indices)
    {
        var mirrors = PuzzleLevel.StartingMirrors.ToArray();
        foreach (var index in indices)
        {
            var mirror = mirrors[index];
            mirrors[index] = mirror with
            {
                Orientation = mirror.Orientation == MirrorOrientation.Slash
                    ? MirrorOrientation.Backslash
                    : MirrorOrientation.Slash,
            };
        }

        return mirrors;
    }
}
