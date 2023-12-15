using System.Collections.Specialized;
using System.Text;
using Xapier14.AdventOfCode;

AdventOfCode.SetYearAndDay(2023, 15);

var sample = "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7";

var input = AdventOfCode.GetInputText().Trim('\n').Trim();

Utility.Assert(Part1, sample, 1320);
Utility.Assert(Part2, sample, 145);

var part1 = Part1(input);
var part2 = Part2(input);
Console.WriteLine("Part 1: {0}", part1);
Console.WriteLine("Part 2: {0}", part2);
AdventOfCode.SubmitPart2(part2);
return;

long Hash(string input)
{
    var hash = 0;
    var chars = Encoding.ASCII.GetBytes(input);
    foreach (var c in chars)
    {
        hash += (int)c;
        hash *= 17;
        hash %= 256;
    }
    return hash;
}

long Part1(string text)
{
    var sequence = text.Split(',');
    long hash = 0;
    foreach (var str in sequence)
        hash += Hash(str);
    return hash;
}

long Part2(string text)
{
    var sequence = text.Split(',');
    var boxes = new Dictionary<long, OrderedDictionary>();
    foreach (var str in sequence)
    {
        if (str.EndsWith('-'))
        {
            // remove
            var key = str[..^1];
            var hashedKey = Hash(key);
            if (!boxes.TryGetValue(hashedKey, out var box))
            {
                box = new OrderedDictionary();
                boxes.Add(hashedKey, box);
            }

            box.Remove(key);
        }
        else
        {
            var key = str.Split('=')[0];
            var value = long.Parse(str.Split('=')[1]);
            var hashedKey = Hash(key);

            if (!boxes.TryGetValue(hashedKey, out var box))
            {
                box = new OrderedDictionary();
                boxes.Add(hashedKey, box);
            }
            box[key] = value;
        }

    }

    long sum1 = 0;
    foreach (var (boxNo, box) in boxes)
    {
        long sum2 = 0;
        var i = 0;
        foreach (var value in box.Values)
        {
            sum2 += (boxNo + 1) * (i + 1) * (long)value;
            i++;
        }
        sum1 += sum2;
    }

    return sum1;
}

