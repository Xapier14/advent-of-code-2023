using System.Text;

(int[], int[]) ParseInput(string filePath)
{
    var lines = File.ReadAllLines(filePath);
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

ulong Part1(string filePath)
{
    var (timeArray, distanceArray) = ParseInput(filePath);
    var races = timeArray.Length;

    ulong raceProduct = 1;
    for (var i = 0; i < races; ++i)
    {
        var time = (ulong)timeArray[i];
        var distance = (ulong)distanceArray[i];
        var midPoint = (time + 1) / 2;
        ulong halfSpan = 0;
        ulong midPointOffset = 0;
        ulong distanceCheck = 0;
        do
        {
            distanceCheck = SimulateGame((ulong)time, (ulong)(midPoint + midPointOffset));
            if (distanceCheck > distance)
            {
                midPointOffset++;
                halfSpan = midPointOffset;
            }
        } while (distanceCheck > distance);

        var winningRaces = halfSpan * 2 - (ulong)((time + 1) % 2 != 0 ? 1 : 0);
        raceProduct *= winningRaces;
    }

    return raceProduct;
}

ulong Part2(string filePath)
{
    var (timeArray, distanceArray) = ParseInput(filePath);
    var time = Kern(timeArray);
    var distance = Kern(distanceArray);
    // var winRange = CalculateWinningRange(time, distance);
    var midPoint = (time + 1) / 2;
    ulong halfSpan = 0;
    ulong midPointOffset = 0;
    ulong distanceCheck = 0;
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
    for (var i = 0; i < array.Length; ++i)
        builder.Append(array[i].ToString());
    return ulong.Parse(builder.ToString());
}

// returns distance traveled
ulong SimulateGame(ulong raceLength, ulong buttonHoldDuration)
    => (raceLength - buttonHoldDuration) * buttonHoldDuration;

Console.WriteLine("Part 1 Solution: {0}", Part1("input.txt"));
Console.WriteLine("Part 2 Solution: {0}", Part2("input.txt"));