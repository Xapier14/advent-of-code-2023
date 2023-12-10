using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 10);

var input = AdventOfCode.GetInputAsLines();
var sample1 =
    """
    .....
    .S-7.
    .|.|.
    .L-J.
    .....
    """.Split(Environment.NewLine);
var sample2 =
    """
    ..F7.
    .FJ|.
    SJ.L7
    |F--J
    LJ...
    """.Split(Environment.NewLine);
var sample3 =
    """
    ...........
    .S-------7.
    .|F-----7|.
    .||.....||.
    .||.....||.
    .|L-7.F-J|.
    .|..|.|..|.
    .L--J.L--J.
    ...........
    """.Split(Environment.NewLine);
var sample4 =
    """
    ..........
    .S------7.
    .|F----7|.
    .||....||.
    .||....||.
    .|L-7F-J|.
    .|..||..|.
    .L--JL--J.
    ..........
    """.Split(Environment.NewLine);
var sample5 =
    """
    .F----7F7F7F7F-7....
    .|F--7||||||||FJ....
    .||.FJ||||||||L7....
    FJL7L7LJLJ||LJ.L-7..
    L--J.L7...LJS7F-7L7.
    ....F-J..F7FJ|L7L7L7
    ....L7.F7||L7|.L7L7|
    .....|FJLJ|FJ|F7|.LJ
    ....FJL-7.||.||||...
    ....L---J.LJ.LJLJ...
    """.Split(Environment.NewLine);
var sample6 =
    """
    FF7FSF7F7F7F7F7F---7
    L|LJ||||||||||||F--J
    FL-7LJLJ||||||LJL-77
    F--JF--7||LJLJ7F7FJ-
    L---JF-JLJ.||-FJLJJ7
    |F|F-JF---7F7-L7L|7|
    |FFJF7L7F-JF7|JL---7
    7-L-JL7||F7|L7F-7F7|
    L.L7LFJ|||||FJL7||LJ
    L7JLJL-JLJLJL--JLJ.L
    """.Split(Environment.NewLine);

AdventOfCode.Assert(Part1, sample1, 4);
AdventOfCode.Assert(Part1, sample2, 8);
AdventOfCode.Assert(Part2, sample3, 4);
AdventOfCode.Assert(Part2, sample4, 4);
AdventOfCode.Assert(Part2, sample5, 8);
AdventOfCode.Assert(Part2, sample6, 10);

var part1 = Part1(input);
Console.WriteLine("Part 1: {0}", part1);
var part2 = Part2(input);
Console.WriteLine("Part 2: {0}", part2);

return;

(long, long) FindStartingPosition(char[][] map)
{
    // map[y][x]
    for (var y = 0; y < map.Length;  y++)
        for (var x = 0;  x < map[y].Length; x++)
            if (map[y][x] == 'S')
                return (x, y);
    throw new Exception("Starting position not found.");
}

/*
   | is a vertical pipe connecting north and south.
   - is a horizontal pipe connecting east and west.
   L is a 90-degree bend connecting north and east.
   J is a 90-degree bend connecting north and west.
   7 is a 90-degree bend connecting south and west.
   F is a 90-degree bend connecting south and east.
   . is ground; there is no pipe in this tile.
   S is the starting position of the animal; there is a pipe on this tile, but your sketch doesn't show what shape the pipe has.
 */

bool IsOf<T>(T value, params T[] set)
    => set.Any(t => EqualityComparer<T>.Default.Equals(t, value));

(long, long)[] FindAdjacent(char[][] map, (long X, long Y) tile, long resolution = 1)
{
    var ret = new List<(long, long)>();

    // top
    if (tile.Y > resolution - 1 && IsOf(map[tile.Y - resolution][tile.X], '|', '7', 'F') && IsOf(map[tile.Y][tile.X], '|', 'J', 'L', 'S'))
        ret.Add((tile.X, tile.Y - resolution));
    // bottom
    if (tile.Y < map.Length - resolution && IsOf(map[tile.Y + resolution][tile.X], '|', 'J', 'L') && IsOf(map[tile.Y][tile.X], '|', '7', 'F', 'S'))
        ret.Add((tile.X, tile.Y + resolution));
    // left
    if (tile.X > resolution - 1 && IsOf(map[tile.Y][tile.X - resolution], '-', 'L', 'F') && IsOf(map[tile.Y][tile.X], '-', 'J', '7', 'S'))
        ret.Add((tile.X - resolution, tile.Y));
    // right
    if (tile.X < map[tile.Y].Length - resolution && IsOf(map[tile.Y][tile.X + resolution], '-', 'J', '7') && IsOf(map[tile.Y][tile.X], '-', 'L', 'F', 'S'))
        ret.Add((tile.X + resolution, tile.Y));

    if (!ret.Any())
        throw new Exception($"Tile {map[tile.Y][tile.X]} @ ({tile.X}, {tile.Y}) does not have adjacent pipes.");

    return ret.ToArray();
}

(long, long)[] FindAdjacentInList(char[][] map, (long X, long Y) tile, params char[] list)
{
    var ret = new List<(long, long)>();

    // top
    if (tile.Y > 0 && list.Contains(map[tile.Y - 1][tile.X]))
        ret.Add((tile.X, tile.Y - 1));
    // bottom
    if (tile.Y < map.Length - 1 && list.Contains(map[tile.Y + 1][tile.X]))
        ret.Add((tile.X, tile.Y + 1));
    // left
    if (tile.X > 0 && list.Contains(map[tile.Y][tile.X - 1]))
        ret.Add((tile.X - 1, tile.Y));
    // right
    if (tile.X < map[tile.Y].Length - 1 && list.Contains(map[tile.Y][tile.X + 1]))
        ret.Add((tile.X + 1, tile.Y));

    return ret.ToArray();
}

(long, long)[] FindAdjacentExcludingList(char[][] map, (long X, long Y) tile, ICollection<(long X, long Y)> exclusion)
{
    var ret = new List<(long, long)>();

    // top
    if (tile.Y > 0 && (!exclusion.Contains((tile.X, tile.Y - 1))))
        ret.Add((tile.X, tile.Y - 1));
    // bottom
    if (tile.Y < map.Length - 1 && !exclusion.Contains((tile.X, tile.Y + 1)))
        ret.Add((tile.X, tile.Y + 1));
    // left
    if (tile.X > 0 && !exclusion.Contains((tile.X - 1, tile.Y)))
        ret.Add((tile.X - 1, tile.Y));
    // right
    if (tile.X < map[tile.Y].Length - 1 && !exclusion.Contains((tile.X + 1, tile.Y)))
        ret.Add((tile.X + 1, tile.Y));

    return ret.ToArray();
}

long Part1(string[] input)
{
    // map[y][x]
    var map = input.Select(line => line.ToCharArray()).ToArray();
    

    var start = FindStartingPosition(map);
    var distanceMap = new Dictionary<(long, long), long>
    {
        {start, 0}
    };
    var adjacentTiles = new List<(long X, long Y)>();
    adjacentTiles.AddRange(FindAdjacent(map, start));
    var distance = 1;
    while (adjacentTiles.Any())
    {
        var newAdjacent = new List<(long X, long Y)>();
        foreach (var adjacent in adjacentTiles)
        {
            newAdjacent.AddRange(FindAdjacent(map, adjacent));
            if (distanceMap.TryGetValue(adjacent, out var storedDistance))
            {
                if (distance >= storedDistance)
                    continue;
            }
            distanceMap[adjacent] = distance;
        }
        adjacentTiles.Clear();
        newAdjacent.ForEach(tile =>
        {
            if (!distanceMap.ContainsKey(tile))
                adjacentTiles.Add(tile);
        });
        newAdjacent.Clear();
        distance++;
    }
    return distanceMap.Values.Max(dist => dist);
}

long Part2(string[] input)
{
    // map[y][x]
    var raw = input.Select(line =>
    {
        var arr = new List<char>();
        foreach (var c in line)
        {
            arr.Add(c);
            arr.Add('.');
        }
        return arr.ToArray();
    }).ToArray();
    var mapTemp = new List<char[]>();
    foreach (var c in raw)
    {
        var emptyLine = new char[raw[0].Length];
        Array.Fill(emptyLine, '.');
        mapTemp.Add(c);
        mapTemp.Add(emptyLine);
    }
    var map = mapTemp.ToArray();
    var start = FindStartingPosition(map);
    var distanceMap = new Dictionary<(long X, long Y), long>
    {
        {start, 0}
    };
    var adjacentTiles = new List<(long X, long Y)>();
    var startingAdjacent = FindAdjacent(map, start, 2);

    foreach (var (aX, aY) in startingAdjacent)
    {
        var offsetX = (aX - start.Item1) / 2;
        var offsetY = (aY - start.Item2) / 2;
        var modX = start.Item1 + offsetX;
        var modY = start.Item2 + offsetY;
        map[modY][modX] = offsetX != 0 ? '-' : '|';
        distanceMap.Add((modX, modY), -1);
    }
    adjacentTiles.AddRange(startingAdjacent);
    var distance = 1;

    while (adjacentTiles.Any())
    {
        var newAdjacent = new List<(long X, long Y)>();
        foreach (var adjacent in adjacentTiles)
        {
            var thisAdjacent = FindAdjacent(map, adjacent, 2);
            newAdjacent.AddRange(thisAdjacent);
            if (distanceMap.TryGetValue(adjacent, out var storedDistance))
            {
                if (distance >= storedDistance)
                    continue;
            }

            foreach (var (aX, aY) in thisAdjacent)
            {
                var offsetX = (aX - adjacent.X) / 2;
                var offsetY = (aY - adjacent.Y) / 2;
                var modX = adjacent.X + offsetX;
                var modY = adjacent.Y + offsetY;
                map[modY][modX] = offsetX != 0 ? '-' : '|';
                if (!distanceMap.ContainsKey((modX, modY)))
                    distanceMap.Add((modX, modY), -1);
            }
            distanceMap[adjacent] = distance;
        }
        adjacentTiles.Clear();
        newAdjacent.ForEach(tile =>
        {
            if (!distanceMap.ContainsKey(tile))
                adjacentTiles.Add(tile);
        });
        newAdjacent.Clear();
        distance++;
    }

    var emptyTilesNearLoop = new HashSet<(long X, long Y)>();

    foreach (var pipe in distanceMap.Keys)
    {
        var adjacentOpen = FindAdjacentInList(map, pipe, '.');
        foreach (var adjacent in adjacentOpen)
            emptyTilesNearLoop.Add(adjacent);
    }

    // flood fill
    var exploredTiles = new List<(long X, long Y)>();
    var groups = new Dictionary<(long X, long Y), List<(long X, long Y)>>();
    foreach (var tile in emptyTilesNearLoop)
    {
        if (exploredTiles.Contains(tile))
            continue;
        exploredTiles.Add(tile);
        var group = new List<(long X, long Y)> { tile };
        groups.Add(tile, group);
        var adjacentOpen = new List<(long X, long Y)>();
        adjacentOpen.AddRange(FindAdjacentExcludingList(map, tile, distanceMap.Keys));
        var exploredTilesForThisRun = new List<(long X, long Y)> { tile };
        while (adjacentOpen.Any())
        {
            var newAdjacentOpen = new List<(long X, long Y)>();
            foreach (var openTile in adjacentOpen)
            {
                if (openTile.X == 0 || openTile.X >= map[openTile.Y].Length - 1 ||
                    openTile.Y == 0 || openTile.Y >= map.Length - 1)
                {
                    group.Clear();
                    groups.Remove(tile);
                    newAdjacentOpen.Clear();
                    exploredTilesForThisRun.Clear();
                    break;
                }
                if (exploredTilesForThisRun.Contains(openTile))
                    continue;
                exploredTilesForThisRun.Add(openTile);
                group.Add(openTile);
                newAdjacentOpen.AddRange(FindAdjacentExcludingList(map, openTile, distanceMap.Keys));
            }
            adjacentOpen.Clear();
            adjacentOpen.AddRange(newAdjacentOpen);
            newAdjacentOpen.Clear();
        }
        exploredTiles.AddRange(exploredTilesForThisRun);
    }

    var sum = groups.Sum(group => group.Value.Count(tuple => tuple.X % 2 == 0 && tuple.Y % 2 == 0));
    return sum;
}