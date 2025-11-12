using System.Collections.Generic;
using Core.Graph;

namespace Core.Metrics.Local
{
    public class BetweennessCentrality : Metric
    {
        public override float Process(Node node, AdjacencyMatrix snapshot)
        {
            var score = 0f;

            for (var s = 0; s < snapshot.Length; s++)
            {
                var stack = new Stack<int>();
                var predecessors = new List<int>[snapshot.Length];
                var sigma = new float[snapshot.Length];
                var distance = new int[snapshot.Length];
                var queue = new Queue<int>();

                for (int i = 0; i < snapshot.Length; i++)
                {
                    predecessors[i] = new List<int>();
                    sigma[i] = 0;
                    distance[i] = -1;
                }

                sigma[s] = 1;
                distance[s] = 0;
                queue.Enqueue(s);

                while (queue.Count > 0)
                {
                    var v = queue.Dequeue();
                    stack.Push(v);

                    for (var w = 0; w < snapshot.Length; w++)
                    {
                        if (snapshot[v, w] == 0) continue;

                        if (distance[w] < 0)
                        {
                            queue.Enqueue(w);
                            distance[w] = distance[v] + 1;
                        }

                        if (distance[w] != distance[v] + 1) continue;

                        sigma[w] += sigma[v];
                        predecessors[w].Add(v);
                    }
                }

                var delta = new float[snapshot.Length];
                while (stack.Count > 0)
                {
                    var w = stack.Pop();
                    foreach (var v in predecessors[w])
                        delta[v] += (sigma[v] / sigma[w]) * (1 + delta[w]);

                    if (w != s && w == node.NodeIndex)
                        score += delta[w];
                }
            }
            return score;
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