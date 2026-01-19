using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Analysis.Metrics;
using UnityEngine;

namespace Analysis.Global
{
    public class Entropy : GlobalMetric
    {
        public override float Process(GraphCache cache)
        {
            var totalDeg = cache.Matrix.Nodes.Sum(node => node.Degree);
            if (totalDeg <= 0) return 0f;
            var h = 0f;
            for (var i = 0; i < cache.Matrix.Length; i++)
            {
                var p = cache.Matrix.Nodes[i].Degree / totalDeg;
                if (p > 0) h -= p * Mathf.Log(p, 2f);
            }
            return h;
        }

        protected override float ProcessParallel(GraphCache cache, CancellationToken token)
        {
            var totalDeg = cache.Matrix.Nodes.Sum(node => node.Degree);
            if (totalDeg <= 0) return 0f;
            var sum = 0f;
            var locker = new object();
            var part = Partitioner.Create(0, cache.Matrix.Length);
            Parallel.ForEach(part, new ParallelOptions { CancellationToken = token }, range =>
            {
                var local = 0f;
                for (var i = range.Item1; i < range.Item2; i++)
                {
                    var p = cache.Matrix.Nodes[i].Degree / totalDeg;
                    if (p > 0) local -= p * Mathf.Log(p, 2f);
                }
                lock (locker) sum += local;
            });
            return sum;
        }
    }
}