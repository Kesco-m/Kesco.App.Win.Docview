using System;
using System.Threading;
using Kesco.App.Win.DocView.Interface;
using Kesco.App.Win.DocView.Misc;

namespace Kesco.App.Win.DocView.Forms
{
	/// <summary>
	///   класс выполняющий ожидание и включающий-выключающий формы
	/// </summary>
	public class ManagerWindow : IManagerWindowsWait
	{
		public const Int32 COUNTER_MAX = 4; //сек

		private delegate SynchronizationContext WaiterDelegate(SynchronizationContext context, Canceler cancel);

		#region IManagerWindowsWait Members

		private ManualResetEvent asyncWaitHandle = new ManualResetEvent(false);
		private ManualResetEvent asyncWaitHandleCreate = new ManualResetEvent(false);

		public ManualResetEvent AsyncWaitHandleClose
		{
			get { return asyncWaitHandle; }
		}

		public ManualResetEvent AsyncWaitHandleCreate
		{
			get { return asyncWaitHandleCreate; }
		}

		public void WindowsWait(SynchronizationContext context, Canceler cancel)
		{
			AsyncWaitHandleCreate.Reset();
			AsyncWaitHandleClose.Reset();
			//выражение, включающее - выключающее формы
			Action<Boolean> switchingWindows = (Boolean enable) =>
			{
				foreach(System.Windows.Forms.Form window in System.Windows.Forms.Application.OpenForms)
					if(window.Visible)
					{
						if(enable)
							window.Cursor = System.Windows.Forms.Cursors.Default;
						else
							window.Cursor = System.Windows.Forms.Cursors.WaitCursor;
						window.Enabled = enable;
					}
			};
			context.Send((object state) => switchingWindows(false), null);

			//ассинхронная операция, выполняющая ожидание
			WaiterDelegate waiterAction =
				(SynchronizationContext graphicsContext, Canceler canceler) =>
				{
					AsyncWaitHandleCreate.Set();
					Int32 counter = 0;
					try
					{
						do
						{
							if(canceler.ThrowIfCancellationRequested()) break;
							Thread.Sleep(50); //20HZ процесс
						} while((COUNTER_MAX * 20) >= ++counter);
					}
					catch(OperationCanceledException ex)
					{
						Lib.Win.Data.Env.WriteToLog(ex);
					}

					AsyncWaitHandleClose.Set();
					return graphicsContext;
				};

			waiterAction.BeginInvoke(context, cancel, (IAsyncResult ar) =>
			{
				var waiterActionEnd =
					ar.AsyncState as WaiterDelegate;
				if(waiterActionEnd != null)
				{
					waiterActionEnd.EndInvoke(ar);
				}
				if(ar.CompletedSynchronously)
					switchingWindows(true);
				else
					context.Send(
						(object state) => switchingWindows(true),
						null);
			}, waiterAction);
			//ожидание запуска waiterAction
			AsyncWaitHandleCreate.WaitOne(5000);
		}

		#endregion
	}
}