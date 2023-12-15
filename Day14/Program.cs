using System.Text;
using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 14);

var input = AdventOfCode.GetInputLines();

var sample =
    """
    O....#....
    O.OO#....#
    .....##...
    OO.#O....O
    .O.....O#.
    O.#..O.#.#
    ..O..#O..O
    .......O..
    #....###..
    #OO..#....
    """.Split(Environment.NewLine);

var cache = new Dictionary<string, char[][]>();

Utility.Assert(Part1, sample, 136);
Utility.Assert(Part2, sample, 64);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

string Hash(char[][] map)
{
    var sb = new StringBuilder();
    foreach (var line in map)
        sb.Append(line);
    return sb.ToString();
}

char[][] Cycle(char[][] map)
{
    var hash = Hash(map);
    if (cache.TryGetValue(hash, out var ret))
        return ret;
    ret = new char[map.Length][];
    for (var y = 0; y < map.Length; ++y)
    {
        ret[y] = new char[map[y].Length];
        for (var x = 0; x < map[y].Length; ++x)
            ret[y][x] = map[y][x];
    }
    ShiftRocksNorth(ret);
    ShiftRocksWest(ret);
    ShiftRocksSouth(ret);
    ShiftRocksEast(ret);
    cache.Add(hash, ret);
    return ret;
}

void ShiftRocksNorth(char[][] map)
{
    var segments = new List<(int X, int Offset, int Count)>();
    for (var x = 0; x < map[0].Length; ++x)
    {
        var offset = 0;
        var count = 0;
        for (var y = 0; y < map.Length; ++y)
        {
            var token = map[y][x];
            if (token == '.')
                continue;
            if (token == '#')
            {
                segments.Add((x, offset, count));
                offset = y + 1;
                count = 0;
                continue;
            }
            count++;
        }
        if (count > 0)
            segments.Add((x, offset, count));
    }

    for (var x = 0; x < map[0].Length; ++x)
    {
        for (var y = 0; y < map.Length; ++y)
        {
            if (map[y][x] == '#')
                continue;
            var isMovedRock = segments.Any(segment =>
                x == segment.X && y >= segment.Offset && y < segment.Offset + segment.Count
            );
            map[y][x] = isMovedRock ? 'O' : '.';
        }
    }
}

void ShiftRocksSouth(char[][] map)
{
    var segments = new List<(int X, int Offset, int Count)>();
    for (var x = 0; x < map[0].Length; ++x)
    {
        var offset = map.Length - 1;
        var count = 0;
        for (var y = map.Length - 1; y >= 0; --y)
        {
            var token = map[y][x];
            if (token == '.')
                continue;
            if (token == '#')
            {
                segments.Add((x, offset, count));
                offset = y - 1;
                count = 0;
                continue;
            }
            count++;
        }
        if (count > 0)
            segments.Add((x, offset, count));
    }

    for (var x = 0; x < map[0].Length; ++x)
    {
        for (var y = map.Length - 1; y >= 0; --y)
        {
            if (map[y][x] == '#')
                continue;
            var isMovedRock = segments.Any(segment =>
                x == segment.X && y <= segment.Offset && y > segment.Offset - segment.Count
            );
            map[y][x] = isMovedRock ? 'O' : '.';
        }
    }
}

void ShiftRocksEast(char[][] map)
{
    var segments = new List<(int X, int Offset, int Count)>();
    for (var x = 0; x < map[0].Length; ++x)
    {
        var offset = map.Length - 1;
        var count = 0;
        for (var y = map.Length - 1; y >= 0; --y)
        {
            var token = map[x][y];
            if (token == '.')
                continue;
            if (token == '#')
            {
                segments.Add((x, offset, count));
                offset = y - 1;
                count = 0;
                continue;
            }
            count++;
        }
        if (count > 0)
            segments.Add((x, offset, count));
    }

    for (var x = 0; x < map[0].Length; ++x)
    {
        for (var y = map.Length - 1; y >= 0; --y)
        {
            if (map[x][y] == '#')
                continue;
            var isMovedRock = segments.Any(segment =>
                x == segment.X && y <= segment.Offset && y > segment.Offset - segment.Count
            );
            map[x][y] = isMovedRock ? 'O' : '.';
        }
    }
}
void ShiftRocksWest(char[][] map)
{
    var segments = new List<(int X, int Offset, int Count)>();
    for (var x = 0; x < map[0].Length; ++x)
    {
        var offset = 0;
        var count = 0;
        for (var y = 0; y < map.Length; ++y)
        {
            var token = map[x][y];
            if (token == '.')
                continue;
            if (token == '#')
            {
                segments.Add((x, offset, count));
                offset = y + 1;
                count = 0;
                continue;
            }
            count++;
        }
        if (count > 0)
            segments.Add((x, offset, count));
    }

    for (var x = 0; x < map[0].Length; ++x)
    {
        for (var y = 0; y < map.Length; ++y)
        {
            if (map[x][y] == '#')
                continue;
            var isMovedRock = segments.Any(segment =>
                x == segment.X && y >= segment.Offset && y < segment.Offset + segment.Count
            );
            map[x][y] = isMovedRock ? 'O' : '.';
        }
    }
}

long CalculateScore(char[][] map)
{
    var sum = 0;
    for (var x = 0; x < map[0].Length; ++x)
    {
        for (var y = 0; y < map.Length; ++y)
        {
            if (map[y][x] == 'O')
                sum += map.Length - y;
        }
    }
    return sum;
}

long Part1(string[] lines)
{
    var map = lines.Select(line => line.ToCharArray()).ToArray();
    ShiftRocksNorth(map);
    return CalculateScore(map);
}

long Part2(string[] lines)
{
    var map = lines.Select(line => line.ToCharArray()).ToArray();
    var history = new List<long>();
    for (var i = 0; i < 1000; ++i) // should be 1000000000, but this is better for memory :P
    {
        map = Cycle(map);
        history.Add(CalculateScore(map));
    }
    var p1 = 1;
    var p2 = 2;
    while (history[p1] != history[p2])
    {
        p1 += 1;
        p2 += 2;
    }
    var offset = 0;
    p1 = 0;
    while (history[p1] != history[p2])
    {
        p1 += 1;
        p2 += 2;
        offset++;
    }
    var distance = 1;
    p2 = p1 + 1;
    while (history[p1] != history[p2])
    {
        p2 += 1;
        distance += 1;
    }
    
    Console.WriteLine("Cycle distance: {0}, offset {1}", distance, offset);
    for (var i = 0; i < 25; ++i)
        Console.Write("{0} ", history[i]);
    Console.WriteLine();
    var result = (1000000000 - offset) % distance;

    return history[result + offset - 1];
}