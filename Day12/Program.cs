using System.Collections.Immutable;
using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 12);

var input = AdventOfCode.GetInputLines();

var sample =
    """
    ???.### 1,1,3
    .??..??...?##. 1,1,3
    ?#?#?#?#?#?#?#? 1,3,1,6
    ????.#...#... 4,1,1
    ????.######..#####. 1,6,5
    ?###???????? 3,2,1
    """.Split(Environment.NewLine);

var cache = new Dictionary<string, long>();

Utility.Assert(Part1, sample, 21);
Utility.Assert(Part2, sample, 525152);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

long Navigate(string line, ImmutableStack<int> groups)
{
    var id = $"{line}-{string.Join(',', groups.Select(group => group.ToString()))}";
    if (cache.TryGetValue(id, out var result))
        return result;
    char? token = line.Length > 0 ? line[0] : null;
    result = token switch
    {
        '.' => Navigate(line[1..], groups),
        '#' => NavigatePositive(line, groups),
        '?' => Navigate($".{line[1..]}", groups) + Navigate($"#{line[1..]}", groups),
        _ => groups.IsEmpty ? 1 : 0
    };
    cache[id] = result;
    return result;
}

long NavigatePositive(string line, ImmutableStack<int> groups)
{
    if (groups.IsEmpty)
        return 0;
    var group = groups.Peek();
    groups = groups.Pop();

    var rest = 0;
    for (var i = 0; i < line.Length && i < group; ++i)
    {
        if (line[i] == '.')
            continue;
        rest++;
    }

    if (rest < group)
        return 0;
    if (line.Length == rest)
        return Navigate("", groups);
    return line[rest] == '#'
        ? 0
        : Navigate(line[(rest+1)..], groups);
}

long Part1(string[] lines)
{
    long sum = 0;
    foreach (var line in lines)
    {
        var template = line.Split(' ')[0];
        var groups = line.Split(' ')[1].Split(',').Select(int.Parse).ToArray();
        var stack = ImmutableStack<int>.Empty;
        for (var i = groups.Length - 1; i >= 0; i--)
            stack = stack.Push(groups[i]);
        sum += Navigate(template, stack);
    }
    
    return sum;
}

long Part2(string[] lines)
{
    long sum = 0;
    foreach (var line in lines)
    {
        var template = line.Split(' ')[0];
        var explodedTemplate = $"{template}?{template}?{template}?{template}?{template}";
        var rawGroups = line.Split(' ')[1].Split(',').Select(int.Parse).ToArray();
        var stack = ImmutableStack<int>.Empty;
        for (var r = 0; r < 5; ++r)
            for (var i = rawGroups.Length - 1; i >= 0; i--)
                stack = stack.Push(rawGroups[i]);
        sum += Navigate(explodedTemplate, stack);
    };
    
    return sum;
}
