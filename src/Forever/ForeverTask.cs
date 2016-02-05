using System;
using System.Threading;
using System.Threading.Tasks;

namespace Forever
{
    public class ForeverTask
    {
        private Func<bool> _action;
        private int _minWaitBetweenCalls;
        private Action<Exception> _onError;

        public ForeverTask(Func<bool> action, Action<Exception> onError = null, int minWaitBetweenCalls = 1)
        {
            _action = action;
            _onError = onError;
            _minWaitBetweenCalls = minWaitBetweenCalls;
            Start();
        }

        private void Start() => Task.Factory.StartNew(_action).ConfigureAwait(false);

        private void ThreadStart(object state)
        {
            try
            {
                if (!_action()) return;
            }
            catch (Exception ex)
            {
                _onError?.Invoke(ex);
            }

            using (var mre = new ManualResetEventSlim(false))
            {
                mre.Wait(_minWaitBetweenCalls);
                Start();
            }
        }
    }
}