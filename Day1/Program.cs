// https://adventofcode.com/2023/day/1
using System.Text;
using Xapier14.AdventOfCode;

AdventOfCode.SetYearAndDay(2023, 1);
var input = AdventOfCode.GetInputAsLines();

var sum = 0;
foreach (var line in input)
{
    // get digits
    var firstDigit = GetFirstDigit(line);
    var lastDigit = GetLastDigit(line);

    // get calibration value
    var calibrationValue = (firstDigit * 10) + lastDigit;

    sum += calibrationValue;
}

Console.WriteLine("Sum of all calibration values: {0}", sum);
return;
// END

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

int GetFirstDigit(string value)
{
    var buffer = new StringBuilder();

    for (var i = 0; i < value.Length; ++i)
    {
        buffer.Append(value[i]);
        if (char.IsDigit(value[i]))
        {
            return value[i] - '0';
        }
        if (HasTextualNumber(buffer.ToString(), out var number))
        {
            return number;
        }
    }

    return int.MinValue;
}

int GetLastDigit(string value)
{
    var buffer = new StringBuilder();

    for (var i = value.Length - 1; i >= 0; --i)
    {
        buffer.Append(value[i]);
        if (char.IsDigit(value[i]))
        {
            return value[i] - '0';
        }
        if (HasTextualNumber(buffer.ToString(), out var number, true))
        {
            return number;
        }
    }

    return int.MinValue;
}