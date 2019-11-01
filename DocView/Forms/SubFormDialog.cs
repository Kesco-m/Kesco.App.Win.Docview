using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Data.Repository;
using Kesco.Lib.Win.Document;
using Kesco.Lib.Win.Document.Components;
using Kesco.Lib.Win.Document.Dialogs;
using Kesco.Lib.Win.Document.Items;
using Kesco.Lib.Win.Document.Select;
using Kesco.Lib.Win.Error;
using Kesco.Lib.Win.ImageControl;

namespace Kesco.App.Win.DocView.Forms
{
	/// <summary>
	/// Дополнительное окно для открытия документа
	/// </summary>
	public class SubFormDialog : Form
	{
		private SynchronizedCollection<Keys> keyLocker;
		private SynchronizedCollection<string> ignoreDocChanges = new SynchronizedCollection<string>();

		private bool stop;

		private int currentDocID;
		private int imgID;
		private int page;
		private int showMessageState;

		private int curEmpID
		{
			get { return context != null && context.Emp != null ? context.Emp.ID : Environment.CurEmp.ID; }
		}

		private int toolSelected;
		private string fileName;
		private string docString;
		private bool showPushed;

		private bool sendfax;

		private Lib.Win.Options.Folder subLayout;				// пункт в реестре
		private Lib.Win.Options.IOption optShowPagesPanel;		// пункт отвечающий за показ панали превьюшек.

		internal Lib.Win.Receive.Receive docChangedReceiver;	// получатель сообщений об изменении документа
		private System.Timers.Timer receiverTimer;

		private Grids.InfoGrid infoGrid;

		private CommandManagement.CommandManager subCmdManager;	// подчиненый менеджер комманд +)

		private Context context;
		private ImageList toolStripImageList;
		private StatusBarPanel sBPName;
		private StatusBarPanel sBPCount;
		private StatusBarPanel sBPPage;
		internal Kesco.Lib.Win.Document.Controls.DocControl docControl;
		private ContextMenuStrip linkContextMenu;
		private ToolStripMenuItem mIMain;
		private ToolStripMenuItem mIArchive;
		private StatusBar statusBar;
		private Panel panelDoc;
		private ToolStrip toolStrip;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripButton toolStripButtonSave;
		private ToolStripButton toolStripButtonSavePart;
		private ToolStripButton toolStripButtonProperty;
		private ToolStripButton toolStripButtonThumb;
		private ToolStripButton toolStripButtonWeb;
		private ToolStripSplitButtonCheckable toolStripSplitButtonMessage;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripButton toolStripButtonBefore;
		private ToolStripTextBox toolStripTextBoxPageNum;
		private ToolStripButton toolStripButtonNext;
		private ToolStripSeparator toolStripSeparator4;
		private ToolStripComboBox toolStripComboBoxZoom;
		private ToolStripButton toolStripButtonZoomIn;
		private ToolStripButton toolStripButtonZoomOut;
		private ToolStripSeparator toolStripSeparator5;
		private ToolStripButton toolStripButtonHand;
		private ToolStripButton toolStripButtonSelect;
		private ToolStripSeparator toolStripSeparator6;
		private ToolStripSplitButton toolStripButtonPrint;
		private ToolStripButton toolStripButtonSendMessage;
		private ToolStripButton toolStripButtonSendFax;
		private ToolStripSeparator toolStripSeparator7;
		private ToolStripMenuItem toolStripMenuItemJoin;
		private ToolStripSeparator toolStripSeparator8;
		private ToolStripButton toolStripButtonLinks;
		private ToolStripButton toolStripButtonZoomToSelection;
		private ToolStripButton toolStripButtonRotateLeft;
		private ToolStripButton toolStripButtonRotateRight;
		private ToolStripSeparator toolStripSeparator9;
		private ToolStripMenuItem ToolStripMenuItemNewWindow;
		private ToolStripMenuItem ToolStripMenuItemLeft;
		private ToolStripMenuItem ToolStripMenuItemBottom;
		private ToolStripMenuItem ToolStripMenuItemPrintPage;
		private ToolStripMenuItem ToolStripMenuItemPrintImage;
		private ToolStripMenuItem ToolStripMenuItemPrintDoc;
		private ToolStripButton toolStripButtonArrow;
		private ToolStripButton toolStripButtonStamp;
		private ToolStripSeparator toolStripSeparator10;
		private ToolStripMenuItem toolStripMenuItemNewDocEform;
		private ToolStripMenuItem toolStripMenuItemScanDocImage;
		private MenuStrip menuStrip1;
		private ToolStripMenuItem toolStripMenuItemDoc;
		private ToolStripMenuItem toolStripMenuItemLinkEform;
		private ToolStripMenuItem toolStripMenuItemSave;
		private ToolStripMenuItem toolStripMenuItemSavePart;
		private ToolStripMenuItem toolStripMenuItemSaveSelected;
		private ToolStripSeparator toolStripSeparator11;
		private ToolStripMenuItem toolStripMenuItemAddEform;
		private ToolStripMenuItem toolStripMenuItemSaveStamp;
		private ToolStripSeparator toolStripSeparator12;
		private ToolStripMenuItem toolStripMenuItemAddToWork;
		private ToolStripMenuItem toolStripMenuItemEditPlaces;
		private ToolStripMenuItem toolStripMenuItemEndWork;
		private ToolStripSeparator toolStripSeparator13;
		private ToolStripMenuItem toolStripMenuItemEditDesc;
		private ToolStripMenuItem toolStripMenuItemSpam;
		private ToolStripSeparator toolStripSeparator14;
		private ToolStripMenuItem toolStripMenuItemProperties;
		private ToolStripSeparator toolStripSeparator15;
		private ToolStripMenuItem toolStripMenuItemDocPartDelete;
		private ToolStripMenuItem toolStripMenuItemDelFrom;
		private ToolStripMenuItem toolStripMenuItemDelFromSearch;
		private ToolStripMenuItem toolStripMenuItemGotoPerson;
		private ToolStripSeparator toolStripSeparator16;
		private ToolStripButton toolStripButtonReturn;
		private ToolStripButton toolStripButtonSaveSelected;
		private SplitContainer splitContainer;
		private IContainer components;
		private ToolStripButton toolStripButtonDSP;
		private StatusBarPanel statusBarPanelProtected;
		private StatusBarPanel statusBarPanelDSP;
	    private ToolStripMenuItem toolStripMenuItemAddImage;

		private Lib.Win.Document.Objects.TmpFile tf = null;
		private ToolStripContainer toolStripContainer1;
		private TableLayoutPanel tableLayoutPanel1;
		private Lib.Win.Opinion.OpinionControl opinionControl;

        // Параметры отображения
	    private readonly Common.ViewParameters _parameters;

		#region Init

		public SubFormDialog(int docID, int imgID, string zoom, Context context) : this(docID, imgID, zoom, null, context, 0)
		{
		}

		private SubFormDialog()
		{
			try
			{
				page = 1;
				subCmdManager = new CommandManagement.CommandManager();
				subLayout = Environment.Layout.Folders.Add("SubForm");
				InitializeComponent();
				toolStrip.ImageScalingSize = new Size((int)(toolStrip.ImageScalingSize.Width * Lib.Win.Document.Environment.Dpi / 96),  (int)(toolStrip.ImageScalingSize.Height * Lib.Win.Document.Environment.Dpi / 96));
				toolStripMenuItemSaveStamp.Enabled = Lib.Win.Document.Environment.IsDomainAdmin();
				docControl.CanSave = true;
				KeyPreview = true;

				keyLocker = new SynchronizedCollection<Keys>();
				Slave.DoWork(InitializeCommandManager, null);
                LoadReg();
				LoadMenuImages();
				Disposed += SubFormDialog_Disposed;
				DocControlComponent.DocumentSaved += docComponent_DocumentSaved;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

        /// <summary>
        /// Форма документа в отдельном окне
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="imgId"></param>
        /// <param name="zoom"></param>
        /// <param name="parameters"></param>
        /// <param name="context"></param>
        /// <param name="page"></param>
		public SubFormDialog(int docID, int imgID, string zoom, Common.ViewParameters parameters, Context context, int page) : this()
		{
			try
			{
				currentDocID = docID;
				this.imgID = imgID;
				this.page = page;
				this.context = context;
				docString = DBDocString.Format(currentDocID);
				if(docString != null)
				{
					Text = Regex.Replace(docString, @"[\n\r]", " ");
					docControl.CurDocString = Regex.Replace(docString, @"[\n\r]", " ");
				}
				toolStripButtonSendFax.Visible = Environment.IsFaxSender(currentDocID);

				// adding docID to global list
				if(docChangedReceiver != null)
				{
					docChangedReceiver.Exit();
					docChangedReceiver.Received -= receiver_Received;
					docChangedReceiver = null;
				}
				try
				{
					docChangedReceiver = new Lib.Win.Receive.Receive(Environment.ConnectionStringDocument, Environment.SubscribeTable, 14, currentDocID, 3);
					docChangedReceiver.ReservePort();
				}
				catch(Exception ex)
				{
					Lib.Win.Data.Env.WriteToLog(ex, "Не удалось установить подписку на изменения");
				}
				statusBarPanelProtected.Text = 1.Equals(Convert.ToInt32(Environment.DocData.GetField(Environment.DocData.ProtectedField, currentDocID))) ? (Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru") ? "Секретно" : "Private") : "";
				toolStripButtonSendFax.Enabled = false;
				toolStripButtonPrint.Enabled = false;
				toolStripComboBoxZoom.Text = zoom;
				sBPCount.Text = docControl.ImageCount.ToString();

			    _parameters = parameters;

			    Environment.AddOpenDoc(currentDocID, this);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public SubFormDialog(string fileName, string zoom, string DocString, Context context, int page) : this()
		{
			try
			{
				this.fileName = fileName;
				imgID = -2;
				this.page = page;
				this.context = context;

				toolStripButtonLinks.Enabled = false;
				if(DocString != null)
				{
					Text = Regex.Replace(DocString, @"[\n\r]", " ");
					docString = DocString;
					docControl.CurDocString = DocString;
				}
				toolStripButtonProperty.Enabled = true;
				switch(this.context.Mode)
				{
					case Misc.ContextMode.FaxIn:
						int faxID = Environment.FaxData.GetFaxIDFromFileName(Path.GetFileNameWithoutExtension(fileName));
						if(faxID > 0)
							docControl.CreateFaxInComponent(faxID);
						else
							docControl.SetNullComponent();
						break;
					case Misc.ContextMode.FaxOut:
						faxID = Environment.FaxData.GetFaxIDFromFileName(Path.GetFileNameWithoutExtension(fileName));
						if(faxID > 0)
							docControl.CreateFaxOutComponent(faxID);
						else
							docControl.SetNullComponent();
						break;
					case Misc.ContextMode.SystemFolder:
						docControl.NeedToRefresh += docControl_NeedToRefresh;
						docControl.UseLock = true;
						break;
					default:
						docControl.NeedToRefresh += docControl_NeedToRefresh;
						break;
				}
				splitContainer.SendToBack();

				this.Text += (this.docControl.IsReadonly ? " (" + Environment.StringResources.GetString("ReadOnly").Trim().ToLower() + ")" : "");

				if(this.context.Mode == Misc.ContextMode.SystemFolder)
					docControl.Visible = true;
				toolStripSeparator3.Visible = toolStripButtonProperty.Available;
				toolStripButtonSendFax.Enabled = Environment.IsFaxSender(currentDocID);
				UpdateNavigation();
				toolStripComboBoxZoom.Text = zoom;
				sBPCount.Text = "1";
				toolStripMenuItemJoin.Enabled = false;
				StatusBar_UpdatePage();


				tf = Lib.Win.Document.Environment.GetTmpFileByValue(fileName);
				if(tf != null)
				{
					tf.Window = this;
					tf.LinkCnt++;
					this.Text = tf.DocString;// + (tf.Modified ? " (" + StringResources.Changed.Trim() + ")" : "");
					tf.OnModified += new EventHandler(tf_OnModified);
					tf.AscBeforeClose = true;
					this.context = new Context(Misc.ContextMode.SystemFolder);
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public SubFormDialog(string fileName, string zoom, string DocString, Context context)
			: this(fileName, zoom, DocString, context, 0)
		{
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			if(docChangedReceiver != null)
			{
				docChangedReceiver.Exit();
				docChangedReceiver.Received -= receiver_Received;
				docChangedReceiver = null;
			}
			if(receiverTimer != null)
			{
				receiverTimer.Stop();
				receiverTimer.Elapsed -= receiverTimer_Elapsed;
				receiverTimer = null;
			}
			if(subCmdManager != null)
			{
				subCmdManager.Dispose();
				subCmdManager = null;
			}
			if(optShowPagesPanel != null)
			{
				optShowPagesPanel.ValueChanged -= ShowPagesPanel;
				optShowPagesPanel = null;
			}
			if(subLayout != null)
				subLayout = null;

			if(keyLocker != null)
				keyLocker = null;
			if(disposing)
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubFormDialog));
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.panelDoc = new System.Windows.Forms.Panel();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.docControl = new Kesco.Lib.Win.Document.Controls.DocControl();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.toolStripImageList = new System.Windows.Forms.ImageList(this.components);
			this.toolStripButtonReturn = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonProperty = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonSavePart = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonSaveSelected = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonThumb = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonWeb = new System.Windows.Forms.ToolStripButton();
			this.toolStripSplitButtonMessage = new Kesco.Lib.Win.Document.Items.ToolStripSplitButtonCheckable();
			this.ToolStripMenuItemNewWindow = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItemLeft = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItemBottom = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonBefore = new System.Windows.Forms.ToolStripButton();
			this.toolStripTextBoxPageNum = new System.Windows.Forms.ToolStripTextBox();
			this.toolStripButtonNext = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripComboBoxZoom = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripButtonZoomIn = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonZoomOut = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonZoomToSelection = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonHand = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonArrow = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonSelect = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonStamp = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDSP = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonRotateLeft = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonRotateRight = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonPrint = new System.Windows.Forms.ToolStripSplitButton();
			this.ToolStripMenuItemPrintPage = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItemPrintImage = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItemPrintDoc = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripButtonSendMessage = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonSendFax = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonLinks = new System.Windows.Forms.ToolStripButton();
			this.sBPName = new System.Windows.Forms.StatusBarPanel();
			this.sBPCount = new System.Windows.Forms.StatusBarPanel();
			this.sBPPage = new System.Windows.Forms.StatusBarPanel();
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.statusBarPanelProtected = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanelDSP = new System.Windows.Forms.StatusBarPanel();
			this.linkContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mIMain = new System.Windows.Forms.ToolStripMenuItem();
			this.mIArchive = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemNewDocEform = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemScanDocImage = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemLinkEform = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemJoin = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.toolStripMenuItemDoc = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSave = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSavePart = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSaveSelected = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSaveStamp = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemAddEform = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemAddToWork = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemEditPlaces = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemEndWork = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemEditDesc = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSpam = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemProperties = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemGotoPerson = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemDocPartDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemDelFrom = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemDelFromSearch = new System.Windows.Forms.ToolStripMenuItem();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.opinionControl = new Kesco.Lib.Win.Opinion.OpinionControl();
			this.toolStripContainer1.SuspendLayout();
			this.panelDoc.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.SuspendLayout();
			this.toolStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sBPName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sBPCount)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sBPPage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelProtected)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelDSP)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.ContentPanel
			// 
			resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
			resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
			this.toolStripContainer1.Name = "toolStripContainer1";
			// 
			// panelDoc
			// 
			this.panelDoc.Controls.Add(this.splitContainer);
			this.panelDoc.Controls.Add(this.docControl);
			resources.ApplyResources(this.panelDoc, "panelDoc");
			this.panelDoc.Name = "panelDoc";
			// 
			// splitContainer
			// 
			resources.ApplyResources(this.splitContainer, "splitContainer");
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Panel1Collapsed = true;
			// 
			// docControl
			// 
			this.docControl.AlwaysShow = true;
			this.docControl.AnnotationDraw = false;
			this.docControl.CurDocString = null;
			this.docControl.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			resources.ApplyResources(this.docControl, "docControl");
			this.docControl.DocumentID = 0;
			this.docControl.EmpName = null;
			this.docControl.ForceRelicate = false;
			this.docControl.ImageID = -1;
			this.docControl.ImagesPanelOrientation = Kesco.Lib.Win.ImageControl.ImageControl.TypeThumbnailPanelOrientation.Left;
			this.docControl.IsEditNotes = false;
			this.docControl.IsMain = false;
			this.docControl.IsMoveImage = true;
			this.docControl.Name = "docControl";
			this.docControl.Page = 0;
			this.docControl.PersonParamStr = "clid=4&return=1";
			this.docControl.SelectionMode = false;
			this.docControl.ShowThumbPanel = true;
			this.docControl.ShowWebPanel = false;
			this.docControl.SplinterPlace = new System.Drawing.Point(200, 130);
			this.docControl.Subscribe = new System.Guid("00000000-0000-0000-0000-000000000000");
			this.docControl.WatchOnFile = false;
			this.docControl.Zoom = 100;
			this.docControl.ZoomText = "";
			this.docControl.VarListIndexChange += new System.EventHandler(this.docControl_VarListIndexChange);
			this.docControl.PageChanged += new System.EventHandler(this.docControl_PageChanged);
			this.docControl.DocChanged += new Kesco.Lib.Win.Document.Components.DocumentSavedEventHandle(this.docControl_DocChanged);
			this.docControl.LoadComplete += new System.EventHandler(this.docControl_LoadComplete);
			this.docControl.ToolSelected += new Kesco.Lib.Win.ImageControl.ImageControl.ToolSelectedHandler(this.docControl_ToolSelected);
			this.docControl.ImageSigned += new System.EventHandler(this.docControl_ImageSigned);
			// 
			// toolStrip
			// 
			this.toolStrip.AllowItemReorder = true;
			this.toolStrip.ImageList = this.toolStripImageList;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonReturn,
            this.toolStripSeparator1,
            this.toolStripButtonProperty,
            this.toolStripButtonSave,
            this.toolStripButtonSavePart,
            this.toolStripButtonSaveSelected,
            this.toolStripSeparator3,
            this.toolStripButtonThumb,
            this.toolStripButtonWeb,
            this.toolStripSplitButtonMessage,
            this.toolStripSeparator4,
            this.toolStripButtonBefore,
            this.toolStripTextBoxPageNum,
            this.toolStripButtonNext,
            this.toolStripSeparator5,
            this.toolStripComboBoxZoom,
            this.toolStripButtonZoomIn,
            this.toolStripButtonZoomOut,
            this.toolStripButtonZoomToSelection,
            this.toolStripSeparator6,
            this.toolStripButtonHand,
            this.toolStripButtonArrow,
            this.toolStripButtonSelect,
            this.toolStripButtonStamp,
            this.toolStripButtonDSP,
            this.toolStripSeparator9,
            this.toolStripButtonRotateLeft,
            this.toolStripButtonRotateRight,
            this.toolStripSeparator7,
            this.toolStripButtonPrint,
            this.toolStripButtonSendMessage,
            this.toolStripButtonSendFax,
            this.toolStripSeparator8,
            this.toolStripButtonLinks});
			this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
			resources.ApplyResources(this.toolStrip, "toolStrip");
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// toolStripImageList
			// 
			this.toolStripImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("toolStripImageList.ImageStream")));
			this.toolStripImageList.TransparentColor = System.Drawing.SystemColors.Control;
			this.toolStripImageList.Images.SetKeyName(0, "");
			this.toolStripImageList.Images.SetKeyName(1, "");
			this.toolStripImageList.Images.SetKeyName(2, "");
			this.toolStripImageList.Images.SetKeyName(3, "");
			this.toolStripImageList.Images.SetKeyName(4, "");
			this.toolStripImageList.Images.SetKeyName(5, "");
			this.toolStripImageList.Images.SetKeyName(6, "");
			this.toolStripImageList.Images.SetKeyName(7, "");
			this.toolStripImageList.Images.SetKeyName(8, "");
			this.toolStripImageList.Images.SetKeyName(9, "");
			this.toolStripImageList.Images.SetKeyName(10, "");
			this.toolStripImageList.Images.SetKeyName(11, "");
			this.toolStripImageList.Images.SetKeyName(12, "");
			this.toolStripImageList.Images.SetKeyName(13, "");
			this.toolStripImageList.Images.SetKeyName(14, "");
			this.toolStripImageList.Images.SetKeyName(15, "");
			this.toolStripImageList.Images.SetKeyName(16, "");
			this.toolStripImageList.Images.SetKeyName(17, "");
			this.toolStripImageList.Images.SetKeyName(18, "");
			this.toolStripImageList.Images.SetKeyName(19, "");
			this.toolStripImageList.Images.SetKeyName(20, "");
			this.toolStripImageList.Images.SetKeyName(21, "");
			this.toolStripImageList.Images.SetKeyName(22, "Arrow.gif");
			this.toolStripImageList.Images.SetKeyName(23, "stamp.png");
			this.toolStripImageList.Images.SetKeyName(24, "");
			this.toolStripImageList.Images.SetKeyName(25, "ДСП.png");
			// 
			// toolStripButtonReturn
			// 
			this.toolStripButtonReturn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonReturn, "toolStripButtonReturn");
			this.toolStripButtonReturn.Name = "toolStripButtonReturn";
			this.toolStripButtonReturn.Click += new System.EventHandler(this.toolStripMenuItemToMain_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			// 
			// toolStripButtonProperty
			// 
			this.toolStripButtonProperty.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonProperty, "toolStripButtonProperty");
			this.toolStripButtonProperty.Name = "toolStripButtonProperty";
			this.toolStripButtonProperty.Click += new System.EventHandler(this.toolStripButtonProperty_Click);
			// 
			// toolStripButtonSave
			// 
			this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonSave, "toolStripButtonSave");
			this.toolStripButtonSave.Name = "toolStripButtonSave";
			this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
			// 
			// toolStripButtonSavePart
			// 
			this.toolStripButtonSavePart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonSavePart, "toolStripButtonSavePart");
			this.toolStripButtonSavePart.Name = "toolStripButtonSavePart";
			// 
			// toolStripButtonSaveSelected
			// 
			this.toolStripButtonSaveSelected.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonSaveSelected, "toolStripButtonSaveSelected");
			this.toolStripButtonSaveSelected.Name = "toolStripButtonSaveSelected";
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
			// 
			// toolStripButtonThumb
			// 
			this.toolStripButtonThumb.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonThumb, "toolStripButtonThumb");
			this.toolStripButtonThumb.Name = "toolStripButtonThumb";
			this.toolStripButtonThumb.Click += new System.EventHandler(this.toolStripButtonThumb_Click);
			// 
			// toolStripButtonWeb
			// 
			this.toolStripButtonWeb.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonWeb, "toolStripButtonWeb");
			this.toolStripButtonWeb.Name = "toolStripButtonWeb";
			// 
			// toolStripSplitButtonMessage
			// 
			this.toolStripSplitButtonMessage.Checked = false;
			this.toolStripSplitButtonMessage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripSplitButtonMessage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemNewWindow,
            this.ToolStripMenuItemLeft,
            this.ToolStripMenuItemBottom});
			resources.ApplyResources(this.toolStripSplitButtonMessage, "toolStripSplitButtonMessage");
			this.toolStripSplitButtonMessage.Name = "toolStripSplitButtonMessage";
			this.toolStripSplitButtonMessage.Tag = "Отобразить сообщения по документу";
			// 
			// ToolStripMenuItemNewWindow
			// 
			this.ToolStripMenuItemNewWindow.Name = "ToolStripMenuItemNewWindow";
			resources.ApplyResources(this.ToolStripMenuItemNewWindow, "ToolStripMenuItemNewWindow");
			this.ToolStripMenuItemNewWindow.Click += new System.EventHandler(this.toolStripMenuItemNewWindow_Click);
			// 
			// ToolStripMenuItemLeft
			// 
			this.ToolStripMenuItemLeft.Name = "ToolStripMenuItemLeft";
			resources.ApplyResources(this.ToolStripMenuItemLeft, "ToolStripMenuItemLeft");
			this.ToolStripMenuItemLeft.Click += new System.EventHandler(this.toolStripMenuItemLeft_Click);
			// 
			// ToolStripMenuItemBottom
			// 
			this.ToolStripMenuItemBottom.Name = "ToolStripMenuItemBottom";
			resources.ApplyResources(this.ToolStripMenuItemBottom, "ToolStripMenuItemBottom");
			this.ToolStripMenuItemBottom.Click += new System.EventHandler(this.toolStripMenuItemBottom_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
			// 
			// toolStripButtonBefore
			// 
			this.toolStripButtonBefore.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonBefore, "toolStripButtonBefore");
			this.toolStripButtonBefore.Name = "toolStripButtonBefore";
			this.toolStripButtonBefore.Click += new System.EventHandler(this.toolStripButtonBefore_Click);
			// 
			// toolStripTextBoxPageNum
			// 
			this.toolStripTextBoxPageNum.Name = "toolStripTextBoxPageNum";
			resources.ApplyResources(this.toolStripTextBoxPageNum, "toolStripTextBoxPageNum");
			this.toolStripTextBoxPageNum.TextChanged += new System.EventHandler(this.toolStripTextBoxPageNum_TextChanged);
			// 
			// toolStripButtonNext
			// 
			this.toolStripButtonNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonNext, "toolStripButtonNext");
			this.toolStripButtonNext.Name = "toolStripButtonNext";
			this.toolStripButtonNext.Click += new System.EventHandler(this.toolStripButtonNext_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
			// 
			// toolStripComboBoxZoom
			// 
			this.toolStripComboBoxZoom.Items.AddRange(new object[] {
            resources.GetString("toolStripComboBoxZoom.Items"),
            resources.GetString("toolStripComboBoxZoom.Items1"),
            resources.GetString("toolStripComboBoxZoom.Items2"),
            resources.GetString("toolStripComboBoxZoom.Items3"),
            resources.GetString("toolStripComboBoxZoom.Items4"),
            resources.GetString("toolStripComboBoxZoom.Items5"),
            resources.GetString("toolStripComboBoxZoom.Items6"),
            resources.GetString("toolStripComboBoxZoom.Items7")});
			this.toolStripComboBoxZoom.Name = "toolStripComboBoxZoom";
			resources.ApplyResources(this.toolStripComboBoxZoom, "toolStripComboBoxZoom");
			this.toolStripComboBoxZoom.TextChanged += new System.EventHandler(this.toolStripComboBoxZoom_TextChanged);
			// 
			// toolStripButtonZoomIn
			// 
			this.toolStripButtonZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonZoomIn, "toolStripButtonZoomIn");
			this.toolStripButtonZoomIn.Name = "toolStripButtonZoomIn";
			this.toolStripButtonZoomIn.Click += new System.EventHandler(this.toolStripButtonZoomIn_Click);
			// 
			// toolStripButtonZoomOut
			// 
			this.toolStripButtonZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonZoomOut, "toolStripButtonZoomOut");
			this.toolStripButtonZoomOut.Name = "toolStripButtonZoomOut";
			this.toolStripButtonZoomOut.Click += new System.EventHandler(this.toolStripButtonZoomOut_Click);
			// 
			// toolStripButtonZoomToSelection
			// 
			this.toolStripButtonZoomToSelection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonZoomToSelection, "toolStripButtonZoomToSelection");
			this.toolStripButtonZoomToSelection.Name = "toolStripButtonZoomToSelection";
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
			// 
			// toolStripButtonHand
			// 
			this.toolStripButtonHand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonHand, "toolStripButtonHand");
			this.toolStripButtonHand.Name = "toolStripButtonHand";
			// 
			// toolStripButtonArrow
			// 
			this.toolStripButtonArrow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonArrow, "toolStripButtonArrow");
			this.toolStripButtonArrow.Name = "toolStripButtonArrow";
			// 
			// toolStripButtonSelect
			// 
			this.toolStripButtonSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonSelect, "toolStripButtonSelect");
			this.toolStripButtonSelect.Name = "toolStripButtonSelect";
			// 
			// toolStripButtonStamp
			// 
			this.toolStripButtonStamp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonStamp, "toolStripButtonStamp");
			this.toolStripButtonStamp.Name = "toolStripButtonStamp";
			// 
			// toolStripButtonDSP
			// 
			this.toolStripButtonDSP.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonDSP, "toolStripButtonDSP");
			this.toolStripButtonDSP.Name = "toolStripButtonDSP";
			// 
			// toolStripSeparator9
			// 
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
			// 
			// toolStripButtonRotateLeft
			// 
			this.toolStripButtonRotateLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonRotateLeft, "toolStripButtonRotateLeft");
			this.toolStripButtonRotateLeft.Name = "toolStripButtonRotateLeft";
			this.toolStripButtonRotateLeft.Click += new System.EventHandler(this.toolStripButtonRotateLeft_Click);
			// 
			// toolStripButtonRotateRight
			// 
			this.toolStripButtonRotateRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonRotateRight, "toolStripButtonRotateRight");
			this.toolStripButtonRotateRight.Name = "toolStripButtonRotateRight";
			this.toolStripButtonRotateRight.Click += new System.EventHandler(this.toolStripButtonRotateRight_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
			// 
			// toolStripButtonPrint
			// 
			this.toolStripButtonPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonPrint.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemPrintPage,
            this.ToolStripMenuItemPrintImage,
            this.ToolStripMenuItemPrintDoc});
			resources.ApplyResources(this.toolStripButtonPrint, "toolStripButtonPrint");
			this.toolStripButtonPrint.Name = "toolStripButtonPrint";
			// 
			// ToolStripMenuItemPrintPage
			// 
			this.ToolStripMenuItemPrintPage.Name = "ToolStripMenuItemPrintPage";
			resources.ApplyResources(this.ToolStripMenuItemPrintPage, "ToolStripMenuItemPrintPage");
			// 
			// ToolStripMenuItemPrintImage
			// 
			this.ToolStripMenuItemPrintImage.Name = "ToolStripMenuItemPrintImage";
			resources.ApplyResources(this.ToolStripMenuItemPrintImage, "ToolStripMenuItemPrintImage");
			// 
			// ToolStripMenuItemPrintDoc
			// 
			this.ToolStripMenuItemPrintDoc.Name = "ToolStripMenuItemPrintDoc";
			resources.ApplyResources(this.ToolStripMenuItemPrintDoc, "ToolStripMenuItemPrintDoc");
			// 
			// toolStripButtonSendMessage
			// 
			this.toolStripButtonSendMessage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonSendMessage, "toolStripButtonSendMessage");
			this.toolStripButtonSendMessage.Name = "toolStripButtonSendMessage";
			// 
			// toolStripButtonSendFax
			// 
			this.toolStripButtonSendFax.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonSendFax, "toolStripButtonSendFax");
			this.toolStripButtonSendFax.Name = "toolStripButtonSendFax";
			this.toolStripButtonSendFax.Click += new System.EventHandler(this.toolStripButtonSendFax_Click);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
			// 
			// toolStripButtonLinks
			// 
			this.toolStripButtonLinks.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolStripButtonLinks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonLinks, "toolStripButtonLinks");
			this.toolStripButtonLinks.Name = "toolStripButtonLinks";
			this.toolStripButtonLinks.Click += new System.EventHandler(this.toolStripButtonLinks_Click);
			// 
			// sBPName
			// 
			resources.ApplyResources(this.sBPName, "sBPName");
			// 
			// sBPCount
			// 
			resources.ApplyResources(this.sBPCount, "sBPCount");
			// 
			// sBPPage
			// 
			resources.ApplyResources(this.sBPPage, "sBPPage");
			this.sBPPage.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			// 
			// statusBar
			// 
			resources.ApplyResources(this.statusBar, "statusBar");
			this.statusBar.Name = "statusBar";
			this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.sBPName,
            this.statusBarPanelProtected,
            this.statusBarPanelDSP,
            this.sBPCount,
            this.sBPPage});
			this.statusBar.ShowPanels = true;
			// 
			// statusBarPanelProteced
			// 
			resources.ApplyResources(this.statusBarPanelProtected, "statusBarPanelProtected");
			// 
			// statusBarPanelDSP
			// 
			resources.ApplyResources(this.statusBarPanelDSP, "statusBarPanelDSP");
			// 
			// linkContextMenu
			// 
			this.linkContextMenu.Name = "linkContextMenu";
			resources.ApplyResources(this.linkContextMenu, "linkContextMenu");
			// 
			// mIMain
			// 
			this.mIMain.MergeIndex = 0;
			this.mIMain.Name = "mIMain";
			resources.ApplyResources(this.mIMain, "mIMain");
			// 
			// mIArchive
			// 
			this.mIArchive.MergeIndex = 1;
			this.mIArchive.Name = "mIArchive";
			resources.ApplyResources(this.mIArchive, "mIArchive");
			// 
			// toolStripSeparator10
			// 
			this.toolStripSeparator10.Name = "toolStripSeparator10";
			resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
			// 
			// toolStripMenuItemNewDocEform
			// 
			this.toolStripMenuItemNewDocEform.Name = "toolStripMenuItemNewDocEform";
			resources.ApplyResources(this.toolStripMenuItemNewDocEform, "toolStripMenuItemNewDocEform");
			this.toolStripMenuItemNewDocEform.Click += new System.EventHandler(this.toolStripMenuItemNewDocEform_Click);
			// 
			// toolStripMenuItemScanDocImage
			// 
			this.toolStripMenuItemScanDocImage.Name = "toolStripMenuItemScanDocImage";
			resources.ApplyResources(this.toolStripMenuItemScanDocImage, "toolStripMenuItemScanDocImage");
			this.toolStripMenuItemScanDocImage.Click += new System.EventHandler(this.toolStripMenuItemScanDocImage_Click);
			// 
			// toolStripMenuItemLinkEform
			// 
			this.toolStripMenuItemLinkEform.Name = "toolStripMenuItemLinkEform";
			resources.ApplyResources(this.toolStripMenuItemLinkEform, "toolStripMenuItemLinkEform");
			// 
			// toolStripMenuItemJoin
			// 
			this.toolStripMenuItemJoin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripMenuItemJoin.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mIMain,
            this.mIArchive});
			resources.ApplyResources(this.toolStripMenuItemJoin, "toolStripMenuItemJoin");
			this.toolStripMenuItemJoin.Name = "toolStripMenuItemJoin";
			this.toolStripMenuItemJoin.DropDownOpening += new System.EventHandler(this.toolStripButtonJoin_DropDownOpening);
			this.toolStripMenuItemJoin.Click += new System.EventHandler(this.toolStripButtonJoin_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.ImageList = this.toolStripImageList;
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemDoc,
            this.toolStripMenuItemJoin});
			resources.ApplyResources(this.menuStrip1, "menuStrip1");
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.menuStrip1.ShowItemToolTips = true;
			// 
			// toolStripMenuItemDoc
			// 
			this.toolStripMenuItemDoc.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemNewDocEform,
            this.toolStripMenuItemScanDocImage,
            this.toolStripMenuItemLinkEform,
            this.toolStripSeparator10,
            this.toolStripMenuItemSave,
            this.toolStripMenuItemSavePart,
            this.toolStripMenuItemSaveSelected,
            this.toolStripMenuItemSaveStamp,
            this.toolStripSeparator11,
            this.toolStripMenuItemAddEform,
            this.toolStripSeparator12,
            this.toolStripMenuItemAddToWork,
            this.toolStripMenuItemEditPlaces,
            this.toolStripMenuItemEndWork,
            this.toolStripSeparator13,
            this.toolStripMenuItemEditDesc,
            this.toolStripMenuItemSpam,
            this.toolStripSeparator14,
            this.toolStripMenuItemProperties,
            this.toolStripSeparator15,
            this.toolStripMenuItemGotoPerson,
            this.toolStripSeparator16,
            this.toolStripMenuItemDocPartDelete,
            this.toolStripMenuItemDelFrom,
            this.toolStripMenuItemDelFromSearch});
			this.toolStripMenuItemDoc.Name = "toolStripMenuItemDoc";
			resources.ApplyResources(this.toolStripMenuItemDoc, "toolStripMenuItemDoc");
			this.toolStripMenuItemDoc.DropDownOpening += new System.EventHandler(this.toolStripMenuItemDoc_DropDownOpening);
			// 
			// toolStripMenuItemSave
			// 
			this.toolStripMenuItemSave.Name = "toolStripMenuItemSave";
			resources.ApplyResources(this.toolStripMenuItemSave, "toolStripMenuItemSave");
			this.toolStripMenuItemSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
			// 
			// toolStripMenuItemSavePart
			// 
			this.toolStripMenuItemSavePart.Name = "toolStripMenuItemSavePart";
			resources.ApplyResources(this.toolStripMenuItemSavePart, "toolStripMenuItemSavePart");
			// 
			// toolStripMenuItemSaveSelected
			// 
			this.toolStripMenuItemSaveSelected.Name = "toolStripMenuItemSaveSelected";
			resources.ApplyResources(this.toolStripMenuItemSaveSelected, "toolStripMenuItemSaveSelected");
			// 
			// toolStripMenuItemSaveStamp
			// 
			this.toolStripMenuItemSaveStamp.Name = "toolStripMenuItemSaveStamp";
			resources.ApplyResources(this.toolStripMenuItemSaveStamp, "toolStripMenuItemSaveStamp");
			this.toolStripMenuItemSaveStamp.Click += new System.EventHandler(this.toolStripMenuItemSaveStamp_Click);
			// 
			// toolStripSeparator11
			// 
			this.toolStripSeparator11.Name = "toolStripSeparator11";
			resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
			// 
			// toolStripMenuItemAddEform
			// 
			this.toolStripMenuItemAddEform.Name = "toolStripMenuItemAddEform";
			resources.ApplyResources(this.toolStripMenuItemAddEform, "toolStripMenuItemAddEform");
			this.toolStripMenuItemAddEform.Click += new System.EventHandler(this.toolStripMenuItemAddEform_Click);
			// 
			// toolStripSeparator12
			// 
			this.toolStripSeparator12.Name = "toolStripSeparator12";
			resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
			// 
			// toolStripMenuItemAddToWork
			// 
			this.toolStripMenuItemAddToWork.Name = "toolStripMenuItemAddToWork";
			resources.ApplyResources(this.toolStripMenuItemAddToWork, "toolStripMenuItemAddToWork");
			this.toolStripMenuItemAddToWork.Click += new System.EventHandler(this.toolStripMenuItemAddToWork_Click);
			// 
			// toolStripMenuItemEditPlaces
			// 
			this.toolStripMenuItemEditPlaces.Name = "toolStripMenuItemEditPlaces";
			resources.ApplyResources(this.toolStripMenuItemEditPlaces, "toolStripMenuItemEditPlaces");
			this.toolStripMenuItemEditPlaces.Click += new System.EventHandler(this.toolStripMenuItemAddToWork_Click);
			// 
			// toolStripMenuItemEndWork
			// 
			this.toolStripMenuItemEndWork.Name = "toolStripMenuItemEndWork";
			resources.ApplyResources(this.toolStripMenuItemEndWork, "toolStripMenuItemEndWork");
			this.toolStripMenuItemEndWork.Click += new System.EventHandler(this.toolStripMenuItemEndWork_Click);
			// 
			// toolStripSeparator13
			// 
			this.toolStripSeparator13.Name = "toolStripSeparator13";
			resources.ApplyResources(this.toolStripSeparator13, "toolStripSeparator13");
			// 
			// toolStripMenuItemEditDesc
			// 
			this.toolStripMenuItemEditDesc.Name = "toolStripMenuItemEditDesc";
			resources.ApplyResources(this.toolStripMenuItemEditDesc, "toolStripMenuItemEditDesc");
			this.toolStripMenuItemEditDesc.Click += new System.EventHandler(this.toolStripMenuItemEditDesc_Click);
			// 
			// toolStripMenuItemSpam
			// 
			this.toolStripMenuItemSpam.Name = "toolStripMenuItemSpam";
			resources.ApplyResources(this.toolStripMenuItemSpam, "toolStripMenuItemSpam");
			this.toolStripMenuItemSpam.Click += new System.EventHandler(this.toolStripMenuItemSpam_Click);
			// 
			// toolStripSeparator14
			// 
			this.toolStripSeparator14.Name = "toolStripSeparator14";
			resources.ApplyResources(this.toolStripSeparator14, "toolStripSeparator14");
			// 
			// toolStripMenuItemProperties
			// 
			this.toolStripMenuItemProperties.Name = "toolStripMenuItemProperties";
			resources.ApplyResources(this.toolStripMenuItemProperties, "toolStripMenuItemProperties");
			this.toolStripMenuItemProperties.Click += new System.EventHandler(this.toolStripButtonProperty_Click);
			// 
			// toolStripSeparator15
			// 
			this.toolStripSeparator15.Name = "toolStripSeparator15";
			resources.ApplyResources(this.toolStripSeparator15, "toolStripSeparator15");
			// 
			// toolStripMenuItemGotoPerson
			// 
			this.toolStripMenuItemGotoPerson.Name = "toolStripMenuItemGotoPerson";
			resources.ApplyResources(this.toolStripMenuItemGotoPerson, "toolStripMenuItemGotoPerson");
			// 
			// toolStripSeparator16
			// 
			this.toolStripSeparator16.Name = "toolStripSeparator16";
			resources.ApplyResources(this.toolStripSeparator16, "toolStripSeparator16");
			// 
			// toolStripMenuItemDocPartDelete
			// 
			this.toolStripMenuItemDocPartDelete.Name = "toolStripMenuItemDocPartDelete";
			resources.ApplyResources(this.toolStripMenuItemDocPartDelete, "toolStripMenuItemDocPartDelete");
			this.toolStripMenuItemDocPartDelete.Click += new System.EventHandler(this.toolStripMenuItemDocPartDelete_Click);
			// 
			// toolStripMenuItemDelFrom
			// 
			this.toolStripMenuItemDelFrom.Name = "toolStripMenuItemDelFrom";
			resources.ApplyResources(this.toolStripMenuItemDelFrom, "toolStripMenuItemDelFrom");
			this.toolStripMenuItemDelFrom.Click += new System.EventHandler(this.toolStripMenuItemDelFrom_Click);
			// 
			// toolStripMenuItemDelFromSearch
			// 
			this.toolStripMenuItemDelFromSearch.Name = "toolStripMenuItemDelFromSearch";
			resources.ApplyResources(this.toolStripMenuItemDelFromSearch, "toolStripMenuItemDelFromSearch");
			this.toolStripMenuItemDelFromSearch.Click += new System.EventHandler(this.toolStripMenuItemDelFromSearch_Click);
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.toolStrip, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.opinionControl, 1, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
			// 
			// opinionControl
			// 
			resources.ApplyResources(this.opinionControl, "opinionControl");
			this.opinionControl.ConnectionString = null;
			this.opinionControl.Name = "opinionControl";
			// 
			// SubFormDialog
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.panelDoc);
			this.Controls.Add(this.toolStripContainer1);
			this.Controls.Add(this.statusBar);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.menuStrip1);
			this.DoubleBuffered = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "SubFormDialog";
			this.Activated += new System.EventHandler(this.SubFormDialog_Activated);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.SubFormDialog_Closing);
			this.Load += new System.EventHandler(this.SubFormDialog_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SubFormDialog_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SubFormDialog_KeyUp);
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.panelDoc.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sBPName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sBPCount)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sBPPage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelProtected)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelDSP)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		#region ImgEdit

		public void On_ZoomIn()
		{
			try
			{
				float zoom = docControl.Zoom * 2;
				float newZoom = (zoom < MainFormDialog.MaxZoom) ? zoom : MainFormDialog.MaxZoom;
				if(newZoom != docControl.Zoom)
					toolStripComboBoxZoom.Text = Convert.ToInt32(newZoom).ToString() + "%";
			}
			catch(Exception ex)
			{ Lib.Win.Data.Env.WriteToLog(ex); }
		}

		public void On_ZoomOut()
		{
			try
			{
				float zoom = docControl.Zoom / 2;
				float newZoom = (zoom > MainFormDialog.MinZoom) ? zoom : MainFormDialog.MinZoom;
				if(newZoom != docControl.Zoom)
					toolStripComboBoxZoom.Text = Convert.ToInt32(newZoom).ToString() + "%";
			}
			catch(Exception ex)
			{ Lib.Win.Data.Env.WriteToLog(ex); }
		}

		private void UpdateNavigation()
		{
			if(Disposing || IsDisposed)
				return;
			try
			{
				docControl.ShowAnnotationGroup(Missing.Value);
				bool canPrint = docControl.CanPrint && docControl.CanSendOut;
				bool sendFaxEnabled = Environment.IsFaxSender(currentDocID) && (imgID > 0) && docControl.CanSendOut;
				bool imageDisplayed = docControl.ImageDisplayed;

				if(toolStrip.InvokeRequired)
				{
					toolStrip.Invoke((MethodInvoker)(() =>
														  {
															  toolStripButtonPrint.Enabled = canPrint;
															  toolStripButtonSendFax.Enabled = sendFaxEnabled;
															  toolStripButtonThumb.Enabled =
																  toolStripComboBoxZoom.Enabled =
																  toolStripTextBoxPageNum.Enabled =
																  toolStripButtonZoomIn.Enabled =
																  toolStripButtonZoomOut.Enabled = imageDisplayed;
														  }));
				}
				else
				{
					toolStripButtonPrint.Enabled = canPrint;
					toolStripButtonSendFax.Enabled = sendFaxEnabled;
					toolStripButtonThumb.Enabled =
						toolStripComboBoxZoom.Enabled =
						toolStripTextBoxPageNum.Enabled =
						toolStripButtonZoomIn.Enabled =
						toolStripButtonZoomOut.Enabled = imageDisplayed;
				}
				if(imageDisplayed)
				{
					int page = docControl.Page;
					bool thereisnext = page != docControl.PageCount;
					bool isnotsigned = IsNotSigned();

					if(toolStrip.InvokeRequired)
					{
						toolStrip.Invoke((MethodInvoker)(() =>
															  {
																  toolStripTextBoxPageNum.Text = page.ToString();
																  toolStripButtonBefore.Enabled = page != 1;
																  toolStripButtonNext.Enabled = thereisnext;
																  toolStripButtonRotateLeft.Enabled = true;
																  toolStripButtonRotateRight.Enabled = true;
															  }));
					}
					else
					{
						toolStripTextBoxPageNum.Text = page.ToString();
						toolStripButtonBefore.Enabled = page != 1;
						toolStripButtonNext.Enabled = thereisnext;
						toolStripButtonRotateLeft.Enabled = true;
						toolStripButtonRotateRight.Enabled = true;
					}
				}
				else
				{
					if(toolStrip.InvokeRequired)
					{
						toolStrip.Invoke((MethodInvoker)(() =>
															  {
																  toolStripButtonRotateLeft.Enabled = false;
																  toolStripButtonRotateRight.Enabled = false;
																  toolStripTextBoxPageNum.Text = "";
																  toolStripButtonBefore.Enabled = false;
																  toolStripButtonNext.Enabled = false;
															  }));
					}
					else
					{
						toolStripButtonRotateLeft.Enabled = false;
						toolStripButtonRotateRight.Enabled = false;
						toolStripTextBoxPageNum.Text = "";
						toolStripButtonBefore.Enabled = false;
						toolStripButtonNext.Enabled = false;
					}
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void docControl_ToolSelected(object sender, ImageControl.ToolSelectedEventArgs e)
		{
			toolSelected = e.EventType;
		}

		private void docControl_ImageSigned(object sender, EventArgs e)
		{
			UpdateNavigation();
		}

		#endregion

		#region Load

		private void SubFormDialog_Load(object sender, EventArgs e)
		{
            LoadSize();
			if(currentDocID > 0)
			{
				docControl.LoadDocument(currentDocID, imgID, page);
				if(splitContainer!=null)
					splitContainer.Panel2.Controls.Add(docControl);
				if(statusBarPanelProtected != null)

				statusBarPanelProtected.Text =
					1.Equals(
					Convert.ToInt32(Environment.DocData.GetField(
					Environment.DocData.ProtectedField, currentDocID))) ? 
					(Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru") ? 
					"Секретно" : "Private") : "";
				this.imgID = docControl.ImageID;
				toolStripButtonSendFax.Enabled = Environment.IsFaxSender(currentDocID) && (this.imgID > 0) && docControl.CanSendOut;
				toolStripButtonPrint.Enabled = docControl.CanPrint;
			}
			else if(!string.IsNullOrEmpty(fileName))
			{
				docControl.LoadFile(fileName, page);
			}

			UpdateNavigation();
			if(docControl.ImageID > 0)
				fileName = docControl.FileName;
			if(context.Mode == Misc.ContextMode.SystemFolder)
				docControl.WatchOnFile = true;
			if(currentDocID > 0)
				sBPCount.Text = docControl.ImageCount.ToString();
			StatusBar_UpdatePage();
			if(currentDocID > 0)
				RefreshLinks();
            this.ResizeEnd += new System.EventHandler(this.SubFormDialog_ResizeEnd);
			if(currentDocID > 0)
				TimerStart();
			RefreshStatusBar(sender, e);
			toolStripButtonLinks.Enabled = docControl.HasDocLinks;


            this.toolStripMenuItemAddImage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddImage.Name = "toolStripMenuItemAddImage";
            this.toolStripMenuItemAddImage.Text = Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message13") + "...";
            this.toolStripMenuItemAddImage.Click += new System.EventHandler(this.toolStripMenuItemAddImage_Click);
            this.toolStripMenuItemDoc.DropDownItems.Insert(this.toolStripMenuItemDoc.DropDownItems.IndexOf(this.toolStripMenuItemScanDocImage)+1, this.toolStripMenuItemAddImage);

            this.FormClosing += new FormClosingEventHandler(SubFormDialog_FormClosing);
		}

		private void LoadReg()
		{
			try
			{
				LoadSize();

                docControl.SplinterPlace = new Point(subLayout.LoadIntOption("SplitterY", docControl.SplinterPlace.X),
                        subLayout.LoadIntOption("SplitterX", docControl.SplinterPlace.Y));

				string noUse = subLayout.LoadStringOption("noUsePagesPanel", false.ToString());
				bool noUsePagesPanel = noUse.ToLower().Trim() == "true" || noUse.Trim() == "1";
				optShowPagesPanel = subLayout.LoadOption<String>("ShowPagesPanel", docControl.ShowThumbPanel.ToString());
				if(noUsePagesPanel)
				{
					optShowPagesPanel.Value = false.ToString();
					toolStripButtonWeb.Checked = docControl.ShowWebPanel;
					showMessageState = 0;
					showPushed = true;
				}
				else
				{
					bool val = Convert.ToBoolean(optShowPagesPanel.Value);
					docControl.ShowThumbPanel = val;
					toolStripButtonThumb.Checked = val;
					val = Convert.ToBoolean(subLayout.LoadStringOption("ShowWebPanel", docControl.ShowWebPanel.ToString()));
					docControl.ShowWebPanel = val;
					showMessageState = Environment.General.LoadIntOption("ShowInSubWindow", 0);
					showPushed = Convert.ToBoolean(Environment.General.LoadStringOption("ShowInSubWindowPushed", true.ToString()));
				}
				optShowPagesPanel.ValueChanged += ShowPagesPanel;
				splitContainer.SplitterDistance = subLayout.LoadIntOption(splitContainer.Orientation == Orientation.Horizontal ? "SplitterDistanceHeight" : "SplitterDistanceWidth", 200);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void LoadSize()
		{	
			Width = subLayout.LoadIntOption("Width", Width);
			Height = subLayout.LoadIntOption("Heigth", Height);

			if(this.IsHandleCreated)// && true.ToString().Equals(subLayout.LoadOption("Maximized", "False").Value, StringComparison.InvariantCultureIgnoreCase))
				//WindowState = FormWindowState.Maximized;
			{
				System.Timers.Timer tomer = new System.Timers.Timer(200);
				tomer.AutoReset = false;
				tomer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
				tomer.Start();
			}
		}

        void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			System.Timers.Timer timer = sender as System.Timers.Timer;
			if(timer != null)
			{
				timer.Stop();
                timer.Elapsed -= new ElapsedEventHandler(timer_Elapsed);
				timer.Dispose();
			}
			this.BeginInvoke((MethodInvoker)(() => 
            {
				if(showMessageState > 0 && showPushed)
					LoadMessage();
                if (true.ToString().Equals(subLayout.LoadStringOption("Maximized", "False"), StringComparison.InvariantCultureIgnoreCase))
                    WindowState = FormWindowState.Maximized;
                
                docControl.SplinterPlace = new Point(subLayout.LoadIntOption("SplitterY", docControl.SplinterPlace.X), subLayout.LoadIntOption("SplitterX", docControl.SplinterPlace.Y));
            }));
		}

		private void LoadMessage()
		{
			try
			{
				bool initGrid = (infoGrid == null);
				if(initGrid)
					infoGrid = new Grids.InfoGrid();

				((ISupportInitialize)(infoGrid)).BeginInit();
				SuspendLayout();

				if(initGrid)
				{
					infoGrid.MainForm = null;
					infoGrid.BackgroundColor = SystemColors.Window;
					infoGrid.DataMember = "";
					infoGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
					infoGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
				}

				if(splitContainer != null && splitContainer.Orientation == Orientation.Vertical)
					subLayout.OptionForced<int>("SplitterDistanceWidth").Value = splitContainer.SplitterDistance;
				if(splitContainer != null && splitContainer.Orientation == Orientation.Horizontal)
					subLayout.OptionForced<int>("SplitterDistanceHeight").Value = splitContainer.SplitterDistance;

				subLayout.Save();

				switch(showMessageState)
				{
					case 0:
						splitContainer.Orientation = Orientation.Vertical;
						splitContainer.Panel1Collapsed = true;
						splitContainer.Panel1.Controls.Remove(infoGrid);
						splitContainer.Panel1.Controls.Remove(docControl);
						splitContainer.Panel2.Controls.Remove(infoGrid);
						splitContainer.Panel2.Controls.Add(docControl);

						break;
					case 1:
						splitContainer.Orientation = Orientation.Vertical;
						splitContainer.Panel2.Controls.Remove(infoGrid);
						splitContainer.Panel1.Controls.Remove(docControl);
						splitContainer.Panel1Collapsed = false;
						splitContainer.Panel2Collapsed = false;
						splitContainer.Panel2.Controls.Add(docControl);
						splitContainer.Panel1.Controls.Add(infoGrid);
						break;
					case 2:
						splitContainer.Orientation = Orientation.Horizontal;
						splitContainer.Panel1.Controls.Remove(infoGrid);
						splitContainer.Panel2.Controls.Remove(docControl);
						splitContainer.Panel1Collapsed = false;
						splitContainer.Panel2Collapsed = false;
						splitContainer.Panel1.Controls.Add(docControl);
						splitContainer.Panel2.Controls.Add(infoGrid);
						break;
				}
				splitContainer.SplitterDistance = subLayout.LoadIntOption(splitContainer.Orientation == Orientation.Horizontal ? "SplitterDistanceHeight" : "SplitterDistanceWidth", 200);

				if(initGrid)
				{
					infoGrid.Dock = DockStyle.Fill;
					int infoGridHeight = subLayout.LoadIntOption("InfoGridHeight", 200);
					int infoGridWidth = subLayout.LoadIntOption("InfoGridWidth", 200);
					if(infoGridHeight > ClientSize.Height - docControl.VarListHeight - 100)
						infoGridHeight = ClientSize.Height - docControl.VarListHeight - 100;
					if(infoGridWidth > ClientSize.Width - 100)
						infoGridWidth = ClientSize.Width - 100;
					infoGrid.GridColor = infoGrid.BackgroundColor;
					infoGrid.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
					infoGrid.ImageTime = new DateTime(((0)));
					infoGrid.Location = new Point(0, 0);
					infoGrid.Name = "infoGrid";
					infoGrid.ReadOnly = true;
					infoGrid.RowHeadersVisible = false;
					infoGrid.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
					infoGrid.DefaultCellStyle.SelectionForeColor = SystemColors.Window;
					infoGrid.Size = new Size(infoGridWidth, infoGridHeight);
					infoGrid.Style = null;
					infoGrid.TabIndex = 0;
				}

				((ISupportInitialize)(infoGrid)).EndInit();
				ResumeLayout(false);

				// инициализация инфогрида
				if(initGrid)
				{
					infoGrid.Init(Environment.Layout);
					infoGrid.LoadInfo(currentDocID);
				}
				else
					infoGrid.Invalidate();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		#endregion

		#region StatusBar

		private void StatusBar_UpdatePage()
		{
			try
			{
				if(sBPName != null && context != null && context.Mode == Misc.ContextMode.SystemFolder)
				{
					string sbpNameText = sBPName.Text;
					if(docControl.IsPDFMode)
						sbpNameText = string.Concat(Environment.StringResources.GetString("Document"), "PDF");
					else if(docControl.DocumentID > 0 && docControl.ImageID == 0)
						sbpNameText = Lib.Win.Document.Environment.StringResources.GetString("eform");
					else if(!string.IsNullOrEmpty(docControl.FileName))
						sbpNameText = string.Concat(Lib.Win.Document.Environment.StringResources.GetString("Image"), " ",
													docControl.GetImageType());

					if(statusBar.InvokeRequired)
						statusBar.Invoke((MethodInvoker)(() => sBPName.Text = sbpNameText));
					else
						sBPName.Text = sbpNameText;
				}

				string sbppageText = docControl.ImageDisplayed
										 ? string.Concat(Environment.StringResources.GetString("Page"),
														 docControl.Page,
														 Environment.StringResources.GetString("From"),
														 docControl.PageCount)
										 : string.Empty;

				string sbppageToolTip = sBPPage.Text != "" && docControl.PageCount > 0 && docControl.Page > 0
											? string.Format(Environment.StringResources.GetString("Pages"),
															docControl.Page, docControl.PageCount)
											: string.Empty;

				if(statusBar.InvokeRequired)
				{
					statusBar.Invoke((MethodInvoker)(() => { sBPPage.Text = sbppageText; statusBarPanelDSP.Text = !docControl.IsSignInternal ? "" : "ДСП"; }));
					statusBar.Invoke((MethodInvoker)(() => sBPPage.ToolTipText = sbppageToolTip));
				}
				else
				{
					sBPPage.Text = sbppageText;
					statusBarPanelDSP.Text = !docControl.IsSignInternal ? "" : "ДСП";
					sBPPage.ToolTipText = sbppageToolTip;
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void RefreshStatusBar(object sender, EventArgs args)
		{
			if(statusBar.Width <= 0)
				return;
			try
			{
				// 0 - doc info
				StatusBarPanel panel = statusBar.Panels[0];
				panel.Width = statusBar.Width - statusBar.Panels[1].Width - statusBar.Panels[2].Width - statusBar.Panels[3].Width - statusBar.Panels[4].Width - 20;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		#endregion

		#region SubFormDialogEvent

        private void SubFormDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
			try
			{
				SaveFormState();

				// removing docID from global list
				Environment.RemoveOpenDoc(currentDocID);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void SaveFormState()
		{
			if(InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(SaveFormState));
				return;
			}
			if(!Visible || WindowState == FormWindowState.Minimized)
				return;
			try
			{
				if(subLayout != null)
				{
					if(WindowState == FormWindowState.Maximized)
                        subLayout.OptionForced<string>("Maximized").Value = true.ToString();
					else
					{
						subLayout.OptionForced<int>("Width").Value = Width;
						subLayout.OptionForced<int>("Heigth").Value = Height;
                        subLayout.OptionForced<string>("Maximized").Value = false.ToString();
					}

					if(docControl != null)
					{
						subLayout.OptionForced<int>("SplitterY").Value = docControl.SplinterPlace.X;
						subLayout.OptionForced<int>("SplitterX").Value = docControl.SplinterPlace.Y;
						subLayout.OptionForced<string>("ShowWebPanel").Value = docControl.ShowWebPanel.ToString();
						if(optShowPagesPanel != null)
							optShowPagesPanel.Value = docControl.ShowThumbPanel.ToString();
					}
					if(infoGrid != null)
					{
						switch(showMessageState)
						{
							case 1:
								subLayout.OptionForced<int>("InfoGridWidth").Value = infoGrid.Width;
								break;
							case 2:
								subLayout.OptionForced<int>("InfoGridHeight").Value = infoGrid.Height;
								break;
						}
					}
					if(splitContainer != null)
					{
						switch(showMessageState)
						{
							case 1:
								subLayout.OptionForced<int>("SplitterDistanceWidth").Value = splitContainer.SplitterDistance;
								break;
							case 2:
								subLayout.OptionForced<int>("SplitterDistanceHeight").Value = splitContainer.SplitterDistance;
								break;
						}
					}
					subLayout.Save();
				}
				if(Environment.General != null)
				{
					Environment.General.OptionForced<int>("ShowInSubWindow").Value = showMessageState;
					Environment.General.OptionForced<string>("ShowInSubWindowPushed").Value = showPushed.ToString();
					Environment.General.Save();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}

		}

		private void SubFormDialog_Activated(object sender, EventArgs e)
		{
			try
			{
				if(currentDocID > 0)
					RefreshLinks();
				toolStripButtonSave.ImageIndex = (currentDocID > 0) ? 21 : 17;
				toolStripButtonSave.ToolTipText = (currentDocID > 0) ? Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message5")
													  : (new ResourceManager(typeof(SubFormDialog))).GetString("toolStripButtonSave.ToolTipText");

				toolStripButtonLinks.Enabled = docControl.HasDocLinks;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void ReloadDoc(int docID)
		{
			try
			{
                KeyValuePair<string, Lib.Win.Document.Objects.TmpFile> tf;
                if (!string.IsNullOrEmpty(fileName) && Lib.Win.Document.Environment.TmpFilesContains(fileName))
                {
                    tf = Lib.Win.Document.Environment.GetTmpFilePair(fileName);
                    if (tf.Value != null && tf.Value.Window != null && tf.Value.Window.Equals(this))
                    {
                        tf.Value.OnModified -= new EventHandler(tf_OnModified);
                        tf.Value.Modified = false;
                        tf.Value.LinkCnt--;
                        tf.Value.Window = null;
                    }
                }

			    currentDocID = docID;
				Environment.AddOpenDoc(currentDocID, this);
				docString = DBDocString.Format(currentDocID);
				Text = Regex.Replace(docString, @"[\n\r]", " ");
				docControl.CurDocString = Regex.Replace(docString, @"[\n\r]", " ");

                // Переподписка
			    if (docChangedReceiver != null)
			    {
                    var succeed = docChangedReceiver.Update(currentDocID);

			        if (!succeed)
			        {
                        docChangedReceiver.Exit();
                        docChangedReceiver.Received -= receiver_Received;
                        docChangedReceiver = null;
			        }
			    }

                if (docChangedReceiver == null)
			    {
                    try
                    {
                        docChangedReceiver = new Lib.Win.Receive.Receive(Environment.ConnectionStringDocument, Environment.SubscribeTable, 14, currentDocID, 3);
                        docChangedReceiver.ReservePort();
                    }
                    catch (Exception ex)
                    {
                        Lib.Win.Data.Env.WriteToLog(ex, "Не удалось установить подписку на изменения");
                    }
			    }

				docControl.DocumentID = currentDocID;
                if (currentDocID > 0) TimerStart();
				toolStrip.Visible = true;
				if(currentDocID > 0)
				{
					toolStripButtonSave.ImageIndex = 21;
					toolStripButtonSave.ToolTipText =
						Environment.StringResources.GetString("DocGrid.Styles.DBDocsStyle.Message5");
				}
				else
				{
					toolStripButtonSave.ImageIndex = 17;
					toolStripButtonSave.ToolTipText =
						(new ResourceManager(typeof(SubFormDialog))).GetString("toolStripButtonSave.ToolTipText");
				}
				UpdateNavigation();
				StatusBar_UpdatePage();
				sBPCount.Text = docControl.ImageCount.ToString();
				toolStripButtonSendFax.Enabled = Environment.IsFaxSender(currentDocID) && docControl.CanSendOut;
				RefreshLinks();

				toolStripButtonLinks.Enabled = docControl.HasDocLinks;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void ReloadFile(string _fileName)
		{
			if(fileName.Equals(_fileName, StringComparison.CurrentCultureIgnoreCase))
				return;
		    try
		    {
#if AdvancedLogging
		        Lib.Log.Logger.EnterMethod(this, "SubFormDialog ReloadFile  _fileName = " + _fileName);
#endif

		        int page = docControl.Page;

		        fileName = _fileName;
		        docControl.FileName = _fileName;

		        if (docControl.ImageDisplayed && page <= docControl.PageCount)
		            docControl.Page = page;

		        string DocString = string.Empty;
		        switch (context.Mode)
		        {
		            case Misc.ContextMode.Scaner:
		                // doc type
		                DocString = Environment.StringResources.GetString("ScanedDoc").ToLower();

		                var fi = new FileInfo(_fileName);
		                ScanInfo info = TextProcessor.ParseScanInfo(fi) ??
		                                new ScanInfo(fi.CreationTime, Path.GetFileNameWithoutExtension(fi.Name));

		                // date
		                DocString = TextProcessor.StuffSpace(DocString) + Environment.StringResources.GetString("Of") +
		                            info.Date.ToString("dd.MM.yyyy");

		                // description
		                DocString = TextProcessor.StuffSpace(DocString) + "(" + info.Descr + ")";

		                break;
					case Misc.ContextMode.SystemFolder:
		                DocString = _fileName;
		                break;
		        }
		        if (DocString != null)
		        {
		            Text = Regex.Replace(DocString, @"[\n\r]", " ");
		            docString = DocString;
		            docControl.CurDocString = DocString;

		            if (Lib.Win.Document.Environment.TmpFilesContains(this.fileName))
		            {
		                tf = Lib.Win.Document.Environment.GetTmpFile(this.fileName);
		                if (tf != null)
		                {
		                    this.Text = tf.DocString + (tf.Modified ? " (" + Environment.StringResources.GetString("Changed").Trim() + ")" : "");
		                    tf.OnModified += new EventHandler(tf_OnModified);
		                }
		            }
		        }

		        docControl.WatchOnFile = (docControl.DocumentID <= 0);
		    }
		    catch (Exception ex)
		    {
		        Lib.Win.Data.Env.WriteToLog(ex);
		    }
		    finally
		    {
#if AdvancedLogging
                Lib.Log.Logger.LeaveMethod(this, "SubFormDialog ReloadFile  _fileName = " + _fileName);
#endif
		    }
		}

		void tf_OnModified(object sender, EventArgs e)
		{
			docControl.LoadFile(fileName, docControl.Page);
		}

		#endregion

		#region User Work

		private void toolStripComboBoxZoom_TextChanged(object sender, EventArgs e)
		{
			if(Disposing || IsDisposed || docControl == null)
				return;
			try
			{
				string zoom = toolStripComboBoxZoom.Text;

				if(zoom == Environment.StringResources.GetString("ToWindow"))
				{
					docControl.FitTo(0);
					Environment.ZoomString = zoom;
				}
				else if(zoom == Environment.StringResources.GetString("ToWidth"))
					docControl.FitTo(1);
				else if(zoom == Environment.StringResources.GetString("ToHeigth"))
					docControl.FitTo(2);
				else
					try
					{
						bool percent = (zoom.IndexOf("%") != -1);
						zoom = zoom.Replace("%", "");
						int intZ = 0;
						if(!Int32.TryParse(zoom, out intZ))
							return;

						if(intZ < MainFormDialog.MinZoom && percent)
						{
							MainFormDialog.ErrorMessage(
								Environment.StringResources.GetString("MainForm.MainFormDialog.Zoom.Minimum") +
								string.Format("{0}%", MainFormDialog.MinZoom),
								Environment.StringResources.GetString("InputError"));
							toolStripComboBoxZoom.Text = string.Format("{0}%", MainFormDialog.MinZoom);
						}

						if(intZ > MainFormDialog.MaxZoom)
						{
							MainFormDialog.ErrorMessage(
								Environment.StringResources.GetString("MainForm.MainFormDialog.Zoom.Maximum") +
								string.Format("{0}%", MainFormDialog.MaxZoom),
								Environment.StringResources.GetString("InputError"));
							toolStripComboBoxZoom.Text = string.Format("{0}%", MainFormDialog.MaxZoom);
						}

						if(docControl.Zoom != intZ)
							docControl.Zoom = intZ;
					}
					catch(Exception ex)
					{
						Lib.Win.Data.Env.WriteToLog(ex);
						toolStripComboBoxZoom.Text = string.Format("{0}%", docControl.Zoom);
					}
				Environment.ZoomString = toolStripComboBoxZoom.Text;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void toolStripTextBoxPageNum_TextChanged(object sender, EventArgs e)
		{
			try
			{
				int page = 0;
				if(Int32.TryParse(toolStripTextBoxPageNum.Text, out page) && page > 0)
				{
					if(docControl.PageCount > 0 && page <= docControl.PageCount)
					{
						docControl.Page = page;
						toolStripTextBoxPageNum.Text = docControl.Page.ToString();
						return;
					}
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			if(!string.IsNullOrEmpty(toolStripTextBoxPageNum.Text) && docControl.ImageDisplayed && docControl.Page > 0)
				toolStripTextBoxPageNum.Text = docControl.Page.ToString();
		}

		public string DocPathString { get; set; }

		public bool ForceRaplicate
		{
			get { return docControl.ForceRelicate; }
			set { docControl.ForceRelicate = value; }
		}

		public void AddEForm()
		{
			docControl.CreateEForm(false, currentDocID);
		}

		#endregion

		#region Variant List

		private void ShowPagesPanel(object sender, Kesco.Lib.Win.Options.OptionEventArgs e)
		{
			if(toolStripButtonThumb == null)
				return;
			try
			{
				bool val = Convert.ToBoolean(e.Value);
				toolStripButtonThumb.Checked = val;
				docControl.ShowThumbPanel = val;
				toolStripComboBoxZoom_TextChanged(this, EventArgs.Empty);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public void Save()
		{
			try
			{
				if(currentDocID > 0)
				{
					var dialog = new Dialogs.MoveDocDialog(new[] { currentDocID }, curEmpID);
					dialog.Show();
				}
				else
				{
					docControl.TestImage(Lib.Win.Document.Environment.ActionBefore.Save);
					if(Environment.IsConnected)
						docControl.Save();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private bool CheckUDCImage()
		{
			return !Lib.Win.Document.Environment.DocIsPrintedFromUDC(fileName) ||
				   Path.GetDirectoryName(fileName).Equals(Path.GetFullPath(Path.GetTempPath() + "\\Documents")) &&
				   (docControl.GetImagePalette() == 1 || docControl.GetImagePalette() == 3) &&
				   docControl.ImageResolutionX != -1 && docControl.ImageResolutionY != -1 &&
				   docControl.ImageResolutionX <= 200 && docControl.ImageResolutionY <= 200;
		}

		#endregion

		#region CommandManager

		private void InitializeCommandManager(object sender, DoWorkEventArgs e)
		{
			try
			{
				// SavePart
				if(toolStripButtonSavePart != null && !subCmdManager.Commands.Contains("SavePart"))
				{
					subCmdManager.Commands.Add(new CommandManagement.Command(
												   "SavePart",
												   On_SavePart,
												   UpdateCommand_SavePart));

					subCmdManager.Commands["SavePart"].CommandInstances.Add(new Object[]
                                                                                {
                                                                                    toolStripButtonSavePart,
                                                                                    toolStripMenuItemSavePart
                                                                                });
				}

				// SendMessage
				if(toolStripButtonSendMessage != null && !subCmdManager.Commands.Contains("SendMessage"))
				{
					subCmdManager.Commands.Add(new CommandManagement.Command(
												   "SendMessage",
												   On_SendMessage,
												   UpdateCommand_SendMessage));

					subCmdManager.Commands["SendMessage"].CommandInstances.Add(new Object[] { toolStripButtonSendMessage });
				}

				// ShowWeb
				if(toolStripButtonWeb != null)
				{
					subCmdManager.Commands.Add(new CommandManagement.Command(
												   "ShowWeb",
												   On_ShowWeb,
												   UpdateCommand_ShowWeb));

					subCmdManager.Commands["ShowWeb"].CommandInstances.Add(new Object[] { toolStripButtonWeb });
				}

				if(toolStripSplitButtonMessage != null)
				{
					subCmdManager.Commands.Add(new CommandManagement.Command(
												   "ShowMessage",
												   On_ShowMessage,
												   UpdateCommand_ShowMessage));

					subCmdManager.Commands["ShowMessage"].CommandInstances.Add(new Object[] { toolStripSplitButtonMessage });
				}

				if(toolStripButtonPrint != null)
				{
					subCmdManager.Commands.Add(new CommandManagement.Command(
												   "Print",
												   On_Print,
												   UpdateCommand_Print));

					subCmdManager.Commands["Print"].CommandInstances.Add(new Object[]
                                                                             {
                                                                                 toolStripButtonPrint,
                                                                                 ToolStripMenuItemPrintPage
                                                                             });
				}

				// SaveSelected
				subCmdManager.Commands.Add(new CommandManagement.Command(
											   "SaveSelected",
											   On_SaveSelected,
											   UpdateCommand_SaveSelected));

				subCmdManager.Commands["SaveSelected"].CommandInstances.Add(new Object[]
                                                                                {
                                                                                    toolStripMenuItemSaveSelected,
                                                                                    toolStripButtonSaveSelected
                                                                                });


				subCmdManager.Commands.Add(new CommandManagement.Command(
											   "PrintImage",
											   On_PrintImage,
											   UpdateCommand_PrintImage));

				subCmdManager.Commands["PrintImage"].CommandInstances.Add(new Object[] { ToolStripMenuItemPrintImage });

				subCmdManager.Commands.Add(new CommandManagement.Command(
											   "PrintDoc",
											   On_PrintDoc,
											   UpdateCommand_PrintDoc));

				subCmdManager.Commands["PrintDoc"].CommandInstances.Add(new Object[] { ToolStripMenuItemPrintDoc });


				subCmdManager.Commands.Add(new CommandManagement.Command(
											   "DocLink",
											   On_DocLink,
											   UpdateCommand_DocLink));

				subCmdManager.Commands["DocLink"].CommandInstances.Add(new Object[] { mIArchive });

				// Select View
				subCmdManager.Commands.Add(new CommandManagement.Command(
											   "View",
											   On_View,
											   UpdateCommand_View));

				subCmdManager.Commands["View"].CommandInstances.Add(new Object[] { toolStripButtonHand });

				// Select Selection mode
				subCmdManager.Commands.Add(new CommandManagement.Command(
											   "Selection",
											   On_Selection,
											   UpdateCommand_Selection));

				subCmdManager.Commands["Selection"].CommandInstances.Add(new Object[] { toolStripButtonSelect });

				// Select Zoom to selection
				subCmdManager.Commands.Add(new CommandManagement.Command(
											   "ZoomToSelection",
											   On_ZoomSelection,
											   UpdateCommand_ZoomSelection));

				subCmdManager.Commands["ZoomToSelection"].CommandInstances.Add(new Object[] { toolStripButtonZoomToSelection });


				// Select Select Tool
				subCmdManager.Commands.Add(new CommandManagement.Command(
											   "Select",
											   On_Select,
											   UpdateCommand_Select));

				subCmdManager.Commands["Select"].CommandInstances.Add(new Object[] { toolStripButtonArrow });

				// Select ImageStamp
				subCmdManager.Commands.Add(new CommandManagement.Command(
											   "ImageStamp",
											   On_Stamp,
											   UpdateCommand_Stamp));

				subCmdManager.Commands["ImageStamp"].CommandInstances.Add(new Object[] { toolStripButtonStamp });

				// Select StampDSP
				subCmdManager.Commands.Add(new CommandManagement.Command(
											   "StampDSP",
											   On_StampDSP,
											   UpdateCommand_StampDSP));

				subCmdManager.Commands["StampDSP"].CommandInstances.Add(new Object[] { toolStripButtonDSP });
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		#endregion

		#region Commands

		public void On_DocLink(CommandManagement.Command cmd)
		{
			var searchDialog = new XmlSearchForm(currentDocID);
			searchDialog.DialogEvent += SearchDialog_DialogEvent;
			searchDialog.Show();
		}

		public void UpdateCommand_DocLink(CommandManagement.Command cmd)
		{
			cmd.Enabled = currentDocID > 0;
		}

		public void On_ShowWeb(CommandManagement.Command cmd)
		{
			bool push = !docControl.ShowWebPanel;
			docControl.ShowWebPanel = push;
			toolStripComboBoxZoom_TextChanged(this, EventArgs.Empty);
			subLayout.OptionForced<string>("ShowWebPanel").Value = push.ToString();
			toolStripButtonThumb.Enabled = docControl.ImageDisplayed;
		}

		public void UpdateCommand_ShowWeb(CommandManagement.Command cmd)
		{
			cmd.Enabled = currentDocID > 0;
			toolStripButtonWeb.Checked = docControl.HasData;
		}

		// Send message
		public void On_SendMessage(CommandManagement.Command cmd)
		{
			var sendMessageDialog = new SendMessageDialog(currentDocID);
			sendMessageDialog.DialogEvent += sendMessageDialog_DialogEvent;
			sendMessageDialog.Show();
		}

		public void UpdateCommand_SendMessage(CommandManagement.Command cmd)
		{
			cmd.Enabled = currentDocID > 0;
		}

		// Save part
		public void On_SavePart(CommandManagement.Command cmd)
		{
			if(docControl.ImageDisplayed && docControl.PageCount > 1)
			{
				docControl.CurDocString = docString;
				docControl.TestImage();
				docControl.SavePart();
			}
		}

		public void UpdateCommand_SavePart(CommandManagement.Command cmd)
		{
			cmd.Enabled = Environment.IsConnected && docControl.ImageDisplayed && docControl.PageCount > 1 && IsNotSigned() && docControl.CanSendOut;
		}

		// print
		public void On_Print(CommandManagement.Command cmd)
		{
			if(docControl.CanPrint)
				if(docControl.ImageDisplayed && !(docControl.DocumentID > 0 && docControl.ImageID == 0))
					docControl.PrintPage();
				else
					docControl.PrintEForm();
		}

		public void UpdateCommand_Print(CommandManagement.Command cmd)
		{
			if(!Disposing && !stop)
				cmd.Enabled = docControl.CanPrint;
		}

		// print image
		public void On_PrintImage(CommandManagement.Command cmd)
		{
			if(docControl.ImageDisplayed)
				docControl.Print();
		}

		public void UpdateCommand_PrintImage(CommandManagement.Command cmd)
		{
			if(!Disposing && !stop)
				cmd.Enabled = docControl.ImageDisplayed && !(docControl.DocumentID > 0 && (docControl.ImageID == 0));
		}

		//print document
		public void On_PrintDoc(CommandManagement.Command cmd)
		{
			if((docControl.DocumentID > 0) && (docControl.CanPrint || docControl.ImageCount > 1))
				docControl.PrintDocument();
		}

		public void UpdateCommand_PrintDoc(CommandManagement.Command cmd)
		{
			if(!Disposing && !stop)
				cmd.Enabled = (docControl.DocumentID > 0) && (docControl.CanPrint || docControl.ImageCount > 1);
		}

		public void On_ShowMessage(CommandManagement.Command cmd)
		{
			if(showMessageState == 0)
			{
				Dialogs.MessageShowDialog form = new Dialogs.MessageShowDialog(currentDocID, RectangleToScreen(toolStripSplitButtonMessage.Bounds));
				form.Show();
			}
			else
			{
				if(infoGrid != null)
				{
					showPushed = !showPushed;
					Environment.General.Option("ShowInSubWindowPushed").Value = showPushed.ToString();
					Environment.General.Option("ShowInSubWindowPushed").Save();
					if(showMessageState == 1)
						splitContainer.Panel1Collapsed = !showPushed;
					else
						if(showMessageState == 2)
						splitContainer.Panel2Collapsed = !showPushed;
				}
				else
					LoadMessage();
			}
		}

		public void UpdateCommand_ShowMessage(CommandManagement.Command cmd)
		{
			cmd.Enabled = currentDocID > 0;
			toolStripSplitButtonMessage.Checked = (infoGrid != null && showPushed);
			ToolStripMenuItemNewWindow.Checked = showMessageState == 0;
			ToolStripMenuItemLeft.Checked = showMessageState == 1;
			ToolStripMenuItemBottom.Checked = showMessageState == 2;
		}

		private void On_Select(CommandManagement.Command cmd)
		{
			docControl.SelectionMode = false;
			docControl.SelectTool(1);
			docControl.AnnotationDraw = true;
		}

		public void UpdateCommand_Select(CommandManagement.Command cmd)
		{
			cmd.Enabled = docControl.HasAnnotation();
			cmd.Checked = docControl.IsEditNotes && toolSelected.Equals(1);
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

		// Selection
		public void On_Selection(CommandManagement.Command cmd)
		{
			docControl.SelectionMode = true;
		}

		public void UpdateCommand_Selection(CommandManagement.Command cmd)
		{
			cmd.Enabled = docControl.ImageDisplayed;
			cmd.Checked = docControl.SelectionMode;
		}

		private void On_View(CommandManagement.Command cmd)
		{
			docControl.SelectionMode = false;
			docControl.AnnotationDraw = false;
			docControl.SelectTool(0);
		}

		public void UpdateCommand_View(CommandManagement.Command cmd)
		{
			cmd.Enabled = docControl.ImageDisplayed;
			cmd.Checked = !(docControl.AnnotationDraw || docControl.SelectionMode);
		}

		// ZoomSelection
		public void On_ZoomSelection(CommandManagement.Command cmd)
		{
			try
			{
				docControl.ZoomToSelection();
				int newZoom = docControl.Zoom;
				toolStripComboBoxZoom.Text = newZoom.ToString() + "%";
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public void UpdateCommand_ZoomSelection(CommandManagement.Command cmd)
		{
			cmd.Enabled = (docControl.RectDrawn());
		}

		private void On_Stamp(CommandManagement.Command cmd)
		{
			try
			{
				Image res = null;
				int id = 0;
				docControl.TestImage();
				using(Dialogs.SelectStampDialog dlg = new Dialogs.SelectStampDialog(imgID))
				{
					dlg.StartPosition = FormStartPosition.CenterParent;
					if(dlg.ShowDialog(this) == DialogResult.OK)
					{
						res = dlg.GetSelectedStamp();
						id = dlg.GetStampID();
					}
				}
				if(id > 0)
				{
					docControl.SelectTool(9);
					docControl.CurrentStamp = res;
					docControl.CurrentStampID = id;
					docControl.AnnotationDraw = true;
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public void UpdateCommand_Stamp(CommandManagement.Command cmd)
		{
			cmd.Enabled = currentDocID > 0 && docControl.ImageDisplayed;
			cmd.Checked = toolSelected == 9 && docControl.CurrentStampID > 0;
		}

		private void On_StampDSP(CommandManagement.Command cmd)
		{
			try
			{
				docControl.TestImage(Lib.Win.Document.Environment.ActionBefore.None);
				docControl.Page = 1;
				docControl.SelectTool(9);
				docControl.CurrentStamp = (Image)Lib.Win.Document.Environment.GetDSP().Clone();
				docControl.CurrentStampID = -101;
				docControl.AnnotationDraw = true;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public void UpdateCommand_StampDSP(CommandManagement.Command cmd)
		{
			cmd.Enabled = currentDocID > 0 && docControl.ImageDisplayed && !docControl.IsSignInternal;
			cmd.Checked = toolSelected == 9 && docControl.CurrentStampID < 0;
		}

		#endregion

		private void LoadMenuImages()
		{
			try
			{
				toolStripMenuItemSave.Image = toolStripImageList.Images[17];
				toolStripMenuItemSavePart.Image = toolStripImageList.Images[20];
				toolStripMenuItemSaveSelected.Image = toolStripImageList.Images[24];
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public void RefreshMessage()
		{
			if(Disposing || IsDisposed)
				return;
			if(InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(RefreshMessage));
				return;
			}

			if(currentDocID > 0 && infoGrid != null && infoGrid.Visible)
				infoGrid.LoadInfo(currentDocID);
		}

		private void docControl_VarListIndexChange(object sender, EventArgs e)
		{
			OnVarListIndexChanged();
		}

		private void OnVarListIndexChanged()
		{
			if(Disposing || IsDisposed)
				return;
			try
			{
				imgID = docControl.ImageID;
				if(imgID > 0)
					fileName = docControl.FileName;
				StatusBar_UpdatePage();
				UpdateNavigation();
				bool sendFaxEnabled = Environment.IsFaxSender(currentDocID) && (imgID > 0) && docControl.CanSendOut;
				toolStripButtonSendFax.Enabled = sendFaxEnabled;
				bool canPrint = docControl.CanPrint && docControl.CanSendOut;
				toolStripButtonPrint.Enabled = canPrint;
				statusBarPanelDSP.Text = !docControl.IsSignInternal ? "" : "ДСП";
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void docControl_PageChanged(object sender, EventArgs e)
		{
			OnPageChanged();
		}

		private void OnPageChanged()
		{
			if(Disposing || IsDisposed)
				return;
			try
			{
				string pagenumtext = docControl.Page.ToString();
				if(toolStrip.InvokeRequired)
					toolStrip.Invoke((MethodInvoker)(() => toolStripTextBoxPageNum.Text = pagenumtext));
				else
					toolStripTextBoxPageNum.Text = pagenumtext;

				StatusBar_UpdatePage();
				UpdateNavigation();

				bool canprint = docControl.CanPrint;
				bool sendFaxEnabled = Environment.IsFaxSender(currentDocID) && (imgID > 0) && docControl.CanSendOut;
				if(toolStrip.InvokeRequired)
					toolStrip.Invoke((MethodInvoker)(() => { toolStripButtonPrint.Enabled = canprint; toolStripButtonSendFax.Enabled = sendFaxEnabled; }));
				else
				{
					toolStripButtonSendFax.Enabled = sendFaxEnabled;
					toolStripButtonPrint.Enabled = canprint;
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void toPerson_Click(object sender, EventArgs e)
		{
			var item = sender as ToolStripDropDownItem;
			if(item == null)
				return;
			try
			{
				int personID = (int)item.Tag;
				MainFormDialog.nextPersonID = personID;
				MainFormDialog.curDocID = currentDocID;
				Environment.CmdManager.Commands["SelectPersonFolder"].Execute();
				Close();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		#region Link

		private void RefreshLinks()
		{
			try
			{
				if(docControl.InvokeRequired)
					docControl.BeginInvoke((MethodInvoker)(docControl.RefreshLinks));
				else
					docControl.RefreshLinks();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void toolStripButtonJoin_DropDownOpening(object sender, EventArgs e)
		{
			try
			{
				if(mIMain.DropDownItems.Count > 0)
				{
					mIMain.DropDownItems.Clear();
                    Console.WriteLine("{0}: mIMain.MenuItems cleared", DateTime.Now.ToString("HH:mm:ss fff"));
				}
				if(currentDocID != MainFormDialog.curDocID && !Environment.OpenDocsContains(MainFormDialog.curDocID))
				{
					if(!Environment.DocLinksData.HasExistsDocLink(MainFormDialog.curDocID, currentDocID))
					{
						ToolStripMenuItem item = new ToolStripMenuItem
													 {
														 Visible = true,
														 Tag = MainFormDialog.curDocID,
														 Text = DBDocString.Format(MainFormDialog.curDocID)
													 };
						item.Click += item_Click;
                        Console.WriteLine("{0}: TINC", DateTime.Now.ToString("HH:mm:ss fff"));
						mIMain.DropDownItems.Add(item);
					}
				}
				foreach(ToolStripMenuItem item in from t in Environment.OpenDocs
												  where t.Key != currentDocID &&
														!Environment.DocLinksData.HasExistsDocLink(t.Key, currentDocID)
												  select new ToolStripMenuItem
															 {
																 Visible = true,
																 Tag = t.Key,
																 Text = DBDocString.Format(t.Key)
															 })
				{
					item.Click += item_Click;
                    Console.WriteLine("{0}: TramPamPam", DateTime.Now.ToString("HH:mm:ss fff"));
					mIMain.DropDownItems.Add(item);
				}

				if(!mIMain.Enabled == (mIMain.DropDownItems.Count > 0))
					mIMain.Enabled = (mIMain.DropDownItems.Count > 0);
				Console.WriteLine("{0}: mIMain.MenuItems.Count = {1}",DateTime.Now.ToString("HH:mm:ss fff"), mIMain.DropDownItems.Count);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void SearchDialog_DialogEvent(object source, DialogEventArgs e)
		{
			var dialog = e.Dialog as XmlSearchForm;
			if(dialog == null || dialog.DialogResult != DialogResult.OK)
				return;
			try
			{
				if(Environment.DocData.FoundDocsCount() > 0) // есть найденные документы
				{
					// select doc
					var uniDialog =
						new SelectDocUniversalDialog(Environment.DocData.GetFoundDocsIDQuery(curEmpID),
													 dialog.DocID, dialog.GetXML(), true);
					uniDialog.DialogEvent += SelectDocUniversalDialog_ForLink_DialogEvent;
					uniDialog.Show();
				}
				else
				{
					if(MessageBox.Show(Environment.StringResources.GetString("Search.NotFound.Message1")
										+ System.Environment.NewLine + System.Environment.NewLine +
										Environment.StringResources.GetString("Search.NotFound.Message2"),
										Environment.StringResources.GetString("Search.NotFound.Title"),
										MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
										MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						var searchDialog = new XmlSearchForm(dialog.GetXML()) { DocID = dialog.DocID };
						searchDialog.DialogEvent += SearchDialog_DialogEvent;
						searchDialog.Show();
					}
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void SelectDocUniversalDialog_ForLink_DialogEvent(object source, DialogEventArgs e)
		{
			var dialog = e.Dialog as SelectDocUniversalDialog;
			if(dialog == null)
				return;
			try
			{
				if(dialog.DialogResult == DialogResult.OK)
				{
					// make link
					int firstID = dialog.CurDocID;
					if(dialog.DocIDs != null && dialog.DocIDs[0] != -1)
					{
						foreach(int secondID in dialog.DocIDs)
							if(firstID > 0 && secondID > 0)
							{
								var linkDialog = new LinkTypeDialog(firstID, secondID);
								linkDialog.DialogEvent += LinkTypeDialog_DialogEvent;
								linkDialog.Show();
							}
					}
					else
					{
						int secondID = dialog.DocID;
						if(firstID > 0 && secondID > 0)
						{
							var linkDialog = new LinkTypeDialog(firstID, secondID);
							linkDialog.DialogEvent += LinkTypeDialog_DialogEvent;
							linkDialog.Show();
						}
					}
				}
				else
				{
					var searchDialog = new XmlSearchForm(dialog.XML) { DocID = dialog.CurDocID };
					searchDialog.DialogEvent += SearchDialog_DialogEvent;
					searchDialog.Show();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void LinkTypeDialog_DialogEvent(object source, DialogEventArgs e)
		{
			var dialog = e.Dialog as LinkTypeDialog;
			if(dialog == null || dialog.DialogResult != DialogResult.OK)
				return;
			try
			{
				int parentID = dialog.OneID;
				int childID = dialog.TwoID;

				if(!dialog.Basic)
				{
					parentID = dialog.TwoID;
					childID = dialog.OneID;
				}

				if(Environment.CheckLinkDoc(parentID, childID))
				{
					Environment.DocLinksData.AddDocLink(parentID, childID);
					Environment.CmdManager.Commands["RefreshLinks"].Execute();
					RefreshLinks();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void item_Click(object sender, EventArgs e)
		{
			var item = sender as ToolStripMenuItem;
			if(item == null)
				return;
			try
			{
				var sID = (int)item.Tag;
				if(sID > 0)
				{
					var dialog = new LinkTypeDialog(currentDocID, sID);
					dialog.DialogEvent += LinkTypeDialog_DialogEvent;
					dialog.Show();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		#endregion

		private void GoToMain()
		{
			try
			{
				MainFormDialog.returnContext = context != null && context.Emp != null && (context.ID > 0 || context.Mode == Misc.ContextMode.Found) ? context : null;

				if(imgID > -2)
				{
					MainFormDialog.returnFileName = fileName ?? string.Empty;
					MainFormDialog.returnID = currentDocID;
					MainFormDialog.returnPath = DocPathString;
				}
				else
				{
					docControl.TestImage();

					MainFormDialog.returnFileName = fileName ?? string.Empty;
					MainFormDialog.returnID = 0;

					if(tf != null)
						MainFormDialog.returnFileName = tf.originalName;
				}

				Environment.CmdManager.Commands["Return"].Execute();

                // Передача параметров главной форме
                if (Program.MainFormDialog != null)
                {
                    var parameters = new Common.ViewParameters
                    {
                        IsPdf = docControl.IsPDFMode,
                        ImageId = docControl.ImageID,
                        Page = docControl.Page,
                        ActualImageHorizontalScrollValue = docControl.ActualImageHorisontalScrollValue,
                        ActualImageVerticalScrollValue = docControl.ActualImageVerticalScrollValue,
                        ScrollPositionX = docControl.ScrollPositionX,
                        ScrollPositionY = docControl.ScrollPositionY
                    };

                    Program.MainFormDialog.OnNavigate(parameters);
                }

				Close();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void docControl_DocChanged(object sender, DocumentSavedEventArgs e)
		{
			try
			{
				if(sender is Lib.Win.Document.Controls.SignDocumentPanel)
					foreach(int empID in Environment.EmpData.GetCurrentEmployeeIDs())
						Environment.WorkDocData.MarkAsRead(empID, currentDocID);

				if(e.DocID > 0 && currentDocID == 0 && fileName == null)
					ReloadDoc(e.DocID);

				bool create = e.CreateEForm;
				bool createSlave = e.CreateSlaveEForm;

				if(create)
				{
					string url = string.Empty;
					int docTypeID = Environment.DocData.GetDocIntField(Environment.DocData.DocTypeIDField, e.DocID, -1);
					if(docTypeID > -1)
						url = Environment.DocTypeData.GetField(Environment.DocTypeData.URLField, docTypeID).ToString();
					if(string.IsNullOrEmpty(url))
						url = Lib.Win.Document.Environment.SettingsURLString;
					if(!string.IsNullOrEmpty(url))
					{
						url = url.IndexOf("id=") > 0 ? url.Replace("id=", "id=" + e.DocID.ToString()) : (url + ((url.IndexOf("?") > 0) ? "&" : "?") + "id=" + e.DocID.ToString());
						Lib.Win.Document.Environment.IEOpenOnURL(url);
					}
				}

				if(createSlave)
				{
					string url = string.Empty;
					int contID = Environment.DocTypeData.GetSlaveType(e.DocID);
					if(contID > 0)
						url = Environment.URLsData.GetField(Environment.URLsData.NameField, (int)Lib.Win.Data.DALC.Documents.URLsDALC.URLsCode.CreateSlaveUrl).ToString();
					if(!string.IsNullOrEmpty(url))
					{
						url = url.IndexOf("id=") > 0 ? url.Replace("id=", "docid=" + e.DocID.ToString()) : (url + ((url.IndexOf("?") > 0) ? "&" : "?") + "docid=" + e.DocID.ToString()) + "&contractid=" + contID.ToString();
						Lib.Win.Document.Environment.IEOpenOnURL(url);
					}
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void docComponent_DocumentSaved(object sender, DocumentSavedEventArgs e)
		{
			try
			{
				if(!Lib.Win.Document.Environment.DocIsPrintedFromUDC(fileName) || e.DocID < 1 || e.ImageID < 0)
					return;
				imgID = e.ImageID;
				currentDocID = e.DocID;
				ReloadDoc(currentDocID);
				Environment.AddOpenDoc(currentDocID, this);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void ml_DialogEvent(object source, DialogEventArgs e)
		{
			try
			{
				if(e.Dialog.DialogResult != DialogResult.Abort)
					return;
				Dialogs.MenuList dialog = e.Dialog as Dialogs.MenuList;
				if(dialog != null)
				{
					Environment.UserSettings.NeedSave = true;
					Program.MainFormDialog.folders.AddDocumentNode(dialog.DocumentID, true, null, true);
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void TimerStart()
		{
			if(receiverTimer == null)
				receiverTimer = new System.Timers.Timer(3000);
			else
			{
				receiverTimer.Stop();
				receiverTimer.Interval = 3000;
				receiverTimer.Elapsed -= receiverTimer_Elapsed;
			}
			receiverTimer.Elapsed += receiverTimer_Elapsed;
			receiverTimer.AutoReset = false;
			receiverTimer.Start();
		}

		private void SubFormDialog_KeyDown(object sender, KeyEventArgs e)
		{
			if(keyLocker.Contains(e.KeyData))
				return;
			keyLocker.Add(e.KeyData);
			try
			{
				switch(e.KeyData)
				{
					case Keys.F5:
						if(currentDocID > 0)
						{
							int imageID = docControl.ImageID;
							docControl.DocumentID = currentDocID;
                            Console.WriteLine("{0}: ImgID = {1}", DateTime.Now.ToString("HH:mm:ss fff"), docControl.ImageID);
							if(docControl.ImageID > 0)
							{
								int curpage = docControl.Page;
								docControl.ImageID = docControl.ImageID;
								docControl.Page = curpage;
								docControl.ReloadTran(true);
								if(docControl.ShowWebPanel)
									docControl.RefreshEForm(true);
							}
							else
								docControl.RefreshEForm(true);
							RefreshMessage();
						}
						else
						{
							if(File.Exists(fileName))
								docControl.FileName = fileName;
							else
								Close();
						}
						break;
					case Keys.P | Keys.Control:
						subCmdManager.Commands["Print"].ExecuteIfEnabled();
						break;
					case Keys.F1:
						Environment.CmdManager.Commands["Help"].Execute();
						break;
					case Keys.F2:
						Save();
						break;
					case Keys.F7:
						subCmdManager.Commands["SendMessage"].ExecuteIfEnabled();
						break;
					case Keys.B | Keys.Control:
						if(docControl.ImageDisplayed)
							docControl.Page--;
						break;
					case Keys.N | Keys.Control:
						if(docControl.ImageDisplayed)
							docControl.Page++;
						break;
					case Keys.L | Keys.Control:
						if(docControl.ImageDisplayed && IsNotSigned())
							docControl.RotateLeft();
						break;
					case Keys.R | Keys.Control:
						if(docControl.ImageDisplayed && IsNotSigned())
							docControl.RotateRight();
						break;
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			finally
			{
				keyLocker.Remove(e.KeyData);
			}
		}

		private void SubFormDialog_KeyUp(object sender, KeyEventArgs e)
		{
			if(!keyLocker.Contains(e.KeyData))
			{
				keyLocker.Add(e.KeyData);
				try
				{
					switch(e.KeyData)
					{
						case Keys.Left:
							if(docControl.ImageDisplayed && docControl.Page > 1)
								docControl.Page--;
							break;
						case Keys.Right:
							if(docControl.ImageDisplayed && docControl.Page < docControl.PageCount)
								docControl.Page++;
							break;
					}
				}
				catch(Exception ex)
				{
					Lib.Win.Data.Env.WriteToLog(ex);
				}
				finally
				{
					keyLocker.Remove(e.KeyData);
				}
			}
		}

		private bool IsNotSigned()
		{
			return currentDocID < 1 || !docControl.IsSigned;
		}

		private void sendMessageDialog_DialogEvent(object source, DialogEventArgs e)
		{
			try
			{
				if(e.Dialog.DialogResult == DialogResult.OK)
				{
					RefreshMessage();
					if(Environment.CmdManager.Commands.Contains("RefreshInfo"))
						Environment.CmdManager.Commands["RefreshInfo"].ExecuteIfEnabled();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void receiverTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				if(receiverTimer != null)
				{
					receiverTimer.Stop();
					receiverTimer.Elapsed -= receiverTimer_Elapsed;
				}
				if(docChangedReceiver != null)
				{
					docChangedReceiver.Received -= receiver_Received;
					docChangedReceiver.Received += receiver_Received;
					docChangedReceiver.Start(false);
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void receiver_Received(string receivedString, int parameter)
		{
			if(Disposing || IsDisposed)
				return;

		    try
		    {
#if AdvancedLogging
                Lib.Log.Logger.EnterMethod(this, "SubFormDialog receiver_Received  rStr = " + receivedString);
#endif

                Console.WriteLine("{0}: Received : {1}", DateTime.Now.ToString("HH:mm:ss fff"), receivedString);
		        Match m = Regex.Match(receivedString, @"^(?<code>\d*)(\|(?<msg>.*))?$");
		        if (!m.Success)
		            return;

		        int messCode = int.Parse(m.Groups["code"].Value);
		        string msg = m.Groups["msg"].Value;

		        switch (messCode)
		        {
		            case 11: // new message on document
		            case 12: // new fax incoming
		            case 13: // new fax outgoing
		                break;
		            case 14:
		                if (msg.Contains("e") && Environment.UnSubscribeData.CheckIt(docControl.Subscribe))
		                {
                            Console.WriteLine("{0}: e", DateTime.Now.ToString("HH:mm:ss fff"));
		                    return;
		                }
		                if (ignoreDocChanges.Contains(msg))
		                {
		                    ignoreDocChanges.Remove(msg);
		                    ignoreDocChanges.Add(msg);
		                }
		                else
		                {
		                    ignoreDocChanges.Add(msg);
		                    BeginInvoke(new Action<string>(GotDocumentChange), new object[] {msg});
		                }
		                break;
		        }
		    }
		    catch (Exception ex)
		    {
		        Lib.Win.Data.Env.WriteToLog(ex, "message: " + receivedString + ", parameter: " + parameter.ToString());
		    }
		    finally
		    {
#if AdvancedLogging
                Lib.Log.Logger.LeaveMethod(this, "SubFormDialog receiver_Received  rStr = " + receivedString);
#endif
		    }
		}

		private void GotDocumentChange(string receivedString)
		{
			try
			{
#if AdvancedLogging
                Lib.Log.Logger.EnterMethod(this, "SubFormDialog GotDocumentChange  rStr = " + receivedString);
#endif

				int docID = 0;

				bool eform = false;
				bool document = false;
				bool trans = false;
				bool links = false;
				bool document1C = false;
				bool sign = false;

				Match m = Regex.Match(receivedString, @"^(?<ID>\d+)(?<codes>[deltps]{0,6})$", RegexOptions.IgnoreCase);
				if(m.Success)
				{
					docID = int.Parse(m.Groups["ID"].Value);
					if(m.Groups["codes"].Value.Length > 0)
					{
						string codes = m.Groups["codes"].Value.ToLower();
						document = codes.IndexOf("d") > -1;
						eform = codes.IndexOf("e") > -1;
						trans = codes.IndexOf("t") > -1;
						links = codes.IndexOf("l") > -1;
						document1C = codes.IndexOf("p") > -1;
						sign = codes.IndexOf("s") > -1;
					}
				}

				if(docID <= 0)
					return;

				if(eform && Environment.UnSubscribeData.CheckIt(docControl.Subscribe))
					return;
				if(docID != currentDocID)
					return;

				if(!Environment.DocData.IsDocAvailable(docID))
				{
					Close();
					return;
				}

				if(document || eform)
				{
					docString = DBDocString.Format(currentDocID);
					if(docString != null)
					{
						Text = Regex.Replace(docString, @"[\n\r]", " ");
						docControl.CurDocString = Regex.Replace(docString, @"[\n\r]", " ");
					}
					docControl.RefreshDoc();
				}
				if(trans)
					docControl.ReloadTran(false);
				if(document || eform || links)
					if(docControl.ShowWebPanel || docControl.ImageID == 0)
						docControl.RefreshEForm();
				if(document1C)
					docControl.ChangeImageIC();
				if(links)
					// достаем связи
					RefreshLinks();

				if(sign)
				{
					docControl.RefreshSigns();
					UpdateNavigation();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex, "message: " + receivedString + " wasn't processed too well on SubForm");
			}
			finally
			{
				while(ignoreDocChanges.Contains(receivedString))
					ignoreDocChanges.Remove(receivedString);

#if AdvancedLogging
                Lib.Log.Logger.LeaveMethod(this, "SubFormDialog GotDocumentChange  rStr = " + receivedString);
#endif
			}
		}

		private void docControl_NeedToRefresh(object source, EventArgs e)
		{
			try
			{
				string s = source as string;
				if(!string.IsNullOrEmpty(s) && File.Exists(s))
				{
					if(InvokeRequired)
						BeginInvoke((MethodInvoker)(() => ReloadFile(s)));
					else
						ReloadFile(s);
				}
				else
				{
					Closing -= SubFormDialog_Closing;
					stop = true;
					if(InvokeRequired)
						BeginInvoke((MethodInvoker)(Close));
					else
						Close();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void SubFormDialog_Closing(object sender, CancelEventArgs e)
		{
			docControl.TestImage();
			stop = true;

			KeyValuePair<string, Lib.Win.Document.Objects.TmpFile> tf;

			if(!string.IsNullOrEmpty(fileName) && Lib.Win.Document.Environment.TmpFilesContains(fileName))
			{
				tf = Lib.Win.Document.Environment.GetTmpFilePair(fileName);
				if(tf.Value != null && tf.Value.Window != null && tf.Value.Window.Equals(this))
				{
					tf.Value.OnModified -= new EventHandler(tf_OnModified);
					tf.Value.LinkCnt--;
					if(File.Exists(tf.Value.TmpFullName) && (tf.Value.LinkCnt <= 0 || (tf.Value.LinkCnt == 1 && tf.Value.IsInMain)))
					{
						stop = false;
						e.Cancel = true;
						tf.Value.LinkCnt++;
						tf.Value.OnModified += new EventHandler(tf_OnModified);
					}
					else
					{
						tf.Value.Window = null;
					}
				}
			}
		}

		internal void SaveFile()
		{
			if(Disposing || IsDisposed)
				return;
			if(InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(SaveFile));
				return;
			}
			try
			{
				if(docControl.ImageDisplayed && docControl.Modified)
					if(File.Exists(docControl.FileName))
						docControl.TestImage();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void ShowProperties()
		{
			if(Disposing || IsDisposed)
				return;
			if(InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(ShowProperties));
				return;
			}
			try
			{
				if(currentDocID > 0)
				{
					PropertiesDialogs.PropertiesDBDocDialog dialog = new PropertiesDialogs.PropertiesDBDocDialog(currentDocID);
					dialog.Show();
					return;
				}
				switch(context.Mode)
				{
					case Misc.ContextMode.FaxIn:
						int faxInID =
							Environment.FaxData.GetFaxIDFromFileName(Path.GetFileNameWithoutExtension(fileName));
						if(faxInID > 0)
						{
							PropertiesDialogs.PropertiesFaxDialog faxInDialog = new PropertiesDialogs.PropertiesFaxDialog();
							PropertiesDialogs.FaxFillClass.FaxInFillClass(faxInDialog, faxInID);
							faxInDialog.Show();
						}
						break;
					case Misc.ContextMode.FaxOut:
						int faxOutID = Environment.FaxData.GetFaxIDFromFileName(Path.GetFileNameWithoutExtension(fileName));
						if(faxOutID > 0)
						{
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
									case -1:
										MainFormDialog.ErrorMessage(
										Environment.StringResources.GetString("MainForm.MainFormDialog.On_DocProperties.Message1") +
											dr[Environment.FaxOutData.RecvAddressField] +
										Environment.StringResources.GetString("MainForm.MainFormDialog.On_DocProperties.Message2") +
											dr[Environment.FaxOutData.RecipField] + "'" + System.Environment.NewLine +
										Environment.StringResources.GetString("MainForm.MainFormDialog.On_DocProperties.Message3") +
										status + ")",
										Environment.StringResources.GetString("MainForm.MainFormDialog.On_DocProperties.Title1"));
										break;
									case -2:
										MainFormDialog.ErrorMessage(
										Environment.StringResources.GetString("MainForm.MainFormDialog.On_DocProperties.Message1") +
											dr[Environment.FaxOutData.RecvAddressField] +
										Environment.StringResources.GetString("MainForm.MainFormDialog.On_DocProperties.Message2") +
											dr[Environment.FaxOutData.RecipField] + "'" + System.Environment.NewLine +
										Environment.StringResources.GetString("MainForm.MainFormDialog.On_DocProperties.Message4") + status + ")",
										Environment.StringResources.GetString("MainForm.MainFormDialog.On_DocProperties.Title1"));
										break;
								}
							}
							catch(Exception ex)
							{
								Lib.Win.Data.Env.WriteToLog(ex);
								MainFormDialog.ErrorMessage(Environment.StringResources.GetString("MainForm.MainFormDialog.On_DocProperties.Message5"), "On_DocProperties()");
							}
						}
						break;
					case Misc.ContextMode.Scaner:
						PropertiesDialogs.PropertiesScanDialog sDialog = new PropertiesDialogs.PropertiesScanDialog(fileName);
						sDialog.Show();

						break;
					case Misc.ContextMode.SystemFolder:
						docControl.ShowProperties();
						break;
					case Misc.ContextMode.None:
						docControl.ShowProperties(false);
						break;
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void toolStripMenuItemAddToWork_Click(object sender, EventArgs e)
		{
			if(currentDocID <= 0)
				return;
			try
			{
				if(!Environment.WorkDocData.IsInWork(currentDocID, curEmpID))
				{
					Dialogs.FolderSelectDialog dialog = new Dialogs.FolderSelectDialog(new[] { currentDocID }, curEmpID);
					dialog.Show();
				}
				else
				{
					Dialogs.MoveDocDialog dialog = new Dialogs.MoveDocDialog(new[] { currentDocID }, curEmpID);
					dialog.Show();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void mf_DialogEvent(object source, DialogEventArgs e)
		{
			if(e.Dialog.DialogResult != DialogResult.Yes)
				return;

			int personID = Environment.DocDataData.GetDocIntField(Environment.DocDataData.SecondPersonIDNameField, currentDocID, -1);
			if(personID > -1)
			{
				var pd = new PersonContactDialog(personID, "");
				pd.Show();
			}
		}

		private void SubFormDialog_Disposed(object sender, EventArgs e)
		{
			Disposed -= SubFormDialog_Disposed;

			if(Environment.OpenDocsContains(currentDocID))
				Environment.RemoveOpenDoc(currentDocID);

			if(docControl != null)
				docControl.WatchOnFile = false;
		}

		private void subDialog_docControl_LoadComplete(object sender, EventArgs e)
		{
			docControl.LoadComplete -= subDialog_docControl_LoadComplete;
			try
			{
				BringToFront();
				Lib.Win.Document.Objects.TmpFile tf = Lib.Win.Document.Environment.GetTmpFile(this.fileName);
				if(tf != null)
				{
					tf.Window = this;
					tf.DocString = docString;
					this.Text = docString;
					tf.CurAct = Lib.Win.Document.Environment.ActionBefore.Save;
				}
				if(Tag is int[])
				{
					int docID = ((int[])Tag)[0];
					if(docID > 0)
					{
						MessageForm mf = new MessageForm(Environment.StringResources.GetString("MainForm.MainFormDialog.AnalyzeArgsFileName.Message1") +
								docString,
								Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNo) { ShowInTaskbar = false };
						mf.DialogEvent += save_DialogEvent;
						mf.Show(this);
						return;
					}
				}

				System.Timers.Timer tr = new System.Timers.Timer(300);
				tr.AutoReset = false;
				tr.Elapsed += new ElapsedEventHandler(tr_Elapsed);
				tr.Start();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		void tr_Elapsed(object sender, ElapsedEventArgs e)
		{
			System.Timers.Timer tr = sender as System.Timers.Timer;
			if(tr != null)
			{
				tr.Stop();
				tr.Elapsed -= tr_Elapsed;
				tr.Dispose();
				tr = null;
			}
			if(this.InvokeRequired)
				this.BeginInvoke((MethodInvoker)(Save));
			else
				Save();
		}

		private void save_DialogEvent(object source, DialogEventArgs e)
		{
			try
			{
				var form = source as Form;
				if(form != null)
				{
					if(form.DialogResult == DialogResult.Yes)
					{
						int docID = ((int[])Tag)[0];
						ServerInfo server = Lib.Win.Document.Environment.GetRandomLocalServer();
						string fileName = Environment.GenerateFileName();
						string path = server.Path + "\\TEMP\\" + fileName;
						if(File.Exists(path))
							File.Delete(path);
						DateTime creationTime = File.GetCreationTimeUtc(docControl.FileName);
						File.Move(docControl.FileName, path);
						int imgID = 0;
						Environment.DocImageData.DocImageInsert(server.ID, fileName, ref imgID, ref docID, 0, "",
																DateTime.MinValue, "", "", false, creationTime, 0, true,
																((int[])Tag)[1]);
						Environment.General.Option("DocID").Value = docID;
					}
					form.Owner = null;
				}
				Close();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		internal void SetSave()
		{
			docControl.LoadComplete += subDialog_docControl_LoadComplete;
		}

		private void SetMessageState(int newState)
		{
			try
			{
				showMessageState = newState;
				showPushed = true;
				LoadMessage();
				Environment.General.Option("ShowInSubWindow").Value = showMessageState;
				Environment.General.Option("ShowInSubWindowPushed").Value = showPushed.ToString();
				Environment.General.Save();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		#region ToolStripEvent

		private void toolStripMenuItemToMain_Click(object sender, EventArgs e)
		{
			GoToMain();
		}

		private void toolStripMenuItemBottom_Click(object sender, EventArgs e)
		{
			SetMessageState(2);
		}

		private void toolStripMenuItemLeft_Click(object sender, EventArgs e)
		{
			SetMessageState(1);
		}

		private void toolStripMenuItemNewWindow_Click(object sender, EventArgs e)
		{
			showMessageState = 0;
			LoadMessage();
			Environment.General.Option("ShowInSubWindow").Value = showMessageState;
			Environment.General.Save();
		}

		private void toolStripButtonThumb_Click(object sender, EventArgs e)
		{
			if(subLayout != null)
				subLayout.Option("ShowPagesPanel").Value = (!toolStripButtonThumb.Checked).ToString();
			docControl.Refresh();
		}

		private void toolStripButtonBefore_Click(object sender, EventArgs e)
		{
			if(docControl.ImageDisplayed)
			{
				if(toolStripButtonBefore.Enabled)
					toolStripTextBoxPageNum.Text = (docControl.Page - 1).ToString();
				UpdateNavigation();
			}
		}

		private void toolStripButtonNext_Click(object sender, EventArgs e)
		{
			if(docControl.ImageDisplayed)
			{
				if(toolStripButtonNext.Enabled)
					toolStripTextBoxPageNum.Text = (docControl.Page + 1).ToString();
				UpdateNavigation();
			}
		}

		private void toolStripButtonSave_Click(object sender, EventArgs e)
		{
			Save();
		}

		private void toolStripButtonZoomIn_Click(object sender, EventArgs e)
		{
			if(docControl.ImageDisplayed && !docControl.SelectionMode)
				On_ZoomIn();
		}

		private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
		{
			if(docControl.ImageDisplayed && !docControl.SelectionMode)
				On_ZoomOut();
		}

		private void toolStripButtonRotateLeft_Click(object sender, EventArgs e)
		{
			docControl.RotateLeft();
		}

		private void toolStripButtonRotateRight_Click(object sender, EventArgs e)
		{
			docControl.RotateRight();
		}

		private void toolStripButtonJoin_Click(object sender, EventArgs e)
		{
			linkContextMenu.Show(toolStrip, new Point(toolStripMenuItemJoin.Bounds.Left, toolStripMenuItemJoin.Bounds.Bottom));
		}

		private void toolStripButtonLinks_Click(object sender, EventArgs e)
		{
			try
			{
				Rectangle rec = toolStrip.RectangleToScreen(toolStripButtonLinks.Bounds);
				Console.WriteLine("{0}: {1}",DateTime.Now.ToString("HH:mm:ss fff"), rec);
				Dialogs.MenuList ml = new Dialogs.MenuList(currentDocID, rec,
									  docControl.ImageDisplayed ? toolStripComboBoxZoom.Text : Environment.ZoomString);
				ml.DialogEvent += ml_DialogEvent;
				ml.Show();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void toolStripButtonSendFax_Click(object sender, EventArgs e)
		{
			try
			{
				if(sendfax)
				{
					var mf = new MessageForm("Отправить ответ по факсу?", "Подтверждение", MessageBoxButtons.YesNoCancel);
					mf.DialogEvent += mf_DialogEvent;
					mf.Show();
				}
				else if(!string.IsNullOrEmpty(fileName) && (imgID < 0 || docControl.CanSendOut))
				{
					docControl.SendFax();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void toolStripButtonProperty_Click(object sender, EventArgs e)
		{
			ShowProperties();
		}

		#endregion

		private void toolStripMenuItemSaveStamp_Click(object sender, EventArgs e)
		{
			Dialogs.StampEditDialog dlg = new Dialogs.StampEditDialog { StampImage = docControl.GetSelectedRectImage() };
			dlg.Show();
		}

		private void toolStripMenuItemDoc_DropDownOpening(object sender, EventArgs e)
		{
			try
			{
				bool docSelected = currentDocID > 0;
				bool docVisible = docControl.ImageDisplayed;
				int docTypeID = -1;

				toolStripMenuItemLinkEform.Visible = docVisible;

				toolStripMenuItemLinkEform.DropDownItems.Clear();
				if(docVisible)
				{
					docTypeID = Environment.DocData.GetDocIntField(Environment.DocData.DocTypeIDField, currentDocID, -1);
					DataRow dr;
					using(DataTable typesTable = Environment.DocTypeLinkData.GetLinkedTypes(docTypeID))

						for(int j = 0; j < typesTable.Rows.Count; j++)
						{
							dr = typesTable.Rows[j];
							ToolStripMenuItem it = new ToolStripMenuItem();
							it.Text = dr[Environment.DocTypeLinkData.NameField] + ((DBNull.Value.Equals(dr[Environment.FieldData.NameField])
										  ? "" : ("(" + Environment.StringResources.GetString("OnField") + " " + dr[Environment.FieldData.NameField]) + ")"));
							it.Tag = dr;
							it.Click += linkEFormItem_Click;
							toolStripMenuItemLinkEform.DropDownItems.Add(it);
						}

				}

				toolStripMenuItemSave.Visible = docVisible && !docSelected;
				toolStripMenuItemSavePart.Visible = docVisible && docControl.PageCount > 0
					&& subCmdManager.Commands["SavePart"].Enabled;

				toolStripMenuItemSaveSelected.Visible = docControl.RectDrawn();
				toolStripMenuItemSaveStamp.Visible = toolStripMenuItemSaveStamp.Enabled && docControl.RectDrawn();
				toolStripSeparator11.Visible = toolStripMenuItemSave.Available || toolStripMenuItemSavePart.Available ||
											   toolStripMenuItemSaveSelected.Available;

				toolStripMenuItemAddEform.Visible = docSelected;
				toolStripSeparator12.Visible = toolStripMenuItemAddEform.Available;
				toolStripMenuItemScanDocImage.Visible = docSelected;
				toolStripMenuItemAddImage.Visible = docSelected;
				toolStripSeparator10.Visible = toolStripMenuItemNewDocEform.Available ||
											   toolStripMenuItemScanDocImage.Available || toolStripMenuItemAddImage.Available;

				bool inWork = docSelected && Environment.WorkDocData.IsInWork(currentDocID, Environment.CurEmp.ID);

				toolStripMenuItemAddToWork.Visible = docSelected && !inWork;
				toolStripMenuItemEditPlaces.Visible = inWork;
				toolStripMenuItemEndWork.Visible = inWork;
				toolStripSeparator13.Visible = toolStripMenuItemAddToWork.Available || toolStripMenuItemEditPlaces.Available || toolStripMenuItemEndWork.Available;

				toolStripMenuItemEditDesc.Visible = docVisible && context != null && context.Mode == Misc.ContextMode.FaxIn;
				toolStripMenuItemSpam.Visible = docVisible && context != null && context.Mode == Misc.ContextMode.FaxIn;
				toolStripSeparator14.Visible = toolStripMenuItemEditDesc.Available || toolStripMenuItemSpam.Available;

				toolStripMenuItemProperties.Visible = docVisible;
				toolStripSeparator15.Visible = toolStripMenuItemProperties.Available;

				toolStripMenuItemGotoPerson.Visible = docSelected;
				toolStripSeparator16.Visible = toolStripMenuItemGotoPerson.Available;

				toolStripMenuItemDelFrom.Visible = docSelected;
				toolStripMenuItemDelFromSearch.Visible = docSelected && context != null && context.Mode == Misc.ContextMode.Found;
				toolStripMenuItemDocPartDelete.Visible = docVisible && docSelected || (context != null && context.Mode == Misc.ContextMode.Scaner);

				if(docSelected)
				{
					toolStripMenuItemAddEform.Enabled = (!Environment.DocDataData.IsDataPresent(currentDocID) &&
						 Environment.DocTypeData.GetDocBoolField(Environment.DocTypeData.FormPresentField,
									 docTypeID));
					toolStripMenuItemDelFrom.Enabled = Environment.EmpData.IsDocDeleter() ||
													   !Environment.DocImageData.DocHasImages(currentDocID, true);
					toolStripMenuItemDocPartDelete.Enabled = docControl.PageCount > 0 && Environment.EmpData.IsDocDeleter();
				}

				if(context != null && context.Mode == Misc.ContextMode.FaxIn)
				{
					toolStripMenuItemSpam.Checked =
						Environment.FaxInData.GetDocBoolField(Environment.FaxInData.SpamField,
															  Environment.FaxInData.GetFaxIDFromFileName(fileName));
				}

				if(context != null && context.Mode == Misc.ContextMode.FaxOut)
				{
					toolStripMenuItemSave.Enabled =
						!Environment.FaxOutData.FaxHasDocImage(
							Environment.FaxOutData.GetFaxIDFromFileName(Path.GetFileNameWithoutExtension(fileName)));
					// no image id set
					toolStripMenuItemSavePart.Enabled = (toolStripMenuItemSave.Enabled && docControl.PageCount > 1);
				}

				if(context != null && context.Mode == Misc.ContextMode.Scaner)
				{
					toolStripMenuItemDocPartDelete.Enabled = docControl.PageCount > 1;
				}

				toolStripMenuItemGotoPerson.DropDownItems.Clear();
				if(currentDocID > 0)
				{
					// есть ли в группировке лица?
					if(Environment.UserSettings.GroupOrder.Contains(FolderTree.FolderNodes.PathNodes.CatalogNodes.CatalogNode.PersonInitial))
					{
						using(DataTable personTable = Environment.DocData.GetDocPersonsLite(currentDocID, false))
						using(DataTableReader dr = personTable.CreateDataReader())
						{
							while(dr.Read())
							{
								int personID = (int)dr[Environment.PersonData.IDField];
								if(Environment.UserSettings.PersonID != personID || personTable.Rows.Count == 1)
								{
									ToolStripMenuItem item = new ToolStripMenuItem
												   {
													   Tag = personID,
													   Text = dr[Environment.PersonData.NameField] as string ??
														   Environment.StringResources.GetString("MainForm.MainFormDialog.menuDoc_Popup.Message1")
												   };

									item.Click += toPerson_Click;
									toolStripMenuItemGotoPerson.DropDownItems.Add(item);
								}
							}
							dr.Close();
							dr.Dispose();
							personTable.Dispose();
						}
					}
				}
				toolStripMenuItemGotoPerson.Enabled = toolStripMenuItemGotoPerson.DropDownItems.Count > 0;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void toolStripMenuItemNewDocEform_Click(object sender, EventArgs e)
		{
			try
			{
				var dialog = new SelectTypeDialog(0, true, null, Environment.PreviosTypeID, Environment.PreviosDirection, false);
				dialog.DialogEvent += SelectTypeDialog_DialogEvent;
				dialog.Show();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void SelectTypeDialog_DialogEvent(object source, DialogEventArgs e)
		{
			try
			{
				var dialog = e.Dialog as SelectTypeDialog;
				if(dialog == null || dialog.DialogResult != DialogResult.OK)
					return;

				Environment.PreviosTypeID = dialog.TypeID;
				Environment.PreviosDirection = dialog.Out;
				string url = Environment.DocTypeData.GetField(Environment.DocTypeData.URLField, Environment.PreviosTypeID).ToString();
				if(string.IsNullOrEmpty(url))
					throw new Exception("Нет формы создания документа");
				string paramStr = "";
				if(Lib.Win.Document.Environment.PersonID > 0)
					paramStr = "&currentperson=" + Lib.Win.Document.Environment.PersonID.ToString();
				if(Environment.PreviosDirection.Equals(SelectTypeDialog.Direction.Out))
					paramStr += "&docDir=out";
				else if(Environment.PreviosDirection.Equals(SelectTypeDialog.Direction.In))
					paramStr += "&docDir=in";

				if(paramStr.Length > 0)
				{
					url += (url.IndexOf("?") > 0) ? "&" : "?";
					url += paramStr.TrimStart('&');
				}
				Lib.Win.Document.Environment.IEOpenOnURL(url);
			}
			catch(Exception ex)
			{
				ErrorShower.OnShowError(this, ex.Message, "");
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void toolStripMenuItemScanDocImage_Click(object sender, EventArgs e)
		{
			docControl.ScanNewCurrentDoc();
		}

        private void toolStripMenuItemAddImage_Click(object sender, EventArgs e)
        {
            if (currentDocID <= 0)
                return;

            Dialogs.NewImageAddDialog dialog = new Dialogs.NewImageAddDialog(currentDocID);
            dialog.Show();
        }

		private void toolStripMenuItemAddEform_Click(object sender, EventArgs e)
		{
			AddEForm();
		}

		private void toolStripMenuItemSpam_Click(object sender, EventArgs e)
		{
			int curFaxID = Environment.FaxInData.GetFaxIDFromFileName(Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fileName)));
			Environment.FaxData.MarkSpam(curFaxID, !toolStripMenuItemSpam.Checked);
		}

		private void toolStripMenuItemEditDesc_Click(object sender, EventArgs e)
		{
			try
			{
				int curFaxID = Environment.FaxInData.GetFaxIDFromFileName(Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fileName)));
				if(curFaxID > 0)
				{
					string descr =
						Environment.FaxInData.GetField(Environment.FaxInData.DescriptionField, curFaxID) as string ??
						string.Empty;
					EnterStringDialog dialog = new EnterStringDialog(Environment.StringResources.GetString("MainForm.MainFormDialog.On_FaxDescr.Message1"),
							Environment.StringResources.GetString("MainForm.MainFormDialog.On_FaxDescr.Title1"), descr,
							true, curFaxID);
					dialog.DialogEvent += FaxDescrDialog_DialogEvent;
					dialog.Show();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void FaxDescrDialog_DialogEvent(object source, DialogEventArgs e)
		{
			try
			{
				var dialog = e.Dialog as EnterStringDialog;
				if(dialog == null || dialog.DialogResult != DialogResult.OK)
					return;
				if(!(dialog.Data is int))
					return;
				var id = (int)dialog.Data;
				if(Environment.FaxInData.SetField(
					Environment.FaxInData.DescriptionField, SqlDbType.NVarChar, id, dialog.Input))
					Environment.RefreshDocs();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void toolStripMenuItemDocPartDelete_Click(object sender, EventArgs e)
		{
			docControl.TestImage();
			docControl.DeletePart();
		}

		private void toolStripMenuItemDelFrom_Click(object sender, EventArgs e)
		{
			try
			{
				MessageForm dialog = new MessageForm(
						Environment.StringResources.GetString("MainForm.MainFormDialog.On_DeleteDoc.Message1")
						+ docString +
						Environment.StringResources.GetString("MainForm.MainFormDialog.On_DeleteDoc.Message2") + System.Environment.NewLine +
						Environment.StringResources.GetString("MainForm.MainFormDialog.On_DeleteDoc.Message3") + System.Environment.NewLine + System.Environment.NewLine +
						Environment.StringResources.GetString("MainForm.MainFormDialog.On_DeleteDoc.Message4"),
						Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNo,
						MessageBoxDefaultButton.Button2) { Tag = currentDocID };
				dialog.DialogEvent += DeleteDocMessageForm_DialogEvent;
				dialog.Show();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void DeleteDocMessageForm_DialogEvent(object source, DialogEventArgs e)
		{
			try
			{
				var dialog = e.Dialog as MessageForm;
				if(dialog == null || dialog.DialogResult != DialogResult.Yes)
					return;

				var docId = (int)dialog.Tag;

                // 28960 Запрет повторного вызова sp_DeleteDoc
                IDocumentRepository documentRepository = new DocumentRepository();
			    if (documentRepository.DeleteDoc(-1, docId, true))
			        Environment.RefreshDocs();
                //if(Environment.DocData.DeleteDoc(-1, docID, true))
                //    Environment.RefreshDocs();
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void toolStripMenuItemDelFromSearch_Click(object sender, EventArgs e)
		{
			if(Environment.DocData.DeleteFromFound(currentDocID, Environment.CurEmp.ID))
				Environment.RefreshDocs();
		}

		private void toolStripMenuItemEndWork_Click(object sender, EventArgs e)
		{
			if(currentDocID <= 0)
				return;
			if(context == null)
				return;
			try
			{
				bool success = true;
				bool readAfterRemove = false;

				if(Environment.UserSettings.DeleteConfirm)
				{
					bool dontShowDialogAgain = false;
					if(MessageBoxScrollable.Show(Environment.StringResources.GetString("Confirmation"),
												  Environment.StringResources.GetString("RemoveMessageOnEndWork"),
												  docString, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, this,
												  out dontShowDialogAgain, out Environment.UserSettings.NeedSave)
						== DialogResult.Yes)
					{
						if(Environment.UserSettings.NeedSave)
						{
							Environment.UserSettings.DeleteConfirm = !dontShowDialogAgain;
							Environment.UserSettings.Save();
						}
					}
					else
					{
						Environment.UserSettings.NeedSave = false;
						return;
					}
				}

				switch(Environment.UserSettings.ReadMessageOnEndWork)
				{
					case 0:
						readAfterRemove = true;
						break;
					case 1:
						readAfterRemove = false;
						break;
					case 2:
						bool dontShowDialogAgain = false;
						switch(
							MessageBoxScrollable.Show(Environment.StringResources.GetString("Confirmation"),
							Environment.StringResources.GetString("ShouldReadMessageOnEndWork"),
							docString,
							MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, this,
							out dontShowDialogAgain, out Environment.UserSettings.NeedSave)
							)
						{
							case DialogResult.Yes:
								readAfterRemove = true;
								if(dontShowDialogAgain)
									Environment.UserSettings.ReadMessageOnEndWork = 0;
								break;
							case DialogResult.No:
								readAfterRemove = false;
								if(dontShowDialogAgain)
									Environment.UserSettings.ReadMessageOnEndWork = 1;
								break;
							case DialogResult.Cancel:
								Environment.UserSettings.NeedSave = false;
								return;
						}

						if(Environment.UserSettings.NeedSave)
							Environment.UserSettings.Save();
						break;
				}

				success = Environment.WorkDocData.RemoveDocFromWork(currentDocID, curEmpID);
				if(success && readAfterRemove)
					success = Environment.WorkDocData.MarkAsRead(curEmpID, currentDocID);
				if(success)
				{// Всё прошло хорошо
					Environment.UndoredoStack.Add("RemoveDocFromWork", Environment.StringResources.GetString("RemoveDocFromWork"),
							string.Format(Environment.StringResources.GetString("UndoRemoveDocFromWork"), docString),
							string.Format(Environment.StringResources.GetString("RedoRemoveDocFromWork"), docString),
							null, new object[] { currentDocID, (context.WorkOrSharedFolderMode() ? context.ID : 0), curEmpID }, curEmpID);
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void SubFormDialog_ResizeEnd(object sender, EventArgs e)
		{
			SaveFormState();
			RefreshStatusBar(sender, e);
		}

		private void docControl_LoadComplete(object sender, EventArgs e)
		{
			UpdateNavigation();
			// Востанавливаю позицию скролинга
			if(_parameters != null)
			{
				docControl.Page = _parameters.Page;

				if(docControl.IsPDFMode)
				{
					docControl.ScrollPositionX = (int)(_parameters.ActualImageHorizontalScrollValue * docControl.ActualImageHoriszontalScrollMaxValue);
					docControl.ScrollPositionY = (int)(_parameters.ActualImageVerticalScrollValue * docControl.ActualImageVerticalScrollMaxValue);
				}
				else
				{
					docControl.ScrollPositionX = -(int)(_parameters.ActualImageHorizontalScrollValue * docControl.ActualImageHoriszontalScrollMaxValue);
					docControl.ScrollPositionY = -(int)(_parameters.ActualImageVerticalScrollValue * docControl.ActualImageVerticalScrollMaxValue);
				}
			}
		}

		private void linkEFormItem_Click(object sender, EventArgs e)
		{
			try
			{
				ToolStripMenuItem item = sender as ToolStripMenuItem;
				if(item == null)
					return;
				DataRow dr = item.Tag as DataRow;
				if(dr == null)
					return;
				int typeID = (int)dr[Environment.DocTypeLinkData.ChildTypeIDFeild];
				var fieldID = (int)dr[Environment.DocLinksData.SubFieldIDField];
				int docID = currentDocID;
				string searchString = "SELECT " + Environment.DocData.IDField + " FROM " +
									  Environment.DocData.TableName + " T0 " +
									  "WHERE (EXISTS (SELECT * FROM " + Environment.DocLinksData.TableName +
									  " TI WHERE TI." + Environment.DocLinksData.ParentDocIDField + "=" +
									  docID.ToString() +
									  " AND TI." + Environment.DocLinksData.ChildDocIDField + "=T0." +
									  Environment.DocData.IDField + " AND TI." +
									  Environment.DocLinksData.SubFieldIDField + " = " + fieldID.ToString() + "))" +
									  "AND (T0." + Environment.DocData.DocTypeIDField + " = " + typeID.ToString() +
									  ")";

				DataTable dt = Environment.DocData.GetDocsByIDQuery(searchString, Environment.CurCultureInfo.Name);
				if(dt != null && dt.Rows.Count > 0)
				{
					Dialogs.ConfirmTypeDialog dialog = new Dialogs.ConfirmTypeDialog
					{
						TypeID = typeID,
						DocID = docID,
						FieldID = fieldID,
						TypeString = item.Text,
						SearchString = searchString
					};
					dialog.DialogEvent += ConfirmTypeDialog_DialogEvent;
					dialog.Show();
				}
				else
				{
					CreateNewDoc(typeID, docID, fieldID);
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
			Dialogs.ConfirmTypeDialog dialog = e.Dialog as Dialogs.ConfirmTypeDialog;
			if(dialog == null)
				return;
			if(dialog.DialogResult == DialogResult.Yes)
				CreateNewDoc(dialog.TypeID, dialog.DocID, dialog.FieldID);
		}

		private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}
	}
}