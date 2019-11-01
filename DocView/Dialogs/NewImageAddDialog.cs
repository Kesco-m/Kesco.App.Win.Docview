using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Kesco.Lib.Win.Document;
using Kesco.Lib.Win.Options;

namespace Kesco.App.Win.DocView.Dialogs
{
    /// <summary>
    /// Даилога добавления нового изображения к документу
    /// </summary>
    public class NewImageAddDialog : Kesco.Lib.Win.FreeDialog
    {
        private Folder subLayout;
        private Panel panelBottom;
        private Panel panelTree;
        private Panel panelDocControl;
        private Lib.Win.Document.Controls.DocControl docControl;
        private FolderTree.FolderTree tree;
        private ImageList foldersList;
        internal Grids.DocGrid docGrid;
        private Button buttonSave;
        private Button buttonCancel;
        private Splitter splitterVert;
        private Splitter splitterH;
        private CheckBox chbImageMain;
        private Button buttonAdd;
        private GroupBox groupBoxPages;
        private NumericUpDown minPageBox;
        private NumericUpDown maxPageBox;
        private Label label3;
        private RadioButton rbSelectedPages;
        private RadioButton rbAllPages;
        private IContainer components;

		System.Timers.Timer timer;

        public bool isImageMain
        {
            get { return chbImageMain.Checked; }
        }

        public string FileName { get; set; }
        public int DocID { get; private set; }

        public string ImageType
        {
            get { return string.IsNullOrEmpty(docControl.ImageType) ? "TIF" : docControl.ImageType; }
        }

        public bool IsPdf { get { return docControl.IsPDFMode; } }

        public int PagesCount
        {
            get
            {
                if (!groupBoxPages.Visible || rbAllPages.Checked)
                    return docControl.PageCount;
                else
                    return ((int)maxPageBox.Value - (int)minPageBox.Value + 1);
            }
        }

        public int MinPage
        {
            get { return groupBoxPages.Visible && rbSelectedPages.Checked ? (int)minPageBox.Value : 0; }
        }

        public int MaxPage
        {
            get { return groupBoxPages.Visible && rbSelectedPages.Checked ? (int)maxPageBox.Value : 0; }
        }

        public bool IsSavePart
        {
            get { return groupBoxPages.Visible && rbSelectedPages.Checked && ((int)minPageBox.Value > 1 || (int)maxPageBox.Value < docControl.PageCount); }
        }

        private EventHandler StartPage_ValueChanged;
        private EventHandler EndPage_ValueChanged;

        public NewImageAddDialog(int docId)
        {
            FileName = "";
            DocID = docId;

            InitializeComponent();

            docGrid.Init(Environment.Layout);
            docGrid.Style = Grids.Styles.Style.CreateOuterScanerStyle(docGrid);
            docGrid.SelectionChanged += docGrid_CurrentCellChanged;

            // Подписка на изменение файла(имени)
            var style = docGrid.Style as Grids.Styles.OuterScanerStyle;
            if (style != null)
                style.FileChanged += NewImageAddDialog_FileChanged;

            Text = Environment.StringResources.GetString("SelectImage");
            buttonSave.Text = Environment.StringResources.GetString("Save");
            buttonCancel.Text = Environment.StringResources.GetString("Cancel");
            chbImageMain.Text = Environment.StringResources.GetString("SetMain");

            groupBoxPages.Text = Environment.StringResources.GetString("groupBoxPages");
            rbAllPages.Text = Environment.StringResources.GetString("rbAllPages");
            rbSelectedPages.Text = Environment.StringResources.GetString("rbSelectedPages");
            label3.Text = Environment.StringResources.GetString("label3");
            groupBoxPages.Visible = false;
            
            StartPage_ValueChanged = new EventHandler(textBoxStartPage_ValueChanged);
            EndPage_ValueChanged = new EventHandler(textBoxEndPage_ValueChanged);

            minPageBox.Minimum = 1;
            minPageBox.ValueChanged += StartPage_ValueChanged;
            minPageBox.TextChanged += StartPage_ValueChanged;
            maxPageBox.ValueChanged += EndPage_ValueChanged;
            maxPageBox.TextChanged += EndPage_ValueChanged;
            rbSelectedPages.CheckedChanged += new EventHandler(rbSelectedPages_CheckedChanged);

            buttonAdd.Enabled = buttonSave.Enabled = false;
            StartPosition = FormStartPosition.CenterScreen;
            subLayout = Environment.Layout.Folders.Add(Name);
            LoadReg();
            Closed += NewImageAddDialog_Closed;
        }

        /// <summary>
        /// Обработчик изменения файла изображения
        /// </summary>
        private void NewImageAddDialog_FileChanged()
        {
            // Файл изменился, перезагружаю список файлов
            if (tree != null)
            {
                var node = tree.SelectedNode;

                if (node != null)
                {
                    docControl.FileName = "";
                    buttonAdd.Enabled = buttonSave.Enabled = false;

                    Cursor = Cursors.WaitCursor;
                    node.LoadDocs(docGrid, true, 0, null);
                    Cursor = Cursors.Default;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (docGrid != null)
            {
                // Отписываюсь от события
                var style = docGrid.Style as Grids.Styles.OuterScanerStyle;
                if (style != null)
                    style.FileChanged -= NewImageAddDialog_FileChanged;

                docGrid.Style = null;
                docGrid.Dispose();
                docGrid = null;
            }
            Grids.Styles.OuterScanerStyle.Clear();
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewImageAddDialog));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.chbImageMain = new System.Windows.Forms.CheckBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonSave = new System.Windows.Forms.Button();
			this.panelTree = new System.Windows.Forms.Panel();
			this.docGrid = new Kesco.App.Win.DocView.Grids.DocGrid();
			this.splitterH = new System.Windows.Forms.Splitter();
			this.tree = new Kesco.App.Win.DocView.FolderTree.FolderTree();
			this.foldersList = new System.Windows.Forms.ImageList(this.components);
			this.panelDocControl = new System.Windows.Forms.Panel();
			this.groupBoxPages = new System.Windows.Forms.GroupBox();
			this.minPageBox = new System.Windows.Forms.NumericUpDown();
			this.maxPageBox = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.rbSelectedPages = new System.Windows.Forms.RadioButton();
			this.rbAllPages = new System.Windows.Forms.RadioButton();
			this.docControl = new Kesco.Lib.Win.Document.Controls.DocControl();
			this.splitterVert = new System.Windows.Forms.Splitter();
			this.panelBottom.SuspendLayout();
			this.panelTree.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.docGrid)).BeginInit();
			this.panelDocControl.SuspendLayout();
			this.groupBoxPages.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.minPageBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.maxPageBox)).BeginInit();
			this.SuspendLayout();
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.buttonAdd);
			this.panelBottom.Controls.Add(this.chbImageMain);
			this.panelBottom.Controls.Add(this.buttonCancel);
			this.panelBottom.Controls.Add(this.buttonSave);
			resources.ApplyResources(this.panelBottom, "panelBottom");
			this.panelBottom.Name = "panelBottom";
			// 
			// buttonAdd
			// 
			resources.ApplyResources(this.buttonAdd, "buttonAdd");
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// chbImageMain
			// 
			this.chbImageMain.Checked = true;
			this.chbImageMain.CheckState = System.Windows.Forms.CheckState.Checked;
			resources.ApplyResources(this.chbImageMain, "chbImageMain");
			this.chbImageMain.Name = "chbImageMain";
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonSave
			// 
			resources.ApplyResources(this.buttonSave, "buttonSave");
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// panelTree
			// 
			this.panelTree.Controls.Add(this.docGrid);
			this.panelTree.Controls.Add(this.splitterH);
			this.panelTree.Controls.Add(this.tree);
			resources.ApplyResources(this.panelTree, "panelTree");
			this.panelTree.Name = "panelTree";
			// 
			// docGrid
			// 
			this.docGrid.AllowUserToAddRows = false;
			this.docGrid.AllowUserToDeleteRows = false;
			this.docGrid.AllowUserToResizeRows = false;
			this.docGrid.BackgroundColor = System.Drawing.SystemColors.Window;
			this.docGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.docGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.docGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.docGrid.DefaultCellStyle = dataGridViewCellStyle2;
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
			// 
			// splitterH
			// 
			resources.ApplyResources(this.splitterH, "splitterH");
			this.splitterH.Name = "splitterH";
			this.splitterH.TabStop = false;
			// 
			// tree
			// 
			resources.ApplyResources(this.tree, "tree");
			this.tree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
			this.tree.ImageList = this.foldersList;
			this.tree.ItemHeight = 16;
			this.tree.Name = "tree";
			this.tree.SelectedNode = null;
			this.tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterSelect);
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
			// 
			// panelDocControl
			// 
			this.panelDocControl.Controls.Add(this.groupBoxPages);
			this.panelDocControl.Controls.Add(this.docControl);
			resources.ApplyResources(this.panelDocControl, "panelDocControl");
			this.panelDocControl.Name = "panelDocControl";
			// 
			// groupBoxPages
			// 
			this.groupBoxPages.Controls.Add(this.minPageBox);
			this.groupBoxPages.Controls.Add(this.maxPageBox);
			this.groupBoxPages.Controls.Add(this.label3);
			this.groupBoxPages.Controls.Add(this.rbSelectedPages);
			this.groupBoxPages.Controls.Add(this.rbAllPages);
			resources.ApplyResources(this.groupBoxPages, "groupBoxPages");
			this.groupBoxPages.Name = "groupBoxPages";
			this.groupBoxPages.TabStop = false;
			// 
			// minPageBox
			// 
			resources.ApplyResources(this.minPageBox, "minPageBox");
			this.minPageBox.Name = "minPageBox";
			// 
			// maxPageBox
			// 
			resources.ApplyResources(this.maxPageBox, "maxPageBox");
			this.maxPageBox.Name = "maxPageBox";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// rbSelectedPages
			// 
			resources.ApplyResources(this.rbSelectedPages, "rbSelectedPages");
			this.rbSelectedPages.Name = "rbSelectedPages";
			// 
			// rbAllPages
			// 
			this.rbAllPages.Checked = true;
			resources.ApplyResources(this.rbAllPages, "rbAllPages");
			this.rbAllPages.Name = "rbAllPages";
			this.rbAllPages.TabStop = true;
			// 
			// docControl
			// 
			resources.ApplyResources(this.docControl, "docControl");
			this.docControl.AnnotationDraw = false;
			this.docControl.CurDocString = null;
			this.docControl.Cursor = System.Windows.Forms.Cursors.WaitCursor;
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
			this.docControl.SelectionMode = false;
			this.docControl.ShowThumbPanel = true;
			this.docControl.ShowToolBar = true;
			this.docControl.ShowWebPanel = false;
			this.docControl.SplinterPlace = new System.Drawing.Point(200, 50);
			this.docControl.TabStop = false;
			this.docControl.WatchOnFile = false;
			this.docControl.Zoom = 100;
			this.docControl.ZoomText = "";
			// 
			// splitterVert
			// 
			resources.ApplyResources(this.splitterVert, "splitterVert");
			this.splitterVert.Name = "splitterVert";
			this.splitterVert.TabStop = false;
			// 
			// NewImageAddDialog
			// 
			this.AcceptButton = this.buttonSave;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.splitterVert);
			this.Controls.Add(this.panelDocControl);
			this.Controls.Add(this.panelTree);
			this.Controls.Add(this.panelBottom);
			this.Name = "NewImageAddDialog";
			this.Load += new System.EventHandler(this.NewImageAddDialog_Load);
			this.panelBottom.ResumeLayout(false);
			this.panelTree.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.docGrid)).EndInit();
			this.panelDocControl.ResumeLayout(false);
			this.groupBoxPages.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.minPageBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.maxPageBox)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private void NewImageAddDialog_Load(object sender, EventArgs e)
        {
            tree.CreateScanerRoot();

            string path = subLayout.LoadStringOption("SelectedNode", "");
            if(!string.IsNullOrEmpty(path))
            {
                tree.SelectScanerFolder(path, tree.ScanerNode);
            }
        }

        private void tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            docControl.FileName = "";
            buttonAdd.Enabled = buttonSave.Enabled = false;
            var node = e.Node as FolderTree.FolderNodes.Node;
            if (node != null)
            {
                Cursor = Cursors.WaitCursor;
                node.LoadDocs(docGrid, true, 0);
                Cursor = Cursors.Default;
            }
        }

        private void LoadReg()
        {
            Width = subLayout.LoadIntOption("Width", Width);
            Height = subLayout.LoadIntOption("Heigth", Height);
            splitterH.SplitPosition = subLayout.LoadIntOption("SplitterHorizont", splitterH.SplitPosition);
            splitterVert.SplitPosition = subLayout.LoadIntOption("SplitterVertical", splitterVert.SplitPosition);
            if (Convert.ToBoolean( subLayout.LoadStringOption("Maximized", "False")))
			{
                WindowState = FormWindowState.Maximized;
            }

            int spltx = subLayout.LoadIntOption("SplitterX", docControl.SplinterPlace.X);
            int splty = subLayout.LoadIntOption("SplitterY", docControl.SplinterPlace.Y);
            if (spltx < 32)
                spltx = 32;
            if (splty < 32)
                splty = 32;
            docControl.SplinterPlace = new Point(spltx, splty);
            
            docControl.ZoomText = subLayout.LoadStringOption("Zoom", "100%");
        }

        private void NewImageAddDialog_Closed(object sender, EventArgs e)
        {
            if (subLayout == null)
                return;

            subLayout.Option("Width").Value = Width;
            subLayout.Option("Heigth").Value = Height;
            subLayout.Option("Maximized").Value = (WindowState == FormWindowState.Maximized).ToString();
            subLayout.Option("SplitterX").Value = docControl.SplinterPlace.X;
            subLayout.Option("SplitterY").Value = docControl.SplinterPlace.Y;
            subLayout.Option("SplitterHorizont").Value = splitterH.SplitPosition;
            subLayout.Option("SplitterVertical").Value = splitterVert.SplitPosition;
            subLayout.Option("SelectedNode").Value = ((FolderTree.FolderNodes.PathNodes.ScanerNode)tree.SelectedNode).Path;
            subLayout.Option("Zoom").Value = docControl.ZoomText;

            subLayout.Save();

            // Сохраняю стиль сетки списка изображений
            if (docGrid.Style != null)
                docGrid.Style.Save();

            dialog_DialogEvent();
        }

        private void dialog_DialogEvent() //object source, Lib.Win.DialogEventArgs e)
        {
            //Dialogs.NewImageAddDialog dialog = e.Dialog as Dialogs.NewImageAddDialog;
            if (string.IsNullOrEmpty(FileName) || DocID <= 0 || DialogResult == DialogResult.Cancel)
                return;
            try
            {
                int docId = DocID;
                string newFileName = FileName;

                if (IsSavePart)
                {
                    newFileName = Lib.Win.Document.Environment.GenerateFullFileName(Path.GetExtension(FileName).TrimStart('.'));
                    if (IsPdf)
                        Lib.Win.Document.Environment.PDFHelper.SavePart(FileName, newFileName, MinPage, MaxPage);
                    else
                        Lib.Win.Document.Environment.LibTiff.SavePart(FileName, MinPage - 1, PagesCount, newFileName, null);
                }

                switch (DialogResult)
                {
                    case DialogResult.OK:
                        DateTime creationTime = DateTime.Now;
                        ServerInfo server;
                        string fName;

                        if (Lib.Win.Document.Environment.MoveFile(newFileName, ref creationTime, out fName, out server))
                        {
                            int imgID = 0;
                            Environment.DocImageData.DocImageInsert(server.ID, fName, ref imgID, ref docId, 0, "", DateTime.MinValue, "", "", false, creationTime, 0, isImageMain, ImageType, PagesCount);
                        }
                        if (IsSavePart)
                        {
                            if (!IsPdf)
                            {
                                string tempFileName = Lib.Win.Document.Environment.LibTiff.DeletePart(FileName, MinPage - 1, PagesCount, FileName);
                                if (!string.IsNullOrEmpty(tempFileName) && File.Exists(tempFileName))
                                {
                                    File.Copy(tempFileName, FileName, true);
                                    Slave.DeleteFile(tempFileName);
                                }
                            }
                            else
                                Lib.Win.Document.Environment.PDFHelper.DelPart(FileName, MinPage, MaxPage);
                        }
                        break;
                    case DialogResult.Yes:
                        Lib.Win.Document.Select.SelectImageDialog cdialog = new Lib.Win.Document.Select.SelectImageDialog { DocumentID = docId, PagesCount = PagesCount, Tag = newFileName, SrcFileName = FileName, SrcStartPage = MinPage, SrcPagesCount = MaxPage - MinPage + 1 };
                        cdialog.Show();
                        break;
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void docGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            docControl.FileName = "";
            buttonSave.Enabled = false;
            buttonAdd.Enabled = false;

            if (!docGrid.IsScaner())
                return;
			if(timer == null)
			{
				timer = new System.Timers.Timer(200);
				timer.AutoReset = false;
				timer.Elapsed += timer_Elapsed;
				timer.Enabled = true;
			}
			else
				timer.Stop();
			timer.Start();
		}

		void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			timer.Stop();
			if(this.InvokeRequired)
			{
				this.BeginInvoke(new System.Timers.ElapsedEventHandler(timer_Elapsed), sender, e);
				return;
			}
			FileName = (string)docGrid.GetCurValue(Environment.ScanReader.FullNameField);
			if(File.Exists(FileName) && !string.Equals(docControl.FileName, FileName))
			{
				docControl.FileName = FileName;
				buttonSave.Enabled = true;
				buttonAdd.Enabled = true;

				minPageBox.Value = 1;

				maxPageBox.Minimum = 1;

				// Заявка №27393
				// FIX System.ArgumentOutOfRangeException
				// Добавлена проверка значения
				var pageCount = docControl.PageCount;
				if(pageCount < 1)
					pageCount = 1;

				maxPageBox.Maximum = pageCount;
				maxPageBox.Value = pageCount;

				groupBoxPages.Visible = true;
				//rbAllPages.Checked = true;
				rbSelectedPages.Checked = true;
				rbSelectedPages_CheckedChanged(sender, e);
			}
			else
			{
				groupBoxPages.Visible = false;
			}
		}

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FileName))
                End(DialogResult.OK);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FileName))
                End(DialogResult.Yes);
        }

        #region SelectedPages

        private void textBoxStartPage_ValueChanged(object sender, EventArgs e)
        {
            minPageBox.ValueChanged -= StartPage_ValueChanged;
            minPageBox.TextChanged -= StartPage_ValueChanged;

            maxPageBox.Minimum = minPageBox.Value > 0 ? minPageBox.Value : 1;

            if (decimal.Parse(minPageBox.Text) > maxPageBox.Value)
            {
                minPageBox.Value = maxPageBox.Value;
                minPageBox.Text = maxPageBox.Value.ToString();
            }
            minPageBox.ValueChanged += StartPage_ValueChanged;
            minPageBox.TextChanged += StartPage_ValueChanged;
        }

        private void textBoxEndPage_ValueChanged(object sender, EventArgs e)
        {
            maxPageBox.ValueChanged -= EndPage_ValueChanged;
            maxPageBox.TextChanged -= EndPage_ValueChanged;

            minPageBox.Maximum = maxPageBox.Value > 1 ? maxPageBox.Value : 1;

            // Заявка №27393
            // FIX System.FormatException
            // Убран парсинг, для ValueChanged строкое значение уже должно соответствовать Value
            if (maxPageBox.Value < minPageBox.Value)
            {
                maxPageBox.Value = minPageBox.Value;
                maxPageBox.Text = minPageBox.Value.ToString();
            }

            maxPageBox.ValueChanged += EndPage_ValueChanged;
            maxPageBox.TextChanged += EndPage_ValueChanged;
        }

        private void rbSelectedPages_CheckedChanged(object sender, EventArgs e)
        {
            minPageBox.Enabled = rbSelectedPages.Checked;
            maxPageBox.Enabled = rbSelectedPages.Checked;
        }

        #endregion
    }
}