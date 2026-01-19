using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Metrics.Metrics;

namespace Core.Metrics.Global
{
    public class Assortativity : GlobalMetric
    {
        public override float Process(GraphCache cache)
        {
            var deg = cache.Matrix.Nodes.Select(x => (double)x.Degree).ToArray();
            double m1 = 0, m2 = 0, m3 = 0; // for Pearson formula
            double edgeCount = 0;
            for (var i = 0; i < cache.Matrix.Length; i++)
            {
                var neighbors = cache.OutNeighbors[i];
                foreach (var j in neighbors)
                {
                    var ki = deg[i];
                    var kj = deg[j];
                    m1 += ki * kj;
                    m2 += (ki + kj) / 2.0;
                    m3 += (ki * ki + kj * kj) / 2.0;
                    edgeCount += 1.0;
                }
            }
            if (edgeCount == 0) return 0f;
            var num = m1 / edgeCount - (m2 / edgeCount) * (m2 / edgeCount);
            var den = m3 / edgeCount - (m2 / edgeCount) * (m2 / edgeCount);
            return den == 0 ? 0f : (float)(num / den);
        }

        protected override float ProcessParallel(GraphCache cache, CancellationToken token)
        {
            // accumulate using partitions
            double m1 = 0, m2 = 0, m3 = 0;
            var locker = new object();
            var part = Partitioner.Create(0, cache.Matrix.Length);
            Parallel.ForEach(part, new ParallelOptions { CancellationToken = token }, () => (m1: 0.0, m2: 0.0, m3: 0.0),
                (range, _, local) =>
                {
                    double lm1 = 0, lm2 = 0, lm3 = 0;
                    for (var i = range.Item1; i < range.Item2; i++)
                    {
                        var neighbors = cache.OutNeighbors[i];
                        foreach (var j in neighbors)
                        {
                            double ki = cache.Matrix.Nodes[i].Degree;
                            double kj = cache.Matrix.Nodes[j].Degree;
                            lm1 += ki * kj;
                            lm2 += (ki + kj) / 2.0;
                            lm3 += (ki * ki + kj * kj) / 2.0;
                        }
                    }
                    local.m1 += lm1; local.m2 += lm2; local.m3 += lm3;
                    return local;
                },
                local =>
                {
                    lock (locker) { m1 += local.m1; m2 += local.m2; m3 += local.m3; }
                });

            double edgeCount = 0;
            for (var i = 0; i < cache.Matrix.Length; i++) edgeCount += cache.OutNeighbors[i].Length;
            if (edgeCount == 0) return 0f;
            var num = m1 / edgeCount - (m2 / edgeCount) * (m2 / edgeCount);
            var den = m3 / edgeCount - (m2 / edgeCount) * (m2 / edgeCount);
            return den == 0 ? 0f : (float)(num / den);
        }
    }
}