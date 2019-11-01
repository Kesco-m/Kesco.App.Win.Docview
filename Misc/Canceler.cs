namespace Kesco.App.Win.DocView.Misc
{
    /// <summary>
    ///   Класс - прерыватель ассинхронных операция
    /// </summary>
    public class Canceler
    {
        private readonly object _cancelLocker = new object();
        private bool _cancelRequest;

        public bool IsCancellationRequested
        {
            get
            {
                lock (_cancelLocker)
                    return _cancelRequest;
            }
        }

        public void Cancel()
        {
            lock (_cancelLocker)
                _cancelRequest = true;
        }

        public bool ThrowIfCancellationRequested()
        {
            return IsCancellationRequested;
        }
    }
}
