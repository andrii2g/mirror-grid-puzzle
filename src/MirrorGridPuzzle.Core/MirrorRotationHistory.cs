namespace MirrorGridPuzzle.Core;

public readonly record struct MirrorRotation(int MirrorIndex, int PreviousAngle, int NewAngle);

public sealed class MirrorRotationHistory
{
    private readonly Stack<MirrorRotation> _undo = new();
    private readonly Stack<MirrorRotation> _redo = new();

    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    public bool Record(MirrorRotation rotation)
    {
        if (rotation.PreviousAngle == rotation.NewAngle)
        {
            return false;
        }

        _undo.Push(rotation);
        _redo.Clear();
        return true;
    }

    public MirrorRotation? Undo()
    {
        if (!_undo.TryPop(out var rotation))
        {
            return null;
        }

        _redo.Push(rotation);
        return rotation;
    }

    public MirrorRotation? Redo()
    {
        if (!_redo.TryPop(out var rotation))
        {
            return null;
        }

        _undo.Push(rotation);
        return rotation;
    }

    public void Clear()
    {
        _undo.Clear();
        _redo.Clear();
    }
}
