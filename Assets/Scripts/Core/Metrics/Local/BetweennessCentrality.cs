using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Metrics.Metrics;

namespace Core.Metrics.Local
{
    public class BetweennessCentrality : LocalMetric
    {
        public override float[] Process(GraphCache cache) =>
            ProcessParallel(cache, CancellationToken.None);

        protected override float[] ProcessParallel(GraphCache cache, CancellationToken token)
        {
            var cb = new float[cache.Matrix.Length];
            var part = Partitioner.Create(0, cache.Matrix.Length);
            var locker = new object();

            Parallel.ForEach(part, new ParallelOptions { CancellationToken = token },
                () => new float[cache.Matrix.Length],
                (range, _, local) =>
                {
                    // local Brandes accumulators for sources in this range
                    for (var s = range.Item1; s < range.Item2; s++)
                    {
                        // single-source Brandes (unweighted). For weighted graphs you should use Dijkstra.
                        var stack = new Stack<int>();
                        var predecessors = new List<int>[cache.Matrix.Length];
                        for (var i = 0; i < cache.Matrix.Length; i++) predecessors[i] = new List<int>();
                        var sigma = new float[cache.Matrix.Length];
                        var dist = new int[cache.Matrix.Length];
                        for (var i = 0; i < cache.Matrix.Length; i++)
                        {
                            dist[i] = -1;
                            sigma[i] = 0;
                        }

                        var q = new Queue<int>();
                        sigma[s] = 1;
                        dist[s] = 0;
                        q.Enqueue(s);

                        while (q.Count > 0)
                        {
                            var v = q.Dequeue();
                            stack.Push(v);
                            var neighbors = cache.OutNeighbors[v];
                            foreach (var w in neighbors)
                            {
                                if (dist[w] < 0)
                                {
                                    dist[w] = dist[v] + 1;
                                    q.Enqueue(w);
                                }

                                if (dist[w] == dist[v] + 1)
                                {
                                    sigma[w] += sigma[v];
                                    predecessors[w].Add(v);
                                }
                            }
                        }

                        var delta = new float[cache.Matrix.Length];
                        while (stack.Count > 0)
                        {
                            var w = stack.Pop();
                            foreach (var v in predecessors[w])
                                delta[v] += (sigma[v] / sigma[w]) * (1 + delta[w]);
                            if (w != s) local[w] += delta[w];
                        }
                    }

                    return local;
                },
                local =>
                {
                    lock (locker)
                    {
                        for (var i = 0; i < cache.Matrix.Length; i++) cb[i] += local[i];
                    }
                });

            // normalization for undirected graphs:
            var denominate = !cache.Matrix.IsOriented
                ? (cache.Matrix.Length - 1) * (cache.Matrix.Length - 2) / 2f
                : (cache.Matrix.Length - 1) * (cache.Matrix.Length - 2);
            var result = new float[cache.Matrix.Length];
            for (var i = 0; i < cache.Matrix.Length; i++) result[i] = denominate == 0 ? cb[i] : cb[i] / denominate;
            return result;
        }
    }
}