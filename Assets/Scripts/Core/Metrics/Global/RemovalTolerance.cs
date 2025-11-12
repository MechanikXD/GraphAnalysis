using System.Collections.Generic;
using Core.Graph;

namespace Core.Metrics.Global
{
    public class RemovalTolerance : Metric
    {
        public int Removals { get; set; }
        
        public override float Process(Node node, AdjacencyMatrix snapshot)
        {
            var removed = new bool[snapshot.Length];
            removed[node.NodeIndex] = true;

            var visited = new bool[snapshot.Length];
            var largest = 0;
            for (var i = 0; i < snapshot.Length; i++)
            {
                if (removed[i] || visited[i]) continue;
                int size = BfsComponentSize(i, snapshot, removed, visited);
                if (size > largest) largest = size;
            }
            return (float)largest / (snapshot.Length - 1);
        }

        public override float ProcessAsync(Node node, AdjacencyMatrix snapshot)
        {
            throw new System.NotImplementedException();
        }

        public override float ProcessSeparatedAsync(Node node, AdjacencyMatrix snapshot)
        {
            throw new System.NotImplementedException();
        }
        
        private static int BfsComponentSize(int start, AdjacencyMatrix matrix, bool[] removed, bool[] visited)
        {
            var q = new Queue<int>();
            q.Enqueue(start);
            visited[start] = true;
            var count = 0;

            while (q.Count > 0)
            {
                var u = q.Dequeue();
                count++;
                for (var v = 0; v < matrix.Length; v++)
                {
                    if (removed[v] || visited[v] || matrix[u, v] == 0) continue;
                    visited[v] = true;
                    q.Enqueue(v);
                }
            }
            return count;
        }
    }
}