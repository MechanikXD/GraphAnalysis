using System.Collections.Generic;
using System.Threading;
using Core.Metrics;
using Cysharp.Threading.Tasks;
using UI;
using UI.View;

namespace Core.Graph
{
    public class AdjacencyMatrix
    {
        private readonly static CancellationTokenSource Cts = new CancellationTokenSource();
        public List<Node> Nodes { get; } = new List<Node>();
        private readonly List<List<float>> _matrix = new List<List<float>>();
        private Dictionary<string, float> _globalStats;
        public int Length { get; private set; }
        public bool IsOriented { get; private set; }
        public bool IsWeighted { get; set; }

        public void MakeOriented() => IsOriented = true;
        
        public void AddNode(Node node, bool updateStats=true)
        {
            node.NodeIndex = Nodes.Count;
            Nodes.Add(node);
            Length++;
            
            foreach (var col in _matrix) col.Add(0);
            var emptyList = new List<float>( new float[Length] );
            _matrix.Add(emptyList);
            if (updateStats) ProcessStats().Forget();
        }

        public void RemoveNode(int at, bool updateStats=true)
        {
            Nodes.RemoveAt(at);
            _matrix.RemoveAt(at);
            for (var i = 0; i < _matrix.Count; i++)
            {
                _matrix[i].RemoveAt(at);
                Nodes[i].NodeIndex = i;
            }
            Length--;
            if (updateStats) ProcessStats().Forget();
        }

        public void SetValue(float value, int row, int column, bool updateStats)
        {
            _matrix[row][column] = value;
            if (!IsOriented) _matrix[column][row] = value;
            if (updateStats) ProcessStats().Forget();
        }

        public float this[int row, int column] => _matrix[row][column];

        private async UniTask ProcessStats()
        {
            if (Length <= 0)
            {
                UIManager.Instance.GetHUDCanvas<GlobalStatDisplayView>().Hide();
                _globalStats?.Clear();
                return;
            }
            
            Cts.Cancel();
            var clone = Clone();
            
            // TODO: Switch between types of computing based on difficulty
            var stats = MetricProvider.ProcessMetrics(clone);
            _globalStats = stats.global;

            foreach (var node in Nodes)
            {
                node.LoadStats(stats.local);
            }
            
            var hud = UIManager.Instance.GetHUDCanvas<GlobalStatDisplayView>();
            hud.LoadText(_globalStats);
            if (!hud.IsEnabled) UIManager.Instance.ShowHUD<GlobalStatDisplayView>();
        }

        private AdjacencyMatrix Clone()
        {
            var clone = new AdjacencyMatrix();
            foreach (var node in Nodes)
            {
                clone.AddNode(node, false);
            }
            for (var i = 0; i < _matrix.Count; i++)
            {
                for (var j = 0; j < _matrix[i].Count; j++)
                {
                    var value = 0f;
                    if (!IsWeighted) value = _matrix[i][j];
                    else if (!IsWeighted && _matrix[i][j] != 0f) value = 1f;
                    clone.SetValue(value, i, j, false);
                }
            }
            
            return clone;
        }
    }
}