using System.Collections.Immutable;
using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 19);

var input = AdventOfCode.GetInputText();

var sample =
    """
    px{a<2006:qkq,m>2090:A,rfg}
    pv{a>1716:R,A}
    lnx{m>1548:A,A}
    rfg{s<537:gd,x>2440:R,A}
    qs{s>3448:A,lnx}
    qkq{x<1416:A,crn}
    crn{x>2662:A,R}
    in{s<1351:px,qqz}
    qqz{s>2770:qs,m<1801:hdj,R}
    gd{a>3333:R,R}
    hdj{m>838:A,pv}
    
    {x=787,m=2655,a=1222,s=2876}
    {x=1679,m=44,a=2067,s=496}
    {x=2036,m=264,a=79,s=2244}
    {x=2461,m=1339,a=466,s=291}
    {x=2127,m=1623,a=2188,s=1013}
    """;

Utility.Assert(Part1, sample, 19114);
Utility.Assert(Part2, sample, 167409079868000);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

((int X, int M , int A, int S)[] Accepted, (int X, int M , int A, int S)[] Rejected) RunStateMachine(
    IReadOnlyDictionary<string, (bool Default, char Var, char Condition, int Num, string AcceptLabel)[]> workflows,
    (int X, int M , int A, int S)[] inputs)
{
    var inputQueue = new Queue<(int X, int M, int A, int S)>();
    foreach (var input in inputs)
        inputQueue.Enqueue(input);
    var acceptedInputs = new List<(int X, int M , int A, int S)>();
    var rejectedInputs = new List<(int X, int M, int A, int S)>();
    while (inputQueue.TryDequeue(out var input))
    {
        var currentState = "in";
        while (currentState != "A" && currentState != "R")
        {
            if (!workflows.TryGetValue(currentState, out var workflow))
                throw new Exception("Workflow not found.");
            foreach (var (def, var, condition, num, acceptLabel) in workflow)
            {
                if (def)
                {
                    currentState = acceptLabel;
                    break;
                }

                var inputCompare = var switch
                {
                    'x' => input.X,
                    'm' => input.M,
                    'a' => input.A,
                    's' => input.S,
                    _ => throw new Exception("Invalid variable.")
                };
                var result = condition switch
                {
                    '<' => inputCompare < num,
                    '>' => inputCompare > num,
                    _ => throw new Exception("Invalid condition.")
                };
                if (!result)
                    continue;
                currentState = acceptLabel;
                break;
            }
        }
        if (currentState == "A")
            acceptedInputs.Add(input);
        else
            rejectedInputs.Add(input);
    }

    return (acceptedInputs.ToArray(), rejectedInputs.ToArray());
}

long Part1(string text)
{
    var rawWorkflows = text.ReplaceLineEndings("\n").Split("\n\n")[0].Split("\n", StringSplitOptions.RemoveEmptyEntries);
    var rawInputs = text.ReplaceLineEndings("\n").Split("\n\n")[1].Split("\n", StringSplitOptions.RemoveEmptyEntries);
    var workflows = new Dictionary<string, (bool Default, char Var, char Condition, int Num, string AcceptLabel)[]>();
    foreach (var rawWorkflow in rawWorkflows)
    {
        var label = rawWorkflow.Split('{')[0];
        var rawRules = rawWorkflow.Split('{')[1].TrimEnd('}').Split(',');
        var rules = new List<(bool Default, char Var, char Condition, int Num, string AcceptLabel)>();
        foreach (var rawRule in rawRules)
        {
            if (!rawRule.Contains(':'))
            {
                rules.Add((true, ' ', ' ', 0, rawRule));
                break;
            }

            var var = rawRule[0];
            var condition = rawRule[1];
            var num = int.Parse(rawRule.Split(condition)[1].Split(':')[0]);
            var acceptLabel = rawRule.Split(':')[1];
            rules.Add((false, var, condition, num, acceptLabel));
        }
        workflows.Add(label, rules.ToArray());
    }

    var inputs = rawInputs.Select(rawInput =>
    {
        var input = rawInput.Trim('{', '}').Split(',');
        var x = int.Parse(input[0].Split('=')[1]);
        var m = int.Parse(input[1].Split('=')[1]);
        var a = int.Parse(input[2].Split('=')[1]);
        var s = int.Parse(input[3].Split('=')[1]);
        return (x, m, a, s);
    }).ToArray();

    var (accepted, rejected) = RunStateMachine(workflows, inputs);

    return accepted.Sum(input => input.X + input.M + input.A + input.S);
}

Dictionary<string, List<(string PrevLabel, (char Var, char Condition, int Num, bool Inverse)[] Conditions)>> GenerateReverseFsm(
    Dictionary<string, (bool Default, char Var, char Condition, int Num, string AcceptLabel)[]> workflows)
{
    var ret = new Dictionary<string, List<(string PrevLabel, (char Var, char Condition, int Num, bool Inverse)[] Conditions)>>();
    foreach (var (prevLabel, nextNodes) in workflows)
    {
        var negations = ImmutableArray.Create<(char var, char Condition, int Num, bool Inverse)>();
        foreach (var nextNode in nextNodes)
        {
            var label = nextNode.AcceptLabel;
            if (!ret.TryGetValue(label, out var nearList))
            {
                nearList = new List<(string PrevLabel, (char Var, char Condition, int Num, bool Inverse)[] Conditions)>();
                ret.Add(label, nearList);
            }

            if (nextNode.Default)
            {
                nearList.Add((prevLabel, negations.Add((' ', ' ', 0, false)).ToArray()));
                continue;
            }
            var condition = nextNode.Condition;
            var var = nextNode.Var;
            var num = nextNode.Num;
            nearList.Add((prevLabel, negations.Add((var, condition, num, false)).ToArray()));
            negations = negations.Add((var, condition, num, true));
        }
    }

    return ret;
}

List<(string PrevLabel, (char Var, char Condition, int Num, bool Inverse)[] Conditions)[]> GetRulesToNavigate(
    IReadOnlyDictionary<string, List<(string PrevLabel, (char Var, char Condition, int Num, bool Inverse)[] Conditions)>> reverseFsm, string fromLabel,
    string toLabel)
{
    var discoverQueue = new Queue<ImmutableArray<(string PrevLabel, (char Var, char Condition, int Num, bool Inverse)[])>>();
    var paths = new List<(string PrevLabel, (char Var, char Condition, int Num, bool Inverse)[] Conditions)[]>();
    if (!reverseFsm.TryGetValue(fromLabel, out var startingRange))
        throw new Exception($"reverseFsm does not have key {fromLabel}");
    foreach (var starting in startingRange)
    {
        discoverQueue.Enqueue(ImmutableArray.Create(starting));
    }
    while (discoverQueue.TryDequeue(out var array))
    {
        var lastRule = array.Last();
        if (lastRule.PrevLabel == toLabel)
        {
            paths.Add(array.ToArray());
            continue;
        }

        var nearLabels = reverseFsm[lastRule.PrevLabel];
        foreach (var nearLabel in nearLabels)
        {
            discoverQueue.Enqueue(array.Add(nearLabel));
        }
    }

    return paths;
}

long CalculateCombinations(List<(string PrevLabel, (char Var, char Condition, int Num, bool Inverse)[] Conditions)[]> paths)
{
    var sum = 0L;
    foreach (var path in paths)
    {
        var xMin = 1;
        var xMax = 4000;
        var mMin = 1;
        var mMax = 4000;
        var aMin = 1;
        var aMax = 4000;
        var sMin = 1;
        var sMax = 4000;
        foreach (var (_, conditions) in path)
        {
            foreach (var (var, condition, num, inverse) in conditions)
            {
                var isDefault = var == ' ' || condition == ' ';
                if (isDefault)
                    continue;
                if (!inverse)
                {
                    if (condition == '>')
                    {
                        switch (var)
                        {
                            case 'x':
                                xMin = Math.Max(xMin, num + 1);
                                break;
                            case 'm':
                                mMin = Math.Max(mMin, num + 1);
                                break;
                            case 'a':
                                aMin = Math.Max(aMin, num + 1);
                                break;
                            case 's':
                                sMin = Math.Max(sMin, num + 1);
                                break;
                        }
                    }
                    else
                    {
                        switch (var)
                        {
                            case 'x':
                                xMax = Math.Min(xMax, num - 1);
                                break;
                            case 'm':
                                mMax = Math.Min(mMax, num - 1);
                                break;
                            case 'a':
                                aMax = Math.Min(aMax, num - 1);
                                break;
                            case 's':
                                sMax = Math.Min(sMax, num - 1);
                                break;
                        }
                    }
                }
                else
                {
                    // inverse
                    if (condition == '>') // <=
                    {
                        switch (var)
                        {
                            case 'x':
                                xMax = Math.Min(xMax, num);
                                break;
                            case 'm':
                                mMax = Math.Min(mMax, num);
                                break;
                            case 'a':
                                aMax = Math.Min(aMax, num);
                                break;
                            case 's':
                                sMax = Math.Min(sMax, num);
                                break;
                        }
                    }
                    else // >=
                    {
                        switch (var)
                        {
                            case 'x':
                                xMin = Math.Max(xMin, num);
                                break;
                            case 'm':
                                mMin = Math.Max(mMin, num);
                                break;
                            case 'a':
                                aMin = Math.Max(aMin, num);
                                break;
                            case 's':
                                sMin = Math.Max(sMin, num);
                                break;
                        }
                    }
                }
            }
        }
        long xRange = xMax - xMin + 1;
        long mRange = mMax - mMin + 1;
        long aRange = aMax - aMin + 1;
        long sRange = sMax - sMin + 1;
        var total = xRange * mRange * aRange * sRange;
        sum += total;
    }
    return sum;
}

long Part2(string text)
{
    var rawWorkflows = text.ReplaceLineEndings("\n").Split("\n\n")[0].Split("\n", StringSplitOptions.RemoveEmptyEntries);
    var workflows = new Dictionary<string, (bool Default, char Var, char Condition, int Num, string AcceptLabel)[]>();
    foreach (var rawWorkflow in rawWorkflows)
    {
        var label = rawWorkflow.Split('{')[0];
        var rawRules = rawWorkflow.Split('{')[1].TrimEnd('}').Split(',');
        var rules = new List<(bool Default, char Var, char Condition, int Num, string AcceptLabel)>();
        foreach (var rawRule in rawRules)
        {
            if (!rawRule.Contains(':'))
            {
                rules.Add((true, ' ', ' ', 0, rawRule));
                break;
            }

            var var = rawRule[0];
            var condition = rawRule[1];
            var num = int.Parse(rawRule.Split(condition)[1].Split(':')[0]);
            var acceptLabel = rawRule.Split(':')[1];
            rules.Add((false, var, condition, num, acceptLabel));
        }
        workflows.Add(label, rules.ToArray());
    }

    var reverseFsm = GenerateReverseFsm(workflows);

    var paths = GetRulesToNavigate(reverseFsm, "A", "in");
    return CalculateCombinations(paths);
}