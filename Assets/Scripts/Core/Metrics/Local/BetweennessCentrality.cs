using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Graph;

namespace Core.Metrics.Local
{
    public class BetweennessCentrality : Metric<float>
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

        public override float ProcessParallel(Node node, AdjacencyMatrix matrix, CancellationToken token)
        {
            var n = matrix.Length;
            var target = node.NodeIndex;

            var totalScore = 0.0;
            var locker = new object();

            var po = new ParallelOptions { CancellationToken = token };

            Parallel.For(0, n, po, () => 0.0, (s, _, localSum) =>
            {
                var stack = new Stack<int>();
                var predecessors = new List<int>[n];
                for (var i = 0; i < n; i++) predecessors[i] = new List<int>();
                var sigma = new float[n];
                var dist = new int[n];
                for (var i = 0; i < n; i++) { dist[i] = -1; sigma[i] = 0f; }

                var q = new Queue<int>();
                sigma[s] = 1f;
                dist[s] = 0;
                q.Enqueue(s);

                while (q.Count > 0)
                {
                    var v = q.Dequeue();
                    stack.Push(v);
                    for (var w = 0; w < n; w++)
                    {
                        if (matrix[v, w] == 0) continue;
                        if (dist[w] < 0)
                        {
                            dist[w] = dist[v] + 1;
                            q.Enqueue(w);
                        }
                        if (dist[w] == dist[v] + 1)
                        {
                            sigma[w] += sigma[v];
                            predecessors[w].Add(v);
                        }
                    }
                }

                var delta = new double[n];
                while (stack.Count > 0)
                {
                    var w = stack.Pop();
                    foreach (var v in predecessors[w])
                        delta[v] += (sigma[v] / sigma[w]) * (1.0 + delta[w]);

                    // If the popped node is the target and not the source, accumulate delta[w]
                    if (w == target && w != s)
                    {
                        localSum += delta[w];
                    }
                }

                return localSum;
            },
            localResult =>
            {
                lock (locker) totalScore += localResult;
            });

            var denominate = (!matrix.IsOriented) ? (double)(n - 1) * (n - 2) / 2.0 : (double)(n - 1) * (n - 2);
            if (denominate == 0) return (float)totalScore;
            return (float)(totalScore / denominate);
        }
    }
}