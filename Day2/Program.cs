using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 2);

var input = AdventOfCode.GetInputLines();

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

int Part1(string[] lines)
{
    var limits = new Dictionary<string, int>
    {
        {"red", 12},
        {"green", 13},
        {"blue", 14},
    };
    var idSum = 0;

    foreach (var line in lines)
    {
        var raw = line.Split(':');
        var gameId = int.Parse(raw[0].Split(' ')[1]);
        var sets = raw[1].Split(';', StringSplitOptions.TrimEntries)
            .Select(set => set.Split(',', StringSplitOptions.TrimEntries)
                .Select(cubeSet =>
                {
                    var cubeInfo = cubeSet.Split(' ', StringSplitOptions.TrimEntries);
                    return (cubeInfo[1], int.Parse(cubeInfo[0]));
                })
            );
        foreach (var set in sets)
        {
            var hasItemOverLimit = set.Any(cubeInfo =>
            {
                if (!limits.TryGetValue(cubeInfo.Item1, out var limit))
                    return false;
                return cubeInfo.Item2 > limit;
            });
            if (hasItemOverLimit)
            {
                idSum -= gameId;
                break;
            }
        }
        idSum += gameId;
    }

    return idSum;
}

int Part2(string[] lines)
{
    var powerSum = 0;

    foreach (var line in lines)
    {
        var raw = line.Split(':');
        var gameId = int.Parse(raw[0].Split(' ')[1]);
        var sets = raw[1].Split(';', StringSplitOptions.TrimEntries)
            .Select(set => set.Split(',', StringSplitOptions.TrimEntries)
                .Select(cubeSet =>
                {
                    var cubeInfo = cubeSet.Split(' ', StringSplitOptions.TrimEntries);
                    return (cubeInfo[1], int.Parse(cubeInfo[0]));
                })
            );
        // Console.WriteLine($"Game {gameId}: ");
        var minimums = new Dictionary<string, int>
        {
            {"red", 0},
            {"green", 0},
            {"blue", 0}
        };
        foreach (var set in sets)
        {
            foreach (var cube in set)
            {
                var color = cube.Item1;
                var quantity = cube.Item2;
                if (minimums.TryGetValue(color, out var minimum))
                {
                    if (minimum < quantity)
                        minimums.Remove(color);
                    else
                        continue;
                }
                minimums.Add(color, quantity);
            }
        }
        var power = 1;
        foreach (var (_, minimum) in minimums)
            power *= minimum;
        powerSum += power;
    }

    return powerSum;
}