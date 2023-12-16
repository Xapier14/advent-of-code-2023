using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 16);

var input = AdventOfCode.GetInputLines();

var sample =
    """
    .|...\....
    |.-.\.....
    .....|-...
    ........|.
    ..........
    .........\
    ..../.\\..
    .-.-/..|..
    .|....-|.\
    ..//.|....
    """.Split(Environment.NewLine);

Utility.Assert(Part1, sample, 46);
Utility.Assert(Part2, sample, 51);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

void StartBeam(char[][] map, int[][][] heatMaps, int startX, int startY, Direction startDirection)
{
    var queue = new Queue<(int, int, Direction)>();
    queue.Enqueue((startX, startY, startDirection));
    while (queue.Any())
    {
        var (x, y, direction) = queue.Dequeue();
        ProcessBeam(map, heatMaps, x, y, direction, queue);
    }
}

void ProcessBeam(char[][] map, int[][][] heatMap, int x, int y, Direction beamDirection, Queue<(int, int, Direction)> queue)
{
    if (x < 0 || x >= map[0].Length ||
        y < 0 || y >= map.Length)
        return;
    if (heatMap[y][x][(int)beamDirection] > 1)
        return;
    heatMap[y][x][(int)beamDirection] += 1;
    var tile = map[y][x];
    if (tile == '.')
    {
        switch (beamDirection)
        {
            case Direction.N:
                queue.Enqueue((x, y - 1, beamDirection));
                break;
            case Direction.S:
                queue.Enqueue((x, y + 1, beamDirection));
                break;
            case Direction.W:
                queue.Enqueue((x - 1, y, beamDirection));
                break;
            case Direction.E:
                queue.Enqueue((x + 1, y, beamDirection));
                break;
        }
        return;
    }

    if (tile == '-')
    {
        if (beamDirection is Direction.E or Direction.W)
        {
            switch (beamDirection)
            {
                case Direction.W:
                    queue.Enqueue((x - 1, y, beamDirection));
                    break;
                case Direction.E:
                    queue.Enqueue((x + 1, y, beamDirection));
                    break;
            }
        }
        else
        {
            queue.Enqueue((x - 1, y, Direction.W));
            queue.Enqueue((x + 1, y, Direction.E));
        }
        return;
    }

    if (tile == '|')
    {
        if (beamDirection is Direction.N or Direction.S)
        {
            switch (beamDirection)
            {
                case Direction.N:
                    queue.Enqueue((x, y - 1, beamDirection));
                    break;
                case Direction.S:
                    queue.Enqueue((x, y + 1, beamDirection));
                    break;
            }
        }
        else
        {
            queue.Enqueue((x, y - 1, Direction.N));
            queue.Enqueue((x, y + 1, Direction.S));
        }

        return;
    }

    if (tile == '\\')
    {
        switch (beamDirection)
        {
            case Direction.N:
                queue.Enqueue((x - 1, y, Direction.W));
                break;
            case Direction.S:
                queue.Enqueue((x + 1, y, Direction.E));
                break;
            case Direction.W:
                queue.Enqueue((x, y - 1, Direction.N));
                break;
            case Direction.E:
                queue.Enqueue((x, y + 1, Direction.S));
                break;
        }

        return;
    }

    if (tile == '/')
    {
        switch (beamDirection)
        {
            case Direction.N:
                queue.Enqueue((x + 1, y, Direction.E));
                break;
            case Direction.S:
                queue.Enqueue((x - 1, y, Direction.W));
                break;
            case Direction.W:
                queue.Enqueue((x, y + 1, Direction.S));
                break;
            case Direction.E:
                queue.Enqueue((x, y - 1, Direction.N));
                break;
        }

        return;
    }

    Console.WriteLine("Unknown tile.");
}

long Part1(string[] lines)
{
    var map = lines.Select(line => line.ToCharArray()).ToArray();
    var heatMap = new int[map.Length][][];
    for (var i = 0; i < map.Length; ++i)
    {
        heatMap[i] = new int[map[i].Length][];
        for (var j = 0; j < heatMap[i].Length; ++j)
        {
            var tileMap = new int[4];
            tileMap[(int)Direction.N] = 0;
            tileMap[(int)Direction.S] = 0;
            tileMap[(int)Direction.E] = 0;
            tileMap[(int)Direction.W] = 0;
            heatMap[i][j] = tileMap;
        }
    }
    StartBeam(map, heatMap, 0, 0, Direction.E);
    return heatMap.Sum(line => line.Count(tile => tile.Any(count => count > 0)));
}

long Part2(string[] lines)
{
    var map = lines.Select(line => line.ToCharArray()).ToArray();
    var heatMaps = new List<(int[][][], int, int, Direction)>();
    for (var y = 0; y < map.Length; ++y)
    {
        var heatMapE = new int[map.Length][][];
        var heatMapW = new int[map.Length][][];
        heatMaps.Add((heatMapE, 0, y, Direction.E));
        heatMaps.Add((heatMapW, map[y].Length - 1, y, Direction.W));
    }
    for (var x = 0; x < map[0].Length; ++x)
    {
        var heatMapS = new int[map.Length][][];
        var heatMapN = new int[map.Length][][];
        heatMaps.Add((heatMapS, x, 0, Direction.S));
        heatMaps.Add((heatMapN, x, map.Length - 1, Direction.N));
    }

    var results = new List<int>();
    foreach (var (heatMap, x, y, direction) in heatMaps)
    {
        for (var i = 0; i < map.Length; ++i)
        {
            heatMap[i] = new int[map[i].Length][];
            for (var j = 0; j < heatMap[i].Length; ++j)
            {
                var tileMap = new int[4];
                tileMap[(int)Direction.N] = 0;
                tileMap[(int)Direction.S] = 0;
                tileMap[(int)Direction.E] = 0;
                tileMap[(int)Direction.W] = 0;
                heatMap[i][j] = tileMap;
            }
        }
        StartBeam(map, heatMap, x, y, direction);
        var result = heatMap.Sum(line => line.Count(tile => tile.Any(count => count > 0)));
        results.Add(result);
    }
    return results.Max();
}

enum Direction {N = 0, S = 1, E = 2, W = 3};