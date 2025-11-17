using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Graph;
using Core.Metrics.Metrics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Core.Metrics.Local
{
    public class LaplacianSpectrum : LocalMetric
    {
        public override float[] Process(GraphCache cache)
        {
            // compute Laplacian and eigenvalues once
            var l = BuildLaplacianMatrix(cache);
            var fullEvd = l.Evd(Symmetricity.Symmetric);
            var fullEigen = fullEvd.EigenValues.Select(c => c.Real).ToArray();
            var fullEnergy = fullEigen.Sum(x => x * x);

            var result = new float[cache.Matrix.Length];
            for (var v = 0; v < cache.Matrix.Length; v++)
            {
                var lm = BuildLaplacianWithoutNode(cache.Matrix, v);
                var evd = lm.Evd(Symmetricity.Symmetric);
                var eigen = evd.EigenValues.Select(c => c.Real).ToArray();
                var energy = eigen.Sum(x => x * x);
                result[v] = (float)(fullEnergy - energy);
            }

            return result;
        }

        protected override float[] ProcessParallel(GraphCache cache, CancellationToken token)
        {
            // compute Laplacian and eigenvalues once
            var l = BuildLaplacianMatrix(cache);
            var fullEvd = l.Evd(Symmetricity.Symmetric);
            var fullEigen = fullEvd.EigenValues.Select(c => c.Real).ToArray();
            var fullEnergy = fullEigen.Sum(x => x * x);

            var result = new float[cache.Matrix.Length];
            Parallel.For(0, cache.Matrix.Length, new ParallelOptions { CancellationToken = token },
                v =>
                {
                    var lm = BuildLaplacianWithoutNode(cache.Matrix, v);
                    var evd = lm.Evd(Symmetricity.Symmetric);
                    var eigen = evd.EigenValues.Select(c => c.Real).ToArray();
                    var energy = eigen.Sum(x => x * x);
                    result[v] = (float)(fullEnergy - energy);
                });

            return result;
        }
        
        // build Laplacian DenseMatrix (double)
        public Matrix<double> BuildLaplacianMatrix(GraphCache cache)
        {
            var l = DenseMatrix.Create(cache.Matrix.Length, cache.Matrix.Length, 0.0);
            for (var i = 0; i < cache.Matrix.Length; i++)
            {
                double deg = cache.Matrix.Nodes[i].Degree;
                l[i, i] = deg;
                var neighbors = cache.OutNeighbors[i];
                var wts = cache.OutWeights[i];
                for (var k = 0; k < neighbors.Length; k++)
                {
                    var j = neighbors[k];
                    if (i != j) l[i, j] = -wts[k];
                }
            }

            return l;
        }

        // helper: produces dense (n-1)x(n-1) laplacian with node removed
        private Matrix<double> BuildLaplacianWithoutNode(AdjacencyMatrix matrix, int removed)
        {
            var l = DenseMatrix.Create(matrix.Length - 1, matrix.Length - 1, 0.0);
            var ri = 0;
            for (var i = 0; i < matrix.Length; i++)
            {
                if (i == removed) continue;
                var rj = 0;
                double deg = matrix.Nodes[i].Degree;
                l[ri, ri] = deg;
                for (int j = 0; j < matrix.Length; j++)
                {
                    if (j == removed) continue;
                    if (i != j)
                    {
                        l[ri, rj] = -matrix[i, j];
                    }

                    rj++;
                }

                ri++;
            }

            return l;
        }
    }
}