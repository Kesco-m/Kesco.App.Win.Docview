using System;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.Forms
{
	partial class MainFormDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				Lib.Win.Document.Environment.UIThreadSynchronizationContext = null;
				if(Lib.Win.HookClass.hHook != 0)
					while(!Lib.Win.HookClass.UnhookWindowsHookEx(Lib.Win.HookClass.hHook)) { }
				if(docChangedReceiver != null)
				{
					docChangedReceiver.Exit();
					docChangedReceiver.Received -= receiver_Received;
					docChangedReceiver = null;
				}
				if(Environment.CmdManager != null)
				{
					Environment.CmdManager.Commands.Clear();
					Environment.CmdManager.Dispose();
				}
				if(mlmDialog != null)
				{
					mlmDialog.FormClosed -= mlmDialog_FormClosed;
					mlmDialog.Dispose();
					mlmDialog = null;
				}
				if(Disposing || IsDisposed)
				{
					if(components != null)
					{
						components.Dispose();
					}
				}
			}
			catch(Exception ex)
			{ Lib.Win.Data.Env.WriteToLog(ex); }
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFormDialog));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			this.splitContainerMain = new System.Windows.Forms.SplitContainer();
			this.splitContainerGrids = new System.Windows.Forms.SplitContainer();
			this.splitContainerTree = new System.Windows.Forms.SplitContainer();
			this.folders = new Kesco.App.Win.DocView.FolderTree.FolderTree();
			this.foldersList = new System.Windows.Forms.ImageList(this.components);
			this.splitContainerList = new System.Windows.Forms.SplitContainer();
			this.infoGrid = new Kesco.App.Win.DocView.Grids.InfoGrid();
			this.docGrid = new Kesco.App.Win.DocView.Grids.DocGrid();
			this.splitContainerDoc = new System.Windows.Forms.SplitContainer();
			this.docControl = new Kesco.Lib.Win.Document.Controls.DocControl();
			this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
			this.menuDoc = new System.Windows.Forms.MenuItem();
			this.menuAddDocData = new System.Windows.Forms.MenuItem();
			this.menuScan = new System.Windows.Forms.MenuItem();
			this.menuScanCurrentDoc = new System.Windows.Forms.MenuItem();
			this.menuAddImageCurrentDoc = new System.Windows.Forms.MenuItem();
			this.menuLinkEform = new System.Windows.Forms.MenuItem();
			this.menuSeparator = new System.Windows.Forms.MenuItem();
			this.menuSearch = new System.Windows.Forms.MenuItem();
			this.menuFindID = new System.Windows.Forms.MenuItem();
			this.menuSeparator1 = new System.Windows.Forms.MenuItem();
			this.menuSave = new System.Windows.Forms.MenuItem();
			this.menuSavePart = new System.Windows.Forms.MenuItem();
			this.menuItemSaveSelected = new System.Windows.Forms.MenuItem();
			this.menuItemSaveSelectedAsStamp = new System.Windows.Forms.MenuItem();
			this.menuSeparator2 = new System.Windows.Forms.MenuItem();
			this.menuAddEForm = new System.Windows.Forms.MenuItem();
			this.menuSeparator3 = new System.Windows.Forms.MenuItem();
			this.menuAddToWork = new System.Windows.Forms.MenuItem();
			this.menuWorkPlaces = new System.Windows.Forms.MenuItem();
			this.menuEndWork = new System.Windows.Forms.MenuItem();
			this.menuSeparator4 = new System.Windows.Forms.MenuItem();
			this.menuFaxDescr = new System.Windows.Forms.MenuItem();
			this.menuSpam = new System.Windows.Forms.MenuItem();
			this.menuSeparator5 = new System.Windows.Forms.MenuItem();
			this.menuGotoPerson = new System.Windows.Forms.MenuItem();
			this.menuSeparator6 = new System.Windows.Forms.MenuItem();
			this.menuNewWindow = new System.Windows.Forms.MenuItem();
			this.menuDeletePart = new System.Windows.Forms.MenuItem();
			this.menuDelete = new System.Windows.Forms.MenuItem();
			this.menuDeleteFromFound = new System.Windows.Forms.MenuItem();
			this.menuDoc11 = new System.Windows.Forms.MenuItem();
			this.menuSeparator7 = new System.Windows.Forms.MenuItem();
			this.menuDocPropertes = new System.Windows.Forms.MenuItem();
			this.menuItemExit = new System.Windows.Forms.MenuItem();
			this.menuDoc4 = new System.Windows.Forms.MenuItem();
			this.menuRefresh = new System.Windows.Forms.MenuItem();
			this.menuSettingsGroupOrder = new System.Windows.Forms.MenuItem();
			this.menuSettingsFilter = new System.Windows.Forms.MenuItem();
			this.menuDoc7 = new System.Windows.Forms.MenuItem();
			this.menuShowPagesPanel = new System.Windows.Forms.MenuItem();
			this.menuShowWebPanel = new System.Windows.Forms.MenuItem();
			this.menuShowMessage = new System.Windows.Forms.MenuItem();
			this.menuItemBetween = new System.Windows.Forms.MenuItem();
			this.menuItemLeft = new System.Windows.Forms.MenuItem();
			this.menuItemUnder = new System.Windows.Forms.MenuItem();
			this.menuDoc8 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuPageBack = new System.Windows.Forms.MenuItem();
			this.menuPageForward = new System.Windows.Forms.MenuItem();
			this.menuDoc1 = new System.Windows.Forms.MenuItem();
			this.menuRotateCCW = new System.Windows.Forms.MenuItem();
			this.menuRotateCW = new System.Windows.Forms.MenuItem();
			this.menuDoc2 = new System.Windows.Forms.MenuItem();
			this.menuPageMoveBack = new System.Windows.Forms.MenuItem();
			this.menuPageMoveForward = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuZoom = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuZoomIn = new System.Windows.Forms.MenuItem();
			this.menuZoomOut = new System.Windows.Forms.MenuItem();
			this.menuDoc0 = new System.Windows.Forms.MenuItem();
			this.menuSelection = new System.Windows.Forms.MenuItem();
			this.menuZoomSelection = new System.Windows.Forms.MenuItem();
			this.mINotes = new System.Windows.Forms.MenuItem();
			this.menuShowNoteBar = new System.Windows.Forms.MenuItem();
			this.menuItem22 = new System.Windows.Forms.MenuItem();
			this.mILine = new System.Windows.Forms.MenuItem();
			this.mIFLine = new System.Windows.Forms.MenuItem();
			this.mIHighlighter = new System.Windows.Forms.MenuItem();
			this.mIRectangle = new System.Windows.Forms.MenuItem();
			this.mIHRectangle = new System.Windows.Forms.MenuItem();
			this.mIText = new System.Windows.Forms.MenuItem();
			this.mINote = new System.Windows.Forms.MenuItem();
			this.mIStamp = new System.Windows.Forms.MenuItem();
			this.miDSP = new System.Windows.Forms.MenuItem();
			this.mISelect = new System.Windows.Forms.MenuItem();
			this.mIView = new System.Windows.Forms.MenuItem();
			this.menuDoc3 = new System.Windows.Forms.MenuItem();
			this.mIHide = new System.Windows.Forms.MenuItem();
			this.mISelf = new System.Windows.Forms.MenuItem();
			this.mIShow = new System.Windows.Forms.MenuItem();
			this.mISeparator = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuSettingsMessages = new System.Windows.Forms.MenuItem();
			this.menuSettingsMailingLists = new System.Windows.Forms.MenuItem();
			this.menuSettingsFaxes = new System.Windows.Forms.MenuItem();
			this.menuSettingsShow = new System.Windows.Forms.MenuItem();
			this.menuSettingsLinkShow = new System.Windows.Forms.MenuItem();
			this.mIColumns = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuDoc5 = new System.Windows.Forms.MenuItem();
			this.menuColor1 = new System.Windows.Forms.MenuItem();
			this.menuColor2 = new System.Windows.Forms.MenuItem();
			this.menuColor3 = new System.Windows.Forms.MenuItem();
			this.menuDoc6 = new System.Windows.Forms.MenuItem();
			this.menuScale1 = new System.Windows.Forms.MenuItem();
			this.menuScale2 = new System.Windows.Forms.MenuItem();
			this.menuScale3 = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			this.menuPrintSettings = new System.Windows.Forms.MenuItem();
			this.menuSettingsScaner = new System.Windows.Forms.MenuItem();
			this.menuSettingsLanguage = new System.Windows.Forms.MenuItem();
			this.menuGoto = new System.Windows.Forms.MenuItem();
			this.menuGotoWorkFolder = new System.Windows.Forms.MenuItem();
			this.menuGotoFind = new System.Windows.Forms.MenuItem();
			this.menuGotoCatalog = new System.Windows.Forms.MenuItem();
			this.menuGotoFaxIn = new System.Windows.Forms.MenuItem();
			this.menuGotoScaner = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuPrintPage = new System.Windows.Forms.MenuItem();
			this.menuPrintSelection = new System.Windows.Forms.MenuItem();
			this.menuPrintAll = new System.Windows.Forms.MenuItem();
			this.menuPrintDoc = new System.Windows.Forms.MenuItem();
			this.menuFolder = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuSendMessage = new System.Windows.Forms.MenuItem();
			this.menuSendFax = new System.Windows.Forms.MenuItem();
			this.menuNewLink = new System.Windows.Forms.MenuItem();
			this.menuLinkOpenDoc = new System.Windows.Forms.MenuItem();
			this.menuLinkWorkDoc = new System.Windows.Forms.MenuItem();
			this.menuLinkDoc = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuOpenFolder = new System.Windows.Forms.MenuItem();
			this.menuHelp = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.toolbarImageList = new System.Windows.Forms.ImageList(this.components);
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.statusBarPanelArchive = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanelDoc = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanelSecure = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanelDSP = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanelCount = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanelPage = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanelDate = new System.Windows.Forms.StatusBarPanel();
			this.statusBarPanelTime = new System.Windows.Forms.StatusBarPanel();
			this.info = new System.Windows.Forms.RichTextBox();
			this.zoomCombo = new System.Windows.Forms.ToolStripComboBox();
			this.pageNum = new System.Windows.Forms.ToolStripTextBox();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.notifyMenu = new System.Windows.Forms.ContextMenu();
			this.mIShowWindow = new System.Windows.Forms.MenuItem();
			this.menuDoc9 = new System.Windows.Forms.MenuItem();
			this.mIClose = new System.Windows.Forms.MenuItem();
			this.toolBar = new System.Windows.Forms.ToolStrip();
			this.buttonSearch = new System.Windows.Forms.ToolStripButton();
			this.windowButton = new System.Windows.Forms.ToolStripButton();
			this.separator3 = new System.Windows.Forms.ToolStripSeparator();
			this.scanButton = new System.Windows.Forms.ToolStripButton();
			this.propertiesButton = new System.Windows.Forms.ToolStripButton();
			this.saveButton = new System.Windows.Forms.ToolStripButton();
			this.savePartButton = new System.Windows.Forms.ToolStripButton();
			this.saveSelectedButton = new System.Windows.Forms.ToolStripButton();
			this.separator6 = new System.Windows.Forms.ToolStripSeparator();
			this.refreshButton = new System.Windows.Forms.ToolStripButton();
			this.settingsGroupOrderButton = new System.Windows.Forms.ToolStripButton();
			this.settingsFilterButton = new System.Windows.Forms.ToolStripButton();
			this.separator11 = new System.Windows.Forms.ToolStripSeparator();
			this.undoButton = new System.Windows.Forms.ToolStripSplitButton();
			this.redoButton = new System.Windows.Forms.ToolStripSplitButton();
			this.separator9_ = new System.Windows.Forms.ToolStripSeparator();
			this.openFolderButton = new System.Windows.Forms.ToolStripButton();
			this.separator1 = new System.Windows.Forms.ToolStripSeparator();
			this.showPagesPanelButton = new System.Windows.Forms.ToolStripButton();
			this.showWebPanelButton = new System.Windows.Forms.ToolStripButton();
			this.tBBShowMessage = new Kesco.Lib.Win.Document.Items.ToolStripSplitButtonCheckable();
			this.TS_menuItemBetween = new System.Windows.Forms.ToolStripMenuItem();
			this.TS_menuItemLeft = new System.Windows.Forms.ToolStripMenuItem();
			this.TS_menuItemUnder = new System.Windows.Forms.ToolStripMenuItem();
			this.separator4 = new System.Windows.Forms.ToolStripSeparator();
			this.pageBackButton = new System.Windows.Forms.ToolStripButton();
			this.pageForwardButton = new System.Windows.Forms.ToolStripButton();
			this.separator5 = new System.Windows.Forms.ToolStripSeparator();
			this.zoomInButton = new System.Windows.Forms.ToolStripButton();
			this.zoomOutButton = new System.Windows.Forms.ToolStripButton();
			this.zoomSelectionButton = new System.Windows.Forms.ToolStripButton();
			this.separator10 = new System.Windows.Forms.ToolStripSeparator();
			this.viewButton = new System.Windows.Forms.ToolStripButton();
			this.selectAnnButton = new System.Windows.Forms.ToolStripButton();
			this.selectionButton = new System.Windows.Forms.ToolStripButton();
			this.separator2 = new System.Windows.Forms.ToolStripSeparator();
			this.rotateCCWButton = new System.Windows.Forms.ToolStripButton();
			this.rotateCWButton = new System.Windows.Forms.ToolStripButton();
			this.separator7 = new System.Windows.Forms.ToolStripSeparator();
			this.printButton = new System.Windows.Forms.ToolStripButton();
			this.sendMessageButton = new System.Windows.Forms.ToolStripButton();
			this.sendFaxButton = new System.Windows.Forms.ToolStripButton();
			this.separator8 = new System.Windows.Forms.ToolStripSeparator();
			this.linksButton = new System.Windows.Forms.ToolStripButton();
			this.tbbGoTo = new System.Windows.Forms.ToolStripButton();
			this.gotoWorkFolderButton = new System.Windows.Forms.ToolStripButton();
			this.gotoScanerButton = new System.Windows.Forms.ToolStripButton();
			this.gotoFaxInButton = new System.Windows.Forms.ToolStripButton();
			this.profButton = new System.Windows.Forms.ToolStripButton();
			this.gotoFoloderButton = new System.Windows.Forms.ToolStripButton();
			this.blank4 = new System.Windows.Forms.ToolStripButton();
			this.blank1 = new System.Windows.Forms.ToolStripButton();
			this.blankseparator1 = new System.Windows.Forms.ToolStripButton();
			this.blank2 = new System.Windows.Forms.ToolStripButton();
			this.blankseparator2 = new System.Windows.Forms.ToolStripButton();
			this.blank3 = new System.Windows.Forms.ToolStripButton();
			this.blankseparator3 = new System.Windows.Forms.ToolStripButton();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.opinionControl = new Kesco.Lib.Win.Opinion.OpinionControl();
			this.annotationBar = new Kesco.App.Win.DocView.TaggedToolBar();
			this.selectAnnButton1 = new System.Windows.Forms.ToolBarButton();
			this.markerButton = new System.Windows.Forms.ToolBarButton();
			this.rectangleButton = new System.Windows.Forms.ToolBarButton();
			this.textButton = new System.Windows.Forms.ToolBarButton();
			this.NoteButton = new System.Windows.Forms.ToolBarButton();
			this.imageStampButton = new System.Windows.Forms.ToolBarButton();
			this.dspStampButton = new System.Windows.Forms.ToolBarButton();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
			this.splitContainerMain.Panel1.SuspendLayout();
			this.splitContainerMain.Panel2.SuspendLayout();
			this.splitContainerMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerGrids)).BeginInit();
			this.splitContainerGrids.Panel1.SuspendLayout();
			this.splitContainerGrids.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerTree)).BeginInit();
			this.splitContainerTree.Panel1.SuspendLayout();
			this.splitContainerTree.Panel2.SuspendLayout();
			this.splitContainerTree.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerList)).BeginInit();
			this.splitContainerList.Panel1.SuspendLayout();
			this.splitContainerList.Panel2.SuspendLayout();
			this.splitContainerList.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.infoGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.docGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerDoc)).BeginInit();
			this.splitContainerDoc.Panel1.SuspendLayout();
			this.splitContainerDoc.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelArchive)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelDoc)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelSecure)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelDSP)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelCount)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelPage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelDate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelTime)).BeginInit();
			this.toolBar.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainerMain
			// 
			resources.ApplyResources(this.splitContainerMain, "splitContainerMain");
			this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainerMain.Name = "splitContainerMain";
			// 
			// splitContainerMain.Panel1
			// 
			this.splitContainerMain.Panel1.Controls.Add(this.splitContainerGrids);
			// 
			// splitContainerMain.Panel2
			// 
			this.splitContainerMain.Panel2.Controls.Add(this.splitContainerDoc);
			this.splitContainerMain.ClientSizeChanged += new System.EventHandler(this.splitContainerMain_ClientSizeChanged);
			// 
			// splitContainerGrids
			// 
			resources.ApplyResources(this.splitContainerGrids, "splitContainerGrids");
			this.splitContainerGrids.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainerGrids.Name = "splitContainerGrids";
			// 
			// splitContainerGrids.Panel1
			// 
			this.splitContainerGrids.Panel1.Controls.Add(this.splitContainerTree);
			this.splitContainerGrids.Panel2Collapsed = true;
			this.splitContainerGrids.ClientSizeChanged += new System.EventHandler(this.splitContainerGrids_ClientSizeChanged);
			// 
			// splitContainerTree
			// 
			resources.ApplyResources(this.splitContainerTree, "splitContainerTree");
			this.splitContainerTree.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainerTree.Name = "splitContainerTree";
			// 
			// splitContainerTree.Panel1
			// 
			this.splitContainerTree.Panel1.Controls.Add(this.folders);
			// 
			// splitContainerTree.Panel2
			// 
			this.splitContainerTree.Panel2.Controls.Add(this.splitContainerList);
			this.splitContainerTree.ClientSizeChanged += new System.EventHandler(this.splitContainerTree_ClientSizeChanged);
			// 
			// folders
			// 
			this.folders.AllowDrop = true;
			resources.ApplyResources(this.folders, "folders");
			this.folders.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
			this.folders.HideSelection = false;
			this.folders.ImageList = this.foldersList;
			this.folders.ItemHeight = 16;
			this.folders.Name = "folders";
			this.folders.SelectedNode = null;
			this.folders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.folders_AfterSelect);
			// 
			// foldersList
			// 
			this.foldersList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("foldersList.ImageStream")));
			this.foldersList.TransparentColor = System.Drawing.SystemColors.Window;
			this.foldersList.Images.SetKeyName(0, "");
			this.foldersList.Images.SetKeyName(1, "");
			this.foldersList.Images.SetKeyName(2, "");
			this.foldersList.Images.SetKeyName(3, "");
			this.foldersList.Images.SetKeyName(4, "");
			this.foldersList.Images.SetKeyName(5, "");
			this.foldersList.Images.SetKeyName(6, "");
			this.foldersList.Images.SetKeyName(7, "");
			this.foldersList.Images.SetKeyName(8, "");
			this.foldersList.Images.SetKeyName(9, "");
			this.foldersList.Images.SetKeyName(10, "");
			this.foldersList.Images.SetKeyName(11, "");
			this.foldersList.Images.SetKeyName(12, "");
			this.foldersList.Images.SetKeyName(13, "");
			this.foldersList.Images.SetKeyName(14, "");
			this.foldersList.Images.SetKeyName(15, "");
			this.foldersList.Images.SetKeyName(16, "");
			this.foldersList.Images.SetKeyName(17, "");
			// 
			// splitContainerList
			// 
			resources.ApplyResources(this.splitContainerList, "splitContainerList");
			this.splitContainerList.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainerList.Name = "splitContainerList";
			// 
			// splitContainerList.Panel1
			// 
			this.splitContainerList.Panel1.Controls.Add(this.infoGrid);
			// 
			// splitContainerList.Panel2
			// 
			this.splitContainerList.Panel2.Controls.Add(this.docGrid);
			this.splitContainerList.ClientSizeChanged += new System.EventHandler(this.splitContainerList_ClientSizeChanged);
			// 
			// infoGrid
			// 
			this.infoGrid.AllowUserToAddRows = false;
			this.infoGrid.AllowUserToDeleteRows = false;
			this.infoGrid.AllowUserToResizeRows = false;
			this.infoGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
			this.infoGrid.BackgroundColor = System.Drawing.SystemColors.Window;
			this.infoGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.infoGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.infoGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.infoGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.infoGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.infoGrid.Cursor = System.Windows.Forms.Cursors.Default;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			this.infoGrid.DefaultCellStyle = dataGridViewCellStyle2;
			resources.ApplyResources(this.infoGrid, "infoGrid");
			this.infoGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.infoGrid.GridColor = this.infoGrid.BackgroundColor;
			this.infoGrid.ImageTime = new System.DateTime(((long)(0)));
			this.infoGrid.MainForm = null;
			this.infoGrid.MultiSelect = false;
			this.infoGrid.Name = "infoGrid";
			this.infoGrid.ReadOnly = true;
			this.infoGrid.RowHeadersVisible = false;
			this.infoGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.infoGrid.RowsDefaultCellStyle = dataGridViewCellStyle3;
			this.infoGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.infoGrid.ShowCellErrors = false;
			this.infoGrid.ShowCellToolTips = false;
			this.infoGrid.ShowEditingIcon = false;
			this.infoGrid.ShowRowErrors = false;
			this.infoGrid.Style = null;
			// 
			// docGrid
			// 
			this.docGrid.AllowUserToAddRows = false;
			this.docGrid.AllowUserToDeleteRows = false;
			this.docGrid.AllowUserToResizeRows = false;
			this.docGrid.BackgroundColor = System.Drawing.SystemColors.Window;
			this.docGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.docGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			this.docGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.docGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
			this.docGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.docGrid.Cursor = System.Windows.Forms.Cursors.Default;
			resources.ApplyResources(this.docGrid, "docGrid");
			this.docGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.docGrid.ImageTime = new System.DateTime(((long)(0)));
			this.docGrid.MainForm = null;
			this.docGrid.Name = "docGrid";
			this.docGrid.ReadOnly = true;
			this.docGrid.RowHeadersVisible = false;
			this.docGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.docGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.docGrid.ShowCellErrors = false;
			this.docGrid.ShowEditingIcon = false;
			this.docGrid.ShowRowErrors = false;
			this.docGrid.Style = null;
			this.docGrid.DataSourceChanged += new System.EventHandler(this.docGrid_DataSourceChanged);
			this.docGrid.SelectionChanged += new System.EventHandler(this.docGrid_CurrentCellChanged);
			this.docGrid.DoubleClick += new System.EventHandler(this.docGrid_DoubleClick);
			this.docGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.docGrid_KeyDown);
			// 
			// splitContainerDoc
			// 
			resources.ApplyResources(this.splitContainerDoc, "splitContainerDoc");
			this.splitContainerDoc.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainerDoc.Name = "splitContainerDoc";
			// 
			// splitContainerDoc.Panel1
			// 
			this.splitContainerDoc.Panel1.Controls.Add(this.docControl);
			this.splitContainerDoc.Panel2Collapsed = true;
			this.splitContainerDoc.ClientSizeChanged += new System.EventHandler(this.splitContainerDoc_ClientSizeChanged);
			// 
			// docControl
			// 
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
			this.docControl.IsMain = true;
			this.docControl.IsMoveImage = true;
			this.docControl.Name = "docControl";
			this.docControl.Page = 0;
			this.docControl.PersonParamStr = "clid=4&return=1";
			this.docControl.SelectionMode = false;
			this.docControl.ShowThumbPanel = false;
			this.docControl.ShowWebPanel = false;
			this.docControl.WatchOnFile = false;
			this.docControl.Zoom = 100;
			this.docControl.ZoomText = "";
			this.docControl.VarListIndexChange += new System.EventHandler(this.docControl_VarListIndexChange);
			this.docControl.PageChanged += new System.EventHandler(this.docControl_PageChanged);
			this.docControl.DocChanged += new Kesco.Lib.Win.Document.Components.DocumentSavedEventHandle(this.docControl_DocChanged);
			this.docControl.FaxInSave += new Kesco.Lib.Win.Document.Components.DocumentSavedEventHandle(this.docControl_FaxInSave);
			this.docControl.MarkEnd += new System.EventHandler(this.docControl_MarkEnd);
			this.docControl.LinkDoc += new Kesco.Lib.Win.Document.Components.LinkDocEventHandler(this.docControl_LinkDoc);
			this.docControl.ToolSelected += new Kesco.Lib.Win.ImageControl.ImageControl.ToolSelectedHandler(this.docControl_ToolSelected);
			this.docControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.docControl_KeyDown);
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuDoc,
            this.menuDoc4,
            this.mINotes,
            this.menuItem8,
            this.menuGoto,
            this.menuItem3,
            this.menuNewLink,
            this.menuItem4,
            this.menuHelp});
			// 
			// menuDoc
			// 
			this.menuDoc.Index = 0;
			this.menuDoc.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuAddDocData,
            this.menuScan,
            this.menuScanCurrentDoc,
            this.menuAddImageCurrentDoc,
            this.menuLinkEform,
            this.menuSeparator,
            this.menuSearch,
            this.menuFindID,
            this.menuSeparator1,
            this.menuSave,
            this.menuSavePart,
            this.menuItemSaveSelected,
            this.menuItemSaveSelectedAsStamp,
            this.menuSeparator2,
            this.menuAddEForm,
            this.menuSeparator3,
            this.menuAddToWork,
            this.menuWorkPlaces,
            this.menuEndWork,
            this.menuSeparator4,
            this.menuFaxDescr,
            this.menuSpam,
            this.menuSeparator5,
            this.menuGotoPerson,
            this.menuSeparator6,
            this.menuNewWindow,
            this.menuDeletePart,
            this.menuDelete,
            this.menuDeleteFromFound,
            this.menuDoc11,
            this.menuSeparator7,
            this.menuDocPropertes,
            this.menuItemExit});
			resources.ApplyResources(this.menuDoc, "menuDoc");
			this.menuDoc.Popup += new System.EventHandler(this.menuDoc_Popup);
			// 
			// menuAddDocData
			// 
			this.menuAddDocData.Index = 0;
			resources.ApplyResources(this.menuAddDocData, "menuAddDocData");
			// 
			// menuScan
			// 
			this.menuScan.Index = 1;
			resources.ApplyResources(this.menuScan, "menuScan");
			this.menuScan.Click += new System.EventHandler(this.menuScan_Click);
			// 
			// menuScanCurrentDoc
			// 
			this.menuScanCurrentDoc.Index = 2;
			resources.ApplyResources(this.menuScanCurrentDoc, "menuScanCurrentDoc");
			// 
			// menuAddImageCurrentDoc
			// 
			this.menuAddImageCurrentDoc.Index = 3;
			resources.ApplyResources(this.menuAddImageCurrentDoc, "menuAddImageCurrentDoc");
			// 
			// menuLinkEform
			// 
			this.menuLinkEform.Index = 4;
			resources.ApplyResources(this.menuLinkEform, "menuLinkEform");
			// 
			// menuSeparator
			// 
			this.menuSeparator.Index = 5;
			resources.ApplyResources(this.menuSeparator, "menuSeparator");
			// 
			// menuSearch
			// 
			this.menuSearch.Index = 6;
			resources.ApplyResources(this.menuSearch, "menuSearch");
			// 
			// menuFindID
			// 
			this.menuFindID.Index = 7;
			resources.ApplyResources(this.menuFindID, "menuFindID");
			// 
			// menuSeparator1
			// 
			this.menuSeparator1.Index = 8;
			resources.ApplyResources(this.menuSeparator1, "menuSeparator1");
			// 
			// menuSave
			// 
			this.menuSave.Index = 9;
			resources.ApplyResources(this.menuSave, "menuSave");
			// 
			// menuSavePart
			// 
			this.menuSavePart.Index = 10;
			resources.ApplyResources(this.menuSavePart, "menuSavePart");
			// 
			// menuItemSaveSelected
			// 
			this.menuItemSaveSelected.Index = 11;
			resources.ApplyResources(this.menuItemSaveSelected, "menuItemSaveSelected");
			// 
			// menuItemSaveSelectedAsStamp
			// 
			this.menuItemSaveSelectedAsStamp.Index = 12;
			resources.ApplyResources(this.menuItemSaveSelectedAsStamp, "menuItemSaveSelectedAsStamp");
			// 
			// menuSeparator2
			// 
			this.menuSeparator2.Index = 13;
			resources.ApplyResources(this.menuSeparator2, "menuSeparator2");
			// 
			// menuAddEForm
			// 
			this.menuAddEForm.Index = 14;
			resources.ApplyResources(this.menuAddEForm, "menuAddEForm");
			// 
			// menuSeparator3
			// 
			this.menuSeparator3.Index = 15;
			resources.ApplyResources(this.menuSeparator3, "menuSeparator3");
			// 
			// menuAddToWork
			// 
			this.menuAddToWork.Index = 16;
			resources.ApplyResources(this.menuAddToWork, "menuAddToWork");
			// 
			// menuWorkPlaces
			// 
			this.menuWorkPlaces.Index = 17;
			resources.ApplyResources(this.menuWorkPlaces, "menuWorkPlaces");
			// 
			// menuEndWork
			// 
			this.menuEndWork.Index = 18;
			resources.ApplyResources(this.menuEndWork, "menuEndWork");
			// 
			// menuSeparator4
			// 
			this.menuSeparator4.Index = 19;
			resources.ApplyResources(this.menuSeparator4, "menuSeparator4");
			// 
			// menuFaxDescr
			// 
			this.menuFaxDescr.Index = 20;
			resources.ApplyResources(this.menuFaxDescr, "menuFaxDescr");
			// 
			// menuSpam
			// 
			this.menuSpam.Index = 21;
			resources.ApplyResources(this.menuSpam, "menuSpam");
			// 
			// menuSeparator5
			// 
			this.menuSeparator5.Index = 22;
			resources.ApplyResources(this.menuSeparator5, "menuSeparator5");
			// 
			// menuGotoPerson
			// 
			this.menuGotoPerson.Index = 23;
			resources.ApplyResources(this.menuGotoPerson, "menuGotoPerson");
			// 
			// menuSeparator6
			// 
			this.menuSeparator6.Index = 24;
			resources.ApplyResources(this.menuSeparator6, "menuSeparator6");
			// 
			// menuNewWindow
			// 
			this.menuNewWindow.Index = 25;
			resources.ApplyResources(this.menuNewWindow, "menuNewWindow");
			// 
			// menuDeletePart
			// 
			this.menuDeletePart.Index = 26;
			resources.ApplyResources(this.menuDeletePart, "menuDeletePart");
			// 
			// menuDelete
			// 
			this.menuDelete.Index = 27;
			resources.ApplyResources(this.menuDelete, "menuDelete");
			// 
			// menuDeleteFromFound
			// 
			this.menuDeleteFromFound.Index = 28;
			resources.ApplyResources(this.menuDeleteFromFound, "menuDeleteFromFound");
			// 
			// menuDoc11
			// 
			this.menuDoc11.Index = 29;
			resources.ApplyResources(this.menuDoc11, "menuDoc11");
			// 
			// menuSeparator7
			// 
			this.menuSeparator7.Index = 30;
			resources.ApplyResources(this.menuSeparator7, "menuSeparator7");
			// 
			// menuDocPropertes
			// 
			this.menuDocPropertes.Index = 31;
			resources.ApplyResources(this.menuDocPropertes, "menuDocPropertes");
			// 
			// menuItemExit
			// 
			this.menuItemExit.Index = 32;
			resources.ApplyResources(this.menuItemExit, "menuItemExit");
			this.menuItemExit.Click += new System.EventHandler(this.mIClose_Click);
			// 
			// menuDoc4
			// 
			this.menuDoc4.Index = 1;
			this.menuDoc4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuRefresh,
            this.menuSettingsGroupOrder,
            this.menuSettingsFilter,
            this.menuDoc7,
            this.menuShowPagesPanel,
            this.menuShowWebPanel,
            this.menuShowMessage,
            this.menuDoc8,
            this.menuItem6,
            this.menuItem5});
			resources.ApplyResources(this.menuDoc4, "menuDoc4");
			// 
			// menuRefresh
			// 
			this.menuRefresh.Index = 0;
			resources.ApplyResources(this.menuRefresh, "menuRefresh");
			// 
			// menuSettingsGroupOrder
			// 
			this.menuSettingsGroupOrder.Index = 1;
			resources.ApplyResources(this.menuSettingsGroupOrder, "menuSettingsGroupOrder");
			// 
			// menuSettingsFilter
			// 
			this.menuSettingsFilter.Index = 2;
			resources.ApplyResources(this.menuSettingsFilter, "menuSettingsFilter");
			// 
			// menuDoc7
			// 
			this.menuDoc7.Index = 3;
			resources.ApplyResources(this.menuDoc7, "menuDoc7");
			// 
			// menuShowPagesPanel
			// 
			this.menuShowPagesPanel.Index = 4;
			resources.ApplyResources(this.menuShowPagesPanel, "menuShowPagesPanel");
			this.menuShowPagesPanel.Click += new System.EventHandler(this.menuShowPagesPanel_Click);
			// 
			// menuShowWebPanel
			// 
			this.menuShowWebPanel.Index = 5;
			resources.ApplyResources(this.menuShowWebPanel, "menuShowWebPanel");
			// 
			// menuShowMessage
			// 
			this.menuShowMessage.Index = 6;
			this.menuShowMessage.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemBetween,
            this.menuItemLeft,
            this.menuItemUnder});
			resources.ApplyResources(this.menuShowMessage, "menuShowMessage");
			// 
			// menuItemBetween
			// 
			this.menuItemBetween.Index = 0;
			resources.ApplyResources(this.menuItemBetween, "menuItemBetween");
			this.menuItemBetween.Click += new System.EventHandler(this.menuItemBetween_Click);
			// 
			// menuItemLeft
			// 
			this.menuItemLeft.Index = 1;
			resources.ApplyResources(this.menuItemLeft, "menuItemLeft");
			this.menuItemLeft.Click += new System.EventHandler(this.menuItemLeft_Click);
			// 
			// menuItemUnder
			// 
			this.menuItemUnder.Index = 2;
			resources.ApplyResources(this.menuItemUnder, "menuItemUnder");
			this.menuItemUnder.Click += new System.EventHandler(this.menuItemUnder_Click);
			// 
			// menuDoc8
			// 
			this.menuDoc8.Index = 7;
			resources.ApplyResources(this.menuDoc8, "menuDoc8");
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 8;
			this.menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuPageBack,
            this.menuPageForward,
            this.menuDoc1,
            this.menuRotateCCW,
            this.menuRotateCW,
            this.menuDoc2,
            this.menuPageMoveBack,
            this.menuPageMoveForward});
			resources.ApplyResources(this.menuItem6, "menuItem6");
			// 
			// menuPageBack
			// 
			this.menuPageBack.Index = 0;
			resources.ApplyResources(this.menuPageBack, "menuPageBack");
			// 
			// menuPageForward
			// 
			this.menuPageForward.Index = 1;
			resources.ApplyResources(this.menuPageForward, "menuPageForward");
			// 
			// menuDoc1
			// 
			this.menuDoc1.Index = 2;
			resources.ApplyResources(this.menuDoc1, "menuDoc1");
			// 
			// menuRotateCCW
			// 
			this.menuRotateCCW.Index = 3;
			resources.ApplyResources(this.menuRotateCCW, "menuRotateCCW");
			// 
			// menuRotateCW
			// 
			this.menuRotateCW.Index = 4;
			resources.ApplyResources(this.menuRotateCW, "menuRotateCW");
			// 
			// menuDoc2
			// 
			this.menuDoc2.Index = 5;
			resources.ApplyResources(this.menuDoc2, "menuDoc2");
			// 
			// menuPageMoveBack
			// 
			this.menuPageMoveBack.Index = 6;
			resources.ApplyResources(this.menuPageMoveBack, "menuPageMoveBack");
			// 
			// menuPageMoveForward
			// 
			this.menuPageMoveForward.Index = 7;
			resources.ApplyResources(this.menuPageMoveForward, "menuPageMoveForward");
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 9;
			this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuZoom,
            this.menuItem7,
            this.menuZoomIn,
            this.menuZoomOut,
            this.menuDoc0,
            this.menuSelection,
            this.menuZoomSelection});
			resources.ApplyResources(this.menuItem5, "menuItem5");
			// 
			// menuZoom
			// 
			this.menuZoom.Index = 0;
			resources.ApplyResources(this.menuZoom, "menuZoom");
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 1;
			resources.ApplyResources(this.menuItem7, "menuItem7");
			// 
			// menuZoomIn
			// 
			this.menuZoomIn.Index = 2;
			resources.ApplyResources(this.menuZoomIn, "menuZoomIn");
			// 
			// menuZoomOut
			// 
			this.menuZoomOut.Index = 3;
			resources.ApplyResources(this.menuZoomOut, "menuZoomOut");
			// 
			// menuDoc0
			// 
			this.menuDoc0.Index = 4;
			resources.ApplyResources(this.menuDoc0, "menuDoc0");
			// 
			// menuSelection
			// 
			this.menuSelection.Index = 5;
			resources.ApplyResources(this.menuSelection, "menuSelection");
			// 
			// menuZoomSelection
			// 
			this.menuZoomSelection.Index = 6;
			resources.ApplyResources(this.menuZoomSelection, "menuZoomSelection");
			// 
			// mINotes
			// 
			this.mINotes.Index = 2;
			this.mINotes.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuShowNoteBar,
            this.menuItem22,
            this.mILine,
            this.mIFLine,
            this.mIHighlighter,
            this.mIRectangle,
            this.mIHRectangle,
            this.mIText,
            this.mINote,
            this.mIStamp,
            this.miDSP,
            this.mISelect,
            this.mIView,
            this.menuDoc3,
            this.mIHide,
            this.mISelf,
            this.mIShow,
            this.mISeparator});
			resources.ApplyResources(this.mINotes, "mINotes");
			// 
			// menuShowNoteBar
			// 
			this.menuShowNoteBar.Index = 0;
			resources.ApplyResources(this.menuShowNoteBar, "menuShowNoteBar");
			this.menuShowNoteBar.Click += new System.EventHandler(this.menuShowNoteBar_Click);
			// 
			// menuItem22
			// 
			this.menuItem22.Index = 1;
			resources.ApplyResources(this.menuItem22, "menuItem22");
			// 
			// mILine
			// 
			this.mILine.Index = 2;
			resources.ApplyResources(this.mILine, "mILine");
			// 
			// mIFLine
			// 
			this.mIFLine.Index = 3;
			resources.ApplyResources(this.mIFLine, "mIFLine");
			// 
			// mIHighlighter
			// 
			this.mIHighlighter.Index = 4;
			resources.ApplyResources(this.mIHighlighter, "mIHighlighter");
			// 
			// mIRectangle
			// 
			this.mIRectangle.Index = 5;
			resources.ApplyResources(this.mIRectangle, "mIRectangle");
			// 
			// mIHRectangle
			// 
			this.mIHRectangle.Index = 6;
			resources.ApplyResources(this.mIHRectangle, "mIHRectangle");
			// 
			// mIText
			// 
			this.mIText.Index = 7;
			resources.ApplyResources(this.mIText, "mIText");
			// 
			// mINote
			// 
			this.mINote.Index = 8;
			resources.ApplyResources(this.mINote, "mINote");
			// 
			// mIStamp
			// 
			this.mIStamp.Index = 9;
			resources.ApplyResources(this.mIStamp, "mIStamp");
			// 
			// miDSP
			// 
			this.miDSP.Index = 10;
			resources.ApplyResources(this.miDSP, "miDSP");
			// 
			// mISelect
			// 
			this.mISelect.Index = 11;
			resources.ApplyResources(this.mISelect, "mISelect");
			// 
			// mIView
			// 
			this.mIView.Index = 12;
			resources.ApplyResources(this.mIView, "mIView");
			// 
			// menuDoc3
			// 
			this.menuDoc3.Index = 13;
			resources.ApplyResources(this.menuDoc3, "menuDoc3");
			// 
			// mIHide
			// 
			this.mIHide.Index = 14;
			resources.ApplyResources(this.mIHide, "mIHide");
			// 
			// mISelf
			// 
			this.mISelf.Index = 15;
			resources.ApplyResources(this.mISelf, "mISelf");
			// 
			// mIShow
			// 
			this.mIShow.Index = 16;
			resources.ApplyResources(this.mIShow, "mIShow");
			// 
			// mISeparator
			// 
			this.mISeparator.Index = 17;
			resources.ApplyResources(this.mISeparator, "mISeparator");
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 3;
			this.menuItem8.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuSettingsMessages,
            this.menuSettingsMailingLists,
            this.menuSettingsFaxes,
            this.menuSettingsShow,
            this.menuSettingsLinkShow,
            this.mIColumns,
            this.menuItem1,
            this.menuDoc5,
            this.menuDoc6,
            this.menuItem10,
            this.menuPrintSettings,
            this.menuSettingsScaner,
            this.menuSettingsLanguage});
			resources.ApplyResources(this.menuItem8, "menuItem8");
			this.menuItem8.Popup += new System.EventHandler(this.menuItem8_Popup);
			// 
			// menuSettingsMessages
			// 
			this.menuSettingsMessages.Index = 0;
			resources.ApplyResources(this.menuSettingsMessages, "menuSettingsMessages");
			// 
			// menuSettingsMailingLists
			// 
			this.menuSettingsMailingLists.Index = 1;
			resources.ApplyResources(this.menuSettingsMailingLists, "menuSettingsMailingLists");
			// 
			// menuSettingsFaxes
			// 
			this.menuSettingsFaxes.Index = 2;
			resources.ApplyResources(this.menuSettingsFaxes, "menuSettingsFaxes");
			// 
			// menuSettingsShow
			// 
			this.menuSettingsShow.Index = 3;
			resources.ApplyResources(this.menuSettingsShow, "menuSettingsShow");
			// 
			// menuSettingsLinkShow
			// 
			this.menuSettingsLinkShow.Index = 4;
			resources.ApplyResources(this.menuSettingsLinkShow, "menuSettingsLinkShow");
			this.menuSettingsLinkShow.Click += new System.EventHandler(this.menuSettingsLinkShow_Click);
			// 
			// mIColumns
			// 
			this.mIColumns.Index = 5;
			resources.ApplyResources(this.mIColumns, "mIColumns");
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 6;
			resources.ApplyResources(this.menuItem1, "menuItem1");
			// 
			// menuDoc5
			// 
			this.menuDoc5.Index = 7;
			this.menuDoc5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuColor1,
            this.menuColor2,
            this.menuColor3});
			resources.ApplyResources(this.menuDoc5, "menuDoc5");
			// 
			// menuColor1
			// 
			this.menuColor1.Index = 0;
			resources.ApplyResources(this.menuColor1, "menuColor1");
			this.menuColor1.Click += new System.EventHandler(this.menuColor1_Click);
			// 
			// menuColor2
			// 
			this.menuColor2.Index = 1;
			resources.ApplyResources(this.menuColor2, "menuColor2");
			this.menuColor2.Click += new System.EventHandler(this.menuColor2_Click);
			// 
			// menuColor3
			// 
			this.menuColor3.Index = 2;
			resources.ApplyResources(this.menuColor3, "menuColor3");
			this.menuColor3.Click += new System.EventHandler(this.menuColor3_Click);
			// 
			// menuDoc6
			// 
			this.menuDoc6.Index = 8;
			this.menuDoc6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuScale1,
            this.menuScale2,
            this.menuScale3});
			resources.ApplyResources(this.menuDoc6, "menuDoc6");
			// 
			// menuScale1
			// 
			this.menuScale1.Index = 0;
			resources.ApplyResources(this.menuScale1, "menuScale1");
			this.menuScale1.Click += new System.EventHandler(this.menuScale1_Click);
			// 
			// menuScale2
			// 
			this.menuScale2.Index = 1;
			resources.ApplyResources(this.menuScale2, "menuScale2");
			this.menuScale2.Click += new System.EventHandler(this.menuScale2_Click);
			// 
			// menuScale3
			// 
			this.menuScale3.Index = 2;
			resources.ApplyResources(this.menuScale3, "menuScale3");
			this.menuScale3.Click += new System.EventHandler(this.menuScale3_Click);
			// 
			// menuItem10
			// 
			this.menuItem10.Index = 9;
			resources.ApplyResources(this.menuItem10, "menuItem10");
			// 
			// menuPrintSettings
			// 
			this.menuPrintSettings.Index = 10;
			resources.ApplyResources(this.menuPrintSettings, "menuPrintSettings");
			this.menuPrintSettings.Click += new System.EventHandler(this.menuPrintSettings_Click);
			// 
			// menuSettingsScaner
			// 
			this.menuSettingsScaner.Index = 11;
			resources.ApplyResources(this.menuSettingsScaner, "menuSettingsScaner");
			// 
			// menuSettingsLanguage
			// 
			resources.ApplyResources(this.menuSettingsLanguage, "menuSettingsLanguage");
			this.menuSettingsLanguage.Index = 12;
			this.menuSettingsLanguage.Click += new System.EventHandler(this.menuSettingsLanguage_Click);
			// 
			// menuGoto
			// 
			this.menuGoto.Index = 4;
			this.menuGoto.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuGotoWorkFolder,
            this.menuGotoFind,
            this.menuGotoCatalog,
            this.menuGotoFaxIn,
            this.menuGotoScaner});
			resources.ApplyResources(this.menuGoto, "menuGoto");
			// 
			// menuGotoWorkFolder
			// 
			this.menuGotoWorkFolder.Index = 0;
			resources.ApplyResources(this.menuGotoWorkFolder, "menuGotoWorkFolder");
			// 
			// menuGotoFind
			// 
			this.menuGotoFind.Index = 1;
			resources.ApplyResources(this.menuGotoFind, "menuGotoFind");
			// 
			// menuGotoCatalog
			// 
			this.menuGotoCatalog.Index = 2;
			resources.ApplyResources(this.menuGotoCatalog, "menuGotoCatalog");
			// 
			// menuGotoFaxIn
			// 
			this.menuGotoFaxIn.Index = 3;
			resources.ApplyResources(this.menuGotoFaxIn, "menuGotoFaxIn");
			// 
			// menuGotoScaner
			// 
			this.menuGotoScaner.Index = 4;
			resources.ApplyResources(this.menuGotoScaner, "menuGotoScaner");
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 5;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuPrintPage,
            this.menuPrintSelection,
            this.menuPrintAll,
            this.menuPrintDoc,
            this.menuFolder,
            this.menuItem2,
            this.menuSendMessage,
            this.menuSendFax});
			resources.ApplyResources(this.menuItem3, "menuItem3");
			// 
			// menuPrintPage
			// 
			this.menuPrintPage.Index = 0;
			resources.ApplyResources(this.menuPrintPage, "menuPrintPage");
			// 
			// menuPrintSelection
			// 
			resources.ApplyResources(this.menuPrintSelection, "menuPrintSelection");
			this.menuPrintSelection.Index = 1;
			this.menuPrintSelection.Click += new System.EventHandler(this.menuPrintSelection_Click);
			// 
			// menuPrintAll
			// 
			this.menuPrintAll.Index = 2;
			resources.ApplyResources(this.menuPrintAll, "menuPrintAll");
			this.menuPrintAll.Click += new System.EventHandler(this.menuPrintAll_Click);
			// 
			// menuPrintDoc
			// 
			this.menuPrintDoc.Index = 3;
			resources.ApplyResources(this.menuPrintDoc, "menuPrintDoc");
			// 
			// menuFolder
			// 
			this.menuFolder.Index = 4;
			resources.ApplyResources(this.menuFolder, "menuFolder");
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 5;
			resources.ApplyResources(this.menuItem2, "menuItem2");
			// 
			// menuSendMessage
			// 
			this.menuSendMessage.Index = 6;
			resources.ApplyResources(this.menuSendMessage, "menuSendMessage");
			// 
			// menuSendFax
			// 
			this.menuSendFax.Index = 7;
			resources.ApplyResources(this.menuSendFax, "menuSendFax");
			// 
			// menuNewLink
			// 
			this.menuNewLink.Index = 6;
			this.menuNewLink.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuLinkOpenDoc,
            this.menuLinkWorkDoc,
            this.menuLinkDoc});
			resources.ApplyResources(this.menuNewLink, "menuNewLink");
			this.menuNewLink.Popup += new System.EventHandler(this.menuNewLink_Popup);
			// 
			// menuLinkOpenDoc
			// 
			this.menuLinkOpenDoc.Index = 0;
			resources.ApplyResources(this.menuLinkOpenDoc, "menuLinkOpenDoc");
			// 
			// menuLinkWorkDoc
			// 
			this.menuLinkWorkDoc.Index = 1;
			resources.ApplyResources(this.menuLinkWorkDoc, "menuLinkWorkDoc");
			// 
			// menuLinkDoc
			// 
			this.menuLinkDoc.Index = 2;
			resources.ApplyResources(this.menuLinkDoc, "menuLinkDoc");
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 7;
			this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuOpenFolder});
			resources.ApplyResources(this.menuItem4, "menuItem4");
			// 
			// menuOpenFolder
			// 
			this.menuOpenFolder.Index = 0;
			resources.ApplyResources(this.menuOpenFolder, "menuOpenFolder");
			// 
			// menuHelp
			// 
			this.menuHelp.Index = 8;
			resources.ApplyResources(this.menuHelp, "menuHelp");
			// 
			// menuItem9
			// 
			this.menuItem9.Index = -1;
			resources.ApplyResources(this.menuItem9, "menuItem9");
			// 
			// toolbarImageList
			// 
			this.toolbarImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("toolbarImageList.ImageStream")));
			this.toolbarImageList.TransparentColor = System.Drawing.SystemColors.Control;
			this.toolbarImageList.Images.SetKeyName(0, "");
			this.toolbarImageList.Images.SetKeyName(1, "");
			this.toolbarImageList.Images.SetKeyName(2, "");
			this.toolbarImageList.Images.SetKeyName(3, "");
			this.toolbarImageList.Images.SetKeyName(4, "");
			this.toolbarImageList.Images.SetKeyName(5, "");
			this.toolbarImageList.Images.SetKeyName(6, "");
			this.toolbarImageList.Images.SetKeyName(7, "");
			this.toolbarImageList.Images.SetKeyName(8, "");
			this.toolbarImageList.Images.SetKeyName(9, "");
			this.toolbarImageList.Images.SetKeyName(10, "");
			this.toolbarImageList.Images.SetKeyName(11, "");
			this.toolbarImageList.Images.SetKeyName(12, "");
			this.toolbarImageList.Images.SetKeyName(13, "");
			this.toolbarImageList.Images.SetKeyName(14, "");
			this.toolbarImageList.Images.SetKeyName(15, "");
			this.toolbarImageList.Images.SetKeyName(16, "");
			this.toolbarImageList.Images.SetKeyName(17, "");
			this.toolbarImageList.Images.SetKeyName(18, "");
			this.toolbarImageList.Images.SetKeyName(19, "");
			this.toolbarImageList.Images.SetKeyName(20, "");
			this.toolbarImageList.Images.SetKeyName(21, "");
			this.toolbarImageList.Images.SetKeyName(22, "");
			this.toolbarImageList.Images.SetKeyName(23, "");
			this.toolbarImageList.Images.SetKeyName(24, "");
			this.toolbarImageList.Images.SetKeyName(25, "");
			this.toolbarImageList.Images.SetKeyName(26, "");
			this.toolbarImageList.Images.SetKeyName(27, "");
			this.toolbarImageList.Images.SetKeyName(28, "");
			this.toolbarImageList.Images.SetKeyName(29, "");
			this.toolbarImageList.Images.SetKeyName(30, "");
			this.toolbarImageList.Images.SetKeyName(31, "");
			this.toolbarImageList.Images.SetKeyName(32, "");
			this.toolbarImageList.Images.SetKeyName(33, "");
			this.toolbarImageList.Images.SetKeyName(34, "");
			this.toolbarImageList.Images.SetKeyName(35, "");
			this.toolbarImageList.Images.SetKeyName(36, "");
			this.toolbarImageList.Images.SetKeyName(37, "");
			this.toolbarImageList.Images.SetKeyName(38, "");
			this.toolbarImageList.Images.SetKeyName(39, "");
			this.toolbarImageList.Images.SetKeyName(40, "");
			this.toolbarImageList.Images.SetKeyName(41, "");
			this.toolbarImageList.Images.SetKeyName(42, "");
			this.toolbarImageList.Images.SetKeyName(43, "");
			this.toolbarImageList.Images.SetKeyName(44, "");
			this.toolbarImageList.Images.SetKeyName(45, "");
			this.toolbarImageList.Images.SetKeyName(46, "");
			this.toolbarImageList.Images.SetKeyName(47, "ДСП");
			// 
			// statusBar
			// 
			resources.ApplyResources(this.statusBar, "statusBar");
			this.statusBar.Name = "statusBar";
			this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanelArchive,
            this.statusBarPanelDoc,
            this.statusBarPanelSecure,
            this.statusBarPanelDSP,
            this.statusBarPanelCount,
            this.statusBarPanelPage,
            this.statusBarPanelDate});
			this.statusBar.ShowPanels = true;
			// 
			// statusBarPanelArchive
			// 
			resources.ApplyResources(this.statusBarPanelArchive, "statusBarPanelArchive");
			// 
			// statusBarPanelDoc
			// 
			resources.ApplyResources(this.statusBarPanelDoc, "statusBarPanelDoc");
			// 
			// statusBarPanelSecure
			// 
			resources.ApplyResources(this.statusBarPanelSecure, "statusBarPanelSecure");
			// 
			// statusBarPanelDSP
			// 
			resources.ApplyResources(this.statusBarPanelDSP, "statusBarPanelDSP");
			// 
			// statusBarPanelCount
			// 
			resources.ApplyResources(this.statusBarPanelCount, "statusBarPanelCount");
			this.statusBarPanelCount.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			// 
			// statusBarPanelPage
			// 
			resources.ApplyResources(this.statusBarPanelPage, "statusBarPanelPage");
			// 
			// statusBarPanelDate
			// 
			resources.ApplyResources(this.statusBarPanelDate, "statusBarPanelDate");
			// 
			// statusBarPanelTime
			// 
			resources.ApplyResources(this.statusBarPanelTime, "statusBarPanelTime");
			// 
			// info
			// 
			resources.ApplyResources(this.info, "info");
			this.info.Name = "info";
			this.info.ReadOnly = true;
			// 
			// zoomCombo
			// 
			this.zoomCombo.BackColor = System.Drawing.SystemColors.Window;
			this.zoomCombo.DropDownWidth = 72;
			this.zoomCombo.ForeColor = System.Drawing.SystemColors.ControlText;
			this.zoomCombo.Items.AddRange(new object[] {
            resources.GetString("zoomCombo.Items"),
            resources.GetString("zoomCombo.Items1"),
            resources.GetString("zoomCombo.Items2"),
            resources.GetString("zoomCombo.Items3"),
            resources.GetString("zoomCombo.Items4"),
            resources.GetString("zoomCombo.Items5"),
            resources.GetString("zoomCombo.Items6"),
            resources.GetString("zoomCombo.Items7")});
			this.zoomCombo.Name = "zoomCombo";
			resources.ApplyResources(this.zoomCombo, "zoomCombo");
			this.zoomCombo.TextChanged += new System.EventHandler(this.zoomCombo_TextChanged);
			// 
			// pageNum
			// 
			this.pageNum.BackColor = System.Drawing.SystemColors.Window;
			resources.ApplyResources(this.pageNum, "pageNum");
			this.pageNum.ForeColor = System.Drawing.SystemColors.ControlText;
			this.pageNum.Name = "pageNum";
			this.pageNum.ReadOnly = true;
			this.pageNum.TextChanged += new System.EventHandler(this.pageNum_TextChanged);
			// 
			// notifyIcon
			// 
			this.notifyIcon.ContextMenu = this.notifyMenu;
			resources.ApplyResources(this.notifyIcon, "notifyIcon");
			this.notifyIcon.DoubleClick += new System.EventHandler(this.mIShowWindow_Click);
			this.notifyIcon.MouseUp += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseUp);
			// 
			// notifyMenu
			// 
			this.notifyMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mIShowWindow,
            this.menuDoc9,
            this.mIClose});
			this.notifyMenu.Popup += new System.EventHandler(this.notifyMenu_Popup);
			// 
			// mIShowWindow
			// 
			this.mIShowWindow.DefaultItem = true;
			this.mIShowWindow.Index = 0;
			resources.ApplyResources(this.mIShowWindow, "mIShowWindow");
			this.mIShowWindow.Click += new System.EventHandler(this.mIShowWindow_Click);
			// 
			// menuDoc9
			// 
			this.menuDoc9.Index = 1;
			resources.ApplyResources(this.menuDoc9, "menuDoc9");
			// 
			// mIClose
			// 
			this.mIClose.Index = 2;
			resources.ApplyResources(this.mIClose, "mIClose");
			this.mIClose.Click += new System.EventHandler(this.mIClose_Click);
			// 
			// toolBar
			// 
			this.toolBar.ImageList = this.toolbarImageList;
			resources.ApplyResources(this.toolBar, "toolBar");
			this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSearch,
            this.windowButton,
            this.separator3,
            this.scanButton,
            this.propertiesButton,
            this.saveButton,
            this.savePartButton,
            this.saveSelectedButton,
            this.separator6,
            this.refreshButton,
            this.settingsGroupOrderButton,
            this.settingsFilterButton,
            this.separator11,
            this.undoButton,
            this.redoButton,
            this.separator9_,
            this.openFolderButton,
            this.separator1,
            this.showPagesPanelButton,
            this.showWebPanelButton,
            this.tBBShowMessage,
            this.separator4,
            this.pageBackButton,
            this.pageNum,
            this.pageForwardButton,
            this.separator5,
            this.zoomCombo,
            this.zoomInButton,
            this.zoomOutButton,
            this.zoomSelectionButton,
            this.separator10,
            this.viewButton,
            this.selectAnnButton,
            this.selectionButton,
            this.separator2,
            this.rotateCCWButton,
            this.rotateCWButton,
            this.separator7,
            this.printButton,
            this.sendMessageButton,
            this.sendFaxButton,
            this.separator8,
            this.linksButton,
            this.tbbGoTo,
            this.gotoWorkFolderButton,
            this.gotoScanerButton,
            this.gotoFaxInButton,
            this.profButton,
            this.gotoFoloderButton});
			this.toolBar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
			this.toolBar.Name = "toolBar";
			this.toolBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// buttonSearch
			// 
			this.buttonSearch.BackColor = System.Drawing.SystemColors.Control;
			this.buttonSearch.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.buttonSearch, "buttonSearch");
			this.buttonSearch.Name = "buttonSearch";
			this.buttonSearch.Tag = "Search";
			// 
			// windowButton
			// 
			this.windowButton.BackColor = System.Drawing.SystemColors.Control;
			this.windowButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.windowButton, "windowButton");
			this.windowButton.Name = "windowButton";
			this.windowButton.Tag = "Window";
			// 
			// separator3
			// 
			this.separator3.Name = "separator3";
			resources.ApplyResources(this.separator3, "separator3");
			// 
			// scanButton
			// 
			this.scanButton.BackColor = System.Drawing.SystemColors.Control;
			this.scanButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.scanButton, "scanButton");
			this.scanButton.Name = "scanButton";
			this.scanButton.Tag = "Scan";
			// 
			// propertiesButton
			// 
			this.propertiesButton.BackColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.propertiesButton, "propertiesButton");
			this.propertiesButton.ForeColor = System.Drawing.SystemColors.Control;
			this.propertiesButton.Name = "propertiesButton";
			this.propertiesButton.Tag = "Property";
			// 
			// saveButton
			// 
			this.saveButton.BackColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.saveButton, "saveButton");
			this.saveButton.ForeColor = System.Drawing.SystemColors.Control;
			this.saveButton.Name = "saveButton";
			this.saveButton.Tag = "Save";
			// 
			// savePartButton
			// 
			this.savePartButton.BackColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.savePartButton, "savePartButton");
			this.savePartButton.ForeColor = System.Drawing.SystemColors.Control;
			this.savePartButton.Name = "savePartButton";
			this.savePartButton.Tag = "SavePart";
			// 
			// saveSelectedButton
			// 
			this.saveSelectedButton.BackColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.saveSelectedButton, "saveSelectedButton");
			this.saveSelectedButton.ForeColor = System.Drawing.SystemColors.Control;
			this.saveSelectedButton.Name = "saveSelectedButton";
			this.saveSelectedButton.Tag = "SaveSelected";
			// 
			// separator6
			// 
			this.separator6.Name = "separator6";
			resources.ApplyResources(this.separator6, "separator6");
			// 
			// refreshButton
			// 
			this.refreshButton.BackColor = System.Drawing.SystemColors.Control;
			this.refreshButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.refreshButton, "refreshButton");
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Tag = "Refresh";
			// 
			// settingsGroupOrderButton
			// 
			this.settingsGroupOrderButton.BackColor = System.Drawing.SystemColors.Control;
			this.settingsGroupOrderButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.settingsGroupOrderButton, "settingsGroupOrderButton");
			this.settingsGroupOrderButton.Name = "settingsGroupOrderButton";
			this.settingsGroupOrderButton.Tag = "SettingsGroupOrder";
			// 
			// settingsFilterButton
			// 
			this.settingsFilterButton.BackColor = System.Drawing.SystemColors.Control;
			this.settingsFilterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.settingsFilterButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.settingsFilterButton, "settingsFilterButton");
			this.settingsFilterButton.Name = "settingsFilterButton";
			this.settingsFilterButton.Tag = "SettingsFilter";
			// 
			// separator11
			// 
			this.separator11.Name = "separator11";
			resources.ApplyResources(this.separator11, "separator11");
			// 
			// undoButton
			// 
			this.undoButton.BackColor = System.Drawing.SystemColors.Control;
			this.undoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.undoButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.undoButton, "undoButton");
			this.undoButton.Name = "undoButton";
			this.undoButton.Tag = "Undo";
			this.undoButton.DropDownOpening += new System.EventHandler(this.undoButton_DropDownOpening);
			this.undoButton.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.undoButton_DropDownItemClicked);
			// 
			// redoButton
			// 
			this.redoButton.BackColor = System.Drawing.SystemColors.Control;
			this.redoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.redoButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.redoButton, "redoButton");
			this.redoButton.Name = "redoButton";
			this.redoButton.Tag = "Redo";
			this.redoButton.DropDownOpening += new System.EventHandler(this.redoButton_DropDownOpening);
			this.redoButton.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.redoButton_DropDownItemClicked);
			// 
			// separator9_
			// 
			this.separator9_.Name = "separator9_";
			resources.ApplyResources(this.separator9_, "separator9_");
			// 
			// openFolderButton
			// 
			this.openFolderButton.BackColor = System.Drawing.SystemColors.Control;
			this.openFolderButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.openFolderButton, "openFolderButton");
			this.openFolderButton.Name = "openFolderButton";
			this.openFolderButton.Tag = "OpenFolder";
			// 
			// separator1
			// 
			this.separator1.Name = "separator1";
			resources.ApplyResources(this.separator1, "separator1");
			// 
			// showPagesPanelButton
			// 
			this.showPagesPanelButton.BackColor = System.Drawing.SystemColors.Control;
			this.showPagesPanelButton.CheckOnClick = true;
			this.showPagesPanelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.showPagesPanelButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.showPagesPanelButton, "showPagesPanelButton");
			this.showPagesPanelButton.Name = "showPagesPanelButton";
			this.showPagesPanelButton.Tag = "ShowPagesPanel";
			this.showPagesPanelButton.Click += new System.EventHandler(this.showPagesPanelButton_Click);
			// 
			// showWebPanelButton
			// 
			this.showWebPanelButton.BackColor = System.Drawing.SystemColors.Control;
			this.showWebPanelButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.showWebPanelButton, "showWebPanelButton");
			this.showWebPanelButton.Name = "showWebPanelButton";
			this.showWebPanelButton.Tag = "ShowWeb";
			// 
			// tBBShowMessage
			// 
			this.tBBShowMessage.Checked = false;
			this.tBBShowMessage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tBBShowMessage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TS_menuItemBetween,
            this.TS_menuItemLeft,
            this.TS_menuItemUnder});
			resources.ApplyResources(this.tBBShowMessage, "tBBShowMessage");
			this.tBBShowMessage.Name = "tBBShowMessage";
			this.tBBShowMessage.Overflow = System.Windows.Forms.ToolStripItemOverflow.Always;
			this.tBBShowMessage.Tag = "ShowMessage";
			// 
			// TS_menuItemBetween
			// 
			this.TS_menuItemBetween.Name = "TS_menuItemBetween";
			resources.ApplyResources(this.TS_menuItemBetween, "TS_menuItemBetween");
			this.TS_menuItemBetween.Click += new System.EventHandler(this.menuItemBetween_Click);
			// 
			// TS_menuItemLeft
			// 
			this.TS_menuItemLeft.Name = "TS_menuItemLeft";
			resources.ApplyResources(this.TS_menuItemLeft, "TS_menuItemLeft");
			this.TS_menuItemLeft.Click += new System.EventHandler(this.menuItemLeft_Click);
			// 
			// TS_menuItemUnder
			// 
			this.TS_menuItemUnder.Name = "TS_menuItemUnder";
			resources.ApplyResources(this.TS_menuItemUnder, "TS_menuItemUnder");
			this.TS_menuItemUnder.Click += new System.EventHandler(this.menuItemUnder_Click);
			// 
			// separator4
			// 
			this.separator4.Name = "separator4";
			resources.ApplyResources(this.separator4, "separator4");
			// 
			// pageBackButton
			// 
			this.pageBackButton.BackColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.pageBackButton, "pageBackButton");
			this.pageBackButton.ForeColor = System.Drawing.SystemColors.Control;
			this.pageBackButton.Name = "pageBackButton";
			this.pageBackButton.Tag = "PageBack";
			// 
			// pageForwardButton
			// 
			this.pageForwardButton.BackColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.pageForwardButton, "pageForwardButton");
			this.pageForwardButton.ForeColor = System.Drawing.SystemColors.Control;
			this.pageForwardButton.Name = "pageForwardButton";
			this.pageForwardButton.Tag = "PageForward";
			// 
			// separator5
			// 
			this.separator5.Name = "separator5";
			resources.ApplyResources(this.separator5, "separator5");
			// 
			// zoomInButton
			// 
			this.zoomInButton.BackColor = System.Drawing.SystemColors.Control;
			this.zoomInButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.zoomInButton, "zoomInButton");
			this.zoomInButton.Name = "zoomInButton";
			this.zoomInButton.Tag = "ZoomIn";
			// 
			// zoomOutButton
			// 
			this.zoomOutButton.BackColor = System.Drawing.SystemColors.Control;
			this.zoomOutButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.zoomOutButton, "zoomOutButton");
			this.zoomOutButton.Name = "zoomOutButton";
			this.zoomOutButton.Tag = "ZoomOut";
			// 
			// zoomSelectionButton
			// 
			this.zoomSelectionButton.BackColor = System.Drawing.SystemColors.Control;
			this.zoomSelectionButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.zoomSelectionButton, "zoomSelectionButton");
			this.zoomSelectionButton.Name = "zoomSelectionButton";
			this.zoomSelectionButton.Tag = "ZoomSelection";
			// 
			// separator10
			// 
			this.separator10.Name = "separator10";
			resources.ApplyResources(this.separator10, "separator10");
			// 
			// viewButton
			// 
			this.viewButton.BackColor = System.Drawing.SystemColors.Control;
			this.viewButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.viewButton, "viewButton");
			this.viewButton.Name = "viewButton";
			this.viewButton.Tag = "View";
			// 
			// selectAnnButton
			// 
			this.selectAnnButton.BackColor = System.Drawing.SystemColors.Control;
			this.selectAnnButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.selectAnnButton, "selectAnnButton");
			this.selectAnnButton.Name = "selectAnnButton";
			this.selectAnnButton.Tag = "SelectAnn";
			// 
			// selectionButton
			// 
			this.selectionButton.BackColor = System.Drawing.SystemColors.Control;
			this.selectionButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.selectionButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.selectionButton, "selectionButton");
			this.selectionButton.Name = "selectionButton";
			this.selectionButton.Tag = "Selection";
			// 
			// separator2
			// 
			this.separator2.BackColor = System.Drawing.SystemColors.Control;
			this.separator2.ForeColor = System.Drawing.SystemColors.Control;
			this.separator2.Name = "separator2";
			resources.ApplyResources(this.separator2, "separator2");
			// 
			// rotateCCWButton
			// 
			this.rotateCCWButton.BackColor = System.Drawing.SystemColors.Control;
			this.rotateCCWButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.rotateCCWButton, "rotateCCWButton");
			this.rotateCCWButton.Name = "rotateCCWButton";
			this.rotateCCWButton.Tag = "RotateCCW";
			// 
			// rotateCWButton
			// 
			this.rotateCWButton.BackColor = System.Drawing.SystemColors.Control;
			this.rotateCWButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.rotateCWButton, "rotateCWButton");
			this.rotateCWButton.Name = "rotateCWButton";
			this.rotateCWButton.Tag = "RotateCW";
			// 
			// separator7
			// 
			this.separator7.Name = "separator7";
			resources.ApplyResources(this.separator7, "separator7");
			// 
			// printButton
			// 
			this.printButton.BackColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.printButton, "printButton");
			this.printButton.ForeColor = System.Drawing.SystemColors.Control;
			this.printButton.Name = "printButton";
			this.printButton.Tag = "Print";
			// 
			// sendMessageButton
			// 
			this.sendMessageButton.BackColor = System.Drawing.SystemColors.Control;
			this.sendMessageButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.sendMessageButton, "sendMessageButton");
			this.sendMessageButton.Name = "sendMessageButton";
			this.sendMessageButton.Tag = "SendMessage";
			// 
			// sendFaxButton
			// 
			this.sendFaxButton.BackColor = System.Drawing.SystemColors.Control;
			this.sendFaxButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.sendFaxButton, "sendFaxButton");
			this.sendFaxButton.Name = "sendFaxButton";
			this.sendFaxButton.Tag = "SendFax";
			// 
			// separator8
			// 
			this.separator8.Name = "separator8";
			resources.ApplyResources(this.separator8, "separator8");
			// 
			// linksButton
			// 
			this.linksButton.BackColor = System.Drawing.SystemColors.Control;
			this.linksButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.linksButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.linksButton, "linksButton");
			this.linksButton.Name = "linksButton";
			this.linksButton.Tag = "Links";
			// 
			// tbbGoTo
			// 
			this.tbbGoTo.BackColor = System.Drawing.SystemColors.Control;
			this.tbbGoTo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.tbbGoTo, "tbbGoTo");
			this.tbbGoTo.ForeColor = System.Drawing.SystemColors.Control;
			this.tbbGoTo.Name = "tbbGoTo";
			this.tbbGoTo.Tag = "Goto";
			// 
			// gotoWorkFolderButton
			// 
			this.gotoWorkFolderButton.BackColor = System.Drawing.SystemColors.Control;
			this.gotoWorkFolderButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.gotoWorkFolderButton, "gotoWorkFolderButton");
			this.gotoWorkFolderButton.Name = "gotoWorkFolderButton";
			this.gotoWorkFolderButton.Tag = "GotoWorkFolder";
			// 
			// gotoScanerButton
			// 
			this.gotoScanerButton.BackColor = System.Drawing.SystemColors.Control;
			this.gotoScanerButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.gotoScanerButton, "gotoScanerButton");
			this.gotoScanerButton.Name = "gotoScanerButton";
			this.gotoScanerButton.Tag = "GotoScaner";
			// 
			// gotoFaxInButton
			// 
			this.gotoFaxInButton.BackColor = System.Drawing.SystemColors.Control;
			this.gotoFaxInButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.gotoFaxInButton, "gotoFaxInButton");
			this.gotoFaxInButton.Name = "gotoFaxInButton";
			this.gotoFaxInButton.Tag = "GotoFaxIn";
			// 
			// profButton
			// 
			this.profButton.BackColor = System.Drawing.SystemColors.Control;
			this.profButton.ForeColor = System.Drawing.SystemColors.Control;
			this.profButton.Name = "profButton";
			resources.ApplyResources(this.profButton, "profButton");
			this.profButton.Tag = "11";
			// 
			// gotoFoloderButton
			// 
			this.gotoFoloderButton.BackColor = System.Drawing.SystemColors.Control;
			this.gotoFoloderButton.ForeColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.gotoFoloderButton, "gotoFoloderButton");
			this.gotoFoloderButton.Name = "gotoFoloderButton";
			this.gotoFoloderButton.Tag = "GotoCatalog";
			// 
			// blank4
			// 
			resources.ApplyResources(this.blank4, "blank4");
			this.blank4.Name = "blank4";
			// 
			// blank1
			// 
			resources.ApplyResources(this.blank1, "blank1");
			this.blank1.Name = "blank1";
			// 
			// blankseparator1
			// 
			resources.ApplyResources(this.blankseparator1, "blankseparator1");
			this.blankseparator1.Name = "blankseparator1";
			// 
			// blank2
			// 
			resources.ApplyResources(this.blank2, "blank2");
			this.blank2.Name = "blank2";
			// 
			// blankseparator2
			// 
			resources.ApplyResources(this.blankseparator2, "blankseparator2");
			this.blankseparator2.Name = "blankseparator2";
			// 
			// blank3
			// 
			resources.ApplyResources(this.blank3, "blank3");
			this.blank3.Name = "blank3";
			// 
			// blankseparator3
			// 
			resources.ApplyResources(this.blankseparator3, "blankseparator3");
			this.blankseparator3.Name = "blankseparator3";
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.toolBar, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.opinionControl, 1, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// opinionControl
			// 
			resources.ApplyResources(this.opinionControl, "opinionControl");
			this.opinionControl.ConnectionString = null;
			this.opinionControl.Name = "opinionControl";
			// 
			// annotationBar
			// 
			this.annotationBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.selectAnnButton1,
            this.markerButton,
            this.rectangleButton,
            this.textButton,
            this.NoteButton,
            this.imageStampButton,
            this.dspStampButton});
			resources.ApplyResources(this.annotationBar, "annotationBar");
			this.annotationBar.ImageList = this.toolbarImageList;
			this.annotationBar.Name = "annotationBar";
			this.annotationBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.annotationBar_MouseUp);
			// 
			// selectAnnButton1
			// 
			resources.ApplyResources(this.selectAnnButton1, "selectAnnButton1");
			this.selectAnnButton1.Name = "selectAnnButton1";
			this.selectAnnButton1.Tag = "SelectAnn";
			// 
			// markerButton
			// 
			resources.ApplyResources(this.markerButton, "markerButton");
			this.markerButton.Name = "markerButton";
			this.markerButton.Tag = "Marker";
			// 
			// rectangleButton
			// 
			resources.ApplyResources(this.rectangleButton, "rectangleButton");
			this.rectangleButton.Name = "rectangleButton";
			this.rectangleButton.Tag = "Rectangle";
			// 
			// textButton
			// 
			resources.ApplyResources(this.textButton, "textButton");
			this.textButton.Name = "textButton";
			this.textButton.Tag = "Text";
			// 
			// NoteButton
			// 
			resources.ApplyResources(this.NoteButton, "NoteButton");
			this.NoteButton.Name = "NoteButton";
			this.NoteButton.Tag = "Note";
			// 
			// imageStampButton
			// 
			resources.ApplyResources(this.imageStampButton, "imageStampButton");
			this.imageStampButton.Name = "imageStampButton";
			this.imageStampButton.Tag = "ImageStamp";
			// 
			// dspStampButton
			// 
			resources.ApplyResources(this.dspStampButton, "dspStampButton");
			this.dspStampButton.Name = "dspStampButton";
			this.dspStampButton.Tag = "DSPStamp";
			// 
			// MainFormDialog
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.splitContainerMain);
			this.Controls.Add(this.info);
			this.Controls.Add(this.annotationBar);
			this.Controls.Add(this.statusBar);
			this.Controls.Add(this.tableLayoutPanel1);
			this.KeyPreview = true;
			this.Menu = this.mainMenu;
			this.Name = "MainFormDialog";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormDialog_FormClosing);
			this.Load += new System.EventHandler(this.MainFormDialog_Load);
			this.LocationChanged += new System.EventHandler(this.MainFormDialog_LocationChanged);
			this.SizeChanged += new System.EventHandler(this.MainFormDialog_SizeChanged);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainFormDialog_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainFormDialog_KeyUp);
			this.splitContainerMain.Panel1.ResumeLayout(false);
			this.splitContainerMain.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
			this.splitContainerMain.ResumeLayout(false);
			this.splitContainerGrids.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerGrids)).EndInit();
			this.splitContainerGrids.ResumeLayout(false);
			this.splitContainerTree.Panel1.ResumeLayout(false);
			this.splitContainerTree.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerTree)).EndInit();
			this.splitContainerTree.ResumeLayout(false);
			this.splitContainerList.Panel1.ResumeLayout(false);
			this.splitContainerList.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerList)).EndInit();
			this.splitContainerList.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.infoGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.docGrid)).EndInit();
			this.splitContainerDoc.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerDoc)).EndInit();
			this.splitContainerDoc.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelArchive)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelDoc)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelSecure)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelDSP)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelCount)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelPage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelDate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanelTime)).EndInit();
			this.toolBar.ResumeLayout(false);
			this.toolBar.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		#region GUI

		private MainMenu mainMenu;
		private MenuItem menuItem3;
		private ToolStrip toolBar;
		private ImageList toolbarImageList;
		private StatusBar statusBar;
		private ToolStripButton showPagesPanelButton;
		internal FolderTree.FolderTree folders;
		private RichTextBox info;
		private ToolStripButton refreshButton;
		private ToolStripComboBox zoomCombo;
		private ToolStripSeparator separator1;
		private ToolStripButton blank1;
		private ToolStripButton blank2;
		private ToolStripSeparator separator2;
		private ToolStripButton zoomInButton;
		private ToolStripButton zoomOutButton;
		private ToolStripButton selectionButton;
		private ToolStripButton blank3;
		private ToolStripButton blankseparator1;
		private ToolStripButton blankseparator2;
		private ToolStripButton blankseparator3;
		private ToolStripButton zoomSelectionButton;
		private ToolStripSeparator separator3;
		private ToolStripButton rotateCWButton;
		private ToolStripButton rotateCCWButton;
		private ToolStripButton openFolderButton;
		private ToolStripSeparator separator4;
		private ImageList foldersList;
		internal Grids.DocGrid docGrid;
		private Grids.InfoGrid infoGrid;
		private ToolStripButton pageBackButton;
		private ToolStripButton pageForwardButton;
		private ToolStripButton blank4;
		private ToolStripSeparator separator5;
		internal ToolStripTextBox pageNum;
		private ToolStripButton scanButton;
		private ToolStripSeparator separator6;
		private ToolStripButton saveButton;
		private ToolStripButton printButton;
		private MenuItem menuPrintSettings;
		private MenuItem menuPrintPage;
		private MenuItem menuPrintAll;
		private MenuItem menuPrintSelection;
		private MenuItem menuItem2;
		private MenuItem menuDoc;
		private MenuItem menuSave;
		private MenuItem menuItem5;
		private MenuItem menuZoom;
		private MenuItem menuItem7;
		private MenuItem menuZoomIn;
		private MenuItem menuZoomOut;
		private MenuItem menuDoc0;
		private MenuItem menuSelection;
		private MenuItem menuZoomSelection;
		private MenuItem menuItem6;
		private MenuItem menuDoc1;
		private MenuItem menuRotateCCW;
		private MenuItem menuRotateCW;
		private MenuItem menuPageForward;
		private MenuItem menuPageBack;
		private ToolStripButton sendMessageButton;
		private MenuItem menuSendMessage;
		private MenuItem menuItem8;
		private MenuItem mIHide;
		private MenuItem mISelf;
		private MenuItem mIShow;
		private MenuItem mILine;
		private MenuItem menuDoc3;
		private MenuItem mIHighlighter;
		private MenuItem mIRectangle;
		private MenuItem mIHRectangle;
		private MenuItem mIText;
		private MenuItem mIView;
		private MenuItem mISelect;
		private MenuItem mINote;
		private MenuItem mIFLine;
		private ToolStripButton windowButton;
		private MenuItem mISeparator;
		private MenuItem mINotes;
		private ToolStripButton sendFaxButton;
		private MenuItem menuSendFax;
		private StatusBarPanel statusBarPanelTime;
		private StatusBarPanel statusBarPanelDate;
		private StatusBarPanel statusBarPanelPage;
		private StatusBarPanel statusBarPanelCount;
		private StatusBarPanel statusBarPanelDoc;
		private ToolStripSeparator separator7;
		private ToolStripSeparator separator8;
		private ToolStripButton gotoWorkFolderButton;
		private ToolStripButton gotoScanerButton;
		private ToolStripButton gotoFaxInButton;
		private ToolStripButton settingsGroupOrderButton;
		private ToolStripButton settingsFilterButton;
		private MenuItem menuItem9;
		private MenuItem menuSavePart;
		private MenuItem menuDoc2;
		private MenuItem menuLinkEform;
		private MenuItem menuPageMoveBack;
		private MenuItem menuPageMoveForward;
		private MenuItem menuDoc4;
		private MenuItem menuDoc5;
		private MenuItem menuDoc6;
		private MenuItem menuColor1;
		private MenuItem menuColor2;
		private MenuItem menuColor3;
		private MenuItem menuScale1;
		private MenuItem menuScale2;
		private MenuItem menuScale3;
		private MenuItem menuGotoCatalog;
		private MenuItem menuGotoWorkFolder;
		private MenuItem menuGotoScaner;
		private MenuItem menuGotoFaxIn;
		private MenuItem menuDoc8;
		private MenuItem menuShowPagesPanel;
		private ToolStripButton linksButton;
		private ToolStripButton profButton;
		private ToolStripButton viewButton;
		private ToolStripSeparator separator10;
		private ToolStripButton selectAnnButton;
		private ToolBarButton selectAnnButton1;
		private ToolBarButton markerButton;
		private ToolBarButton rectangleButton;
		private ToolBarButton textButton;
		private ToolBarButton NoteButton;
		private TaggedToolBar annotationBar;
		public NotifyIcon notifyIcon;
		private MenuItem mIShowWindow;
		private MenuItem mIClose;
		private ContextMenu notifyMenu;
		private MenuItem menuDoc9;
		private MenuItem menuDoc11;
		private MenuItem menuItem4;
		private MenuItem menuOpenFolder;
		private MenuItem menuGoto;
		private ToolStripButton gotoFoloderButton;
		private ToolStripSeparator separator9_;
		private ToolStripButton tbbGoTo;
		private MenuItem menuRefresh;
		private MenuItem menuShowNoteBar;
		private ToolStripButton savePartButton;
		private ToolStripButton buttonSearch;
		private MenuItem menuSearch;
		private MenuItem menuFindID;
		private MenuItem menuItem22;
		private MenuItem menuDoc7;
		private MenuItem menuGotoFind;
		private MenuItem menuNewWindow;
		private MenuItem menuDocPropertes;
		private MenuItem menuLinkOpenDoc;
		private MenuItem menuLinkWorkDoc;
		private MenuItem menuLinkDoc;
		private MenuItem menuScan;
		private MenuItem menuScanCurrentDoc;
		private MenuItem menuAddImageCurrentDoc;
		private MenuItem menuNewLink;
		private MenuItem menuAddDocData;
		private MenuItem menuAddEForm;
		private MenuItem menuSeparator;
		private MenuItem menuSeparator1;
		private MenuItem menuSeparator2;
		private ToolStripButton propertiesButton;
		internal Kesco.Lib.Win.Document.Controls.DocControl docControl;
		private MenuItem menuSeparator3;
		private MenuItem menuAddToWork;
		private MenuItem menuWorkPlaces;
		private MenuItem menuEndWork;
		private MenuItem menuSeparator4;
		private MenuItem menuFaxDescr;
		private MenuItem menuSpam;
		private MenuItem menuSeparator5;
		private MenuItem menuSeparator6;
		private MenuItem menuGotoPerson;
		private MenuItem menuSeparator7;
		private MenuItem menuDelete;
		private MenuItem menuDeleteFromFound;
		private MenuItem menuSettingsMessages;
		private MenuItem menuSettingsGroupOrder;
		private MenuItem menuSettingsFilter;
		private MenuItem menuSettingsScaner;
		private MenuItem menuSettingsLanguage;
		private MenuItem menuDeletePart;
		private MenuItem menuSettingsFaxes;
		private MenuItem menuItem1;
		private MenuItem menuItem10;
		private MenuItem menuSettingsMailingLists;
		private MenuItem menuFolder;
		private MenuItem menuPrintDoc;
		private ToolStripButton showWebPanelButton;
		private MenuItem menuHelp;
		private MenuItem menuItemSaveSelected;
		private ToolStripButton saveSelectedButton;
		private MenuItem menuSettingsShow;
		private Kesco.Lib.Win.Document.Items.ToolStripSplitButtonCheckable tBBShowMessage;
		private MenuItem menuItemBetween;
		private MenuItem menuItemLeft;
		private MenuItem menuItemUnder;
		private ToolStripMenuItem TS_menuItemBetween;
		private ToolStripMenuItem TS_menuItemLeft;
		private ToolStripMenuItem TS_menuItemUnder;
		private MenuItem menuShowMessage;
		private MenuItem menuShowWebPanel;
		private MenuItem mIColumns;
		private ToolStripSeparator separator11;
		private MenuItem mIStamp;
		private ToolBarButton imageStampButton;
		private SplitContainer splitContainerMain;
		private SplitContainer splitContainerGrids;
		private SplitContainer splitContainerTree;
		private SplitContainer splitContainerList;
		private SplitContainer splitContainerDoc;
		private MenuItem menuItemSaveSelectedAsStamp;
		private MenuItem menuSettingsLinkShow;
		private ToolStripSplitButton undoButton;
		private ToolStripSplitButton redoButton;
		private StatusBarPanel statusBarPanelArchive;
		private StatusBarPanel statusBarPanelSecure;
		private StatusBarPanel statusBarPanelDSP;
		private ToolBarButton dspStampButton;
		private MenuItem miDSP;
		private MenuItem menuItemExit;
		private TableLayoutPanel tableLayoutPanel1;
		private Lib.Win.Opinion.OpinionControl opinionControl;

		#endregion
	}
}