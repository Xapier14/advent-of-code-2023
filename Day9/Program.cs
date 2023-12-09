using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 9);

var sample =
    """
    0 3 6 9 12 15
    1 3 6 10 15 21
    10 13 16 21 30 45
    """.Split(Environment.NewLine);
const int control1 = 114;
const int control2 = 2;


var input = AdventOfCode.GetInputAsLines();

PassSample(Part1, sample, control1);
PassSample(Part2, sample, control2);

var part1 = Part1(input);
var part2 = Part2(input);
Console.WriteLine("Part 1: {0}", part1);
Console.WriteLine("Part 2: {0}", part2);
return;

void PassSample(Func<string[], long> func, string[] input, int control)
{
    var sample = func(input);
    if (sample != control)
    {
        Console.WriteLine("Sample fail: {0} actual, {1} expected.", sample, control);
        Environment.Exit(-1);
    }

    Console.WriteLine("Sample passed.");
}

long PredictNext(long[] input)
{
    var differences = new long[input.Length - 1];
    var temp = input[0];
    for (var i = 1; i < input.Length; ++i)
    {
        differences[i - 1] = input[i] - temp;
        temp = input[i];
    }
    if (differences.GroupBy(difference => difference).Count() == 1)
    {
        return differences[0] + input.Last();
    }

    return PredictNext(differences) + input.Last();
}

long PredictPrev(long[] input)
{
    var differences = new long[input.Length - 1];
    var temp = input[0];
    for (var i = 1; i < input.Length; ++i)
    {
        differences[i - 1] = input[i] - temp;
        temp = input[i];
    }
    if (differences.GroupBy(difference => difference).Count() == 1)
    {
        return input.First() - differences[0];
    }

    return input.First() - PredictPrev(differences);
}

long Part1(string[] lines)
{
    var inputs = lines.Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        .Select(line => line.Select(long.Parse).ToArray());

    return inputs.Sum(PredictNext);
}

long Part2(string[] lines)
{
    var inputs = lines.Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        .Select(line => line.Select(long.Parse).ToArray());

    return inputs.Sum(PredictPrev);
}