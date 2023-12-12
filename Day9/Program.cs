using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 9);

var sample =
    """
    0 3 6 9 12 15
    1 3 6 10 15 21
    10 13 16 21 30 45
    """.Split(Environment.NewLine);

var input = AdventOfCode.GetInputLines();

Utility.Assert(Part1, sample, 114);
Utility.Assert(Part2, sample, 2);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

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