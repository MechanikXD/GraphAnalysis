using System.Threading;
using System.Threading.Tasks;
using Core.Graph;
using UnityEngine;

namespace Core.Metrics.Local
{
    public class Efficiency : Metric<float>
    {
        public override float Process(Node node, AdjacencyMatrix snapshot)
        {
            var n = snapshot.Length;
            var distances = snapshot.IsWeighted
                ? snapshot.Dijkstra(node.NodeIndex)
                : snapshot.Bfs(node.NodeIndex);

            var eff = 0f;
            for (var i = 0; i < n; i++)
            {
                if (i == node.NodeIndex || float.IsInfinity(distances[i])) continue;
                eff += 1f / distances[i];
            }
            return eff / (n - 1);
        }

        public override float ProcessParallel(Node node, AdjacencyMatrix matrix, CancellationToken token)
        {
            var dist = matrix.IsWeighted ? matrix.Dijkstra(node.NodeIndex) 
                                                : matrix.Bfs(node.NodeIndex);
            var sum = 0.0;

            var locker = new object();
            Parallel.For(0, matrix.Length, new ParallelOptions { CancellationToken = token }, () => 0.0,
                (i, _, local) =>
                {
                    if (i == node.NodeIndex) return local;
                    if (!float.IsInfinity(dist[i])) local += 1.0 / dist[i];
                    return local;
                },
                local =>
                {
                    lock (locker) sum += local;
                });

            return (float)(sum / Mathf.Max(1, matrix.Length - 1));
        }
    }
}