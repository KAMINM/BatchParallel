//https://www.codeproject.com/Articles/5289661/Batch-Parallel

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BatchParallel
{
    class Program
    {
        static ConcurrentDictionary<int, int> threadIdCounts;

        static void Main(string[] args)
        {
            Console.WriteLine("Parallel.ForeEach example:");
            threadIdCounts = new ConcurrentDictionary<int, int>();
            var plr = Parallel.ForEach(Enumerable.Range(0, 1000), DoSomething);
            threadIdCounts.ForEach(kvp => Console.WriteLine($"TID: {kvp.Key}, Count = {kvp.Value}"));

            Console.WriteLine("\r\nBatchParallel example:");
            threadIdCounts = new ConcurrentDictionary<int, int>();
            var tasks = Enumerable.Range(0, 1000).BatchParallel(batch => DoSomething(batch));
            Task.WaitAll(tasks);
            threadIdCounts.ForEach(kvp => Console.WriteLine($"TID: {kvp.Key}, Count = {kvp.Value}"));

            Console.WriteLine("\r\nBatchParallel with remainder example:");
            threadIdCounts = new ConcurrentDictionary<int, int>();
            tasks = Enumerable.Range(0, 1003).BatchParallel(batch => DoSomething(batch));
            Task.WaitAll(tasks);
            threadIdCounts.ForEach(kvp => Console.WriteLine($"TID: {kvp.Key}, Count = {kvp.Value}"));

            Console.WriteLine("\r\nBatch.Parallel with remainder example:");
            threadIdCounts = new ConcurrentDictionary<int, int>();
            tasks = Batch.Parallel(Enumerable.Range(0, 1003), batch => DoSomething(batch));
            Task.WaitAll(tasks);
            threadIdCounts.ForEach(kvp => Console.WriteLine($"TID: {kvp.Key}, Count = {kvp.Value}"));
        }

        static void DoSomething(int n)
        {
            DoWork();
        }

        static void DoSomething<T>(IEnumerable<T> batch)
        {
            // Do setup stuff

            // The process the batch.
            batch.ForEach(n => DoWork());
        }

        static void DoWork()
        {
            int tid = Thread.CurrentThread.ManagedThreadId;

            if (!threadIdCounts.TryGetValue(tid, out int count))
            {
                threadIdCounts[tid] = 0;
            }

            threadIdCounts[tid] = count + 1;
            Thread.Sleep(1);
        }
    }
}
