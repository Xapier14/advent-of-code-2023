using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 4);

var input = AdventOfCode.GetInputLines();

var sample =
    """
    Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
    Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
    Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
    Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
    Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
    Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
    """.Split(Environment.NewLine);

Utility.Assert(Part1, sample, 13);
Utility.Assert(Part2, sample, 30);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

int Part1(string[] lines)
{
    var splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    var cards = lines.Select(line => line.Split(':', splitOptions))
        .Select(splitLines => splitLines[1])
        .Select(card => card.Split('|', splitOptions))
        .Select(lines => lines.Select(line => line
            .Split(' ', splitOptions)
            .Select(int.Parse)
            .ToArray()
        ).ToArray())
        .ToArray();

    return cards.Sum(line => CalculateCardPoints(line, out _));
}

int Part2(string[] lines)
{
    var splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    var cardTable = new Dictionary<int, CardMetadata>();
    var cards = lines.Select(line => line.Split(':', splitOptions)[1])
        .Select(cardNumbers => cardNumbers.Split('|', splitOptions))
        .Select(lines => lines.Select(line => line
            .Split(' ', splitOptions)
            .Select(int.Parse)
            .ToArray()
        ).ToArray())
        .ToArray();

    for (var i = 0; i < cards.Length; ++i)
        cardTable.Add(i + 1, new CardMetadata
        {
            CardLines = cards[i],
            Quantity = 1
        });

    foreach (var (cardNumber, card) in cardTable)
    {
        var multiplier = card.Quantity;
        _ = CalculateCardPoints(card.CardLines, out var matchingNumbers);
        for (var i = 0; i < matchingNumbers; ++i) cardTable[cardNumber + i + 1].Quantity += multiplier;
    }

    var totalCards = cardTable.Values.Sum(card => card.Quantity);

    return totalCards;
}

int CalculateCardPoints(int[][] cardLines, out int matchingNumbers)
{
    matchingNumbers = 0;
    var winningNumbers = cardLines[0];
    var cardNumbers = cardLines[1];
    var cardPoints = 0;

    foreach (var cardNumber in cardNumbers)
    {
        if (!winningNumbers.Contains(cardNumber))
            continue;
        matchingNumbers++;

        if (cardPoints == 0)
            cardPoints = 1;
        else
            cardPoints *= 2;
    }

    return cardPoints;
}

public class CardMetadata
{
    public int[][] CardLines = Array.Empty<int[]>();
    public int Quantity;
}