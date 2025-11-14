using System.Collections.Generic;
using System.Linq;
using Core.Graph;
using UnityEngine;

namespace Core.Metrics.Global
{
    public class Assortativity : Metric<float>
    {
        public override float Process(Node node, AdjacencyMatrix snapshot)
        {
            var n = snapshot.Length;
            var degrees = new float[n];
            for (var i = 0; i < n; i++)
            {
                float sum = 0;
                for (var j = 0; j < n; j++) sum += snapshot[i, j];
                degrees[i] = sum;
            }

            var pairs = new List<(float, float)>();
            for (var i = 0; i < n; i++)
                for (var j = 0; j < n; j++)
                    if (snapshot[i, j] > 0)
                        pairs.Add((degrees[i], degrees[j]));

            var meanK = pairs.Average(p => p.Item1);
            var meanL = pairs.Average(p => p.Item2);
            var num = pairs.Sum(p => (p.Item1 - meanK) * (p.Item2 - meanL));
            var den = Mathf.Sqrt(pairs.Sum(p => Mathf.Pow(p.Item1 - meanK, 2)) *
                                 pairs.Sum(p => Mathf.Pow(p.Item2 - meanL, 2)));

            return den == 0 ? 0 : num / den;
        }
    }
}