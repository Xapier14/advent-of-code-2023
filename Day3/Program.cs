using Xapier14.AdventOfCode;
using System.Text;
AdventOfCode.SetYearAndDay(2023, 3);

var input = AdventOfCode.GetInputLines();

var sample =
    """
    467..114..
    ...*......
    ..35..633.
    ......#...
    617*......
    .....+.58.
    ..592.....
    ......755.
    ...$.*....
    .664.598..
    """.Split(Environment.NewLine);

Utility.Assert(Part1, sample, 4361);
Utility.Assert(Part2, sample, 467835);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

bool IsSymbol(char c)
    => c != '.' && !char.IsLetterOrDigit(c);

int Part1(string[] lines)
{
    var sum = 0;
    var cols = lines[0].Length;
    var rows = lines.Length;

    var buffer = new StringBuilder();
    var bufferIsNearSymbol = false;
    for (var row = 0; row < rows; ++row)
    {
        for (var col = 0; col < cols; ++col)
        {
            var charToRead = lines[row][col];
            if (char.IsDigit(charToRead))
            {
                buffer.Append(charToRead);
                if (bufferIsNearSymbol)
                    continue;
                // check if adjacent to left with symbol
                if (col > 0 && IsSymbol(lines[row][col - 1]))
                    bufferIsNearSymbol = true;
                // check if adjacent to right with symbol
                if (col < cols - 1 && IsSymbol(lines[row][col + 1]))
                    bufferIsNearSymbol = true;
                // check if adjacent to top with symbol
                if (row > 0 && IsSymbol(lines[row - 1][col]))
                    bufferIsNearSymbol = true;
                // check if adjacent to bottom with symbol
                if (row < rows - 1 && IsSymbol(lines[row + 1][col]))
                    bufferIsNearSymbol = true;

                // check if adjacent to top left with symbol
                if (col > 0 && row > 0 && IsSymbol(lines[row - 1][col - 1]))
                    bufferIsNearSymbol = true;
                // check if adjacent to top left with symbol
                if (col < cols - 1 && row > 0 && IsSymbol(lines[row - 1][col + 1]))
                    bufferIsNearSymbol = true;
                // check if adjacent to bottom left with symbol
                if (col < cols - 1 && row < rows - 1 && IsSymbol(lines[row + 1][col + 1]))
                    bufferIsNearSymbol = true;
                // check if adjacent to bottom left with symbol
                if (col > 0 && row < rows - 1 && IsSymbol(lines[row + 1][col - 1]))
                    bufferIsNearSymbol = true;
                continue;
            }
            // interrupted by non digit
            if (buffer.Length > 0 && bufferIsNearSymbol)
            {
                var partNumber = int.Parse(buffer.ToString());
                // Console.WriteLine("Identified part number {0}", partNumber);
                sum += partNumber;
            }
            buffer.Clear();
            bufferIsNearSymbol = false;
        }
        // eol
        if (buffer.Length > 0 && bufferIsNearSymbol)
        {
            var partNumber = int.Parse(buffer.ToString());
            // Console.WriteLine("Identified part number {0}", partNumber);
            sum += partNumber;
        }
        buffer.Clear();
        bufferIsNearSymbol = false;
    }

    return sum;
}

int Part2(string[] lines)
{
    var cols = lines[0].Length;
    var rows = lines.Length;

    // (int, int) is the xy position of the gear that serves as its identifier
    var numbers = new List<((int, int), int)>();
    var gears = new Dictionary<(int, int), List<int>>();

    var buffer = new StringBuilder();
    // symbol now just refers to '*'.
    const char gearSymbol = '*';
    // if symbol row & column is not -1, the number being read is adjacent to a '*' symbol.
    var symbolRow = -1;
    var symbolCol = -1;
    for (var row = 0; row < rows; ++row)
    {
        for (var col = 0; col < cols; ++col)
        {
            var charToRead = lines[row][col];
            if (char.IsDigit(charToRead))
            {
                buffer.Append(charToRead);
                if (symbolRow >= 0 && symbolCol >= 0)
                    continue;
                // check if adjacent to left with symbol
                if (col > 0 && lines[row][col - 1] == gearSymbol)
                {
                    symbolRow = row;
                    symbolCol = col - 1;
                }
                // check if adjacent to right with symbol
                if (col < cols - 1 && lines[row][col + 1] == gearSymbol)
                {
                    symbolRow = row;
                    symbolCol = col + 1;
                }
                // check if adjacent to top with symbol
                if (row > 0 && lines[row - 1][col] == gearSymbol)
                {
                    symbolRow = row - 1;
                    symbolCol = col;
                }
                // check if adjacent to bottom with symbol
                if (row < rows - 1 && lines[row + 1][col] == gearSymbol)
                {
                    symbolRow = row + 1;
                    symbolCol = col;
                }

                // check if adjacent to top left with symbol
                if (col > 0 && row > 0 && lines[row - 1][col - 1] == gearSymbol)
                {
                    symbolRow = row - 1;
                    symbolCol = col - 1;
                }
                // check if adjacent to top left with symbol
                if (col < cols - 1 && row > 0 && lines[row - 1][col + 1] == gearSymbol)
                {
                    symbolRow = row - 1;
                    symbolCol = col + 1;
                }
                // check if adjacent to bottom left with symbol
                if (col < cols - 1 && row < rows - 1 && lines[row + 1][col + 1] == gearSymbol)
                {
                    symbolRow = row + 1;
                    symbolCol = col + 1;
                }
                // check if adjacent to bottom left with symbol
                if (col > 0 && row < rows - 1 && lines[row + 1][col - 1] == gearSymbol)
                {
                    symbolRow = row + 1;
                    symbolCol = col - 1;
                }
                continue;
            }
            // interrupted by non digit
            if (buffer.Length > 0 && symbolRow >= 0 && symbolCol >= 0)
            {
                var partNumber = int.Parse(buffer.ToString());
                // Console.WriteLine("Identified part number {0}", partNumber);
                var gearId = (symbolRow, symbolCol);
                numbers.Add((gearId, partNumber));
            }
            buffer.Clear();
            symbolRow = -1;
            symbolCol = -1;
        }
        // eol
        if (buffer.Length > 0 && symbolRow >= 0 && symbolCol >= 0)
        {
            var partNumber = int.Parse(buffer.ToString());
            // Console.WriteLine("Identified part number {0}", partNumber);
            var gearId = (symbolRow, symbolCol);
            numbers.Add((gearId, partNumber));
        }
        buffer.Clear();
        symbolRow = -1;
        symbolCol = -1;
    }

    // establish gear mesh relationship
    foreach (var number in numbers)
    {
        if (!gears.TryGetValue(number.Item1, out var gearParts))
        {
            gearParts = new List<int>();
            gears.Add(number.Item1, gearParts);
        }
        gearParts.Add(number.Item2);
    }

    // calculate sum of gear ratios
    var sum = 0;
    var gearNumber = 1; // just for debugging.
    foreach (var (gear, gearParts) in gears)
    {
        // gears must have two parts connected
        if (gearParts.Count != 2)
            continue;
        // Console.WriteLine("Identified valid gear {0}. Ratio of {1}.", gearNumber, gearParts[0] * gearParts[1]);
        sum += gearParts[0] * gearParts[1];
        gearNumber++;
    }

    return sum;
}