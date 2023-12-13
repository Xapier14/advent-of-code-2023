using System.Text;
using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 13);

var input = AdventOfCode.GetInputText();

var sample =
    """
    #.##..##.
    ..#.##.#.
    ##......#
    ##......#
    ..#.##.#.
    ..##..##.
    #.#.##.#.

    #...##..#
    #....#..#
    ..##..###
    #####.##.
    #####.##.
    ..##..###
    #....#..#
    """.ReplaceLineEndings("\n");

Utility.Assert(Part1, sample, 405);
Utility.Assert(Part2, sample, 400);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 1: {0}", Part2(input));
return;

void HashLines(string[] grid, out int[] horizontalLines, out int[] verticalLines)
{
    var hRet = new List<int>();
    var vRet = new List<int>();

    var cols = grid[0].Length;
    var rows = grid.Length;
    foreach (var row in grid)
        hRet.Add(row.GetHashCode());
    var sb = new StringBuilder();
    for (var x = 0; x < cols; x++)
    {
        for (var y = 0; y < rows; y++)
        {
            sb.Append(grid[y][x]);
        }
        vRet.Add(sb.ToString().GetHashCode());
        sb.Clear();
    }

    horizontalLines = hRet.ToArray();
    verticalLines = vRet.ToArray();
}

void SplitLines(string[] grid, out string[] horizontalLines, out string[] verticalLines)
{
    var hRet = new List<string>();
    var vRet = new List<string>();

    var cols = grid[0].Length;
    var rows = grid.Length;
    foreach (var row in grid)
        hRet.Add(row);
    var sb = new StringBuilder();
    for (var x = 0; x < cols; x++)
    {
        for (var y = 0; y < rows; y++)
        {
            sb.Append(grid[y][x]);
        }
        vRet.Add(sb.ToString());
        sb.Clear();
    }

    horizontalLines = hRet.ToArray();
    verticalLines = vRet.ToArray();
}

bool HasSymmetry(int[] lines, out int splitPoint)
{
    for (splitPoint = 1; splitPoint < lines.Length; splitPoint++)
    {
        var left = lines.Take(splitPoint).Reverse().ToArray();
        var right = lines.Skip(splitPoint).ToArray();
        var con = false;
        for (var i = 0; i < left.Length && i < right.Length; ++i)
        {
            if (left[i] == right[i])
                continue;
            con = true;
            break;
        }
        if (con)
            continue;
        return true;
    }

    splitPoint = 0;
    return false;
}

bool HasSymmetryWithSmudge(string[] lines, out int splitPoint)
{
    for (splitPoint = 1; splitPoint < lines.Length; splitPoint++)
    {
        var left = lines.Take(splitPoint).Reverse().ToArray();
        var right = lines.Skip(splitPoint).ToArray();
        var symmetrical = true;
        var smudged = false;
        for (var i = 0; i < left.Length && i < right.Length; ++i)
        {
            var difference = Difference(left[i], right[i]);
            if (difference == 1)
            {
                if (!smudged)
                {
                    smudged = true;
                }
                else
                {
                    symmetrical = false;
                    break;
                }
            }

            if (difference <= 1)
                continue;
            symmetrical = false;
            break;
        }

        if (symmetrical && smudged)
            return true;
    }

    splitPoint = 0;
    return false;
}

int Difference(string a, string b)
{
    if (a.Length != b.Length)
        return int.MaxValue;
    var diff = 0;
    for (var i = 0; i < a.Length && i < b.Length; ++i)
        if (a[i] != b[i])
            diff++;
    return diff;
}

long Part1(string text)
{
    var mirrors = text.Split("\n\n", StringSplitOptions.TrimEntries)
        .Select(mirror => mirror.Split('\n'))
        .ToArray();
    long total = 0;
    foreach (var mirror in mirrors)
    {
        HashLines(mirror, out var hLines, out var vLines);
        if (HasSymmetry(hLines, out var lSplit))
            total += lSplit * 100;
        if (HasSymmetry(vLines, out var tSplit))
            total += tSplit;
    }
    return total;
}

long Part2(string text)
{
    var mirrors = text.Split("\n\n", StringSplitOptions.TrimEntries)
        .Select(mirror => mirror.Split('\n'))
        .ToArray();
    long total = 0;
    foreach (var mirror in mirrors)
    {
        SplitLines(mirror, out var hLines, out var vLines);
        var vSymmetry = HasSymmetryWithSmudge(hLines, out var lSplit);
        var hSymmetry = HasSymmetryWithSmudge(vLines, out var tSplit);
        if (!vSymmetry && !hSymmetry)
        {
            Console.WriteLine("No symmetry found!");
            vSymmetry = HasSymmetryWithSmudge(hLines, out lSplit);
            hSymmetry = HasSymmetryWithSmudge(vLines, out tSplit);
        }
        if (vSymmetry)
            total += lSplit * 100;
        if (hSymmetry)
            total += tSplit;
    }
    return total;
}