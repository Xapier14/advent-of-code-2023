using Xapier14.AdventOfCode;
AdventOfCode.SetYearAndDay(2023, 20);

var input = AdventOfCode.GetInputLines();

var sample =
    """
    broadcaster -> a, b, c
    %a -> b
    %b -> c
    %c -> inv
    &inv -> a
    """.Split(Environment.NewLine);
var sample2 =
    """
    broadcaster -> a
    %a -> inv, con
    &inv -> b
    %b -> con
    &con -> output
    """.Split(Environment.NewLine);

Utility.Assert(Part1, sample, 32000000);
Utility.Assert(Part1, sample2, 11687500);

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

long Part1(string[] lines)
{
    var machine = new Machine();
    var relationships = new List<(string input, string output)>();
    foreach (var rawModule in lines)
    {
        var rawModuleName = rawModule.Split(" -> ")[0];
        var moduleName = rawModuleName;
        var outputs = rawModule.Split(" -> ")[1].Split(',', StringSplitOptions.TrimEntries);
        var isBroadcaster = rawModuleName == "broadcaster";
        var isFlipFlop = rawModuleName[0] == '%';
        var isConjunction = rawModuleName[0] == '&';
        IModule module;
        if (isBroadcaster)
        {
            module = new BroadcasterModule(machine);
        } else if (isFlipFlop)
        {
            moduleName = rawModuleName[1..];
            module = new FlipFlopModule(moduleName, machine);
        } else if (isConjunction)
        {
            moduleName = rawModuleName[1..];
            module = new ConjunctionModule(moduleName, machine);
        }
        else
        {
            throw new Exception("Unknown module type.");
        }
        machine.Modules.Add(moduleName, module);
        relationships.AddRange(outputs.Select(output => (moduleName, output)));
    }

    foreach (var (input, output) in relationships)
    {
        if (!machine.Modules.ContainsKey(output))
            machine.Modules[output] = new DummyModule(output, machine);
        var outputModule = machine.Modules[output];
        var inputModule = machine.Modules[input];
        outputModule.AddInput(inputModule);
    }

    for (var i = 0; i < 1000; ++i)
        machine.PressButton();
    return machine.LowSignals * machine.HighSignals;
}

long Part2(string[] lines)
{
    var machine = new Machine();
    var relationships = new List<(string input, string output)>();
    foreach (var rawModule in lines)
    {
        var rawModuleName = rawModule.Split(" -> ")[0];
        var moduleName = rawModuleName;
        var outputs = rawModule.Split(" -> ")[1].Split(',', StringSplitOptions.TrimEntries);
        var isBroadcaster = rawModuleName == "broadcaster";
        var isFlipFlop = rawModuleName[0] == '%';
        var isConjunction = rawModuleName[0] == '&';
        IModule module;
        if (isBroadcaster)
        {
            module = new BroadcasterModule(machine);
        } else if (isFlipFlop)
        {
            moduleName = rawModuleName[1..];
            module = new FlipFlopModule(moduleName, machine);
        } else if (isConjunction)
        {
            moduleName = rawModuleName[1..];
            module = new ConjunctionModule(moduleName, machine);
        }
        else
        {
            throw new Exception("Unknown module type.");
        }
        machine.Modules.Add(moduleName, module);
        relationships.AddRange(outputs.Select(output => (moduleName, output)));
    }

    foreach (var (input, output) in relationships)
    {
        if (output == "rx")
            machine.Modules.Add("rx", new RxModule("rx", machine));
        else if (!machine.Modules.ContainsKey(output))
            machine.Modules[output] = new DummyModule(output, machine);

        var outputModule = machine.Modules[output];
        var inputModule = machine.Modules[input];
        outputModule.AddInput(inputModule);
    }

    return machine.RxLoop();
}

enum Signal { Low = 0, High = 1 }

static class Signals
{
    public static Signal Low = Signal.Low;
    public static Signal High = Signal.High;

    public static Signal Flip(Signal signal)
        => signal == Signal.High ? Signal.Low : Signal.High;

    public static bool IsHigh(Signal signal)
        => signal == Signal.High;

    public static bool IsLow(Signal signal)
        => signal ==  Signal.Low;

    public static string ToString(Signal signal)
        => IsHigh(signal) ? "high" : "low";
}

class Machine
{
    private readonly Dictionary<IModule, Signal> _lastOutputSignal = new();
    private readonly Dictionary<string, IModule> _modules = new();
    private readonly Queue<(IModule FromModule, IModule ToModule, Signal Signal)> _pulseQueue = new();
    private int _highSignals, _lowSignals;

    public Dictionary<IModule, Signal> LastOutputSignal => _lastOutputSignal;
    public Queue<(IModule, IModule, Signal)> PulseQueue => _pulseQueue;
    public Dictionary<string, IModule> Modules => _modules;
    public int HighSignals => _highSignals;
    public int LowSignals => _lowSignals;

    public void PressButton()
    {
        var broadcaster = _modules["broadcaster"];
        var button = new DummyModule("button", this);
        _pulseQueue.Enqueue((button, broadcaster, Signal.Low));
        while (_pulseQueue.TryDequeue(out var instruction))
        {
            if (Signals.IsHigh(instruction.Signal))
                _highSignals++;
            else
                _lowSignals++;
            instruction.ToModule.ProcessPulse(instruction.Signal);
        }
    }

    private long Gcf(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    private long Lcm(long a, long b)
    {
        return (a / Gcf(a, b)) * b;
    }

    private long LcmArray(long[] values)
    {
        var lcm = Lcm(values[0], values[1]);
        for (var i = 2; i < values.Length; i++)
            lcm = Lcm(values[i], lcm);
        return lcm;
    }

    public long RxLoop()
    {
        var count = 1L;
        var rxModule = Modules["rx"];
        var broadcaster = _modules["broadcaster"];
        var button = new DummyModule("button", this);
        
        var cycles = new Dictionary<IModule, long>();
        // rx only has 1 input
        // that input needs its sub inputs to eventually all go high
        // assume that it is a perfect cycle
        // get lcm
        var rxInput = rxModule.Inputs[0];
        var rxSubInputs = rxInput.Inputs;
        var cyclesFound = false;
        while (true)
        {
            // button press
            _pulseQueue.Enqueue((button, broadcaster, Signal.Low));
            while (_pulseQueue.TryDequeue(out var instruction))
            {
                if (Signals.IsHigh(instruction.Signal))
                    _highSignals++;
                else
                    _lowSignals++;
                instruction.ToModule.ProcessPulse(instruction.Signal);
                if (instruction.ToModule == rxInput && Signals.IsHigh(instruction.Signal))
                {
                    cycles.TryAdd(instruction.FromModule, count);
                }
                if (!rxSubInputs.All(cycles.ContainsKey))
                    continue;
                cyclesFound = true;
                break;
            }

            if (cyclesFound)
                break;
            count++;
        }

        return LcmArray(cycles.Values.ToArray());
    }
}

interface IModule
{
    public IList<IModule> Inputs { get; }
    public IList<IModule> Outputs { get; }
    public string Label { get; }

    public void AddInput(IModule input)
    {
        Inputs.Add(input);
        input.Outputs.Add(this);
    }

    public void ProcessPulse(Signal signal);
}

abstract class Module : IModule
{
    protected readonly List<IModule> _inputs = new(), _outputs = new();
    protected Machine Machine { get; set; }
    private string _label = string.Empty;
    public IList<IModule> Inputs =>_inputs;
    public IList<IModule> Outputs => _outputs;
    public string Label => _label;

    public Module(string label, Machine machine)
    {
        _label = label;
        Machine = machine;
    }

    public abstract void ProcessPulse(Signal signal);

    public override string ToString()
    {
        return $"{Label}";
    }
}

class BroadcasterModule : Module
{
    public BroadcasterModule(Machine machine) : base("broadcaster", machine)
    {
        
    }

    public override void ProcessPulse(Signal signal)
    {
        Machine.LastOutputSignal[this] = signal;
        foreach (var output in Outputs)
            Machine.PulseQueue.Enqueue((this, output, signal));
    }
}

class FlipFlopModule : Module
{
    // Low = Off, High = On
    public Signal State { get; private set; } = Signal.Low;

    public FlipFlopModule(string label, Machine machine) : base($"%{label}", machine)
    {
        
    }

    public override void ProcessPulse(Signal signal)
    {
        if (Signals.IsHigh(signal))
            return;
        State = Signals.Flip(State);
        Machine.LastOutputSignal[this] = State;
        foreach (var output in Outputs)
            Machine.PulseQueue.Enqueue((this, output, State));
    }
}

class ConjunctionModule : Module
{
    public ConjunctionModule(string label, Machine machine) : base($"&{label}", machine)
    {

    }

    public override void ProcessPulse(Signal signal)
    {
        var lastInputs = Inputs.Select(input => Machine.LastOutputSignal.GetValueOrDefault(input, Signal.Low));
        var allHigh = lastInputs.All(Signals.IsHigh);
        var pulse = allHigh ? Signal.Low : Signal.High;
        Machine.LastOutputSignal[this] = pulse;
        foreach (var output in Outputs)
            Machine.PulseQueue.Enqueue((this, output, pulse));
    }
}

class DummyModule : Module
{
    public DummyModule(string label, Machine machine) : base($"{label}", machine)
    {
        
    }

    public override void ProcessPulse(Signal signal)
    {

    }
}

class RxModule : Module
{
    public int LowPulses { get; set; }

    public RxModule(string label, Machine machine) : base($"{label}", machine)
    {
        
    }

    public override void ProcessPulse(Signal signal)
    {
        if (Signals.IsLow(signal))
            LowPulses++;
    }
}