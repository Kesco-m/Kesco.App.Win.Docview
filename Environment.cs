using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Kesco.Lib.Log;
using Kesco.Lib.Win.Data.DALC;
using Kesco.Lib.Win.Data.DALC.Corporate;
using Kesco.Lib.Win.Data.DALC.Directory;
using Kesco.Lib.Win.Data.DALC.Documents;
using Kesco.Lib.Win.Data.Documents;
using Kesco.Lib.Win.Document;
using Kesco.Lib.Win.Document.Select;

namespace Kesco.App.Win.DocView
{
	/// <summary>
	///   Summary description for Environment.
	/// </summary>
	public class Environment
	{
		private static ResourceManager stringResources;

		private static PersonWord personWord; // объект с падежами "лица"
		private static Lib.Win.Data.Temp.Objects.Employee curEmp; // текущий сотрудник
		private static string userName; // имя пользователя
		private static string companyName; // имя лица фирмы сотрудника

		private static string personSearchString; // Адрес строки поиска лиц
		private static string employeeSearchString; // Адрес строки поиска сотрудников
		private static string createContactString; // Адрес строки создания контакта
		private static string createDocsLinkString; // Адрес строки создания связи между документами

		static string connectionStringErrors = null; // строка соединения с БД ЗаявкиIT

		private static string showHelpString; // Адрес строки показа помощи
		private static string showHelpFirstTimeString; // Адрес строки показа помоци при старте приложения

		private static Lib.Win.Options.Folder settings; // весь набор опций
		private static Lib.Win.Options.Folder general; // общие опции
		private static Lib.Win.Options.Folder layout; // опции внешнего вида
		private static Lib.Win.Options.Folder debug; // опции для отладки
		private static DataTable readData; // количетво сообщений

		private static string printFirstName; // Отпечатка пустого документа при старте.

		private static Form activeForm;

		public static string ZoomString; // zoom для использования в случае электронной форме

		internal static SynchronizedCollection<Lib.Win.MessageForm> messageWindows; // Открытые окна сообщений

		private static CultureInfo curCultureInfo;

		private static bool personMessage;

		public static Dictionary<Type, string> HelpTopics;

		#region DALCs & Readers

		private static ErrorMessageDALC faxErrorMessageData; // Ошибки исходящих факсов

		private static DocTypeLinksDALC docTypeLinkData; // связи типов док.

		private static UnSubscribeDALC unsubscribeData; // отписка от подписки.

		private static FaxFolderDALC faxFolderData; // папки факсов

		private static FaxRecipientDALC faxRecipientData; // получатели факсов

		private static RequestDALC requestData; // заявки

		private static LanguageDALC langData; // языки интерфейса
		private static StampRightsDALC stampRightsData; // права на штампы документов
		private static StampChecksDALC stampChecksData; // правила на штампы документов

		private static ImageRead.ImageReader imageReader; // файлы с диска
		private static ImageRead.ScanReader scanReader; // сканер

		#endregion

		#region Accessors

		/// <summary>
		///   строка подключения к базе данных Документы
		/// </summary>
		public static string ConnectionStringDocument
		{
			get { return Lib.Win.Document.Environment.ConnectionStringDocument; }
			set
			{
				string connectionStringDocument = value;
				if(!string.IsNullOrEmpty(connectionStringDocument))
					connectionStringDocument += ";Application Name=DocView";
				else
				{
					IsConnected = false;
					return;
				}
				TestConnection(connectionStringDocument);
			}
		}

		private static void TestConnection(string connectionStringDocument)
		{
			string IPAddress;
			bool isConnect = DocumentDALC.TestConnection(connectionStringDocument, out IPAddress);
			if(isConnect)
			{
				if(string.IsNullOrEmpty(IPAddress))
					Lib.Win.Data.Env.WriteExtExToLog("Ошибка соединения с базой", "Не получен ip адрес", 
						Assembly.GetExecutingAssembly().GetName(), MethodBase.GetCurrentMethod());
				else
					connectionStringDocument += ";wsid=" + IPAddress;
				Lib.Win.Document.Environment.ConnectionStringDocument = connectionStringDocument;
			}
			else
				Lib.Win.Document.Environment.ConnectionStringDocument = connectionStringDocument;
			IsConnected = isConnect;
			IsConnected &= CurEmp != null;
		}

		public static string ConnectionStringErrors
		{
			get { return connectionStringErrors; }
			set
			{
				connectionStringErrors = value;
				if(!string.IsNullOrEmpty(connectionStringErrors))
				{
					connectionStringErrors += ";Application Name=DocView";
					IsConnectedErrors = DALC.TestConnection(connectionStringErrors);
				}
				else
				{
					IsConnectedErrors = false;
					return;
				}
			}
		}

		/// <summary>
		///   строка подключения к базе данных для Бухгалтерии
		/// </summary>
		public static string ConnectionStringAccounting
		{
			get { return Lib.Win.Document.Environment.ConnectionStringAccounting; }
			set
			{
				Lib.Win.Document.Environment.ConnectionStringAccounting = value + ";Application Name=DocView";
				IsConnectedBuh = DALC.TestConnection(Lib.Win.Document.Environment.ConnectionStringAccounting);
			}
		}

		public static PersonWord PersonWord
		{
			get { return personWord ?? (personWord = new PersonWord(PersonModes.PersonContragent)); }
		}

		public static Lib.Win.Data.Temp.Objects.Employee CurEmp
		{
			get
			{
				try
				{
					if(IsConnected && (curEmp == null || curEmp.ID < 1))
						curEmp = Lib.Win.Data.Temp.Objects.Employee.GetSystemEmployee(EmpData);
				}
				catch(Exception ex)
				{
					Lib.Win.Data.Env.WriteToLog(ex);
				}

				return curEmp;
			}
		}

		/// <summary>
		/// время обновления папок запросов.
		/// </summary>
		public static int FolderUpdateTime;

		public static CultureInfo CurCultureInfo
		{
			get { return curCultureInfo ?? (curCultureInfo = Thread.CurrentThread.CurrentCulture); }
			set
			{
				curCultureInfo = value;
				Lib.Win.Document.Environment.CurCultureInfo = value;
			}
		}

		public static string UserName
		{
			get
			{
				if(userName == null)
				{
					string login = System.Environment.UserName;
					try
					{
						var entry =
							new DirectoryEntry(
								"WinNT://" + System.Environment.UserDomainName + "/" + login + ",user");

						userName = (string)entry.Properties["FullName"][0];
					}
					catch
					{
						userName = login;
					}
				}

				return userName;
			}
		}

		public static string CompanyName
		{
			get
			{
				if(string.IsNullOrEmpty(companyName))
				{
					if(!IsConnected)
						return null;
					if(UserSettings.PersonID > 0)
						try
						{
							companyName = " " + PersonData.GetPerson(UserSettings.PersonID);
						}
						catch
						{
							companyName = " #" + UserSettings.PersonID;
						}
				}
				return companyName;
			}
			set
			{
				companyName = value;
			}
		}

		public static UndoRedoStaсk UndoredoStack
		{
			get { return Lib.Win.Document.Environment.UndoredoStack; }
		}

		/// <summary>
		///   проверка на наличие подключений к БД документов
		/// </summary>
		public static bool IsConnected { get; private set; }

		/// <summary>
		///   проверка на наличие подключения к БД бухгалтерии
		/// </summary>
		public static bool IsConnectedBuh { get; private set; }

		/// <summary>
		///   проверка на наличие подключений к БД ЗАЯВОК
		/// </summary>
		public static bool IsConnectedErrors { get; private set; }

		/// <summary>
		///   строка вызова поиска лиц
		/// </summary>
		public static string PersonSearchString
		{
			get
			{
				if(IsConnected && string.IsNullOrEmpty(personSearchString))
				{
					try
					{
						personSearchString =
							URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.SearchPersonURL).ToString().
								Trim();
					}
					catch(Exception ex)
					{
						Lib.Win.Data.Env.WriteToLog(ex);
					}
				}
				return personSearchString;
			}
			set { personSearchString = value; }
		}

		/// <summary>
		///   строка вызова поиска сотрудника
		/// </summary>
		public static string EmployeeSearchString
		{
			get
			{
				if(IsConnected && string.IsNullOrEmpty(employeeSearchString))
				{
					try
					{
						employeeSearchString =
							URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.SearchEmployeeURL).ToString().
								Trim();
					}
					catch(Exception ex)
					{
						Lib.Win.Data.Env.WriteToLog(ex);
					}
				}
				return employeeSearchString;
			}
			set { employeeSearchString = value; }
		}

		/// <summary>
		///   строка вызова создания контакта
		/// </summary>
		public static string CreateContactString
		{
			get
			{
				if(IsConnected && string.IsNullOrEmpty(createContactString))
				{
					try
					{
						createContactString =
							URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.CreateContractURL).ToString().
								Trim();
					}
					catch(Exception ex)
					{
						Lib.Win.Data.Env.WriteToLog(ex);
					}
				}
				return createContactString;
			}
			set { createContactString = value; }
		}

		/// <summary>
		///   строка вызова создания связи между документами
		/// </summary>
		public static string CreateDocsLinkString
		{
			get
			{
				if(IsConnected && string.IsNullOrEmpty(createDocsLinkString))
				{
					try
					{
						createDocsLinkString = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.CreateDocsLinkURL).ToString().Trim();
					}
					catch(Exception ex)
					{
						Lib.Win.Data.Env.WriteToLog(ex);
					}
				}
				return createDocsLinkString;
			}
			set { createDocsLinkString = value; }
		}

		/// <summary>
		///   строка вызова помощи
		/// </summary>
		public static string ShowHelpString
		{
			get
			{
				if(IsConnected && string.IsNullOrEmpty(showHelpString))
				{
					try
					{
						showHelpString =
							URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.HelpURL).ToString().Trim();
					}
					catch(Exception ex)
					{
						Lib.Win.Data.Env.WriteToLog(ex);
					}
				}
				return showHelpString;
			}
			set { showHelpString = value; }
		}

		/// <summary>
		///   строка вызова создания связи между документами
		/// </summary>
		public static string ShowHelpFirstTimeString
		{
			get
			{
				if(IsConnected && string.IsNullOrEmpty(showHelpFirstTimeString))
				{
					try
					{
						showHelpFirstTimeString =
							URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.ChangesURL).ToString().Trim();
					}
					catch(Exception ex)
					{
						Lib.Win.Data.Env.WriteToLog(ex);
					}
				}
				return showHelpFirstTimeString;
			}
			set { showHelpFirstTimeString = value; }
		}

		/// <summary>
		///   Свойства, хранимые в реестре
		/// </summary>
		public static Lib.Win.Options.Folder Settings
		{
			get
			{
				if(settings != null)
					return settings;

				throw new Exception("Environment: settings not initialized");
			}
		}

		public static Lib.Win.Options.Folder General
		{
			get
			{
				if(general != null)
					return general;

				throw new Exception("Environment: general not initialized");
			}
		}

		public static Lib.Win.Options.Folder Layout
		{
			get
			{
				if(layout != null)
					return layout;

				throw new Exception("Environment: layout not initialized");
			}
		}

		public static Lib.Win.Options.Folder Debug
		{
			get
			{
				if(debug != null)
					return debug;

				throw new Exception("Environment: debug not initialized");
			}
		}

		public static int PreviosTypeID { get; set; }

		public static SelectTypeDialog.Direction PreviosDirection { get; set; }

		public static CommandManagement.CommandManager CmdManager { get; private set; }

		public static UserSettings UserSettings
		{
			get { return Lib.Win.Document.Environment.UserSettings; }
		}

		public static bool PersonMessage
		{
			get { return personMessage; }
			set
			{
				personMessage = value;
				Lib.Win.Document.Environment.PersonMessage = personMessage;
				General.Option("PersonMessage").Value = personMessage.ToString();
				General.Save();
			}
		}

		#region DALCs & Readers Accessors

		public static DocumentDALC DocData
		{
			get { return Lib.Win.Document.Environment.DocData; }
		}

		public static WorkDocDALC WorkDocData
		{
			get { return Lib.Win.Document.Environment.WorkDocData; }
		}

		public static DocDataDALC DocDataData
		{
			get { return Lib.Win.Document.Environment.DocDataData; }
		}

		public static FaxDALC FaxData
		{
			get { return Lib.Win.Document.Environment.FaxData; }
		}

		public static FaxInDALC FaxInData
		{
			get { return Lib.Win.Document.Environment.FaxInData; }
		}

		public static FaxOutDALC FaxOutData
		{
			get { return Lib.Win.Document.Environment.FaxOutData; }
		}

		public static ErrorMessageDALC FaxErrorMessageData
		{
			get
			{
				return faxErrorMessageData ?? (faxErrorMessageData = new ErrorMessageDALC(
																		 Lib.Win.Document.Environment.
																			 ConnectionStringDocument));
			}
		}

		/// <summary>
		///   статический объект доступа к правам на штампы (факсимиле)
		/// </summary>
		public static StampRightsDALC StampRightsData
		{
			get
			{
				return stampRightsData ??
					   (stampRightsData = new StampRightsDALC(Lib.Win.Document.Environment.ConnectionStringDocument));
			}
		}

		/// <summary>
		///   статический объект доступа к правилам на штампы (факсимиле)
		/// </summary>
		public static StampChecksDALC StampChecksData
		{
			get
			{
				return stampChecksData ??
					   (stampChecksData = new StampChecksDALC(Lib.Win.Document.Environment.ConnectionStringDocument));
			}
		}

		/// <summary>
		///   статический объект доступа к штампам (факсимиле)
		/// </summary>
		public static StampDALC StampData
		{
			get { return Lib.Win.Document.Environment.StampData; }
		}

		/// <summary>
		///   статический объект доступа к данным по проводке в 1С
		/// </summary>
		public static BuhParamDocDALC BuhParamDocData
		{
			get { return Lib.Win.Document.Environment.BuhParamDocData; }
		}

		public static SettingsDALC SettingsData
		{
			get { return Lib.Win.Document.Environment.SettingsData; }
		}

		public static DocLinksDALC DocLinksData
		{
			get { return Lib.Win.Document.Environment.DocLinksData; }
		}

		public static DocImageDALC DocImageData
		{
			get { return Lib.Win.Document.Environment.DocImageData; }
		}

		public static DocTypeDALC DocTypeData
		{
			get { return Lib.Win.Document.Environment.DocTypeData; }
		}

		public static DocTypeLinksDALC DocTypeLinkData
		{
			get
			{
				return docTypeLinkData ?? (docTypeLinkData = new DocTypeLinksDALC(
																 Lib.Win.Document.Environment.ConnectionStringDocument));
			}
		}

		public static DocTreeSPDALC DocTreeSPData
		{
			get { return Lib.Win.Document.Environment.DocTreeSPData; }
		}

		public static FolderDALC FolderData
		{
			get { return Lib.Win.Document.Environment.FolderData; }
		}

		/// <summary>
		///   доступ к данным отписки
		/// </summary>
		public static UnSubscribeDALC UnSubscribeData
		{
			get
			{
				return unsubscribeData ??
					   (unsubscribeData = new UnSubscribeDALC(Lib.Win.Document.Environment.ConnectionStringDocument));
			}
		}

		public static SharedFolderDALC SharedFolderData
		{
			get { return Lib.Win.Document.Environment.SharedFolderData; }
		}

		public static MessageDALC MessageData
		{
			get { return Lib.Win.Document.Environment.MessageData; }
		}

		public static FieldDALC FieldData
		{
			get { return Lib.Win.Document.Environment.FieldData; }
		}

		public static ArchiveDALC ArchiveData
		{
			get { return Lib.Win.Document.Environment.ArchiveData; }
		}

		public static FolderRuleDALC FolderRuleData
		{
			get { return Lib.Win.Document.Environment.FolderRuleData; }
		}

		public static LogEmailDALC LogEmailData
		{
			get { return Lib.Win.Document.Environment.LogEmailData; }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static URLsDALC URLsData
		{
			get { return Lib.Win.Document.Environment.URLsData; }
		}

		public static PersonsUsedDALC PersonsUsedData
		{
			get { return Lib.Win.Document.Environment.PersonsUsedData; }
		}

		public static QueryDALC QueryData
		{
			get { return Lib.Win.Document.Environment.QueryData; }
		}

		public static DocSignatureDALC DocSignatureData
		{
			get { return Lib.Win.Document.Environment.DocSignatureData; }
		}

		/// <summary>
		/// доступ к vwПапкиФаксов
		/// </summary>
		public static FaxFolderDALC FaxFolderData
		{
			get { return Lib.Win.Document.Environment.FaxFolderData; }
		}

		public static MailingListDALC MailingListData
		{
			get { return Lib.Win.Document.Environment.MailingListData; }
		}

		public static PersonDALC PersonData
		{
			get { return Lib.Win.Document.Environment.PersonData; }
		}

		public static FaxRecipientDALC FaxRecipientData
		{
			get
			{
				return faxRecipientData ?? (faxRecipientData = new FaxRecipientDALC(Lib.Win.Document.Environment.ConnectionStringDocument));
			}
		}

		public static RequestDALC RequestData
		{
			get
			{
				if(IsConnectedErrors)
					return requestData ?? (requestData = new RequestDALC(connectionStringErrors));
				return null;
			}
		}

		public static EmployeeDALC EmpData
		{
			get { return Lib.Win.Document.Environment.EmpData; }
		}

		public static LanguageDALC LangData
		{
			get { return langData ?? (langData = new LanguageDALC(Lib.Win.Document.Environment.ConnectionStringUser)); }
		}

		public static ImageRead.ImageReader ImageReader
		{
			get { return imageReader ?? (imageReader = new ImageRead.ImageReader(null)); }
		}

		public static ImageRead.ScanReader ScanReader
		{
			get { return scanReader ?? (scanReader = new ImageRead.ScanReader(null)); }
		}

		#endregion

		#endregion

		#region ReadData

        /// <summary>
        /// Объект синхронизации доступа к readData
        /// Необходим, тк данные обновляются из разных потоков
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly object readDataSyncObject = new object();

		/// <summary>
		/// Процедура получает свежие данные папок ВРаботе
		/// </summary>
		public static void ReloadReadData()
		{
			try
			{
				lock(readDataSyncObject)
				{
					if(readData == null)
						readData = FolderData.GetFolders();
					else if(!FolderData.RefreshFolders(ref readData))
					{
						if(readData != null)
							readData.Dispose();

						readData = FolderData.GetFolders();
					}
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		/// <summary>
		/// Получение отображаемого пользователю названия папки ВРаботе и количества документов в папке
		/// </summary>
		/// <param name="id">Код папки</param>
		/// <param name="empID">Код сотрудника</param>
		/// <returns>Название папки, отображаемые пользователю</returns>
		public static DataRow WorkFolderData(int id, int empID)
		{
			try
			{
				lock(readDataSyncObject)
				{
					if(readData != null && readData.Rows.Count > 0)
					{
						return readData.Rows.Cast<DataRow>().FirstOrDefault(x => x[FolderData.IDField].Equals(id) && x[FolderData.ClientIDField].Equals(empID));
					}
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			return null;
		}

		public static bool UpdateWorkFolderData(int id, int empID, int count, int countAll)
		{
			try
			{
				lock(readDataSyncObject)
				{
					if(readData != null)
						for(int i = 0; i < readData.Rows.Count; i++)
						{
							if(readData.Rows[i][FolderData.IDField].Equals(id) && readData.Rows[i][FolderData.ClientIDField].Equals(empID))
							{
								DataRow dr = readData.Rows[i];
								if(count != 0)
								{
									count += dr[Environment.FolderData.UnreadField] is int ? (int)dr[Environment.FolderData.UnreadField] : 0;
									if(count == -1)
										count = 0;
									readData.Rows[i][Environment.FolderData.UnreadField] = count;
								}
								countAll += dr[Environment.FolderData.AllDocsCountField] is int ? (int)dr[Environment.FolderData.AllDocsCountField] : 0;
								if(countAll == -1)
									countAll = 0;
								readData.Rows[i][Environment.FolderData.AllDocsCountField] = countAll;
								return true;
							}
						}
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			return false;
		}

		#endregion

		#region Print to archiv

		public static void PrintOnOk(object source, Lib.Win.DialogEventArgs e)
		{
			var dialog = e.Dialog as Lib.Win.Document.Dialogs.DocPrintDialog;
			if(dialog != null && dialog.DialogResult == DialogResult.OK)
				dialog.Print();
		}

        //public static void SetPrinterProfile()
        //{
        //    printFirstName = null;
        //    Lib.Win.Document.Checkers.TestPrinter.SetPrinterProfile(Lib.Win.Document.Checkers.ProfileType.TiffProfile);
        //}

        /// <summary>
        /// Установить профиль принтера
        /// Этот метод безопасен для вызова из WndProc
        /// 
        /// </summary>
        public static void SetPrinterProfileAsync()
        {
            printFirstName = null;
            Lib.Win.Document.Checkers.TestPrinter.SetPrinterProfileAsync(Lib.Win.Document.Checkers.ProfileType.TiffProfile);
        }

		public static object[] ReprintArgs { get; set; }

		public static void SetPrinterDocParams(int imageID, string fileName, bool isPDFMode, int startPage, int endPage, int countPage, string docName)
		{
			Lib.Win.Document.Environment.SetPrinterDocParams(imageID, fileName, isPDFMode, startPage, endPage, countPage, docName);
		}

		#endregion

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="switchOnLocalLogger">Включить локальное логирование</param>
		public static void Init(bool switchOnLocalLogger = false)
		{
			Console.WriteLine("{0}: Enviroment Thread: {1}", DateTime.Now.ToString("HH:mm:ss fff"), Thread.CurrentThread.GetHashCode());
			CmdManager = new CommandManagement.CommandManager();

			settings = new Lib.Win.Options.Root("DocView");

			// creating folders
			general = settings.Folders.Add("General");
			layout = settings.Folders.Add("Layout");
			debug = settings.Folders.Add("Debug");
			const string appName = "Архив документов";

			Logger.EnterMethod(CmdManager, "logger load");
			// настроим отсылку ошибок в службу поддержки
			Lib.Win.Options.Folder root = new Lib.Win.Options.Root();
			var logModule = new LogModule(appName);
			logModule.Init(root.OptionForced<string>("SmtpServer").Value as string, root.OptionForced<string>("Email_support").GetValue<string>());
			logModule.OnDispose += logModule_OnDispose;
			Logger.Init(logModule, appName, switchOnLocalLogger);
			Logger.LeaveMethod(CmdManager, "logger load");
			// init message windows hash
			messageWindows = new SynchronizedCollection<Lib.Win.MessageForm>();
			UndoredoStack.UndoEnd += UndoredoStack_UndoEnd;
			UndoredoStack.RedoEnd += UndoredoStack_UndoEnd;
			Logger.EnterMethod(CmdManager, "help load");
			HelpTopics = new Dictionary<Type, string>();
			//HelpTopics.Add(, @"Files\Contents\_PageChange.htm");
			HelpTopics.Add(typeof(Lib.Win.Document.Dialogs.DocUserListDialog), @"Files\Contents\Access.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\AddDBDoc.htm");
			HelpTopics.Add(typeof(Lib.Win.Document.Dialogs.AddDBDocDialog), @"Files\Contents\AddDBDocDialog.htm");
			HelpTopics.Add(typeof(Lib.Win.Document.Dialogs.ColorCompressionDialog), @"Files\Contents\ColorCompresionDilog.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\CommonForms.htm");
			HelpTopics.Add(typeof(Lib.Win.Document.Dialogs.DocPrintDialog), @"Files\Contents\DocPrintDialog.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\Document.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\GoTo.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\index.html");
			//HelpTopics.Add(typeof(), @"Files\Contents\Link.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\Menu.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\MenuList.htm");
			HelpTopics.Add(typeof(Dialogs.NewImageAddDialog), @"Files\Contents\NewImageAddDialog.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\NewPageAddDialog.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\Notes.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\OptionsDialog.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\OptionsDialogBuh.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\OptionsDialogDate.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\OptionsDialogDescr.htm");
			HelpTopics.Add(typeof(Forms.XmlSearchForm), @"Files\Contents\OptionsDialogDoc.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\OptionsDialogElForm.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\OptionsDialogFin.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\OptionsDialogImg.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\OptionsDialogLink.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\OptionsDialogMsg.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\OptionsDialogStore.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\OptionsDialogSum.htm");
			HelpTopics.Add(typeof(Lib.Win.Document.Dialogs.PersonContactDialog), @"Files\Contents\PersonContactDialog.htm");
			HelpTopics.Add(typeof(Lib.Win.Document.Dialogs.PrintAllDialog), @"Files\Contents\PrintAllDialog.htm");
			HelpTopics.Add(typeof(PropertiesDialogs.PropertiesDBDocDialog), @"Files\Contents\PropertiesDocDialog.htm");
			HelpTopics.Add(typeof(Lib.Win.Document.PropertiesDialogs.PropertiesDocImageDialog), @"Files\Contents\PropertiesDocImageDialog.htm");
			HelpTopics.Add(typeof(SelectDocDialog), @"Files\Contents\SelectDocDialog.htm");
			HelpTopics.Add(typeof(SelectDocUniversalDialog), @"Files\Contents\SelectDocUniversalDialog.htm");
			HelpTopics.Add(typeof(Dialogs.SelectStampDialog), @"Files\Contents\SelectStamp.htm");
			HelpTopics.Add(typeof(SelectTypeDialog), @"Files\Contents\SelectTypeDialog.htm");
			HelpTopics.Add(typeof(Lib.Win.Document.Dialogs.SendFaxDialog), @"Files\Contents\SendFaxDialog.htm");
			HelpTopics.Add(typeof(Lib.Win.Document.Dialogs.SendMessageDialog), @"Files\Contents\SendMessageDialog.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\SendTo.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\Settings.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\SettingsColor.htm");
			HelpTopics.Add(typeof(Settings.SettingsColumnsDialog), @"Files\Contents\SettingsColumnsDialog.htm");
			HelpTopics.Add(typeof(Settings.SettingsDateFiltersDialog), @"Files\Contents\SettingsDateFiltersDialog.htm");
			HelpTopics.Add(typeof(Settings.SettingsDocumentLoadDialog), @"Files\Contents\SettingsDocumentLoadDialog.htm");
			HelpTopics.Add(typeof(Settings.SettingsFaxesDisplayDialog), @"Files\Contents\SettingsFaxesDisplayDialog.htm");
			HelpTopics.Add(typeof(Settings.SettingsGroupOrderDialog), @"Files\Contents\SettingsGroupOrderDialog.htm");
			HelpTopics.Add(typeof(Lib.Win.Document.Dialogs.MailingListManageDialog), @"Files\Contents\SettingsMailLists.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\SettingsMash.htm");
			HelpTopics.Add(typeof(Settings.SettingsMessagesAndConfirmsDialog), @"Files\Contents\SettingsMessagesAndConfirmsDialog.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\SettingsScaner.htm");
			//HelpTopics.Add(typeof(), @"Files\Contents\View.htm");
			HelpTopics.Add(typeof(Forms.MainFormDialog), @"Files\default.htm");
			HelpTopics.Add(typeof(Lib.Win.Document.Dialogs.SaveChangesDialog), @"Files\Contents\SaveChangesDialog.htm");
			Logger.LeaveMethod(CmdManager, "help load");
#if(DEBUG)
			System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate(Object obj, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
					System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
			{
				return (true);
			};
#endif
		}

		private static void logModule_OnDispose(LogModule sender)
		{
			Lib.Win.Options.Folder root = new Lib.Win.Options.Root();
			sender.Init(root.OptionForced<string>("SmtpServer").GetValue<string>(), root.OptionForced<string>("Email_support").GetValue<string>());
		}

		public static ResourceManager StringResources
		{
			get
			{
				return stringResources ?? (stringResources = new ResourceManager("Kesco.App.Win.DocView.StringResources", Assembly.GetExecutingAssembly()));
			}
		}

		public static void Refresh()
        {
#if AdvancedLogging
            Logger.Message("DocView_Environment Refresh()");
#endif
			CmdManager.Commands["Refresh"].Execute();
		}

		public static void RefreshDocs()
		{
#if AdvancedLogging
            Logger.Message("DocView_Environment RefreshDocs()");
#endif
			CmdManager.Commands["RefreshDocs"].Execute();
		}

		public static string GenerateFileName()
		{
			return Lib.Win.Document.Environment.GenerateFileName(".tif");
		}

		public static bool IsFaxReceiver()
		{
			return IsConnected && EmpData.IsFaxReceiver();
		}

		/// <summary>
		///   проверка на возможность отправки из офиса
		/// </summary>
		public static bool IsFaxSender()
		{
			return IsConnected && EmpData.IsFaxSender();
		}

		/// <summary>
		///   проверка на возможность отправки из офиса по коду документа
		/// </summary>
		public static bool IsFaxSender(int docID)
		{
			if(docID <= 0)
				return IsFaxSender();
			return IsConnected && EmpData.IsFaxSender(DocData.GetDocPersonsIDs(docID));
		}

		/// <summary>
		///   проверка на возможность отправки из офиса по коду документа
		/// </summary>
		public static bool IsFaxSenderFolder(int folderID)
		{
			if(folderID <= 0)
				return IsFaxSender();
			return IsConnected && EmpData.IsFaxSender("," + FaxFolderData.GetFaxFolderPersonID(folderID).ToString());
		}

		/// <summary>
		/// проверка на возможность отправки из офиса по кодам документов
		/// </summary>
		public static int GetImagesToSent(int[] docIDs, out Dictionary<int, int> sendImages)
		{
			sendImages = null;
			if(docIDs.Length <= 0 || !IsConnected)
				return -1;
			StringBuilder sb = new StringBuilder();
			foreach(int docID in docIDs)
			{
				if(sb.Length > 500)
					return -3;
				if(sb.Length > 0)
					sb.Append(",");
				sb.Append(docID);
			}
			if(sb.Length < 1)
				return -1;
			string personsIDsString = DocData.GetDocsPersonsID(sb.ToString());

			if(EmpData.IsFaxSender(personsIDsString))
				return -2;

			sendImages = DocData.GetMainImages(sb.ToString());
			return 0;
		}

		public static int[] CurrentEmpIDs()
		{
			return EmpData.GetCurrentEmployeeIDs();
		}

		public static void SaveActiveForm()
		{
		    try
		    {
#if AdvancedLogging
                using (Logger.DurationMetter("Environment GC.WaitForPendingFinalizers "))
#endif
		        GC.WaitForPendingFinalizers(); //Активная форма как раз в этот момент может закрываться
		        activeForm = Form.ActiveForm;
		    }
		    catch (Exception e)
		    {
		        activeForm = null;

#if AdvancedLogging
		        Logger.Exception("Environment SaveActiveForm ", e);
#endif
		    }
		}

		/// <summary>
		///   данные по подписке
		/// </summary>
		public static string SubscribeTable { get; private set; }

		public static void RestoreActiveForm()
		{
			if(activeForm != null)
				if(activeForm.InvokeRequired)
					activeForm.Invoke((MethodInvoker)(activeForm.BringToFront));
				else
					activeForm.BringToFront();
		}

		public static bool CheckLinkDoc(int parentID, int childID)
		{
			string errorStr = null;

			object obj = DocLinksData.CheckDocLink(parentID, childID);
			if(obj is int)
			{
				var result = (int)obj;
				switch(result)
				{
					case 1:
						//errorStr = "Связь уже есть.";
						return false;

					case 2:
						errorStr = StringResources.GetString("Environment.CheckLinkDoc.Error");
						break;
				}
			}
			else if(obj is DataTable)
			{
				Lib.Win.Document.Dialogs.LoopLinkDialog llDialog = new Lib.Win.Document.Dialogs.LoopLinkDialog(parentID, childID, (DataTable)obj);
				llDialog.Show();

				return false;
			}

			if(errorStr != null)
			{
				Lib.Win.MessageForm.Show(
					StringResources.GetString("Environment.CheckLinkDoc.Error2") + ":\n\n" + errorStr,
					StringResources.GetString("Error"));
				return false;
			}

			return true;
		}

		public static bool LoadMessageFirst;

		static Environment()
		{
			SubscribeTable = "Документы.dbo.РегистрацияРассылки";
			SearchXml = "";
		}

		#region NewWindow

        public static void NewWindow(int docId, string zoom, Context context = null, int docImageId = -1, int page = 1, string path = null, bool createEForm = false, bool forceReplicate = false)
        {
            NewWindow(docId, zoom, null, context, docImageId, page, path, createEForm, forceReplicate);
        }

        /// <summary>
        /// Создать окно
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="zoom"></param>
        /// <param name="parameters">Параметры отображения</param>
        /// <param name="context"></param>
        /// <param name="docImageId"></param>
        /// <param name="page"></param>
        /// <param name="path"></param>
        /// <param name="createEForm"></param>
        /// <param name="forceReplicate"></param>
        public static void NewWindow(int docId, string zoom, Common.ViewParameters parameters, Context context = null, int docImageId = -1, int page = 1, string path = null, bool createEForm = false, bool forceReplicate = false)
		{
			try
			{
				foreach(Forms.SubFormDialog sfd in from t in OpenDocs where docId == t.Key select t.Value as Forms.SubFormDialog)
				{
					if(sfd.WindowState == FormWindowState.Minimized)
						sfd.WindowState = FormWindowState.Normal;
					sfd.BringToFront();
					sfd.Activate();
					if(createEForm)
						sfd.AddEForm();

					return;
				}

				Forms.SubFormDialog newsfd = new Forms.SubFormDialog(docId, docImageId, zoom, parameters, context, page) { DocPathString = path };

				newsfd.Show();
				newsfd.BringToFront();
				newsfd.Activate();

				if(createEForm)
					newsfd.AddEForm();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public static void NewWindow(string fName, string zoom, string docString, Context context, int page)
		{
			try
			{
				Forms.SubFormDialog sfd;
				if(Lib.Win.Document.Environment.TmpFilesContains(fName) &&
					Lib.Win.Document.Environment.GetTmpFile(fName).Window != null)
				{
					sfd = Lib.Win.Document.Environment.GetTmpFile(fName).Window as Forms.SubFormDialog;
					if(sfd != null)
					{
						sfd.Refresh();
						if(sfd.WindowState == FormWindowState.Minimized)
							sfd.WindowState = FormWindowState.Normal;
						sfd.BringToFront();
						sfd.Activate();
						return;
					}
				}

				sfd = new Forms.SubFormDialog(fName, zoom, docString, context, page);
				sfd.Show();
				sfd.BringToFront();
				sfd.Activate();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		#endregion

		#region HashTable Work

		public static SynchronizedCollection<KeyValuePair<int, Form>> OpenDocs
		{
			get { return Lib.Win.Document.Environment.OpenDocs; }
		}

		public static bool OpenDocsContains(int docID)
		{
			return OpenDocs.Any(t => docID == t.Key);
		}

		public static bool AddOpenDoc(int docID, Forms.SubFormDialog sForm)
		{
			if(OpenDocs.Any(t => docID == t.Key))
				return false;

			OpenDocs.Add(new KeyValuePair<int, Form>(docID, sForm));
			return true;
		}

		public static void RemoveOpenDoc(int docID)
		{
			if(OpenDocsContains(docID))
				OpenDocs.Remove(OpenDocs.First(t => t.Key == docID));
		}

		public static bool AddMessageWindow(Lib.Win.MessageForm mForm)
		{
			if(messageWindows.Any(t => t.DocID == mForm.DocID && t.EmpID == mForm.EmpID))
				return false;

			messageWindows.Add(mForm);
			return true;
		}

		public static void RemoveMessageWindow(int docID, int empID, bool close)
		{
			for(int i = 0; i < messageWindows.Count; i++)
				if(messageWindows[i].DocID == docID && (empID == 0 || empID == messageWindows[i].EmpID))
				{
					Lib.Win.MessageForm mForm = messageWindows[i];
					messageWindows.Remove(mForm);
					if(close)
						if(Program.MainFormDialog != null && Program.MainFormDialog.InvokeRequired)
						{
							Program.MainFormDialog.BeginInvoke(
								(Action<Form>)(mmForm => mmForm.Close()),
								mForm);
						}
						else
							mForm.Close();
				}
		}

		public static void ShowHiddenMessageWindows()
		{
			foreach(Lib.Win.MessageForm mForm in messageWindows.Where(t => t.DocID > 0))
			{
				if(Program.MainFormDialog != null && Program.MainFormDialog.InvokeRequired)
				{
					Program.MainFormDialog.BeginInvoke(
						(Action<Form>)delegate(Form form)
										   {
											   if(!form.Visible)
												   form.Show();
										   }, mForm);
				}
				else
				{
					if(!mForm.Visible)
						mForm.Show();
				}
			}
		}

		#endregion

		private static void UndoredoStack_UndoEnd(string command, int count, int lastID)
		{
			if(count <= 0)
				return;

			if(Program.MainFormDialog != null && Program.MainFormDialog.InvokeRequired)
			{
				Program.MainFormDialog.BeginInvoke((MethodInvoker)(() => UndoredoStack_UndoEnd(command, count, lastID)));
			}
			else if(CmdManager.Commands.Contains("Refresh"))
				CmdManager.Commands["Refresh"].Execute();
		}

		public static string SearchXml { get; set; }

		public static bool DoesDocTypeNameExist(int docTypeID)
		{
			return Lib.Win.Document.Environment.DoesDocTypeNameExist(docTypeID);
		}

		public static string GetPersonName(int empID)
		{
			return Lib.Win.Document.Environment.GetPersonName(empID);
		}

		public static Control FindControlAtPoint(Control container, System.Drawing.Point pos)
		{
			Control child;
			foreach(Control c in container.Controls)
			{
				if(c.Visible && c.Bounds.Contains(pos))
				{
					child = FindControlAtPoint(c, new System.Drawing.Point(pos.X - c.Left, pos.Y - c.Top));
					if(child == null)
						return c;
					else
						return child;
				}
			}
			return null;
		}
	}
}