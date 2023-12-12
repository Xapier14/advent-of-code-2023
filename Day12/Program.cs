using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 12);

var input = AdventOfCode.GetInputLines(true);

var sample =
    """
    ???.### 1,1,3
    .??..??...?##. 1,1,3
    ?#?#?#?#?#?#?#? 1,3,1,6
    ????.#...#... 4,1,1
    ????.######..#####. 1,6,5
    ?###???????? 3,2,1
    """.Split(Environment.NewLine);

var cache = new Dictionary<string, string[]>();

var combi = GenerateCombinations("?###????????");
var count = combi.Where(str => MatchesContiguousGroups(str, new[] {3, 2, 1}));

Utility.Assert(Part1, sample, 21);
Utility.Assert(Part2, sample, 525152);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

// string[] GenerateAllCombinations(string template)
// {
//     var ret = new List<string>();
//     foreach (var VARIABLE in template)
//     {
//         
//     }
//     return ret.ToArray();
// }
string[] GenerateCombinations(string template)
{
    if (template.Length == 0)
        return Array.Empty<string>();
    if (template.Length == 1)
        return template[0] != '?'
            ? new[] { $"{template[0]}" }
            : new[] { ".", "#" };
    var ret = new List<string>();
    var firstIndex = template[0];
    var subString = template[1..];
    if (firstIndex != '?')
    {
        var subCombinations = GenerateCombinations(subString);
        foreach (var subCombination in subCombinations)
        {
            ret.Add($"{firstIndex}{subCombination}");
        }
    }
    else
    {
        var subCombinations = GenerateCombinations(subString);
        foreach (var subCombination in subCombinations)
        {
            ret.Add($".{subCombination}");
            ret.Add($"#{subCombination}");
        }
    }

    return ret.ToArray();
}

bool MatchesContiguousGroups(string line, int[] groups)
{
    var group = 0;
    var counter = 0;
    for (var i = 0; i < line.Length; ++i)
    {
        if (line[i] == '#')
        {
            if (group >= groups.Length)
                return false;
            counter++;
            continue;
         }
        if (counter == 0)
            continue;
        if (counter != groups[group])
            return false;
        group++;
        counter = 0;
    }

    if (counter != 0)
    {
        if (counter != groups[group])
            return false;
        group++;
    }

    return group == groups.Length;
}

long Part1(string[] lines)
{
    var templates = new List<(string Template, int[] Groups, string[] Combinations)>();
    foreach (var line in lines)
    {
        var template = line.Split(' ')[0];
        var groups = line.Split(' ')[1].Split(',').Select(int.Parse).ToArray();
        var combinations = GenerateCombinations(template);
        templates.Add((template, groups, combinations));
    }

    var validLines = templates.Select(template =>
        template.Combinations.Count(line => MatchesContiguousGroups(line, template.Groups)));
    
    return validLines.Sum();
}

long Part2(string[] lines)
{
    var templates = new List<(string Template, int[] Groups, string[] Combinations)>();
    var ic = 1;
    long sum = 0;
    foreach (var line in lines)
    {
        var template = line.Split(' ')[0];
        var explodedTemplate = $"{template}?{template}?{template}?{template}?{template}";
        var groups = line.Split(' ')[1].Split(',').Select(int.Parse).ToArray();
        Console.WriteLine("Expanding line {0} / {1}", ic, lines.Length);
        var combinations = GenerateCombinations(explodedTemplate);
        Console.WriteLine("Filtering...");
        var add = combinations.Count(l => MatchesContiguousGroups(line, groups));
        Console.WriteLine("Result: {0} {1}", template, add);
        ic++;
        sum += add;
        templates.Add((template, groups, combinations)); 
    };
    
    return sum;
}
