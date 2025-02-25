using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Assets.Scripts.Utils
{
    public class UnityMainThreadDispatcher : SingletonMonoBehaviour<UnityMainThreadDispatcher>
    {
        private readonly Queue<Action> queue = new Queue<Action>();

        public void Enqueue(Action action)
        {
            lock (queue)
            {
                queue.Enqueue(action);
            }
        }

        public Task EnqueueAsync(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            Enqueue(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(true);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });

            return tcs.Task;
        }

        public Task<T> EnqueueAsync<T>(Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();

            Enqueue(() =>
            {
                try
                {
                    var result = func();
                    tcs.SetResult(result);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });

            return tcs.Task;
        }

        private void Update()
        {
            lock (queue)
            {
                while (queue.Any())
                {
                    queue.Dequeue().Invoke();
                }
            }
        }
    }
}
