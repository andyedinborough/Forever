using System;
using System.Threading;

namespace Forever
{
    public class ForeverThreadPool
    {
        private Func<bool> _action;
        private WaitCallback _callback;
        private int _minWaitBetweenCalls;
        private ManualResetEventSlim _mre;
        private Action<Exception> _onError;

        public ForeverThreadPool(Func<bool> action, Action<Exception> onError = null, int minWaitBetweenCalls = 1)
        {
            _action = action;
            _onError = onError;
            _minWaitBetweenCalls = minWaitBetweenCalls;
            _mre = new ManualResetEventSlim(false);
            _callback = new WaitCallback(ThreadStart);
            Start();
        }

        private void Start() => ThreadPool.QueueUserWorkItem(_callback);

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
            _mre.Wait(_minWaitBetweenCalls);
            Start();
        }
    }
}