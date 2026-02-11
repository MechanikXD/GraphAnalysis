using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Analysis.Metrics;

namespace Analysis.Local
{
    public class RemovalTolerance : LocalMetric
    {
        public override float[] Process(GraphCache cache)
        {
            var result = new float[cache.Matrix.Length];
            var baselineSize = GetLargestComponentSize(cache, excludedNode: -1);

            for (var i = 0; i < cache.Matrix.Length; i++)
                result[i] = GetLargestComponentSize(cache, excludedNode: i) - baselineSize;

            return result;
        }

        protected override float[] ProcessParallel(GraphCache cache, CancellationToken token)
        {
            var result = new float[cache.Matrix.Length];
            var baselineSize = GetLargestComponentSize(cache, excludedNode: -1);
            var rangePartitioner = Partitioner.Create(0, cache.Matrix.Length);

            Parallel.ForEach(rangePartitioner, new ParallelOptions { CancellationToken = token },
                range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        result[i] = GetLargestComponentSize(cache, excludedNode: i) - baselineSize;
                    }
                });

            return result;
        }

        private static int GetLargestComponentSize(GraphCache cache, int excludedNode)
        {
            var n = cache.Matrix.Length;
            var visited = new bool[n];

            // Mark excluded node as visited
            if (excludedNode >= 0)
                visited[excludedNode] = true;

            var largestComponentSize = 0;

            for (var start = 0; start < n; start++)
            {
                if (visited[start])
                    continue;

                // BFS to find component size
                var componentSize = 0;
                var queue = new Queue<int>();
                queue.Enqueue(start);
                visited[start] = true;

                while (queue.Count > 0)
                {
                    var u = queue.Dequeue();
                    componentSize++;

                    var neighbors = cache.OutNeighbors[u];
                    foreach (var v in neighbors)
                    {
                        if (!visited[v])
                        {
                            visited[v] = true;
                            queue.Enqueue(v);
                        }
                    }
                }

                if (componentSize > largestComponentSize)
                    largestComponentSize = componentSize;
            }

            return largestComponentSize;
        }
    }
}