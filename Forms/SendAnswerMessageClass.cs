using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Kesco.Lib.Win.Error;
using Timer = System.Timers.Timer;

namespace Kesco.App.Win.DocView.Forms
{
	/// <summary>
	///   Статический класс посылки сообшений в окна
	/// </summary>
	public class SendAnswerMessageClass
	{
		private static SynchronizedCollection<KeyValuePair<IntPtr, object[]>> wndHandleHashtable;
		private static SynchronizedCollection<Common.SendAnswerParams> answerCollection;
		private static Timer timer;
		public static Thread thread;

		public static void Add(IntPtr wndHandle, IntPtr formHandle)
		{
			if(wndHandleHashtable == null)
				wndHandleHashtable = new SynchronizedCollection<KeyValuePair<IntPtr, object[]>>();

			if(ContainsKey(formHandle))
				return;
			try
			{
				wndHandleHashtable.Add(new KeyValuePair<IntPtr, object[]>(formHandle, new object[] { wndHandle, (timer != null) }));
				Start();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public static void Add(string answerControl, string answerKey, string answerUrl, string command, System.Windows.Forms.Form form = null)
		{
			if(answerCollection == null)
				answerCollection = new SynchronizedCollection<Common.SendAnswerParams>();
			if(string.IsNullOrEmpty(answerControl) || string.IsNullOrEmpty(answerKey) || string.IsNullOrEmpty(answerUrl))
				return;
			if(Contains(answerControl, answerKey, answerUrl, command, form))
				return;
			try
			{
				answerCollection.Add(new Common.SendAnswerParams(answerControl, answerKey, answerUrl, command, form));
				Start();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private static bool Contains(string answerControl, string answerKey, string answerUrl, string command, Form form)
		{
			if(answerCollection == null || answerCollection.Count == 0)
				return false;
			return answerCollection.Any(x => x.AnswerControl.Equals(answerControl) && x.AnswerKey.Equals(answerKey) && x.AnswerUrl.Equals(answerUrl) && x.AnswerCommand.Equals(command) && (form == null || form.Equals(x.Form)));
		}

		public static bool Contains(Form form)
		{
			if(answerCollection == null || answerCollection.Count == 0 || form == null)
				return false;
			return answerCollection.Any(x => form.Equals(x.Form));
		}

		public static void Start()
		{
			try
			{
				if(timer == null)
				{
					timer = new Timer(1000);
					timer.Elapsed += timer_Elapsed;
					timer.Start();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public static void Remove(IntPtr formHandle)
		{
			if(!ContainsKey(formHandle))
				return;

			try
			{
				wndHandleHashtable.Remove(wndHandleHashtable.First(x => x.Key.Equals(formHandle)));
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex, "Error while removing " + formHandle.ToString() + " from wndHandleHashTable");
			}

			if(timer != null && wndHandleHashtable.Count == 0)
			{
				timer.Elapsed -= timer_Elapsed;
				timer.Stop();
				timer = null;
			}
		}

		public static void Remove(Form form)
		{
			if(!ContainsKey(form.Handle) && !Contains(form))
				return;

			try
			{
				if(ContainsKey(form.Handle))
				wndHandleHashtable.Remove(wndHandleHashtable.First(x => x.Key.Equals(form.Handle)));
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex, "Error while removing " + form.Handle.ToString() + " from wndHandleHashTable");
			}

			try
			{
				if(Contains(form))
				answerCollection.Remove(answerCollection.First(x => x.Form.Equals(form)));
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex, "Error while removing " + form.Name.ToString() + " from wndHandleHashTable");
			}

			if(timer != null && (wndHandleHashtable == null || wndHandleHashtable.Count == 0) && (answerCollection == null || answerCollection.Count == 0))
			{
				timer.Elapsed -= timer_Elapsed;
				timer.Stop();
				timer = null;
			}
		}
		public static IntPtr Value(IntPtr formHandle)
		{
			if(!ContainsKey(formHandle))
				return IntPtr.Zero;
			try
			{
				return (IntPtr)wndHandleHashtable.FirstOrDefault(x => x.Key.Equals(formHandle)).Value[0];
			}
			catch
			{
				return IntPtr.Zero;
			}
		}

		public static Common.SendAnswerParams Value(Form form)
		{
			if(!Contains(form))
				return null;
			try
			{
				return answerCollection.FirstOrDefault(x => form.Equals(x.Form));
			}
			catch
			{
				return null;
			}
		}

		public static bool ContainsKey(IntPtr formHandle)
		{
			try
			{
				return wndHandleHashtable != null && wndHandleHashtable.Any(x => x.Key.Equals(formHandle));
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex, "Error while trying to find formHandle: " + formHandle.ToString() + ", HashTable is " + (wndHandleHashtable == null ? "" : "NOT ") + "null");
			}
			return false;
		}

		private static void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if((wndHandleHashtable == null || wndHandleHashtable.Count == 0) && (answerCollection == null || answerCollection.Count == 0))
				return;
			try
			{
				Console.Write("+");
				if(wndHandleHashtable != null)
				for(int i = wndHandleHashtable.Count - 1; i > -1; i--)
				{
					KeyValuePair<IntPtr, object[]> item = wndHandleHashtable[i];

					if(item.Value == null)
					{
						if(wndHandleHashtable.Contains(item))
							wndHandleHashtable.Remove(item);
					}
					else
					{
						if(item.Value[1].Equals(false))
						{
							if(Program.MainFormDialog.InvokeRequired)
							{
								Program.MainFormDialog.BeginInvoke(new MessageSender(HideForm.SendMessage),
																   new object[] { (IntPtr)item.Value[0], "s" });
							}
							else
								HideForm.SendMessage((IntPtr)item.Value[0], "s");
							Application.DoEvents();
						}
						else
						{
							var newitem = new KeyValuePair<IntPtr, object[]>(item.Key, new[] { item.Value[0], false });

							if(wndHandleHashtable.Contains(item))
								wndHandleHashtable.Remove(item);

							wndHandleHashtable.Add(newitem);
						}
					}
				}
				//if(answerCollection != null)
				//    using(WebClient wc = new WebClient())
				//    {
				//        wc.Credentials = CredentialCache.DefaultNetworkCredentials;
				//        for(int i = answerCollection.Count - 1; i > -1; i--)
				//        {
				//            Common.SendAnswerParams item = answerCollection[i];

				//            if(String.IsNullOrEmpty(item.AnswerKey))
				//            {
				//                if(answerCollection.Contains(item))
				//                    answerCollection.Remove(item);
				//            }
				//            else
				//            {
				//                var values = new NameValueCollection();
				//                values["value"] = "s";
				//                values["control"] = item.AnswerControl;
				//                values["callbackKey"] = item.AnswerKey;
				//                values["escaped"] = "0";
				//                wc.UploadValues(new Uri(item.AnswerUrl), "POST", values);
				//                Application.DoEvents();
				//            }
				//        }
				//    }
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
				ErrorShower.OnShowError(Program.MainFormDialog, ex.Message, string.Empty);
			}
		}

		internal static void PostAnswer(Common.SendAnswerParams sap, string sendString)
		{
			using(WebClient wc = new WebClient())
			{
				wc.Credentials = CredentialCache.DefaultNetworkCredentials;
				var values = new NameValueCollection();
				values["value"] = sendString;
				values["valueText"] = "";
				values["control"] = sap.AnswerControl;
				values["callbackKey"] = sap.AnswerKey;
				values["command"] = sap.AnswerCommand;
				values["escaped"] = "0";
				wc.UploadValuesAsync(new Uri(sap.AnswerUrl), values);
				Application.DoEvents();
			}
		}
	}

	public delegate int MessageSender(IntPtr hwnd, string str);
}