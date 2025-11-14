using System.Threading;
using Core.Graph;

namespace Core.Metrics.Global
{
    public class Density : Metric<float>
    {
        public override float Process(Node node, AdjacencyMatrix snapshot)
        {
            var n = snapshot.Length;
            var edgeCount = 0;
            
            for (var i = 0; i < n; i++)
                for (var j = 0; j < n; j++)
                    if (snapshot[i, j] > 0) edgeCount++;

            if (!snapshot.IsOriented)
                edgeCount /= 2;

            var maxEdges = snapshot.IsOriented ? n * (n - 1) : n * (n - 1) / 2f;
            return maxEdges == 0f ? 0f : edgeCount / maxEdges;
        }
    }
}