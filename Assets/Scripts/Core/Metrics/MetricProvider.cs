using System.Collections.Generic;
using Core.Graph;

namespace Core.Metrics
{
    public class MetricProvider
    {
        private List<Metric> _globalMetrics = new List<Metric>{};
        private List<Metric> _localMetrics = new List<Metric>{};

        public Dictionary<string, float> GetGlobalMetrics() { return null; }
        public Dictionary<string, float> GetLocalMetrics(Node node, AdjacencyMatrix snapshot){ return null; }
    }
}