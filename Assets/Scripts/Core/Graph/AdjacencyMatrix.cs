using System.Collections.Generic;
using System.Threading;
using Core.Metrics;
using Cysharp.Threading.Tasks;

namespace Core.Graph
{
    public class AdjacencyMatrix
    {
        private static CancellationTokenSource _cts;
        public List<Node> Nodes { get; } = new List<Node>();
        private readonly List<List<float>> _matrix = new List<List<float>>();
        public Dictionary<string, float> GlobalStats { get; private set; }
        public int Length { get; private set; }
        public bool IsOriented { get; private set; }
        public bool IsWeighted { get; set; }

        public void MakeOriented() => IsOriented = true;
        
        public void AddNode(Node node)
        {
            node.NodeIndex = Nodes.Count;
            Nodes.Add(node);
            Length++;
            
            foreach (var col in _matrix) col.Add(0);
            var emptyList = new List<float>( new float[Length] );
            _matrix.Add(emptyList);
        }

        public void RemoveNode(int at)
        {
            Nodes.RemoveAt(at);
            _matrix.RemoveAt(at);
            for (var i = 0; i < _matrix.Count; i++)
            {
                _matrix[i].RemoveAt(at);
                Nodes[i].NodeIndex = i;
            }
            Length--;
        }

        public float this[int row, int column]
        {
            get => _matrix[row][column];
            set
            {
                _matrix[row][column] = value;
                if (!IsOriented) _matrix[column][row] = value;
            }
        }

        private async UniTask ProcessStats()
        {
            _cts.Cancel();
            var snapshot = TakeSnapshot();
            await UniTask.SwitchToThreadPool();
            
            // TODO: Switch between types of computing based on difficulty
            var stats = MetricProvider.ProcessMetrics(snapshot);
            GlobalStats = stats.global;

            await UniTask.SwitchToMainThread();
            foreach (var node in Nodes)
            {
                node.LoadStats(stats.local);
            }
        }

        private AdjacencyMatrix TakeSnapshot()
        {
            var snapshot = new AdjacencyMatrix();
            foreach (var node in Nodes)
            {
                snapshot.AddNode(node);
            }
            for (var i = 0; i < _matrix.Count; i++)
            {
                for (var j = 0; j < _matrix[i].Count; j++)
                {
                    snapshot[i, j] = IsWeighted ? _matrix[i][j] : 1f;
                }
            }
            
            return snapshot;
        }
    }
}