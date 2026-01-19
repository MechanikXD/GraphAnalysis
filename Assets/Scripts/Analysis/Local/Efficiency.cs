using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Analysis.Metrics;
using UnityEngine;

namespace Analysis.Local
{
    public class Efficiency : LocalMetric
    {
        public override float[] Process(GraphCache cache) => ProcessParallel(cache, CancellationToken.None);

        protected override float[] ProcessParallel(GraphCache cache, CancellationToken token)
        {
            var result = new float[cache.Matrix.Length];
            var part = Partitioner.Create(0, cache.Matrix.Length);
            Parallel.ForEach(part, new ParallelOptions { CancellationToken = token }, range =>
            {
                for (var i = range.Item1; i < range.Item2; i++)
                {
                    var sum = 0f;
                    for (var j = 0; j < cache.Matrix.Length; j++)
                    {
                        if (i == j) continue;
                        var d = cache.AspsDistances![i, j];
                        if (!float.IsInfinity(d)) sum += 1f / d;
                    }
                    result[i] = sum / Mathf.Max(1, cache.Matrix.Length - 1);
                }
            });
            return result;
        }
    }
}