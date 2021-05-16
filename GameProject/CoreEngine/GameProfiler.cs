using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GameProject.CoreEngine
{
    internal sealed class GameProfiler : IDisposable
    {
        private static readonly List<(string name, long time, long tid, bool start)> events
            = new List<(string, long, long, bool)>();

        private static readonly Stopwatch startTime = Stopwatch.StartNew();

        private readonly string name;
        private static string previousEvent;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameProfiler(string name)
        {
            this.name = name;
            StartEvent(name);
        }

        public static void MakeEvent(string name)
        {
            if (previousEvent != null)
                events.Add((previousEvent, startTime.ElapsedTicks, 10, false));
            events.Add((name, startTime.ElapsedTicks, 10, true));
            previousEvent = name;
        }

        public static void Save(string fileName)
        {
            var lines = new List<string> {"["};
            foreach (var (name, time, tid, b) in events.Take(events.Count - 1))
            {
                WriteEvent(lines, name, time, tid, b ? "B" : "E");
                lines[lines.Count - 1] += ",";
            }

            foreach (var (name, time, tid, b) in events.Skip(events.Count - 1))
                WriteEvent(lines, name, time, tid, b ? "B" : "E");

            lines.Add("]");

            File.WriteAllLines(fileName, lines.ToArray());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            EndEvent(name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void StartEvent(string name)
        {
            var tid = Thread.CurrentThread.ManagedThreadId;
            events.Add((name, startTime.ElapsedTicks, tid, true));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EndEvent(string name)
        {
            var tid = Thread.CurrentThread.ManagedThreadId;
            events.Add((name, startTime.ElapsedTicks, tid, false));
        }

        private static void WriteEvent(ICollection<string> lines, string name, long time, long tid, string type)
        {
            lines.Add("\t{");

            lines.Add($"\t\t\"name\": \"{name}\",");
            lines.Add("\t\t\"cat\": \"default\",");
            lines.Add($"\t\t\"ph\": \"{type}\",");
            lines.Add("\t\t\"ts\": " +
                      $"{(time / (Stopwatch.Frequency * 0.000_001)).ToString(CultureInfo.InvariantCulture)},");
            lines.Add("\t\t\"pid\": 0,");
            lines.Add($"\t\t\"tid\": {tid}");

            lines.Add("\t}");
        }
    }
}