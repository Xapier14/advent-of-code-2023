using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 4);

var input = AdventOfCode.GetInputAsLines();

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