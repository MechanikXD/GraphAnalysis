using Core.Graph;

namespace Core.Metrics
{
    public abstract class Metric
    {
        public abstract float Process(Node node, AdjacencyMatrix snapshot);
        public abstract float ProcessAsync(Node node, AdjacencyMatrix snapshot);
        public abstract float ProcessSeparatedAsync(Node node, AdjacencyMatrix snapshot);
    }
}