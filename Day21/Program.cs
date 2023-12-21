using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 21);

var input = AdventOfCode.GetInputLines();

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

int Mod(int x, int m)
{
    return (x % m + m) % m;
}

long GetReachableInExact(char[][] map, (int X, int Y) starting, long steps)
{
    var queue = new PriorityQueue<(int X, int Y), long>();
    queue.Enqueue((starting.X, starting.Y), 0);
    var visited = new HashSet<(int X, int Y)>();
    var count = 0L;
    
    var switcher = starting.X % 2 != starting.Y % 2 ? 1 : 0;
    for (var i = 0; i < steps; ++i)
        switcher = switcher == 1 ? 0 : 1;

    while (queue.TryDequeue(out var instruction, out var currentSteps))
    {
        var (x, y) = instruction;
        if (!visited.Add(instruction))
            continue;
        
        if (switcher == 0)
            count += x % 2 == y % 2 ? 1 : 0;
        else
            count += x % 2 != y % 2 ? 1 : 0;

        if (currentSteps >= steps)
            continue;

        currentSteps++;

        // left
        if (x > 0 && map[y][x - 1] != '#')
            queue.Enqueue((x - 1, y), currentSteps);
        // up
        if (y > 0 && map[y - 1][x] != '#')
            queue.Enqueue((x, y - 1), currentSteps);
        // right
        if (x < map[y].Length - 1 && map[y][x + 1] != '#')
            queue.Enqueue((x + 1, y), currentSteps);
        // down
        if (y < map.Length - 1 && map[y + 1][x] != '#')
            queue.Enqueue((x, y + 1), currentSteps);
    }
    
    return count;
}


long GetReachableInExactInfinite(char[][] map, (int X, int Y) starting, long steps)
{
    var queue = new PriorityQueue<(int X, int Y), long>();
    queue.Enqueue((starting.X, starting.Y), 0);
    var visited = new HashSet<(int X, int Y)>();
    var count = 0L;
    
    var switcher = starting.X % 2 != starting.Y % 2 ? 1 : 0;
    for (var i = 0; i < steps; ++i)
        switcher = switcher == 1 ? 0 : 1;

    while (queue.TryDequeue(out var instruction, out var currentSteps))
    {
        var (x, y) = instruction;
        if (!visited.Add(instruction))
            continue;
        
        if (switcher == 0)
            count += Mod(x, 2) == Mod(y, 2) ? 1 : 0;
        else
            count += Mod(x, 2) != Mod(y, 2) ? 1 : 0;

        if (currentSteps >= steps)
            continue;

        currentSteps++;

        var adjacentList = new (int X, int Y)[] { (x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1) };
        foreach (var adjacent in adjacentList)
        {
            if (adjacent.X >= 0 && adjacent.X < map[0].Length &&
                adjacent.Y >= 0 && adjacent.Y < map.Length)
            {
                if (map[adjacent.Y][adjacent.X] != '#')
                    queue.Enqueue(adjacent, currentSteps);
            }
            else
            {
                // out of bounds
                (int X, int Y) nextPosition = (Mod(adjacent.X, map[0].Length), Mod(adjacent.Y, map.Length));
                if (map[nextPosition.Y][nextPosition.X] != '#')
                    queue.Enqueue(adjacent, currentSteps);
            }
        }
    }
    
    return count;
}

long Part1(string[] lines)
{
    var map = lines.Select(line => line.ToCharArray()).ToArray();
    (int X, int Y) starting = (-1, -1);
    for (var mY = 0; mY < map.Length && starting == (-1, -1); ++mY)
        for (var mX = 0; mX < map[mY].Length && starting == (-1, -1); ++mX)
            if (map[mY][mX] == 'S')
                starting = (mX, mY);
    if (starting == (-1, -1))
        throw new Exception("Starting position not found.");
    var result = GetReachableInExact(map, starting, 64);
    return result;
}

ulong Part2(string[] lines)
{
    var map = lines.Select(line => line.ToCharArray()).ToArray();
    (int X, int Y) starting = (-1, -1);
    for (var mY = 0; mY < map.Length && starting == (-1, -1); ++mY)
        for (var mX = 0; mX < map[mY].Length && starting == (-1, -1); ++mX)
            if (map[mY][mX] == 'S')
                starting = (mX, mY);
    if (starting == (-1, -1))
        throw new Exception("Starting position not found.");
    
    var y1 = GetReachableInExactInfinite(map, starting, 65 + lines.Length * 0);
    var y2 = GetReachableInExactInfinite(map, starting, 65 + lines.Length * 1);
    var y3 = GetReachableInExactInfinite(map, starting, 65 + lines.Length * 2);
    const int steps = 26501365;

    double c = y1;
    var ab = y2 - c;
    var _4a2b = y3 - c;
    var _2a = _4a2b - 2 * ab;
    var a = _2a / 2;
    var b = ab - a;
    var x = (steps - 65) / lines.Length;
    return (ulong)(a * Math.Pow(x, 2) + b * x + c);
}