using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 17);

var input = AdventOfCode.GetInputLines();

var sample =
    """
    2413432311323
    3215453535623
    3255245654254
    3446585845452
    4546657867536
    1438598798454
    4457876987766
    3637877979653
    4654967986887
    4564679986453
    1224686865563
    2546548887735
    4322674655533
    """.Split(Environment.NewLine);

Utility.Assert(Part1, sample, 102);
Utility.Assert(Part2, sample, 94);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

long Dijkstra(int[][] map, int finishX, int finishY, int minTurn, int maxRun)
{
    var dist = new Dictionary<(int X, int Y, Direction Direction, int Run), int>();
    var queue = new PriorityQueue<(int X, int Y, Direction Direction, int Run), int>();
    queue.Enqueue((0, 0, Direction.Right, 0), 0);
    queue.Enqueue((0, 0, Direction.Down, 0), 0);
    dist.Add((0, 0, Direction.Right, 0), 0);
    dist.Add((0, 0, Direction.Down, 0), 0);

    var directions = new[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

    while (queue.TryDequeue(out var vertex, out var priority))
    {
        var (x, y, curDir, run) = vertex;
        if (x == finishX && y == finishY)
        {
            return priority;
        }

        foreach (var direction in directions)
        {
            if (direction == Direction.Up && curDir == Direction.Down)
                continue;
            if (direction == Direction.Down && curDir == Direction.Up)
                continue;
            if (direction == Direction.Left && curDir == Direction.Right)
                continue;
            if (direction == Direction.Right && curDir == Direction.Left)
                continue;
            if (run < minTurn && direction != curDir)
                continue;
            if (direction == curDir && run >= maxRun)
                continue;
            var nextX = direction switch
            {
                Direction.Left => x - 1,
                Direction.Right => x + 1,
                _ => x
            };
            var nextY = direction switch
            {
                Direction.Up => y - 1,
                Direction.Down => y + 1,
                _ => y
            };
            if (nextY < 0 || nextY >= map.Length ||
                nextX < 0 || nextX >= map[nextY].Length)
                continue;
            var nextVertex = (nextX, nextY, direction, direction == curDir ? run + 1 : 1);
            var localHeat = dist[vertex] + map[nextY][nextX];

            if (localHeat < dist.GetValueOrDefault(nextVertex, int.MaxValue))
            {
                dist[nextVertex] = localHeat;
                queue.Enqueue(nextVertex, localHeat);
            }
        }
    }

    return long.MinValue;
}

long Part1(string[] lines)
{
    var map = lines.Select(line => line.ToCharArray().Select(c => c - '0').ToArray()).ToArray();
    return Dijkstra(map, map[0].Length - 1, map.Length - 1, 0, 3);
}

long Part2(string[] lines)
{
    var map = lines.Select(line => line.ToCharArray().Select(c => c - '0').ToArray()).ToArray();
    return Dijkstra(map, map[0].Length - 1, map.Length - 1, 4, 10);
}

enum Direction { Up, Down, Left, Right }