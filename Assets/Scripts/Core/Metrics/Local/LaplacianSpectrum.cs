using System.Threading;
using Core.Graph;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Core.Metrics.Local
{
    public class LaplacianSpectrum : Metric<float>
    {
        public override float Process(Node node, AdjacencyMatrix snapshot)
        {
            return ProcessParallel(node, snapshot, CancellationToken.None);
        }

        public override float ProcessParallel(Node node, AdjacencyMatrix matrix, CancellationToken token)
        {
            var l = BuildLaplacian(matrix);
            var fullEnergy = LaplacianEnergy(l);

            var lMinus = BuildLaplacianWithNodeRemoved(matrix, node.NodeIndex);
            double energyMinus = LaplacianEnergy(lMinus);

            var lc = fullEnergy - energyMinus;
            return (float)lc;   
        }
        
        public static Matrix<double> BuildLaplacian(AdjacencyMatrix matrix)
        {
            var l = DenseMatrix.Create(matrix.Length, matrix.Length, 0f);
            for (var i = 0; i < matrix.Length; i++)
            {
                var deg = 0f;
                for (var j = 0; j < matrix.Length; j++)
                    deg += matrix[i, j];
                l[i, i] = deg;
                for (var j = 0; j < matrix.Length; j++)
                    if (i != j)
                        l[i, j] = -matrix[i, j];
            }
            
            return l;
        }

        public static Matrix<double> BuildLaplacianWithNodeRemoved(AdjacencyMatrix matrix, int removedIndex)
        {
            var l = DenseMatrix.Create(matrix.Length - 1, matrix.Length - 1, 0f);
            var ri = 0;
            for (var i = 0; i < matrix.Length; i++)
            {
                if (i == removedIndex) continue;
                var rj = 0;
                var deg = 0.0;
                for (var k = 0; k < matrix.Length; k++)
                    deg += matrix[i, k];
                l[ri, ri] = deg;
                for (var j = 0; j < matrix.Length; j++)
                {
                    if (j == removedIndex) continue;
                    if (i != j) l[ri, rj] = -matrix[i, j];
                    rj++;
                }
                ri++;
            }
            
            return l;
        }

        public static float LaplacianEnergy(Matrix<double> lap)
        {
            var evd = lap.Evd(Symmetricity.Symmetric);
            var eigs = evd.EigenValues;
            var sum = 0f;
            foreach (var eig in eigs)
            {
                var real = (float)eig.Real;
                sum += real * real;
            }
            return sum;
        }
    }
}