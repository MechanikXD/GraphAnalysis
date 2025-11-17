using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Core.Metrics.Metrics;

namespace Core.Metrics.Local
{
    public class RemovalTolerance : LocalMetric
    {
        public override float[] Process(GraphCache cache)
        {
            var result = new float[cache.Matrix.Length];

            var baselineEff = ComputeGlobalEfficiency(cache.AspsDistances, cache.Matrix.Length);

            for (var i = 0; i < cache.Matrix.Length; i++)
                result[i] = baselineEff - ComputeEfficiencyWithoutNode(cache, i);

            return result;
        }

        protected override float[] ProcessParallel(GraphCache cache, CancellationToken token)
        {
            float[] result = new float[cache.Matrix.Length];

            float baselineEff = ComputeGlobalEfficiency(cache.AspsDistances, cache.Matrix.Length);

            var rangePartitioner = Partitioner.Create(0, cache.Matrix.Length);

            Parallel.ForEach(rangePartitioner, range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    result[i] = baselineEff - ComputeEfficiencyWithoutNode(cache, i);
                }
            });

            return result;
        }
        
        private static float ComputeGlobalEfficiency(float[,] dist, int n)
        {
            var sum = 0f;
            var pairs = n * (n - 1);

            for (var u = 0; u < n; u++)
            {
                for (var v = 0; v < n; v++)
                {
                    if (u == v) continue;
                    var d = dist[u, v];
                    if (d > 0 && !float.IsPositiveInfinity(d))
                        sum += 1f / d;
                }
            }

            return sum / pairs;
        }

        private static float ComputeEfficiencyWithoutNode(GraphCache cache, int removedNode)
        {
            var sum = 0f;
            var validPairs = 0;

            var dist = cache.AspsDistances;

            for (var u = 0; u < cache.Matrix.Length; u++)
            {
                if (u == removedNode) continue;

                for (var v = 0; v < cache.Matrix.Length; v++)
                {
                    if (v == removedNode || u == v) continue;
                    var d = dist![u, v];
                    
                    if (d > 0 && !float.IsPositiveInfinity(d))
                    {
                        sum += 1f / d;
                        validPairs++;
                    }
                }
            }

            if (validPairs == 0)
                return 0f;

            return sum / validPairs;
        }
    }
}