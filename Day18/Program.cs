using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 18);

var input = AdventOfCode.GetInputLines();

var sample =
    """
    R 6 (#70c710)
    D 5 (#0dc571)
    L 2 (#5713f0)
    D 2 (#d2c081)
    R 2 (#59c680)
    D 2 (#411b91)
    L 5 (#8ceee2)
    U 2 (#caa173)
    L 1 (#1b58a2)
    U 2 (#caa171)
    R 2 (#7807d2)
    U 3 (#a77fa3)
    L 2 (#015232)
    U 2 (#7a21e3)
    """.Split(Environment.NewLine);

Utility.Assert(Part1, sample, 62);
Utility.Assert(Part2, sample, 952408144115);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

(long X, long Y)[] GenerateVertices((Direction Direction, long Count)[] instructions, out long length)
{
    var tiles = new List<(long X, long Y)>();
    long x = 0;
    long y = 0;
    length = 0;
    foreach (var instruction in instructions)
    {
        var moveX = instruction.Direction switch
        {
            Direction.Left => -1,
            Direction.Right => 1,
            _ => 0
        };
        var moveY = instruction.Direction switch
        {
            Direction.Up => -1,
            Direction.Down => 1,
            _ => 0
        };
        x += instruction.Count * moveX;
        y += instruction.Count * moveY;
        length += instruction.Count;
        tiles.Add((x, y));
    }

    return tiles.ToArray();
}

long Shoelace((long X, long Y)[] vertices, long boundaryLength)
{
    long sum = 0;
    sum += vertices[^1].X * vertices[0].Y - vertices[0].X * vertices[^1].Y;
    for (var i = 0; i < vertices.Length - 1; ++i)
        sum += vertices[i].X * vertices[i + 1].Y - vertices[i + 1].X * vertices[i].Y;
    return (Math.Abs(sum) + boundaryLength + 2L) / 2L;
}

long Part1(string[] lines)
{
    var instructions = lines.Select(line =>
    {
        var direction = (Direction)line.Split()[0][0];
        var count = long.Parse(line.Split()[1]);
        return (direction, count);
    }).ToArray();
    var vertices = GenerateVertices(instructions, out var length);
    var area = Shoelace(vertices, length);

    return area;
}

long Part2(string[] lines)
{
    var instructions = lines.Select(line =>
    {
        var hex = line.Split()[2].Trim('(', ')');
        var direction = hex[^1] switch
        {
            '0' => Direction.Right,
            '1' => Direction.Down,
            '2' => Direction.Left,
            '3' => Direction.Up,
            _ => throw new Exception("Unknown direction.")
        };
        var count = Convert.ToInt64(hex.TrimStart('#')[..5], 16);
        return (direction, count);
    }).ToArray();
    var vertices = GenerateVertices(instructions, out var length);
    var area = Shoelace(vertices.Reverse().ToArray(), length);

    return area;
}

enum Direction { Up = 'U', Down = 'D', Left = 'L', Right = 'R'}