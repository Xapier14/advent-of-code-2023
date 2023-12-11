using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 11);

var input = AdventOfCode.GetInputAsLines();

var sample =
    """
    ...#......
    .......#..
    #.........
    ..........
    ......#...
    .#........
    .........#
    ..........
    .......#..
    #...#.....
    """.Split(Environment.NewLine);

AdventOfCode.Assert(Part1, sample, 374);
AdventOfCode.Assert(Part2, sample, 82000210);
Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

void ExpandMap(char[][] map, out long[] expandedRows, out long[] expandedCols)
{
    var rows = new List<long>();
    var cols = new List<long>();
    for (var i = 0; i < map.Length; ++i)
    {
        if (map[i].Any(c => c != '.'))
            continue;
        rows.Add(i);
    }

    for (var i = 0; i < map[0].Length; i++)
    {
        var isAllEmpty = map.All(line => line[i] == '.');
        if (!isAllEmpty)
            continue;
        cols.Add(i);
    }
    expandedRows = rows.ToArray();
    expandedCols = cols.ToArray();
}

(long, long)[] FindGalaxies(char[][] map, char galaxy)
{
    var ret = new List<(long, long)>();
    for (var y = 0; y < map.Length; ++y)
        for (var x = 0; x < map[y].Length; ++x)
            if (map[y][x] == galaxy)
                ret.Add((x, y));
    return ret.ToArray();
}

long Part1(string[] lines)
{
    var map = lines.Select(line => line.ToCharArray()).ToArray();
    ExpandMap(map, out var expandedRows, out var expandedCols);
    (long X, long Y)[] galaxies = FindGalaxies(map, '#');
    var distances = new List<long>();
    foreach (var coord1 in galaxies)
    {
        foreach (var coord2 in galaxies)
        {
            var expandedX = expandedCols.Count(x => x >= Math.Min(coord1.X, coord2.X) && x <= Math.Max(coord1.X, coord2.X));
            var expandedY = expandedRows.Count(y => y >= Math.Min(coord1.Y, coord2.Y) && y <= Math.Max(coord1.Y, coord2.Y));
            var distance = Math.Abs(coord1.X - coord2.X) + expandedX + Math.Abs(coord1.Y - coord2.Y) + expandedY;
            distances.Add(distance);
        }
    }

    return distances.Sum(distance => distance) / 2;
}

long Part2(string[] lines)
{
    var map = lines.Select(line => line.ToCharArray()).ToArray();
    ExpandMap(map, out var expandedRows, out var expandedCols);
    (long X, long Y)[] galaxies = FindGalaxies(map, '#');
    var distances = new List<long>();
    foreach (var coord1 in galaxies)
    {
        foreach (var coord2 in galaxies)
        {
            var expandedX = expandedCols.Count(x => x >= Math.Min(coord1.X, coord2.X) && x <= Math.Max(coord1.X, coord2.X)) * 999999;
            var expandedY = expandedRows.Count(y => y >= Math.Min(coord1.Y, coord2.Y) && y <= Math.Max(coord1.Y, coord2.Y)) * 999999;
            var distance = Math.Abs(coord1.X - coord2.X) + expandedX + Math.Abs(coord1.Y - coord2.Y) + expandedY;
            distances.Add(distance);
        }
    }

    return distances.Sum(distance => distance) / 2;
}