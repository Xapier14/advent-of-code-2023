// https://adventofcode.com/2023/day/1
using System.Text;
using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 1);

var input = AdventOfCode.GetInputLines();

var sample1 =
    """
    1abc2
    pqr3stu8vwx
    a1b2c3d4e5f
    treb7uchet
    """.Split(Environment.NewLine);
var sample2 =
    """
    two1nine
    eightwothree
    abcone2threexyz
    xtwone3four
    4nineeightseven2
    zoneight234
    7pqrstsixteen
    """.Split(Environment.NewLine);

Utility.Assert(Part1, sample1, 142);
Utility.Assert(Part2, sample2, 281);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

bool HasTextualNumber(string value, out int number, bool reversed = false)
{
    number = int.MinValue;
    value = value.ToLower();

    var mappings = new Dictionary<string, int>
    {
        {"one", 1},
        {"two", 2},
        {"three", 3},
        {"four", 4},
        {"five", 5},
        {"six", 6},
        {"seven", 7},
        {"eight", 8},
        {"nine", 9},
    };

    foreach (var (textual, numerical) in mappings)
    {
        var toCheck = textual;
        if (reversed)
        {
            var buffer = toCheck.ToCharArray();
            Array.Reverse(buffer);
            toCheck = new string(buffer);
        }
        if (!value.Contains(toCheck))
            continue;
        number = numerical;
        return true;
    }

    return false;
}

int GetFirstDigit(string value, bool includeText)
{
    var buffer = new StringBuilder();

    for (var i = 0; i < value.Length; ++i)
    {
        buffer.Append(value[i]);
        if (char.IsDigit(value[i]))
        {
            return value[i] - '0';
        }
        if (includeText && HasTextualNumber(buffer.ToString(), out var number))
        {
            return number;
        }
    }

    return int.MinValue;
}

int GetLastDigit(string value, bool includeText)
{
    var buffer = new StringBuilder();

    for (var i = value.Length - 1; i >= 0; --i)
    {
        buffer.Append(value[i]);
        if (char.IsDigit(value[i]))
        {
            return value[i] - '0';
        }
        if (includeText && HasTextualNumber(buffer.ToString(), out var number, true))
        {
            return number;
        }
    }

    return int.MinValue;
}

long Part1(string[] input)
{
    long sum = 0;
    foreach (var line in input)
    {
        var firstDigit = GetFirstDigit(line, false);
        var lastDigit = GetLastDigit(line, false);

        var calibrationValue = firstDigit * 10 + lastDigit;
        sum += calibrationValue;
    }

    return sum;
}

long Part2(string[] input)
{
    long sum = 0;
    foreach (var line in input)
    {
        var firstDigit = GetFirstDigit(line, true);
        var lastDigit = GetLastDigit(line, true);

        var calibrationValue = firstDigit * 10 + lastDigit;
        sum += calibrationValue;
    }

    return sum;
}