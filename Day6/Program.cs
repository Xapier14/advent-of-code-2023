﻿using Xapier14.AdventOfCode;
using System.Text;
AdventOfCode.SetYearAndDay(2023, 6);

var input = AdventOfCode.GetInputLines();

var sample =
    """
    Time:      7  15   30
    Distance:  9  40  200
    """.Split(Environment.NewLine);

Utility.Assert(Part1, sample, 288u);
Utility.Assert(Part2, sample, 71503u);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

(int[], int[]) ParseInput(string[] lines)
{
    if (lines.Length != 2)
        throw new Exception();
    var timeArray = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Skip(1)
        .Select(int.Parse)
        .ToArray();
    var distanceArray = lines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Skip(1)
        .Select(int.Parse).
        ToArray();

    return (timeArray, distanceArray);
}

ulong Part1(string[] lines)
{
    var (timeArray, distanceArray) = ParseInput(lines);
    var races = timeArray.Length;

    ulong raceProduct = 1;
    for (var i = 0; i < races; ++i)
    {
        var time = (ulong)timeArray[i];
        var distance = (ulong)distanceArray[i];
        var midPoint = (time + 1) / 2;
        ulong halfSpan = 0;
        ulong midPointOffset = 0;
        ulong distanceCheck;
        do
        {
            distanceCheck = SimulateGame((ulong)time, (ulong)(midPoint + midPointOffset));
            if (distanceCheck <= distance)
                continue;
            midPointOffset++;
            halfSpan = midPointOffset;
        } while (distanceCheck > distance);

        var winningRaces = halfSpan * 2 - (ulong)((time + 1) % 2 != 0 ? 1 : 0);
        raceProduct *= winningRaces;
    }

    return raceProduct;
}

ulong Part2(string[] lines)
{
    var (timeArray, distanceArray) = ParseInput(lines);
    var time = Kern(timeArray);
    var distance = Kern(distanceArray);
    var midPoint = (time + 1) / 2;
    ulong halfSpan = 0;
    ulong midPointOffset = 0;
    ulong distanceCheck;
    do
    {
        distanceCheck = SimulateGame(time, midPoint + midPointOffset);
        if (distanceCheck > distance)
        {
            midPointOffset++;
            halfSpan = midPointOffset;
        }
    } while (distanceCheck > distance);

    var winningRaces = halfSpan * 2 - (ulong)((time + 1) % 2 != 0 ? 1 : 0);

    return winningRaces;
}

ulong Kern(int[] array)
{
    var builder = new StringBuilder();
    foreach (var t in array)
        builder.Append(t.ToString());

    return ulong.Parse(builder.ToString());
}

ulong SimulateGame(ulong raceLength, ulong buttonHoldDuration)
    => (raceLength - buttonHoldDuration) * buttonHoldDuration;