using System.Collections.Generic;
using System.Threading;
using Core.Metrics.Global;
using Core.Metrics.Local;
using Core.Metrics.Metrics;

namespace Core.Metrics
{
    public class MetricProvider
    {
        private static CancellationTokenSource _cts;
        
        private Dictionary<string, GlobalMetric> _globalMetrics = new Dictionary<string, GlobalMetric>
        {
            ["Assortativity"] = new Assortativity(),
            ["Density"] = new Density(),
            ["Diameter"] = new Diameter(),
            ["Entropy"] = new Entropy()
        };
        private Dictionary<string, LocalMetric> _localMetrics = new Dictionary<string, LocalMetric>
        {
            ["Betweenness Centrality"] = new BetweennessCentrality(),
            ["Clustering Coefficient"] = new ClusteringCoefficient(),
            ["Efficiency"] = new Efficiency(),
            ["Eigenvector Centrality"] = new EigenvectorCentrality(),
            ["Laplacian Spectrum"] = new LaplacianSpectrum(),
            ["Removal Tolerance"] = new RemovalTolerance()
        };
    }
}