using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Analysis.Metrics;

namespace Analysis.Global
{
    public class Diameter : GlobalMetric
    {
        public override float Process(GraphCache cache)
        {
            var dist = cache.AspsDistances;
            var maxDistance = 0f;
            for (var i = 0; i < cache.Matrix.Length; i++)
                for (var j = 0; j < cache.Matrix.Length; j++)
                    if (!float.IsInfinity(dist![i, j]) && dist[i, j] > maxDistance) maxDistance = dist[i, j];
            return maxDistance;
        }

        protected override float ProcessParallel(GraphCache cache, CancellationToken token)
        {
            var dist = cache.AspsDistances;
            var globalMax = 0f;
            var part = Partitioner.Create(0, cache.Matrix.Length);
            var locker = new object();
            Parallel.ForEach(part, new ParallelOptions { CancellationToken = token }, range =>
            {
                var localMax = 0f;
                for (var i = range.Item1; i < range.Item2; i++)
                    for (var j = 0; j < cache.Matrix.Length; j++)
                        if (!float.IsInfinity(dist![i, j]) && dist[i, j] > localMax) localMax = dist[i, j];
                lock (locker) if (localMax > globalMax) globalMax = localMax;
            });
            return globalMax;
        }
    }
}