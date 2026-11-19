using MirrorGridPuzzle.Core;
using Xunit;

namespace MirrorGridPuzzle.Tests;

public sealed class OpticalTracerTests
{
    [Fact]
    public void StartingBoardReflectsOffTheFirstTwoMirrorsAndExits()
    {
        var trace = OpticalTracer.Trace(OpticsLevel.StartingMirrors);

        Assert.Equal(BeamOutcome.Exited, trace.Outcome);
        Assert.Equal([0d, 30d, 180d], trace.Path.Select(step => step.IncomingAngle).Distinct());
        Assert.Equal(0, trace.Path[^1].Point.X, 8);
        Assert.Equal(1, trace.Path[^1].Cell.Column);
    }

    [Fact]
    public void WinningAnglesReachTheTargetsRimAfterThreeReflections()
    {
        var trace = OpticalTracer.Trace(WithAngles(15, 90, 30));

        Assert.Equal(BeamOutcome.TargetHit, trace.Outcome);
        Assert.Equal([0d, 30d, 150d, 270d], trace.Path.Select(step => step.IncomingAngle).Distinct());
        Assert.Equal(OpticsLevel.Target, trace.Path[^1].Cell);
        var target = trace.Path[^1].Point;
        var dx = target.X - (OpticsLevel.Target.Column - 0.5);
        var dy = target.Y - (OpticsLevel.Target.Row - 0.5);
        Assert.Equal(OpticsLevel.TargetRadius, Math.Sqrt(dx * dx + dy * dy), 8);
    }

    [Fact]
    public void OnlyTheVerifiedLayoutWinsAcrossAllAllowedAngles()
    {
        var winningLayouts = new List<int[]>();
        foreach (var first in OpticsLevel.AllowedAngles)
        {
            foreach (var second in OpticsLevel.AllowedAngles)
            {
                foreach (var third in OpticsLevel.AllowedAngles)
                {
                    if (OpticalTracer.Trace(WithAngles(first, second, third)).Outcome == BeamOutcome.TargetHit)
                    {
                        winningLayouts.Add([first, second, third]);
                    }
                }
            }
        }

        Assert.Equal([15, 90, 30], Assert.Single(winningLayouts));
    }

    [Theory]
    [InlineData(15)]
    [InlineData(30)]
    [InlineData(45)]
    [InlineData(60)]
    [InlineData(90)]
    [InlineData(105)]
    [InlineData(120)]
    [InlineData(135)]
    [InlineData(150)]
    [InlineData(165)]
    public void EachAllowedAngleReflectsAnEastboundBeam(int angle)
    {
        var mirror = new AngledMirror(new Cell(3, 4), angle);
        var trace = OpticalTracer.Trace(7, new Cell(3, 2), 0, new Cell(1, 1), [mirror]);
        var outgoing = Assert.Single(trace.Path.Select(step => step.IncomingAngle).Distinct().Skip(1));

        Assert.Equal(2d * angle, outgoing);
    }

    [Fact]
    public void PassingThroughAMirrorCellCanMissTheFiniteMirror()
    {
        var trace = OpticalTracer.Trace(WithAngles(15, 15, 45));

        Assert.Equal(BeamOutcome.Exited, trace.Outcome);
        Assert.Contains(trace.Path, step => step.Cell == OpticsLevel.StartingMirrors[1].Cell);
        Assert.Equal([0d, 30d], trace.Path.Select(step => step.IncomingAngle).Distinct());
    }

    [Fact]
    public void NearerMirrorReflectsBeforeTheBeamCanReachTheTarget()
    {
        var trace = OpticalTracer.Trace(7, new Cell(3, 2), 0, new Cell(3, 5),
            [new AngledMirror(new Cell(3, 4), 90)]);

        Assert.Equal(BeamOutcome.Exited, trace.Outcome);
        Assert.Equal(180, trace.Path[^1].IncomingAngle);
    }

    [Fact]
    public void DiagonalCornerCrossingsEnterOnlyOneNewCell()
    {
        var trace = OpticalTracer.Trace(7, new Cell(2, 2), 45, new Cell(4, 4), []);

        Assert.Equal(BeamOutcome.TargetHit, trace.Outcome);
        Assert.Equal([new Cell(2, 2), new Cell(3, 3), new Cell(4, 4), new Cell(4, 4)],
            trace.Path.Select(step => step.Cell));
    }

    [Fact]
    public void RepeatedMirrorImpactClosesALoop()
    {
        AngledMirror[] mirrors =
        [
            new(new Cell(2, 5), 45),
            new(new Cell(5, 5), 135),
            new(new Cell(5, 2), 45),
            new(new Cell(2, 2), 135)
        ];

        var trace = OpticalTracer.Trace(6, new Cell(2, 3), 0, new Cell(6, 6), mirrors);

        Assert.Equal(BeamOutcome.Looped, trace.Outcome);
        Assert.Equal(new OpticalStep(new OpticalPoint(4.5, 1.5), new Cell(2, 5), 0), trace.Path[^1]);
        Assert.Contains(trace.Path.SkipLast(1), step => step == trace.Path[^1]);
    }

    private static AngledMirror[] WithAngles(params int[] angles) =>
        OpticsLevel.StartingMirrors.Select((mirror, index) => mirror with { Angle = angles[index] }).ToArray();
}
