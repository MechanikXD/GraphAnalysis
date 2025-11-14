using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Graph;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

namespace Core.Metrics.Local
{
    public class EigenvectorCentrality : Metric<float>
    {
        public int Iterations { get; set; } = 100;
        public float Tolerance { get; set; } = 1e-6f;
        
        public override float Process(Node node, AdjacencyMatrix snapshot)
        {
            var centrality = Vector<float>.Build.Dense(snapshot.Length, 1f);

            for (var iter = 0; iter < Iterations; iter++)
            {
                var newC = Matrix<float>.Build.SparseOfArray(snapshot.AsArray()) * centrality;
                var norm = (float)newC.L2Norm();
                if (norm > 0) newC /= norm;

                if ((newC - centrality).L2Norm() < Tolerance)
                    break;
                centrality = newC;
            }

            return centrality[node.NodeIndex];
        }

        public override float ProcessParallel(Node node, AdjacencyMatrix matrix, CancellationToken token)
        {
            var x = new float[matrix.Length];
            for (var i = 0; i < matrix.Length; i++) x[i] = 1f;
            var xNew = new float[matrix.Length];

            for (var iter = 0; iter < Iterations; iter++)
            {
                Array.Clear(xNew, 0, matrix.Length);

                Parallel.For(0, matrix.Length, new ParallelOptions { CancellationToken = token }, () => 0f,
                    (i, _, _) =>
                    {
                        var s = 0f;
                        for (var j = 0; j < matrix.Length; j++)
                            s += matrix[i, j] * x[j];
                        xNew[i] = s;
                        return 0f;
                    }, _ => { });

                var norm = 0f;
                for (var i = 0; i < matrix.Length; i++) norm += xNew[i] * xNew[i];
                norm = Mathf.Sqrt(norm);
                if (norm == 0f) break;
                for (var i = 0; i < matrix.Length; i++) xNew[i] /= norm;

                // diff
                var diff = 0f;
                for (var i = 0; i < matrix.Length; i++) diff += Mathf.Abs(xNew[i] - x[i]);
                Array.Copy(xNew, x, matrix.Length);
                if (diff < Tolerance) break;
            }

            return x[node.NodeIndex];
        }
    }
}