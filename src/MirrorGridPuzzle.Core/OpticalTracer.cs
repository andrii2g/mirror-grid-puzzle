namespace MirrorGridPuzzle.Core;

public readonly record struct OpticalPoint(double X, double Y)
{
    public static OpticalPoint operator +(OpticalPoint left, OpticalPoint right) =>
        new(left.X + right.X, left.Y + right.Y);

    public static OpticalPoint operator -(OpticalPoint left, OpticalPoint right) =>
        new(left.X - right.X, left.Y - right.Y);

    public static OpticalPoint operator *(OpticalPoint point, double scale) =>
        new(point.X * scale, point.Y * scale);
}

public readonly record struct OpticalStep(OpticalPoint Point, Cell Cell, double IncomingAngle);

public sealed record OpticalTrace(BeamOutcome Outcome, IReadOnlyList<OpticalStep> Path);

public static class OpticalTracer
{
    // Coordinates begin at the upper-left grid edge. Angles increase clockwise from east.
    private const double Epsilon = 1e-9;
    private const int MaxReflections = 256;

    public static OpticalTrace Trace(IReadOnlyList<AngledMirror> mirrors) =>
        Trace(OpticsLevel.Size, OpticsLevel.Source, OpticsLevel.LaunchAngle, OpticsLevel.Target, mirrors);

    public static OpticalTrace Trace(int size, Cell source, double launchAngle, Cell target,
        IReadOnlyList<AngledMirror> mirrors) =>
        Trace(size, source, launchAngle, target, mirrors, OpticsLevel.AllowedAngles);

    public static OpticalTrace Trace(OpticalBoard board, OpticalBeamRoute beam, IReadOnlyList<AngledMirror> mirrors)
    {
        ArgumentNullException.ThrowIfNull(board);
        ArgumentNullException.ThrowIfNull(beam);
        return Trace(board.Size, beam.Source, beam.LaunchAngle, beam.Target, mirrors, board.AllowedAngles);
    }

    private static OpticalTrace Trace(int size, Cell source, double launchAngle, Cell target,
        IReadOnlyList<AngledMirror> mirrors, IReadOnlyList<int> allowedAngles)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);
        ArgumentNullException.ThrowIfNull(mirrors);
        if (!IsOnBoard(source, size))
        {
            throw new ArgumentOutOfRangeException(nameof(source));
        }
        if (!IsOnBoard(target, size))
        {
            throw new ArgumentOutOfRangeException(nameof(target));
        }
        if (!double.IsFinite(launchAngle))
        {
            throw new ArgumentOutOfRangeException(nameof(launchAngle));
        }

        var occupiedCells = new HashSet<Cell> { source, target };
        var segments = new List<(AngledMirror Mirror, OpticalPoint Center, OpticalPoint Axis)>();
        foreach (var mirror in mirrors)
        {
            if (!IsOnBoard(mirror.Cell, size) || !occupiedCells.Add(mirror.Cell) ||
                !allowedAngles.Contains(mirror.Angle))
            {
                throw new ArgumentException("Mirrors must occupy distinct free cells and use an allowed angle.",
                    nameof(mirrors));
            }

            segments.Add((mirror, Center(mirror.Cell), DirectionVector(mirror.Angle)));
        }

        var origin = Center(source);
        var targetCenter = Center(target);
        var angle = Normalize(launchAngle);
        var path = new List<OpticalStep> { new(origin, source, angle) };
        if (source == target)
        {
            return new OpticalTrace(BeamOutcome.TargetHit, path.AsReadOnly());
        }

        var impacts = new List<OpticalStep>();
        for (var reflection = 0; reflection < MaxReflections; reflection++)
        {
            var direction = DirectionVector(angle);
            var distance = ExitDistance(origin, direction, size);
            var targetDistance = TargetDistance(origin, direction, targetCenter);
            var hitsTarget = targetDistance < distance;
            if (hitsTarget)
            {
                distance = targetDistance;
            }

            AngledMirror? hitMirror = null;
            foreach (var segment in segments)
            {
                var mirrorDistance = MirrorDistance(origin, direction, segment.Center, segment.Axis);
                if (mirrorDistance < distance)
                {
                    distance = mirrorDistance;
                    hitMirror = segment.Mirror;
                }
            }

            AppendCellEntries(path, origin, direction, angle, distance, size);
            var endpoint = origin + direction * distance;
            if (hitMirror is null)
            {
                // Use the last in-bounds cell when the endpoint is on the board edge.
                var endCell = hitsTarget ? target : path[^1].Cell;
                path.Add(new OpticalStep(endpoint, endCell, angle));
                return new OpticalTrace(hitsTarget ? BeamOutcome.TargetHit : BeamOutcome.Exited, path.AsReadOnly());
            }

            var impact = new OpticalStep(endpoint, hitMirror.Cell, angle);
            var repeated = impacts.FindIndex(previous => SameImpact(previous, impact));
            if (repeated >= 0)
            {
                path.Add(impacts[repeated]);
                return new OpticalTrace(BeamOutcome.Looped, path.AsReadOnly());
            }

            path.Add(impact);
            impacts.Add(impact);
            origin = endpoint;
            angle = Normalize(2 * hitMirror.Angle - angle);
        }

        return new OpticalTrace(BeamOutcome.TraceLimit, path.AsReadOnly());
    }

    private static double ExitDistance(OpticalPoint origin, OpticalPoint direction, int size) => Math.Min(
        DistanceToEdge(origin.X, direction.X, size), DistanceToEdge(origin.Y, direction.Y, size));

    private static double DistanceToEdge(double coordinate, double component, int size)
    {
        if (component == 0)
        {
            return double.PositiveInfinity;
        }

        return (component > 0 ? size - coordinate : -coordinate) / component;
    }

    private static double MirrorDistance(OpticalPoint origin, OpticalPoint direction, OpticalPoint center,
        OpticalPoint axis)
    {
        var determinant = Cross(direction, axis);
        if (Math.Abs(determinant) <= Epsilon)
        {
            return double.PositiveInfinity;
        }

        var offset = center - origin;
        var distance = Cross(offset, axis) / determinant;
        var alongMirror = Cross(offset, direction) / determinant;
        return distance > Epsilon && Math.Abs(alongMirror) <= OpticsLevel.MirrorHalfLength + Epsilon
            ? distance
            : double.PositiveInfinity;
    }

    private static double TargetDistance(OpticalPoint origin, OpticalPoint direction, OpticalPoint center)
    {
        var offset = center - origin;
        var perpendicular = Cross(offset, direction);
        var radius = OpticsLevel.TargetRadius;
        var overlap = radius * radius - perpendicular * perpendicular;
        if (overlap < 0)
        {
            return double.PositiveInfinity;
        }

        var projection = offset.X * direction.X + offset.Y * direction.Y;
        var distance = projection - Math.Sqrt(overlap);
        return distance > Epsilon ? distance : double.PositiveInfinity;
    }

    private static void AppendCellEntries(List<OpticalStep> path, OpticalPoint origin, OpticalPoint direction,
        double angle, double distance, int size)
    {
        var crossings = new List<double>();
        for (var line = 1; line < size; line++)
        {
            AddCrossing(origin.X, direction.X);
            AddCrossing(origin.Y, direction.Y);

            void AddCrossing(double coordinate, double component)
            {
                if (component == 0)
                {
                    return;
                }

                var crossing = (line - coordinate) / component;
                if (crossing > Epsilon && crossing < distance - Epsilon)
                {
                    crossings.Add(crossing);
                }
            }
        }

        crossings.Sort();
        var lastCrossing = 0d;
        foreach (var crossing in crossings)
        {
            // A corner crossing enters the diagonal cell once.
            if (crossing - lastCrossing <= Epsilon)
            {
                continue;
            }

            var point = origin + direction * crossing;
            var interior = point + direction * (4 * Epsilon);
            var cell = new Cell((int)Math.Floor(interior.Y) + 1, (int)Math.Floor(interior.X) + 1);
            path.Add(new OpticalStep(point, cell, angle));
            lastCrossing = crossing;
        }
    }

    private static bool IsOnBoard(Cell cell, int size) =>
        cell.Row >= 1 && cell.Row <= size && cell.Column >= 1 && cell.Column <= size;

    private static OpticalPoint Center(Cell cell) => new(cell.Column - 0.5, cell.Row - 0.5);

    private static double Cross(OpticalPoint left, OpticalPoint right) => left.X * right.Y - left.Y * right.X;

    private static OpticalPoint DirectionVector(double angle)
    {
        var radians = angle * (Math.PI / 180);
        var x = Math.Cos(radians);
        var y = Math.Sin(radians);
        return new(Math.Abs(x) <= Epsilon ? 0 : x, Math.Abs(y) <= Epsilon ? 0 : y);
    }

    private static double Normalize(double angle) => (angle % 360 + 360) % 360;

    private static bool SameImpact(OpticalStep previous, OpticalStep current)
    {
        var angleDifference = Math.Abs(previous.IncomingAngle - current.IncomingAngle);
        return previous.Cell == current.Cell &&
            Math.Abs(previous.Point.X - current.Point.X) <= Epsilon &&
            Math.Abs(previous.Point.Y - current.Point.Y) <= Epsilon &&
            (angleDifference <= Epsilon || angleDifference >= 360 - Epsilon);
    }
}
