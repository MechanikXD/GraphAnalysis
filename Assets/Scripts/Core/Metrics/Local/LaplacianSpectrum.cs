using Core.Graph;

namespace Core.Metrics.Local
{
    public class LaplacianSpectrum : Metric
    {
        public override float Process(Node node, AdjacencyMatrix snapshot)
        {
            var degree = node.Degree(snapshot);
            var sumRow = 0f;

            for (var j = 0; j < snapshot.Length; j++)
                sumRow += snapshot[node.NodeIndex, j];

            return degree - sumRow; // local contribution
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