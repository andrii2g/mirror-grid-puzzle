using MirrorGridPuzzle.Core;
using Xunit;

namespace MirrorGridPuzzle.Tests;

public sealed class OpticalBoardTests
{
    [Fact]
    public void OffAxisStartsWithTwoBeamsThatExitTheBoard()
    {
        var board = MiddleBoard.Definition;

        Assert.Equal(7, board.Size);
        Assert.Equal(2, board.Beams.Count);
        Assert.All(board.Beams, beam =>
            Assert.Equal(BeamOutcome.Exited, OpticalTracer.Trace(board, beam, board.StartingMirrors).Outcome));
    }

    [Fact]
    public void OffAxisBeamsUseASharedMirrorAndReachTheirOwnTargets()
    {
        var board = MiddleBoard.Definition;
        var mirrors = board.StartingMirrors.ToArray();
        mirrors[1] = mirrors[1] with { Angle = 90 };

        Assert.Equal(
            [BeamOutcome.Exited, BeamOutcome.TargetHit],
            board.Beams.Select(beam => OpticalTracer.Trace(board, beam, mirrors).Outcome));

        mirrors[2] = mirrors[2] with { Angle = 30 };
        for (var index = 0; index < board.Beams.Count; index++)
        {
            var beam = board.Beams[index];
            var trace = OpticalTracer.Trace(board, beam, mirrors);

            Assert.Equal(BeamOutcome.TargetHit, trace.Outcome);
            Assert.Equal(beam.Target, trace.Path[^1].Cell);
            Assert.Contains(trace.Path, step => step.Cell == mirrors[1].Cell && Math.Abs(step.Point.X - 6.5) < 1e-8);
        }
    }

    [Fact]
    public void OpticalTracerUsesTheSelectedBoardsAllowedAngles()
    {
        var beam = new OpticalBeamRoute(new Cell(3, 2), 0, new Cell(1, 1));
        var mirror = new AngledMirror(new Cell(3, 4), 5);
        var board = new OpticalBoard("Fine Angle", 7, [beam], [mirror], [5, 30, 65]);

        var trace = OpticalTracer.Trace(board, beam, board.StartingMirrors);

        Assert.Contains(trace.Path, step => step.IncomingAngle == 10);
        Assert.Throws<ArgumentException>(() => OpticalTracer.Trace(board, beam, [mirror with { Angle = 15 }]));
        Assert.Throws<ArgumentException>(() => OpticalTracer.Trace(board.Size, beam.Source, beam.LaunchAngle,
            beam.Target, board.StartingMirrors));
    }
}
