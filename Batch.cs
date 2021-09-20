using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchParallel
{
    public static class Batch
    {
        public static Task[] Parallel<T>(this IEnumerable<T> collection, Action<IEnumerable<T>> action, bool singleThread = false)
        {
            int processors = singleThread ? 1 : Environment.ProcessorCount;
            int n = collection.Count();
            int nPerProc = n / processors;
            Task[] tasks = new Task[processors + 1];

            processors.ForEach(p => tasks[p] = Task.Run(() => action(collection.Skip(p * nPerProc).Take(nPerProc))));

            int remainder = n - nPerProc * processors;
            var lastTask = Task.Run(() => action(collection.Skip(nPerProc * processors).Take(remainder)));
            tasks[processors] = lastTask;

            return tasks;
        }
    }

    public static class ExtensionMethods
    {
        // Process a subset of the collection on separate threads.
        public static Task[] BatchParallel<T>(this IEnumerable<T> collection, Action<IEnumerable<T>> action, bool singleThread = false)
        {
            int processors = singleThread ? 1 : Environment.ProcessorCount;
            int n = collection.Count();
            int nPerProc = n / processors;
            Task[] tasks = new Task[processors + 1];

            processors.ForEach(p => tasks[p] = Task.Run(() => action(collection.Skip(p * nPerProc).Take(nPerProc))));

            int remainder = n - nPerProc * processors;
            var lastTask = Task.Run(() => action(collection.Skip(nPerProc * processors).Take(remainder)));
            tasks[processors] = lastTask;

            return tasks;
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static void ForEach(this int n, Action<int> action)
        {
            for (int i = 0; i < n; i++)
            {
                action(i);
            }
        }
    }
}
