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

void StartBeam(char[][] map, bool[][][] heatMaps, int startX, int startY, Direction startDirection)
{
    var queue = new Queue<(int, int, Direction)>();
    queue.Enqueue((startX, startY, startDirection));
    while (queue.Any())
    {
        var (x, y, direction) = queue.Dequeue();
        ProcessBeam(map, heatMaps, x, y, direction, queue);
    }
}

void ProcessBeam(char[][] map, bool[][][] heatMap, int x, int y, Direction beamDirection, Queue<(int, int, Direction)> queue)
{
    if (x < 0 || x >= map[0].Length ||
        y < 0 || y >= map.Length)
        return;
    if (heatMap[y][x][(int)beamDirection])
        return;
    heatMap[y][x][(int)beamDirection] = true;
    var tile = map[y][x];
    switch (tile)
    {
        case '.':
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
        case '-':
            switch (beamDirection)
            {
                case Direction.W:
                    queue.Enqueue((x - 1, y, beamDirection));
                    break;
                case Direction.E:
                    queue.Enqueue((x + 1, y, beamDirection));
                    break;
                default: 
                    queue.Enqueue((x - 1, y, Direction.W));
                    queue.Enqueue((x + 1, y, Direction.E));
                    break;
            }
            return;
        case '|':
            switch (beamDirection)
            {
                case Direction.N:
                    queue.Enqueue((x, y - 1, beamDirection));
                    break;
                case Direction.S:
                    queue.Enqueue((x, y + 1, beamDirection));
                    break;
                default:
                    queue.Enqueue((x, y - 1, Direction.N));
                    queue.Enqueue((x, y + 1, Direction.S));
                    break;
            }
            return;
        case '\\':
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
        case '/':
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
        default:
            Console.WriteLine("Unknown tile.");
            break;
    }
}

long Part1(string[] lines)
{
    var map = lines.Select(line => line.ToCharArray()).ToArray();
    var heatMap = new bool[map.Length][][];
    for (var i = 0; i < map.Length; ++i)
    {
        heatMap[i] = new bool[map[i].Length][];
        for (var j = 0; j < heatMap[i].Length; ++j)
        {
            var tileMap = new bool[4];
            heatMap[i][j] = tileMap;
        }
    }
    StartBeam(map, heatMap, 0, 0, Direction.E);
    return heatMap.Sum(line => line.Count(tile => tile.Any(status => status)));
}

long Part2(string[] lines)
{
    var map = lines.Select(line => line.ToCharArray()).ToArray();
    var heatMaps = new List<(bool[][][], int, int, Direction)>();
    for (var y = 0; y < map.Length; ++y)
    {
        var heatMapE = new bool[map.Length][][];
        var heatMapW = new bool[map.Length][][];
        heatMaps.Add((heatMapE, 0, y, Direction.E));
        heatMaps.Add((heatMapW, map[y].Length - 1, y, Direction.W));
    }
    for (var x = 0; x < map[0].Length; ++x)
    {
        var heatMapS = new bool[map.Length][][];
        var heatMapN = new bool[map.Length][][];
        heatMaps.Add((heatMapS, x, 0, Direction.S));
        heatMaps.Add((heatMapN, x, map.Length - 1, Direction.N));
    }

    var results = new List<int>();
    foreach (var (heatMap, x, y, direction) in heatMaps)
    {
        for (var i = 0; i < map.Length; ++i)
        {
            heatMap[i] = new bool[map[i].Length][];
            for (var j = 0; j < heatMap[i].Length; ++j)
            {
                var tileMap = new bool[4];
                heatMap[i][j] = tileMap;
            }
        }
        StartBeam(map, heatMap, x, y, direction);
        var result = heatMap.Sum(line => line.Count(tile => tile.Any(status => status)));
        results.Add(result);
    }
    return results.Max();
}

enum Direction {N = 0, S = 1, E = 2, W = 3};