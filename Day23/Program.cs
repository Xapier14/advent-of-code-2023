using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 23);

var input = AdventOfCode.GetInputLines();

var sample =
"""
#.#####################
#.......#########...###
#######.#########.#.###
###.....#.>.>.###.#.###
###v#####.#v#.###.#.###
###.>...#.#.#.....#...#
###v###.#.#.#########.#
###...#.#.#.......#...#
#####.#.#.#######.#.###
#.....#.#.#.......#...#
#.#####.#.#.#########v#
#.#...#...#...###...>.#
#.#.#v#######v###.###v#
#...#.>.#...>.>.#.###.#
#####v#.#.###v#.#.###.#
#.....#...#...#.#.#...#
#.#########.###.#.#.###
#...###...#...#...#.###
###.###.#.###v#####v###
#...#...#.#.>.>.#.>.###
#.###.###.#.###.#.#v###
#.....###...###...#...#
#####################.#
""".Split(Environment.NewLine);

Utility.Assert(Part1, sample, 94);
Utility.Assert(Part2, sample, 0);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;


Segment[] GenerateGraph(char[][] map, (int X, int Y) starting, (int X, int Y) end, out Graph graph)
{
    var forwardGraph = new Dictionary<Segment, Segment[]>();
    var transposeGraph = new Dictionary<Segment, Segment>();

    var startingSegments = new List<Segment>();
    var probeQueue = new Queue<(int X, int Y, Segment? Previous)>();
    probeQueue.Enqueue((starting.X, starting.Y, null));

    while (probeQueue.TryDequeue(out var instruction))
    {
        var probedSegments = FindSegments(map, (instruction.X, instruction.Y), end);
        var actualSegments = probedSegments
            .Select(probedSegment => probedSegment.Segment)
            .ToArray();
        
        if (!startingSegments.Any())
            startingSegments.AddRange(actualSegments);
        if (instruction.Previous != null)
        {
            forwardGraph[instruction.Previous] = actualSegments;
            instruction.Previous.Next = actualSegments;
        }

        foreach (var probedSegment in probedSegments)
        {
            if (instruction.Previous != null)
                transposeGraph[probedSegment.Segment] = instruction.Previous;
            probedSegment.Segment.Previous = instruction.Previous;
            if (probedSegment.Next == end)
                continue;
            probeQueue.Enqueue((probedSegment.Next.X, probedSegment.Next.Y, probedSegment.Segment));
        }
    }

    graph = new Graph
    {
        ForwardGraph = forwardGraph,
        TransposeGraph = transposeGraph
    };
    return startingSegments.ToArray();
}

(Segment Segment, (int X, int Y) Next)[] FindSegments(char[][] map, (int X, int Y) probe, (int X, int Y) terminal)
{
    var resultingSegments = new List<(Segment Segment, (int X, int Y) Next)>();
    var queue = new Queue<((int X, int Y) Probe, long Length)>();
    var visited = new List<(int X, int Y)>();
    queue.Enqueue((probe, 1));
    visited.Add(probe);

    if (probe == terminal)
    {
        return new[]
        {
            (new Segment
            {
                Start = probe,
                End = probe,
                Length = 1,
            }, terminal)
        };
    }

    while (queue.TryDequeue(out var scan))
    {
        var validAdjacent = GetAdjacent(map, scan.Probe);
        foreach (var adjacent in validAdjacent)
        {
            if (visited.Contains(adjacent))
                continue;
            visited.Add(adjacent);
            var adjacentTile = map[adjacent.Y][adjacent.X];
            var newSegment = new Segment
            {
                Start = probe,
                End = adjacent,
                Length = scan.Length
            };
            if (adjacent == terminal)
            {
                resultingSegments.Add((newSegment, terminal));
                continue;
            }
            newSegment.Length += 1L;
            if (adjacentTile == '.')
            {
                queue.Enqueue((adjacent, scan.Length + 1L));
                continue;
            }
            switch (adjacentTile)
            {
                case '>':
                    resultingSegments.Add((newSegment, (adjacent.X + 1, adjacent.Y)));
                    break;
                case 'v':
                    resultingSegments.Add((newSegment, (adjacent.X, adjacent.Y + 1)));
                    break;
            }
        }
    }

    return resultingSegments.ToArray();
}

(int X, int Y)[] GetAdjacent(char[][] map, (int X, int Y) tile)
{
    var checkList = new (int X, int Y)[]
        { (tile.X - 1, tile.Y), (tile.X + 1, tile.Y), (tile.X, tile.Y - 1), (tile.X, tile.Y + 1) };
    var adjacent = new List<(int X, int Y)>();
    foreach (var check in checkList)
    {
        if (check.X < 0 || check.X >= map[0].Length ||
            check.Y < 0 || check.Y >= map.Length)
            continue;
        var checkTile = map[check.Y][check.X];
        if (checkTile == '.')
        {
            adjacent.Add(check);
            continue;
        }
        (int X, int Y) relative = (check.X - tile.X, check.Y - tile.Y);
        var allowedTile =
            relative.X == 1 ? '>' :
            relative.Y == 1 ? 'v' :
            'N';
        if (checkTile == allowedTile)
            adjacent.Add(check);
    }

    return adjacent.ToArray();
}

long[] CalculatePathLengths(params Segment[] segments)
{
    var lengths = new List<long>();
    var queue = new Queue<(Segment Segment, long Length)>();
    foreach (var segment in segments)
        queue.Enqueue((segment, 0L));
    while (queue.TryDequeue(out var segment))
    {
        var updatedLength = segment.Segment.Length + segment.Length;
        if (segment.Segment.Next == null)
        {
            lengths.Add(updatedLength);
            continue;
        }
        foreach (var nextSegment in segment.Segment.Next)
        {
            queue.Enqueue((nextSegment, updatedLength));
        }
    }

    return lengths.ToArray();
}

long Part1(string[] lines)
{
    var map = lines.Select(line => line.ToCharArray()).ToArray();
    var starting = (1, 0);
    var end = (map[^1].Length - 2, map.Length - 1);
    var startingSegments = GenerateGraph(map, starting, end, out var graph);
    var pathLengths = CalculatePathLengths(startingSegments);
    return pathLengths.Max();
}

long Part2(string[] lines)
{
    return 0;
}

public class Segment
{
    public (int X, int Y) Start { get; set; }
    public (int X, int Y) End { get; set; }
    public long Length { get; set; }
    public Segment? Previous { get; set; }
    public Segment[]? Next { get; set; }

    public Segment Clone()
        => new Segment
        {
            Start = Start,
            End = End,
            Length = Length,
            Previous = Previous,
            Next = Next
        };
}

public class Graph
{
    public Dictionary<Segment, Segment[]> ForwardGraph { get; init; }
    public Dictionary<Segment, Segment> TransposeGraph { get; init; }
}