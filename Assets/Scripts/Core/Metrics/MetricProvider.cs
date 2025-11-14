using System.Collections.Generic;
using Core.Graph;

namespace Core.Metrics
{
    public class MetricProvider
    {
        private List<Metric<float>> _globalMetrics = new List<Metric<float>>{};
        private List<Metric<float>> _localMetrics = new List<Metric<float>>{};

        public Dictionary<string, float> GetGlobalMetrics() { return null; }
        public Dictionary<string, float> GetLocalMetrics(Node node, AdjacencyMatrix snapshot){ return null; }
    }
}