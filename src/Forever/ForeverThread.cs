using System;
using System.Threading;

namespace Forever
{
    public class ForeverThread
    {
        private Func<bool> _action;
        private int _minWaitBetweenCalls;
        private Action<Exception> _onError;
        private Thread _thread;

        public ForeverThread(Func<bool> action, Action<Exception> onError = null, int minWaitBetweenCalls = 1)
        {
            _action = action;
            _onError = onError;
            _minWaitBetweenCalls = minWaitBetweenCalls;
            _thread = new Thread(ThreadStart);
            _thread.Priority = ThreadPriority.BelowNormal;
            _thread.Start();
        }

        private void ThreadStart(object state)
        {
            using (var mre = new ManualResetEventSlim(false))
                while (true)
                {
                    try
                    {
                        if (!_action()) break;
                    }
                    catch (Exception ex)
                    {
                        _onError?.Invoke(ex);
                    }
                    mre.Wait(_minWaitBetweenCalls);
                }
        }
    }
}