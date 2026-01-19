using System.Threading;
using System.Threading.Tasks;
using Core.Metrics.Metrics;

namespace Core.Metrics.Global
{
    public class Density : GlobalMetric
    {
        public override float Process(GraphCache cache)
        {
            var len = cache.Matrix.Length;
            long edges = 0;
            for (var i = 0; i < len; i++)
                for (var j = 0; j < len; j++)
                    if (cache.Matrix[i, j] > 0) edges++;
            float e = edges;
            if (!cache.Matrix.IsOriented) e /= 2f;
            var possible = cache.Matrix.IsOriented ? len * (len - 1) : len * (len - 1) / 2f;
            return possible == 0 ? 0f : e / possible;
        }

        protected override float ProcessParallel(GraphCache cache, CancellationToken token)
        {
            // parallel edge counting by chunks
            var len = cache.Matrix.Length;
            long total = 0;
            var part = System.Collections.Concurrent.Partitioner.Create(0, len);
            Parallel.ForEach(part, new ParallelOptions { CancellationToken = token }, range =>
            {
                long local = 0;
                for (var i = range.Item1; i < range.Item2; i++)
                    for (var j = 0; j < len; j++)
                        if (cache.Matrix[i, j] > 0) local++;
                Interlocked.Add(ref total, local);
            });
            float edges = total;
            if (!cache.Matrix.IsOriented) edges /= 2f;
            var possible = cache.Matrix.IsOriented ? len * (len - 1) : len * (len - 1) / 2f;
            return possible == 0f ? 0f : edges / possible;
        }
    }
}