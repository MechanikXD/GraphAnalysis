using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Analysis.Metrics;

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
                    // Get neighbors from adjacency matrix (direct connections only)
                    var neighbors = cache.OutNeighbors[i];
                    if (neighbors.Length < 2)
                    {
                        result[i] = 0f;
                        continue;
                    }
            
                    // Compute efficiency among neighbors
                    var sum = 0f;
                    var count = 0;
            
                    for (var j = 0; j < neighbors.Length; j++)
                    {
                        for (var k = j + 1; k < neighbors.Length; k++)
                        {
                            var u = neighbors[j];
                            var v = neighbors[k];
                            var d = cache.AspsDistances![u, v];
                    
                            if (!float.IsInfinity(d) && d > 0)
                            {
                                sum += 1f / d;
                                count++;
                            }
                        }
                    }
            
                    result[i] = count > 0 ? sum / count : 0f;
                }
            });
    
            return result;
        }
    }
}