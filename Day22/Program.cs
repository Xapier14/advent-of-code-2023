using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 22);

var input = AdventOfCode.GetInputLines();

var sample =
    """
    1,0,1~1,2,1
    0,0,2~2,0,2
    0,2,3~2,2,3
    0,0,4~0,2,4
    2,0,5~2,2,5
    0,1,6~2,1,6
    1,1,8~1,1,9
    """.Split(Environment.NewLine);

Utility.Assert(Part1, sample, 5);
Utility.Assert(Part2, sample, 7);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

SortedList<int, List<Brick>> MakeBricksFall(SortedList<int, List<Brick>> floatingBricks)
{
    var currentState = new SortedList<int, List<Brick>>();
    foreach (var (z, brickLayer) in floatingBricks)
    {
        // if first layer, copy layer over to currentState
        if (z == 1)
        {
            // create brick layer in currentState if it does not exist
            if (!currentState.TryGetValue(z, out var currentLayer))
            {
                currentState[z] = new List<Brick>();
                currentLayer = currentState[z];
            }
            currentLayer.AddRange(brickLayer);
            continue;
        }
        // not first layer, check collision of brick while moving down
        foreach (var brick in brickLayer)
        {
            var placementZ = 1;
            var verticalCollision = false;
            var checkZ = 1;
            // start checking from the first layer up until last layer or floating z
            for (; checkZ <= z;)
            {
                if (!currentState.ContainsKey(checkZ))
                {
                    if (verticalCollision)
                    {
                        placementZ = checkZ;
                        verticalCollision = false;
                    }
                    checkZ++;
                    continue;
                }

                var checkLayer = currentState[checkZ];
                var collidingBricks = checkLayer
                    .Where(checkBrick => XCheckCondition(checkBrick) && YCheckCondition(checkBrick)).ToArray();

                // if no collision, this is our placementZ
                if (!collidingBricks.Any())
                {
                    if (verticalCollision)
                    {
                        placementZ = checkZ;
                        verticalCollision = false;
                    }
                    checkZ++;
                    continue;
                }

                verticalCollision = true;
                var collidingBricksCount = collidingBricks.Length;
                var collidingBricksMax = collidingBricks.Select(collidingBrick => collidingBrick.Height).Max();
                // if collision x or y, make checkZ the top part of the highest brick colliding with
                checkZ += collidingBricksMax;
                continue;

                bool XCheckCondition(Brick checkBrick) =>
                    checkBrick.OccupyingX.Any(checkX => brick.OccupyingX.Any(x => checkX == x));

                bool YCheckCondition(Brick checkBrick) =>
                    checkBrick.OccupyingY.Any(checkY => brick.OccupyingY.Any(y => checkY == y));
            }
            
            if (verticalCollision)
            {
                placementZ = checkZ;
            }

            if (!currentState.TryGetValue(placementZ, out var placementLayer))
            {
                currentState[placementZ] = new List<Brick>();
                placementLayer = currentState[placementZ];
            }

            var brickCopy = new Brick
            {
                Origin = (brick.Origin.X, brick.Origin.Y, placementZ),
                End = (brick.End.X, brick.End.Y, placementZ + brick.Height - 1),
                Label = brick.Label
            };
            placementLayer.Add(brickCopy);
        }
    }

    // filter currentState by removing layers with no bricks
    var stableState = new SortedList<int, List<Brick>>();
    foreach (var (z, brickLayer) in currentState)
        if (brickLayer.Any())
            stableState.Add(z, brickLayer);

    return stableState;
}

Brick[] GetSafeBricks(SortedList<int, List<Brick>> stableBricks, bool computeChain, out long chain)
{
    var safeBricks = new HashSet<Brick>();
    var supports = new Dictionary<Brick, Brick[]>();
    
    var unsafeBricks = new HashSet<Brick>();
    var reversedStableBricks = stableBricks.Reverse().Select(kp =>
    {
        (int Z, List<Brick> BrickLayer) tuple = (kp.Key, kp.Value);
        return tuple;
    }).ToArray();
    // top bricks
    foreach (var firstBrick in reversedStableBricks[0].BrickLayer)
    {
        safeBricks.Add(firstBrick);
        supports[firstBrick] = Array.Empty<Brick>();
    }

    for (var i = 1; i < reversedStableBricks.Length; ++i)
    {
        var (_, brickLayer) = reversedStableBricks[i];
        // for each brick in this brick layer
        foreach (var brick in brickLayer)
        {
            // check if another brick is on top of this brick in the layer above
            bool XCheckCondition(Brick checkBrick) =>
                checkBrick.OccupyingX.Any(checkX => brick.OccupyingX.Any(x => checkX == x));
            bool YCheckCondition(Brick checkBrick) =>
                checkBrick.OccupyingY.Any(checkY => brick.OccupyingY.Any(y => checkY == y));
            bool CheckCombinedCondition(Brick checkBrick) =>
                XCheckCondition(checkBrick) && YCheckCondition(checkBrick);

            var zAbove = i - brick.Height;

            // no layers above brick
            if (zAbove < 0)
            {
                safeBricks.Add(brick);
                supports[brick] = Array.Empty<Brick>();
                continue;
            }
            var bricksOnLayerAbove = reversedStableBricks[zAbove].BrickLayer.Where(CheckCombinedCondition).ToArray();
            
            // no bricks on top of this brick, it is safe
            if (!bricksOnLayerAbove.Any())
            {
                safeBricks.Add(brick);
                supports[brick] = Array.Empty<Brick>();
                continue;
            }

            supports[brick] = bricksOnLayerAbove;

            var safeToRemove = true;
            foreach (var brickOnLayerAbove in bricksOnLayerAbove)
            {
                var hasOtherSupportingBrick = false;
                for (var z = zAbove + 1; z < reversedStableBricks.Length && !hasOtherSupportingBrick; ++z)
                {
                    var heightRequirement = z - zAbove;
                    var bricks = reversedStableBricks[z].BrickLayer;
                    hasOtherSupportingBrick = bricks.Any(brickCheck =>
                        brick != brickCheck &&
                        brickCheck.Height == heightRequirement &&
                        brickCheck.OccupyingX.Any(x => brickOnLayerAbove.OccupyingX.Any(bX => x == bX)) &&
                        brickCheck.OccupyingY.Any(y => brickOnLayerAbove.OccupyingY.Any(bY => y == bY)));
                }

                if (!hasOtherSupportingBrick)
                {
                    safeToRemove = false;
                    break;
                }
            }

            if (safeToRemove)
                safeBricks.Add(brick);
            else
                unsafeBricks.Add(brick);
        }
    }
    var dependencies = new Dictionary<Brick, List<Brick>>();
    foreach (var (supporter, dependents) in supports)
    {
        foreach (var dependent in dependents)
        {
            if (!dependencies.TryGetValue(dependent, out var dependencyList))
            {
                dependencyList = new List<Brick>();
                dependencies[dependent] = dependencyList;
            }
            dependencyList.Add(supporter);
        }
    }

    var dependencyTree = new Dictionary<Brick, Brick[]>();
    foreach (var kp in dependencies)
        dependencyTree[kp.Key] = kp.Value.ToArray();
    chain = 0L;
    if (!computeChain)
        return safeBricks.ToArray();
    var counter = 1;
    foreach (var cause in unsafeBricks)
    {
        Console.WriteLine("Traversing {0} ({1}/{2})", cause.Label, counter, unsafeBricks.Count);
        chain += ChainReaction(cause, supports, dependencyTree);
        counter++;
    }

    return safeBricks.ToArray();
}

long ChainReaction(Brick node, Dictionary<Brick, Brick[]> graph, Dictionary<Brick, Brick[]> dependencies)
{
    var removed = new HashSet<Brick>();
    var queue = new Queue<Brick>();
    removed.Add(node);
    var supporting = graph[node];
    foreach (var supported in supporting)
        queue.Enqueue(supported);
    while (queue.TryDequeue(out var curNode))
    {
        var dependencyList = dependencies[curNode];
        var canBeRemoved = dependencyList.All(item => removed.Any(i => i == item));
        if (!canBeRemoved)
            continue;
        removed.Add(curNode);
        foreach (var child in graph[curNode])
            queue.Enqueue(child);
    }
    return removed.Count - 1;
}

long Part1(string[] lines)
{
    var sortedBricks = new SortedList<int, List<Brick>>();
    var bricks = lines.Select(line =>
    {
        var dim1 = line.Split('~')[0].Split(',').Select(int.Parse).ToArray();
        var dim2 = line.Split('~')[1].Split(',').Select(int.Parse).ToArray();
        (int X, int Y, int Z) point1 = (dim1[0], dim1[1], dim1[2]);
        (int X, int Y, int Z) point2 = (dim2[0], dim2[1], dim2[2]);
        ((int X, int Y, int Z) Origin, (int X, int Y, int Z) End) ret = (point1, point2);
        return ret;
    });
    var label = 'A';
    foreach (var (origin, end) in bricks)
    {
        if (!sortedBricks.TryGetValue(origin.Z, out var brickList))
        {
            sortedBricks[origin.Z] = new List<Brick>();
            brickList = sortedBricks[origin.Z];
        }

        var brick = new Brick
        {
            Label = $"{label}",
            Origin = origin,
            End = end
        };
        brick.RecalculateOccupyingArrays();

        brickList.Add(brick);
        label++;
    }
    var fallenBricks = MakeBricksFall(sortedBricks);
    var safeBricks = GetSafeBricks(fallenBricks, false, out _);
    return safeBricks.Length;
}

long Part2(string[] lines)
{
    var sortedBricks = new SortedList<int, List<Brick>>();
    var bricks = lines.Select(line =>
    {
        var dim1 = line.Split('~')[0].Split(',').Select(int.Parse).ToArray();
        var dim2 = line.Split('~')[1].Split(',').Select(int.Parse).ToArray();
        (int X, int Y, int Z) point1 = (dim1[0], dim1[1], dim1[2]);
        (int X, int Y, int Z) point2 = (dim2[0], dim2[1], dim2[2]);
        ((int X, int Y, int Z) Origin, (int X, int Y, int Z) End) ret = (point1, point2);
        return ret;
    });
    var label = 'A';
    foreach (var (origin, end) in bricks)
    {
        if (!sortedBricks.TryGetValue(origin.Z, out var brickList))
        {
            sortedBricks[origin.Z] = new List<Brick>();
            brickList = sortedBricks[origin.Z];
        }

        var brick = new Brick
        {
            Label = $"{label}",
            Origin = origin,
            End = end
        };
        brick.RecalculateOccupyingArrays();

        brickList.Add(brick);
        label++;
    }
    var fallenBricks = MakeBricksFall(sortedBricks);
    _ = GetSafeBricks(fallenBricks, true, out var chainReaction);

    return chainReaction;
}

public struct Brick
{
    private (int X, int Y, int Z)? _origin, _end;
    private int[] _occupyingX, _occupyingY, _occupyingZ;
    public string? Label { get; set; }

    public (int X, int Y, int Z) Origin
    {
        readonly get => _origin!.Value;
        set
        {
            _origin = value;
            RecalculateOccupyingArrays();
        }
    }

    public (int X, int Y, int Z) End
    {
        readonly get => _end!.Value;
        set
        {
            _end = value;
            RecalculateOccupyingArrays();
        }
    }

    public readonly int[] OccupyingX => _occupyingX;
    public readonly int[] OccupyingY => _occupyingY;
    public readonly int[] OccupyingZ => _occupyingZ;
    
    public readonly int WidthX => _occupyingX.Length;
    public readonly int WidthY => _occupyingY.Length;
    public readonly int Height => _occupyingZ.Length;

    public static bool operator ==(Brick left, Brick right)
    {
        return left.Origin == right.Origin &&
               left.End == right.End;
    }

    public static bool operator !=(Brick left, Brick right)
    {
        return left.Origin != right.Origin ||
               left.End != right.End;
    }

    public override string ToString()
    {
        return Label != null
            ? $"{Label} @ ({Origin.X}, {Origin.Y}, {Origin.Z})"
            : $"{Origin.X}, {Origin.Y}, {Origin.Z}";
    }

    public void RecalculateOccupyingArrays()
    {
        if (!_origin.HasValue || !_end.HasValue)
            return;
        _occupyingX = new int[_end.Value.X - _origin.Value.X + 1];
        _occupyingY = new int[_end.Value.Y - _origin.Value.Y + 1];
        _occupyingZ = new int[_end.Value.Z - _origin.Value.Z + 1];
        for (var i = _origin.Value.X; i <= _end.Value.X; ++i)
            _occupyingX[i - _origin.Value.X] = i;
        for (var i = _origin.Value.Y; i <= _end.Value.Y; ++i)
            _occupyingY[i - _origin.Value.Y] = i;
        for (var i = _origin.Value.Z; i <= _end.Value.Z; ++i)
            _occupyingZ[i - _origin.Value.Z] = i;
    }
}