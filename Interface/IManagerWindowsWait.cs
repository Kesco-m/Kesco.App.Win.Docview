using System.Threading;
using Kesco.App.Win.DocView.Misc;

namespace Kesco.App.Win.DocView.Interface
{
    /// <summary>
    ///   Включение- выключение окон
    /// </summary>
    public interface IManagerWindowsWait
    {
        ManualResetEvent AsyncWaitHandleClose { get; }
        ManualResetEvent AsyncWaitHandleCreate { get; }
        void WindowsWait(SynchronizationContext context, Canceler canceler);
    }
}
