using System.Linq;
using Core.Graph;
using UnityEngine;

namespace Core.Metrics.Global
{
    public class Entropy : Metric
    {
        public override float Process(Node node, AdjacencyMatrix snapshot)
        {
            var degrees = new float[snapshot.Length];
            for (var i = 0; i < snapshot.Length; i++)
                degrees[i] = snapshot.Nodes[i].Degree(snapshot);

            var total = degrees.Sum();
            if (total == 0) return 0f;

            var p = degrees[node.NodeIndex] / total;
            return -p * Mathf.Log(p + 1e-9f);
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