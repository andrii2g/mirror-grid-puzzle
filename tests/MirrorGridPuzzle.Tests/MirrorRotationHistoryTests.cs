using MirrorGridPuzzle.Core;
using Xunit;

namespace MirrorGridPuzzle.Tests;

public sealed class MirrorRotationHistoryTests
{
    [Fact]
    public void UndoAndRedoPreserveTheOrderOfRotations()
    {
        var history = new MirrorRotationHistory();
        var first = new MirrorRotation(0, -45, 45);
        var second = new MirrorRotation(2, 25, 30);
        var third = new MirrorRotation(0, 45, -45);

        Assert.True(history.Record(first));
        Assert.True(history.Record(second));
        Assert.True(history.Record(third));
        Assert.True(history.CanUndo);
        Assert.False(history.CanRedo);

        Assert.Equal(third, history.Undo());
        Assert.Equal(second, history.Undo());
        Assert.Equal(first, history.Undo());
        Assert.Null(history.Undo());
        Assert.False(history.CanUndo);
        Assert.True(history.CanRedo);

        Assert.Equal(first, history.Redo());
        Assert.Equal(second, history.Redo());
        Assert.Equal(third, history.Redo());
        Assert.Null(history.Redo());
        Assert.True(history.CanUndo);
        Assert.False(history.CanRedo);
    }

    [Fact]
    public void ANewRotationAfterUndoDiscardsTheRedoBranch()
    {
        var history = new MirrorRotationHistory();
        var first = new MirrorRotation(0, 30, 35);
        var second = new MirrorRotation(1, 45, 40);
        var alternative = new MirrorRotation(1, 45, 60);

        history.Record(first);
        history.Record(second);
        history.Undo();

        Assert.True(history.Record(alternative));
        Assert.False(history.CanRedo);
        Assert.Null(history.Redo());
        Assert.Equal(alternative, history.Undo());
        Assert.Equal(first, history.Undo());
    }

    [Fact]
    public void RecordingAnUnchangedAnglePreservesRedo()
    {
        var history = new MirrorRotationHistory();
        var rotation = new MirrorRotation(2, 30, 35);
        var unchanged = new MirrorRotation(2, 30, 30);

        Assert.False(history.Record(unchanged));
        Assert.False(history.CanUndo);
        history.Record(rotation);
        history.Undo();

        Assert.False(history.Record(unchanged));
        Assert.False(history.CanUndo);
        Assert.True(history.CanRedo);
        Assert.Equal(rotation, history.Redo());
    }

    [Fact]
    public void ClearDiscardsUndoAndRedo()
    {
        var history = new MirrorRotationHistory();
        var rotation = new MirrorRotation(0, -45, 45);

        history.Record(rotation);
        history.Record(new MirrorRotation(1, 30, 35));
        history.Undo();

        history.Clear();

        Assert.False(history.CanUndo);
        Assert.False(history.CanRedo);
        Assert.Null(history.Undo());
        Assert.Null(history.Redo());
        Assert.True(history.Record(rotation));
        Assert.Equal(rotation, history.Undo());
    }
}
