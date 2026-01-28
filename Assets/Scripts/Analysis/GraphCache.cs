#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Graph;

namespace Analysis
{
    public class GraphCache
    {
        public AdjacencyMatrix Matrix { get; }

        public int[][] OutNeighbors { get; }
        public float[][] OutWeights { get; }

        public float[,]? AspsDistances { get; private set; }
        private volatile double[]? _laplacianEigenvalues;

        public GraphCache(AdjacencyMatrix matrix)
        {
            Matrix = matrix;

            OutNeighbors = new int[Matrix.Length][];
            OutWeights = new float[Matrix.Length][];

            BuildAdjacencyLists();
            GetAllPairsShortestPaths();
        }

        private void BuildAdjacencyLists()
        {
            var outLists = new List<int>[Matrix.Length];
            var outWLists = new List<float>[Matrix.Length];
            var inLists = new List<int>[Matrix.Length];
            var inWLists = new List<float>[Matrix.Length];
            for (var i = 0; i < Matrix.Length; i++)
            {
                outLists[i] = new List<int>();
                outWLists[i] = new List<float>();
                inLists[i] = new List<int>();
                inWLists[i] = new List<float>();
            }

            for (var i = 0; i < Matrix.Length; i++)
            {
                for (var j = 0; j < Matrix.Length; j++)
                {
                    var w = Matrix[i, j];
                    if (w > 0f)
                    {
                        outLists[i].Add(j);
                        outWLists[i].Add(w);
                        inLists[j].Add(i);
                        inWLists[j].Add(w);
                    }
                }
            }

            for (var i = 0; i < Matrix.Length; i++)
            {
                OutNeighbors[i] = outLists[i].ToArray();
                OutWeights[i] = outWLists[i].ToArray();
            }
        }

        public void GetAllPairsShortestPaths(bool useWeighted = false)
        {
            var dist = new float[Matrix.Length, Matrix.Length];

            // initialize
            for (var i = 0; i < Matrix.Length; i++)
                for (var j = 0; j < Matrix.Length; j++)
                    dist[i, j] = float.PositiveInfinity;

            // run single-source from each node in parallel (BFS or Dijkstra)
            Parallel.For(0, Matrix.Length,
                new ParallelOptions { CancellationToken = CancellationToken.None }, i =>
                {
                    var d = useWeighted || Matrix.IsWeighted
                        ? DijkstraSingleSource(i)
                        : BFS_Distances(i);
                    for (var j = 0; j < Matrix.Length; j++) dist[i, j] = d[j];
                });

            AspsDistances = dist;
        }

        private float[] BFS_Distances(int start)
        {
            var dist = Enumerable.Repeat(float.PositiveInfinity, Matrix.Length).ToArray();
            var q = new Queue<int>();
            dist[start] = 0f;
            q.Enqueue(start);
            while (q.Count > 0)
            {
                var u = q.Dequeue();
                var neighbor = OutNeighbors[u];
                foreach (var v in neighbor)
                {
                    if (float.IsPositiveInfinity(dist[v]))
                    {
                        dist[v] = dist[u] + 1f;
                        q.Enqueue(v);
                    }
                }
            }

            return dist;
        }

        // Dijkstra standard (dense adjacency lists ok)
        private float[] DijkstraSingleSource(int start)
        {
            var dist = Enumerable.Repeat(float.PositiveInfinity, Matrix.Length).ToArray();
            var visited = new bool[Matrix.Length];
            dist[start] = 0f;

            for (var iter = 0; iter < Matrix.Length; iter++)
            {
                var u = -1;
                var best = float.PositiveInfinity;
                for (var i = 0; i < Matrix.Length; i++)
                {
                    if (!visited[i] && dist[i] < best)
                    {
                        best = dist[i];
                        u = i;
                    }
                }

                if (u == -1) break;
                visited[u] = true;
                var neighbors = OutNeighbors[u];
                var wts = OutWeights[u];
                for (var k = 0; k < neighbors.Length; k++)
                {
                    var v = neighbors[k];
                    var w = wts[k];
                    var alt = dist[u] + w;
                    if (alt < dist[v]) dist[v] = alt;
                }
            }

            return dist;
        }
    }
}