using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Metrics.Metrics
{
    public abstract class Metric<T>
    {
        public abstract T Process(GraphCache cache);
        
        public async UniTask<T> ProcessAsync(GraphCache cache, CancellationToken token)
        {
            return await UniTask.RunOnThreadPool(() => Process(cache), cancellationToken: token);
        }
        
        protected virtual T ProcessParallel(GraphCache cache, CancellationToken token) => Process(cache);

        public UniTask<T> ProcessParallelAsync(GraphCache cache, CancellationToken token)
        {
            return UniTask.RunOnThreadPool(() => ProcessParallel(cache, token), cancellationToken: token);
        }
    }
}