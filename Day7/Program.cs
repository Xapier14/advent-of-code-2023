using System.Text;
using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 7);

var cardRanking = new[] { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };
var jokerCardRanking = new[] { 'J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A' };

var sample = 
    """
    32T3K 765
    T55J5 684
    KK677 28
    KTJJT 220
    QQQJA 483
    """.Split(Environment.NewLine);
var input = AdventOfCode.GetInputLines();

Utility.Assert(Part1, sample, 6440);
Utility.Assert(Part2, sample, 5905);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

string ToPowerString(string rawHand)
{
    var builder = new StringBuilder();
    foreach (var c in rawHand)
    {
        builder.Append((char)('a' + GetPowerFromCard(c)));
    }

    return builder.ToString();
}

string FromPowerString(string powerString)
{
    var builder = new StringBuilder();
    foreach (var c in powerString)
    {
        builder.Append(GetCardFromPower(c - 'a'));
    }

    return builder.ToString();
}

int GetPowerFromCard(char card)
{
    var power = (long)Array.IndexOf(cardRanking!, card);
    if (power < 0)
        throw new Exception();
    return (int)power;
}

char GetCardFromPower(int power) 
    => cardRanking![power];


string ToJokerString(string rawHand)
{
    var builder = new StringBuilder();
    foreach (var c in rawHand)
    {
        builder.Append((char)('a' + GetJokerFromCard(c)));
    }

    return builder.ToString();
}

string FromJokerString(string powerString)
{
    var builder = new StringBuilder();
    foreach (var c in powerString)
    {
        builder.Append(GetCardFromJoker(c - 'a'));
    }

    return builder.ToString();
}

int GetJokerFromCard(char card)
{
    var power = (long)Array.IndexOf(jokerCardRanking!, card);
    if (power < 0)
        throw new Exception();
    return (int)power;
}

char GetCardFromJoker(int power) 
    => jokerCardRanking![power];

bool IsFiveOfAKindHand(string hand, bool disableJoker = true)
{
    var index = new Dictionary<char, int>();
    foreach (var c in hand)
    {
        index.Remove(c, out var oldCount);
        index.Add(c, oldCount + 1);
    }
    var hasFiveOfSameCard = index.Any(kp => kp.Value == 5);
    var hasFourOfSameNonJokerCardAndJoker = index.Any(kp => kp.Key != 'a' && kp.Value == 4)
                                            && index.Any(kp => kp.Key == 'a' && kp.Value >= 1)
                                            && !disableJoker;
    var hasThreeOfSameNonJokerCardAndJoker = index.Any(kp => kp.Key != 'a' && kp.Value == 3)
                                            && index.Any(kp => kp.Key == 'a' && kp.Value >= 2)
                                            && !disableJoker;
    var hasTwoOfSameNonJokerCardAndJoker = index.Any(kp => kp.Key != 'a' && kp.Value == 2)
                                             && index.Any(kp => kp.Key == 'a' && kp.Value >= 3)
                                             && !disableJoker;
    var hasOneOfSameNonJokerCardAndJoker = index.Any(kp => kp.Key != 'a' && kp.Value == 1)
                                           && index.Any(kp => kp.Key == 'a' && kp.Value >= 4)
                                           && !disableJoker;
    return hasFiveOfSameCard
           || hasFourOfSameNonJokerCardAndJoker
           || hasThreeOfSameNonJokerCardAndJoker
           || hasTwoOfSameNonJokerCardAndJoker
           || hasOneOfSameNonJokerCardAndJoker;
}

bool IsFourOfAKindHand(string hand, bool disableJoker = true)
{
    var index = new Dictionary<char, int>();
    foreach (var c in hand)
    {
        index.Remove(c, out var oldCount);
        index.Add(c, oldCount + 1);
    }
    var hasFourOfSameCard = index.Any(kp => kp.Value == 4);
    var hasThreeOfSameNonJokerCardAndJoker = index.Any(kp => kp.Key != 'a' && kp.Value == 3)
                                             && index.Any(kp => kp.Key == 'a' && kp.Value >= 1)
                                             && !disableJoker;
    var hasTwoOfSameNonJokerCardAndJoker = index.Any(kp => kp.Key != 'a' && kp.Value == 2)
                                           && index.Any(kp => kp.Key == 'a' && kp.Value >= 2)
                                           && !disableJoker;
    var hasOneOfSameNonJokerCardAndJoker = index.Any(kp => kp.Key != 'a' && kp.Value == 1)
                                           && index.Any(kp => kp.Key == 'a' && kp.Value >= 3)
                                           && !disableJoker;
    return hasFourOfSameCard
           || hasThreeOfSameNonJokerCardAndJoker
           || hasTwoOfSameNonJokerCardAndJoker
           || hasOneOfSameNonJokerCardAndJoker;
}

bool IsFullHouseHand(string hand, bool disableJoker = true)
{
    var index = new Dictionary<char, int>();
    foreach (var c in hand)
    {
        index.Remove(c, out var oldCount);
        index.Add(c, oldCount + 1);
    }
    var hasThreeOfSameCard = index.Any(kp => kp.Value == 3);
    var hasTwoOfSameCard = index.Any(kp => kp.Value == 2);
    var isFullHouse = hasThreeOfSameCard && hasTwoOfSameCard;
    var hasTwoPairsAndJoker = index.Count(kp => kp.Key != 'a' && kp.Value == 2) == 2
                              && index.Any(kp => kp.Key == 'a' && kp.Value == 1)
                              && !disableJoker;
    return isFullHouse || hasTwoPairsAndJoker;
}

bool IsThreeOfAKindHand(string hand, bool disableJoker = true)
{
    var index = new Dictionary<char, int>();
    foreach (var c in hand)
    {
        index.Remove(c, out var oldCount);
        index.Add(c, oldCount + 1);
    }
    var hasThreeOfSameCard = index.Any(kp => kp.Value == 3);
    var hasTwoOfSameCard = index.Any(kp => kp.Value == 2);
    var hasTwoOfSameNonJokerCardAndJoker = index.Any(kp => kp.Key != 'a' && kp.Value == 2)
                                           && index.Any(kp => kp.Key == 'a' && kp.Value >= 1)
                                           && !disableJoker;
    var hasOneOfSameNonJokerCardAndJoker = index.Any(kp => kp.Key != 'a' && kp.Value == 1)
                                           && index.Any(kp => kp.Key == 'a' && kp.Value >= 2)
                                           && !disableJoker;
    var isThreeOfAKind = hasThreeOfSameCard && !hasTwoOfSameCard;
    return isThreeOfAKind
        || hasTwoOfSameNonJokerCardAndJoker
        || hasOneOfSameNonJokerCardAndJoker;
}

bool IsTwoPairHand(string hand, bool disableJoker = true)
{
    var index = new Dictionary<char, int>();
    foreach (var c in hand)
    {
        index.Remove(c, out var oldCount);
        index.Add(c, oldCount + 1);
    }
    var hasTwoPairs = index.Count(kp => kp.Value == 2) == 2;
    var hasAtLeastTwoDifferentCardsAndJokers = index.Count(kp => kp.Key != 'a' && kp.Value == 1) >= 2
                                               && index.Any(kp => kp.Key == 'a' && kp.Value >= 2)
                                               && !disableJoker;
    var hasOnePairAndOneDifferentCardAndJoker = index.Any(kp => kp.Key != 'a' && kp.Value == 2)
                                                && index.Any(kp => kp.Key != 'a' && kp.Value == 1)
                                                && index.Any(kp => kp.Key == 'a' && kp.Value >= 1)
                                                && !disableJoker;
    return hasTwoPairs
        || hasAtLeastTwoDifferentCardsAndJokers
        || hasOnePairAndOneDifferentCardAndJoker;
}

bool IsPairHand(string hand, bool disableJoker = true)
{
    var index = new Dictionary<char, int>();
    foreach (var c in hand)
    {
        index.Remove(c, out var oldCount);
        index.Add(c, oldCount + 1);
    }
    var hasOnlyTwoOfSameKind = index.Count(kp => kp.Value == 2) == 1;
    var hasThreeOfSameKind = index.Any(kp => kp.Value == 3);
    var isPair = hasOnlyTwoOfSameKind && !hasThreeOfSameKind;
    var hasOneCardAndJoker = index.Any(kp => kp.Key != 'a' && kp.Value == 1)
                             && index.Any(kp => kp.Key == 'a' && kp.Value >= 1)
                             && !disableJoker;
    return isPair
        || hasOneCardAndJoker;
}

bool IsHighCardHand(string hand, bool disableJoker = true)
{
    var index = new Dictionary<char, int>();
    foreach (var c in hand)
    {
        index.Remove(c, out var oldCount);
        index.Add(c, oldCount + 1);
    }
    var isHighCardHand = index.All(kp => kp.Value == 1)
        && (disableJoker || index.Count(kp => kp is { Key: 'a' }) == 0);
    return isHighCardHand;
}

int Part1(string[] lines)
{
    var handBets = lines.Select(line =>
    {
        var split = line.Split(' ');
        return (ToPowerString(split[0]), int.Parse(split[1]));
    })
    .ToList();
    
    var fiveHandsBets = handBets.Where(tuple => IsFiveOfAKindHand(tuple.Item1)).ToList();
    fiveHandsBets.ForEach(tuple => handBets.Remove(tuple));
    var fiveHands = fiveHandsBets.Select(tuple => tuple.Item1).ToArray();
    var fiveBets = fiveHandsBets.Select(tuple => tuple.Item2).ToArray();
    Array.Sort(fiveHands, fiveBets);
    Array.Reverse(fiveHands);
    Array.Reverse(fiveBets);
    
    var fourHandsBets = handBets.Where(tuple => IsFourOfAKindHand(tuple.Item1)).ToList();
    fourHandsBets.ForEach(tuple => handBets.Remove(tuple));
    var fourHands = fourHandsBets.Select(tuple => tuple.Item1).ToArray();
    var fourBets = fourHandsBets.Select(tuple => tuple.Item2).ToArray();
    Array.Sort(fourHands, fourBets);
    Array.Reverse(fourHands);
    Array.Reverse(fourBets);
    
    var fullHouseHandsBets = handBets.Where(tuple => IsFullHouseHand(tuple.Item1)).ToList();
    fullHouseHandsBets.ForEach(tuple => handBets.Remove(tuple));
    var fullHouseHands = fullHouseHandsBets.Select(tuple => tuple.Item1).ToArray();
    var fullHouseBets = fullHouseHandsBets.Select(tuple => tuple.Item2).ToArray();
    Array.Sort(fullHouseHands, fullHouseBets);
    Array.Reverse(fullHouseHands);
    Array.Reverse(fullHouseBets);
    
    var trioHandsBets = handBets.Where(tuple => IsThreeOfAKindHand(tuple.Item1)).ToList();
    trioHandsBets.ForEach(tuple => handBets.Remove(tuple));
    var trioHands = trioHandsBets.Select(tuple => tuple.Item1).ToArray();
    var trioBets = trioHandsBets.Select(tuple => tuple.Item2).ToArray();
    Array.Sort(trioHands, trioBets);
    Array.Reverse(trioHands);
    Array.Reverse(trioBets);
    
    var twoPairHandsBets = handBets.Where(tuple => IsTwoPairHand(tuple.Item1)).ToList();
    twoPairHandsBets.ForEach(tuple => handBets.Remove(tuple));
    var twoPairHands = twoPairHandsBets.Select(tuple => tuple.Item1).ToArray();
    var twoPairBets = twoPairHandsBets.Select(tuple => tuple.Item2).ToArray();
    Array.Sort(twoPairHands, twoPairBets);
    Array.Reverse(twoPairHands);
    Array.Reverse(twoPairBets);
    
    var pairHandsBets = handBets.Where(tuple => IsPairHand(tuple.Item1)).ToList();
    pairHandsBets.ForEach(tuple => handBets.Remove(tuple));
    var pairHands = pairHandsBets.Select(tuple => tuple.Item1).ToArray();
    var pairBets = pairHandsBets.Select(tuple => tuple.Item2).ToArray();
    Array.Sort(pairHands, pairBets);
    Array.Reverse(pairHands);
    Array.Reverse(pairBets);
    
    var highCardHandsBets = handBets.Where(tuple => IsHighCardHand(tuple.Item1)).ToList();
    highCardHandsBets.ForEach(tuple => handBets.Remove(tuple));
    var highHands = highCardHandsBets.Select(tuple => tuple.Item1).ToArray();
    var highBets = highCardHandsBets.Select(tuple => tuple.Item2).ToArray();
    Array.Sort(highHands, highBets);
    Array.Reverse(highHands);
    Array.Reverse(highBets);

    var allHandsBets = new List<(string Hand, int Bet)>();
    for (var i = 0; i < fiveHandsBets.Count; ++i)
        allHandsBets.Add((fiveHands[i], fiveBets[i]));
    for (var i = 0; i < fourHandsBets.Count; ++i)
        allHandsBets.Add((fourHands[i], fourBets[i]));
    for (var i = 0; i < fullHouseHandsBets.Count; ++i)
        allHandsBets.Add((fullHouseHands[i], fullHouseBets[i]));
    for (var i = 0; i < trioHandsBets.Count; ++i)
        allHandsBets.Add((trioHands[i], trioBets[i]));
    for (var i = 0; i < twoPairHandsBets.Count; ++i)
        allHandsBets.Add((twoPairHands[i], twoPairBets[i]));
    for (var i = 0; i < pairHandsBets.Count; ++i)
        allHandsBets.Add((pairHands[i], pairBets[i]));
    for (var i = 0; i < highCardHandsBets.Count; ++i)
        allHandsBets.Add((highHands[i], highBets[i]));
    var allHands = allHandsBets.ToArray();
    
    int totalWinnings = 0;
    for (int i = 0; i < (int)allHands.Length; ++i)
    {
        var hand = FromPowerString(allHands[i].Item1);
        var bet = allHands[i].Item2;
        var rank = (int)allHands.Length - i;
        var winning = rank * bet;
        totalWinnings += winning;
    }
    
    return totalWinnings;
}

int Part2(string[] lines)
{
    var handBets = lines.Select(line =>
    {
        var split = line.Split(' ');
        return (ToJokerString(split[0]), int.Parse(split[1]));
    })
    .ToList();
    
    var fiveHandsBets = handBets.Where(tuple => IsFiveOfAKindHand(tuple.Item1, false)).ToList();
    fiveHandsBets.ForEach(tuple => handBets.Remove(tuple));
    var fiveHands = fiveHandsBets.Select(tuple => tuple.Item1).ToArray();
    var fiveBets = fiveHandsBets.Select(tuple => tuple.Item2).ToArray();
    Array.Sort(fiveHands, fiveBets);
    Array.Reverse(fiveHands);
    Array.Reverse(fiveBets);
    
    var fourHandsBets = handBets.Where(tuple => IsFourOfAKindHand(tuple.Item1, false)).ToList();
    fourHandsBets.ForEach(tuple => handBets.Remove(tuple));
    var fourHands = fourHandsBets.Select(tuple => tuple.Item1).ToArray();
    var fourBets = fourHandsBets.Select(tuple => tuple.Item2).ToArray();
    Array.Sort(fourHands, fourBets);
    Array.Reverse(fourHands);
    Array.Reverse(fourBets);
    
    var fullHouseHandsBets = handBets.Where(tuple => IsFullHouseHand(tuple.Item1, false)).ToList();
    fullHouseHandsBets.ForEach(tuple => handBets.Remove(tuple));
    var fullHouseHands = fullHouseHandsBets.Select(tuple => tuple.Item1).ToArray();
    var fullHouseBets = fullHouseHandsBets.Select(tuple => tuple.Item2).ToArray();
    Array.Sort(fullHouseHands, fullHouseBets);
    Array.Reverse(fullHouseHands);
    Array.Reverse(fullHouseBets);
    
    var trioHandsBets = handBets.Where(tuple => IsThreeOfAKindHand(tuple.Item1, false)).ToList();
    trioHandsBets.ForEach(tuple => handBets.Remove(tuple));
    var trioHands = trioHandsBets.Select(tuple => tuple.Item1).ToArray();
    var trioBets = trioHandsBets.Select(tuple => tuple.Item2).ToArray();
    Array.Sort(trioHands, trioBets);
    Array.Reverse(trioHands);
    Array.Reverse(trioBets);
    
    var twoPairHandsBets = handBets.Where(tuple => IsTwoPairHand(tuple.Item1, false)).ToList();
    twoPairHandsBets.ForEach(tuple => handBets.Remove(tuple));
    var twoPairHands = twoPairHandsBets.Select(tuple => tuple.Item1).ToArray();
    var twoPairBets = twoPairHandsBets.Select(tuple => tuple.Item2).ToArray();
    Array.Sort(twoPairHands, twoPairBets);
    Array.Reverse(twoPairHands);
    Array.Reverse(twoPairBets);
    
    var pairHandsBets = handBets.Where(tuple => IsPairHand(tuple.Item1, false)).ToList();
    pairHandsBets.ForEach(tuple => handBets.Remove(tuple));
    var pairHands = pairHandsBets.Select(tuple => tuple.Item1).ToArray();
    var pairBets = pairHandsBets.Select(tuple => tuple.Item2).ToArray();
    Array.Sort(pairHands, pairBets);
    Array.Reverse(pairHands);
    Array.Reverse(pairBets);
    
    var highCardHandsBets = handBets.Where(tuple => IsHighCardHand(tuple.Item1, false)).ToList();
    highCardHandsBets.ForEach(tuple => handBets.Remove(tuple));
    var highHands = highCardHandsBets.Select(tuple => tuple.Item1).ToArray();
    var highBets = highCardHandsBets.Select(tuple => tuple.Item2).ToArray();
    Array.Sort(highHands, highBets);
    Array.Reverse(highHands);
    Array.Reverse(highBets);
    
    var allHandsBets = new List<(string Hand, int Bet)>();
    for (var i = 0; i < fiveHandsBets.Count; ++i)
        allHandsBets.Add((fiveHands[i], fiveBets[i]));
    for (var i = 0; i < fourHandsBets.Count; ++i)
        allHandsBets.Add((fourHands[i], fourBets[i]));
    for (var i = 0; i < fullHouseHandsBets.Count; ++i)
        allHandsBets.Add((fullHouseHands[i], fullHouseBets[i]));
    for (var i = 0; i < trioHandsBets.Count; ++i)
        allHandsBets.Add((trioHands[i], trioBets[i]));
    for (var i = 0; i < twoPairHandsBets.Count; ++i)
        allHandsBets.Add((twoPairHands[i], twoPairBets[i]));
    for (var i = 0; i < pairHandsBets.Count; ++i)
        allHandsBets.Add((pairHands[i], pairBets[i]));
    for (var i = 0; i < highCardHandsBets.Count; ++i)
        allHandsBets.Add((highHands[i], highBets[i]));
    var allHands = allHandsBets.ToArray();

    int totalWinnings = 0;
    for (int i = 0; i < allHands.Length; ++i)
    {
        var hand = FromJokerString(allHands[i].Item1);
        var bet = allHands[i].Item2;
        var rank = allHands.Length - i;
        var winning = rank * bet;
        totalWinnings += winning;
    }
    
    return totalWinnings;
}