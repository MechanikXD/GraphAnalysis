using System.Linq;
using System.Threading;
using Core.Graph;
using UnityEngine;

namespace Core.Metrics.Global
{
    public class Entropy : Metric<float>
    {
        public override float Process(Node node, AdjacencyMatrix snapshot)
        {
            var degrees = new float[snapshot.Length];
            for (var i = 0; i < snapshot.Length; i++)
                degrees[i] = snapshot.Nodes[i].Degree;

            var total = degrees.Sum();
            if (total == 0) return 0f;

            var p = degrees[node.NodeIndex] / total;
            return -p * Mathf.Log(p + 1e-9f);
        }
    }
}