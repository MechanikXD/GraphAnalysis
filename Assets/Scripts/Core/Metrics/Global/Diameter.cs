using System.Threading;
using Core.Graph;
using UnityEngine;

namespace Core.Metrics.Global
{
    public class Diameter : Metric<float>
    {
        public override float Process(Node node, AdjacencyMatrix snapshot)
        {
            var n = snapshot.Length;
            float maxDist = 0;
            
            for (var i = 0; i < n; i++)
            {
                var distances = snapshot.IsWeighted
                    ? snapshot.Dijkstra(i)
                    : snapshot.Bfs(i);
                foreach (var d in distances)
                    if (d < Mathf.Infinity)
                        maxDist = Mathf.Max(maxDist, d);
            }
            return maxDist;
        }
    }
}