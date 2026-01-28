using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Analysis.Metrics;
using UnityEngine;

namespace Analysis.Local
{
    public class ClusteringCoefficient : LocalMetric
    {
        public override float[] Process(GraphCache cache) => ProcessParallel(cache, CancellationToken.None);

        protected override float[] ProcessParallel(GraphCache cache, CancellationToken token)
        {
            var result = new float[cache.Matrix.Length];
            var part = Partitioner.Create(0, cache.Matrix.Length);
            Parallel.ForEach(part, new ParallelOptions { CancellationToken = token }, () => 0f,
                (range, _, _) =>
                {
                    for (var i = range.Item1; i < range.Item2; i++)
                    {
                        var neighbors = cache.OutNeighbors[i];
                        var k = neighbors.Length;
                        if (k < 2)
                        {
                            result[i] = 0f;
                            continue;
                        }

                        long links = 0;
                        for (var a = 0; a < k; a++)
                        {
                            var na = neighbors[a];

                            // to check membership quickly, convert neighbor list to HashSet? To keep memory low we do linear scan.
                            for (var b = a + 1; b < k; b++)
                            {
                                var nb = neighbors[b];

                                // check if na connects to nb or nb connects to na
                                var connected = false;

                                // choose smaller list to scan
                                if (cache.OutNeighbors[na].Length < cache.OutNeighbors[nb].Length)
                                {
                                    for (int t = 0; t < cache.OutNeighbors[na].Length; t++)
                                    {
                                        if (cache.OutNeighbors[na][t] == nb)
                                        {
                                            connected = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    for (var t = 0; t < cache.OutNeighbors[nb].Length; t++)
                                    {
                                        if (cache.OutNeighbors[nb][t] == na)
                                        {
                                            connected = true;
                                            break;
                                        }
                                    }
                                }

                                if (connected) links++;
                            }
                        }

                        result[i] = (2f * links) / (k * Mathf.Max(1, k - 1));
                    }

                    return 0f;
                },
                _ => { });
            return result;
        }
    }
}