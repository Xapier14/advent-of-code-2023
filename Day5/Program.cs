﻿using System.Collections.Concurrent;

var blockSeparator = $"{Environment.NewLine}{Environment.NewLine}";
var splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

// parses a map section block into map line definitions
ulong[][] ProcessSection(string sectionText)
    => sectionText.Split(Environment.NewLine)
        .Skip(1)
        .Select(line => line.Split(' '))
        .Select(numberStrings => numberStrings.Select(ulong.Parse).ToArray())
        .ToArray();

ulong Part1(string filePath)
{
    var sections = File.ReadAllText(filePath).Split(blockSeparator, splitOptions);
    var seeds = sections[0].Split(':', StringSplitOptions.TrimEntries)[1]
        .Split(' ')
        .Select(ulong.Parse)
        .ToArray();

    var mapper = new Mapper();
    for (var i = 1; i < 8; ++i)
    {
        mapper.Assign(ProcessSection(sections[i]));
    }

    return seeds.Select(mapper.Convert).Min();
}

ulong Part2(string filePath)
{
    var startTime = DateTime.Now;
    var sections = File.ReadAllText(filePath)
        .Split(blockSeparator, splitOptions)
        .ToArray();
    var seeds = sections[0].Split(':', StringSplitOptions.TrimEntries)[1]
        .Split(' ')
        .Select(ulong.Parse);


    var mapper = new Mapper();
    for (var j = 1; j < sections.Length; ++j)
    {
        mapper.Assign(ProcessSection(sections[j]));
    }

    var id = 1;
    var queue = new Queue<ulong>();
    var ranges = new ConcurrentDictionary<int, (ulong start, ulong length)>();
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
    var minimums = new ConcurrentDictionary<int, ulong?>();
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
        Console.WriteLine("Threads: {0} / {1}", maxThreads - availableThreads, maxThreads);
        Console.WriteLine("Running for {0:hh\\:mm\\:ss\\:fff}", DateTime.Now - startTime);
        foreach (var (threadId, progress) in progresses)
        {
            _ = minimums.TryGetValue(threadId, out var min);
            Console.SetCursorPosition(0, threadId + 1);
            Console.WriteLine("Thread {0,-3}:\t{1,6:N2}%\t{2,16}\t{3:hh\\:mm\\:ss\\:fff}", threadId, ((double)progress.Progress / (double)progress.Total) * 100.0, min?.ToString() ?? "-n/a-", (progress.EndTime ?? DateTime.Now) - progress.StartTime);
        }
        Thread.Sleep(100);
    }
    Console.Clear();
    Console.WriteLine("Time taken: {0:hh\\:mm\\:ss\\:fff}", DateTime.Now - startTime);
    foreach (var (threadId, progress) in progresses)
    {
        _ = minimums.TryGetValue(threadId, out var min);
        Console.WriteLine("Thread {0,-3}:\t{1,6:N2}%\t{2,16}\t{3:hh\\:mm\\:ss\\:fff}", threadId, ((double)progress.Progress / (double)progress.Total) * 100.0, min?.ToString() ?? "-n/a-", progress.EndTime - progress.StartTime);
    }
    return minimums.Select(kp => kp.Value).Min()!.Value;
}

ulong Crunch(ulong start, ulong length, Mapper mapper, ref ProgressInfo progress)
{
    var minLoc = ulong.MaxValue;
    for (ulong i = 0; i < length; ++i)
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

Console.WriteLine("Solution: {0}", Part2("input.txt"));
return;

public class ProgressInfo
{
    public ulong Progress { get; set; }
    public ulong Total { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public ProgressInfo(ulong total)
    {
        Progress = 0;
        Total = total;
        StartTime = DateTime.Now;
    }
}

public class Mapper
{
    private readonly List<(ulong srt, ulong end, ulong offset)[]> _map = new();

    public Mapper() { }

    public void Assign(ulong[][] mapperData)
    {
        var arr = new (ulong, ulong, ulong)[mapperData.Length];
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

    public ulong Convert(ulong value)
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