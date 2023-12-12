using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 8);

var part1Sample1 =
    """
    RL
    
    AAA = (BBB, CCC)
    BBB = (DDD, EEE)
    CCC = (ZZZ, GGG)
    DDD = (DDD, DDD)
    EEE = (EEE, EEE)
    GGG = (GGG, GGG)
    ZZZ = (ZZZ, ZZZ)
    """.Split(Environment.NewLine);
var part1Sample2 =
    """
    LLR

    AAA = (BBB, BBB)
    BBB = (AAA, ZZZ)
    ZZZ = (ZZZ, ZZZ)
    """.Split(Environment.NewLine);
var part2Sample1 =
    """
    LR

    11A = (11B, XXX)
    11B = (XXX, 11Z)
    11Z = (11B, XXX)
    22A = (22B, XXX)
    22B = (22C, 22C)
    22C = (22Z, 22Z)
    22Z = (22B, 22B)
    XXX = (XXX, XXX)
    """.Split(Environment.NewLine);

var input = AdventOfCode.GetInputLines();

Utility.Assert(Part1, part1Sample1, 2);
Utility.Assert(Part1, part1Sample2, 6);
Utility.Assert(Part2, part2Sample1, 6);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

long Part1(string[] lines)
{
    var path = lines[0].Trim();

    var nodeDictionary = new Dictionary<string, (Node Node, string[] Children)>();
    var nodeAssignments = lines.Skip(2);
    foreach (var line in nodeAssignments)
    {
        var assignment = line.Split('=', StringSplitOptions.TrimEntries);
        var nodeName = assignment[0];
        var nodeChildren = assignment[1].Trim('(', ')').Split(',', StringSplitOptions.TrimEntries)
            .ToArray();
        if (nodeChildren.All(name => name == nodeName))
            nodeChildren = Array.Empty<string>();
        var node = new Node(nodeName);
        nodeDictionary.Add(nodeName, (node, nodeChildren));
    }
    BuildNodes(nodeDictionary);
    var currentNode = nodeDictionary["AAA"].Node;
    var moves = 0;
    while (currentNode?.Name != "ZZZ")
        foreach (var direction in path)
        {
            currentNode = currentNode![direction];
            moves++;
        }

    return moves;
}

long Part2(string[] lines)
{
    var path = lines[0].Trim();

    var nodeDictionary = new Dictionary<string, (Node Node, string[] Children)>();
    var nodeAssignments = lines.Skip(2);
    foreach (var line in nodeAssignments)
    {
        var assignment = line.Split('=', StringSplitOptions.TrimEntries);
        var nodeName = assignment[0];
        var nodeChildren = assignment[1].Trim('(', ')').Split(',', StringSplitOptions.TrimEntries)
            .ToArray();
        if (nodeChildren.All(name => name == nodeName))
            nodeChildren = Array.Empty<string>();
        var node = new Node(nodeName);
        nodeDictionary.Add(nodeName, (node, nodeChildren));
    }
    BuildNodes(nodeDictionary);
    var currentNodeList = new List<Node>();
    foreach (var (name, (node, _)) in nodeDictionary)
        if (name.EndsWith('A'))
            currentNodeList.Add(node);
    var currentNodes = currentNodeList.ToArray();
    
    var moves = 0;
    var endNodeMoves = new double[currentNodes.Length];
    Array.Fill(endNodeMoves, -1);
    while (endNodeMoves.Any(length => length < 0))
    {
        foreach (var direction in path)
        {
            moves++;
            var newNodeList = new List<Node>();
            for (var i = 0; i < currentNodes.Length; ++i)
            {
                if (endNodeMoves[i] >= 0)
                {
                    newNodeList.Add(currentNodes[i]);
                    continue;
                }

                var toVisitNode = currentNodes[i][direction]!;
                newNodeList.Add(toVisitNode);
                if (toVisitNode.Name.EndsWith('Z'))
                {
                    endNodeMoves[i] = moves;
                }
            }
            currentNodes = newNodeList.ToArray();
        }
    }

    var lcm = endNodeMoves[0];
    var skipped = endNodeMoves.Skip(1);
    lcm = skipped.Aggregate(lcm, Lcm);

    return (long)lcm;
}

void BuildNodes(Dictionary<string, (Node Node, string[] Children)> dictionary)
{
    foreach (var (nodeName, (node, children)) in dictionary)
    {
        if (children.Length == 0)
            continue;
        if (children[0] != nodeName)
            node.LeftNode = dictionary[children[0]].Node;
        if (children[1] != nodeName)
            node.RightNode = dictionary[children[1]].Node;
    }
}

double Gcf(double a, double b)
{
    while (b != 0)
    {
        var temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}

double Lcm(double a, double b)
{
    return (a / Gcf(a, b)) * b;
}

public class Node
{
    private Node? _left, _right;
    public Node? Parent { get; set; }
    public string Name { get; init; }
    
    public Node? LeftNode
    {
        get => _left;
        set
        {
            if (value != null)
                value.Parent = this;
            _left = value;
        }
    }

    public Node? RightNode
    {
        get => _right;
        set
        {
            if (value != null)
                value.Parent = this;
            _right = value;
        }
    }
    
    public Node(string name)
    {
        Name = name;
    }

    public Node? this[char direction]
        => char.ToUpper(direction) switch
        {
            'L' => LeftNode,
            'R' => RightNode,
            _ => throw new Exception($"Invalid direction '{direction}'.")
        };
}