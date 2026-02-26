using System;
using System.Linq;
using System.Threading;
using Analysis.Metrics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Analysis.Local
{
    // Has to be local, because of return type
    public class LaplacianSpectrum : LocalMetric
    {
        public override float[] Process(GraphCache cache)
        {
            if (cache.Matrix.Length == 0) return Array.Empty<float>();
            if (cache.Matrix.Length == 1) return new[] { 0f };
    
            // Build Laplacian matrix
            var l = BuildLaplacianMatrix(cache);
    
            // Compute eigenvalues
            var evd = l.Evd(Symmetricity.Symmetric);
            var eigenvalues = evd.EigenValues.Select(c => (float)Math.Abs(c.Real)).ToArray();
    
            // Sort eigenvalues (optional, but conventional)
            Array.Sort(eigenvalues);
    
            return eigenvalues;
        }

        protected override float[] ProcessParallel(GraphCache cache, CancellationToken token)
        {
            // Eigenvalue decomposition is already internally parallelized by MathNet.Numerics
            // So just call the sequential version
            return Process(cache);
        }

        private static Matrix<double> BuildLaplacianMatrix(GraphCache cache)
        {
            var l = DenseMatrix.Create(cache.Matrix.Length, cache.Matrix.Length, 0.0);
            for (var i = 0; i < cache.Matrix.Length; i++)
            {
                double deg = cache.Matrix.IsWeighted ? 
                    cache.Matrix.Nodes[i].WeightedDegree : cache.Matrix.Nodes[i].Degree;
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
    }
}