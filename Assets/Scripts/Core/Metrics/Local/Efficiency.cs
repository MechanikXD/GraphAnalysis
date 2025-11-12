using Core.Graph;

namespace Core.Metrics.Local
{
    public class Efficiency : Metric
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

        public override float ProcessAsync(Node node, AdjacencyMatrix snapshot)
        {
            throw new System.NotImplementedException();
        }

        public override float ProcessSeparatedAsync(Node node, AdjacencyMatrix snapshot)
        {
            throw new System.NotImplementedException();
        }
    }
}