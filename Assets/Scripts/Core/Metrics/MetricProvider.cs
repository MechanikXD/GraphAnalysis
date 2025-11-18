using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Core.Graph;
using Core.Metrics.Global;
using Core.Metrics.Local;
using Core.Metrics.Metrics;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Metrics
{
    public static class MetricProvider
    {
        private readonly static CancellationTokenSource Cts = new CancellationTokenSource();
        
        private readonly static Dictionary<string, GlobalMetric> GlobalMetrics = new Dictionary<string, GlobalMetric>
        {
            ["Assortativity"] = new Assortativity(),
            ["Density"] = new Density(),
            ["Diameter"] = new Diameter(),
            ["Entropy"] = new Entropy()
        };
        
        private readonly static Dictionary<string, LocalMetric> LocalMetrics = new Dictionary<string, LocalMetric>
        {
            ["Betweenness Centrality"] = new BetweennessCentrality(),
            ["Clustering Coefficient"] = new ClusteringCoefficient(),
            ["Efficiency"] = new Efficiency(),
            ["Eigenvector Centrality"] = new EigenvectorCentrality(),
            ["Laplacian Spectrum"] = new LaplacianSpectrum(),
            ["Removal Tolerance"] = new RemovalTolerance()
        };

        public static (Dictionary<string, float> global, Dictionary<string, float[]> local) ProcessMetrics(AdjacencyMatrix matrix)
        {
            var cache = new GraphCache(matrix);

            var global =
                GlobalMetrics.ToDictionary(met => met.Key, met => met.Value.Process(cache));
            var local =
                LocalMetrics.ToDictionary(met => met.Key, met => met.Value.Process(cache));

            return (global, local);
        }

        public async static UniTask<(Dictionary<string, float> global, Dictionary<string, float[]> local)> ProcessMatrixAsync(AdjacencyMatrix matrix)
        {
            Cts.Cancel();
            
            await UniTask.SwitchToTaskPool();
            var cache = new GraphCache(matrix);

            var globalTasks = Enumerable.Select(GlobalMetrics, m => RunGlobalAsync(m, cache)).ToList();
            var localTasks  = Enumerable.Select(LocalMetrics, m => RunLocalAsync(m, cache)).ToList();

            var gResults = await UniTask.WhenAll(globalTasks);
            var lResults = await UniTask.WhenAll(localTasks);

            return (
                gResults.ToDictionary(p => p.Key, p => p.Value),
                lResults.ToDictionary(p => p.Key, p => p.Value)
            );
        }
        
        private async static UniTask<KeyValuePair<string, float>> RunGlobalAsync(KeyValuePair<string, GlobalMetric> kvp, GraphCache cache) =>
            new KeyValuePair<string, float>(kvp.Key, await kvp.Value.ProcessAsync(cache, Cts.Token));

        private async static UniTask<KeyValuePair<string, float[]>> RunLocalAsync(KeyValuePair<string, LocalMetric> kvp, GraphCache cache) =>
            new KeyValuePair<string, float[]>(kvp.Key, await kvp.Value.ProcessAsync(cache, Cts.Token));

        public async static UniTask<(Dictionary<string, float> global, Dictionary<string, float[]> local)> ProcessMatrixParallel(AdjacencyMatrix matrix)
        {
            Cts.Cancel();
            
            await UniTask.SwitchToTaskPool();
            var cache = new GraphCache(matrix);

            var globalTasks = Enumerable.Select(GlobalMetrics, m => ComputeParallelGlobalAsync(m, cache)).ToList();
            var localTasks  = Enumerable.Select(LocalMetrics, m => ComputeParallelLocalAsync(m, cache)).ToList();

            var gResults = await UniTask.WhenAll(globalTasks);
            var lResults = await UniTask.WhenAll(localTasks);

            return (
                gResults.ToDictionary(p => p.Key, p => p.Value),
                lResults.ToDictionary(p => p.Key, p => p.Value)
            );
        }

        private async static UniTask<KeyValuePair<string, float>> ComputeParallelGlobalAsync(KeyValuePair<string, GlobalMetric> kvp, GraphCache cache)
            => new KeyValuePair<string, float>(kvp.Key, await kvp.Value.ProcessParallelAsync(cache, Cts.Token));

        private async static UniTask<KeyValuePair<string, float[]>> ComputeParallelLocalAsync(KeyValuePair<string, LocalMetric> kvp, GraphCache cache)
            => new KeyValuePair<string, float[]>(kvp.Key, await kvp.Value.ProcessParallelAsync(cache, Cts.Token));
    }
}