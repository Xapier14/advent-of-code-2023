using Xapier14.AdventOfCode;
using System.Text;
using System.Collections.Concurrent;
AdventOfCode.SetYearAndDay(2023, 5);

var input = AdventOfCode.GetInputText();

Console.WriteLine("Part 1: {0}", Part1(input));
Console.WriteLine("Part 2: {0}", Part2(input));
return;

// parses a map section block into map line definitions
uint[][] ProcessSection(string sectionText)
    => sectionText.Split('\n')
        .Skip(1)
        .Select(line => line.Split(' '))
        .Select(numberStrings => numberStrings.Select(uint.Parse).ToArray())
        .ToArray();

uint Part1(string input)
{
    var blockSeparator = "\n\n";
    var splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    var sections = input.Split(blockSeparator, splitOptions);
    var seeds = sections[0].Split(':', StringSplitOptions.TrimEntries)[1]
        .Split(' ')
        .Select(uint.Parse)
        .ToArray();

    var mapper = new Mapper();
    for (var i = 1; i < 8; ++i)
    {
        mapper.Assign(ProcessSection(sections[i]));
    }

    return seeds.Select(mapper.Convert).Min();
}

uint Part2(string input)
{
    var blockSeparator = "\n\n";
    var splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    var startTime = DateTime.Now;
    var sections = input.Split(blockSeparator, splitOptions)
        .ToArray();
    var seeds = sections[0].Split(':', StringSplitOptions.TrimEntries)[1]
        .Split(' ')
        .Select(uint.Parse);


    var mapper = new Mapper();
    for (var j = 1; j < sections.Length; ++j)
    {
        mapper.Assign(ProcessSection(sections[j]));
    }

    var id = 1;
    var queue = new Queue<uint>();
    var ranges = new ConcurrentDictionary<int, (uint start, uint length)>();
    foreach (var seed in seeds)
        queue.Enqueue(seed);
    while (queue.Any())
    {
        var start = queue.Dequeue();
        var length = queue.Dequeue();

        var length1 = length / 2;
        var start1 = start;

        var length2 = length - length1;
        var start2 = start1 + length1;

        ranges.TryAdd(id, (start1, length1));
        ranges.TryAdd(id + 1, (start2, length2));

        id += 2;
    }

    // start threads
    var minimums = new ConcurrentDictionary<int, uint?>();
    var progresses = new ConcurrentDictionary<int, ProgressInfo>();
    foreach (var (threadId, (start, length)) in ranges)
    {
        ThreadPool.QueueUserWorkItem((_) =>
        {
            var progress = new ProgressInfo(length);
            progresses.TryAdd(threadId, progress);
            minimums.TryAdd(threadId, Crunch(start, length, mapper, ref progress));
            progress.EndTime = DateTime.Now;
        });
    }

    Console.Clear();
    var blanking = new string(' ', 55);
    while (minimums.Count != ranges.Count)
    {
        ThreadPool.GetMaxThreads(out var maxThreads, out _);
        ThreadPool.GetAvailableThreads(out var availableThreads, out _);
        Console.SetCursorPosition(0, 0);
        Console.WriteLine("Active Threads: {0} / {1}", maxThreads - availableThreads, maxThreads);
        Console.WriteLine("Running for {0:hh\\:mm\\:ss\\:fff}", DateTime.Now - startTime);
        foreach (var (threadId, progress) in progresses)
        {
            _ = minimums.TryGetValue(threadId, out var min);
            Console.SetCursorPosition(0, threadId + 1);
            Console.WriteLine("Thread {0,2:D2} | {1,6:N2}% {2} {3,10} / {4,10} | {5:hh\\:mm\\:ss\\:fff} | Result: {6,10} |",
                threadId,
                ((double)progress.Progress / (double)progress.Total) * 100.0,
                ProgressBar(32, ((double)progress.Progress / (double)progress.Total)),
                progress.Progress,
                progress.Total,
                (progress.EndTime ?? DateTime.Now) - progress.StartTime,
                min?.ToString() ?? "-n/a-"
            );
        }
        Thread.Sleep(100);
    }
    Console.Clear();
    Console.WriteLine("Time taken: {0:hh\\:mm\\:ss\\:fff}", DateTime.Now - startTime);
    foreach (var (threadId, progress) in progresses)
    {
        _ = minimums.TryGetValue(threadId, out var min);
        Console.WriteLine("Thread {0,2:D2} | {1,6:N2}% {2} {3,10} / {4,10} | {5:hh\\:mm\\:ss\\:fff} | Result: {6,10} |",
            threadId,
            ((double)progress.Progress / (double)progress.Total) * 100.0,
            ProgressBar(32, ((double)progress.Progress / (double)progress.Total)),
            progress.Progress,
            progress.Total,
            (progress.EndTime ?? DateTime.Now) - progress.StartTime,
            min?.ToString() ?? "-n/a-"
        );
    }
    return minimums.Select(kp => kp.Value).Min()!.Value;
}

string ProgressBar(int length, double value)
{
    var builder = new StringBuilder();
    builder.Append('[');
    for (var i = 0; i < length - 2; ++i)
        builder.Append(
            i < Math.Floor(value * length)
            ? '='
            : '-'
        );
    builder.Append(']');

    return builder.ToString();
}

uint Crunch(uint start, uint length, Mapper mapper, ref ProgressInfo progress)
{
    var minLoc = uint.MaxValue;
    for (uint i = 0; i < length; ++i)
    {
        progress.Progress = i;
        var seed = start + i;
        seed = mapper.Convert(seed);
        if (seed < minLoc)
            minLoc = seed;
    }
    progress.Progress = length;
    return minLoc;
}

public class ProgressInfo
{
    public uint Progress { get; set; }
    public uint Total { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public ProgressInfo(uint total)
    {
        Progress = 0;
        Total = total;
        StartTime = DateTime.Now;
    }
}

public class Mapper
{
    private readonly List<(uint srt, uint end, uint offset)[]> _map = new();

    public Mapper() { }

    public void Assign(uint[][] mapperData)
    {
        var arr = new (uint, uint, uint)[mapperData.Length];
        for (var i = 0; i < arr.Length; ++i)
        {
            var srcStart = mapperData[i][1];
            var dstStart = mapperData[i][0];
            var range = mapperData[i][2] - 1;
            var offset = dstStart - srcStart;
            arr[i] = (srcStart, srcStart + range, offset);
        }
        _map.Add(arr);
    }

    public void Clear()
        => _map.Clear();

    public uint Convert(uint value)
    {
        var seed = value;
        foreach (var block in _map)
        {
            for (var i = 0; i < block.Length; ++i)
            {
                var (start, end, offset) = block[i];
                if (seed >= start && seed <= end)
                {
                    seed += offset;
                    i = block.Length;
                }
            }
        }
        return seed;
    }
}
