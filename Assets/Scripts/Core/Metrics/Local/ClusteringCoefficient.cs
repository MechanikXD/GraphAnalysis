using System.Collections.Generic;
using Core.Graph;

namespace Core.Metrics.Local
{
    public class ClusteringCoefficient : Metric
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