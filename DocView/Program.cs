using System;
using System.Globalization;
using System.Security;
using System.Threading;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView
{
	/// <summary>
	///   Start class
	/// </summary>
	public class Program
	{
		// single instance vars
		public static Lib.Win.SingleInstance.SingleInstanceHandler InstanceHandler; // класс проверки на запуск архива

		public static ApplicationContext docViewContext;
		public static Forms.HideForm hide;
		internal static Forms.SplashForm splash;
		internal static Forms.MainFormDialog MainFormDialog;
		internal static string[] arguments;

      /// <summary>
		///   The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main(string[] argument)
		{
			arguments = argument;

			try
			{
				// Init and load options
				InstanceHandler = new Lib.Win.SingleInstance.SingleInstanceHandler();
				if(InstanceHandler.IsCreated("DocView.EXE"))
				{
					string str1 = System.Environment.CommandLine;
					string text = " ";
					if(str1.StartsWith("\""))
						text = str1.Substring(str1.IndexOf('"', 1) + 1).Trim();
					else
						if(str1.IndexOf(' ') > 0)
							text = str1.Substring(str1.IndexOf(' ')).Trim();

					if(text.Length == 0)
						text = " ";
					try
					{
						IntPtr hWnd = Lib.Win.Document.Win32.User32.FindWindow(null, "DocViewReciver");
						if(hWnd != IntPtr.Zero)
						{
							int processID = 0;
							Lib.Win.Document.Win32.User32.GetWindowThreadProcessId(hWnd, ref processID);
							if(processID > 0)
							{
								Lib.Win.Document.Win32.User32.AllowSetForegroundWindow(processID);
								Lib.Win.Document.Win32.User32.SetForegroundWindow(hWnd);
							}
							int result = Forms.HideForm.SendMessage(hWnd, text);
							if(result < 1)
								Forms.MainFormDialog.ErrorMessage(
									Environment.StringResources.GetString("Main.Message"),
									Environment.StringResources.GetString("Warning"));
						}
					}
					catch(Exception ex)
					{
						Lib.Win.Data.Env.WriteToLog(ex);
						Forms.MainFormDialog.ErrorMessage(ex.Message, Environment.StringResources.GetString("Main.Error1"));
					}

					return;
				}

				if(AnalyzeArgs(argument))
				{
					if(System.Environment.OSVersion.Version.Major > 5)
						Lib.Win.Document.Win32.User32.SetProcessDPIAware();
					//Application.EnableVisualStyles();
					hide = new Forms.HideForm();
					hide.Show();
					if(!Forms.MainFormDialog.toTray)
					{
						splash = new Forms.SplashForm();
						splash.Show();
					}

					Application.DoEvents();

					bool switchOnLocalLogger = false;

					// ¬ключение локального логировани€ по запуску приложени€ с аргументом либо флагу локального логировани€
#if AdvancedLogging
					switchOnLocalLogger = true;
#else
					if(argument.Length > 0 && argument[0].ToLower() == "/logger")
						switchOnLocalLogger = true;
#endif

					// Init Environment
					Environment.Init(switchOnLocalLogger);

					var root = new Lib.Win.Options.Root();

					// init connection strings
					Environment.ConnectionStringDocument = root.OptionForced<string>("DS_doc").Value as string;
					Environment.ConnectionStringAccounting = root.OptionForced<string>("DS_buh").Value as string;

					// »нициализаци€ репозитори€
					if(Environment.IsConnected)
						Kesco.Lib.Win.Data.Repository.DocumentRepository.Init(Lib.Win.Document.Environment.ConnectionStringDocument);

					using(Lib.Log.Logger.DurationMetter("DS_user load and check"))
						Lib.Win.Document.Environment.ConnectionStringUser = root.OptionForced<string>("DS_user").Value as string;

					if(Environment.IsConnected)
					{
						string curLang = Environment.LangData.GetEmployeeLanguage();

						if(String.IsNullOrEmpty(curLang))
						{
							curLang = Environment.CurEmp.Language;
						}

						if(!String.IsNullOrEmpty(curLang))
						{
							CultureInfo testCulture = null;
							try
							{
								// попытка загрузки €зыка сотрудника
								testCulture = new CultureInfo(curLang);
							}
							catch
							{
								// провалилась
								testCulture = null;
							}
							Environment.CurCultureInfo = testCulture ?? CultureInfo.CurrentCulture;
							if(splash != null)
								splash.ReloadUI();
						}
					}
#if(DEBUG)
					GuiConsole.CreateConsole();
#endif
					try
					{
						MainFormDialog = new Forms.MainFormDialog();
						Console.WriteLine("{0}: Start", DateTime.Now.ToString("HH:mm:ss fff"));
						if(Forms.MainFormDialog.toTray)
						{
							MainFormDialog.WindowState = FormWindowState.Minimized;
							MainFormDialog.ShowInTaskbar = false;
						}

						docViewContext = new ApplicationContext(MainFormDialog);

						// —охранение контекста
						Lib.Win.Document.Environment.UIThreadSynchronizationContext = SynchronizationContext.Current;

						Application.Run(docViewContext);
					}
					catch(Exception ex)
					{
						if(splash != null)
						{
							splash.Close();
							splash = null;
						}
						Lib.Win.Data.Env.WriteToLog(ex);
						Forms.MainFormDialog.ErrorMessage(ex.Message, Environment.StringResources.GetString("Main.Error1"));
						return;
					}
					finally
					{
						hide.Close();
						try
						{
							if(MainFormDialog != null)
								MainFormDialog.notifyIcon.Visible = false;
						}
						catch { }
					}
				}
			}

			catch(SecurityException)
			{
				MessageBox.Show(@"This application must be run from a secure location, such as a local disk.");
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
				Forms.MainFormDialog.ErrorMessage(Environment.StringResources.GetString("Main.Error2") + "\n" + ex.Message);
			}
		}

		#region AnalyzeArgs

		public static bool AnalyzeArgs(string[] args)
		{
			if(args.Length > 0)
			{
				switch(args[0].ToLower())
				{
					case "/exit":
						return false;

					case "/tray":
						Forms.MainFormDialog.toTray = true;
						break;
				}
			}

			return true;
		}

		public static string BuildArgsString(string[] argsArray)
		{
			string argsString = "";

			if(argsArray.Length > 0)
			{
				argsString = argsArray[0];

				for(int i = 1; i < argsArray.Length; i++)
					argsString += " " + argsArray[i];

				argsString = Lib.Win.Document.TextProcessor.ReplaceKesco(argsString);
			}

			return argsString;
		}

		#endregion
	}
}