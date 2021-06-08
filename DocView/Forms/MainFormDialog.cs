using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using Kesco.App.Win.DocView.FolderTree.FolderNodes;
using Kesco.App.Win.DocView.Grids.Styles;
using Kesco.Lib.Win.Data.DALC.Documents.Search;
using Kesco.Lib.Win.Data.Repository;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Document;
using Kesco.Lib.Win.Document.Components;
using Kesco.Lib.Win.Document.Dialogs;
using Kesco.Lib.Win.Document.Search;
using Kesco.Lib.Win.Document.Select;
using Kesco.Lib.Win.Error;
using Kesco.Lib.Win.ImageControl;
using Kesco.Lib.Win.Options;
using Kesco.Lib.Win.Receive;

namespace Kesco.App.Win.DocView.Forms
{
	/// <summary>
	/// Основная форма приложения
	/// </summary>
	public partial class MainFormDialog : Kesco.Lib.Win.FreeDialog
    {

		public MainFormDialog()
		{
			Thread.CurrentThread.CurrentUICulture = Environment.CurCultureInfo;
			InitializeComponent();
			this.tableLayoutPanel1.Controls.Remove(this.toolBar);
			this.tableLayoutPanel1.Visible = false;
			this.Controls.Remove(this.tableLayoutPanel1);
			this.Controls.Add(this.toolBar);
			opinionControl.Enabled = false;
			opinionControl.Visible = false;
			InitializeTranlation();
			try
			{
				toolBar.ImageScalingSize = new Size((int)(toolBar.ImageScalingSize.Width * Lib.Win.Document.Environment.Dpi / 96), (int)(toolBar.ImageScalingSize.Height * Lib.Win.Document.Environment.Dpi / 96));

				docControl.CanSave = true;

				KeyPreview = true;

				keyLocker = new SynchronizedCollection<Keys>();

				showDocsAndMessages = Environment.General.LoadIntOption("ShowDocAndMessage", 0);

				// setting person word where needed
				menuGotoPerson.Text += Environment.PersonWord.GetForm(Cases.R, false, false);

				// Init Command Manager
				Slave.DoWork(InitializeCommandManager, null);

				// setting status bar refresh
				statusBarPanelDoc.Text = string.Empty;

				Kesco.Lib.Win.Data.Settings.DS_document = Environment.ConnectionStringDocument;
				Kesco.Lib.Win.Data.Settings.DS_person = Environment.ConnectionStringDocument;

				if(!Environment.IsConnected)
					ErrorMessage(
						Environment.StringResources.GetString("MainForm.MainFormDialog.MainFormDialog.Error1"),
						Environment.StringResources.GetString("Error"));
				if(!Environment.IsConnectedBuh)
					ErrorMessage(
						Environment.StringResources.GetString("MainForm.MainFormDialog.MainFormDialog.Error2"),
						Environment.StringResources.GetString("Error"));

				Lib.Win.Document.Environment.NewWindow += DocControl_NewWindow;
				Lib.Win.Document.Environment.NeedRefresh += new EventHandler(docControl_NeedRefresh);

				// filename, path, page, scrollpositionx, scrollpositiony
				Environment.General.LoadStringOption("FileName", string.Empty);
				Environment.General.LoadStringOption("Path", string.Empty);
				Environment.General.LoadStringOption("ContextMode", Misc.ContextMode.WorkFolder.ToString());

				Environment.General.LoadIntOption("Page", 0);
				Environment.General.LoadIntOption("ScrollPositionX", 0);
				Environment.General.LoadIntOption("ScrollPositionY", 0);

				Environment.General.LoadIntOption("DocID", 0);
				Environment.General.LoadIntOption("ImageID", -1);

				// Величина максимальной задержки получения сообщения(макс. время хранения принятого сообщения в буфере сообщений), сек
				// Для изменения значения прописать значение в реестре
				int messageMaxDelayInterval = Environment.General.LoadIntOption("MessagesRefreshTimeout", 2000);

				// Проверка на диапазон значений. От 1000 милисек до 60000 милисек
				if(messageMaxDelayInterval >= 1000 && messageMaxDelayInterval <= 60000)
					gotMessageTimeout = messageMaxDelayInterval;

				// масштаб документа
				zoomCombo.Text = Environment.Layout.LoadStringOption("Zoom", Environment.StringResources.GetString("ToWidth"));

				// установка "нажатости" кнопки в зависимости от режима картинки
				selectionButton.Checked = docControl.SelectionMode;

				// установка в меню цвета и масштабирования
				switch(docControl.GetImagePalette())
				{
					case 1:
						menuColor1.Checked = true;
						break;

					case 2:
						menuColor2.Checked = true;
						break;

					case 3:
						menuColor3.Checked = true;
						break;
				}

				switch(docControl.GetDisplayScaleAlgorithm())
				{
					case 1:
						menuScale1.Checked = true;
						break;

					case 2:
						menuScale2.Checked = true;
						break;

					case 3:
						menuScale3.Checked = true;
						break;
				}

				// инициализация датагрида
				docGrid.Init(Environment.Layout, this);

				// инициализация инфогрида
				infoGrid.Init(Environment.Layout, this);
				SetInfoPlace();

				// Максимальное число результатов поиска
				maxSearchResults = Environment.General.LoadIntOption("MaxSearchResults", maxSearchResults);


				// таймер статусбара
				statusBarTimer = new System.Windows.Forms.Timer();
				statusBarTimer.Tick += StatusBarTimerProcessor;

				statusBarTimer.Interval =
					(int)Convert.ToUInt32((DateTime.Today.AddDays(1) - DateTime.Now.AddMinutes(1)).TotalMilliseconds);
				statusBarTimer.Start();
				//обновление статуса 
				if(statusBar != null)
					statusBar.Panels[3].Text = DateTime.Now.ToString("dd.MM.yyyy");

				if(Convert.ToBoolean(Environment.General.LoadStringOption("CatchScan", false.ToString())))
				{
					KeyboardLLHookProcedure = KeyboardLLHookProc;
					int t = 0;
					while(Lib.Win.HookClass.hHook == 0 && t < 3)
					{
						Lib.Win.HookClass.hHook = Lib.Win.HookClass.SetWindowsHookEx(Lib.Win.HookClass.WH_KEYBOARD_LL, KeyboardLLHookProcedure,
									 Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
						t++;
					}
				}
				DocControlComponent.DocumentSaved += docComponent_DocumentSaved;
				DocControlComponent.FaxInContactCreated += Lib.Win.Document.Controls.DocControl.docComponent_FaxInContactCreated;
				docControl.NeedRefreshGrid += docControl_NeedRefreshGrid;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void InitializeTranlation()
		{
			this.menuLinkEform.Text = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message2") + "...";
		}

        #region Variables & Constants

        private Lib.Win.HookProc KeyboardLLHookProcedure;

		private volatile SynchronizedCollection<Keys> keyLocker;

        private System.Timers.Timer timer;
        private bool buttonPush;
        private StringBuilder pushedButton = new StringBuilder();

		private volatile int showDocsAndMessages;
		private volatile int showMessageState;									// положение infoGrid'а 0- между docGrid и folders, 1 - слева от docConrol, 2 - под DocControl

        public const int MinZoom = 2;										// минимальный масштаб в %
        public const int MaxZoom = 6500;									// максимальный масштаб в %
        public string zoom = string.Empty;

        public static bool toTray;										// Сверуть в трей

        public enum SFIndex													// индексы "особенных" папок
        {
            Faxes = -2,
            None = -1,
            CatalogScan
        }

        public enum ButtonCheck
        {
            No,
            MoveImg,
            SelectAnn,
            SelectingPart,
            Marker,
            Rectangle,
            Text,
            Note,
            ImageStamp
        }

        private ButtonCheck bCheck = ButtonCheck.No;

        private string fileName;
        private string path;
        internal static int curDocID;
        internal static int curImageID = -1;
        internal static int curFaxID;
        private string curDocString;
        private bool zoomIsBeingUpdated;

        internal static int nextPersonID;

        internal static int maxSearchResults = 500;				// максимальное кол-во результатов поиска

        internal static Receive docChangedReceiver;	// получатель сообщений	об изменении документа
        internal static Receive messageReceiver;				// получатели сообщений о сообщениях по документу
        internal static Receive faxInReceiver;		// получатели сообщений о входящих факсах

        private System.Windows.Forms.Timer readTimer;			// таймер прочтения
        private System.Windows.Forms.Timer statusBarTimer;		// таймер для обновления статусбара

        private const int afterLoadTimeout = 300; // таймаут загузки изображения документа
        private System.Timers.Timer afterLoadTimer;

        private const int documentChangeTimeout = 3000;
        private System.Timers.Timer documentChangeTimer;

        private const int documentSelectedTimeout = 700;
        private System.Timers.Timer documentSelectedTimer;

        private int gotMessageTimeout = 2000; // таймаут при получении сообщения
        private System.Timers.Timer gotMessageTimer;
        private BackgroundWorker docMessageMarker; // фоновый проставитель статуса Прочитано / НеПрочитано выделенных для документов

		private System.Windows.Forms.Timer folderUpdateTimer;

        private const int gotFaxTimeout = 2000; // таймаут при получении факсов
        private System.Timers.Timer gotFaxTimer;
        private BackgroundWorker gotFaxProcessor;

        internal static string returnFileName;					// имя файла возврата
        internal static int returnID;							// id документа возврата (0 если нет)
        internal static bool returnForce;						// Принудительная загрузка документа из внешнего архива
        internal static string returnPath;						// путь для документа возврата (null если отсутствует)
        internal static Context returnContext;                  // контекст возврата

        // настройки пользователя

        internal static int templateDocTypeID = -1;				// шаблонный тип документа
        internal static DateTime templateDocDate = DateTime.MinValue; // шаблонная дата документа
        internal static int[] templatePersonIDs;			// шаблонные коды лиц

        internal static string personParamStr = "clid=4&return=1";				// параметры URL'а выбора лиц
        internal static string userParamStr = "clid=3&return=1&UserOur=true&UserAccountDisabled=0";	// параметры URL'а выбора сотрудника
        internal static string userMultipleParamStr = "clid=3&return=2&UserOur=true&UserAccountDisabled=0";	// параметры URL'а выбора сотрудников (множественный)

        internal static Dialogs.ChangesDialog changesDialog;

        public delegate void RestoreDelegate();

        internal static bool stateLoaded;

        private FormWindowState lastGoodState = FormWindowState.Normal;

        private FileInfo oldFileInfo;

        bool openDocWin;

        private MailingListManageDialog mlmDialog;

        int eformID;
        int documentID;
        int transID;
        int linksID;
        int document1СID;
        int signID;

        private SynchronizedCollection<string> _ignoreDocChanges = new SynchronizedCollection<string>();
        private SynchronizedCollection<string> _gotFax = new SynchronizedCollection<string>();
        private SynchronizedCollection<object[]> _markDocMessagesArgs = new SynchronizedCollection<object[]>();
        private static readonly object _gotUpdatedDocsCodeLock = new object();
        private static int _gotUpdatedDocsCode = 0;

        #endregion

        #region Accessors

        public Context curContext
        {
            get { return folders.GetContext() ?? new Context(Misc.ContextMode.None); }
        }

		public int curEmpID
		{
			get
			{
				try
				{
					if(curContext.Emp != null)
						return curContext.Emp.ID;
					var workNode = folders.SelectedNode as FolderTree.FolderNodes.WorkNodes.WorkNode;
					if(workNode != null && workNode.Emp != null)
						return workNode.Emp.ID;
					return Environment.CurEmp.ID;
				}
				catch(Exception ex)
				{
					Lib.Win.Data.Env.WriteToLog(ex);
				}
				return -1;
			}
		}

        #endregion

        #region Instance...

        public void RestoreWindow(string argStr, IntPtr wndHandle)
        {
            GetFromTray();

            if (string.IsNullOrEmpty(argStr))
                return;

            CursorSleep();

            string[] args = Regex.Split(argStr, System.Environment.NewLine);
            if (!Program.AnalyzeArgs(args))
                ExitApp();

            CursorWake();
        }

        #endregion

        #region Annotation Bar

        private void annotationBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Right == e.Button)
            {
                int x = e.X;
                int y = e.Y;
                int b = GetButton(x, y);
                switch (b)
                {
                    case 1:
                        docControl.ShowAttribsDialog(2);
                        break;
                    case 2:
                        docControl.ShowAttribsDialog(4);
                        break;
                    case 3:
                        docControl.ShowAttribsDialog(6);
                        break;
                    case 4:
                        docControl.ShowAttribsDialog(7);
                        break;
                }
            }
        }

        private int GetButton(int x, int y)
        {
            if (annotationBar.Buttons.Count > 0)
            {
                if (y > annotationBar.Buttons[0].Rectangle.Y || y < annotationBar.Buttons[0].Rectangle.Bottom)
                {
                    for (int i = 0; i < annotationBar.Buttons.Count; i++)
                    {
                        if (x > annotationBar.Buttons[i].Rectangle.X && x < annotationBar.Buttons[i].Rectangle.Right)
                            return i;
                    }
                }
            }
            return -1;
        }

        #endregion

        #region Annotation Select Tool

        private void On_Line(CommandManagement.Command cmd)
        {
            docControl.SelectTool(4);
            docControl.AnnotationDraw = true;
        }

        private void On_FLine(CommandManagement.Command cmd)
        {
            docControl.SelectTool(2);
            docControl.AnnotationDraw = true;
        }

        private void On_Highlighter(CommandManagement.Command cmd)
        {
            docControl.SelectTool(3);
            docControl.AnnotationDraw = true;
        }

        private void On_Rectangle(CommandManagement.Command cmd)
        {
            docControl.SelectTool(5);
            docControl.AnnotationDraw = true;
        }

        private void On_HRectangle(CommandManagement.Command cmd)
        {
            docControl.SelectTool(6);
            docControl.AnnotationDraw = true;
        }

        private void On_Text(CommandManagement.Command cmd)
        {
            docControl.SelectTool(7);
            docControl.AnnotationDraw = true;
        }

        private void On_Note(CommandManagement.Command cmd)
        {
            docControl.SelectTool(8);
            docControl.AnnotationDraw = true;
        }

        private void On_Stamp(CommandManagement.Command cmd)
        {
            Image res = null;
            int id = 0;
            docControl.TestImage(Lib.Win.Document.Environment.ActionBefore.None);
            using (Dialogs.SelectStampDialog dlg = new Dialogs.SelectStampDialog(curImageID))
            {
                dlg.StartPosition = FormStartPosition.CenterParent;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    res = dlg.GetSelectedStamp();
                    id = dlg.GetStampID();
                }
            }
            if (id > 0)
            {
                docControl.SelectTool(9);
                docControl.CurrentStamp = res;
                docControl.CurrentStampID = id;
                docControl.AnnotationDraw = true;
            }
        }

        private void On_StampDSP(CommandManagement.Command cmd)
        {
            docControl.TestImage(Lib.Win.Document.Environment.ActionBefore.None);
            docControl.Page = 1;
            docControl.SelectTool(9);
            docControl.CurrentStamp = (Image)Lib.Win.Document.Environment.GetDSP().Clone();
            docControl.CurrentStampID = -101;
            docControl.AnnotationDraw = true;
        }

        private void On_Select(CommandManagement.Command cmd)
        {
            docControl.SelectionMode = false;
            docControl.SelectTool(1);
            docControl.AnnotationDraw = true;
        }

        private void On_View(CommandManagement.Command cmd)
        {
            docControl.SelectionMode = false;
            docControl.AnnotationDraw = false;
            docControl.SelectTool(0);
        }

        #endregion

        #region Annotation Show

        private void On_Hide(CommandManagement.Command cmd)
        {
            docControl.HideAnnotationGroup(Missing.Value);
            for (int i = mINotes.MenuItems.Count - 1; i >= 0; i--)
            {
                if (mINotes.MenuItems[i].Tag != null && mINotes.MenuItems[i].Tag.Equals("group"))
                {
                    mINotes.MenuItems[i].Checked = false;
                }
            }
        }

        private void On_Self(CommandManagement.Command cmd)
        {
            docControl.HideAnnotationGroup(Missing.Value);
            docControl.ShowAnnotationGroup(docControl.GetCurrentAnnotationGroup());
            for (int i = mINotes.MenuItems.Count - 1; i >= 0; i--)
            {
                if (mINotes.MenuItems[i].Text == docControl.GetCurrentAnnotationGroup())
                {
                    mINotes.MenuItems[i].Checked = true;
                }
                else
                {
                    if (mINotes.MenuItems[i].Tag != null && mINotes.MenuItems[i].Tag.Equals("group"))
                    {
                        mINotes.MenuItems[i].Checked = false;
                    }
                }
            }
        }

        private void On_Show(CommandManagement.Command cmd)
        {
            docControl.ShowAnnotationGroup(Missing.Value);
            if (mISeparator.Visible)
            {
                for (int i = mINotes.MenuItems.Count - 1; i >= 0; i--)
                {
                    if (mINotes.MenuItems[i].Tag != null && mINotes.MenuItems[i].Tag.Equals("group"))
                    {
                        mINotes.MenuItems[i].Checked = true;
                    }
                }
            }
        }

        private void mi_Click(object sender, EventArgs e)
        {
            var mi = sender as MenuItem;
            if (mi == null)
                return;
            bool check = !mi.Checked;
            mi.Checked = check;
            var groupnum = (short)(mi.Index - mISeparator.Index);
            string groupName = docControl.GetAnnotationGroup(groupnum - 1);
            if (check)
                docControl.ShowAnnotationGroup(groupName ?? string.Empty);
            else
                docControl.HideAnnotationGroup(groupName ?? string.Empty);
        }

        private void textShow(string text)
        {
            if (Disposing || IsDisposed)
                return;
            try
            {
                if (mISeparator.Visible)
                {
                    mISeparator.Visible = false;
                    for (int i = mINotes.MenuItems.Count - 1; i >= 0; i--)
                    {
                        if (mINotes.MenuItems[i].Tag != null && mINotes.MenuItems[i].Tag.Equals("group"))
                            mINotes.MenuItems.Remove(mINotes.MenuItems[i]);
                    }
                }
                if (docControl.IsMoveImage)
                    bCheck = ButtonCheck.MoveImg;
                else if (docControl.IsEditNotes)
                    bCheck = ButtonCheck.SelectAnn;
                else if (docControl.SelectionMode)
                    bCheck = ButtonCheck.SelectingPart;


                if (docControl.AnnotationGroupCount > 0)
                {
                    for (short i = 0; i < (short)docControl.AnnotationGroupCount; i++)
                    {
                        string groupName = docControl.GetAnnotationGroup(i);
                        if (string.IsNullOrEmpty(groupName))
                            groupName = string.Empty;
                        var mi =
                            new MenuItem(groupName == string.Empty
                                             ? Environment.StringResources.GetString(
                                                 "MainForm.MainFormDialog.textShow.Message1")
                                             : groupName) { Tag = "group" };

                        if (docControl.IsNotesOnlySelfShow && groupName == docControl.GetCurrentAnnotationGroup())
                        {
                            docControl.ShowAnnotationGroup(groupName);
                            mi.Checked = true;
                        }
                        else if (docControl.IsNotesOnlySelfShow)
                        {
                            mi.Checked = false;
                        }
                        else if (docControl.IsNotesAllHide)
                        {
                            mi.Checked = false;
                        }
                        else
                            mi.Checked = true;
                        mi.Click += mi_Click;

                        mINotes.MenuItems.Add(mi);
                    }
                    if (docControl.IsNotesAllShow)
                        docControl.ShowAnnotationGroup(Missing.Value);
                    mISeparator.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        #endregion

        #region Args, Analyze Args

        /// <summary>
        /// Анализ аргументов запуска приложения
        /// </summary>
        /// <param name="argsString"></param>
        /// <returns></returns>
        private bool AnalyzeArgsFileName(string argsString)
        {
            if (Disposing || IsDisposed)
                return false;

            string argPath = string.Empty;
			try
			{
				if(File.Exists(argsString))
				{
					var fileInfo = new FileInfo(argsString);

					if(Lib.Win.Document.Environment.DocIsPrintedFromUDC(fileInfo.FullName))
					{
					    return DocIsPrintedFromUDCHandler(fileInfo);
					}

					Environment.General.Option("FileName").Value = fileInfo.FullName;

					if(fileInfo.Directory != null)
						argPath = fileInfo.Directory.FullName;
                    Console.WriteLine("{0}: {1}", DateTime.Now.ToString("HH:mm:ss fff"), argPath);
				}
				else if(Directory.Exists(argsString))
				{
					var dirInfo = new DirectoryInfo(argsString);
					argPath = Path.Combine(dirInfo.Parent == null ? "" : dirInfo.Parent.FullName, dirInfo.Name);
					if(!string.IsNullOrEmpty(Lib.Win.Document.Environment.PrinterPath) && Lib.Win.Document.Environment.PrinterPath.Equals(argPath, StringComparison.CurrentCultureIgnoreCase))
					{
						argPath = "";
						return false;
					}
				}

				if(argPath.Length > 0)
				{
					Environment.General.Option("Path").Value = argPath;
					Environment.General.Option("ContextMode").Value = ParsePath(argPath).ToString();
				}
				return true;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
				return false;
			}
        }

       /// <summary>
       /// Обработчик печати архивом документов. Если внешнее приложение отправило на печать в принтер "Архив документов"
       /// </summary>
       /// <param name="fileInfo"></param>
       /// <returns></returns>
        private bool DocIsPrintedFromUDCHandler(FileInfo fileInfo)
        {
            if (Lib.Win.Document.Checkers.TestPrinter.CheckPrinterProfileAsync())
            {
                var r = new Regex(@"^#(?<image>\d+)(\(\d+\))?(#(?<type>\d+))?(\(\d+\))?\.tif$");
                Match m = r.Match(fileInfo.Name);
                if (m.Success)
                {
                    int typeId;
                    int docId;

                    if (!int.TryParse(m.Groups["image"].Value, out docId) || !Environment.DocData.IsDocAvailable(docId))
                        docId = 0;
                    if (string.IsNullOrEmpty(m.Groups["type"].Value) || !int.TryParse(m.Groups["type"].Value, out typeId))
                        typeId = 0;

                    ServerInfo server = Lib.Win.Document.Environment.GetRandomLocalServer();
                    bool isPDF = Lib.Win.Document.Environment.IsPdf(fileInfo.FullName);
                    int count = docControl.GetFilePagesCount(fileInfo.FullName, isPDF);
                    string fileName = Environment.GenerateFileName();
                    string path = server.Path + "\\TEMP\\" + fileName;
                    if (File.Exists(path))
                        File.Delete(path);
                    DateTime creationTime = fileInfo.CreationTimeUtc;
                    File.Move(fileInfo.FullName, path);
                    int imgID = 0;
                    Environment.DocImageData.DocImageInsert(server.ID, fileName, ref imgID, ref docId, 0, "",
                                                            DateTime.MinValue, "", "", false, creationTime, 0,
															false, typeId, isPDF?"PDF":"TIF", count);
                    if (docControl.DocumentID == docId)
                        docControl.RefreshDoc();
                    else
                    {
                        int dialIndex = -1;
                        for (int i = 0; i < Environment.OpenDocs.Count && dialIndex == -1; i++)
                            if (Environment.OpenDocs[i].Key == docId)
                                dialIndex = i;

                        if (dialIndex > -1)
                        {
                            var dial = Environment.OpenDocs[dialIndex].Value as SubFormDialog;
                            if (dial != null)
                            {
                                dial.docControl.RefreshDoc();
                                if (dial.WindowState == FormWindowState.Minimized)
                                    dial.WindowState = FormWindowState.Normal;
                                dial.Show();
                                dial.BringToFront();
                            }
                        }
                        else
                        {
                            returnID = docId;
                            Environment.CmdManager.Commands["Return"].Execute();
                        }
                    }
                    return false;
                }
                else
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action<string>(
                                   delegate(string t)
                                   {
                                       Lib.Win.Document.Environment.AddTmpFile("", t, Lib.Win.Document.Environment.IsPdf(t));
                                       var subDialog = new SubFormDialog(t, Environment.ZoomString,
                                                                         Environment.StringResources.
                                                                             GetString("MainForm.MainFormDialog.AnalyzeArgsFileName.Title1"),
                                                                         new Context(Misc.ContextMode.None));

                                       subDialog.Tag = Lib.Win.Document.Environment.GetPrinterDocParams(t);
                                       subDialog.SetSave();
                                       subDialog.Show();
                                   }), fileInfo.FullName);
                    }
                    else
                    {
                        Lib.Win.Document.Environment.AddTmpFile("", fileInfo.FullName, Lib.Win.Document.Environment.IsPdf(fileInfo.FullName));
                        var subDialog = new SubFormDialog(fileInfo.FullName, Environment.ZoomString,
                                                          Environment.StringResources.GetString("MainForm.MainFormDialog.AnalyzeArgsFileName.Title1"),
                                                          new Context(Misc.ContextMode.None));
                        subDialog.Tag = Lib.Win.Document.Environment.GetPrinterDocParams(fileInfo.FullName);
                        subDialog.SetSave();
                        subDialog.Show();
                    }
                }
            }
            else
            {
                Environment.ReprintArgs = Lib.Win.Document.Environment.GetPrinterDocParams(fileInfo.FullName) ?? new object[] { fileInfo.FullName };
                Slave.DeleteFile(fileInfo.FullName);
                Environment.CmdManager.Commands["Reprint"].Execute();
            }
            return false;
        }

        private Misc.ContextMode ParsePath(string argPath)
		{
			try
			{
				if(Lib.Win.Document.Environment.IsConnectedDocs &&
					Lib.Win.Document.Environment.GetServers().Any(t => argPath.StartsWith(t.Path, StringComparison.OrdinalIgnoreCase)))
					return Misc.ContextMode.Catalog;

				SFIndex matchedIndex = SFIndex.None;
				
				if(Lib.Win.Document.Environment.IsConnectedDocs &&
					Lib.Win.Document.Environment.GetServers().Any(t => !string.IsNullOrEmpty(t.ScanPath) && argPath.StartsWith(t.ScanPath, StringComparison.OrdinalIgnoreCase))) // special folder scan
				{
					matchedIndex = SFIndex.CatalogScan;
				}

				if(Lib.Win.Document.Environment.IsConnectedDocs &&
					Lib.Win.Document.Environment.GetServers().Any(t =>!string.IsNullOrEmpty(t.FaxPath) && argPath.StartsWith(t.FaxPath, StringComparison.OrdinalIgnoreCase))) // special folder fax
				{
					matchedIndex = SFIndex.Faxes;
				}
				

				switch(matchedIndex)
				{
					case SFIndex.CatalogScan:
						return Misc.ContextMode.Scaner;

					case SFIndex.Faxes:
						return Misc.ContextMode.FaxIn;		// переделать

					//case SFIndex.FaxOut:
					//return ContextMode.FaxOut;	// переделать
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			return Misc.ContextMode.SystemFolder;
		}

        #endregion

        #region Color & Scale

        private void menuColor1_Click(object sender, EventArgs e)
        {
            menuColor1.Checked = docControl.SetImagePalette(1);
            menuColor2.Checked = false;
            menuColor3.Checked = false;
        }

        private void menuColor2_Click(object sender, EventArgs e)
        {
            menuColor1.Checked = false;
            menuColor2.Checked = docControl.SetImagePalette(2);
            menuColor3.Checked = false;
        }

        private void menuColor3_Click(object sender, EventArgs e)
        {
            menuColor1.Checked = false;
            menuColor2.Checked = false;
            menuColor3.Checked = docControl.SetImagePalette(3);
        }

        private void menuScale1_Click(object sender, EventArgs e)
        {
            menuScale1.Checked = docControl.SetDisplayScaleAlgorithm(1);
            menuScale2.Checked = false;
            menuScale3.Checked = false;
        }

        private void menuScale2_Click(object sender, EventArgs e)
        {
            menuScale1.Checked = false;
            menuScale2.Checked = docControl.SetDisplayScaleAlgorithm(2);
            menuScale3.Checked = false;
        }

        private void menuScale3_Click(object sender, EventArgs e)
        {
            menuScale1.Checked = false;
            menuScale2.Checked = false;
            menuScale3.Checked = docControl.SetDisplayScaleAlgorithm(3);
        }

        #endregion

        #region CommandManager

        private void InitializeCommandManager(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Show web panel
                if (showWebPanelButton != null)
                {
                    Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                            "ShowWeb",
                                                            On_ShowWeb,
                                                            UpdateCommand_ShowWeb));

                    Environment.CmdManager.Commands["ShowWeb"].CommandInstances.Add(new Object[]
                                                                                        {
                                                                                            menuShowWebPanel,
                                                                                            showWebPanelButton
                                                                                        });
                }

                //show info
                if (tBBShowMessage != null)
                {
                    Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                            "ShowMessage",
                                                            On_ShowMessage,
                                                            UpdateCommand_ShowMessage));

                    Environment.CmdManager.Commands["ShowMessage"].CommandInstances.Add(new Object[]
                                                                                            {
                                                                                                menuShowMessage,
                                                                                                tBBShowMessage
                                                                                            });
                }

                // Save
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Save",
                                                        On_Save,
                                                        UpdateCommand_Save));

                Environment.CmdManager.Commands["Save"].CommandInstances.Add(new Object[] { menuSave, saveButton });

                // Print
                if (printButton != null)
                {
                    Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                            "Print",
                                                            On_Print,
                                                            UpdateCommand_Print));

                    Environment.CmdManager.Commands["Print"].CommandInstances.Add(new Object[] { menuPrintPage, printButton });
                }

                // Print document
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "PrintDoc",
                                                        On_PrintDoc,
                                                        UpdateCommand_PrintDoc));

                Environment.CmdManager.Commands["PrintDoc"].CommandInstances.Add(new Object[] { menuPrintDoc });

                // Print Folder
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "PrintFolder",
                                                        On_PrintFolder,
                                                        UpdateCommand_PrintFolder));

                Environment.CmdManager.Commands["PrintFolder"].CommandInstances.Add(new Object[] { menuFolder });

                // Open Folder
                if (openFolderButton != null)
                {
                    Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                            "OpenFolder",
                                                            On_OpenFolder,
                                                            null));

                    Environment.CmdManager.Commands["OpenFolder"].CommandInstances.Add(new Object[]
                                                                                           {
                                                                                               menuOpenFolder,
                                                                                               openFolderButton
                                                                                           });
                }

                // Zoom
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Zoom",
                                                        On_Zoom,
                                                        UpdateCommand_Zoom));

                Environment.CmdManager.Commands["Zoom"].CommandInstances.Add(new Object[] { menuZoom });

                // ZoomIn
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "ZoomIn",
                                                        On_ZoomIn,
                                                        UpdateCommand_ZoomIn));

                Environment.CmdManager.Commands["ZoomIn"].CommandInstances.Add(new Object[] { menuZoomIn, zoomInButton });

                // ZoomOut
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "ZoomOut",
                                                        On_ZoomOut,
                                                        UpdateCommand_ZoomOut));

                Environment.CmdManager.Commands["ZoomOut"].CommandInstances.Add(new Object[] { menuZoomOut, zoomOutButton });


                // Selection
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Selection",
                                                        On_Selection,
                                                        UpdateCommand_Selection));

                Environment.CmdManager.Commands["Selection"].CommandInstances.Add(new Object[] { menuSelection, selectionButton });

                // ZoomSelection
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "ZoomSelection",
                                                        On_ZoomSelection,
                                                        UpdateCommand_ZoomSelection));

                Environment.CmdManager.Commands["ZoomSelection"].CommandInstances.Add(new Object[]
                                                                                          {
                                                                                              menuZoomSelection,
                                                                                              zoomSelectionButton
                                                                                          });

                // SaveSelected
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "SaveSelected",
                                                        On_SaveSelected,
                                                        UpdateCommand_SaveSelected));

                Environment.CmdManager.Commands["SaveSelected"].CommandInstances.Add(new Object[]
                                                                                         {
                                                                                             menuItemSaveSelected,
                                                                                             saveSelectedButton
                                                                                         });

                // SaveSelectedAsStamp
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "SaveSelectedAsStamp",
                                                        On_SaveSelectedAsStamp,
                                                        UpdateCommand_SaveSelectedAsStamp));

                Environment.CmdManager.Commands["SaveSelectedAsStamp"].CommandInstances.Add(menuItemSaveSelectedAsStamp);

                // ColumnsSettings
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Columns",
                                                        On_Columns,
                                                        UpdateCommand_Columns));

                Environment.CmdManager.Commands["Columns"].CommandInstances.Add(new Object[] { mIColumns });

                // ResetColumnsSettings
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "ResetColumns",
                                                        On_ResetColumns,
                                                        null));

                // PageBack
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "PageBack",
                                                        On_PageBack,
                                                        UpdateCommand_PageBack));

                Environment.CmdManager.Commands["PageBack"].CommandInstances.Add(new Object[] { menuPageBack, pageBackButton });

                // PageForward
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "PageForward",
                                                        On_PageForward,
                                                        UpdateCommand_PageForward));

                Environment.CmdManager.Commands["PageForward"].CommandInstances.Add(new Object[]
                                                                                        {
                                                                                            menuPageForward,
                                                                                            pageForwardButton
                                                                                        });

                // RotateCW
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "RotateCW",
                                                        On_RotateCW,
                                                        UpdateCommand_RotateCW));

                Environment.CmdManager.Commands["RotateCW"].CommandInstances.Add(new Object[] { menuRotateCW, rotateCWButton });

                // RotateCCW
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "RotateCCW",
                                                        On_RotateCCW,
                                                        UpdateCommand_RotateCCW));

                Environment.CmdManager.Commands["RotateCCW"].CommandInstances.Add(new Object[] { menuRotateCCW, rotateCCWButton });

                // Refresh
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Refresh",
                                                        On_Refresh,
                                                        null));

                Environment.CmdManager.Commands["Refresh"].CommandInstances.Add(new Object[] { menuRefresh, refreshButton });

                // Refresh Docs
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "RefreshDocs",
                                                        On_RefreshDocs,
                                                        null));

                // Refresh Work Folders
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "RefreshWorkFolders",
                                                        On_RefreshWorkFolders,
                                                        null));

                // SendMessage
                if (sendMessageButton != null && !Environment.CmdManager.Commands.Contains("SendMessage"))
                {
                    Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                            "SendMessage",
                                                            On_SendMessage,
                                                            UpdateCommand_SendMessage));

                    Environment.CmdManager.Commands["SendMessage"].CommandInstances.Add(new Object[]
                                                                                            {
                                                                                                menuSendMessage,
                                                                                                sendMessageButton
                                                                                            });
                }
                // SendMessage
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "RefreshInfo",
                                                        On_RefreshInfo,
                                                        UpdateCommand_RefreshInfo));

                // SendFax
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "SendFax",
                                                        On_SendFax,
                                                        UpdateCommand_SendFax));

                Environment.CmdManager.Commands["SendFax"].CommandInstances.Add(new Object[] { menuSendFax, sendFaxButton });

                // Doc Properties
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "DocProperties",
                                                        On_DocProperties,
                                                        UpdateCommand_DocProperties));

                Environment.CmdManager.Commands["DocProperties"].CommandInstances.Add(new Object[]
                                                                                          {
                                                                                              menuDocPropertes,
                                                                                              propertiesButton
                                                                                          });

                // DB Doc Properties
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "DocProperties_DBDoc",
                                                        On_DocProperties_DBDoc_Or_SaveToDB,
                                                        null));

                // Save to DB
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "SaveToDB",
                                                        On_DocProperties_DBDoc_Or_SaveToDB,
                                                        null));

                // Search
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Search",
                                                        On_Search,
                                                        UpdateCommand_Search));

                Environment.CmdManager.Commands["Search"].CommandInstances.Add(new Object[] { menuSearch, buttonSearch });

                // Search
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "FindID",
                                                        On_FindID,
                                                        UpdateCommand_FindID));

                Environment.CmdManager.Commands["FindID"].CommandInstances.Add(new Object[] { menuFindID });

                // Settings Messages
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "SettingsMessages",
                                                        On_SettingsMessages,
                                                        null));

                Environment.CmdManager.Commands["SettingsMessages"].CommandInstances.Add(new Object[] { menuSettingsMessages });

                // Settings Faxes
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "SettingsFaxes",
                                                        On_SettingsFaxes,
                                                        null));

                Environment.CmdManager.Commands["SettingsFaxes"].CommandInstances.Add(new Object[] { menuSettingsFaxes });

                // Settings Mailing Lists
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "SettingsMailingLists",
                                                        On_SettingsMailingLists,
                                                        null));

                Environment.CmdManager.Commands["SettingsMailingLists"].CommandInstances.Add(new Object[]
                                                                                                 {
                                                                                                     menuSettingsMailingLists
                                                                                                 });

                // Settings documents load
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "SettingsDocumentLoad",
                                                        On_SettingsDocumentLoad,
                                                        null));

                Environment.CmdManager.Commands["SettingsDocumentLoad"].CommandInstances.Add(new Object[] { menuSettingsShow });

                // Settings Group Order
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "SettingsGroupOrder",
                                                        On_SettingsGroupOrder,
                                                        UpdateCommand_SettingsGroupOrder));

                Environment.CmdManager.Commands["SettingsGroupOrder"].CommandInstances.Add(new Object[]
                                                                                               {
                                                                                                   menuSettingsGroupOrder,
                                                                                                   settingsGroupOrderButton
                                                                                               });

                // Mark Read Messages
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "MarkReadMessages",
                                                        On_MarkReadMessages,
                                                        null));

                // Select Line
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Line",
                                                        On_Line,
                                                        UpdateCommand_Line));

                Environment.CmdManager.Commands["Line"].CommandInstances.Add(new Object[] { mILine });

                // Select FreeLine
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "FLine",
                                                        On_FLine,
                                                        UpdateCommand_FLine));

                Environment.CmdManager.Commands["FLine"].CommandInstances.Add(new Object[] { mIFLine });

                // Select Highlighter
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Highlighter",
                                                        On_Highlighter,
                                                        UpdateCommand_Highlighter));

                Environment.CmdManager.Commands["Highlighter"].CommandInstances.Add(new Object[]
                                                                                        {
                                                                                            mIHighlighter,
                                                                                            annotationBar.Button("Marker")
                                                                                        });

                // Select Rectangle
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Rectangle",
                                                        On_Rectangle,
                                                        UpdateCommand_Rectangle));

                Environment.CmdManager.Commands["Rectangle"].CommandInstances.Add(new Object[]
                                                                                      {
                                                                                          mIRectangle,
                                                                                          annotationBar.Button("Rectangle")
                                                                                      });

                // Show All Annotation Group
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Show",
                                                        On_Show,
                                                        UpdateCommand_Show));

                Environment.CmdManager.Commands["Show"].CommandInstances.Add(new Object[] { mIShow });

                // Hide All Annotation Group
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Hide",
                                                        On_Hide,
                                                        UpdateCommand_Hide));

                Environment.CmdManager.Commands["Hide"].CommandInstances.Add(new Object[] { mIHide });

                // Show Only  Self Annotation Group
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Self",
                                                        On_Self,
                                                        UpdateCommand_Self));

                Environment.CmdManager.Commands["Self"].CommandInstances.Add(new Object[] { mISelf });

                // Select View
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "View",
                                                        On_View,
                                                        UpdateCommand_View));

                Environment.CmdManager.Commands["View"].CommandInstances.Add(new Object[] { mIView, viewButton });

                // Select HRectangle
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "HRectangle",
                                                        On_HRectangle,
                                                        UpdateCommand_HRectangle));

                Environment.CmdManager.Commands["HRectangle"].CommandInstances.Add(new Object[] { mIHRectangle });

                // Select Text
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Text",
                                                        On_Text,
                                                        UpdateCommand_Text));

                Environment.CmdManager.Commands["Text"].CommandInstances.Add(new Object[] { mIText, annotationBar.Button("Text") });

                // Select Note
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Note",
                                                        On_Note,
                                                        UpdateCommand_Note));

                Environment.CmdManager.Commands["Note"].CommandInstances.Add(new Object[] { mINote, annotationBar.Button("Note") });

                // Select ImageStamp
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "ImageStamp",
                                                        On_Stamp,
                                                        UpdateCommand_Stamp));

                Environment.CmdManager.Commands["ImageStamp"].CommandInstances.Add(new Object[]
                                                                                       {
                                                                                           mIStamp,
                                                                                           annotationBar.Button("ImageStamp")
                                                                                       });

                // Select StampDSP
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "StampDSP",
                                                        On_StampDSP,
                                                        UpdateCommand_StampDSP));

                Environment.CmdManager.Commands["StampDSP"].CommandInstances.Add(new Object[]
                                                                                       {
                                                                                           miDSP,
                                                                                           annotationBar.Button("DSPStamp")
                                                                                       });

                // Select Select Tool
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Select",
                                                        On_Select,
                                                        UpdateCommand_Select));

                Environment.CmdManager.Commands["Select"].CommandInstances.Add(new Object[]
                                                                                   {
                                                                                       mISelect, selectAnnButton,
                                                                                       annotationBar.Button("SelectAnn")
                                                                                   });

                // Select Links
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Links",
                                                        On_Links,
                                                        UpdateCommand_Links));

                Environment.CmdManager.Commands["Links"].CommandInstances.Add(new Object[] { linksButton });

                // GotoCatalog
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "GotoCatalog",
                                                        On_GotoCatalog,
                                                        UpdateCommand_GotoCatalog));

                Environment.CmdManager.Commands["GotoCatalog"].CommandInstances.Add(new Object[]
                                                                                        {
                                                                                            menuGotoCatalog,
                                                                                            gotoFoloderButton
                                                                                        });

                // GotoFound
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "GotoFind",
                                                        On_GotoFound,
                                                        UpdateCommand_GotoFound));

                Environment.CmdManager.Commands["GotoFind"].CommandInstances.Add(menuGotoFind);

                // GotoWorkFolder
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "GotoWorkFolder",
                                                        On_GotoWorkFolder,
                                                        UpdateCommand_GotoWorkFolder));

                Environment.CmdManager.Commands["GotoWorkFolder"].CommandInstances.Add(new Object[]
                                                                                           {
                                                                                               menuGotoWorkFolder,
                                                                                               gotoWorkFolderButton
                                                                                           });

                // GotoScaner
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "GotoScaner",
                                                        On_GotoScaner,
                                                        UpdateCommand_GotoScaner));

                Environment.CmdManager.Commands["GotoScaner"].CommandInstances.Add(new Object[]
                                                                                       {
                                                                                           menuGotoScaner, gotoScanerButton
                                                                                       });

                // GotoFaxIn
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "GotoFaxIn",
                                                        On_GotoFaxIn,
                                                        UpdateCommand_GotoFaxIn));

                Environment.CmdManager.Commands["GotoFaxIn"].CommandInstances.Add(new Object[] { menuGotoFaxIn, gotoFaxInButton });

                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "NewWindow",
                                                        On_NewWindow,
                                                        UpdateCommand_NewWindow));

                Environment.CmdManager.Commands["NewWindow"].CommandInstances.Add(new Object[] { menuNewWindow, windowButton });

                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Return",
                                                        On_Return,
                                                        null));

                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Reprint",
                                                        On_Reprint,
                                                        null));

                // Settings Filter
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "SettingsFilter",
                                                        On_SettingsFilter,
                                                        UpdateCommand_SettingsFilter));

                Environment.CmdManager.Commands["SettingsFilter"].CommandInstances.Add(new Object[]
                                                                                           {
                                                                                               menuSettingsFilter,
                                                                                               settingsFilterButton
                                                                                           });
				Environment.CmdManager.Commands.Add(new CommandManagement.Command(
													  "SettingsFolder",
													  On_SettingsFolder,
													  UpdateCommand_SettingsFolder));

				Environment.CmdManager.Commands["SettingsFolder"].CommandInstances.Add(menuItemFolders);

				// SavePart
				if (savePartButton != null && !Environment.CmdManager.Commands.Contains("SavePart"))
                {
                    Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                            "SavePart",
                                                            On_SavePart,
                                                            UpdateCommand_SavePart));

                    Environment.CmdManager.Commands["SavePart"].CommandInstances.Add(new Object[] { menuSavePart, savePartButton });
                }

                // PageMoveForward
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "PageMoveForward",
                                                        On_PageMoveForward,
                                                        UpdateCommand_PageMoveForward));

                Environment.CmdManager.Commands["PageMoveForward"].CommandInstances.Add(new Object[] { menuPageMoveForward });

                // PageMoveBack
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "PageMoveBack",
                                                        On_PageMoveBack,
                                                        UpdateCommand_PageMoveBack));

                Environment.CmdManager.Commands["PageMoveBack"].CommandInstances.Add(new Object[] { menuPageMoveBack });

                // Fax Description Edit
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "FaxDescriptionEdit",
                                                        On_FaxDescr,
                                                        UpdateCommand_FaxDescr));

                Environment.CmdManager.Commands["FaxDescriptionEdit"].CommandInstances.Add(new Object[] { menuFaxDescr });

                // Fax To Spam
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "FaxToSpam",
                                                        On_FaxToSpam,
                                                        UpdateCommand_FaxToSpam));

                Environment.CmdManager.Commands["FaxToSpam"].CommandInstances.Add(new Object[] { menuSpam });

                // Link Doc
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "LinkDoc",
                                                        On_LinkDoc,
                                                        UpdateCommand_LinkDoc));

                Environment.CmdManager.Commands["LinkDoc"].CommandInstances.Add(new Object[] { menuLinkDoc });

                // Refresh Links
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "RefreshLinks",
                                                        On_RefreshLinks,
                                                        null));

                // AddDocData
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "AddDocData",
                                                        On_AddDocData,
                                                        null));

                Environment.CmdManager.Commands["AddDocData"].CommandInstances.Add(new Object[] { menuAddDocData });

                // Select Person Folder
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "SelectPersonFolder",
                                                        On_SelectPersonFolder,
                                                        null));

                // Add EForm To Doc
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "AddEForm",
                                                        On_AddEForm,
                                                        UpdateCommand_AddEForm));

                Environment.CmdManager.Commands["AddEForm"].CommandInstances.Add(new Object[] { menuAddEForm });

                // Delete Doc
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "DeleteDoc",
                                                        On_DeleteDoc,
                                                        null));

                Environment.CmdManager.Commands["DeleteDoc"].CommandInstances.Add(new Object[] { menuDelete });

                //ScanCurrentDocument
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "ScanCurrentDocument",
                                                        On_ScanCurrentDocument,
                                                        UpdateCommand_ScanCurrentDocument));

                Environment.CmdManager.Commands["ScanCurrentDocument"].CommandInstances.Add(new Object[] { menuScanCurrentDoc });

                //AddImageCurrentDoc
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "AddImageCurrentDoc",
                                                        On_AddImageCurrentDoc,
                                                        UpdateCommand_AddImageCurrentDoc));

                Environment.CmdManager.Commands["AddImageCurrentDoc"].CommandInstances.Add(new Object[] { menuAddImageCurrentDoc });

                // Add To Work
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "AddToWork",
                                                        On_AddToWork,
                                                        UpdateCommand_AddToWork));

                Environment.CmdManager.Commands["AddToWork"].CommandInstances.Add(new Object[] { menuAddToWork });

                // Work Places
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "WorkPlaces",
                                                        On_WorkPlaces,
                                                        UpdateCommand_WorkPlaces));

                Environment.CmdManager.Commands["WorkPlaces"].CommandInstances.Add(new Object[] { menuWorkPlaces });

                // End Work
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "EndWork",
                                                        On_EndWork,
                                                        UpdateCommand_EndWork));

                Environment.CmdManager.Commands["EndWork"].CommandInstances.Add(new Object[] { menuEndWork });

                // Delete From Found
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "DeleteFromFound",
                                                        On_DeleteFromFound,
                                                        UpdateCommand_DeleteFromFound));

                Environment.CmdManager.Commands["DeleteFromFound"].CommandInstances.Add(new Object[] { menuDeleteFromFound });

                // Save
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Exit",
                                                        On_Exit,
                                                        null));

                // Settings Scaner
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "SettingsScaner",
                                                        On_SettingsScaner,
                                                        null));

                Environment.CmdManager.Commands["SettingsScaner"].CommandInstances.Add(new Object[] { menuSettingsScaner });

                // Help
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Help",
                                                        On_Help,
                                                        null));

                Environment.CmdManager.Commands["Help"].CommandInstances.Add(new Object[] { menuHelp });

                //Delete part of scan document
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "DeletePart",
                                                        On_DeletePart,
                                                        UpdateCommand_DeletePart));

                Environment.CmdManager.Commands["DeletePart"].CommandInstances.Add(new Object[] { menuDeletePart });


                // undo command
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Undo",
                                                        On_Undo,
                                                        UpdateCommand_Undo));

                Environment.CmdManager.Commands["Undo"].CommandInstances.Add(new Object[] { undoButton });

                // redo command
                Environment.CmdManager.Commands.Add(new CommandManagement.Command(
                                                        "Redo",
                                                        On_Redo,
                                                        UpdateCommand_Redo));

                Environment.CmdManager.Commands["Redo"].CommandInstances.Add(new Object[] { redoButton });
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        #endregion

        #region Commands

        // Show web panel
        public void On_ShowWeb(CommandManagement.Command cmd)
        {
            bool push = !docControl.ShowWebPanel;
            docControl.ShowWebPanel = push;
            Environment.Layout.Option("ShowWebPanel").Value = push.ToString();
        }

        public void UpdateCommand_ShowWeb(CommandManagement.Command cmd)
        {
            showWebPanelButton.Checked = docControl.HasData;
        }

        public void On_ShowMessage(CommandManagement.Command cmd)
        {
            bool visible = !infoGrid.Visible;
            infoGrid.Visible = visible;
            switch (showMessageState)
            {
                case 0:
                    splitContainerList.Panel1Collapsed = !visible;
                    break;
                case 1:
                    splitContainerGrids.Panel2Collapsed = !visible;
                    break;
                case 2:
                    splitContainerDoc.Panel2Collapsed = !visible;
                    break;
            }
        }

        public void UpdateCommand_ShowMessage(CommandManagement.Command cmd)
        {
            tBBShowMessage.Checked = infoGrid.Visible;
            TS_menuItemBetween.Checked = menuItemBetween.Checked = showMessageState == 0;
            TS_menuItemLeft.Checked = menuItemLeft.Checked = showMessageState == 1;
            TS_menuItemUnder.Checked = menuItemUnder.Checked = showMessageState == 2;
        }

        // Save
        public void On_Save(CommandManagement.Command cmd)
        {
            try
            {
                if (IsSelectedSingle())
                {
                    string realFileName = Lib.Win.Document.Environment.TmpFilesContains(docControl.FileName)
                            ? Lib.Win.Document.Environment.GetTmpFile(docControl.FileName).TmpFullName
                            : docControl.FileName;

                    if (File.Exists(realFileName))
                    {
                        var fi = new FileInfo(realFileName);
                        if (docGrid.IsDBDocs())
                        {
                            if (docControl.ImageDisplayed && docControl.Modified)
                                SaveFile(Lib.Win.Document.Environment.ActionBefore.None);
                            else
                                Environment.CmdManager.Commands["WorkPlaces"].Execute();
                        }
                        else
                        {
                            docControl.TestImage(Lib.Win.Document.Environment.ActionBefore.Save);
							if(Environment.IsConnected)
								Environment.CmdManager.Commands["SaveToDB"].Execute();
                        }
                    }
                    else
                    {
                        if (curDocID > 0)
                            Environment.CmdManager.Commands["WorkPlaces"].Execute();
                    }
                }
                else if (docGrid.IsMultiple && docGrid.IsInWork())
                {
                    Environment.CmdManager.Commands["WorkPlaces"].Execute();
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public void UpdateCommand_Save(CommandManagement.Command cmd)
        {
            bool faxSaved = false;
            if (docGrid.IsFaxes())
                faxSaved = docGrid.GetBoolValue(Environment.FaxData.SavedField);
            if ((curDocID > 0 && !(docControl.ImageDisplayed && docControl.Modified)) || docGrid.IsMultiple)
            {
                if (saveButton.ImageIndex != 43)
                    saveButton.ImageIndex = 43;
                saveButton.ToolTipText = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message5");
            }
            else
            {
                if (saveButton.ImageIndex != 12)
                    saveButton.ImageIndex = 12;
                var resources = new ComponentResourceManager(typeof(MainFormDialog));
                saveButton.ToolTipText = resources.GetString("saveButton.ToolTipText");
            }

            cmd.Enabled = (IsSelectedSingle() && !faxSaved) || (docGrid.IsMultiple && docGrid.IsInWork());
        }

        /// <summary>
        /// Обработка печати
        /// </summary>
        /// <param name="cmd"></param>
        public void On_Print(CommandManagement.Command cmd)
        {
            try
            {
                if (docGrid.IsMultiple)
                    return;
                if (docControl.ImageDisplayed && !(docControl.DocumentID > 0 && docControl.ImageID == 0))
                    docControl.PrintPage();
                else if (docControl.CanPrint)
                    docControl.PrintEForm();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public void UpdateCommand_Print(CommandManagement.Command cmd)
        {
            cmd.Enabled = docGrid.IsSingle && docControl.CanPrint;
            menuPrintAll.Enabled = docGrid.IsSingle && docControl.ImageDisplayed && !(docControl.DocumentID > 0 && (docControl.ImageID == 0 || !docControl.CanSendOut));
        }

        public void On_PrintDoc(CommandManagement.Command cmd)
        {
            try
            {
                if (docControl.DocumentID > 0 && (docControl.CanPrint || docControl.ImageID > 0))
                    docControl.PrintDocument();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public void UpdateCommand_PrintDoc(CommandManagement.Command cmd)
        {
            cmd.Enabled = docGrid.IsSingle && (docControl.DocumentID > 0) && (docControl.CanPrint || docControl.ImageID > 0);
        }

        // Print Folder
        public void On_PrintFolder(CommandManagement.Command cmd)
        {
            try
            {
                if (docGrid.IsFine)
                {
                    var node = folders.SelectedNode as FolderTree.FolderNodes.WorkNodes.WorkNode;
                    if (node != null)
                    {
                        var dialog = new PrintAllDialog(node.ID, node.Emp.ID,
                                                        node.IsWorkFolder()
                                                            ? PrintAllFoldersType.WorkFolder
                                                            : node.IsFound()
                                                                  ? ((node.ID > 0)
                                                                         ? PrintAllFoldersType.InquiryFolder
                                                                         : PrintAllFoldersType.SearchFolder)
                                                                  : PrintAllFoldersType.ArchivFolder);
                        dialog.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public void UpdateCommand_PrintFolder(CommandManagement.Command cmd)
        {
            Node node = folders.SelectedNode;
            cmd.Enabled = (node != null && (node.IsWork() || node.IsFound()));
        }

        // OpenFolder
        public void On_OpenFolder(CommandManagement.Command cmd)
        {
            var dialog = new Dialogs.FolderDialog();
            dialog.ShowDialog();
            if (!string.IsNullOrEmpty(dialog.Path))
                folders.AddSystemFolder(dialog.Path, null);
        }

        // Zoom
        public void On_Zoom(CommandManagement.Command cmd)
        {
            try
            {
                zoomCombo.Focus();
                zoomCombo.DroppedDown = true;
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public void UpdateCommand_Zoom(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle();
            zoomCombo.Enabled = cmd.Enabled;
        }

        // ZoomIn
        public void On_ZoomIn(CommandManagement.Command cmd)
        {
            try
            {
                float zoom = docControl.Zoom * 2;
                float newZoom = (zoom < MaxZoom) ? zoom : MaxZoom;
                if (newZoom != docControl.Zoom)
                    zoomCombo.Text = Convert.ToInt32(newZoom).ToString() + "%";
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public void UpdateCommand_ZoomIn(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle();
        }

        // ZoomOut
        public void On_ZoomOut(CommandManagement.Command cmd)
        {
            var zoom = (float)(docControl.Zoom * 0.5);
            float newZoom = (zoom > MinZoom) ? zoom : MinZoom;
            if (newZoom != docControl.Zoom)
                zoomCombo.Text = Convert.ToInt32(newZoom).ToString() + "%";
        }

        public void UpdateCommand_ZoomOut(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle();
        }

        // Selection
        public void On_Selection(CommandManagement.Command cmd)
        {
            docControl.SelectionMode = true;
            bCheck = ButtonCheck.SelectingPart;
        }

        public void UpdateCommand_Selection(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle();
            cmd.Checked = (bCheck == ButtonCheck.SelectingPart);
        }

        // ZoomSelection
        public void On_ZoomSelection(CommandManagement.Command cmd)
        {
            try
            {
                docControl.ZoomToSelection();
                int newZoom = docControl.Zoom;
                zoomCombo.Text = newZoom.ToString() + "%";
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public void UpdateCommand_ZoomSelection(CommandManagement.Command cmd)
        {
            cmd.Enabled = (docControl.RectDrawn());
            menuPrintSelection.Enabled = cmd.Enabled;
        }

        // SaveSelected
        public void On_SaveSelected(CommandManagement.Command cmd)
        {
            docControl.SaveSelected();
        }

        public void UpdateCommand_SaveSelected(CommandManagement.Command cmd)
        {
            cmd.Enabled = docControl.RectDrawn() && docControl.CanSendOut;
        }

        // SaveSelectedAsStamp
        public void On_SaveSelectedAsStamp(CommandManagement.Command cmd)
        {
            Dialogs.StampEditDialog dlg = new Dialogs.StampEditDialog { StampImage = docControl.GetSelectedRectImage() };
            dlg.Show();
        }

        public void UpdateCommand_SaveSelectedAsStamp(CommandManagement.Command cmd)
        {
            cmd.Enabled = docControl.RectDrawn();
        }

        /// <summary>
        /// Команда вызова диалога настройки колонок
        /// </summary>
        public void On_Columns(CommandManagement.Command cmd)
        {
            if (docGrid.Style != null)
            {
                docGrid.Style.Save();
                Settings.SettingsColumnsDialog dialog = new Settings.SettingsColumnsDialog(docGrid.Style, docGrid.OptionFolder, docGrid.Style);
                dialog.FormClosed += SettingsColumnsDialog_FormClosed;
                dialog.Show();
            }
        }

        /// <summary>
        /// Команда вызова процедуры сброса всех колонок
        /// </summary>
        public void On_ResetColumns(CommandManagement.Command cmd)
        {
            try
            {
                CursorSleep();
				FolderCollection savedStyles = docGrid.OptionFolder.GetSavedFolders();
				foreach(var savedStyle in savedStyles)
					docGrid.OptionFolder.Delete(savedStyle.Name);
				docGrid.OptionFolder.Clear();
				Style.ResetStyles();
				//foreach(Node node in folders.Nodes)
				//    if(node != folders.SelectedNode)
				//    {
				//        docGrid.Style.Reset = true;
				//        node.LoadDocs(docGrid, false);

				//        if(node is FolderTree.FolderNodes.PathNodes.CatalogNodes.CatalogNode)
				//        {
				//            node.LoadSubNodes();
				//            RecursiveNodeLoadDocs(node);
				//        }
				//    }

				docGrid.Style.Reset = true;
                docGrid.Style = null;
                folders.SelectedNode.LoadDocs(docGrid, false, 0, null);
				//if (folders.SelectedNode is FolderTree.FolderNodes.PathNodes.CatalogNodes.CatalogNode)
				//{
				//    folders.SelectedNode.LoadSubNodes();
				//    RecursiveNodeLoadDocs(folders.SelectedNode);
				//}
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
            finally
            {
                CursorWake();
                //Environment.CmdManager.Commands["Columns"].ExecuteIfEnabled();
            }
        }

		private void SettingsColumnsDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			if(e.CloseReason == CloseReason.UserClosing && ((Settings.SettingsColumnsDialog)sender).DialogResult == DialogResult.Retry)
			{
				Environment.CmdManager.Commands["ResetColumns"].Execute();
			}
		}

        private void RecursiveNodeLoadDocs(Node node)
        {
            for (int i = 0; i < node.Nodes.Count && docGrid.Style is Grids.Styles.FormattedStyles.DBDocsStyles.DBDocsStyle && !docGrid.Style.WasInited; i++)
                if (node.Nodes[i] is Node)
                {
                    var subNode = (Node)node.Nodes[i];
                    subNode.LoadDocs(docGrid, false, 0);
                    if (!docGrid.Style.WasInited)
                    {
                        subNode.LoadSubNodes();
                        RecursiveNodeLoadDocs(subNode);
                    }
                }
        }


        public void UpdateCommand_Columns(CommandManagement.Command cmd)
        {
            cmd.Enabled = (docGrid != null && docGrid.Style != null);
        }

        // PageBack
        public void On_PageBack(CommandManagement.Command cmd)
        {
            pageNum.Text = (docControl.Page - 1).ToString();
        }

        public void UpdateCommand_PageBack(CommandManagement.Command cmd)
        {
            bool disp = IsDisplayedSingle();

            cmd.Enabled = disp && (docControl.Page > 1);

            pageNum.Enabled = disp;
            pageNum.ReadOnly = !pageNum.Enabled;
        }

        // PageForward
        public void On_PageForward(CommandManagement.Command cmd)
        {
            pageNum.Text = ((docControl.Page > 0) ? (docControl.Page + 1).ToString() : "1");
        }

        public void UpdateCommand_PageForward(CommandManagement.Command cmd)
        {
            bool disp = IsDisplayedSingle();

            cmd.Enabled = disp && (docControl.Page < docControl.PageCount);

            pageNum.Enabled = disp;
            pageNum.ReadOnly = !pageNum.Enabled;
        }

        // RotateCW
        public void On_RotateCW(CommandManagement.Command cmd)
        {
            try
            {
                if (IsDisplayedSingle())
                {
                    docControl.RotateRight();

                    Func<object[], bool> delegate1 = RotateCCW;
                    Func<object[], bool> delegate2 = RotateCW;

                    var undoText = Environment.StringResources.GetString("MainForm.UNDORotateImageCW");
                    var redoText = Environment.StringResources.GetString("MainForm.REDORotateImageCW");

                    Environment.UndoredoStack.Add("RotateCW", "RotateCW", undoText, redoText, null, new object[] { docControl.DocumentID, delegate1, delegate2 }, 0);
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

		public void UpdateCommand_RotateCW(CommandManagement.Command cmd)
		{
			cmd.Enabled = IsDisplayedSingle();
		}

		// RotateCCW
		public void On_RotateCCW(CommandManagement.Command cmd)
		{
			try
			{
				if(IsDisplayedSingle())
				{
					docControl.RotateLeft();

					Func<object[], bool> delegate1 = RotateCW;
					Func<object[], bool> delegate2 = RotateCCW;

					var undoText = Environment.StringResources.GetString("MainForm.UNDORotateImageCW");
					var redoText = Environment.StringResources.GetString("MainForm.REDORotateImageCW");

					Environment.UndoredoStack.Add("RotateCCW", "RotateCCW", undoText, redoText, null, new object[] { docControl.DocumentID, delegate1, delegate2 }, 0);
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public void UpdateCommand_RotateCCW(CommandManagement.Command cmd)
		{
			cmd.Enabled = IsDisplayedSingle();
		}

        private bool RotateCW(object[] o)
        {
            int docId = (int)o[0];

            if (docId == docControl.DocumentID)
                docControl.RotateRight();

            return true;
        }

        private bool RotateCCW(object[] o)
        {
            int docId = (int)o[0];

            if (docId == docControl.DocumentID)
                docControl.RotateLeft();

            return true;
        }

        // Refresh
        public void On_Refresh(CommandManagement.Command cmd)
        {
            cmd.Enabled = false;
            try
            {
                CursorSleep();
				Lib.Win.Options.Folder root = new Lib.Win.Options.Root();
				if(!Environment.IsConnected)
				{
					Environment.ConnectionStringDocument = root.OptionForced<string>("DS_doc").Value as string;
					Environment.UserSettings.Load();
					Lib.Win.Document.Environment.PersonID = Environment.UserSettings.PersonID;
					Kesco.Lib.Win.Data.Repository.DocumentRepository.Init(Environment.ConnectionStringDocument);
				}
                if (!Environment.IsConnectedBuh)
					Environment.ConnectionStringAccounting = root.OptionForced<string>("DS_buh").Value as string;

                Lib.Win.Document.Environment.ResetTypes();
                RefreshFolders();

                // кнопка отправки факсов/почты
                sendFaxButton.Visible = Environment.IsFaxSender();
                menuSendFax.Visible = sendFaxButton.Visible;
				if(folders.SelectedNode !=null && !((Node)folders.SelectedNode).IsDocument())
				   docGrid.SelectByID(curDocID);
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
                ErrorMessage(Environment.StringResources.GetString("MainForm.MainFormDialog.On_Refresh.Message1") + "\n" + ex.Message);
            }
            finally
            {
                CursorWake();
                cmd.Enabled = true;
            }
        }

        // Refresh Docs
        public void On_RefreshDocs(CommandManagement.Command cmd)
        {
            try
            {
                cmd.Enabled = false;
                RefreshDocs();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
            finally
            {
                cmd.Enabled = true;
            }
        }

        public void On_RefreshWorkFolders(CommandManagement.Command cmd)
        {
            try
            {
                cmd.Enabled = false;
                RefreshWorkFolders();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
            finally
            {
                cmd.Enabled = true;
            }
        }

        // SendMessage
        public void On_SendMessage(CommandManagement.Command cmd)
        {
            try
            {
				if(docGrid.IsDBDocs() && (IsSelectedSingle() || docGrid.IsMultipleSmall)) //&& IsSelectedSingle() && curDocID > 0
                {
                    int[] docIDs = IsSelectedSingle() ? new[] { curDocID } : docGrid.GetCurIDs();
                    if (docIDs[0] <= 0)
                        return;

                    if (docGrid.IsDBDocs() && IsNotRead())
                        Environment.CmdManager.Commands["MarkReadMessages"].Execute();
                    if (docGrid.IsWorkFolder())
                    {
                        Context context = curContext;
                        if (context != null && context.Emp != null)
                            SendMessage(docIDs, context.Emp.ID, context.ID);
                    }
                    else
                        SendMessage(docIDs);
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void SendMessage(int[] docIDs)
        {
            SendMessage(docIDs, Environment.CurEmp.ID, 0);
        }

        private void SendMessage(int[] docIDs, int empID, int folderID)
        {
            try
            {
                var dialog = new SendMessageDialog(docIDs) { EmpID = empID, FolderID = folderID };

                Console.WriteLine("{0}: send message call", DateTime.Now.ToLongTimeString());

                dialog.Show();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public void UpdateCommand_SendMessage(CommandManagement.Command cmd)
        {
            cmd.Enabled = (docGrid.IsDBDocs() && (IsSelectedSingle() || docGrid.IsMultipleSmall)); // && curDocID > 0
        }

        public void On_RefreshInfo(CommandManagement.Command cmd)
        {
            RefreshInfo(true);
        }

        public void UpdateCommand_RefreshInfo(CommandManagement.Command cmd)
        {
            cmd.Enabled = (curDocID > 0);
        }

        // SendFax
        public void On_SendFax(CommandManagement.Command cmd)
        {
			try
			{               
                //SendFax();
				SendFaxNew();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
				ErrorMessage(ex.Message);
			}
        }

        public void UpdateCommand_SendFax(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle() && docControl.CanSendOut || docGrid.IsMultiple;
        }

        private void SendFax()
        {
            int code = -1;
            try
            {
                if ((docGrid.IsScaner() || docGrid.IsDiskImages()) && !Environment.IsFaxSenderWithOutSave())
                {
                    code = -2;
                }
                else if (docGrid.IsSingle)
                {
                    if (docGrid.IsDBDocs() && docControl.CanSendOut)
                    {
                        code = docControl.SendFax();
                    }
                    else if (docGrid.IsFaxes())
                    {
                        if (Environment.IsFaxSenderFolder((folders.SelectedNode as FolderTree.FolderNodes.FaxNodes.FaxNode).ID))
                        {
                            if (docGrid.IsFaxesOut())
                            {
                                var faxOutID = (int)docGrid.GetCurValue(Environment.FaxData.IDField);
                                code = docControl.SendFax(faxOutID);
                            }
                            else
                                code = docControl.SendFax();
                        }
                        else
                            code = 0;
                    }
                    else
                        code = docControl.SendFax();
                }
                else if (docGrid.IsMultiple)
                {
                    if (docGrid.IsDBDocs())
                    {
						var mainImgIDs = docGrid.GetSelectedValues(Environment.DocData.MainImageIDField);
						var IDs = docGrid.GetCurIDs();
						var selectedIDs = new List<int>();
						var selectedImageIDs = new ArrayList();
						for(int i = 0; i < mainImgIDs.Length; i++)
						{
							if(mainImgIDs[i] is int && (int)mainImgIDs[i] > 0)
							{
								selectedIDs.Add(IDs[i]);
								selectedImageIDs.Add(mainImgIDs[i]);
							}
						}
						if(selectedIDs.Count > 0)
						{
							code = Environment.DocData.CheckFaxSenderRule(string.Join(",", selectedIDs.Select(x => x.ToString()).ToArray()));
							if(code == 0)
							{
								if(selectedIDs.Count < IDs.Length)
									code = -3;
								if(selectedIDs.Count > 0)
									new SendFaxDialog(selectedIDs.ToArray(),
													  selectedImageIDs.ToArray());
								if(code > -1)
									code = -1;
							}
							else
								if(code == -1)
								code = 0;
						}
						else
							code = -3; 
					}
                    else
                        if (docGrid.IsFaxes())
                        {
							//if (Environment.IsFaxSenderFolder((folders.SelectedNode as FolderTree.FolderNodes.FaxNodes.FaxNode).ID))
							//{
							//    if (docGrid.IsFaxesOut())
							//    {
							//        new SendFaxDialog(docGrid.GetSelectedValues(Environment.FaxData.FileNameField),
							//                          docGrid.MakeCurDocsString(),
							//                          docGrid.GetSelectedValues(Environment.FaxData.IDField));
							//    }
							//    else if (docGrid.IsFaxes())
							//    {
							//        new SendFaxDialog(docGrid.GetSelectedValues(Environment.FaxData.FileNameField),
							//                          docGrid.MakeCurDocsString());
							//    }
							//}
							//else
                                code = 0;
                        }
                        else if (Environment.IsFaxSender())
                        {
                            if (docGrid.IsScaner())
                            {
                                new SendFaxDialog(docGrid.GetSelectedValues(Environment.ScanReader.FullNameField),
                                                  docGrid.MakeCurDocsString());
                            }
                            else if (docGrid.IsDiskImages())
                            {
                                new SendFaxDialog(docGrid.GetSelectedValues(Environment.ImageReader.FullNameField),
                                                  docGrid.MakeCurDocsString());
                            }
                        }
                        else
                            code = 0;
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }


            switch (code)
            {
                case -1://всё прошло хорошо
                    break;
                case -2://нет прав для отправки не сохраненного документа
                    ErrorShower.OnShowError(this, Environment.StringResources.GetString("MainFormDialog.SendFax.Message_2"), Environment.StringResources.GetString("MainFormDialog.ErrorSend"));
                    break;
				case -3://Документы не имеют изображения
					ErrorShower.OnShowError(this, Environment.StringResources.GetString("MainFormDialog.SendFax.Message_3"), Environment.StringResources.GetString("MainFormDialog.ErrorSend"));
					break;
				case 0://нет прав на отправку факсов
                    ErrorShower.OnShowError(this, Environment.StringResources.GetString("MainFormDialog.SendFax.Message1"), Environment.StringResources.GetString("MainFormDialog.ErrorSend"));
                    break;
                default://нет прав на отправку факсов определённого документа
                    ErrorShower.OnShowError(this, string.Concat(Environment.StringResources.GetString("MainFormDialog.SendFax.Message1"),
                        "\n", (docGrid.IsDBDocs() ? DBDocString.Format(code) : docGrid.Style.MakeDocString(docGrid.GetIndex(code))))
                        , Environment.StringResources.GetString("MainFormDialog.ErrorSend"));
                    break;
            }
        }

		private void SendFaxNew()
		{
			//SendFax();return;
			int code = -1;
			try
			{
				if((docGrid.IsScaner() || docGrid.IsDiskImages()) && !Environment.IsFaxSenderWithOutSave())
				{
					code = -2;
				}
				else if(docGrid.IsSingle)
				{
					if(docGrid.IsDBDocs() && docControl.CanSendOut)
					{
						code = docControl.SendFaxNew();
					}
					else if(docGrid.IsFaxes())
					{
						if(Environment.IsFaxSenderFolder((folders.SelectedNode as FolderTree.FolderNodes.FaxNodes.FaxNode).ID))
						{
							if(docGrid.IsFaxesOut())
							{
								var faxOutID = (int)docGrid.GetCurValue(Environment.FaxData.IDField);
								code = docControl.SendFax(faxOutID);
							}
							else
								code = docControl.SendFaxNew();
						}
						else
							code = 0;
					}
					else
						code = docControl.SendFaxNew();
				}
				else if(docGrid.IsMultiple)
				{
					if(docGrid.IsDBDocs())
					{
						var mainImgIDs = docGrid.GetSelectedValues(Environment.DocData.MainImageIDField);
						var IDs = docGrid.GetCurIDs();
						var selectedIDs = new List<int>();
						var selectedImageIDs = new ArrayList();
						for(int i = 0; i < mainImgIDs.Length; i++)
						{
							if(mainImgIDs[i] is int && (int)mainImgIDs[i] > 0)
							{
								selectedIDs.Add(IDs[i]);
								selectedImageIDs.Add(mainImgIDs[i]);
							}
						}
						code = Environment.DocData.CheckFaxSenderRule(string.Join(",", selectedIDs.Select(x => x.ToString()).ToArray()));
						if(code == 0)
						{
							if(selectedIDs.Count < IDs.Length)
								code = -3;
							SendOutDialog.Send(docGrid.GetCurIDs(), docGrid.GetSelectedValues(Environment.DocData.MainImageIDField));
							code = -1;
						}
						else
							if(code == -1)
							code = 0;
					}
					else
						if(docGrid.IsFaxes())
					{
						if(Environment.IsFaxSenderFolder((folders.SelectedNode as FolderTree.FolderNodes.FaxNodes.FaxNode).ID))
						{
							if(docGrid.IsFaxesOut())
							{
								new SendFaxDialog(docGrid.GetSelectedValues(Environment.FaxData.FileNameField),
												  docGrid.MakeCurDocsString(),
												  docGrid.GetSelectedValues(Environment.FaxData.IDField));
							}
							else if(docGrid.IsFaxes())
							{
								SendOutDialog.Send(docGrid.GetSelectedValues(Environment.FaxData.FileNameField),
												  docGrid.MakeCurDocsString());
							}
						}
						else
							code = 0;
					}
					else if(Environment.IsFaxSender())
					{
						if(docGrid.IsScaner())
						{
							SendOutDialog.Send(docGrid.GetSelectedValues(Environment.ScanReader.FullNameField),
											  docGrid.MakeCurDocsString());
						}
						else if(docGrid.IsDiskImages())
						{
							SendOutDialog.Send(docGrid.GetSelectedValues(Environment.ImageReader.FullNameField),
											  docGrid.MakeCurDocsString());
						}
					}
					else
						code = 0;
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			switch(code)
			{
				case -1://всё прошло хорошо
					break;
				case -2://нет прав для отправки не сохраненного документа
					ErrorShower.OnShowError(this, Environment.StringResources.GetString("MainFormDialog.SendFax.Message_2"), Environment.StringResources.GetString("MainFormDialog.ErrorSend"));
					break;
				case -3://Документы не имеют изображения
					ErrorShower.OnShowError(this, Environment.StringResources.GetString("MainFormDialog.SendFax.Message_3"), Environment.StringResources.GetString("MainFormDialog.ErrorSend"));
					break;
				case 0://нет прав на отправку факсов
					ErrorShower.OnShowError(this, Environment.StringResources.GetString("MainFormDialog.SendFax.Message1"), Environment.StringResources.GetString("MainFormDialog.ErrorSend"));
					break;
				default://нет прав на отправку факсов определённого документа
					ErrorShower.OnShowError(this, string.Concat(Environment.StringResources.GetString("MainFormDialog.SendFax.Message1"),
						"\n", (docGrid.IsDBDocs() ? DBDocString.Format(code) : docGrid.Style.MakeDocString(docGrid.GetIndex(code))))
						, Environment.StringResources.GetString("MainFormDialog.ErrorSend"));
					break;
			}
		}
		

        // Document Properties
		public void On_DocProperties(CommandManagement.Command cmd)
		{
			try
			{
				if(docGrid.IsDBDocs())
					Environment.CmdManager.Commands["DocProperties_DBDoc"].Execute();
				else switch(curContext.Mode)
					{
						case Misc.ContextMode.Document:
							Environment.CmdManager.Commands["DocProperties_DBDoc"].Execute();
							break;

						case Misc.ContextMode.FaxOut:
							var faxOutID = (int)docGrid.GetCurValue(Environment.FaxData.IDField);
							DataRow dr = Environment.FaxOutData.GetFaxOut(faxOutID);
							try
							{
								int status = (short)dr[Environment.FaxOutData.StatusField];

								switch(status)
								{
									case 0:
										PropertiesDialogs.PropertiesFaxDialog faxOutDialog = new PropertiesDialogs.PropertiesFaxDialog();
										PropertiesDialogs.FaxFillClass.Fax0utFillClass(faxOutDialog, faxOutID, dr);
										faxOutDialog.Show();
										break;
									default:
										ErrorMessage(Environment.StringResources.GetString("MainForm.MainFormDialog.On_DocProperties.Message1") +
											dr[Environment.FaxOutData.RecvAddressField] +
											Environment.StringResources.GetString("MainForm.MainFormDialog.On_DocProperties.Message2") +
											dr[Environment.FaxOutData.RecipField] + "'" + System.Environment.NewLine + "'" +
											((Environment.CurCultureInfo.TwoLetterISOLanguageName == "ru")? Environment.FaxErrorMessageData.GetField(Environment.FaxErrorMessageData.NameField, status)
												 : Environment.FaxErrorMessageData.GetField(Environment.FaxErrorMessageData.NameFieldEng, status)) + "'(" + status + ")",
											Environment.StringResources.GetString("MainForm.MainFormDialog.On_DocProperties.Title1"));
										break;
								}
							}
							catch(Exception ex)
							{
								Lib.Win.Data.Env.WriteToLog(ex);
								ErrorMessage( Environment.StringResources.GetString("MainForm.MainFormDialog.On_DocProperties.Message5"),"On_DocProperties()");
							}
							break;

						case Misc.ContextMode.FaxIn:
							if(!string.IsNullOrEmpty(fileName))
							{
								int faxInID = (int)docGrid.GetCurValue(Environment.FaxData.IDField);
								if(faxInID > 0)
								{
									PropertiesDialogs.PropertiesFaxDialog faxInDialog = new PropertiesDialogs.PropertiesFaxDialog();
									PropertiesDialogs.FaxFillClass.FaxInFillClass(faxInDialog, faxInID);
									faxInDialog.Show();
								}
							}
							break;

						case Misc.ContextMode.Scaner:
							if(!string.IsNullOrEmpty(fileName))
							{
								string fullFileName =
									docGrid.GetValue(docGrid.CurrentRowIndex, Environment.ImageReader.FullNameField).
										ToString();
								PropertiesDialogs.PropertiesScanDialog sDialog = new PropertiesDialogs.PropertiesScanDialog(fullFileName);
								sDialog.DialogEvent += PropertiesScanDialog_DialogEvent;
								sDialog.Show();
							}
							break;

						default:
							if(!string.IsNullOrEmpty(fileName))
								docControl.ShowProperties();
							break;
					}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

        private void PropertiesScanDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            try
            {
                Focus();
                if (e.Dialog.DialogResult == DialogResult.OK)
                    Environment.RefreshDocs();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public void UpdateCommand_DocProperties(CommandManagement.Command cmd)
        {
            Node node = folders.SelectedNode;
            cmd.Enabled = (node != null && IsSelectedSingle() &&
                (!string.IsNullOrEmpty(fileName) || curDocID > 0));
        }

        // DB Document Properties or Saving to DB
        public void On_DocProperties_DBDoc_Or_SaveToDB(CommandManagement.Command cmd)
        {
            try
            {
                if (curDocID == 0)
                {
                    //docControl.CurDocString = docGrid.DocStringToSave();
                    docControl.Save();
                }
                else
                {
                    PropertiesDialogs.PropertiesDBDocDialog dialog = new PropertiesDialogs.PropertiesDBDocDialog(curDocID);
                    dialog.DialogEvent += PropertiesDBDocDialog_DialogEvent;
                    dialog.Show();
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void PropertiesDBDocDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            try
            {
				Lib.Win.FreeDialog dial = e.Dialog as Lib.Win.FreeDialog;
				if(dial != null)
					dial.DialogEvent -= PropertiesDBDocDialog_DialogEvent;
                if (e.Dialog.DialogResult == DialogResult.OK)
                {
                    if (docGrid.IsWorkFolder())
                        RefreshDocs();
                    else
                        UpdateWorkContext();
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

		// Search
		public void On_Search(CommandManagement.Command cmd)
		{
			//docControl.TestImage(Lib.Win.Document.Environment.ActionBefore.None);
			try
			{
				var dialog = new XmlSearchForm(0, OptionsDialog.EnabledFeatures.All);
				dialog.DialogEvent += SearchDialog_DialogEvent;
				dialog.Show();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

        // FindID
        public void On_FindID(CommandManagement.Command cmd)
        {
            try
            {
                var doc = new XmlDocument();
                XmlElement elOptions = doc.CreateElement("Options");
                XmlElement elOption = doc.CreateElement("Option");
                elOption.SetAttribute("name", "КодДокумента");
                elOption.SetAttribute("open", "true");
                elOptions.AppendChild(elOption);
                var dialog = new XmlSearchForm(elOptions.OuterXml, OptionsDialog.EnabledFeatures.All);
                dialog.DialogEvent += SearchDialog_DialogEvent;
                dialog.Show();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

		public void SearchDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
		{
			try
			{
				var dialog = e.Dialog as OptionsDialog;
				if(dialog != null)
					switch(dialog.DialogResult)
					{
						case DialogResult.OK:
							CursorSleep();
							Environment.SearchXml = dialog.GetXML();
							if(folders.SelectedNode != folders.FoundNode)
								folders.SelectedNode = folders.FoundNode; // не выбран - выбираем
							if(Environment.DocData.FoundDocsCount() > 0) // есть найденные документы
							{
								folders.SelectedNode.LoadDocs(docGrid, true, 0); // выбран - обновляем
							}
							else
							{
								var ynDialog =
									new Lib.Win.MessageForm(Environment.StringResources.GetString("Search.NotFound.Message1")
													+ System.Environment.NewLine + System.Environment.NewLine +
													Environment.StringResources.GetString("Search.NotFound.Message2"),
													Environment.StringResources.GetString("Search.NotFound.Title"),
													MessageBoxButtons.YesNo) { Tag = Environment.SearchXml };
								ynDialog.DialogEvent += NotFoundDialog_DialogEvent;
								ynDialog.Show();
							}
							CursorWake();
							break;
						case DialogResult.Yes:
							{
								CursorSleep();
								int id = dialog.GetID();
								if(id > 0)
								{
									RefreshFolders();
									folders.SelectFoundFolder(id, folders.FoundNode, 0);
								}
								CursorWake();
							}
							break;
					}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

        private void NotFoundDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            var dialog = e.Dialog as Lib.Win.MessageForm;
            if (dialog != null && dialog.DialogResult == DialogResult.Yes)
            {
                var searchDialog = new XmlSearchForm(dialog.Tag as string, OptionsDialog.EnabledFeatures.All);
                searchDialog.DialogEvent += SearchDialog_DialogEvent;
                searchDialog.Show();
            }
        }

        public void UpdateCommand_Search(CommandManagement.Command cmd)
        {
            cmd.Enabled = Environment.IsConnected;
        }

        public void UpdateCommand_FindID(CommandManagement.Command cmd)
        {
            cmd.Enabled = Environment.IsConnected;
        }

        public void UpdateCommand_SettingsGroupOrder(CommandManagement.Command cmd)
        {
            cmd.Enabled = Environment.IsConnected;
        }

        // Settings Messages
        public void On_SettingsMessages(CommandManagement.Command cmd)
        {
            Settings.SettingsMessagesAndConfirmsDialog dialog = new Settings.SettingsMessagesAndConfirmsDialog();
            dialog.DialogEvent += SettingsMessagesAndConfirmsDialog_DialogEvent;
            dialog.Show();
        }

        private void SettingsMessagesAndConfirmsDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            switch (e.Dialog.DialogResult)
            {
                case DialogResult.OK:
                    Environment.UserSettings.Save();
                    break;
                case DialogResult.Yes:
                    {
                        Environment.UserSettings.Save();
                        var mf = new Lib.Win.MessageForm(Environment.StringResources.GetString("SettingsMessagesAndConfirmsDialog.buttonOk_Click.Message4"), Environment.StringResources.GetString("Warning"), MessageBoxButtons.YesNo);
                        mf.DialogEvent += mf_DialogEvent;
                        mf.Show();
                    }
                    break;
				case DialogResult.Retry:
					Environment.UserSettings.Save();
					RefreshFolders();
					break;
            }
        }

        // Settings Faxes
        public void On_SettingsFaxes(CommandManagement.Command cmd)
        {
            Settings.SettingsFaxesDisplayDialog dialog = new Settings.SettingsFaxesDisplayDialog();
            dialog.DialogEvent += SettingsFaxesDisplayDialog_DialogEvent;
            dialog.Show();
        }

        private void SettingsFaxesDisplayDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            if (e == null || e.Dialog == null || e.Dialog.DialogResult != DialogResult.OK)
                return;
            try
            {
                if (docGrid.IsFaxes())
                {
                    string column = Environment.FaxData.SavedField;
                    // updating faxes in column style
                    Style style = Style.CreateFaxesInStyle(docGrid);
                    style.Grid.Columns[column].Visible = !Environment.UserSettings.FaxesInUnsavedOnly;

                    // updating faxes out column style
                    style = Style.CreateFaxesOutStyle(docGrid);
                    style.Grid.Columns[column].Visible = !Environment.UserSettings.FaxesOutUnsavedOnly;
                }
                Environment.UserSettings.Save();
                if (docGrid.IsFaxes())
                    RefreshDocs();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        // Settings Mailing Lists
        public void On_SettingsMailingLists(CommandManagement.Command cmd)
        {
            ShowMailingList();
        }

        private void ShowMailingList()
        {
            if (mlmDialog == null)
            {
                mlmDialog = new MailingListManageDialog();
                mlmDialog.FormClosed += mlmDialog_FormClosed;
                mlmDialog.Show();
            }
            else
            {
                mlmDialog.Activate();
            }
        }

        void mlmDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            mlmDialog.FormClosed -= mlmDialog_FormClosed;
            mlmDialog = null;
        }

        public void On_SettingsDocumentLoad(CommandManagement.Command cmd)
        {
            Settings.SettingsDocumentLoadDialog dialog = new Settings.SettingsDocumentLoadDialog();
            dialog.DialogEvent += settingsDocumentLoadDialog_DialogEvent;
            dialog.Show();
        }

        // Settings Group Order
        public void On_SettingsGroupOrder(CommandManagement.Command cmd)
        {
            Settings.SettingsGroupOrderDialog dialog = new Settings.SettingsGroupOrderDialog();
            dialog.DialogEvent += SettingsDialog_DialogEvent;
            dialog.Show();
        }

        private void SettingsDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            if (e.Dialog.DialogResult == DialogResult.OK)
            {
                Environment.UserSettings.Save();
                RefreshFolders();
            }
        }

        // Mark Read Messages
		public void On_MarkReadMessages(CommandManagement.Command cmd)
		{
			if(!docGrid.IsFine)
				return;

			docGrid.Cursor = Cursors.AppStarting;
			try
			{
				bool isNotRead = IsNotRead();

				switch(curContext.Mode)
				{
					case Misc.ContextMode.FaxIn:
						if((docGrid.IsSingle &&
							 Environment.FaxData.SetField(Environment.FaxData.ReadField, SqlDbType.Bit, curFaxID, isNotRead)) ||
							(docGrid.IsMultiple && Environment.FaxData.SetField(Environment.FaxData.ReadField, SqlDbType.Bit,
														  string.Join(",", docGrid.GetCurIDs().Select(id => id.ToString()).ToArray()), isNotRead)))

							docGrid.SetSelectedValues(Environment.FaxData.ReadField, isNotRead);

						folders.UpdateFaxStatus();
						return;
					case Misc.ContextMode.Catalog:
					case Misc.ContextMode.WorkFolder:
					case Misc.ContextMode.SharedWorkFolder:
					case Misc.ContextMode.Found:
					case Misc.ContextMode.Document:
						if(docMessageMarker == null)
						{
							docMessageMarker = new BackgroundWorker();
							docMessageMarker.DoWork += docMessageMarker_DoWork;
						}

						int[] docIDs = IsSelectedSingle() ? new[] { curDocID } : docGrid.GetCurIDs();
						if(docIDs[0] <= 0)
							return;

						string[] docStrings = docGrid.MakeCurDBDocsStrings();

						_markDocMessagesArgs.Add(new object[] { docIDs, docStrings, isNotRead });

						if(!docMessageMarker.IsBusy)
							docMessageMarker.RunWorkerAsync();
						return;
				}

			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			finally
			{
				docGrid.Cursor = Cursors.Default;
			}
		}

        private void docMessageMarker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (_markDocMessagesArgs.Count > 0)
            {
                int[] docIDs = (int[])_markDocMessagesArgs[0][0];
                string[] docStrings = (string[])_markDocMessagesArgs[0][1];
                bool isNotRead = (bool)_markDocMessagesArgs[0][2];

                _markDocMessagesArgs.RemoveAt(0);

                foreach (int empID in Environment.EmpData.GetCurrentEmployeeIDs())
                {
                    for (int i = 0; i < docIDs.Length; i++)
                    {
                        int minMessID;
                        int maxMessID;
                        if (Environment.WorkDocData.GetDocMessagesIDs(empID, docIDs[i], isNotRead, out minMessID, out maxMessID) &&
                            Environment.WorkDocData.MarkAsRead(empID, (isNotRead ? minMessID : maxMessID), maxMessID, isNotRead))
                        {
                            if (isNotRead)
                                Environment.UndoredoStack.Add("MarkDocAsRead",
                                                              Environment.StringResources.GetString("MarkDocAsRead"),
                                                              string.Concat(Environment.StringResources.GetString("UndoMarkDocAsRead"), docStrings[i]),
                                                              string.Concat(Environment.StringResources.GetString("RedoMarkDocAsRead"), docStrings[i]),
                                                              null, new object[] { empID, minMessID, maxMessID }, empID);
                            else
                                Environment.UndoredoStack.Add("MarkDocAsNotRead",
                                                              Environment.StringResources.GetString("MarkDocAsNotRead"),
                                                              string.Concat(Environment.StringResources.GetString("UndoMarkDocAsNotRead"), docStrings[i]),
                                                              string.Concat(Environment.StringResources.GetString("RedoMarkDocAsNotRead"), docStrings[i]),
                                                              null, new object[] { empID, minMessID, maxMessID }, empID);
                        }
                    }
                    if (docIDs.Length == 1 && isNotRead)
                        Environment.RemoveMessageWindow(docIDs[0], empID, true);
                }
            }
        }

        public void UpdateCommand_Line(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle() && IsNotSigned();
        }

        public void UpdateCommand_FLine(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle() && IsNotSigned();
        }

        public void UpdateCommand_Highlighter(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle() && IsNotSigned() && !docControl.IsPDFMode;
            cmd.Checked = (bCheck == ButtonCheck.Marker);
        }

        public void UpdateCommand_Rectangle(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle() && IsNotSigned() && !docControl.IsPDFMode;
            cmd.Checked = (bCheck == ButtonCheck.Rectangle);
        }

        public void UpdateCommand_Select(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle() && docControl.HasAnnotation();
            cmd.Checked = (bCheck == ButtonCheck.SelectAnn);
        }

        public void UpdateCommand_Note(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle() && IsNotSigned() && !docControl.IsPDFMode;
            cmd.Checked = (bCheck == ButtonCheck.Note);
        }

        public void UpdateCommand_Stamp(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle() && curDocID > 0;
            cmd.Checked = (bCheck == ButtonCheck.ImageStamp) && docControl.CurrentStampID > -1;
        }

        public void UpdateCommand_StampDSP(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle() && curDocID > 0 && !docControl.IsSignInternal;
            cmd.Checked = (bCheck == ButtonCheck.ImageStamp) && docControl.CurrentStampID < 0;
        }

        public void UpdateCommand_HRectangle(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle() && IsNotSigned();
        }

        public void UpdateCommand_Text(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle() && IsNotSigned() && !docControl.IsPDFMode;
            cmd.Checked = (bCheck == ButtonCheck.Text);
        }

        public void UpdateCommand_View(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle();
            cmd.Checked = (bCheck == ButtonCheck.MoveImg);
        }

        public void UpdateCommand_Hide(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle() && (docControl.AnnotationGroupCount > 0);
        }

        public void UpdateCommand_Show(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle() && (docControl.AnnotationGroupCount > 0);
        }

        public void UpdateCommand_Self(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle() && (docControl.AnnotationGroupCount > 0);
        }

        public void On_Links(CommandManagement.Command cmd)
        {
            try
            {
                Rectangle rec = RectangleToScreen(linksButton.Bounds);
                Console.WriteLine("{0}: {1}", DateTime.Now.ToString("HH:mm:ss fff"), rec);
                Dialogs.MenuList ml = new Dialogs.MenuList(curDocID, rec, zoomCombo.Text);
                ml.DialogEvent += ml_DialogEvent;
                ml.Owner = this;
                ml.Show();

            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public void UpdateCommand_Links(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsSelectedSingle() && (curDocID > 0) && docControl.HasDocLinks;
        }

        public void On_Return(CommandManagement.Command cmd)
        {
            if (Disposing || IsDisposed)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new CommandManagement.Command.ExecuteHandler(On_Return), cmd);
                return;
            }
            // достаем окно из трея
            if (!toTray)
                GetFromTray();

            if (returnContext != null && ChoosePathAndDoc(returnContext, returnID, returnFileName))
                return;
            if (returnID > 0)
            {
                if (string.IsNullOrEmpty(returnPath))
                {
                    SelectWorkOrDBDoc(returnID);
                }
                else
                    SelectDBDoc(returnID, returnPath);
                docControl.ForceRelicate = returnForce;
            }
            else
            {
                try
                {
                    if (AnalyzeArgsFileName(returnFileName))
                        LoadFolders();
                }
                catch (Exception ex)
                {
                    Lib.Win.Data.Env.WriteToLog(ex);
                }
            }
        }

        public void On_Reprint(CommandManagement.Command cmd)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new CommandManagement.Command.ExecuteHandler(On_Reprint), cmd);
                return;
            }
			float ver = 0;
            object[] args = null;
            var details = new StringBuilder();
			try
			{
				details.Append(" Printer name: " + Lib.Win.Document.Environment.PrinterName);
				ver = Lib.Win.Document.Environment.PrinterVersionAsync;
				details.Append(", Driver version: " + ver.ToString());
				if(Environment.ReprintArgs != null)
            {
					if(Environment.ReprintArgs.Length > 2)
                {
                    args = Environment.ReprintArgs;
                    details.Append(", imageID: " + args[0] + ", fName: " + args[1] + ", isPDFMode: " + args[2] + ", startPage: " + args[3] + ", endPage: " + args[4] + ", countPage: " + args[5] + ", docName: " + args[6]);
                }
                else
                {
                    details.Append(", filename: " + Environment.ReprintArgs[0]);
                }
                Environment.ReprintArgs = null;
            }
			}
			catch(Exception ex)
			{
			}

			if(ver < (float)6.1)
            {
				Lib.Win.Data.Env.WriteExtExToLog("Driver version is incorrect", "UDC printer driver is too old!" + details, 
					Assembly.GetExecutingAssembly().GetName(), MethodBase.GetCurrentMethod());

                MessageBox.Show(Environment.StringResources.GetString("VerifyUDCPrinter.ImpossibleToSave"),
                            Environment.StringResources.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
				//if(MessageBox.Show(Environment.StringResources.GetString("VerifyUDCPrinter.AutoTunePrinter.YesNo"),
				//    Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
				//{
                    Environment.SetPrinterProfileAsync();
					//Lib.Log.Logger.WriteEx(new Lib.Log.LogicalException("UDC printer auto setup was performed!", details.ToString(),
					//        Assembly.GetExecutingAssembly().GetName(), MethodBase.GetCurrentMethod().Name, Lib.Log.Priority.Info));
					//MessageBox.Show(Environment.StringResources.GetString("VerifyUDCPrinter.AutoTunePrinter.Success"));

					if(args != null)
                    {
                        DocPrintDialog dialog = new DocPrintDialog(0, (int)args[0], args[1].ToString(), (bool)args[2], (int)args[3], (int)args[4], (int)args[5], Environment.Settings.Folders.Add("Print"), args[6].ToString());
                        dialog.DialogEvent += Environment.PrintOnOk;
                        dialog.Show();
                    }
                    else
                    {
                        MessageBox.Show(Environment.StringResources.GetString("VerifyUDCPrinter.SendDocToPrinterAgain"));
                    }
				//}
				//else
				//{
				//    Lib.Log.Logger.WriteEx(new Lib.Log.LogicalException("User refused to perform auto setup of UDC printer",
				//        details.ToString(), Assembly.GetExecutingAssembly().GetName(), MethodBase.GetCurrentMethod().Name,
				//        Lib.Log.Priority.ExternalError));

				//    MessageBox.Show(Environment.StringResources.GetString("VerifyUDCPrinter.ImpossibleToSave"),
				//                Environment.StringResources.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//}
            }
        }

        // GotoCatalog
        public void On_GotoCatalog(CommandManagement.Command cmd)
        {
            folders.SelectedNode = folders.CatalogNode;
        }

        public void UpdateCommand_GotoCatalog(CommandManagement.Command cmd)
        {
            bool enabled = (folders.CatalogNode != null);

            if (folders.SelectedNode != null)
                enabled = enabled && !folders.SelectedNode.IsCatalog();

            cmd.Enabled = enabled;
        }

        // GotoFound
        public void On_GotoFound(CommandManagement.Command cmd)
        {
            folders.SelectedNode = folders.FoundNode;
        }

        public void UpdateCommand_GotoFound(CommandManagement.Command cmd)
        {
            cmd.Enabled = (
                folders.FoundNode != null &&
                folders.SelectedNode != folders.FoundNode);
        }

        // GotoWorkFolder
        public void On_GotoWorkFolder(CommandManagement.Command cmd)
        {
            folders.SelectedNode = folders.WorkFolderNode;
        }

        public void UpdateCommand_GotoWorkFolder(CommandManagement.Command cmd)
        {
            bool enabled = (folders.WorkFolderNode != null);

            if (folders.SelectedNode != null)
                enabled = enabled && !folders.SelectedNode.IsWorkFolder();

            cmd.Enabled = enabled;
        }

        // GotoScaner
        public void On_GotoScaner(CommandManagement.Command cmd)
        {
            folders.SelectedNode = folders.ScanerNode;
        }

        public void UpdateCommand_GotoScaner(CommandManagement.Command cmd)
        {
            bool enabled = (folders.ScanerNode != null);

            if (folders.SelectedNode != null)
                enabled = enabled && !folders.SelectedNode.IsScaner();

            cmd.Enabled = enabled;
        }

        // GotoFaxIn
        public void On_GotoFaxIn(CommandManagement.Command cmd)
        {
            folders.SelectedNode = folders.FaxInNode;
        }

        public void UpdateCommand_GotoFaxIn(CommandManagement.Command cmd)
        {
            cmd.Enabled = (
                folders.FaxInNode != null &&
                folders.SelectedNode != folders.FaxInNode);
        }

        public void On_NewWindow(CommandManagement.Command cmd)
        {
            docControl.TestImage(Lib.Win.Document.Environment.ActionBefore.None);

            CursorSleep();

            Context context = curContext;
            if (context == null)
                return;

            Environment.SaveActiveForm();

            if (docGrid.IsDBDocs())
            {
                var node = folders.SelectedNode as FolderTree.FolderNodes.PathNodes.PathNode;

                if (IsSelectedSingle() && curDocID > 0)
                {
                    var parameters = new Common.ViewParameters
                    {
                        IsPdf = docControl.IsPDFMode,
                        Page = docControl.Page,
                        ActualImageHorizontalScrollValue = docControl.ActualImageHorisontalScrollValue,
                        ActualImageVerticalScrollValue = docControl.ActualImageVerticalScrollValue,
                        ScrollPositionX = docControl.ScrollPositionX,
                        ScrollPositionY = docControl.ScrollPositionY
                    };

                    Environment.NewWindow(curDocID, IsDisplayedSingle() ? zoomCombo.Text : Environment.ZoomString, parameters, context, curImageID, docControl.Page, (node != null) ? node.Path : null);
                }
                else
                    foreach (int selectedDocID in docGrid.GetCurIDs())
                        Environment.NewWindow(selectedDocID, Environment.ZoomString, context);
            }
            else
            {
                if (IsSelectedSingle() && curDocID > 0)
                    Environment.NewWindow(fileName, zoomCombo.Text, curDocString, context, docControl.Page);
                else
                {
                    string fieldName = string.Empty;

					//if (docGrid.IsFaxes())
					//    fieldName = Environment.FaxData.FileNameField;
                    if (docGrid.IsScaner())
                        fieldName = Environment.ScanReader.FullNameField;
                    if (docGrid.IsDiskImages())
                        fieldName = Environment.ImageReader.FullNameField;

                    if (!string.IsNullOrEmpty(fieldName))
                        for (int i = 0; i < docGrid.SelectedRows.Count; i++)
                            Environment.NewWindow(docGrid.SelectedRows[i].Cells[fieldName].Value.ToString(), zoomCombo.Text,
                                                  docGrid.Style.MakeDocString(docGrid.SelectedRows[i].Index), context, 1);
                }
            }
            CursorWake();
        }

        public void UpdateCommand_NewWindow(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsSelectedSingle() || docGrid.IsMultiple;
        }

        // Settings Filter
        public void On_SettingsFilter(CommandManagement.Command cmd)
        {
            Settings.SettingsDateFiltersDialog dialog = new Settings.SettingsDateFiltersDialog();
            dialog.DialogEvent += SettingsDialog_DialogEvent;
            dialog.Show();
        }

        public void UpdateCommand_SettingsFilter(CommandManagement.Command cmd)
        {
            ToolStripButton button = settingsFilterButton;
            if (button == null)
                return;
            bool push = Environment.IsConnected && (Environment.UserSettings.FilterArchiveDate +
                         Environment.UserSettings.FilterDocDate > 0);
            if (push != button.Checked)
                button.Checked = push;
        }

		public void  On_SettingsFolder(CommandManagement.Command cmd)
		{
			Settings.SettingsAdditionDialog dialog = new Settings.SettingsAdditionDialog();
            dialog.DialogEvent += SettingsDialog_DialogEvent;
            dialog.Show();
		}

		public void UpdateCommand_SettingsFolder(CommandManagement.Command cmd)
		{
		}

		// SavePart
		public void On_SavePart(CommandManagement.Command cmd)
		{
			if(!IsDisplayedSingle())
				return;
			//docControl.CurDocString = docGrid.DocStringToSave();
			docControl.TestImage(Lib.Win.Document.Environment.ActionBefore.SavePart);
			docControl.SavePart();
		}

        public void UpdateCommand_SavePart(CommandManagement.Command cmd)
        {
            cmd.Enabled = Environment.IsConnected && !docGrid.IsFaxesIn() && IsDisplayedSingle() && (docControl.PageCount > 1) && IsNotSigned() && docControl.CanSendOut;
        }

        // PageMoveForward
		public void On_PageMoveForward(CommandManagement.Command cmd)
		{
			if(!IsDisplayedSingle() || docControl.Page >= docControl.PageCount)
				return;
			docControl.MovePage(docControl.Page);
			//pageNum.Text = (docControl.Page + 1).ToString();
		}

		public void UpdateCommand_PageMoveForward(CommandManagement.Command cmd)
		{
			if(docControl.ImageDisplayed)
				cmd.Enabled = (IsDisplayedSingle() && (docControl.Page < docControl.PageCount)) && IsNotSigned();
		}

        // PageMoveBack
		public void On_PageMoveBack(CommandManagement.Command cmd)
		{
			if(!IsDisplayedSingle() || docControl.Page <= 1)
				return;
			docControl.MovePage(docControl.Page - 1);
			//pageNum.Text = (docControl.Page - 1).ToString();
		}

		public void UpdateCommand_PageMoveBack(CommandManagement.Command cmd)
		{
			if(docControl.ImageDisplayed)
				cmd.Enabled = (IsDisplayedSingle() && (docControl.Page > 1)) && IsNotSigned();
		}

        // Fax Description Edit
        public void On_FaxDescr(CommandManagement.Command cmd)
        {
            if (curFaxID <= 0)
                return;

            string descr = docGrid.GetCurValue(Environment.FaxInData.DescriptionField) as string ?? string.Empty;
            var dialog =
                new EnterStringDialog(Environment.StringResources.GetString("MainForm.MainFormDialog.On_FaxDescr.Message1"), Environment.StringResources.GetString("MainForm.MainFormDialog.On_FaxDescr.Title1"), descr, true, curFaxID);
            dialog.DialogEvent += FaxDescrDialog_DialogEvent;
            dialog.Show();
        }

        private void FaxDescrDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            var dialog = e.Dialog as EnterStringDialog;
            if (dialog == null || dialog.DialogResult != DialogResult.OK)
                return;
            if (dialog.Data is int)
            {
                var id = (int)dialog.Data;
                if (Environment.FaxInData.SetField(
                    Environment.FaxInData.DescriptionField, SqlDbType.NVarChar, id, dialog.Input))
                    Environment.RefreshDocs();
            }
        }

        public void UpdateCommand_FaxDescr(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsSelectedSingle() && docGrid.IsFaxesIn();
        }

        // Fax To Spam
        public void On_FaxToSpam(CommandManagement.Command cmd)
        {
            Environment.FaxData.MarkSpam(curFaxID, !docGrid.IsSpam());
        }

        public void UpdateCommand_FaxToSpam(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsSelectedSingle() && docGrid.IsFaxesIn();
        }

        // Link Doc
        public void On_LinkDoc(CommandManagement.Command cmd)
        {
            LinkDoc(curDocID);
        }

        /// <summary>
        /// Связать документ
        /// </summary>
        /// <param name="docId"></param>
        private void LinkDoc(int docID)
        {
            try
            {
                var dialog = new XmlSearchForm(docID, true);
                dialog.DialogEvent += SearchAndLink_DialogEvent;
                dialog.Show();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        /// <summary>
        /// Обработчик события DialogEvent диалога поиска и создания связи
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void SearchAndLink_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            if (e.Dialog.DialogResult != DialogResult.OK)
                return;
        
            var dialog = e.Dialog as XmlSearchForm;
            if (dialog == null)
                return;
            dialog.DialogEvent -= SearchAndLink_DialogEvent;
            try
            {
                string xmlString = dialog.GetXML();
                string sqlString = Options.GetSQL(xmlString);
                if (Environment.DocData.GetDocCount("(" + sqlString + ") SearchAndLink_DialogEvent") > 0)
                {
                    var uniDialog = new SelectDocUniversalDialog(Environment.DocData.GetFoundDocsIDQuery(sqlString, false), dialog.DocID, xmlString, true);
                    uniDialog.ResultEvent += uniDialog_ResultEvent;
                    uniDialog.Show();
                }
                else
                {
                    var ynDialog = new Lib.Win.MessageForm(Environment.StringResources.GetString("Search.NotFound.Message1") + System.Environment.NewLine + System.Environment.NewLine +
                       Environment.StringResources.GetString("Search.NotFound.Message2"),
                       Environment.StringResources.GetString("Search.NotFound.Title"),
                       MessageBoxButtons.YesNo, dialog.DocID) { Tag = xmlString };

                    ynDialog.DialogEvent += SearchAndLinkNotFoundDialog_DialogEvent;
                    ynDialog.Show();
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        /// <summary>
        /// Обработчик события DialogEvent Да Нет диалога функционала поиска и создания связи
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void SearchAndLinkNotFoundDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
			try
			{
				var dialog = e.Dialog as Lib.Win.MessageForm;

				if(dialog != null)
				{
					dialog.DialogEvent -= SearchAndLinkNotFoundDialog_DialogEvent;
					if(dialog.DialogResult == DialogResult.Yes)
					{
						var searchDialog = new XmlSearchForm(dialog.DocID, true, dialog.Tag as string);
						searchDialog.DialogEvent += SearchAndLink_DialogEvent;
						searchDialog.Show();
					}
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
        }

        private void SearchAndSelect_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            if (e.Dialog.DialogResult != DialogResult.OK)
                return;
            var dialog = e.Dialog as XmlSearchForm;
            if (dialog == null)
                return;
            if (Environment.DocData.FoundDocsCount() > 0)					// есть найденные документы
            {
                // select doc
                var uniDialog = new SelectDocUniversalDialog(Environment.DocData.GetFoundDocsIDQuery(Environment.CurEmp.ID), dialog.DocID, dialog.GetXML(), true);
                uniDialog.ResultEvent += uniDialog_ResultEvent;
                uniDialog.Show();
            }
            else
            {
                if (folders.SelectedNode == folders.FoundNode)
                    Environment.RefreshDocs();					// выбран - обновляем

                var ynDialog = new Lib.Win.MessageForm(Environment.StringResources.GetString("Search.NotFound.Message1")
                                               + System.Environment.NewLine + System.Environment.NewLine +
                                               Environment.StringResources.GetString("Search.NotFound.Message2"),
                                               Environment.StringResources.GetString("Search.NotFound.Title"),
                                               MessageBoxButtons.YesNo, dialog.DocID) { Tag = dialog.GetXML() };
                ynDialog.DialogEvent += SearchAndSelect_NotFoundDialog_DialogEvent;
                ynDialog.Show();
            }
        }

        private void SearchAndSelect_NotFoundDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            var dialog = e.Dialog as Lib.Win.MessageForm;
            if (dialog != null && dialog.DialogResult == DialogResult.Yes)
            {
                var searchDialog = new XmlSearchForm(dialog.Tag as string) { DocID = dialog.DocID };
                searchDialog.DialogEvent += SearchAndSelect_DialogEvent;
                searchDialog.Show();
            }
        }

        private void PerMessageForm_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            var form = e.Dialog as Lib.Win.MessageForm;
            if (form != null && form.DialogResult == DialogResult.Yes)
            {
                Environment.UserSettings.PersonID = (int)form.Tag;
                Environment.UserSettings.Save();
                Lib.Win.Document.Environment.PersonID = Environment.UserSettings.PersonID;
                Environment.CompanyName = null;
                Environment.CmdManager.Commands["Refresh"].Execute();
                RealSelectDBDoc(form.DocID, path, false);
            }
			else
				RealSelectDBDoc(0, path, false);
        }

        private void LinkTypeDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            try
            {
                var dialog = e.Dialog as LinkTypeDialog;
                if (dialog != null && dialog.DialogResult == DialogResult.OK)
                {
                    int parentID = dialog.OneID;
                    int childID = dialog.TwoID;

                    if (!dialog.Basic)
                    {
                        parentID = dialog.TwoID;
                        childID = dialog.OneID;
                    }

                    if (Environment.CheckLinkDoc(parentID, childID))
                    {
                        Environment.DocLinksData.AddDocLink(parentID, childID);
                        Environment.UndoredoStack.Add("AddLink", Environment.StringResources.GetString("AddLink"),
                            string.Format(Environment.StringResources.GetString("UndoAddLink"),
                            dialog.DocumentOneText, dialog.DocumentTwoText), string.Format(Environment.StringResources.GetString("RedoAddLink"),
                            dialog.DocumentOneText, dialog.DocumentTwoText), UndoRedoCommands.AddLink,
                            new object[] { parentID, childID }, (curContext.Emp != null ? curContext.Emp.ID : Environment.CurEmp.ID));
                        RefreshLinks();
                    }
                }
            }
            catch (Exception ex)
            { Lib.Win.Data.Env.WriteToLog(ex); }
        }

        public void UpdateCommand_LinkDoc(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsSelectedSingle() && docGrid.IsDBDocs();
        }

        // Refresh Links
        public void On_RefreshLinks(CommandManagement.Command cmd)
        {
            RefreshLinks();
        }

        // AddDocData
        public void On_AddDocData(CommandManagement.Command cmd)
        {
            var dialog = new SelectTypeDialog(0, true, null, Environment.PreviosTypeID, Environment.PreviosDirection, false);
            dialog.DialogEvent += SelectTypeDialog_DialogEvent;
            dialog.Show();
        }

        private void SelectTypeDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            try
            {
                var dialog = e.Dialog as SelectTypeDialog;
                if (dialog == null || dialog.DialogResult != DialogResult.OK)
                    return;

                Environment.PreviosTypeID = dialog.TypeID;
                Environment.PreviosDirection = dialog.Out;
                string url =
                    Environment.DocTypeData.GetField(Environment.DocTypeData.URLField, Environment.PreviosTypeID).ToString
                        ();
                string paramStr = ((Lib.Win.Document.Environment.PersonID > 0)
                                       ? ("&currentperson=" + Lib.Win.Document.Environment.PersonID.ToString())
                                       : "") +
                                  (Environment.PreviosDirection.Equals(SelectTypeDialog.Direction.Out)
                                       ? "&docDir=out"
                                       : (Environment.PreviosDirection.Equals(SelectTypeDialog.Direction.In)
                                              ? "&docDir=in"
                                              : ""));
                if (string.IsNullOrEmpty(url))
                    throw new Exception("Нет формы создания документа");

                if (paramStr.Length > 0)
                {
                    url += url.Contains("?") ? "&" : "?";
                    url += paramStr.TrimStart("&".ToCharArray());
                }
                Lib.Win.Document.Environment.IEOpenOnURL(url);
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
                ErrorShower.OnShowError(this, ex.Message, string.Empty);
            }
        }

        // AddEForm
        public void On_AddEForm(CommandManagement.Command cmd)
        {
            docControl.CreateEForm(false, curDocID);
        }

        public void UpdateCommand_AddEForm(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsSelectedSingle() && docGrid.IsDBDocs();
        }

        // Select Person Folder
        public void On_SelectPersonFolder(CommandManagement.Command cmd)
        {
            if (curDocID > 0)
                SelectDBDocAtPersonFolder(curDocID, nextPersonID);
        }

        public void UpdateCommand_SelectPersonFolder(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsSelectedSingle() && docGrid.IsDBDocs();
        }

        /// <summary>
        /// Список форм удаления документа
        /// </summary>
       private readonly List<Lib.Win.MessageForm> _deleteDocForms = new List<Lib.Win.MessageForm>();

        // DeleteDoc
	   public void On_DeleteDoc(CommandManagement.Command cmd)
	   {
		   // Предотворащение появления двух диалогов удаления документа по одному документу
		   foreach(var form in _deleteDocForms)
		   {
			   if(form.IsDisposed && form.Disposing)
				   continue;

			   if(form.Tag is int)
			   {
				   var docId = (int)form.Tag;

				   if(docId == curDocID)
				   {
					   form.BringToFront();
					   return;
				   }
			   }
		   }
		   Lib.Win.MessageForm dialog = new Lib.Win.MessageForm(Environment.StringResources.GetString("MainForm.MainFormDialog.On_DeleteDoc.Message1")
								+ curDocString +
								Environment.StringResources.GetString("MainForm.MainFormDialog.On_DeleteDoc.Message2") + System.Environment.NewLine +
								Environment.StringResources.GetString("MainForm.MainFormDialog.On_DeleteDoc.Message3") + System.Environment.NewLine + System.Environment.NewLine +
								Environment.StringResources.GetString("MainForm.MainFormDialog.On_DeleteDoc.Message4"),
								Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNo,
								MessageBoxDefaultButton.Button2) { Tag = curDocID };
		   dialog.DialogEvent += DeleteDocMessageForm_DialogEvent;

		   // Подписка на закрытие
		   dialog.Closed += OnClosedDeleteDocumentForm;
		   // Добавляем диалог в список диалогов
		   _deleteDocForms.Add(dialog);
		   dialog.Show();
	   }

        // ScanCurrentDocument
        public void On_AddImageCurrentDoc(CommandManagement.Command cmd)
        {
            if (curDocID <= 0)
                return;

            Dialogs.NewImageAddDialog dialog = new Dialogs.NewImageAddDialog(curDocID);
            //dialog.DialogEvent += dialog_DialogEvent;
            dialog.Show();
        }

        public void UpdateCommand_AddImageCurrentDoc(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsSelectedSingle() && docGrid.IsDBDocs();
        }

		public void On_ScanCurrentDocument(CommandManagement.Command cmd)
		{
			docControl.ScanNewCurrentDoc();

		}

        public void UpdateCommand_ScanCurrentDocument(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsSelectedSingle() && docGrid.IsDBDocs();
        }

        private void DeleteDocMessageForm_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            var dialog = e.Dialog as Lib.Win.MessageForm;
            if (dialog == null || dialog.DialogResult != DialogResult.Yes)
                return;
            try
            {
				int docID = (int)dialog.Tag;

                // 28960 Запрет повторного вызова sp_DeleteDoc
                IDocumentRepository documentRepository = new DocumentRepository();
                if (documentRepository.DeleteDoc(-1, docID, true))
				{
                    int rowIndex = docID == curDocID ? docGrid.CurrentRowIndex : -1;
                    docGrid.DeleteRowConditional(docID);
                    UpdateWorkContext();
                    if (rowIndex > -1)
                        docGrid.SelectRow(rowIndex);
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        // AddToWork
        public void On_AddToWork(CommandManagement.Command cmd)
        {
            int[] ids = IsSelectedSingle() ? new[] { curDocID } : docGrid.GetCurIDs();
            if (ids[0] == -1)
                return;

            if (readTimer != null && readTimer.Enabled)
                readTimer.Stop();

			Dialogs.FolderSelectDialog dialog = new Dialogs.FolderSelectDialog(ids, curEmpID);
            dialog.DialogEvent += FolderSelectDialog_DialogEvent;
            dialog.Show();
        }

        private void FolderSelectDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            if (e.Dialog.DialogResult != DialogResult.OK)
                return;
            try
            {
                RefreshWorkFolders();
                docGrid.UpdateIsInWorkStatus();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public void UpdateCommand_AddToWork(CommandManagement.Command cmd)
        {
            cmd.Enabled = (docGrid.IsSingle || docGrid.IsMultiple) && docGrid.IsDBDocs() && !docGrid.IsInWork();
        }

        // WorkPlaces
        public void On_WorkPlaces(CommandManagement.Command cmd)
        {
            int[] ids = IsSelectedSingle() ? new[] { curDocID } : docGrid.GetCurIDs();
            if (ids[0] == -1)
                return;

            if (readTimer != null && readTimer.Enabled)
                readTimer.Stop();

            Dialogs.MoveDocDialog dialog = new Dialogs.MoveDocDialog(ids, curEmpID);
            dialog.DialogEvent += MoveDocDialog_DialogEvent;
            dialog.Show();
        }

		private void MoveDocDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
		{
			if(e.Dialog.DialogResult != DialogResult.OK)
				return;
			try
			{
				//bool moveToNext = docGrid.IsSingle;
				int index = docGrid.SelectedRowsMinIndex;
				int oldCount = docGrid.Rows.Count;

				RefreshWorkFolders();

				//index -= (oldCount > docGrid.Rows.Count) ? 1 : 0;
				docGrid.SelectRow(index);
				docGrid.UpdateIsInWorkStatus();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

        public void UpdateCommand_WorkPlaces(CommandManagement.Command cmd)
        {
            cmd.Enabled = (IsSelectedSingle() || docGrid.IsMultiple) && docGrid.IsDBDocs();
        }

        // EndWork
        public void On_EndWork(CommandManagement.Command cmd)
        {
            Console.WriteLine("{0}: On_EndWork", DateTime.Now.ToString("HH:mm:ss fff"));
            if (!docGrid.IsFine || !docGrid.IsInWork() || docGrid.SelectedRows.Count <= 0)
                return;
            if (readTimer != null)
                readTimer.Stop();

            Context context = curContext;
            if (context == null)
                return;

            if (docGrid.IsDBDocs())
            {
                StopDocChangeReceiver();
                SaveFile();
            }
            bool isNotRead = IsNotRead();
            bool readAfterRemove = false;
            int[] docIDs = IsSelectedSingle() ? new[] { curDocID } : docGrid.GetCurIDs();
            if (docIDs[0] <= 0)
                return;
            string[] docStrings = docGrid.MakeCurDBDocsStrings();

            if (Environment.UserSettings.DeleteConfirm)
            {
                bool dontShowDialogAgain = false;
                switch (
                    Lib.Win.MessageBoxScrollable.Show(Environment.StringResources.GetString("Confirmation"),
                                              Environment.StringResources.GetString("RemoveMessageOnEndWork"),
                                              docStrings,
                                              MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, this,
                                              out dontShowDialogAgain, out Environment.UserSettings.NeedSave)
                    )
                {
                    case DialogResult.Yes:
                        if (Environment.UserSettings.NeedSave)
                        {
                            Environment.UserSettings.DeleteConfirm = !dontShowDialogAgain;
                            Environment.UserSettings.Save();
                        }
                        break;
                    default:
                        Environment.UserSettings.NeedSave = false;
                        return;
                }
            }
            if (isNotRead)
            {
                switch (Environment.UserSettings.ReadMessageOnEndWork)
                {
                    case 0:
                        readAfterRemove = true;
                        break;
                    case 1:
                        readAfterRemove = false;
                        break;
                    case 2:
                        bool dontShowDialogAgain = false;
                        switch (
                            Lib.Win.MessageBoxScrollable.Show(Environment.StringResources.GetString("Confirmation"),
                                                      Environment.StringResources.GetString("ShouldReadMessageOnEndWork"),
                                                      docGrid.MakeCurDBDocsStringForNotRead(),
                                                      MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, this,
                                                      out dontShowDialogAgain, out Environment.UserSettings.NeedSave)
                            )
                        {
                            case DialogResult.Yes:
                                readAfterRemove = true;
                                if (dontShowDialogAgain)
                                    Environment.UserSettings.ReadMessageOnEndWork = 0;
                                break;
                            case DialogResult.No:
                                readAfterRemove = false;
                                if (dontShowDialogAgain)
                                    Environment.UserSettings.ReadMessageOnEndWork = 1;
                                break;
                            case DialogResult.Cancel:
                                Environment.UserSettings.NeedSave = false;
                                return;
                        }

                        if (Environment.UserSettings.NeedSave)
                            Environment.UserSettings.Save();
                        break;
                }
            }

            Slave.DoWork(workEnder_DoWork, new object[] { docIDs, docStrings, context, readAfterRemove });
            int rowIndex = docGrid.SelectedRowsMinIndex;
            switch (context.Mode)
            {
				case Misc.ContextMode.Found:
                    var fnode = folders.SelectedNode as FoundNode;
                    if (fnode != null && fnode.XML != null)
						if(Options.HasOption(fnode.XML, typeof(Lib.Win.Data.DALC.Documents.Search.Document.ВРаботе)) ||
                            (Options.HasOption(fnode.XML, typeof(Lib.Win.Data.DALC.Documents.Search.Message.Incoming.Incoming_Read))))
                        {
                            docGrid.DeleteSelectedRows();
                            if (!docGrid.SelectRow(rowIndex))
                                ClearAll();
                        }
                    break;
				case Misc.ContextMode.WorkFolder:
                    docGrid.DeleteSelectedRows();
                    if (!docGrid.SelectRow(rowIndex))
                        ClearAll();
                    break;
            }
        }

        private void workEnder_DoWork(object sender, DoWorkEventArgs e)
        {
            int[] docIDs = (int[])((object[])e.Argument)[0];
            string[] docStrings = (string[])((object[])e.Argument)[1];
            Context context = (Context)((object[])e.Argument)[2];
            bool readAfterRemove = (bool)((object[])e.Argument)[3];

            Employee emp = context.Emp ?? Environment.CurEmp;

            bool success = Environment.WorkDocData.RemoveDocFromWork(docIDs, emp.ID);

            if (success && readAfterRemove)
                foreach (int id in docIDs)
                {
                    int minMessID;
                    int maxMessID;
                    if (Environment.WorkDocData.GetDocMessagesIDs(emp.ID, id, true, out minMessID, out maxMessID))
                        Environment.WorkDocData.MarkAsRead(emp.ID, minMessID, maxMessID, true);
                }

            if (success)
            {// Всё прошло хорошо
                for (int i = 0; i < docIDs.Length; i++)
                {
                    Environment.UndoredoStack.Add("RemoveDocFromWork", Environment.StringResources.GetString("RemoveDocFromWork"),
                                                  string.Format(Environment.StringResources.GetString("UndoRemoveDocFromWork"), docStrings[i]),
                                                  string.Format(Environment.StringResources.GetString("RedoRemoveDocFromWork"), docStrings[i]),
                                                  null, new object[] { docIDs[i], (context.WorkOrSharedFolderMode() ? context.ID : 0), emp.ID }, emp.ID);
                }
            }
            else
            {// Что-то наебнулось в процессе

                // Заявка 27393
                // Работаем в backgraund процессе.
                // Нужен маршалинг в UI поток
                BeginInvoke((MethodInvoker) RefreshDocs);
            }
        }

        public void UpdateCommand_EndWork(CommandManagement.Command cmd)
        {
            cmd.Enabled = (docGrid.IsSingle || docGrid.IsMultiple) && docGrid.IsInWork();
        }

        // DeleteFromFound
        public void On_DeleteFromFound(CommandManagement.Command cmd)
        {
            if (Environment.DocData.DeleteFromFound(curDocID, Environment.CurEmp.ID))
                Environment.RefreshDocs();
        }

        public void UpdateCommand_DeleteFromFound(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsSelectedSingle() && docGrid.IsFound();
        }

        public void On_Exit(CommandManagement.Command cmd)
        {
            ExitApp();
        }

        // Settings Scaner
        public void On_SettingsScaner(CommandManagement.Command cmd)
        {
            docControl.SelectScanner();
        }

        // Help
        public void On_Help(CommandManagement.Command cmd)
        {
            if (changesDialog == null)
            {
                changesDialog = new Dialogs.ChangesDialog();
                changesDialog.Show();
            }
            else
            {
                changesDialog.WindowState = FormWindowState.Normal;
                changesDialog.BringToFront();
            }
        }

        public void On_DeletePart(CommandManagement.Command cmd)
        {
            try
            {
                docControl.TestImage(Lib.Win.Document.Environment.ActionBefore.None);
                docControl.DeletePart();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public void UpdateCommand_DeletePart(CommandManagement.Command cmd)
        {
            cmd.Enabled = IsDisplayedSingle() && docGrid.IsScaner() && (docControl.PageCount > 1);
        }

        public void On_Undo(CommandManagement.Command cmd)
        {
            Environment.UndoredoStack.Undo(1);
        }

        public void UpdateCommand_Undo(CommandManagement.Command cmd)
        {
            bool enabled = Environment.UndoredoStack.UndoItems.Count > 0;
            cmd.Enabled = enabled;
            undoButton.ToolTipText = enabled ? Environment.UndoredoStack.UndoText : string.Empty;
        }

        public void On_Redo(CommandManagement.Command cmd)
        {
            Environment.UndoredoStack.Redo(1);
        }

        public void UpdateCommand_Redo(CommandManagement.Command cmd)
        {
            bool enabled = Environment.UndoredoStack.RedoItems.Count > 0;
            cmd.Enabled = enabled;
            redoButton.ToolTipText = enabled ? Environment.UndoredoStack.RedoText : string.Empty;
        }

        #endregion

        #region DocGrid

        private void docGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (!docGrid.IsFine)
                return;

            if (!keyLocker.Contains(e.KeyData))
            {
                keyLocker.Add(e.KeyData);
                try
                {
                    switch (e.KeyData)
                    {
                        case Keys.Space:
                            if (!IsNotRead())
                                break;
                            if (docGrid.IsSingle)
                            {
                                if (readTimer != null && readTimer.Enabled)
                                    ReadTimerProcessor(null, EventArgs.Empty);
                                else
                                    Environment.CmdManager.Commands["MarkReadMessages"].Execute();
                            }
                            else if (docGrid.IsMultiple && docGrid.IsInWork())
                            {
                                if (readTimer != null && readTimer.Enabled)
                                    readTimer.Stop();

                                Environment.CmdManager.Commands["MarkReadMessages"].Execute();
                            }

                            break;
                        case Keys.Control | Keys.A:
                            ClearInfo();
                            ClearDoc();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Lib.Win.Data.Env.WriteToLog(ex);
                }
                finally
                {
                    keyLocker.Remove(e.KeyData);
                }
            }
        }

        private void UpdateWorkContext()
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(UpdateWorkContext));
                return;
            }
            try
            {
#if AdvancedLogging
                Lib.Log.Logger.EnterMethod(this, "UpdateWorkContext()");
#endif

#if AdvancedLogging
                using (Lib.Log.Logger.DurationMetter("UpdateWorkContext folders.UpdateWorkFolderStatus();"))
#endif
                folders.UpdateWorkFolderStatus();

                StatusBar_UpdateDocCount();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
            finally
            {
#if AdvancedLogging
                Lib.Log.Logger.LeaveMethod(this, "UpdateWorkContext()");
#endif
            }
        }

        /// <summary>
        /// Обновление статуса рабочих папок, панели статуса(кол-во д-в). Асинхронно.
        /// </summary>
        private void BeginUpdateWorkContextAsync( bool refresh)
        {
#if AdvancedLogging
            Lib.Log.Logger.Message( "BeginUpdateWorkContextAsync()");
#endif

            Task.Factory.StartNew(UpdateWorkContextAsync, refresh);
        }

		private void UpdateWorkContextAsync(object param)
		{
			try
			{
#if AdvancedLogging
				Lib.Log.Logger.EnterMethod(this, "UpdateWorkContextAsync(object param)");
#endif
				bool refresh = (bool)param;
#if AdvancedLogging
				using(Lib.Log.Logger.DurationMetter("UpdateWorkContext folders.UpdateWorkFolderStatus();"))
#endif
				folders.UpdateWorkFolderStatus(true, refresh);

				BeginInvoke((MethodInvoker)(StatusBar_UpdateDocCount));
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			finally
			{
#if AdvancedLogging
				Lib.Log.Logger.LeaveMethod(this, "UpdateWorkContextAsync(object param)");
#endif
			}
		}

        public void RowDeleteTest()
        {
            if (InvokeRequired)
                BeginInvoke((MethodInvoker)(RowDeleteTest));
            else
            {
                var ren = new Random(100000);
                int id = ren.Next(100);
                docGrid.DeleteRowConditional(id);
            }
        }

        internal void AnalyzeCurrentCell()
        {
            Console.WriteLine("{0}: AnalyzeCurrentCell", DateTime.Now.ToString("HH:mm:ss fff"));
			if(!docGrid.IsFine || docGrid.SelectedRows.Count != 1)
			{
				if(docGrid.Rows.Count == 0 || docGrid.SelectedRows.Count > 1 || docGrid.SelectedRows.Count == 0)
				{
					ClearInfo();
					ClearDoc();
				}
				return;
			}
          

            // мы уже тут
            if (docGrid.IsDBDocs() && curDocID == docGrid.GetCurID())
                return;
            if (docGrid.IsFaxes() && curFaxID == docGrid.GetCurID())
                return;
            if (docGrid.IsScaner() && fileName == docGrid.KeyObject as string)
                return;

            // выключить таймер пост-загрузки
            if (afterLoadTimer != null && afterLoadTimer.Enabled)
                afterLoadTimer.Stop();

			docControl.Visible = false;

            // выключить таймер получения сообщения по документу
            if (documentChangeTimer != null && documentChangeTimer.Enabled)
                documentChangeTimer.Stop();
			if(docGrid.IsFaxesIn() && !0.Equals(Convert.ToInt32( docGrid.GetCurValue(Environment.FaxData.StatusField))))
			{
				if(Environment.UserSettings.ReadTimeout > 0 && IsNotRead())
				{
					if(readTimer == null)
					{
						readTimer = new System.Windows.Forms.Timer();
						readTimer.Tick += ReadTimerProcessor;
					}

					readTimer.Interval = Environment.UserSettings.ReadTimeout;
					readTimer.Start();
				}
				return;
			}
            // выключить таймер прочтения
            if (readTimer != null && readTimer.Enabled)
                readTimer.Stop();

            if (documentSelectedTimer == null)
            {
                documentSelectedTimer = new System.Timers.Timer(documentSelectedTimeout);
                documentSelectedTimer.Elapsed += documentSelectedTimerProcessor;
            }
            documentSelectedTimer.Stop();
            documentSelectedTimer.Start();
        }

        private void documentSelectedTimerProcessor(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("{0}: documentSelectedTimerProcessor", DateTime.Now.ToString("HH:mm:ss fff"));
            documentSelectedTimer.Stop();

            if (Disposing || IsDisposed)
                return;
            InvokeIfRequired((MethodInvoker)(RealAnalyzeCurrentCell));
        }

        private void afterLoadTimerProcessor(object sender, EventArgs e)
        {
            Console.WriteLine("{0}: afterLoadTimerProcessor", DateTime.Now.ToString("HH:mm:ss fff"));
            afterLoadTimer.Stop();
			InvokeIfRequired((MethodInvoker)(AfterLoadCell));
        }

        private void AfterLoadCell()
        {
            Console.WriteLine("{0}: AfterLoadCell", DateTime.Now.ToString("HH:mm:ss fff"));
			if(showDocsAndMessages != 1 && !Environment.LoadMessageFirst)
			{
				RefreshInfo();

				//if(docGrid.IsFine && (!docGrid.Focused || !docGrid.CurrentRowSelected))
				//    docGrid.Select();
			}
            StatusBar_UpdatePage();
        }

        private void docControl_LoadComplete(object sender, EventArgs e)
        {
            docControl.LoadComplete -= docControl_LoadComplete;
            AfterLoadCell();
        }

        private void docControl_SetScroll_LoadComplete(object sender, EventArgs e)
        {
            docControl.PageChanged -= docControl_SetScroll_LoadComplete;

            RestoreScrollPositionFromOption(true);
        }

        private void ReloadCurrentDoc()
        {
            try
            {
                if (documentChangeTimer != null)
                {
                    documentChangeTimer.Stop();
                    documentChangeTimer.Dispose();
                    documentChangeTimer = null;
                }

                documentChangeTimer = new System.Timers.Timer(documentChangeTimeout) { AutoReset = false };
                documentChangeTimer.Elapsed += documentChangeTimer_Elapsed;
                documentChangeTimer.Start();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void RealAnalyzeCurrentCell()
        {
			if(!docGrid.IsFine || docGrid.CurrentCell == null || docGrid.SelectedRows.Count != 1)
			{
				ClearInfo();
				ClearDoc();
				return;
			}
            Console.WriteLine("{0}: RealAnalyzeCurrentCell", DateTime.Now.ToString("HH:mm:ss fff"));
			
            try
            {
#if AdvancedLogging
                Lib.Log.Logger.EnterMethod(this, "RealAnalyzeCurrentCell()");
#endif

                var spliterPlace = new Point(-1, -1);

                if (curImageID > 0 && docControl.ImageID > 0 && docControl.PageCount > 0)
                    spliterPlace = docControl.SplinterPlace;

				if(docGrid.IsDBDocs())
				{
					// получаем код текущего документа
					int docID = docGrid.GetCurID();

					if(docID <= 0)
						throw new Exception(Environment.StringResources.GetString("MainForm.MainFormDialog.RealAnalyzeCurrentCell.Message1"));

					if(curDocID != docID)
						curDocID = docID;

					SaveFile();
					var res = Environment.DocData.GetField(Environment.DocData.ProtectedField, curDocID);
					statusBarPanelSecure.Text = 1.Equals(Convert.ToInt32(res)) ? (Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru") ? "Секретно" : "Private") : "";
					if(showDocsAndMessages != 1 && Environment.LoadMessageFirst)
						RefreshInfo();

					if(showDocsAndMessages != 2)
					{
						if(docControl.DocumentID != curDocID)
						{
							// Оптимизация. При переходе с документа на документ, обновляю подписку, а не пересоздаю Receiver
							//StopDocChangeReceiver();
							//StartDocChangeReceiver(curDocID);
							Console.WriteLine("{0}: UpdateOrCreateDocChangeReceiver start", DateTime.Now.ToString("HH:mm:ss fff"));
							UpdateOrCreateDocChangeReceiver(curDocID);
							Console.WriteLine("{0}: UpdateOrCreateDocChangeReceiver end", DateTime.Now.ToString("HH:mm:ss fff"));
							int imgID = Environment.General.LoadIntOption("ImageID", -1);
							int page = (int)Environment.General.Option("Page").Value;
							docControl.Visible = true;
							if(imgID == -1)
							{
								if(docGrid[Environment.DocData.MainImageIDField, docGrid.SelectedRowsMinIndex].Value is int)
								{
									imgID = (int)docGrid[Environment.DocData.MainImageIDField, docGrid.SelectedRowsMinIndex].Value;
								}
								else
									imgID = 0;
								page = 0;
							}
							if(curDocID > 0 && imgID >= 0)
							{
								docControl.PageChanged += docControl_Load_PageChanged;
								docControl.LoadDocument(curDocID, imgID, page);

								docControl.ScrollPositionX = (int)Environment.General.Option("ScrollPositionX").Value;
								docControl.ScrollPositionY = (int)Environment.General.Option("ScrollPositionY").Value;
								if(docControl.CompliteLoading)
								{
									Environment.General.Option("Page").Value = 0;
									Environment.General.Option("ImageID").Value = -1;
								}
							}
							else
								docControl.DocumentID = curDocID;
						}
						else
						{
							Console.WriteLine("{0}: ImgID = {1}", DateTime.Now.ToString("HH:mm:ss fff"), curImageID);
							// При быстром переходе на иной документ и обратно документ нужно показать.
							docControl.Visible = curDocID > 0;
							if(curImageID > 0)
							{
								int curpage = docControl.Page;
								docControl.ImageID = curImageID;
								docControl.Page = curpage;
								if(docControl.ShowWebPanel)
									docControl.RefreshEForm();
							}
							else
							{
								docControl.RefreshEForm();
							}
							docControl.ReloadTran(true);
							docControl.RefreshSigns();
						}
					}

					fileName = curImageID > 0 ? docControl.FileName : string.Empty;

					bCheck = ButtonCheck.MoveImg;
					UpdateZoom();
					Console.WriteLine("{0}: RealAnalyzeCurrentCell start timers", DateTime.Now.ToString("HH:mm:ss fff"));
					// work folder timer
					if(Environment.UserSettings.ReadTimeout > 0 && IsNotRead())
					{
						if(readTimer == null)
						{
							readTimer = new System.Windows.Forms.Timer();
							readTimer.Tick += ReadTimerProcessor;
						}

						readTimer.Interval = Environment.UserSettings.ReadTimeout;
						readTimer.Start();
					}

					// пост-загрузка
					if(docGrid.IsDBDocs() && !(docGrid.GetCurValue(Environment.DocData.MainImageIDField) is int))
					{
						if(afterLoadTimer == null)
						{
							afterLoadTimer = new System.Timers.Timer();
							afterLoadTimer.Elapsed += afterLoadTimerProcessor;
						}
						else
						{
							afterLoadTimer.Stop();
						}
						afterLoadTimer.Interval = afterLoadTimeout;
						afterLoadTimer.Start();
					}
					else
					{
						if(docControl.CompliteLoading)
							AfterLoadCell();
						else
						{
							docControl.LoadComplete += docControl_LoadComplete;
							docControl.PageChanged += docControl_SetScroll_LoadComplete;
							if(docControl.CompliteLoading)
								AfterLoadCell();
						}
					}

					ReloadCurrentDoc();
				}
				else
				{
					if(docGrid.IsFaxes())
					{
						FolderTree.FolderNodes.FaxNodes.FaxNode node = folders.SelectedNode as FolderTree.FolderNodes.FaxNodes.FaxNode;
						fileName = Path.Combine(node.FaxPath, (string)docGrid.GetCurValue(Environment.FaxData.FileNameField)); 
						curFaxID = docGrid.GetCurID();

						// fax in timer
						if(docGrid.IsFaxesIn() && (Environment.UserSettings.ReadTimeout > 0) &&
							!docGrid.GetBoolValue(docGrid.CurrentRowIndex, Environment.FaxInData.ReadField))
						{
							if(readTimer == null)
							{
								readTimer = new System.Windows.Forms.Timer();
								readTimer.Tick += ReadTimerProcessor;
							}

							readTimer.Interval = Environment.UserSettings.ReadTimeout;
							readTimer.Start();
						}
					}
					else if(docGrid.IsScaner())
						fileName = ((string)docGrid.GetCurValue(Environment.ScanReader.FullNameField));
					else if(docGrid.IsDiskImages())
						fileName = ((string)docGrid.GetCurValue(Environment.ImageReader.FullNameField));

					curDocID = 0;
					curImageID = -1;

					ShowFile();
				}

                if (spliterPlace.X > -1 && spliterPlace.Y > -1)
                {
                    if (curImageID > 0 && docControl.ImageID > 0 && docControl.PageCount > 0 && spliterPlace != docControl.SplinterPlace)
                        docControl.SplinterPlace = spliterPlace;
                    else
                    {
                        Environment.Layout.Option("SplitterX").Value = spliterPlace.X;
                        Environment.Layout.Option("SplitterY").Value = spliterPlace.Y;
                    }
                }
                curDocString = docGrid.MakeCurDocString();

                StatusBar_UpdateDoc(docControl.IsReadonly);
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
                ErrorMessage(ex.Message);
            }
            finally
            {
                if ((!docGrid.Focused || !docGrid.CurrentRowSelected) && this == ActiveForm)
                    docGrid.Select();

                Console.WriteLine("{0}: RealAnalyzeCell released DocGrid", DateTime.Now.ToString("HH:mm:ss fff"));

#if AdvancedLogging
                Lib.Log.Logger.LeaveMethod(this, "RealAnalyzeCurrentCell()");
#endif
            }
        }

		private void docGrid_CurrentCellChanged(object sender, EventArgs e)
		{
			try
			{
#if AdvancedLogging
                Lib.Log.Logger.EnterMethod(this, "docGrid_CurrentCellChanged");
#endif
				StatusBar_UpdateDocCount();
				if(docGrid.SelectedRows.Count == 1)
				{
					AnalyzeCurrentCell();
				}
				else if(docGrid.SelectedRows.Count != 1 && curContext.Mode != Misc.ContextMode.Document)
				{
					if(docGrid.IsDBDocs())
					{
						StopDocChangeReceiver();
						SaveFile();
					}
					ClearDoc();
					curDocID = 0;
					curFaxID = 0;
					fileName = string.Empty;
					ClearInfo();
				}
				docGrid.UpdateIsInWorkStatus();
			}
			finally
			{
#if AdvancedLogging
                Lib.Log.Logger.LeaveMethod(this, "docGrid_CurrentCellChanged");
#endif
			}
		}

		void docGrid_DataSourceChanged(object sender, EventArgs e)
		{
			if(!(docGrid.DataSource is DataView) || ((DataView)docGrid.DataSource).Table.Rows.Count != 0)
				return;
			if(curContext.Mode == Misc.ContextMode.Document)
				return;
			try
			{
				if(docGrid.IsDBDocs())
					StopDocChangeReceiver();

				ClearDoc();
				curDocID = 0;
				curFaxID = 0;
				fileName = string.Empty;
				ClearInfo();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

        internal static void AnalyzeTmpFile(KeyValuePair<string, Lib.Win.Document.Objects.TmpFile> tf, ref bool revert)
        {
            try
            {
                AddDBDocDialog sad = Lib.Win.Document.Environment.DocToSave[tf.Value.TmpFullName] as AddDBDocDialog;
                SavePartDialog spd = Lib.Win.Document.Controls.DocControl.GetSavePartDialog(tf.Value.TmpFullName);
                if (spd != null && spd.Delete)
                    spd = null;
                SendFaxDialog sfd = Lib.Win.Document.Environment.DocToSend[tf.Value.TmpFullName] as SendFaxDialog;
                DocPrintDialog pd = Lib.Win.Document.Environment.DocToPrint[tf.Value.TmpFullName] as DocPrintDialog;

                revert = false;

                if (sad != null)
                {
                    if (new Lib.Win.MessageForm(
                            string.Format(
                                Environment.StringResources.GetString("DocControl.TmpFileWarning4") + " " +
                                Environment.StringResources.GetString("DocControl.TmpFileSaveWarning") +
                                "{1}" + Environment.StringResources.GetString("DocControl.TmpFileWarning2") +
                                (tf.Value.Modified
                                     ? "{1}" + Environment.StringResources.GetString("DocControl.TmpFileWarning3")
                                     : ""),
                                tf.Key, System.Environment.NewLine), "", MessageBoxButtons.YesNo).ShowDialog() ==
                        DialogResult.Yes)
                        revert = true;
                    else
                        sad.Close();
                }

                if (spd != null && !revert)
                {
                    if (new Lib.Win.MessageForm(
                            string.Format(
                                Environment.StringResources.GetString("DocControl.TmpFileWarning4") + " " +
                                Environment.StringResources.GetString("DocControl.TmpFileSavePartWarning") +
                                "{1}" + Environment.StringResources.GetString("DocControl.TmpFileWarning2") +
                                (tf.Value.Modified
                                     ? "{1}" + Environment.StringResources.GetString("DocControl.TmpFileWarning3")
                                     : ""),
                                tf.Key, System.Environment.NewLine), "", MessageBoxButtons.YesNo).ShowDialog() ==
                        DialogResult.Yes)
                        revert = true;
                    else
                        spd.Close();
                }

                if (sfd != null && !revert)
                {
                    if (new Lib.Win.MessageForm(
                            string.Format(
                                Environment.StringResources.GetString("DocControl.TmpFileWarning4") + " " +
                                Environment.StringResources.GetString("DocControl.TmpFileSendWarning") +
                                "{1}" + Environment.StringResources.GetString("DocControl.TmpFileWarning2") +
                                (tf.Value.Modified
                                     ? "{1}" + Environment.StringResources.GetString("DocControl.TmpFileWarning3")
                                     : ""),
                                tf.Key, System.Environment.NewLine), "", MessageBoxButtons.YesNo).ShowDialog() ==
                        DialogResult.Yes)
                        revert = true;
                    else
                        sfd.Close();
                }

                if (pd != null && !revert)
                {
                    if (new Lib.Win.MessageForm(
                            string.Format(
                                Environment.StringResources.GetString("DocControl.TmpFileWarning4") + " " +
                                Environment.StringResources.GetString("DocControl.TmpFilePrintWarning") +
                                "{1}" + Environment.StringResources.GetString("DocControl.TmpFileWarning2") +
                                (tf.Value.Modified
                                     ? "{1}" + Environment.StringResources.GetString("DocControl.TmpFileWarning3")
                                     : ""),
                                tf.Key, System.Environment.NewLine), "", MessageBoxButtons.YesNo).ShowDialog() ==
                        DialogResult.Yes)
                        revert = true;
                    else
                        pd.Close();
                }

                if (revert)
                {
                    if (pd != null)
                    {
                        pd.BringToFront();
                        pd.Activate();
                    }
                    if (sfd != null)
                    {
                        sfd.BringToFront();
                        sfd.Activate();
                    }
                    if (sad != null)
                    {
                        sad.BringToFront();
                        sad.Activate();
                    }
                    if (spd != null)
                    {
                        spd.BringToFront();
                        spd.Activate();
                    }
                }
                else
                {
                    tf.Value.LinkCnt--;
                    if (tf.Value.LinkCnt <= 0)
                        Lib.Win.Document.Environment.RemoveTmpFile(tf.Key);
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        #endregion

        #region Exit

        public const int SC_CLOSE = 0xF060;
        public const int WM_SYSCOMMAND = 0x0112;
        public bool _closeClick;

		protected override void WndProc(ref Message m)
		{
			try
			{
				if(m.Msg == WM_SYSCOMMAND && m.WParam != null && (int)m.WParam == SC_CLOSE)
					_closeClick = true;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex, m.ToString());
			}
			base.WndProc(ref m);
		}

        private void ExitApp(bool withCheck = false)
        {
            if (withCheck)
            {
                if (Lib.Win.Document.Environment.TmpFiles.Count > 0)
                {
                    SaveFile();
                    foreach (KeyValuePair<string, Lib.Win.Document.Objects.TmpFile> kv in Lib.Win.Document.Environment.TmpFiles)
                    {
                        if (kv.Value.Window != null)
                        {
                            SubFormDialog sfd = kv.Value.Window as SubFormDialog;
                            if (sfd != null && sfd.docControl != null)
                                sfd.SaveFile();
                        }
                    }
                }

                if (Lib.Win.Document.Environment.TmpFiles.Count(x => x.Value.Modified || (x.Value.Window == null && x.Value.LinkCnt > 1) || (x.Value.Window != null && x.Value.LinkCnt > 2)) > 0)
                {
                    if (new Lib.Win.MessageForm(string.Format(Environment.StringResources.GetString("TmpFilesOnCloseWarning"), System.Environment.NewLine), "", MessageBoxButtons.YesNo).ShowDialog(this) == DialogResult.Yes)
                        return;
                }
                try
                {
                    for (int i = 0; i < Lib.Win.Document.Environment.TmpFiles.Count; i++)
                    {
                        if (Lib.Win.Document.Environment.TmpFiles[i].Value.Window != null)
                        {
                            SubFormDialog sfd = Lib.Win.Document.Environment.TmpFiles[i].Value.Window as SubFormDialog;
                            if (sfd != null && sfd.docControl != null)
                                sfd.docControl.WatchOnFile = false;
                        }
                        else if (Lib.Win.Document.Environment.TmpFiles[i].Value.IsInMain && docControl != null)
                            docControl.WatchOnFile = false;

                        try { Slave.DeleteFile(Lib.Win.Document.Environment.TmpFiles[i].Value.TmpFullName); }
                        catch { }
                    }
                }
                catch { }
            }

            ExitForm();
            if (notifyIcon != null)
                notifyIcon.Visible = false;
            if (Program.InstanceHandler != null)
                Program.InstanceHandler.RaiseMutex("DocView.EXE");
            Application.Exit();
        }

        private void ExitForm()
        {
            notifyIcon.Visible = true;
            SaveState();
            if (faxInReceiver != null)
            {
                faxInReceiver.Exit();
                faxInReceiver.Dispose();
            }
            if (messageReceiver != null)
            {
                messageReceiver.Exit();
                messageReceiver.Dispose();
            }
        }

        #endregion

        #region File

		private bool ShowFile()
		{
			bool result = false;
			try
			{
				Console.WriteLine("{0}: ShowFile start", DateTime.Now.ToString("HH:mm:ss fff"));
				Environment.SaveActiveForm();
				SaveFile();
				if(File.Exists(fileName) || Directory.Exists(Path.GetDirectoryName(fileName) ?? string.Empty) || docGrid.IsFaxes())
				{
					bool update = true;
					docControl.Visible = true;
					string oldFileName = docControl.FileName;

					if(oldFileName == fileName && oldFileInfo != null && File.Exists(fileName))
					{
						var fileInfo = new FileInfo(fileName);
						if(fileInfo.LastWriteTime == oldFileInfo.LastWriteTime && !docControl.Modified)
							update = false;
					}

					if(update)
					{
						int page = (int)Environment.General.Option("Page").Value;
						// updating image
						if(page > 0)
						{
							docControl.PageChanged += docControl_Load_PageChanged;
							docControl.LoadFile(fileName, page);
							Environment.General.Option("Page").Value = 0;
						}
						else
							docControl.FileName = fileName;
						if(docGrid.IsFaxesIn())
							docControl.CreateFaxInComponent(curFaxID);
						else if(docGrid.IsFaxesOut())
							docControl.CreateFaxOutComponent(curFaxID);
						else
						{
							docControl.NeedToRefresh -= docControl_NeedToRefresh;
							docControl.WatchOnFile = (docControl.DocumentID <= 0);
							docControl.NeedToRefresh += docControl_NeedToRefresh;
						}

						UpdateZoom();

						oldFileInfo = File.Exists(fileName) ? new FileInfo(fileName) : null;

						textShow(null);
						result = true;
					}
				}
				else
					ClearDoc();

				StatusBar_UpdatePage();
				Environment.RestoreActiveForm();
				Console.WriteLine("{0}: ShowFile end", DateTime.Now.ToString("HH:mm:ss fff"));
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex, string.Format("Filename: {0}", fileName));
			}
			return result;
		}

        private void SaveFile(Lib.Win.Document.Environment.ActionBefore act = Lib.Win.Document.Environment.ActionBefore.LeaveFile)
        {
            if (docControl != null && docControl.ImageDisplayed && docControl.Modified)
                if (File.Exists(docControl.FileName))
                {
                    docControl.TestImage(act);
                }
        }

		private void ClearDoc()
		{
			if(InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(ClearDoc));
				return;
			}

			SaveFile();
			docControl.Visible = false;
			try
			{
				fileName = string.Empty;
				curDocString = string.Empty;
				curDocID = 0;
				curFaxID = 0;

				docControl.FileName = string.Empty;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}

			if(mISeparator != null && mISeparator.Visible)
			{
				for(int i = mINotes.MenuItems.Count - 1; i >= 0; i--)
					if(!string.IsNullOrEmpty(mINotes.MenuItems[i].Tag as string) && mINotes.MenuItems[i].Tag.Equals("group"))
						mINotes.MenuItems.RemoveAt(i);
				mISeparator.Visible = false;
			}

			statusBarPanelDoc.Text = string.Empty;
			statusBarPanelSecure.Text = string.Empty;
			statusBarPanelDSP.Text = string.Empty;
			statusBarPanelPage.Text = string.Empty;
			statusBarPanelPage.ToolTipText = string.Empty;

			Text = Environment.StringResources.GetString("ArchiveDocs") + Environment.CompanyName;
		}

        private void ClearAll()
        {
            try
            {
                if (docChangedReceiver != null)
                {
                    docChangedReceiver.Exit();
                    docChangedReceiver.Received -= receiver_Received;
                    docChangedReceiver = null;
                }

                curImageID = -1;

                ClearDoc();

                bCheck = ButtonCheck.No;

                curDocID = 0;
                RefreshInfo(true);

                curFaxID = 0;
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        #endregion

        #region Folders

		private void folders_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if(folderUpdateTimer != null)
			{
				folderUpdateTimer.Tick -= new EventHandler(folderUpdateTimer_Tick);
				folderUpdateTimer.Stop();
				folderUpdateTimer.Dispose();
				folderUpdateTimer = null;
			}
			Context oldContext = null;
			if(folders.PreviouseSelectedNode != null)
				oldContext = folders.PreviouseSelectedNode.BuildContext();
			CursorSleep();
			Node node = (Node)e.Node;
			showPagesPanelButton.Enabled = true;
			docControl.TestImage();
			Context nodeContext = node.BuildContext();
			if(node.IsDocument())
			{
				if(curDocID != nodeContext.ID)
					ClearAll();
				curDocID = nodeContext.ID;
				//  if (curDocID == 0)
				docGrid.CurrentCell = null;
				if(docControl.DocumentID != curDocID)
				{
					docControl.Visible = true;
					if(curDocID > 0)
						StartDocChangeReceiver(curDocID);
					docControl.DocumentID = curDocID;
				}
				else
					documentID = -curDocID;
				curDocString = node.Text;
				Text = curDocString;
				fileName = curImageID > 0 ? docControl.FileName : string.Empty;

				bCheck = ButtonCheck.MoveImg;
				UpdateZoom();

				AfterLoadCell();
				ReloadCurrentDoc();
			}
			else
				if(!nodeContext.AtHome(oldContext))
					ClearAll();

			Console.WriteLine("{0}: Node selected. Loading docs", DateTime.Now.ToString("HH:mm:ss fff"));
			if(folders.SelectedNode == node)
			{
				folders.Cursor = Cursors.AppStarting;
				docGrid.Cursor = Cursors.WaitCursor;

				node.LoadDocs(docGrid, true, node.IsDocument()?-1:node.CurID, null);
				InvokeIfRequired(() =>
					{
						if(Environment.UserSettings.FolderUpdateTime > 0 && node.IsFound() && node.ID != 0)
						{
							folderUpdateTimer = new System.Windows.Forms.Timer();
							folderUpdateTimer.Enabled = true;
							folderUpdateTimer.Interval = Environment.UserSettings.FolderUpdateTime * 60000;
							folderUpdateTimer.Tick += new EventHandler(folderUpdateTimer_Tick);
							folderUpdateTimer.Start();
						}
					});
				CursorWake();

				Console.WriteLine("{0}: Docs loaded", DateTime.Now.ToString("HH:mm:ss fff"));
				
			}
		}

		void folderUpdateTimer_Tick(object sender, EventArgs e)
		{
			InvokeIfRequired(() =>
			{
				Node node = folders.SelectedNode as Node;
				if(node != null && node.IsFound() && node.ID > 0)
				{
					node.LoadDocs(docGrid, false, curDocID);
					if(folderUpdateTimer != null)
						folderUpdateTimer.Enabled = true;
				}
				else
				{
					var tim = sender as System.Windows.Forms.Timer;
					if(tim != null)
					{
						tim.Tick -= new EventHandler(folderUpdateTimer_Tick);
						tim.Stop();
						tim.Dispose();
					}
					if(folderUpdateTimer != null)
					{
						if(folderUpdateTimer != tim)
						{
							folderUpdateTimer.Tick -= new EventHandler(folderUpdateTimer_Tick);
							folderUpdateTimer.Stop();
							folderUpdateTimer.Dispose();
						}
						folderUpdateTimer = null;
					}
				}
			});
		}

        private void InitFolders()
        {
            if (Disposing || IsDisposed)
                return;
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(InitFolders));
                return;
            }

            try
            {
#if AdvancedLogging
                Lib.Log.Logger.EnterMethod(this, "InitFolders()");
#endif

            folders.SelectedNode = null;
            statusBarPanelArchive.Text = Environment.CompanyName;

            if (folders.Nodes.Count > 0)
                folders.Nodes.Clear();

            // добавить специальные папки 
			if(Environment.IsConnected)
			{
				try
				{
					folders.CreateWorkFolderRoot();
					string ids = string.Join(",", Environment.UserSettings.LinkDocIDs.Cast<int>().Select(id => id.ToString()).ToArray());
					if(!string.IsNullOrEmpty(ids))
						using(DataTable dt = Environment.DocData.GetLinkDocs(ids))
						{
							foreach(DataRow dr in dt.Rows)
								folders.AddDocumentNode((int)dr[Environment.DocData.IDField], false, DBDocString.Format(dr), 1.Equals(dr["Связи"]));
							dt.Dispose();
						}

#if AdvancedLogging
                    using (Lib.Log.Logger.DurationMetter("InitFolders folders.CreateFoundRoot();"))
#endif
					folders.CreateFoundRoot();

					folders.CreateCatalogRoot();
				}
				catch(Exception ex)
				{
					Lib.Win.Data.Env.WriteToLog(ex);
					ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("Error"));
				}

				if(Lib.Win.Document.Environment.GetServers().Any(x => !string.IsNullOrEmpty(x.FaxPath)))
				{
					// факсы входящие
					folders.CreateFaxInRoot();

					menuGotoFaxIn.Visible = (folders.FaxInNode != null);

					// факсы отправленные
					folders.CreateFaxOutRoot();
				}

				// сканер
				if(Lib.Win.Document.Environment.GetServers().Any(x => !string.IsNullOrEmpty(x.ScanPath)))
					folders.CreateScanerRoot();
			}

            // убираем болд
            folders.RemoveBold();

            }
            finally
            {
#if AdvancedLogging
                Lib.Log.Logger.LeaveMethod(this, "InitFolders()");
#endif
            }
        }

		private void LoadFolders()
		{
			if(InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(LoadFolders));
				return;
			}

			try
			{
#if AdvancedLogging
                Lib.Log.Logger.EnterMethod(this, "LoadFolders()");
#endif
				Task.Factory.StartNew(new Action (Environment.ReloadReadData));
                Console.WriteLine("{0}: Init folders", DateTime.Now.ToString("HH:mm:ss fff"));
				InitFolders();
                Console.WriteLine("{0}: Init finished", DateTime.Now.ToString("HH:mm:ss fff"));

				string path = Environment.General.Option("Path").Value as string;
				try
				{
					if(!path.StartsWith("|"))
						this.path = Directory.Exists(path) ? path : string.Empty;
				}
				catch
				{
					this.path = string.Empty;
				}
				curDocID = 0;
				int loadedDocID = (int)Environment.General.Option("DocID").Value;
				string loadedFileName = Environment.General.Option("FileName").Value as string;

				int workFolderID = Environment.General.LoadIntOption("WorkFolderID", 0);

				int empID = 0;
				if(Environment.CurEmp != null)
					empID = Environment.General.LoadIntOption("EmpID", Environment.CurEmp.ID);

                Console.WriteLine("{0}: Registry loaded", DateTime.Now.ToString("HH:mm:ss fff"));

				if(!openDocWin)
				{
					string contextModeStr = Environment.General.Option("ContextMode").Value as string;
					var emp = new Employee(empID, Environment.EmpData);
					Context context = Context.BuildContext(contextModeStr, path, workFolderID, emp);

                    Console.WriteLine("{0}: Choosing doc and path", DateTime.Now.ToString("HH:mm:ss fff"));
					ChoosePathAndDoc(context, loadedDocID, loadedFileName);
				}
				else
				{
                    Console.WriteLine("{0}: Opening in new window", DateTime.Now.ToString("HH:mm:ss fff"));
					Environment.NewWindow(loadedDocID, zoomCombo.Text, new Context(Misc.ContextMode.Document));
				}

                Console.WriteLine("{0}: Updating Work Folders", DateTime.Now.ToString("HH:mm:ss fff"));

				//UpdateWorkContext();

				folders.UpdateFaxStatus();

                Console.WriteLine("{0}: Folders Loaded", DateTime.Now.ToString("HH:mm:ss fff"));

			}
			finally
			{
#if AdvancedLogging
                Lib.Log.Logger.LeaveMethod(this, "LoadFolders()");
#endif
			}
		}

		private bool ChoosePathAndDoc(Context context, int loadedDocID, string loadedFileName)
		{
			if(context == null)
				return false;

			bool docSoft = (loadedDocID == curDocID);
			bool fileSoft = string.Equals(fileName, loadedFileName, StringComparison.InvariantCultureIgnoreCase);
			bool result = false;

			switch(context.Mode)
			{
				case Misc.ContextMode.Catalog:
					if(folders.CatalogNode != null)
					{
						Console.WriteLine("{0}: Choosing catalog doc", DateTime.Now.ToString("HH:mm:ss fff"));
						if(loadedDocID > 0)
							result = SelectDBDoc(loadedDocID, context.Path, docSoft);
						else
							folders.SelectCatalogNode(Environment.DocData.GetFullPath(context.Path), loadedDocID);
					}
					break;

				case Misc.ContextMode.Scaner:
					if(folders.ScanerNode != null)
					{
                        Console.WriteLine("{0}: Choosing scan", DateTime.Now.ToString("HH:mm:ss fff"));
						folders.SelectScanerFolder(context.Path, folders.ScanerNode);
						result = docGrid.SelectConditional(Environment.ImageReader.FullNameField, loadedFileName, fileSoft);
					}
					break;

				case Misc.ContextMode.FaxIn:
					if(folders.FaxInNode != null)
					{
                        Console.WriteLine("{0}: Choosing fax in", DateTime.Now.ToString("HH:mm:ss fff"));
						FileInfo fi = new FileInfo(loadedFileName);
						folders.SelectFaxIn(context.ID);
						result = docGrid.SelectConditional(Environment.FaxData.FileNameField, fi.Name, fileSoft);
					}
					break;

				case Misc.ContextMode.FaxOut:
					if(folders.FaxOutNode != null)
					{
						Console.WriteLine("{0}: Choosing fax out", DateTime.Now.ToString("HH:mm:ss fff"));
						FileInfo fi = new FileInfo(loadedFileName);
                        folders.SelectFaxOut(context.ID);
						result = docGrid.SelectConditional(Environment.FaxData.FileNameField, fi.Name, fileSoft);
					}
					break;

				case Misc.ContextMode.SystemFolder:
					if(context.Path.Length > 0 && Directory.Exists(context.Path))
					{
                        Console.WriteLine("{0}: Choosing disk file", DateTime.Now.ToString("HH:mm:ss fff"));
						folders.AddSystemFolder(context.Path, loadedFileName);
						result = true;//docGrid.SelectConditional(Environment.ImageReader.FullNameField, loadedFileName, fileSoft);
					}
					break;

				case Misc.ContextMode.WorkFolder:
					if(folders.WorkFolderNode != null)
					{
						try
						{
							Console.WriteLine("{0}: choosing work folder {1}", DateTime.Now.ToLongTimeString(), context.ID);
							result = folders.SelectWorkFolder(context.ID, context.Emp, loadedDocID);

							if(result && loadedDocID > 0)
							{
								Console.WriteLine("{0}: choosing document in work folder {1}", DateTime.Now.ToLongTimeString(), loadedDocID);
								result = docGrid.SelectConditional(Environment.DocData.IDField, loadedDocID, docSoft);
							}
						}
						catch(Exception ex)
						{
							Lib.Win.Data.Env.WriteToLog(ex, "Рабочая папка" + System.Environment.NewLine + System.Environment.NewLine + "loadedDocID: " + loadedDocID);
						}
					}
					break;

				case Misc.ContextMode.SharedWorkFolder:
					if(folders.SharedWorkFolderNode != null)
					{
						try
						{
                            Console.WriteLine("{0}: Choosing shared work folder {1}", DateTime.Now.ToString("HH:mm:ss fff"), context.ID);
							folders.SelectSharedWorkFolder(context.ID, folders.SharedWorkFolderNode, loadedDocID);

							//if(loadedDocID > 0)
							//{
                            //    Console.WriteLine("{0}: Choosing document in shared work folder", DateTime.Now.ToString("HH:mm:ss fff"));
							//    result = docGrid.SelectConditional(Environment.DocData.IDField, loadedDocID, docSoft);
							//}
						}
						catch(Exception ex)
						{
							Lib.Win.Data.Env.WriteToLog(ex, "Общая папка" + System.Environment.NewLine + System.Environment.NewLine + "loadedDocID: " + loadedDocID.ToString());
						}
					}
					break;

				case Misc.ContextMode.Found:
					if(folders.FoundNode != null)
					{
                        Console.WriteLine("{0}: Choosing found doc", DateTime.Now.ToString("HH:mm:ss fff"));
						folders.SelectFoundFolder(context.ID, folders.FoundNodes(context.Emp.ID) ?? folders.FoundNode, loadedDocID);
						result = true;
						//if(loadedDocID > 0)
						//    result = docGrid.SelectConditional(Environment.DocData.IDField, loadedDocID, docSoft);
					}
					break;
				case Misc.ContextMode.Document:
					if(Environment.UserSettings.LinkDocIDs.Count > 0)
					{
                        Console.WriteLine("{0}: Choosing document", DateTime.Now.ToString("HH:mm:ss fff"));
						result = folders.SelectDocumentNode(context.ID, (context.ID == loadedDocID)?-1: loadedDocID) != null;
						//if(loadedDocID > 0 && loadedDocID != context.ID)
						//    result = docGrid.SelectConditional(Environment.DocData.IDField, loadedDocID, docSoft);
					}

					break;
			}

			return result;
		}

		private void RefreshMessageReceivers(object sender, DoWorkEventArgs e)
		{
			try
			{

				if(Environment.IsConnected && Environment.CurEmp != null)
				{
					if(Kesco.Lib.Win.Data.Settings.DS_document != Environment.ConnectionStringDocument)
						Kesco.Lib.Win.Data.Settings.DS_document = Environment.ConnectionStringDocument;
					if(Kesco.Lib.Win.Data.Settings.DS_person != Environment.ConnectionStringDocument)
						Kesco.Lib.Win.Data.Settings.DS_person = Environment.ConnectionStringDocument;
					// получатели сообщений по документам
					if(messageReceiver != null)
					{ // это надёжней чем UpdateSQL
						messageReceiver.Exit();
						messageReceiver.Received -= receiver_Received;
						messageReceiver.Dispose();
						messageReceiver = null;
					}
					messageReceiver = CreateReceiver(11, Environment.CurEmp.ID);
					if(messageReceiver != null)
					{
						messageReceiver.Received += receiver_Received;
						messageReceiver.Start();
					}
					// получатели факсов
					if(Environment.IsFaxReceiver())
					{
						if(faxInReceiver == null)
						{
							faxInReceiver = CreateReceiver(12, Environment.CurEmp.ID);
							if(faxInReceiver != null)
							{
								faxInReceiver.Received += receiver_Received;
								faxInReceiver.Start();
							}
						}
						else
							faxInReceiver.UpdateSql();
					}

					if(docControl.ConnectionStringDocument != Environment.ConnectionStringDocument)
						docControl.ConnectionStringDocument = Environment.ConnectionStringDocument;
					if(docControl.ConnectionStringAccounting != Environment.ConnectionStringAccounting)
						docControl.ConnectionStringAccounting = Environment.ConnectionStringAccounting;
				}
			}
			catch(Exception ex)
			{ Lib.Win.Data.Env.WriteToLog(ex); }
		}

		private Lib.Win.Receive.Receive CreateReceiver(int condition, int empID)
		{
			return Environment.IsConnected ? new Lib.Win.Receive.Receive(Lib.Win.Document.Environment.ConnectionStringDocument,
				Environment.SubscribeTable, condition, empID, 3) : null;
		}

        private void RefreshFolders()
        {
            if (Disposing || IsDisposed)
                return;
            CursorSleep();
            try
            {
                Context lastContext = curContext;
                int lastDocID = curDocID;
                string lastFileName = fileName;

                if (docGrid.Style != null)
                    docGrid.Style.Save();
				docGrid.SetSilent();
                docGrid.DataSource = null;

                ClearInfo();

                Slave.DoWork(RefreshMessageReceivers, null);

                Lib.Win.Document.Environment.RefreshServers();

                InitFolders();

                folders.UpdateFaxStatus();
                UpdateWorkContext();

                if (!ChoosePathAndDoc(lastContext, lastDocID, lastFileName))
                    ClearAll();

                RefreshLinks();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
            CursorWake();
        }

        private void RefreshWorkFolders()
        {
            try
            {
                if (folders.WorkFolderNode != null)
                {
                    if (folders.SelectedNode != null && folders.SelectedNode.IsWorkFolder())
                        RefreshDocs();
                    else
                        UpdateWorkContext();
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

		private void RefreshDocs()
		{
			RefreshDocs(0);
		}

        private void RefreshDocs(int curID)
        {
            if (IsDisposed || Disposing)
                return;
            try
            {
#if AdvancedLogging
                Lib.Log.Logger.EnterMethod(this, "RefreshDocs()");
#endif

                if (!docGrid.IsMultiple)
                {
					if(folders.SelectedNode.IsDocument() && docGrid.SelectedRows.Count < 1)
						return;
                    int rowIndex = docGrid.CurrentRowIndex;
					if(curID < 1)
						curID = docGrid.GetCurID();
					
#if AdvancedLogging
                    using (Lib.Log.Logger.DurationMetter("RefreshDocs folders.SelectedNode.LoadDocs(docGrid, false)"))
#endif
				
					if(curID > 0 || !string.IsNullOrEmpty(fileName))
					{
						folders.SelectedNode.LoadDocs(docGrid, false, curID, fileName);
						
					}
					else if(curID > 0 && docGrid.IsFaxes())
					{
						folders.SelectedNode.LoadDocs(docGrid, false, curID);
						docGrid.SelectByID(curID);
					}
					else
						folders.SelectedNode.LoadDocs(docGrid, false, 0);

					//if (docGrid.CurrentRowIndex == -1)
					//    docGrid.SelectRow(rowIndex);
					//else
					//    RefreshInfo(true);
                }
                else if (docGrid.IsMultiple)
                {
                    ClearInfo();
                    folders.SelectedNode.LoadDocs(docGrid, false, 0);
                }

#if AdvancedLogging
                using (Lib.Log.Logger.DurationMetter("RefreshDocs UpdateWorkContext();"))
#endif
                UpdateWorkContext();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
            finally
            {
#if AdvancedLogging
                Lib.Log.Logger.LeaveMethod(this, "RefreshDocs()");
#endif
            }
        }

        private void SelectDBDocAtPersonFolder(int docID, int personID)
        {
			SelectDBDoc(docID, FolderTree.FolderNodes.PathNodes.CatalogNodes.CatalogNode.CatalogPathInitial + FolderTree.FolderNodes.PathNodes.CatalogNodes.CatalogNode.WrapPersonID(personID));
        }

        #endregion

        #region Info

        private void SetInfoPlace()
        {
            if (Environment.General == null)
                return;

            int testParam = Environment.General.LoadIntOption("ShowInMainWindow", showMessageState);
            bool showMess = infoGrid.Visible;
            if (showMessageState != testParam)
            {
                showMessageState = testParam;

                switch (showMessageState)
                {
                    case 0:
                        if (splitContainerDoc.Panel2.Contains(infoGrid))
                        {
                            splitContainerDoc.Panel2.Controls.Remove(infoGrid);
                            splitContainerDoc.Panel2Collapsed = true;
                        }
                        if (splitContainerGrids.Panel2.Contains(infoGrid))
                        {
                            splitContainerGrids.Panel2.Controls.Remove(infoGrid);
                            splitContainerGrids.Panel2Collapsed = true;
                        }
                        if (!splitContainerList.Panel1.Contains(infoGrid))
                            splitContainerList.Panel1.Controls.Add(infoGrid);
                        splitContainerTree.Panel2MinSize = 2 * splitContainerTree.Panel1MinSize;
                        splitContainerList.Panel1Collapsed = !showMess;
                        break;
                    case 1:
                        if (splitContainerList.Panel1.Contains(infoGrid))
                        {
                            splitContainerList.Panel1.Controls.Remove(infoGrid);
                            splitContainerList.Panel1Collapsed = true;
                            splitContainerTree.Panel2MinSize = splitContainerTree.Panel1MinSize;
                        }
                        if (splitContainerDoc.Panel2.Contains(infoGrid))
                        {
                            splitContainerDoc.Panel2.Controls.Remove(infoGrid);
                            splitContainerDoc.Panel2Collapsed = true;
                        }
                        if (!splitContainerGrids.Panel2.Contains(infoGrid))
                            splitContainerGrids.Panel2.Controls.Add(infoGrid);
                        splitContainerGrids.Panel2Collapsed = !showMess;
                        break;
                    case 2:
                        if (splitContainerList.Panel1.Contains(infoGrid))
                        {
                            splitContainerList.Panel1.Controls.Remove(infoGrid);
                            splitContainerList.Panel1Collapsed = true;
                            splitContainerTree.Panel2MinSize = splitContainerTree.Panel1MinSize;
                        }
                        if (splitContainerGrids.Panel2.Contains(infoGrid))
                        {
                            splitContainerGrids.Panel2.Controls.Remove(infoGrid);
                            splitContainerGrids.Panel2Collapsed = true;
                        }
                        if (!splitContainerDoc.Panel2.Contains(infoGrid))
                            splitContainerDoc.Panel2.Controls.Add(infoGrid);
                        splitContainerDoc.Panel2Collapsed = !showMess;
                        break;
                }
            }
        }

        private void ReadTimerProcessor(object sender, EventArgs e)
        {
            readTimer.Stop();
            if (docGrid.IsSingle && IsNotRead())
                Environment.CmdManager.Commands["MarkReadMessages"].Execute();
        }

		protected override void OnActivated(EventArgs e)
		{
			using(Lib.Log.Logger.DurationMetter("MainFormDialoge OnActivated"))
			base.OnActivated(e);
		}

        private void RefreshInfo(bool forced = false)
        {
            Console.WriteLine("{0}: RefreshInfo", DateTime.Now.ToString("HH:mm:ss fff"));
			if((curDocID > 0 && curDocID != infoGrid.DocID) || forced)
			{
				InvokeIfRequired((MethodInvoker)(() => infoGrid.LoadInfo(curDocID)));
			}
        }

		private void ClearInfo()
		{
            Console.WriteLine("{0}: ClearInfo", DateTime.Now.ToString("HH:mm:ss fff"));
			InvokeIfRequired((MethodInvoker)(() => infoGrid.LoadInfo(0)));
		}

        #endregion

        #region Links

        private void RefreshLinks()
        {
            if (Disposing || IsDisposed)
                return;
            try
            {
                if (InvokeRequired)
                    BeginInvoke((MethodInvoker)(docControl.RefreshLinks));
                else
                    docControl.RefreshLinks();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        #endregion

        #region Layout

        private void splitContainerTree_ClientSizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (splitContainerTree.Height < splitContainerTree.SplitterDistance + splitContainerTree.Panel2MinSize)
                    if (splitContainerTree.SplitterDistance > splitContainerTree.Panel1MinSize)
                    {
                        if (splitContainerTree.Height - splitContainerTree.Panel2MinSize >
                            splitContainerTree.Panel1MinSize)
                            splitContainerTree.SplitterDistance = splitContainerTree.Height -
                                                                  splitContainerTree.Panel2MinSize;
                        else if (splitContainerTree.Height > splitContainerTree.Panel1MinSize)
                            splitContainerTree.SplitterDistance = splitContainerTree.Panel1MinSize;
                    }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void splitContainerList_ClientSizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (!splitContainerList.Panel1Collapsed &&
                    splitContainerList.Height < splitContainerList.SplitterDistance + splitContainerList.Panel2MinSize)
                    if (splitContainerList.SplitterDistance > splitContainerList.Panel1MinSize)
                    {
                        if (splitContainerList.Height - splitContainerList.Panel2MinSize >
                            splitContainerList.Panel1MinSize)
                            splitContainerList.SplitterDistance = splitContainerList.Height -
                                                                  splitContainerList.Panel2MinSize;
                        else if (splitContainerList.Height > splitContainerList.Panel1MinSize)
                            splitContainerList.SplitterDistance = splitContainerList.Panel1MinSize;
                    }
                if (splitContainerList.Panel1.ClientSize.Height == splitContainerList.Panel1MinSize)
                {// чтобы обновился docGrid
                    splitContainerList.SplitterDistance++;
                }
                if (splitContainerList.Panel2.ClientSize.Height == splitContainerList.Panel2MinSize)
                {
                    splitContainerList.SplitterDistance--;
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void splitContainerMain_ClientSizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (splitContainerMain.Width < splitContainerMain.SplitterDistance + splitContainerMain.Panel2MinSize)
                    if (splitContainerMain.SplitterDistance > splitContainerMain.Panel1MinSize)
                    {
                        if (splitContainerMain.Width - splitContainerMain.Panel2MinSize >
                            splitContainerMain.Panel1MinSize)
                            splitContainerMain.SplitterDistance = splitContainerMain.Width -
                                                                  splitContainerMain.Panel2MinSize;
                        else if (splitContainerMain.Width > splitContainerMain.Panel1MinSize)
                            splitContainerMain.SplitterDistance = splitContainerMain.Panel1MinSize;
                    }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void splitContainerGrids_ClientSizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (!splitContainerGrids.Panel2Collapsed &&
                    splitContainerGrids.Width < splitContainerGrids.SplitterDistance + splitContainerGrids.Panel2MinSize)
                    if (splitContainerGrids.SplitterDistance > splitContainerGrids.Panel1MinSize)
                    {
                        if (splitContainerGrids.Width - splitContainerGrids.Panel2MinSize >
                            splitContainerGrids.Panel1MinSize)
                            splitContainerGrids.SplitterDistance = splitContainerGrids.Width -
                                                                   splitContainerGrids.Panel2MinSize;
                        else if (splitContainerGrids.Width > splitContainerGrids.Panel1MinSize)
                            splitContainerGrids.SplitterDistance = splitContainerGrids.Panel1MinSize;
                    }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void splitContainerDoc_ClientSizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (!splitContainerDoc.Panel2Collapsed &&
                    splitContainerDoc.Height < splitContainerDoc.SplitterDistance + splitContainerDoc.Panel2MinSize)
                    if (splitContainerDoc.SplitterDistance > splitContainerDoc.Panel1MinSize)
                    {
                        if (splitContainerDoc.Height - splitContainerDoc.Panel2MinSize > splitContainerDoc.Panel1MinSize)
                            splitContainerDoc.SplitterDistance = splitContainerDoc.Height -
                                                                 splitContainerDoc.Panel2MinSize;
                        else if (splitContainerDoc.Height > splitContainerDoc.Panel1MinSize)
                            splitContainerDoc.SplitterDistance = splitContainerDoc.Panel1MinSize;

                    }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        #endregion

        #region MainFormDialog

        public void docComponent_DocumentSaved(object sender, DocumentSavedEventArgs e)
        {
            try
            {
                if (docControl != null)
                    docControl.OnDocSaved(sender, e);
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

		private void MainFormDialog_Load(object sender, EventArgs e)
		{
			if(Program.splash != null)
			{
				Program.splash.Close();
				Program.splash = null;
			}
			ClearAll();
			try
			{
				bCheck = ButtonCheck.MoveImg;

				// установка имени пользователя
				docControl.EmpName = Environment.UserName;

				// получатели сообщений
				Slave.DoWork(RefreshMessageReceivers, null);

				// кнопка отправки факсов/почты
				ToolStripButton button = sendFaxButton;
				button.Visible = Environment.IsFaxSender();
				menuSendFax.Visible = button.Visible;
				menuItemSaveSelectedAsStamp.Visible = Lib.Win.Document.Environment.IsDomainAdmin();
				Lib.Win.Options.Folder root = new Lib.Win.Options.Root();
				//Environment.ConnectionStringErrors = root.OptionForced<string>("DS_errors").GetValue<string>();
				//opinionControl.AppID = "FL";
				//opinionControl.EmpID = Environment.CurEmp.ID;
				//opinionControl.ConnectionString = Environment.ConnectionStringErrors;

				// отображение панели страниц
				if(!toTray)
				{
					Environment.Layout.LoadStringOption("ShowPagesPanel", true.ToString(), ShowPagesPanel);
					docControl.ShowWebPanel = Convert.ToBoolean(Environment.Layout.LoadStringOption("ShowWebPanel", docControl.ShowWebPanel.ToString()));
				}

				// отображение панели заметок
				annotationBar.Visible = false;
				Environment.Layout.LoadStringOption("ShowNoteBar", annotationBar.Visible.ToString(), ShowNoteBar);

				// координата X окна
				Left = Environment.Layout.LoadIntOption("WindowX", Left);

				// координата Y окна
				Top = Environment.Layout.LoadIntOption("WindowY", Top);

				// высота окна
				Height = Environment.Layout.LoadIntOption("WindowHeight", Height);

				// ширина окна
				Width = Environment.Layout.LoadIntOption("WindowWidth", Width);

				// полный экран или нет
				bool maximized = (WindowState == FormWindowState.Maximized);
				object optValue = Environment.Layout.LoadStringOption("FullScreen", maximized.ToString());
				lastGoodState = Convert.ToBoolean(optValue) ? FormWindowState.Maximized : FormWindowState.Normal;

				if(WindowState != FormWindowState.Minimized)
					Lib.Win.Document.Environment.FormRectangle = this.Bounds;
				if(!toTray)
					WindowState = lastGoodState;


				optValue = Environment.Layout.LoadStringOption("InfoGridVisible", infoGrid.Visible.ToString());
				infoGrid.Visible = Convert.ToBoolean(optValue);

				Environment.LoadMessageFirst = Convert.ToBoolean(Environment.Layout.LoadStringOption("LoadMessageFirst", false.ToString()));

				Environment.PersonMessage = Convert.ToBoolean(Environment.General.LoadStringOption("PersonMessage", true.ToString()));

				// высота дерева папок
				splitContainerTree.SplitterDistance = Environment.Layout.LoadIntOption("FoldersHeight", splitContainerTree.SplitterDistance);

				// высота информационного блока
				splitContainerList.SplitterDistance = Environment.Layout.LoadIntOption("DocGridHeight", splitContainerList.SplitterDistance);

				// ширина левой панели
				splitContainerMain.SplitterDistance = Environment.Layout.LoadIntOption("LeftPanelWidth", splitContainerMain.SplitterDistance);

				// ширина отображения панели сообщений в режиме слева от дерева
				splitContainerGrids.SplitterDistance = Environment.Layout.LoadIntOption("InfoGridWidth", splitContainerGrids.SplitterDistance);

				// ширина отображения панели сообщений в режиме под документом
				splitContainerDoc.SplitterDistance = Environment.Layout.LoadIntOption("InfoGridHeight", splitContainerDoc.SplitterDistance);

				int spltx = Environment.Layout.LoadIntOption("SplitterX", docControl.SplinterPlace.X);
				int splty = Environment.Layout.LoadIntOption("SplitterY", docControl.SplinterPlace.Y);
				if(spltx < 32)
					spltx = 32;
				if(splty < 32)
					splty = 32;
				docControl.SplinterPlace = new Point(spltx, splty);

				// загружаем настройки пользователя
				if(Environment.IsConnected)
				{
					Environment.UserSettings.Load();
					Lib.Win.Document.Environment.PersonID = Environment.UserSettings.PersonID;
				}

				statusBarPanelArchive.Text = Environment.CompanyName;
				Text += " " + statusBarPanelArchive.Text;

				string argStr = Program.BuildArgsString(Program.arguments);

				HideForm.AnalyzeArgs(argStr);
				if(!toTray)
					LoadFolders();
				else
					InitFolders();

				SendMessageDialog.MessageSend += SendMessageDialog_MessageSend;

				stateLoaded = true;
				if(toTray)
					Hide();
				else if(Environment.IsConnected && Environment.UserSettings.ShowNews)
				{
					changesDialog = new Dialogs.ChangesDialog { Changes = true, Owner = this };
					changesDialog.Show();
				}

				//this.ActiveControl = folders;

				menuSettingsLanguage.Enabled = Environment.IsConnected;

                Console.WriteLine("{0}: Form Loaded", DateTime.Now.ToString("HH:mm:ss fff"));
				SendMessageDialog.NeedSendWindow += SendMessageDialog_NeedSendWindow;
				System.Timers.Timer udcTimer = new System.Timers.Timer(2000);
				udcTimer.AutoReset = false;
				udcTimer.Elapsed += new ElapsedEventHandler(udcTimer_Elapsed);
				udcTimer.Start();
				int workerThreads = 0;
				int avalibleThreads = 0;
				ThreadPool.GetMaxThreads(out workerThreads, out avalibleThreads);
				if(workerThreads < 20)
					Lib.Win.Data.Env.WriteToLog("Малое число доступных потоков - " + workerThreads.ToString());
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		void udcTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
            Console.WriteLine("{0}: udcTimer_Elapsed", DateTime.Now.ToString("HH:mm:ss fff"));
			System.Timers.Timer udcTimer = sender as System.Timers.Timer;
			if(udcTimer != null)
			{
				udcTimer.Stop();
				udcTimer.Elapsed -= udcTimer_Elapsed;
				udcTimer.Dispose();
				udcTimer = null;
			}
			if(this.InvokeRequired)
				this.BeginInvoke(new Action(delegate()
					{
						if(Lib.Win.Document.Checkers.TestPrinter.CheckPrinterExists())
						{
							string path = Lib.Win.Document.Environment.PrinterPath;
						}
					}));
			else if(Lib.Win.Document.Checkers.TestPrinter.CheckPrinterExists())
			{
				string path = Lib.Win.Document.Environment.PrinterPath;
			}
			// отключаем звук в браузере
			try
			{
				Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_DISABLE_NAVIGATION_SOUNDS")
					.SetValue(Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName), 1, Microsoft.Win32.RegistryValueKind.DWord);
			}
			catch
			{
			}
		}

        private void SaveState()
        {
            if (!stateLoaded)
                return;
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)(SaveState));
                return;
            }
            try
            {
                SaveFile();

                // сохраняем стили датагрида
                docGrid.Style.Save();

                // сохраняем стили инфогрида
                infoGrid.Style.Save();

                //сохраняем внешний вид окна
                SaveFormState();

                // настройки файла
                Environment.General.OptionForced<string>("FileName").Value = fileName ?? "";

                Environment.General.OptionForced<int>("DocID").Value = curDocID;
                Environment.General.OptionForced<int>("ImageID").Value = curImageID;

                Context context = folders.GetContext();

                if (context != null)
                {
                    Environment.General.OptionForced<string>("ContextMode").Value = context.Mode.ToString();
                    // forced temporary?
                    Environment.General.OptionForced<string>("Path").Value = context.Path;
                    Environment.General.OptionForced<int>("WorkFolderID").Value = context.ID;
                    if (context.Emp != null)
                        Environment.General.OptionForced<int>("EmpID").Value = context.Emp.ID;
                }

                try
                {
                    if (docControl.ImageDisplayed)
                    {
                        Environment.General.OptionForced<int>("Page").Value = docControl.Page;
                        Environment.General.OptionForced<int>("ScrollPositionX").Value = docControl.ScrollPositionX;
                        Environment.General.OptionForced<int>("ScrollPositionY").Value = docControl.ScrollPositionY;
                    }
                }
                catch (Exception ex)
                {
                    Lib.Win.Data.Env.WriteToLog(ex);
                }

                Environment.Settings.Save();

                // сохраняем настройки пользователя
                if (Environment.IsConnected && Environment.UserSettings.NeedSave)
                    Environment.UserSettings.Save();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

		private void SaveFormState()
		{
			if(InvokeRequired)
			{
				this.Invoke((MethodInvoker)(SaveFormState));
				return;
			}
			if(WindowState == FormWindowState.Minimized || !Visible || docControl == null || !docControl.CompliteLoading)
				return;
			try
			{
				// сохраняем полный или не полный экран
				bool maximized = (WindowState == FormWindowState.Maximized);
				if(Environment.Layout != null)
				{
					Environment.Layout.OptionForced<string>("FullScreen").Value = maximized.ToString();

					// сохраняем видимость сообщений
					Environment.Layout.OptionForced<string>("InfoGridVisible").Value = infoGrid.Visible.ToString();

					// сохраняем масштаб
					Environment.Layout.OptionForced<string>("Zoom").Value = zoomCombo.Text;

					//сохраняем порядок загрузки документа
					Environment.Layout.OptionForced<string>("LoadMessageFirst").Value = Environment.LoadMessageFirst.ToString();

					Environment.Layout.OptionForced<int>("SplitterX").Value = docControl.SplinterPlace.X;
					Environment.Layout.OptionForced<int>("SplitterY").Value = docControl.SplinterPlace.Y;

					// высота дерева папок
					Environment.Layout.OptionForced<int>("FoldersHeight").Value = splitContainerTree.SplitterDistance;

					// высота информационного блока
					Environment.Layout.OptionForced<int>("DocGridHeight").Value = splitContainerList.SplitterDistance;

					// ширина левой панели
					Environment.Layout.OptionForced<int>("LeftPanelWidth").Value = splitContainerMain.SplitterDistance;

					// ширина отображения панели сообщений в режиме слева от дерева
					Environment.Layout.OptionForced<int>("InfoGridWidth").Value = splitContainerGrids.SplitterDistance;

					// ширина отображения панели сообщений в режиме под документом
					Environment.Layout.OptionForced<int>("InfoGridHeight").Value = splitContainerDoc.SplitterDistance;

					Environment.Layout.Save();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void MainFormDialog_LocationChanged(object sender, EventArgs e)
		{
			if(InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() => MainFormDialog_LocationChanged(sender, e)));
				return;
			}
			try
			{
				bool normal = (WindowState == FormWindowState.Normal);

				if(normal && Environment.Layout != null)
				{
					if(Environment.Layout.Option("WindowX") == null)
						// координаты окна : X
						Left = Environment.Layout.LoadIntOption("WindowX", Left);
					else
						Environment.Layout.Option("WindowX").Value = Left;

					if(Environment.Layout.Option("WindowY") == null)
						// координаты окна : Y
						Top = Environment.Layout.LoadIntOption("WindowY", Top);
					else
						Environment.Layout.Option("WindowY").Value = Top;
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			SaveFormState();
		}

		private void MainFormDialog_SizeChanged(object sender, EventArgs e)
		{
			if(InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() => MainFormDialog_SizeChanged(sender, e)));
				return;
			}
			try
			{
				if(WindowState != FormWindowState.Minimized)
					lastGoodState = WindowState;

				bool normal = (WindowState == FormWindowState.Normal);

				if(normal && Environment.Layout != null)
				{
					if(Environment.Layout.Option("WindowWidth") == null)
						Width = Environment.Layout.LoadIntOption("WindowWidth", Width);
					else
						Environment.Layout.Option("WindowWidth").Value = Width;

					if(Environment.Layout.Option("WindowHeight") == null)
						// размер окна : Y
						Height = Environment.Layout.LoadIntOption("WindowHeight", Height);
					else
						Environment.Layout.Option("WindowHeight").Value = Height;
					if(WindowState != FormWindowState.Minimized)
						Lib.Win.Document.Environment.FormRectangle = this.Bounds;
				}
				RefreshStatusBar();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			SaveFormState();
		}

        private void MainFormDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (keyLocker.Contains(e.KeyData))
                return;
            keyLocker.Add(e.KeyData);
            try
            {
				switch(e.KeyData)
				{
					case Keys.Subtract:
						Environment.CmdManager.Commands["ZoomOut"].ExecuteIfEnabled();
						break;

					case Keys.Add:
						Environment.CmdManager.Commands["ZoomIn"].ExecuteIfEnabled();
						break;

					case Keys.Right | Keys.Control:
						Environment.CmdManager.Commands["VariantNext"].ExecuteIfEnabled();
						break;

					case Keys.Left | Keys.Control:
						Environment.CmdManager.Commands["VariantPrevious"].ExecuteIfEnabled();
						break;

					case Keys.F1:
						Environment.CmdManager.Commands["Help"].Execute();
						break;

					case Keys.F12:
						Environment.CmdManager.Commands["OpenFolder"].Execute();
						break;

					case Keys.Right | Keys.Alt:
						if(docGrid.IsDBDocs())
						{
							if(Environment.UserSettings.GroupOrder.Contains(FolderTree.FolderNodes.PathNodes.CatalogNodes.CatalogNode.PersonInitial))
							{
								var regex = new Regex(FolderTree.FolderNodes.PathNodes.CatalogNodes.CatalogNode.PersonInitial + @":(?<person>\d+)");
								Match m = regex.Match(folders.GetContext().Path);

								if(m.Success)
								{
									try
									{
										int curPersonID = Int32.Parse(m.Groups["person"].Value);
										int index = -1;

										using(DataTable personTable = Environment.DocData.GetDocPersonsLite(curDocID, false))
										{
											int personCount = personTable.Rows.Count;

											for(int i = 0; i < personCount; i++)
											{
												DataRow row = personTable.Rows[i];
												var personID = (int)row[Environment.PersonData.IDField];
												if(personID == curPersonID)
												{
													index = i;
													break;
												}
											}

											if(index != -1)
											{
												index = (index + 1) % personCount;

												DataRow row = personTable.Rows[index];
												var personID = (int)row[Environment.PersonData.IDField];

												SelectDBDocAtPersonFolder(curDocID, personID);
											}
											else
												ErrorMessage(
													Environment.StringResources.GetString(
														"MainForm.MainFormDialog.MainFormDialog_KeyDown.Message1") +
													System.Environment.NewLine +
													Environment.StringResources.GetString(
														"MainForm.MainFormDialog.MainFormDialog_KeyDown.Message2") +
													curPersonID,
													Environment.StringResources.GetString("Error"));

											personTable.Dispose();
										}
									}
									catch(Exception ex)
									{
										Lib.Win.Data.Env.WriteToLog(ex);
									}
								}
							}
						}
						break;

					case Keys.Z | Keys.Control:
						Environment.UndoredoStack.Undo(1);
						break;
#if(DEBUG)

					case Keys.F10:
						ShowMailingList();
						break;
#endif
					case Keys.F11:
						var style = docGrid.Style;
						if(style != null && style.IsShownInSettings())
						{
							style.Save();
							Settings.SettingsColumnsDialog dialog = new Settings.SettingsColumnsDialog(style, docGrid.OptionFolder, style);
							dialog.FormClosed += SettingsColumnsDialog_FormClosed;
							dialog.Show();
						}
						break;

					case Keys.Space:
						e.Handled = false;
						break;
				}
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex, string.Format("KeyData {0}", e.KeyData));
            }
            finally
            {
                keyLocker.Remove(e.KeyData);
            }
        }

        private void MainFormDialog_KeyUp(object sender, KeyEventArgs e)
        {
            if (keyLocker.Contains(e.KeyData))
                return;
            keyLocker.Add(e.KeyData);
            try
            {
                if (bCheck == ButtonCheck.No || bCheck == ButtonCheck.MoveImg || bCheck == ButtonCheck.SelectAnn ||
                    bCheck == ButtonCheck.ImageStamp)
                {
                    switch (e.KeyData)
                    {
                        case Keys.Left:
                            Environment.CmdManager.Commands["PageBack"].ExecuteIfEnabled();
                            break;

                        case Keys.Right:
                            Environment.CmdManager.Commands["PageForward"].ExecuteIfEnabled();
                            break;

                        case Keys.Escape:
                            Environment.CmdManager.Commands["View"].ExecuteIfEnabled();
                            break;
                    }
                }
                docControl.Invalidate();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex, string.Format("KeyData {0}", e.KeyData));
            }
            finally
            {
                keyLocker.Remove(e.KeyData);
            }
        }

        #endregion

        #region MainMenu

        void menuSettingsLinkShow_Click(object sender, EventArgs e)
        {
            Settings.SettingsLinkShowDialog sd = new Settings.SettingsLinkShowDialog();
            sd.ShowDialog();
        }

        private void menuPrintAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (docControl.CanPrint)
                    if (docControl.ImageDisplayed)
                        docControl.Print();
                    else
                        docControl.PrintEForm();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void menuPrintSelection_Click(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired)
                    BeginInvoke((MethodInvoker)(docControl.PrintSelection));
                else
                    docControl.PrintSelection();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void Update_MenuLinkOpenDoc()
        {
            menuLinkOpenDoc.MenuItems.Clear();

            foreach (KeyValuePair<int, Form> t in Environment.OpenDocs.Where(t => t.Key != curDocID && !Environment.DocLinksData.HasExistsDocLink(t.Key, curDocID)))
            {
                var item = new MenuItem { Text = DBDocString.Format(t.Key) };
                item.Click += LinkOpenDoc_Click;

                menuLinkOpenDoc.MenuItems.Add(item);
            }

            menuLinkOpenDoc.Enabled = (menuLinkOpenDoc.MenuItems.Count > 0);
        }

        private void menuNewLink_Popup(object sender, EventArgs e)
        {
            if (docGrid.IsSingle && curDocID > 0)
                Update_MenuLinkOpenDoc();
            else
            {
                menuLinkOpenDoc.MenuItems.Clear();
                menuLinkOpenDoc.Enabled = false;
            }
        }

        private void menuDoc_Popup(object sender, EventArgs e)
        {
            if (Disposing || IsDisposed)
                return;
            try
            {
                bool docSelected = IsSelectedSingle();

                menuSave.Visible = docSelected && !docGrid.IsDBDocs();
                menuSavePart.Visible = docSelected;
                menuSeparator1.Visible = menuSave.Visible || menuSavePart.Visible;

                menuAddEForm.Visible = docSelected && docGrid.IsDBDocs();
                menuSeparator2.Visible = menuAddEForm.Visible;
                menuScanCurrentDoc.Visible = docSelected && docGrid.IsDBDocs();
                menuAddImageCurrentDoc.Visible = docSelected && docGrid.IsDBDocs();
				menuLinkEform.Visible = docSelected && docGrid.IsDBDocs();
				menuLinkEform.MenuItems.Clear();
                bool inWork = docGrid.IsInWork();

                menuAddToWork.Visible = docSelected && docGrid.IsDBDocs() && !inWork;
                menuWorkPlaces.Visible = docSelected && docGrid.IsDBDocs() && inWork;
                menuEndWork.Visible = docSelected && docGrid.IsDBDocs() && inWork;
                menuSeparator3.Visible = menuAddToWork.Visible || menuWorkPlaces.Visible || menuEndWork.Visible;

                menuFaxDescr.Visible = docSelected && docGrid.IsFaxesIn();
                menuSpam.Visible = docSelected && docGrid.IsFaxesIn();
                menuSeparator4.Visible = menuFaxDescr.Visible || menuSpam.Visible;

                menuDocPropertes.Visible = docSelected;
                menuSeparator5.Visible = menuDocPropertes.Visible;

                menuGotoPerson.Visible = docSelected && docGrid.IsDBDocs();
                menuNewWindow.Visible = docSelected;
                menuSeparator6.Visible = menuGotoPerson.Visible || menuNewWindow.Visible;

                menuDelete.Visible = docSelected && docGrid.IsDBDocs();
                menuDeleteFromFound.Visible = docSelected && docGrid.IsFound();
                menuDeletePart.Visible = docSelected && (docGrid.IsScaner() || docGrid.IsDBDocs());

                menuSeparator7.Visible = menuDelete.Visible || menuDeleteFromFound.Visible || menuDeletePart.Visible;

                if (docSelected && docGrid.CurrentCell != null)
                {
                    if (docGrid.IsDBDocs())
                    {
                        // Заявка 27393
                        // FIX ошибки nullReferenceException
                        int docTypeID = -1;
                        var obj = docGrid.GetCurValue(Environment.DocData.DocTypeIDField);
                        if(obj is int)
                            docTypeID = (int) obj;

                        menuAddEForm.Enabled = !Environment.DocDataData.IsDataPresent(curDocID) &&
                                               Environment.DocTypeData.GetDocBoolField(
                                                   Environment.DocTypeData.FormPresentField, docTypeID);

                        menuDelete.Enabled = Environment.EmpData.IsDocDeleter() ||
                                             !Environment.DocImageData.DocHasImages(curDocID, true);
                        menuDeletePart.Enabled = IsDisplayedSingle() && (docControl.PageCount > 0) &&
                                                 Environment.EmpData.IsDocDeleter();
						DataRow dr;
						using(DataTable typesTable = Environment.DocTypeLinkData.GetLinkedTypes(docTypeID))
							for(int j = 0; j < typesTable.Rows.Count; j++)
							{
								dr = typesTable.Rows[j];
								Items.IDMenuItem it = new Items.IDMenuItem((int)dr[Environment.DocTypeLinkData.ChildTypeIDFeild]);
								it.Text = dr[Environment.DocTypeLinkData.NameField] +
									   ((DBNull.Value.Equals(typesTable.Rows[j][Environment.FieldData.NameField]) ? ""
											 : ("(" + Environment.StringResources.GetString("OnField") + " " +
											   dr[Environment.FieldData.NameField]) + ")"));
								it.Tag = typesTable.Rows[j];
								it.Click += linkEFormItem_Click;
								menuLinkEform.MenuItems.Add(it);
							}
						

						menuLinkEform.Enabled = (menuLinkEform.MenuItems.Count > 0);
                    }

                    if (docGrid.IsFaxesIn())
                        menuSpam.Checked = docGrid.GetBoolValue(Environment.FaxInData.SpamField);

                    if (docGrid.IsFaxesOut())
                    {
                        menuSave.Enabled = !Environment.FaxOutData.FaxHasDocImage(curFaxID); // no image id set
                        menuSavePart.Enabled = (menuSave.Enabled && docControl.PageCount > 1);
                    }

                    if (docGrid.IsScaner())
                        menuDeletePart.Enabled = IsDisplayedSingle() && (docControl.PageCount > 1);
                }

                if (menuGotoPerson.Visible)
                {
                    // goto person
                    menuGotoPerson.MenuItems.Clear();

                    // есть ли в группировке лица?
                    if (Environment.UserSettings.GroupOrder.Contains(FolderTree.FolderNodes.PathNodes.CatalogNodes.CatalogNode.PersonInitial))
                    {
                        using (DataTable personTable = Environment.DocData.GetDocPersonsLite(curDocID, false))
                        using (DataTableReader dr = personTable.CreateDataReader())
                        {
                            while (dr.Read())
                            {
                                var personID = (int)dr[Environment.PersonData.IDField];
                                if (Environment.UserSettings.PersonID != personID || personTable.Rows.Count == 1)
                                {
                                    var item = new Items.IDMenuItem(personID)
                                                   {
                                                       Text = dr[Environment.PersonData.NameField] as
                                                           string ??
                                                           Environment.StringResources.GetString(
                                                           "MainForm.MainFormDialog.menuDoc_Popup.Message1")
                                                   };

                                    item.Click += toPerson_Click;
                                    menuGotoPerson.MenuItems.Add(item);
                                }
                            }
                            dr.Close();
                            dr.Dispose();
                            personTable.Dispose();
                        }
                    }

                    menuGotoPerson.Enabled = (menuGotoPerson.MenuItems.Count > 0);
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void toPerson_Click(object sender, EventArgs e)
        {
            var item = sender as Items.IDMenuItem;
            if (item != null)
            {
                nextPersonID = item.ID;
                Environment.CmdManager.Commands["SelectPersonFolder"].Execute();
            }
        }

        #endregion

        #region Notify

        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (!Visible)
                notifyIcon.ContextMenu = notifyMenu;
        }

        private void mIClose_Click(object sender, EventArgs e)
        {
            ExitApp(true);
        }

        private void mIShowWindow_Click(object sender, EventArgs e)
        {
            GetFromTray();
        }

        public void GetFromTray()
        {
            if (Disposing || IsDisposed)
                return;
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(GetFromTray));
                return;
            }
            try
            {
                Console.WriteLine("{0}: GetFromTray", DateTime.Now.ToString("HH:mm:ss fff"));
                if (!Visible)
                {
                    Show();
                    ShowInTaskbar = true;
                    if (WindowState == FormWindowState.Minimized)
                        WindowState = lastGoodState;
                    BringToFront();
                    if (toTray)
                    {
                        LoadFolders();
                        Environment.Layout.LoadStringOption("ShowPagesPanel", docControl.ShowThumbPanel.ToString(),
                                                      ShowPagesPanel);
                        docControl.ShowWebPanel =
                            Environment.Layout.LoadStringOption("ShowWebPanel", docControl.ShowWebPanel.ToString()).
                                Equals(true.ToString());
                        if (Environment.UserSettings.ShowNews)
                        {
                            changesDialog = new Dialogs.ChangesDialog();
                            changesDialog.Show();
                        }
                        toTray = false;
                    }
                }
                else
                {
                    Show();
                    ShowInTaskbar = true;
                    WindowState = lastGoodState;
                    int lwprid = 0;
                    Kesco.Lib.Win.Document.Win32.User32.GetWindowThreadProcessId(Handle, ref lwprid);
                    if (lwprid > 0)
                    {
                        Kesco.Lib.Win.Document.Win32.User32.AllowSetForegroundWindow(lwprid);
                        Kesco.Lib.Win.Document.Win32.User32.SetForegroundWindow(Handle);
                    }
                    BringToFront();
                }

              //  Activate();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void notifyMenu_Popup(object sender, EventArgs e)
        {
            mIShowWindow.Enabled = !Visible;
        }

        #endregion

        #region Pages

        private void pageNum_TextChanged(object sender, EventArgs e)
        {
            pageNum.Text = pageNum.Text.Trim();
            int page;
            if (Int32.TryParse(pageNum.Text, out page) && page > 0)
            {
                try
                {
                    if (docControl.PageCount > 0 && page <= docControl.PageCount)
                    {
                        int oldPage = docControl.Page;
                        if (page != oldPage)
                        {
                            SaveFile();
                            docControl.Page = page;
                            if (docControl.Page != page)
                            {
                                docControl.Page = oldPage;
                                pageNum.Text = oldPage.ToString();
                                pageNum.SelectionStart = pageNum.Text.Length;
                            }
                        }
                        StatusBar_UpdatePage();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Lib.Win.Data.Env.WriteToLog(ex);
                }
            }

            if (!string.IsNullOrEmpty(pageNum.Text) && docControl.ImageDisplayed && docControl.Page > 0)
            {
                pageNum.Text = docControl.Page.ToString();
                pageNum.SelectionStart = pageNum.Text.Length;
            }
        }

        #endregion

        #region Pages Panel

        private void menuShowPagesPanel_Click(object sender, EventArgs e)
        {
            menuShowPagesPanel.Checked = !menuShowPagesPanel.Checked;

            IOption opt = Environment.Layout.Option("ShowPagesPanel");
            opt.Value = menuShowPagesPanel.Checked.ToString();
        }

		private void ShowPagesPanel(object sender, OptionEventArgs e)
		{
			if(InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() => ShowPagesPanel(sender, e)));
				return;
			}
			ToolStripButton button = showPagesPanelButton;

			if(button != null)
			{
				bool val = Convert.ToBoolean(e.Value);
				button.Checked = val;
				menuShowPagesPanel.Checked = val;
				docControl.ShowThumbPanel = val;
			}

			UpdateZoom();
		}

        #endregion

        #region Recievers

		private void receiver_Received(string rStr, int parameter)
		{
			if(Disposing || IsDisposed)
				return;
			try
			{
                Console.WriteLine("{0}: Received : {1} with param: {2}", DateTime.Now.ToString("HH:mm:ss fff"), rStr, parameter);
				string[] receivedMessage = string.IsNullOrEmpty(rStr) ? null : rStr.Split('|');

				if(receivedMessage == null || receivedMessage.Length <= 1)
				{
					Lib.Win.Data.Env.WriteToLog(string.Format("Didn't deconstruct the message: {0} with parameter: {1}", rStr, parameter));
					return;
				}

#if AdvancedLogging
                Lib.Log.Logger.Message("MainFormDialog receiver_Received  rStr = " + rStr);
#endif

				switch(receivedMessage[0])
				{
					case "11": // new message on document or folder
						BeginInvoke((MethodInvoker)(() => GotMessage(receivedMessage)));
						break;
					case "12": // new fax incoming
						BeginInvoke((MethodInvoker)(() => GotFax(receivedMessage[1])));
						break;
					case "14":
						if(receivedMessage[1].ToLower().Contains("e") &&
							Environment.UnSubscribeData.CheckIt(docControl.Subscribe))
						{
                            Console.WriteLine("{0}: e", DateTime.Now.ToString("HH:mm:ss fff"));
							return;
						}
						if(_ignoreDocChanges.Contains(receivedMessage[1]))
						{
							_ignoreDocChanges.Remove(receivedMessage[1]);
							_ignoreDocChanges.Add(receivedMessage[1]);
						}
						else
						{
							_ignoreDocChanges.Add(receivedMessage[1]);
							BeginInvoke((MethodInvoker)(() => GotDocumentChange(receivedMessage[1])));
						}
						break;
					default:
						Lib.Win.Data.Env.WriteExtExToLog("receiver error", string.Format("Didn't understand the message: {0} with parameter: {1}", rStr, parameter),
									   Assembly.GetExecutingAssembly().GetName(), MethodBase.GetCurrentMethod());
						return;
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(string.Format("Error on recieve the message: {0} with parameter: {1}", rStr, parameter));
			}
		}

		private void gotMessageTimerProcessor(object sender, EventArgs e)
		{
			if(Disposing || IsDisposed)
				return;

#if AdvancedLogging
            Lib.Log.Logger.Message("MainFormDialog rgotMessageTimerProcessor");
#endif

			if(gotMessageTimer == null)
			{
				gotMessageTimer = new System.Timers.Timer { Interval = gotMessageTimeout };
				gotMessageTimer.AutoReset = false;
				gotMessageTimer.Elapsed += gotMessageTimerProcessor;
			}
			else
				gotMessageTimer.Stop();

			Environment.SaveActiveForm();

			int code = 0;
			lock(_gotUpdatedDocsCodeLock)
			{
				code = _gotUpdatedDocsCode;
				_gotUpdatedDocsCode = 0;
			}

			try
			{
				if(this.InvokeRequired)
					this.BeginInvoke((MethodInvoker)(delegate
					{
						MessageWork(code);
					}));
				else
					MessageWork(code);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			finally
			{
				Environment.RestoreActiveForm();
				Environment.ShowHiddenMessageWindows();
			}
		}

		private void MessageWork(int code)
		{
		    try
		    {
#if AdvancedLogging
                Lib.Log.Logger.EnterMethod(this, "MessageWork code=" + code);
#endif

			    switch(code)
			    {
				    case 0:
					    return;
				    case 1:
#if AdvancedLogging
                        using (Lib.Log.Logger.DurationMetter("AnalyzeCurrentCell()"))
#endif
					    AnalyzeCurrentCell();

                        BeginUpdateWorkContextAsync(false);
					    break;
				    case 2:
					    if(folders.SelectedNode is FolderTree.FolderNodes.WorkNodes.WorkFolderNode)
#if AdvancedLogging
                            using (Lib.Log.Logger.DurationMetter("RefreshDocs()"))
#endif
						    RefreshDocs();
					    else
					    {
						    var fNode = folders.SelectedNode as FoundNode;
						    if(fNode != null && fNode.XML != null &&
							    (Options.HasOption(fNode.XML, typeof(Lib.Win.Data.DALC.Documents.Search.Document.ВРаботе)) ||
							     Options.HasOption(fNode.XML, typeof(Lib.Win.Data.DALC.Documents.Search.Message.Incoming.Incoming_Unread))))
#if AdvancedLogging
                                using (Lib.Log.Logger.DurationMetter("RefreshDocs()"))
#endif
							    RefreshDocs();
						    else
                                BeginUpdateWorkContextAsync(true);
					    }
					    break;
				    case 3:
					    Environment.Refresh();
					    break;
			    }
            }
            finally
            {
#if AdvancedLogging
                Lib.Log.Logger.LeaveMethod(this, "MessageWork code=" + code);
#endif
            }
		}

        public void GotMessage(string[] receivedString)
        {
            if (receivedString == null)
                return;

#if AdvancedLogging
            var m = string.Empty;

            if (receivedString.Length > 0)
            {
                foreach (var s in receivedString)
                    m += s + "|";
            }

            Lib.Log.Logger.Message("MainFormDialog GotMessage receivedString= " + m);
#endif

            int empID;
            int folderID;
            int docID;

            if (gotMessageTimer == null)
            {
                gotMessageTimer = new System.Timers.Timer(gotMessageTimeout);
				gotMessageTimer.AutoReset = false;
                gotMessageTimer.Elapsed += gotMessageTimerProcessor;
            }
            gotMessageTimer.Stop();

            int code = _gotUpdatedDocsCode;
            //0 - ничего не делать
            //1 - обновить счётчики папок в работе
            //2 - перезагрузить текущий список документов
            //3 - перестроить всё дерево и обновить список документов
            try
            {
                switch (receivedString[0])
                {
                    case "11":
                        switch (receivedString.Length)
                        {
                            case 2:
                                if (receivedString[0].Equals("11") && receivedString[1].Equals("0"))
                                    code = Math.Max(code, 2);
                                else
                                    Lib.Win.Data.Env.WriteToLog(string.Join("|", receivedString));
                                break;
                            case 4:
                                if (!int.TryParse(receivedString[1], out empID))
                                    return;
                                if (!int.TryParse(receivedString[3], out docID))
                                    return;

                                if (docID > 0)
                                {
                                    GotMessage(new string[] { "11", receivedString[1], receivedString[2], receivedString[3], "r", "0" });

                                    if (Environment.UserSettings.NotifyMessage)
                                    {
                                        // создать окошко с сообщением
                                        Thread.CurrentThread.CurrentUICulture = Environment.CurCultureInfo;
                                        DataRow row = Environment.DocData.GetDocProperties(docID, Environment.CurCultureInfo.TwoLetterISOLanguageName);

										string empName = empID == Environment.CurEmp.ID ? string.Empty : string.Format("({0})", Environment.EmpData.GetEmployee(empID, false));

                                        Lib.Win.MessageForm mf = new Lib.Win.MessageForm(Environment.StringResources.GetString(
                                            "MainForm.MainFormDialog.GotMessage.Message1") +
                                                                         System.Environment.NewLine +
                                                                         DBDocString.Format(row) +
                                                                         System.Environment.NewLine +
                                                                         System.Environment.NewLine +
                                                                         Environment.StringResources.GetString("MainForm.MainFormDialog.GotMessage.Message2"),
                                                                         Environment.StringResources.GetString("MainForm.MainFormDialog.GotMessage.Title1") +
                                                                         empName,
                                                                         MessageBoxButtons.YesNo, docID, empID);
                                        mf.DialogEvent += MessageForm_DialogEvent;

                                        if (Environment.AddMessageWindow(mf))
                                            mf.Hide();
                                    }
                                }
                                break;
                            default:
                                if (!int.TryParse(receivedString[1], out empID))
                                    return;
                                if (!int.TryParse(receivedString[2], out folderID))
                                    return;
                                if (!int.TryParse(receivedString[3], out docID))
                                    return;

                                var fNode = folders.SelectedNode as FoundNode;
                                var wNode = folders.SelectedNode as FolderTree.FolderNodes.WorkNodes.WorkFolderNode;
                                int rowIndex;

								switch(receivedString[4])
								{
									case "r": //прочитали документ
										int isRead;
										if(!int.TryParse(receivedString[5], out isRead))
											return;

										bool isread = Convert.ToBoolean(isRead);
										bool needToSet = false;
										rowIndex = docGrid.GetIndexConditional(Environment.DocData.IDField, docID);

										if(fNode != null && fNode.Emp.ID == empID)
										{
											if(!string.IsNullOrEmpty(fNode.XML) &&
												((!isread && Options.HasOption(fNode.XML, typeof(Lib.Win.Data.DALC.Documents.Search.Message.Incoming.Incoming_Read))) ||
												 (isread && Options.HasOption(fNode.XML, typeof(Lib.Win.Data.DALC.Documents.Search.Message.Incoming.Incoming_Unread))))
												)
											{ // мы в найденных, документ не соответствует критериям поиска, надо его удалить из списка
												if(rowIndex > -1)
												{
													docGrid.DeleteRow(rowIndex);
												}
											}
											else if(!string.IsNullOrEmpty(fNode.XML) && !isread && Options.HasOption(fNode.XML, typeof(Lib.Win.Data.DALC.Documents.Search.Message.Incoming.Incoming_Unread)))
											{// мы в найденных, документ соответствует критериям поиска, надо его добавить в список или обновить
												if(rowIndex == -1)
													code = Math.Max(code, 2);
												else
													needToSet = true;
											}
											else if(rowIndex > -1)
												needToSet = true;
										}
										else if(docGrid.IsDBDocs() && rowIndex > -1 && curEmpID == empID)
											needToSet = true;

										if(needToSet)
										{// мы просматриваем документы, документ есть в списке, его читал текущий пользователь
											docGrid.SetValue(rowIndex, Environment.WorkDocData.ReadField, isread);
											if(docGrid.Columns.Contains(Environment.DocData.MessageField))
												code = Math.Max(code, 1);
											if(docID == curDocID && infoGrid.DocID != 0)
												RefreshInfo(true);

											code = Math.Max(code, 1);
										}

										foreach(KeyValuePair<int, Form> t in Environment.OpenDocs.Where(t => docID == t.Key).ToList())
											(t.Value as SubFormDialog).RefreshMessage();
										break;
									case "-d": //удалили документ из работы
										if((fNode != null && fNode.Emp.ID == empID && !string.IsNullOrEmpty(fNode.XML) &&
											 Options.HasOption(fNode.XML, typeof(Lib.Win.Data.DALC.Documents.Search.Document.ВРаботе))) ||
											(wNode != null && wNode.Emp.ID == empID && wNode.ID == folderID))
										{
											if(docID < 0)
												docID = 0 - docID;
											rowIndex = docGrid.GetIndexConditional(Environment.DocData.IDField, docID);
											if(rowIndex > -1)
											{
												bool res = docGrid.IsSingle && docGrid.SelectedRows[0].Index == rowIndex; 
												docGrid.DeleteRow(rowIndex);
												if(res)
													docGrid.SelectRow(rowIndex);
											}
										}
										code = Math.Max(code, 1);
										ThreadPool.QueueUserWorkItem(new WaitCallback((object x) =>
										{
											object[] objs = x as object[];
											if(objs != null && objs.Length > 2)
											{
												int fID = (int)objs[0];
												int len = (int)objs[1];
												int eID = (int)objs[2];
												bool read = (bool)objs[3];
												if(!read || !Environment.UpdateWorkFolderData(fID, eID, -1, -1))
													Environment.ReloadReadData();
											}
										}), new object[] { folderID, receivedString[4].Length - 1, empID, receivedString.Length < 6 || !string.IsNullOrEmpty(receivedString[5]) && receivedString[5] == "r" && !string.IsNullOrEmpty(receivedString[6]) && "0".Equals(receivedString[6]) });
										
										break;
									case "d": //изменили или добавили документ в работе
									case "+d":
										if(wNode != null && wNode.Emp.ID == empID && wNode.ID == folderID)
											code = Math.Max(code, 2);
										else if(fNode != null && fNode.Emp.ID == empID && !string.IsNullOrEmpty(fNode.XML) && Options.HasOption(fNode.XML, typeof(Lib.Win.Data.DALC.Documents.Search.Document.ВРаботе)))
											code = Math.Max(code, 2);
										else
										 code = Math.Max(code, 1);
										
											ThreadPool.QueueUserWorkItem(  new WaitCallback((object x)=>
											{
												object[] objs = x as object[];
												if(objs != null && objs.Length > 2)
												{
													int fID = (int)objs[0];
													int len = (int)objs[1];
													int eID = (int)objs[2];
													bool read = (bool) objs[3];
													lock(this)
													{
														if(!read || !Environment.UpdateWorkFolderData(fID, eID, 1, len))
															Environment.ReloadReadData();
													}
												}
											}), new object[] { folderID, receivedString[4].Length - 1, empID, receivedString.Length > 6 && !string.IsNullOrEmpty(receivedString[5]) && receivedString[5] == "r" && !string.IsNullOrEmpty(receivedString[6]) && "0".Equals(receivedString[6]) });
										
										//if(receivedString.Length > 5 && receivedString[5].Length > 0)
										//{
										//    GotMessage(new string[] { "11", receivedString[1], receivedString[2], receivedString[3], receivedString[5], receivedString[6] });
										//}
										break;
									case "-f": //удалили папку в работе
										if(folderID < 0)
											folderID = 0 - folderID;
										if(folders.WorkFolderNode.ContainsByID(folderID, empID))
											folders.WorkFolderNode.RemoveByID(folderID, empID);
										break;
									case "f": //переименовали или добавили папку в работе
									case "+f":
										code = Math.Max(code, (folders.WorkFolderNode.ContainsByID(folderID, empID)
											|| folders.WorkFolderNode.AddExistingFolder(folderID, empID) != null) ? 1 : 3);
										// Если может найти или сам добавить папку, то только обновим счётчики
										break;
								}
                                break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex, string.Join("|", receivedString));
            }
            lock (_gotUpdatedDocsCodeLock)
            {
                _gotUpdatedDocsCode = Math.Max(code, _gotUpdatedDocsCode);
            }

            gotMessageTimer.Start();

#if AdvancedLogging
            Lib.Log.Logger.Message("MainFormDialog GotMessage leave receivedString= " + m);
#endif
        }

        private void GotDocumentChange(string receivedString)
        {
            Environment.SaveActiveForm();

			int docID = 0;
            bool eform = false;
            bool document = false;
            bool trans = false;
            bool links = false;
            bool document1С = false;
            bool sign = false;

            try
            {
                Match m = Regex.Match(receivedString, @"^(?<ID>\d+)(?<codes>[deltps]{0,6})$", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    docID = int.Parse(m.Groups["ID"].Value);
                    if (m.Groups["codes"].Value.Length > 0)
                    {
                        string codes = m.Groups["codes"].Value.ToLower();
                        document = codes.Contains("d");
                        eform = codes.Contains("e");
                        trans = codes.Contains("t");
                        links = codes.Contains("l");
                        document1С = codes.Contains("p");
                        sign = codes.Contains("s");
                    }
                }

                if (docID <= 0)
                    return;
                if (eform && Environment.UnSubscribeData.CheckIt(docControl.Subscribe))
                    return;
                if (!docGrid.IsDBDocs())
                    return;
                if (document)
                {
                    if (!(documentID == docID || documentID == -docID))
                    {
                        RefreshDocs();
                        if (docID == curDocID)
                            docControl.RefreshDoc();
                        documentID = docID;
                        ReloadCurrentDoc();
                    }
                    else
                    {
                        if (documentID == docID)
                            documentID = -docID;
                    }
                    return;
                }

                if (docID != curDocID)
                    return;
                if (documentID == -curDocID)
                    return;

                int timgID = docControl.ImageID;
                if (showDocsAndMessages == 2)
                    return;

                bool startTimer = false;
                if (eform)
                {
                    if (eformID == docID || eformID == -docID)
                        eformID = -docID;
                    else
                    {
                        docControl.RefreshDoc();
                        docControl.ChangeImageIC();
                        eformID = docID;
                        startTimer = true;
                    }
                }
                if (trans)
                    if (transID == docID || transID == -docID)
                        transID = -docID;
                    else
                    {
                        docControl.ReloadTran(false);
                        transID = docID;
                        startTimer = true;
                    }
                if (document1С)
                {
                    if (document1СID == docID || document1СID == -docID)
                        document1СID = -docID;
                    else
                    {
                        CheckSpent(curDocID);
                        docControl.ChangeImageIC();
                        document1СID = docID;
                        startTimer = true;
                    }
                }
                if (links)
                {
                    if (linksID == docID || linksID == -docID)
                        linksID = -docID;
                    else
                    {
                        if (docControl.ShowWebPanel || timgID == 0)
                        {
                            if (timgID == 0 && docControl.ImageID != 0)
                                docControl.ImageID = timgID;
                            docControl.RefreshEForm();
                        }
                        // достаем связи
                        RefreshLinks();
                        linksID = docID;
                        startTimer = true;
                    }
                }
                if (sign)
                    if (signID == docID || signID == -docID)
                    {
                        signID = -docID;
                    }
                    else
                    {
                        docControl.RefreshSigns();
                        signID = docID;
                        startTimer = true;
                    }
                if (startTimer)
                    ReloadCurrentDoc();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex, "message: " + receivedString + " wasn't processed too well on MainForm");
            }
            finally
            {
                Environment.RestoreActiveForm();
                while (_ignoreDocChanges.Contains(receivedString))
                    _ignoreDocChanges.Remove(receivedString);
            }
        }

        private void StartDocChangeReceiver(int docID)
        {
            if (string.IsNullOrEmpty(Environment.ConnectionStringDocument) || string.IsNullOrEmpty(Environment.SubscribeTable))
                return;
            try
            {
                Console.WriteLine("{0}: new Receive {1}", DateTime.Now.ToString("HH:mm:ss fff"), docID);
                docChangedReceiver = new Receive(Environment.ConnectionStringDocument, Environment.SubscribeTable, 14, docID, 3);
                Console.WriteLine("{0}: ReservePort start {1}", DateTime.Now.ToString("HH:mm:ss fff"), docID);
                docChangedReceiver.ReservePort();
                Console.WriteLine("{0}: ReservePort end {1}", DateTime.Now.ToString("HH:mm:ss fff"), docID);
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex, "Не удалось установить подписку на изменения");
            }
        }

        private void StopDocChangeReceiver()
        {
            try
            {
                if (docChangedReceiver != null)
                {
                    docChangedReceiver.Exit();
                    docChangedReceiver.Received -= receiver_Received;
                    docChangedReceiver.Dispose();
                    docChangedReceiver = null;
                }
                _ignoreDocChanges.Clear();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex, "Не удалось отменить подписку на изменения");
            }
        }

        /// <summary>
        /// Обновить или создать DocChangeReceiver
        /// </summary>
        private void UpdateOrCreateDocChangeReceiver(int docId)
        {
            if (docChangedReceiver == null)
            {
                Console.WriteLine("{0}: Create new receiver start {1}", DateTime.Now.ToString("HH:mm:ss fff"), docId);
                StartDocChangeReceiver(docId);
                Console.WriteLine("{0}: Create new receiver end {1}", DateTime.Now.ToString("HH:mm:ss fff"), docId);
            }
            else
            {
                Console.WriteLine("{0}: Update receiver start {1}", DateTime.Now.ToString("HH:mm:ss fff"), docId);
                var success = docChangedReceiver.Update(docId);
                Console.WriteLine("{0}: Update receiver end {1}", DateTime.Now.ToString("HH:mm:ss fff"), docId);

                // Что-то пошло не так, пересоздаю Receiver
                if (!success)
                {
                    Console.WriteLine("{0}: !!!RECreate new receiver start {1}", DateTime.Now.ToString("HH:mm:ss fff"), docId);
                    StartDocChangeReceiver(docId);
                    Console.WriteLine("{0}: !!!RECreate new receiver end {1}", DateTime.Now.ToString("HH:mm:ss fff"), docId);
                }
            }
        }

        private void GotFax(string receivedString)
        {
            if (receivedString == null)
                return;

#if AdvancedLogging
            Lib.Log.Logger.Message("MainFormDialog GotFax ");
#endif

            if (_gotFax == null)
                _gotFax = new SynchronizedCollection<string>();
            _gotFax.Add(receivedString);
            ProcessFaxString();

#if AdvancedLogging
            Lib.Log.Logger.Message("MainFormDialog GotFax leave ");
#endif
        }

        private void gotFaxTimer_Tick(object sender, EventArgs e)
        {
            ProcessFaxString();
        }

        private void ProcessFaxString()
        {
            if (gotFaxTimer != null)
                gotFaxTimer.Stop();

            if (gotFaxProcessor == null)
            {
                gotFaxProcessor = new BackgroundWorker();
                gotFaxProcessor.DoWork += ProcessFaxes;
                gotFaxProcessor.RunWorkerCompleted += ProcessFaxesCompleted;
            }

            if (gotFaxProcessor.IsBusy)
            {
                if (gotFaxTimer == null)
                {
                    gotFaxTimer = new System.Timers.Timer();
                    gotFaxTimer.Elapsed += gotFaxTimer_Tick;
                }
                gotFaxTimer.Interval = gotFaxTimeout;
                gotFaxTimer.Start();
            }
            else
                gotFaxProcessor.RunWorkerAsync(folders.SelectedNode as FolderTree.FolderNodes.FaxNodes.FaxNode);
        }

        private void ProcessFaxes(object sender, DoWorkEventArgs e)
        {
            try
            {
                var node = e.Argument as FolderTree.FolderNodes.FaxNodes.FaxNode;
                if (node == null)
                {
                    _gotFax.Clear();
                    return;
                }

                bool needRefresh = false;
                while (_gotFax.Count > 0)
                {
                    string faxStr = _gotFax[0];
                    int id = 0;
                    Match m = Regex.Match(faxStr, @"^(?<node>\d*)?(\|(?<fax>\d*))?(\|(?<code>\d*))?$");
                    if (m.Success)
                        id = int.Parse(m.Groups["node"].Value);

                    Console.WriteLine("{0}: fax received: {1} full string: {2}", DateTime.Now.ToString("HH:mm:ss fff"), id, _gotFax[0]);
                    if (id > 0)
                    {
                        if (node.ID == id)
                            needRefresh = true;
                    }
                    while (_gotFax.Contains(faxStr))
                        _gotFax.Remove(faxStr);
                }
                if (needRefresh)
                    _gotFax.Clear();

                e.Result = needRefresh;
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex, string.Format("Error while processing {0} fax message.", _gotFax[0]));
            }
        }

        private void ProcessFaxesCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Result is bool && (bool)e.Result)
                    if (InvokeRequired)
                        BeginInvoke((MethodInvoker)(RefreshDocs));
                    else
                        RefreshDocs();
                else
                    try
                    {
                        if (InvokeRequired)
                            BeginInvoke((MethodInvoker)(folders.UpdateFaxStatus));
                        else
                            folders.UpdateFaxStatus();
                    }
                    catch (Exception ex)
                    {
                        Lib.Win.Data.Env.WriteToLog(ex);
                    }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void docControl_NeedToRefresh(object source, EventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke((MethodInvoker)(Environment.RefreshDocs));
            else
                Environment.RefreshDocs();
        }

        #endregion

        #region Select

        private void SelectWorkDoc(int docID, int wfID, Employee emp)
        {
            if (wfID != -1)
            {
				bool athome = false;
				if(folders.WorkFolderNode != null)
				{
					athome = folders.SelectWorkFolder(wfID, emp, docID);
				}

                if (docID == curDocID)
                    Environment.RefreshDocs();
				else
				{
					if(athome || !(bool)typeof(Control).GetProperty("IsLayoutSuspended", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(docGrid, null) && !docGrid.SelectConditional(Environment.DocData.IDField, docID, false))
					{
						RefreshDocs(docID);
						//docGrid.SelectConditional(Environment.DocData.IDField, docID, false);
					}
				}
            }
            else
                ErrorMessage(Environment.StringResources.GetString("MainForm.MainFormDialog.SelectWorkDoc.Message1") + docID, Environment.StringResources.GetString("Error"));
        }

        private void SelectDBDoc(int docID, string path = FolderTree.FolderNodes.PathNodes.CatalogNodes.CatalogNode.CatalogPathInitial)
        {
            SelectDBDoc(docID, path, false);
        }

        private bool RealSelectDBDoc(int docID, string path, bool soft)
        {
            Console.WriteLine("{0}: Getting doc path", DateTime.Now.ToString("HH:mm:ss fff"));
            path = Environment.DocData.GetDocPath(docID, path);
            if (path != null)
            {
                Console.WriteLine("{0}: Selecting folder", DateTime.Now.ToString("HH:mm:ss fff"));
				folders.SelectCatalogNode(path, docID);
                Console.WriteLine("{0}: Selecting document", DateTime.Now.ToString("HH:mm:ss fff"));
                return true;
            }
            return false;
        }

        private bool SelectDBDoc(int docID, string path, bool soft)
        {
            if (folders.CatalogNode != null && docID > 0)
                try
                {
                    if (Environment.UserSettings.PersonID > 0)
                    {
                        if (Environment.DocData.CheckDocAndCurrentPerson(docID, Environment.UserSettings.PersonID))
                            return RealSelectDBDoc(docID, path, soft);
                        int testPersonID = Environment.DocData.GetCurrentPersonFromDoc(docID);
                        if (testPersonID > 0)
                        {
							var dialog = new Lib.Win.MessageForm(
									Environment.StringResources.GetString("MainForm.MainFormDialog.SelectDBDoc.Message1") +
									"\n" + Environment.PersonData.GetPerson(testPersonID) + "\n\n" +
									Environment.StringResources.GetString("MainForm.MainFormDialog.SelectDBDoc.Message2"),
									Environment.StringResources.GetString("MainForm.MainFormDialog.SelectDBDoc.Title1"),
									MessageBoxButtons.YesNo, docID);
							dialog.Tag = testPersonID;
                            this.path = path;
                            dialog.DialogEvent += PerMessageForm_DialogEvent;
                            dialog.Show();
                            return true;
                        }
                        using (DataTable personDT = Environment.PersonData.GetCurrentPersons())
                        {
                            for (int j = 0; personDT.Rows.Count > j; j++)
                            {
                                if (personDT.Rows[j].IsNull(Environment.PersonData.IDField))
                                {
									var dialog = new Lib.Win.MessageForm(
										Environment.StringResources.GetString("MainForm.MainFormDialog.SelectDBDoc.Message3") + "\n" +
										Environment.StringResources.GetString("MainForm.MainFormDialog.SelectDBDoc.Message4"),
										Environment.StringResources.GetString("MainForm.MainFormDialog.SelectDBDoc.Title1"),
										MessageBoxButtons.YesNo, docID);
									dialog.Tag = 0;
                                    this.path = path;
                                    dialog.DialogEvent += PerMessageForm_DialogEvent;
                                    dialog.Show();
                                    return true;
                                }
                            }
                        }
                    }
                    else
                    {
                        return RealSelectDBDoc(docID, path, soft);
                    }
                }
                catch (Exception ex)
                {
                    Lib.Win.Data.Env.WriteToLog(ex);
                }
            return false;
        }

        private void SelectWorkOrDBDoc(int docID, int empID = 0)
        {
            if (docID <= 0)
                return;
            try
            {
                object workFOlderID;
                Employee emp = Environment.CurEmp;
                if (empID > 0)
                {
                    workFOlderID = Environment.WorkDocData.DocPresentInWorkFolders(docID, empID);
                    emp = new Employee(empID, Environment.EmpData);
                }
                else
                    workFOlderID = Environment.WorkDocData.DocPresentInWorkFolders(docID, emp.ID);

                if (workFOlderID != null)
                {
					if(folders.WorkFolderNode != null)
					{
						if(DBNull.Value.Equals(workFOlderID))
							SelectWorkDoc(docID, 0, emp);
						else
							SelectWorkDoc(docID, (int)workFOlderID, emp);
					}
                }
                else
                    SelectDBDoc(docID);
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        #endregion

        #region Scan

		private void ScanDoc()
		{
			Environment.CmdManager.Commands["GotoScaner"].Execute();
			docControl.Visible = true;
			if(documentSelectedTimer != null)
				documentSelectedTimer.Stop();
			docControl.UseLock = false;
			docControl.ScanNewDocument(ScanNewDocumentHandler);
		}

		private void ScanNewDocumentHandler(object arg)
		{
			if(documentSelectedTimer != null)
				documentSelectedTimer.Stop();
			string fileNameTemp = arg.ToString();
			if(string.IsNullOrEmpty(fileNameTemp) || !File.Exists(fileNameTemp))
				return;
			Lib.Win.Document.Environment.AddTmpFile("", fileNameTemp, false);
			curDocString = Environment.StringResources.GetString("ScanedDoc");
			docControl.CurDocString = curDocString;
			docControl.Visible = true;
		}

        #endregion

        #region StatusBar

        private void RefreshStatusBar()
        {
            // 0 - doc info
            if (statusBar.Width <= 0)
                return;
            try
            {
                StatusBarPanel panel = statusBarPanelDoc;
                int newWidth = statusBar.Width - statusBarPanelCount.Width - statusBarPanelArchive.Width -
                               statusBarPanelSecure.Width - statusBarPanelDSP.Width - statusBarPanelPage.Width -
                               statusBarPanelDate.Width - 20;

                if (newWidth > panel.MinWidth)
                    panel.Width = newWidth;
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public void StatusBar_UpdateDocCount()
        {
            if (statusBar.Width <= 0)
                return;
            if (statusBar.Panels.Count < 2)
                return;

            // 1 - doc count
            try
            {
                StatusBarPanel panel = statusBarPanelCount;
                int count = docGrid.Rows.Count;
				int selectedCount = docGrid.SelectedRows.Count;

				panel.Text = count > 0 ? (selectedCount > 0 ? selectedCount.ToString() + "/" : string.Empty) + count.ToString() : string.Empty;
				panel.ToolTipText = Environment.StringResources.GetString("AllDocs") + count.ToString() + (selectedCount > 0 ? Environment.StringResources.GetString("SelectedDocs") + selectedCount.ToString() : "");
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void StatusBar_UpdatePage()
        {
            if (statusBar.Width <= 0)
                return;
            if (statusBar.Panels.Count < 3)
                return;

            // 2 - page
            try
            {
                StatusBarPanel panel = statusBarPanelPage;
                panel.Text = docControl.ImageDisplayed && !(docControl.DocumentID > 0 && curImageID == 0)
                                 ? Environment.StringResources.GetString("Page") + docControl.Page +
                                   Environment.StringResources.GetString("From") + docControl.PageCount
                                 : string.Empty;
                // updating status bar
                panel.ToolTipText = string.IsNullOrEmpty(panel.Text) || docControl.PageCount <= 0 ||
                                    docControl.Page <= 0
                                        ? string.Empty
                                        : string.Format(Environment.StringResources.GetString("Pages"), docControl.Page,
                                                        docControl.PageCount);
                statusBarPanelDSP.Text = !docControl.IsSignInternal ? "" : "ДСП";
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void StatusBar_UpdateDoc(bool isReadOnly = false)
        {
            if (statusBar.Width <= 0)
                return;
            if (statusBar.Panels.Count == 0)
                return;

            try
            {
                statusBarPanelDoc.Text = curDocString;
                docControl.CurDocString = curDocString;
                if (string.IsNullOrEmpty(curDocString))
                    Text = Environment.CompanyName;
                else
                    Text = curDocString + (isReadOnly ? " (" + Environment.StringResources.GetString("ReadOnly").Trim().ToLower() + ")" : "");
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void StatusBarTimerProcessor(object sender, EventArgs e)
        {
            if (DateTime.Today.AddMinutes(1).Day != DateTime.Today.Day)
            {
                statusBarTimer.Interval = 500;
                return;
            }

            statusBarTimer.Interval = 86395000;
            if (statusBar != null)
                statusBar.Panels[3].Text = DateTime.Now.ToString("dd.MM.yyyy");
        }

        #endregion

        #region ToolBar

        private void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            ToolBarButton button = e.Button;
            var tag = (string)button.Tag;
            bool pushed = button.Pushed;

            IOption opt = Environment.Layout.Option(tag);

            switch (tag)
            {
                case "ShowPagesPanel":					// включение/выключение панели страниц
                    if (opt != null)
                        opt.Value = pushed.ToString();

                    docControl.ShowThumbPanel = pushed;
                    break;
            }
        }

        private void toolBar_ButtonDropDown(object sender, ToolBarButtonClickEventArgs e)
        {
            ToolBarButton button = e.Button;
            var tag = (string)button.Tag;
            bool pushed = button.Pushed;

            switch (tag)
            {
                case "Links":
                    if (Environment.CmdManager.Commands.Contains("Links"))
                        Environment.CmdManager.Commands["Links"].ExecuteIfEnabled();
                    break;
                case "Undo":
					var pulb = new Kesco.Lib.Win.Document.Controls.PopUpListBox(RectangleToScreen(e.Button.Rectangle), "Отмена");
                    int count = Environment.UndoredoStack.UndoItems.Count;
					var text = new Kesco.Lib.Win.Document.Controls.ToolTopItem[count];
                    for (int i = 0; i < count; ++i)
                    {
                        UndoRedoElement element = Environment.UndoredoStack.UndoItems[i];
						text[count - 1 - i] = new Kesco.Lib.Win.Document.Controls.ToolTopItem(element.UndoString, element.UndoLongString);
                    }
                    pulb.DialogEvent += pulb_DialogEvent;
                    pulb.Items.AddRange(text);
                    pulb.Show();
                    break;
                case "Redo":
                    var redoPULB = new Kesco.Lib.Win.Document.Controls.PopUpListBox(RectangleToScreen(e.Button.Rectangle), "Возврат");
                    count = Environment.UndoredoStack.RedoItems.Count;
					text = new Kesco.Lib.Win.Document.Controls.ToolTopItem[count];
                    for (int i = 0; i < count; ++i)
                    {
                        UndoRedoElement element = Environment.UndoredoStack.RedoItems[i];
						text[count - 1 - i] = new Kesco.Lib.Win.Document.Controls.ToolTopItem(element.RedoString, element.RedoLongString);
                    }
                    redoPULB.DialogEvent += redoPULB_DialogEvent;
                    redoPULB.Items.AddRange(text);
                    redoPULB.Show();
                    break;
            }
        }

        private void zoomCombo_TextChanged(object sender, EventArgs e)
        {
            UpdateZoom();
        }

        private void UpdateZoom()
        {
            if (Disposing || IsDisposed)
                return;
            if (zoomIsBeingUpdated)
                return;
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(UpdateZoom));
                return;
            }
            try
            {
                zoomIsBeingUpdated = true;
                try
                {
                    zoom = zoomCombo.Text;
                }
                catch
                {
                    zoom = string.Empty;
                }
                if (zoom == Environment.StringResources.GetString("ToWindow"))
                {
                    docControl.FitTo(0);
                    Environment.ZoomString = zoom;
                }
                else if (zoom == Environment.StringResources.GetString("ToWidth"))
                    docControl.FitTo(1);
                else if (zoom == Environment.StringResources.GetString("ToHeigth"))
                    docControl.FitTo(2);
                else
                    try
                    {
                        bool percent = (zoom.IndexOf("%") != -1);
                        zoom = zoom.Replace("%", string.Empty);
                        int intZ;
                        if (!int.TryParse(zoom, out intZ))
                            return;
                        if (intZ < MinZoom && percent)
                        {
                            ErrorMessage(
                                Environment.StringResources.GetString("MainForm.MainFormDialog.Zoom.Minimum") +
                                MinZoom.ToString() + "%", Environment.StringResources.GetString("InputError"));
                            zoomCombo.Text = MinZoom.ToString() + "%";
                        }

                        if (intZ > MaxZoom)
                        {
                            ErrorMessage(
                                Environment.StringResources.GetString("MainForm.MainFormDialog.Zoom.Maximum") +
                                MaxZoom.ToString() + "%", Environment.StringResources.GetString("InputError"));
                            zoomCombo.Text = MaxZoom.ToString() + "%";
                        }

                        if (docControl.Zoom != intZ)
                            docControl.Zoom = intZ;
                    }
                    catch (Exception ex)
                    {
                        Lib.Win.Data.Env.WriteToLog(ex);
                        zoomCombo.Text = docControl.Zoom.ToString() + "%";
                    }
                try
                {
                    Environment.ZoomString = zoomCombo.Text;
                }
                catch
                {
                    Environment.ZoomString = docControl.Zoom.ToString() + "%";
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
            finally
            {
                zoomIsBeingUpdated = false;
            }
        }

        private void showPagesPanelButton_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as ToolStripButton;
                if (button == null)
                    return;
                IOption opt = Environment.Layout.Option((string)button.Tag);
                if (opt != null)
                    opt.Value = button.Checked.ToString();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        #endregion

        #region Clicks

        private void menuShowNoteBar_Click(object sender, EventArgs e)
        {
            menuShowNoteBar.Checked = !menuShowNoteBar.Checked;

            IOption opt = Environment.Layout.Option("ShowNoteBar");
            opt.Value = menuShowNoteBar.Checked.ToString();
        }

        private void ShowNoteBar(object sender, OptionEventArgs e)
        {
            bool val = Convert.ToBoolean(e.Value);
            menuShowNoteBar.Checked = val;
            annotationBar.Visible = val;
            annotationBar.Enabled = val;
        }

        private void menuScan_Click(object sender, EventArgs e)
        {
            ScanDoc();
        }

        private void LinkOpenDoc_Click(object sender, EventArgs e)
        {
            var item = sender as MenuItem;
            if (item == null)
                return;
            Match m = Regex.Match(item.Text, @"^\[(?<id>\d+)\]");
            if (m.Success)
            {
                var dialog = new LinkTypeDialog(curDocID, Int32.Parse(m.Groups["id"].Value));
                dialog.DialogEvent += LinkTypeDialog_DialogEvent;
                dialog.Show();
            }
        }

        #endregion

        /// <summary>
        /// Возврат с SubFormDialog
        /// </summary>
        /// <param name="parameters"></param>
        internal void OnNavigate(Common.ViewParameters parameters)
        {
            // Востанавливаю позицию скролинга
            if (parameters != null && parameters.ImageId == docControl.ImageID)
            {
                docControl.Page = parameters.Page;

                if (docControl.IsPDFMode)
                {
                    docControl.ScrollPositionX = (int)(parameters.ActualImageHorizontalScrollValue * docControl.ActualImageHoriszontalScrollMaxValue);
                    docControl.ScrollPositionY = (int)(parameters.ActualImageVerticalScrollValue * docControl.ActualImageVerticalScrollMaxValue);
                }
                else
                {
                    docControl.ScrollPositionX = -(int)(parameters.ActualImageHorizontalScrollValue * docControl.ActualImageHoriszontalScrollMaxValue);
                    docControl.ScrollPositionY = -(int)(parameters.ActualImageVerticalScrollValue * docControl.ActualImageVerticalScrollMaxValue);
                }
            }
        }

        private void CursorSleep()
        {
            if (InvokeRequired)
                BeginInvoke((MethodInvoker)(CursorSleep));
            else
            {
                if (Cursor != Cursors.WaitCursor)
                    Cursor = Cursors.WaitCursor;
            }
        }

        private void CursorWake()
        {
            if (InvokeRequired)
                BeginInvoke((MethodInvoker)(CursorWake));
            else
            {
                if (Cursor != Cursors.Default)
                    Cursor = Cursors.Default;
            }
        }

        private bool IsSelectedSingle()
        {
            return docGrid.IsSingle && (docControl.ImageDisplayed || curImageID >= 0) || (docGrid.SelectedRows.Count == 0 && ((folders.SelectedNode is FolderTree.FolderNodes.PathNodes.CatalogNodes.DocumentNode && (docControl.DocumentID > 0))|| (folders.SelectedNode is FolderTree.FolderNodes.PathNodes.ScanerNode && folders.SelectedNode.Level < 1 && !string.IsNullOrEmpty(docControl.FileName))));
        }

        private bool IsDisplayedSingle()
        {
			return IsSelectedSingle() && docControl.ImageDisplayed;
        }

        /// <summary>
        /// проверка на наличие подписей у текущего документа
        /// </summary>
        private bool IsNotSigned()
        {
            return curDocID < 1 || !docControl.IsSigned;
        }

        /// <summary>
        /// Если хотя бы один из выделенных документов не прочитан, вернёт true
        /// </summary>
        public bool IsNotRead()
        {
            string field = null;
            if (docGrid.IsDBDocs())
                field = Environment.DocData.WorkDocReadField;
            else if (docGrid.IsFaxes())
                field = Environment.FaxData.ReadField;
            else
                return false;

            return !docGrid.GetSelectedBoolValuesSummary(field);
        }


        /// <summary>
        /// проверка документа на проведение в 1С
        /// </summary>
        /// <param name="docID">код проверяемого документа</param>
        private void CheckSpent(int docID)
        {
            int index = docGrid.GetIndex(docID);
            if (index > -1)
            {
                string str = Environment.BuhParamDocData.GetSentDocToIcString(docID, Environment.UserSettings.PersonID);
                docGrid.SetValue(index, Environment.DocData.SpentField, str);
            }
        }

        #region DocControl

        private void docControl_DocChanged(object sender, DocumentSavedEventArgs e)
        {
            if (sender is Lib.Win.Document.Controls.SignDocumentPanel)
                try
                {
                    if (IsSelectedSingle() && IsNotRead())
                    {
                        if (readTimer != null && readTimer.Enabled)
                            readTimer.Stop();
                        foreach (int empID in Environment.EmpData.GetCurrentEmployeeIDs())
                            Environment.WorkDocData.MarkAsRead(empID, e.DocID == 0 ? curDocID : e.DocID);
                    }
                    var node = folders.SelectedNode as FoundNode;
                    if (node != null && node.XML != null)
                        if (Options.HasOption(node.XML, typeof(Lib.Win.Data.DALC.Documents.Search.EForm.Sign.Подписан)) ||
                            Options.HasOption(node.XML, typeof(Lib.Win.Data.DALC.Documents.Search.EForm.Sign.ПодписанМной)) ||
                            Options.HasOption(node.XML, typeof(Lib.Win.Data.DALC.Documents.Search.EForm.NoSign.НеПодписан)) ||
                            Options.HasOption(node.XML, typeof(Lib.Win.Data.DALC.Documents.Search.EForm.NoSign.НеПодписанМной)) ||
                            Options.HasOption(node.XML, typeof(Lib.Win.Data.DALC.Documents.Search.Image.Sign.Подписан)) ||
                            Options.HasOption(node.XML, typeof(Lib.Win.Data.DALC.Documents.Search.Image.Sign.ПодписанМной)) ||
                            Options.HasOption(node.XML, typeof(Lib.Win.Data.DALC.Documents.Search.Image.NoSign.НеПодписан)) ||
                            Options.HasOption(node.XML, typeof(Lib.Win.Data.DALC.Documents.Search.Image.NoSign.НеПодписанМной))
                            )
                        {
							if(curDocID < 1)
							{
								ClearInfo();
								docControl.Visible = false;
							}
                            RefreshDocs();
                        }
                    return;
                }
                catch (Exception ex)
                {
                    Lib.Win.Data.Env.WriteToLog(ex);
                }

            try
            {
                int docID = e.DocID;
                bool goTo = e.GoToDoc;
                bool create = e.CreateEForm;
                bool createSlave = e.CreateSlaveEForm;
                Environment.SaveActiveForm();
                UpdateWorkContext();
                if (goTo)
                    SelectWorkOrDBDoc(docID);
                else
                    RefreshDocs();
                Environment.RestoreActiveForm();
                if (create)
                {
                    string url = string.Empty;
                    int docTypeID = Environment.DocData.GetDocIntField(Environment.DocData.DocTypeIDField, docID, -1);
                    if (docTypeID > -1)
                    {
                        url = Environment.DocTypeData.GetField(Environment.DocTypeData.URLField, docTypeID).ToString();
                    }
                    if (string.IsNullOrEmpty(url))
                    {
                        url = Lib.Win.Document.Environment.SettingsURLString;
                    }

                    url = url.IndexOf("id=") > 0 ? url.Replace("id=", "id=" + docID.ToString()) : (url + ((url.IndexOf("?") > 0) ? "&" : "?") + "id=" + docID.ToString());
                    Lib.Win.Document.Environment.IEOpenOnURL(url);
                }

                if (createSlave)
                {
                    string url = string.Empty;
                    int contID = Environment.DocTypeData.GetSlaveType(docID);
                    if (contID > 0)
                        url = Environment.URLsData.GetField(Environment.URLsData.NameField, (int)Lib.Win.Data.DALC.Documents.URLsDALC.URLsCode.CreateSlaveUrl).ToString();
                    //}
                    //string paramStr = ((Lib.Win.Document.Environment.PersonID > 0) ? ("currentperson=" + Lib.Win.Document.Environment.PersonID.ToString()) : "")
                    //    + ((docID > 0) ? "&docID=" + docID.ToString() : "") + ((dr[1] > 0) ? "&contractid=" + dr[1].ToString() : "") + ((dr[2] > 0) ? "&fieldid=" + dr[2].ToString() : "") + "&auto=1";

                    if (!string.IsNullOrEmpty(url))
                    {
                        url = url.IndexOf("id=") > 0 ? url.Replace("id=", "docid=" + docID.ToString()) : (url + ((url.IndexOf("?") > 0) ? "&" : "?") + "docid=" + docID.ToString()) + "&contractid=" + contID.ToString();
                        Lib.Win.Document.Environment.IEOpenOnURL(url);
                    }
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void docControl_Load_PageChanged(object sender, EventArgs e)
        {
            docControl.PageChanged -= docControl_Load_PageChanged;

            RestoreScrollPositionFromOption();
        }

        /// <summary>
        /// Востановление скролинга из сохраненных в опциях значений
        /// </summary>
        /// <param name="resetOptions">Флаг сброса Options</param>
		private void RestoreScrollPositionFromOption(bool resetOptions = false)
		{
			// Проверка соответсвия сохраненных настроек и документа, изображения, страницы
			int docId = Environment.General.Option("DocID").GetValue<int>();

			if(docControl.DocumentID == docId)
			{
				int imageId = Environment.General.Option("ImageID").GetValue<int>();

				if(docControl.ImageID == imageId)
				{
					int page = Environment.General.Option("Page").GetValue<int>();

					if(docControl.Page == page)
					{
						docControl.ScrollPositionX = (int)Environment.General.Option("ScrollPositionX").Value;
						docControl.ScrollPositionY = (int)Environment.General.Option("ScrollPositionY").Value;

						if(resetOptions)
						{
							Environment.General.Option("Page").Value = 0;
							Environment.General.Option("ImageID").Value = -1;
						}
					}
				}
			}
		}

		private void docControl_PageChanged(object sender, EventArgs e)
		{
			infoGrid.ImageTime = docControl.ImageDate;
			if(showDocsAndMessages != 1 && Environment.LoadMessageFirst)
				RefreshInfo();
			StatusBar_UpdatePage();
			pageNum.Text = (docControl.Page > 0) ? (docControl.Page.ToString()) + " " : " ";
			pageNum.SelectionStart = docControl.Page.ToString().Length;
			textShow(null);
		}

		private void docControl_KeyDown(object sender, KeyEventArgs e)
		{
			docGrid_KeyDown(sender, e);
		}

		private void docControl_MarkEnd(object sender, EventArgs e)
		{
			BeginInvoke((MethodInvoker)(() => textShow(string.Empty)));
		}

        private void DocControl_NewWindow(object sender, DocumentSavedEventArgs e)
        {
            if (e.DocID > 0)
                Environment.NewWindow(e.DocID, zoomCombo.Text, new Context(Misc.ContextMode.Document));
            if (!string.IsNullOrEmpty(e.FileName))
                Environment.NewWindow(e.FileName, zoomCombo.Text, e.DocString, new Context(Misc.ContextMode.Document), 0);
        }

		void docControl_NeedRefresh(object sender, EventArgs e)
		{
			Form fr = sender as Form;
			if(sender != null && sender == Program.MainFormDialog)
			{
				Environment.Refresh();
			}
		}

        public void docControl_LinkDoc(object sender, LinkDocEventArgs e)
        {
            LinkDoc(e.DocID);
        }

		private void docControl_FaxInSave(object sender, DocumentSavedEventArgs e)
		{
			try
			{
				int docID = e.DocID;
				int imageID = e.ImageID;
				bool goTo = e.GoToDoc;
				bool create = e.CreateEForm;

				Environment.SaveActiveForm();
				UpdateWorkContext();

				if(goTo)
				{
					SelectWorkOrDBDoc(docID);

					if(create && !docControl.ShowWebPanel)
						Environment.CmdManager.Commands["ShowWeb"].Execute();
				}
				else
				{
					int faxID = Environment.FaxInData.GetFaxID(imageID);
					if(faxID > 0)
						docGrid.DeleteRowConditional(Environment.FaxData.IDField, faxID);
					if(create)
					{
						string url = string.Empty;
						int docTypeID = Environment.DocData.GetDocIntField(Environment.DocData.DocTypeIDField, docID, -1);
						if(docTypeID > -1)
						{
							url = Environment.DocTypeData.GetField(Environment.DocTypeData.URLField, docTypeID).ToString();
						}
						if(string.IsNullOrEmpty(url))
						{
							url = Lib.Win.Document.Environment.SettingsURLString;
						}

						url = url.IndexOf("id=") > 0
								  ? url.Replace("id=", "id=" + docID.ToString())
								  : (url + ((url.IndexOf("?") > 0) ? "&" : "?") + "id=" + docID.ToString());
						Lib.Win.Document.Environment.IEOpenOnURL(url);
					}
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			Environment.RestoreActiveForm();
		}

        void docControl_NeedRefreshGrid(object sender, NogeIdEventArgs e)
        {
            GotFax(e.NodeID.ToString());
        }

		private void docControl_VarListIndexChange(object sender, EventArgs e)
		{
			curImageID = docControl.ImageID;
			if(curImageID > 0)
				fileName = docControl.FileName;
			infoGrid.ImageTime = docControl.ImageDate;
			if(showDocsAndMessages != 1)
				infoGrid.Invalidate();
			StatusBar_UpdatePage();
		}

        private void docControl_ToolSelected(object sender, ImageControl.ToolSelectedEventArgs e)
        {
            switch (e.EventType)
            {
                case 0:
                    bCheck = ButtonCheck.MoveImg;
                    break;
                case 1:
                    bCheck = ButtonCheck.SelectAnn;
                    break;
                case 3:
                    bCheck = ButtonCheck.Marker;
                    break;
                case 5:
                    bCheck = ButtonCheck.Rectangle;
                    break;
                case 7:
                    bCheck = ButtonCheck.Text;
                    break;
                case 8:
                    bCheck = ButtonCheck.Note;
                    break;
                case 9:
                    bCheck = ButtonCheck.ImageStamp;
                    break;
                default:
                    docControl.SelectTool(0);
                    break;
            }
        }
        #endregion

		/// <summary>
		/// Обработчик закрытия формы удаления документа
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClosedDeleteDocumentForm(object sender, EventArgs e)
		{
			var form = (Lib.Win.MessageForm)sender;
			// Отписка от закрытия
			form.Closed -= OnClosedDeleteDocumentForm;
			_deleteDocForms.Remove(form);
		}

        public int KeyboardLLHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            int retValue = 0;
			if(nCode < 0)
                return Lib.Win.HookClass.CallNextHookEx(Lib.Win.HookClass.hHook, nCode, wParam, lParam);

            var MyKeyStHookStruct = (Lib.Win.HookClass.LowLevelKeyboardStruct)Marshal.PtrToStructure(lParam, typeof(Lib.Win.HookClass.LowLevelKeyboardStruct));

			if(wParam.ToInt32() == 257 && MyKeyStHookStruct.scanCode == 52 && ((MyKeyStHookStruct.flags & 32) == 0))
            {
                Form form = ActiveForm;
				if(form != null)
                {
                    Console.WriteLine("{0}: {1}", DateTime.Now.ToString("HH:mm:ss fff"), form.Name);
					if(form.Name == "OptionsDialog" || form.Name == "XmlSearchForm" || form.Name == "КодДокумента" || form.Name == "AddDBDocDialog")
                    {
                        buttonPush = false;
                        return Lib.Win.HookClass.CallNextHookEx(Lib.Win.HookClass.hHook, nCode, wParam, lParam);
                    }
                }
                buttonPush = true;
				if(timer == null)
                {
                    timer = new System.Timers.Timer(50);
                    timer.Elapsed += timer_Elapsed;
                    timer.Enabled = true;
                }
                else
                    timer.Interval = 50;
                timer.AutoReset = false;
                timer.Start();
                retValue = Lib.Win.HookClass.CallNextHookEx(Lib.Win.HookClass.hHook, -1, IntPtr.Zero, IntPtr.Zero);
            }
            else
            {
				if((MyKeyStHookStruct.flags & 32) == 0 && wParam.ToInt32() == 257)
                {
					switch(MyKeyStHookStruct.scanCode)
                    {
                        case 11:
							if(buttonPush)
                            {

								if(timer != null)
                                    timer.Interval = 50;
                                pushedButton.Append("0");
                                retValue = Lib.Win.HookClass.CallNextHookEx(Lib.Win.HookClass.hHook, -1, IntPtr.Zero, IntPtr.Zero);
                            }
                            else
                                retValue = Lib.Win.HookClass.CallNextHookEx(Lib.Win.HookClass.hHook, 0, wParam, lParam);
                            break;
                        case 10:
                        case 9:
                        case 8:
                        case 7:
                        case 6:
                        case 5:
                        case 4:
                        case 3:
                        case 2:
							if(buttonPush)
                            {
								if(timer != null)
                                    timer.Interval = 50;
                                pushedButton.Append((MyKeyStHookStruct.scanCode - 1).ToString());
                                retValue = Lib.Win.HookClass.CallNextHookEx(Lib.Win.HookClass.hHook, -1, IntPtr.Zero, IntPtr.Zero);
                            }
                            else
                                retValue = Lib.Win.HookClass.CallNextHookEx(Lib.Win.HookClass.hHook, 0, wParam, lParam);
                            break;
                        case 28:
							if(buttonPush)
                            {
                                buttonPush = false;
                                string idString = pushedButton.ToString();
                                pushedButton.Remove(0, pushedButton.Length);
								if(Environment.UserSettings.GotoDocument)
                                {
                                    var doc = new XmlDocument();
                                    XmlElement elOptions = doc.CreateElement("Options");
                                    XmlElement elOption = doc.CreateElement("Option");
                                    elOption.SetAttribute("name", "КодДокумента");
                                    elOption.SetAttribute("value", idString);
                                    elOptions.AppendChild(elOption);
                                    var dialog = new XmlSearchForm(elOptions.OuterXml, OptionsDialog.EnabledFeatures.All);
                                    Kesco.Lib.Win.Document.Win32.User32.SetForegroundWindow(dialog.Handle);
                                    dialog.DialogEvent += SearchDialog_DialogEvent;
                                    dialog.Show();
                                    dialog.BringToFront();
                                    dialog.Activate();
                                }
                                else
                                {
                                    Environment.DocData.SearchDocs("SELECT " + Environment.DocData.IDField + " FROM " + Environment.DocData.TableName + " WHERE " + Environment.DocData.IDField + " = " + idString, false, false, Environment.CurEmp.ID, 1);
                                    Environment.CmdManager.Commands["GotoFind"].ExecuteIfEnabled();
                                    docGrid.LoadFoundDocs(Environment.CurEmp, true, 0);
                                    docGrid.SelectRow(0);
                                }

                                retValue = Lib.Win.HookClass.CallNextHookEx(Lib.Win.HookClass.hHook, -1, IntPtr.Zero, IntPtr.Zero);
                            }
                            else
                                retValue = Lib.Win.HookClass.CallNextHookEx(Lib.Win.HookClass.hHook, 0, wParam, lParam);
                            break;
                        case 0:
                            retValue = Lib.Win.HookClass.CallNextHookEx(Lib.Win.HookClass.hHook, 0, wParam, lParam);
                            break;
                        default:
							if(buttonPush)
                            {
                                buttonPush = false;
                                Console.WriteLine("{0}: scan {1} {2}", DateTime.Now.ToString("HH:mm:ss fff"), MyKeyStHookStruct.scanCode, pushedButton);
                            }
                            retValue = Lib.Win.HookClass.CallNextHookEx(Lib.Win.HookClass.hHook, 0, wParam, lParam);
                            break;
                    }
                }
                else
                    retValue = Lib.Win.HookClass.CallNextHookEx(Lib.Win.HookClass.hHook, 0, wParam, lParam);
            }
            return retValue;
        }

        internal static void ErrorMessage(string message, string title)
        {
            ErrorShower.OnShowError(null, message, title);
        }

        internal static void ErrorMessage(string message)
        {
            ErrorShower.OnShowError(null, message, string.Empty);
        }

        private void MessageForm_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            var dialog = e.Dialog as Lib.Win.MessageForm;
            if (dialog == null || dialog.DocID == 0)
                return;
            try
            {
                if (dialog.DialogResult == DialogResult.Yes)
                {
                    GetFromTray();
                    if (dialog.DocID != curDocID || dialog.EmpID != curEmpID)
                        SelectWorkOrDBDoc(dialog.DocID, dialog.EmpID);
                }

                Environment.RemoveMessageWindow(dialog.DocID, dialog.EmpID, false);
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void ml_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            if (e.Dialog.DialogResult == DialogResult.Abort)
            {
                Dialogs.MenuList dialog = e.Dialog as Dialogs.MenuList;
                Environment.UserSettings.NeedSave = true;
                folders.SelectedNode = folders.AddDocumentNode(dialog.DocumentID, true, null, true);
            }
        }

        private void documentChangeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                documentChangeTimer.Stop();
                if (docChangedReceiver != null)
                {
                    docChangedReceiver.Received -= receiver_Received;
                    docChangedReceiver.Received += receiver_Received;
                    docChangedReceiver.Start(false);
                }
                else
                {
                    ReloadCurrentDoc();
                    return;
                }
                BeginInvoke((MethodInvoker) delegate
                                           {
                                               if (docGrid.IsDBDocs())
                                               {
                                                   if (documentID != 0)
                                                   {
                                                       if (documentID < 0)
                                                       {
                                                           RefreshDocs();
                                                           if (documentID == -curDocID)
                                                               docControl.RefreshDoc();
                                                       }
                                                       eformID = 0;
                                                       documentID = 0;
                                                       transID = 0;
                                                       linksID = 0;
                                                       document1СID = 0;
                                                       signID = 0;
                                                       return;
                                                   }

                                                   if (showDocsAndMessages != 2 && curDocID > 0)
                                                   {
                                                       int timgID = docControl.ImageID;
                                                       if (eformID == -curDocID)
                                                       {
                                                           docControl.RefreshDoc();
                                                           docControl.ChangeImageIC();
                                                       }
                                                       if (transID == -curDocID)
                                                           docControl.ReloadTran(false);

                                                       if (document1СID == -curDocID)
                                                       {
                                                           CheckSpent(curDocID);
                                                           docControl.ChangeImageIC();
                                                       }
                                                       if (linksID == -curDocID)
                                                       {
                                                           if (docControl.ShowWebPanel || timgID == 0)
                                                           {
                                                               if (timgID == 0 && docControl.ImageID != 0)
                                                                   docControl.ImageID = timgID;
                                                               docControl.RefreshEForm();
                                                           }
                                                           // достаем связи
                                                           RefreshLinks();
                                                       }

                                                       if (signID == -curDocID)
                                                           docControl.RefreshSigns();
                                                   }
                                                   eformID = 0;
                                                   transID = 0;
                                                   linksID = 0;
                                                   document1СID = 0;
                                                   signID = 0;
                                               }
                                           });
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void settingsDocumentLoadDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            if (e.Dialog.DialogResult == DialogResult.OK)
            {
                SetInfoPlace();
            }
        }

        #region Message Place

        private void menuItemBetween_Click(object sender, EventArgs e)
        {
            SetMessageState(0);
        }

        private void menuItemLeft_Click(object sender, EventArgs e)
        {
            SetMessageState(1);
        }

        private void menuItemUnder_Click(object sender, EventArgs e)
        {
            SetMessageState(2);
        }

        private void SetMessageState(int state)
        {
            Environment.General.Option("ShowInMainWindow").Value = state;
            Environment.General.Save();
            SetInfoPlace();
        }
        #endregion

        private void uniDialog_ResultEvent(object source, Lib.Win.DialogResultEvent e)
        {
            switch (e.Result)
            {
                case DialogResult.OK:
                    if (e.Params.Length == 3 && e.Params[0] is int)
                    {// make link
                        if (e.Params[1] is int)
                        {
                            var firstID = (int)e.Params[0];
                            var secondID = (int)e.Params[1];
                            if (firstID > 0 && secondID > 0)
                            {
                                var linkDialog = new LinkTypeDialog(firstID, secondID);
                                linkDialog.DialogEvent += LinkTypeDialog_DialogEvent;
                                linkDialog.Show();
                            }
                        }
                        else if (e.Params[1] is int[])
                        {
                            var firstID = (int)e.Params[0];
                            var secondIDs = (int[])e.Params[1];
                            foreach (int secondID in secondIDs)
                                if (firstID > 0 && secondID > 0)
                                {
                                    var linkDialog = new LinkTypeDialog(firstID, secondID);
                                    linkDialog.DialogEvent += LinkTypeDialog_DialogEvent;
                                    linkDialog.Show();
                                }
                        }
                    }
                    break;
                case DialogResult.Retry:
                    if (e.Params.Length == 3 && e.Params[0] is int && e.Params[2] is string)
                    {
                        var searchDialog = new XmlSearchForm(e.Params[2].ToString()) { DocID = (int)e.Params[0] };
                        searchDialog.DialogEvent += SearchAndSelect_DialogEvent;
                        searchDialog.Show();
                    }
                    break;
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsDisposed || Disposing)
                return;
            try
            {
                buttonPush = false;
                pushedButton.Remove(0, pushedButton.Length);

                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void menuSettingsLanguage_Click(object sender, EventArgs e)
        {
            Settings.SettingsLanguageDialog settings = new Settings.SettingsLanguageDialog();
            settings.DialogEvent += settings_DialogEvent;
            settings.Show();
        }

        private void settings_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            if (e.Dialog.DialogResult != DialogResult.OK)
                return;

            Program.docViewContext.MainForm = null;
            Program.MainFormDialog.Close();
            Program.MainFormDialog = new MainFormDialog();
            Program.MainFormDialog.Show();
            Program.docViewContext.MainForm = Program.MainFormDialog;
        }

        private void mf_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            if (e.Dialog.DialogResult != DialogResult.Yes)
                return;

            ExitApp();
            Thread.Sleep(2000);
            if (File.Exists(Application.StartupPath + "\\DocViewRun.exe"))
                Process.Start(Application.StartupPath + "\\DocViewRun.exe");
            else
                if (File.Exists(Path.GetPathRoot(Application.StartupPath) + "\\DocViewRun.exe"))
                    Process.Start(Path.GetPathRoot(Application.StartupPath) + "\\DocViewRun.exe");
                else
                    Process.Start(Application.ExecutablePath);
        }

        private void SendMessageDialog_NeedSendWindow(int[] ids, string forAllMessage, SynchronizedCollection<int> forAllRecipients, Hashtable personMessages)
        {
            if (InvokeRequired)
                BeginInvoke(new SendMessageDialog.SendDelegate(ShowErrorAndMessageWindow), new object[] { ids, forAllMessage, forAllRecipients, personMessages });
            else
                ShowErrorAndMessageWindow(ids, forAllMessage, forAllRecipients, personMessages);
        }

		private void ShowErrorAndMessageWindow(int[] ids, string forAllMessage, SynchronizedCollection<int> forAllRecipients, Hashtable personMessages)
		{
			var sd = new SendMessageDialog(ids);
			sd.Show();
			sd.AllText = forAllMessage;
			sd.AllUsers = forAllRecipients;
			sd.PersonSenders = personMessages;
			//var mf = new Lib.Win.MessageForm(Environment.StringResources.GetString("MainFormDialog.ShowErrorAndMessageWindow.Message1"), Environment.StringResources.GetString("MainFormDialog.ShowErrorAndMessageWindow.Title1"));
			//sd.ShowSubForm(mf);
		}

		private void SendMessageDialog_MessageSend(bool remove, int[] ids)
		{
			if(ids == null)
				return;

			MessageSend(remove, ids);
		}

		private void MessageSend(bool remove, int[] ids)
		{
			CursorSleep();
			Environment.SaveActiveForm();

			try
			{
				
				int rowIndex = docGrid.CurrentRowIndex;

				if(folders.SelectedNode is FolderTree.FolderNodes.WorkNodes.WorkNode)
				{
					if(ids.Length == 1 && !remove && Environment.UserSettings.GotoNext && rowIndex < docGrid.RowCount - 1 && ids[0].Equals(docGrid.GetValue(rowIndex, docGrid.Style.IdField)))
					{
						// Получаю DocId следующего документа
						object nextDocDocId = docGrid.GetValue(rowIndex + 1, docGrid.Style.IdField);

						// Обновляю данные
						RefreshDocs();
						// После обновления
						// Список документов может изменится. 
						// Документы могут стать недоступными или появится новые документы, которых ранее не было 

						// Не понятно, который документ будет выбран, тк после обновления rowIndex может указвать на иной документ
						if(nextDocDocId == null)
							docGrid.SelectRow(rowIndex + 1);
						else
							docGrid.SelectByID((int)nextDocDocId); // Следующий документ может стать недоступным. В этом случае в гриде не будет Selected Row
					}
					else
						RefreshDocs();

					// Не понятно, который документ будет выбран, тк после обновления rowIndex может указвать на иной документ
					if(docGrid.SelectedRows.Count == 0)//документ удалили из работы
						docGrid.SelectRow(rowIndex);
				}
				else
				{
					if(ids.Length > 0 && ids[0] == curDocID && !remove && Environment.UserSettings.GotoNext && rowIndex < docGrid.RowCount - 1)
						docGrid.SelectRow(rowIndex + 1); // Список документов не обновлялся, rowIndex по прежнему указывает на выбранный документ, переходим на следующий документ

				}
				UpdateWorkContext();
				RefreshInfo(true);
				foreach(KeyValuePair<int, Form> t in ids.SelectMany(id => Environment.OpenDocs.Where(t => id == t.Key)))
					(t.Value as SubFormDialog).RefreshMessage();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			finally
			{
				CursorWake();
				Environment.RestoreActiveForm();
			}
		}

        private void pulb_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            if (e.Dialog.DialogResult == DialogResult.OK)
            {
                var pulb = e.Dialog as Kesco.Lib.Win.Document.Controls.PopUpListBox;
                if (pulb != null && pulb.Items.Count > 0 && Environment.UndoredoStack.UndoItems.Count > 0)
                    Environment.UndoredoStack.Undo(pulb.Count + 1);
            }
        }

        private void redoPULB_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            if (e.Dialog.DialogResult == DialogResult.OK)
            {
                var pulb = e.Dialog as Kesco.Lib.Win.Document.Controls.PopUpListBox;
                if (pulb != null && (pulb.Items.Count > 0 && Environment.UndoredoStack.RedoItems.Count > 0))
                    Environment.UndoredoStack.Redo(pulb.Count + 1);
            }
        }

		private void docGrid_DoubleClick(object sender, EventArgs e)
		{
			var em = e as MouseEventArgs;
			int rowIndex = -1;
			if(em == null || (rowIndex = docGrid.HitTest(em.X, em.Y).RowIndex) == -1)
				return;
			//if(docGrid.SelectedRows[0].Index != rowIndex)
			//{
				System.Timers.Timer dctimer = new System.Timers.Timer(documentChangeTimeout);
				dctimer.Elapsed += new ElapsedEventHandler(dctimer_Elapsed);
				dctimer.AutoReset = false;
				dctimer.Enabled = true;
				dctimer.Start();
			//}
			//else
			//if((!docGrid.IsDBDocs() || docControl.CompliteLoading) && docGrid.IsSingle)
			//    Environment.CmdManager.Commands["DocProperties"].ExecuteIfEnabled();
		}

		void dctimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			System.Timers.Timer dctimer = sender as System.Timers.Timer;
			if(dctimer != null)
			{
				dctimer.Stop();
				dctimer.Dispose();
			}
			InvokeIfRequired((MethodInvoker)(() =>
				{
					if((!docGrid.IsDBDocs() || docControl.CompliteLoading) && docGrid.IsSingle)
						Environment.CmdManager.Commands["DocProperties"].ExecuteIfEnabled();
				}));
		}

        private void MainFormDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveFile();
            if (_closeClick)
            {
                e.Cancel = true;
                Hide();
                notifyIcon.Visible = true;
                _closeClick = false;
            }
        }

        #region UndoRedoLists

        private void undoButton_DropDownOpening(object sender, EventArgs e)
        {
            undoButton.DropDown.Items.Clear();
            foreach (UndoRedoElement element in Environment.UndoredoStack.UndoItems)
            {
                var item = new ToolStripMenuItem(element.UndoLongString);
                item.MouseEnter += undoItem_MouseEnter;
                undoButton.DropDown.Items.Insert(0, item);
            }
        }

        private void undoButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Environment.UndoredoStack.Undo(undoButton.DropDown.Items.IndexOf(e.ClickedItem) + 1);
        }

        private void redoButton_DropDownOpening(object sender, EventArgs e)
        {
            redoButton.DropDown.Items.Clear();
            foreach (UndoRedoElement element in Environment.UndoredoStack.RedoItems)
            {
                var item = new ToolStripMenuItem(element.RedoLongString);
                item.MouseEnter += redoItem_MouseEnter;
                redoButton.DropDown.Items.Insert(0, item);
            }
        }

        private void redoButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Environment.UndoredoStack.Redo(redoButton.DropDown.Items.IndexOf(e.ClickedItem) + 1);
        }

        private void undoItem_MouseEnter(object sender, EventArgs e)
        {
            UndoRedoItem_Select(undoButton.DropDown.Items, undoButton.DropDown.Items.IndexOf((ToolStripItem)sender));
        }

        private void redoItem_MouseEnter(object sender, EventArgs e)
        {
            UndoRedoItem_Select(redoButton.DropDown.Items, redoButton.DropDown.Items.IndexOf((ToolStripItem)sender));
        }

        private void UndoRedoItem_Select(ToolStripItemCollection Items, int currentItem)
        {
            for (int i = 0; i <= currentItem && i < Items.Count; i++)
            {
                Items[i].ForeColor = Color.FromKnownColor(KnownColor.HighlightText);
                Items[i].BackColor = Color.FromKnownColor(KnownColor.Highlight);
            }
            for (int i = currentItem + 1; i < Items.Count; i++)
            {
                Items[i].ForeColor = Color.FromKnownColor(KnownColor.ControlText);
                Items[i].BackColor = Color.FromKnownColor(KnownColor.Control);
            }
        }

        #endregion

		private void linkEFormItem_Click(object sender, EventArgs e)
		{
			if(sender is Items.IDMenuItem)
				try
				{
					var item = sender as Items.IDMenuItem;
					int typeID = item.ID;
					var fieldID = (int)(item.Tag as DataRow)[Environment.DocLinksData.SubFieldIDField];
					string searchString = "SELECT " + Environment.DocData.IDField + " FROM " +
										  Environment.DocData.TableName + " T0 " +
										  "WHERE (EXISTS (SELECT * FROM " + Environment.DocLinksData.TableName +
										  " TI WHERE TI." + Environment.DocLinksData.ParentDocIDField + "=" +
										  curDocID.ToString() +
										  " AND TI." + Environment.DocLinksData.ChildDocIDField + "=T0." +
										  Environment.DocData.IDField + " AND TI." +
										  Environment.DocLinksData.SubFieldIDField + " = " + fieldID.ToString() + "))" +
										  "AND (T0." + Environment.DocData.DocTypeIDField + " = " + typeID.ToString() +
										  ")";

					using( DataTable dt = Environment.DocData.GetDocsByIDQuery(searchString, Environment.CurCultureInfo.Name))
					if(dt != null && dt.Rows.Count > 0)
					{
						Dialogs.ConfirmTypeDialog dialog = new Dialogs.ConfirmTypeDialog
										 {
											 TypeID = typeID,
											 DocID = curDocID,
											 FieldID = fieldID,
											 TypeString = item.Text,
											 SearchString = searchString
										 };
						dialog.DialogEvent += ConfirmTypeDialog_DialogEvent;
						dialog.Show();
					}
					else
					{
						CreateNewDoc(typeID, curDocID, fieldID);
					}
				}
				catch(Exception ex)
				{
					Lib.Win.Data.Env.WriteToLog(ex);
				}
		}

		private void CreateNewDoc(int typeID, int docID, int fieldID)
		{
			string url = Environment.DocTypeData.GetField(Environment.DocTypeData.URLField, typeID).ToString();
			string paramStr = ((Lib.Win.Document.Environment.PersonID > 0) ? ("&currentperson=" + Lib.Win.Document.Environment.PersonID.ToString()) : "") + ((docID > 0) ? "&docID=" + docID.ToString() + ((fieldID > 0) ? "&fieldID=" + fieldID.ToString() : "") : "");
			if(string.IsNullOrEmpty(url))
			{
				Kesco.Lib.Win.MessageForm.Show("Нет формы создания документа");
				return;
			}
			if(paramStr.Length > 0)
			{
				url += (url.IndexOf("?") > 0) ? "&" : "?";
				url += paramStr.TrimStart('&');
			}
			Lib.Win.Document.Environment.IEOpenOnURL(url);
		}

		private void ConfirmTypeDialog_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
		{
			if(e.Dialog is Dialogs.ConfirmTypeDialog)
			{
				Dialogs.ConfirmTypeDialog dialog = e.Dialog as Dialogs.ConfirmTypeDialog;
				if(dialog.DialogResult == DialogResult.Yes)
					CreateNewDoc(dialog.TypeID, dialog.DocID, dialog.FieldID);
			}
		}

        // Запоминаем отображение окна настроек печати
        private void menuPrintSettings_Click(object sender, EventArgs e)
        {
            menuPrintSettings.Checked = !menuPrintSettings.Checked;
            docControl.AlwaysShow = menuPrintSettings.Checked;
        }

        // приходится проверять каждый раз
        private void menuItem8_Popup(object sender, EventArgs e)
        {
            menuPrintSettings.Checked = docControl.AlwaysShow;
		}
	}
}