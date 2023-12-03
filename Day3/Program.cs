using System.Security.Cryptography;
using System.Text;

bool IsSymbol(char c)
    => c != '.' && !char.IsLetterOrDigit(c);

bool IsGear(char c)
    => c == '*';

string HashString(string data)
{
    byte[] buffer = Encoding.UTF8.GetBytes(data);
    return Encoding.UTF8.GetString(SHA256.HashData(buffer));
}

int Part1(string filePath)
{
    var sum = 0;

    var lines = File.ReadAllLines(filePath);
    var cols = lines[0].Length;
    var rows = lines.Length;

    var matrix = new char[rows, cols];
    for (var row = 0; row < rows; ++row)
        for (var col = 0; col < cols; ++col)
            matrix[row, col] = lines[row][col];

    var buffer = new StringBuilder();
    var bufferIsNearSymbol = false;
    for (var row = 0; row < rows; ++row)
    {
        for (var col = 0; col < cols; ++col)
        {
            var charToRead = matrix[row, col];
            if (char.IsDigit(charToRead))
            {
                buffer.Append(charToRead);
                // check if adjacent to left with symbol
                if (col > 0 && IsSymbol(matrix[row, col - 1]))
                    bufferIsNearSymbol = true;
                // check if adjacent to right with symbol
                if (col < cols - 1 && IsSymbol(matrix[row, col + 1]))
                    bufferIsNearSymbol = true;
                // check if adjacent to top with symbol
                if (row > 0 && IsSymbol(matrix[row - 1, col]))
                    bufferIsNearSymbol = true;
                // check if adjacent to bottom with symbol
                if (row < rows - 1 && IsSymbol(matrix[row + 1, col]))
                    bufferIsNearSymbol = true;

                // check if adjacent to top left with symbol
                if (col > 0 && row > 0 && IsSymbol(matrix[row - 1, col - 1]))
                    bufferIsNearSymbol = true;
                // check if adjacent to top left with symbol
                if (col < cols - 1 && row > 0 && IsSymbol(matrix[row - 1, col + 1]))
                    bufferIsNearSymbol = true;
                // check if adjacent to bottom left with symbol
                if (col < cols - 1 && row < rows - 1 && IsSymbol(matrix[row + 1, col + 1]))
                    bufferIsNearSymbol = true;
                // check if adjacent to bottom left with symbol
                if (col > 0 && row < rows - 1 && IsSymbol(matrix[row + 1, col - 1]))
                    bufferIsNearSymbol = true;

            }
            else
            {
                // interrupted by non digit
                if (buffer.Length > 0 && bufferIsNearSymbol)
                {
                    var partNumber = int.Parse(buffer.ToString());
                    Console.WriteLine("Identified part number {0}", partNumber);
                    sum += partNumber;
                }
                buffer.Clear();
                bufferIsNearSymbol = false;
            }
        }
        // eol
        if (buffer.Length > 0 && bufferIsNearSymbol)
        {
            var partNumber = int.Parse(buffer.ToString());
            Console.WriteLine("Identified part number {0}", partNumber);
            sum += partNumber;
        }
        buffer.Clear();
        bufferIsNearSymbol = false;
    }

    return sum;
}

int Part2(string filePath)
{
    var lines = File.ReadAllLines(filePath);
    var cols = lines[0].Length;
    var rows = lines.Length;

    var matrix = new char[rows, cols];
    for (var row = 0; row < rows; ++row)
        for (var col = 0; col < cols; ++col)
            matrix[row, col] = lines[row][col];

    var numbers = new List<(string, int)>();
    var gears = new Dictionary<string, List<int>>();

    var buffer = new StringBuilder();
    // symbol now just refers to '*'
    var bufferIsNearSymbol = false;
    var symbolRow = -1;
    var symbolCol = -1;
    for (var row = 0; row < rows; ++row)
    {
        for (var col = 0; col < cols; ++col)
        {
            var charToRead = matrix[row, col];
            if (char.IsDigit(charToRead))
            {
                buffer.Append(charToRead);
                // check if adjacent to left with symbol
                if (col > 0 && IsGear(matrix[row, col - 1]) && !bufferIsNearSymbol)
                {
                    symbolRow = row;
                    symbolCol = col - 1;
                    bufferIsNearSymbol = true;
                }
                // check if adjacent to right with symbol
                if (col < cols - 1 && IsGear(matrix[row, col + 1]) && !bufferIsNearSymbol)
                {
                    symbolRow = row;
                    symbolCol = col + 1;
                    bufferIsNearSymbol = true;
                }
                // check if adjacent to top with symbol
                if (row > 0 && IsGear(matrix[row - 1, col]) && !bufferIsNearSymbol)
                {
                    symbolRow = row - 1;
                    symbolCol = col;
                    bufferIsNearSymbol = true;
                }
                // check if adjacent to bottom with symbol
                if (row < rows - 1 && IsGear(matrix[row + 1, col]) && !bufferIsNearSymbol)
                {
                    symbolRow = row + 1;
                    symbolCol = col;
                    bufferIsNearSymbol = true;
                }

                // check if adjacent to top left with symbol
                if (col > 0 && row > 0 && IsGear(matrix[row - 1, col - 1]) && !bufferIsNearSymbol)
                {
                    symbolRow = row - 1;
                    symbolCol = col - 1;
                    bufferIsNearSymbol = true;
                }
                // check if adjacent to top left with symbol
                if (col < cols - 1 && row > 0 && IsGear(matrix[row - 1, col + 1]) && !bufferIsNearSymbol)
                {
                    symbolRow = row - 1;
                    symbolCol = col + 1;
                    bufferIsNearSymbol = true;
                }
                // check if adjacent to bottom left with symbol
                if (col < cols - 1 && row < rows - 1 && IsGear(matrix[row + 1, col + 1]) && !bufferIsNearSymbol)
                {
                    symbolRow = row + 1;
                    symbolCol = col + 1;
                    bufferIsNearSymbol = true;
                }
                // check if adjacent to bottom left with symbol
                if (col > 0 && row < rows - 1 && IsGear(matrix[row + 1, col - 1]) && !bufferIsNearSymbol)
                {
                    symbolRow = row + 1;
                    symbolCol = col - 1;
                    bufferIsNearSymbol = true;
                }

            }
            else
            {
                // interrupted by non digit
                if (buffer.Length > 0 && bufferIsNearSymbol)
                {
                    var partNumber = int.Parse(buffer.ToString());
                    Console.WriteLine("Identified part number {0}", partNumber);
                    var gearId = HashString($"{symbolRow}, {symbolCol}");
                    numbers.Add((gearId, partNumber));
                }
                buffer.Clear();
                bufferIsNearSymbol = false;
                symbolRow = -1;
                symbolCol = -1;
            }
        }
        // eol
        if (buffer.Length > 0 && bufferIsNearSymbol)
        {
            var partNumber = int.Parse(buffer.ToString());
            Console.WriteLine("Identified part number {0}", partNumber);
            var gearId = HashString($"{symbolRow}, {symbolCol}");
            numbers.Add((gearId, partNumber));
        }
        buffer.Clear();
        bufferIsNearSymbol = false;
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
        Console.WriteLine("Identified valid gear {0}. Ratio of {1}.", gearNumber, gearParts[0] * gearParts[1]);
        sum += gearParts[0] * gearParts[1];
        gearNumber++;
    }

    return sum;
}

Console.WriteLine(Part2("input.txt"));