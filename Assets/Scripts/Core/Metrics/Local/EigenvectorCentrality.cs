using Core.Graph;
using MathNet.Numerics.LinearAlgebra;

namespace Core.Metrics.Local
{
    public class EigenvectorCentrality : Metric
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