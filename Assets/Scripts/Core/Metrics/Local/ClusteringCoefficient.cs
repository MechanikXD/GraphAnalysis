using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Graph;

namespace Core.Metrics.Local
{
    public class ClusteringCoefficient : Metric<float>
    {
        public override float Process(Node node, AdjacencyMatrix snapshot)
        {
            var neighbors = new List<int>();

            for (var j = 0; j < snapshot.Length; j++)
            {
                if (snapshot[node.NodeIndex, j] > 0 || snapshot[j, node.NodeIndex] > 0)
                    neighbors.Add(j);
            }

            var k = neighbors.Count;
            if (k < 2) return 0f;

            var links = 0;
            for (var a = 0; a < k; a++)
            {
                for (var b = a + 1; b < k; b++)
                {
                    if (snapshot[neighbors[a], neighbors[b]] > 0 || snapshot[neighbors[b], neighbors[a]] > 0)
                        links++;
                }
            }

            return (2f * links) / (k * (k - 1));
        }

        public override float ProcessParallel(Node node, AdjacencyMatrix matrix, CancellationToken token)
        {
            var neighbors = new List<int>();
            for (var j = 0; j < matrix.Length; j++)
                if (matrix[node.NodeIndex, j] > 0 || matrix[j, node.NodeIndex] > 0)
                    neighbors.Add(j);

            var k = neighbors.Count;
            if (k < 2) return 0f;

            long links = 0;

            Parallel.For(0, k, new ParallelOptions { CancellationToken = token }, () => 0L,
                (a, _, local) =>
                {
                    var localLinks = local;
                    for (var b = a + 1; b < k; b++)
                    {
                        int na = neighbors[a], nb = neighbors[b];
                        if (matrix[na, nb] > 0 || matrix[nb, na] > 0) localLinks++;
                    }
                    return localLinks;
                },
                local => Interlocked.Add(ref links, local)
            );

            return 2f * links / (k * (k - 1));
        }
    }
}