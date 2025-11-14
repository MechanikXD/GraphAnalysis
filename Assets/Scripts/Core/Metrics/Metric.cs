using System.Threading;
using Core.Graph;
using Cysharp.Threading.Tasks;

namespace Core.Metrics
{
    public abstract class Metric<T>
    {
        public abstract T Process(Node node, AdjacencyMatrix snapshot);
        
        public async UniTask<T> ProcessAsync(Node node, AdjacencyMatrix snapshot, CancellationToken token)
        {
            return await UniTask.RunOnThreadPool(() => Process(node, snapshot), cancellationToken: token);
        }
        
        public virtual T ProcessParallel(Node node, AdjacencyMatrix matrix, CancellationToken token)
        {
            return Process(node, matrix);
        }

        public UniTask<T> ProcessParallelAsync(Node node, AdjacencyMatrix matrix, CancellationToken token)
        {
            return UniTask.RunOnThreadPool(() => ProcessParallel(node, matrix, token), cancellationToken: token);
        }
    }
}