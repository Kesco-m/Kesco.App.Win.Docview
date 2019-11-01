using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Grids.Styles;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Options;

namespace Kesco.App.Win.DocView.Settings
{
    /// <summary>
    /// Диалог настройки столбцов списка документов
    /// </summary>
    public class SettingsColumnsDialog : FreeDialog
    {
        private readonly Dictionary<ColumnsListView, ColParams> ColumnsLists = new Dictionary<ColumnsListView, ColParams>();
        private readonly Folder subLayout;
		private readonly Folder optionFolder;
        private Button buttonCancel;
        private Button buttonDown;
        private Button buttonOK;
        private Button buttonReset;
        private Button buttonResetSort;
        private Button buttonUp;
        private IContainer components;
        private Style finalStyle;
        private ColumnsListView list;
        private Panel panelButtons;
        private Panel panelColumns;
        private TabControl tabControl;
        private Style m_style;

        private Container component;

        public SettingsColumnsDialog(Style style, Folder optionFolder, Style finalStyle)
        {
            try
            {
                this.finalStyle = finalStyle;
                InitializeComponent();
                ComponentResourceManager resources = new ComponentResourceManager(typeof(Forms.MainFormDialog));
                this.Text = resources.GetObject("menuItem8.Text").ToString() + "->" + resources.GetObject("mIColumns.Text").ToString().TrimEnd('.');
				this.optionFolder = optionFolder;
                FolderCollection savedStyles = optionFolder.GetSavedFolders();
                m_style = style;

                if (/*style == null &&*/ savedStyles.Count > 0)
                {
                    panelColumns.Controls.Remove(list);
                    list.Visible = false;

                    int countAvailableStyles = 0;

                    foreach (Folder folder in savedStyles)
                    {
                        string name = folder.Name;
                        ColParams colsNew = new ColParams(folder, false);
                        if (colsNew.WasInited && colsNew.NotEmpty(true))
                        {
                            ColumnsListView listNew = (ColumnsListView) list.Clone();
                            ColumnsLists.Add(listNew, colsNew);

                            countAvailableStyles++;
                        }
                    }

                    if (countAvailableStyles == 1)
                    {
                        ColumnsLists.Clear();
                        TheOnlyStyleToEdit(finalStyle);
                    }
                    else
                    {
                        subLayout = Environment.Layout.Folders.Add("SettingsColumns");
                        Closed += SettingsColumnsDialog_Closed;
                    }
                }
                //else
                //    TheOnlyStyleToEdit(style);
            }
            catch (Exception ex)
            {
                Environment.CmdManager.Commands["ResetColumns"].Execute();
            }
        }

        private ColumnsListView CurrentList
        {
            get
            {
                if (ColumnsLists.Count > 1)
                    return tabControl.SelectedTab.Controls[0] as ColumnsListView;
                return list;
            }
        }

        private void TheOnlyStyleToEdit(Style style)
        {
            if (style != null)
                finalStyle = style;

            ColumnsLists.Add(list, new ColParams(finalStyle.OptionFolder, false));
            //tabControl.Visible = false;
        }

        private void LoadReg()
        {
            if (!tabControl.Visible || tabControl.TabCount == 0 || tabControl.SelectedTab == null || subLayout == null)
                return;

            string selectedTabName = subLayout.LoadStringOption("SelectedTab", tabControl.SelectedTab.Text);
            foreach (TabPage tp in tabControl.TabPages.Cast<TabPage>().Where(tp => tp.Text.Equals(selectedTabName)))
            {
                tabControl.SelectedTab = tp;
                break;
            }
        }

		private void SettingsColumnsDialog_Closed(object sender, EventArgs e)
		{
			if(subLayout != null && tabControl.TabPages.Count > 0)
			{
				subLayout.Option("SelectedTab").Value = tabControl.SelectedTab.Text;
				subLayout.Save();
			}
		}

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void SettingsColumnsDialog_Load(object sender, EventArgs e)
        {
            foreach (ColumnsListView colList in ColumnsLists.Keys)
            {
                ColParams colParams = ColumnsLists[colList];
                //if (ColumnsLists.Count > 1)
                //{
                TabPage page = new TabPage(colParams.FriendlyName) { Name = colParams.FriendlyName };
                    page.Controls.Add(colList);
                    colList.Visible = true;
                    tabControl.TabPages.Add(page);
                //}

                colList.SuspendLayout();
                foreach (ColParam col in colParams.Values.Where(col => col != null && !col.IsSystemField))
                {
                    colList.Rows.Add();
                    int i = colList.Rows.Count - 1;
                    colList[colList.Columns["Visibility"].Index, i].Value = col.Visible;
                    colList[colList.Columns["SortIndex"].Index, i].Value = string.Empty;
                    colList[colList.Columns["SortDirection"].Index, i].Value = SortOrder.None;
                    colList[colList.Columns["FriendlyName"].Index, i].Value = col.HeaderName;
                    colList[colList.Columns["Name"].Index, i].Value = col.Name;
                    colList[colList.Columns["IsKeyField"].Index, i].Value = col.IsKeyField;
                    colList[colList.Columns["OrderIndex"].Index, i].Value = col.Index;
                }
                string[] _s = colParams.SortOrder.ToLower().Replace("[", "").Replace("]", "").Split(new[] {',', ' '}, StringSplitOptions. RemoveEmptyEntries);

                int sortIndex = 1;
                for (int i = 0; i < _s.Length && sortIndex < 6; i++)
                {
                    for (int row = 0; row < colList.Rows.Count && sortIndex < 6; row++)
                    {
                        if (
                            colList[colList.Columns["Name"].Index, row].Value.ToString().ToLower().Equals(_s[i].ToLower()))
                        {
                            i++;
                            colList[colList.Columns["SortIndex"].Index, row].Value = sortIndex.ToString();
                            colList[colList.Columns["SortDirection"].Index, row].Value = _s[i].ToLower().Equals("asc") ? SortOrder.Ascending : SortOrder.Descending;
                            sortIndex++;
                        }
                    }
                }

                colList.SelectionChanged += UpdateButtons;
                colList.ResumeLayout();
                UpdateButtons(colList, null);
            }
            LoadReg();

			if(m_style != null && tabControl.TabPages.Count > 0 && tabControl.TabPages.ContainsKey(m_style.FriendlyName))
                tabControl.SelectTab(tabControl.TabPages[m_style.FriendlyName]);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ColumnsListView colList in ColumnsLists.Keys)
                {
                    ColParams colParams = ColumnsLists[colList];
                    string[] sortOrder = new string[5];
                    colParams.SortOrder = string.Empty;

                    for (int i = 0; i < colList.Rows.Count; i++)
                    {
                        string colName = colList[colList.Columns["Name"].Index, i].Value.ToString();
                        colParams.GetParam(colName).Visible = (bool)colList[colList.Columns["Visibility"].Index, i].Value;
                        colParams.GetParam(colName).HeaderName = colList[colList.Columns["FriendlyName"].Index, i].Value.ToString();
                        colParams.GetParam(colName).IsKeyField = (bool)colList[colList.Columns["IsKeyField"].Index, i].Value;
                        colParams.GetParam(colName).Index = int.Parse(colList[colList.Columns["OrderIndex"].Index, i].Value.ToString());

                        if (!string.IsNullOrEmpty(colList[colList.Columns["SortIndex"].Index, i].Value.ToString()))
                        {
                            int _sortIndex = int.Parse(colList[colList.Columns["SortIndex"].Index, i].Value.ToString());
                            SortOrder _sortOrder = (SortOrder)colList[colList.Columns["SortDirection"].Index, i].Value;
                            sortOrder[_sortIndex - 1] = colName + (_sortOrder == SortOrder.Ascending ? " asc" : " desc");
                        }
                    }

                    StringBuilder sb = new StringBuilder();
                    foreach (string t in sortOrder.Where(t => !string.IsNullOrEmpty(t)))
                    {
                        sb.Append(t);
                        sb.Append(",");
                    }
                    colParams.SortOrder = sb.ToString().TrimEnd(new[] { ',', ' ' }).TrimStart(new[] { ',', ' ' });
                    colParams.Save();
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
            finalStyle.ReloadData();

            End(DialogResult.OK);
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            ColumnsListView colList = CurrentList;
            if (colList == null)
                return;

            ColParams colParams = ColumnsLists[colList];
            if (colParams == null)
                return;

            if (colList.SelectedRows.Count == 1)
            {
                colList.SuspendLayout();
                int row = colList.SelectedRows[0].Index;
                if (row > 0)
                {
                    int col = colList.Columns["OrderIndex"].Index;
                    object a = colList[col, row].Value;
                    object b = colList[col, row - 1].Value;

                    colList[col, row].Value = b;
                    colList[col, row - 1].Value = a;
                }
                colList.Sort(colList.Columns["OrderIndex"], ListSortDirection.Ascending);
                colList.ResumeLayout();
            }
            UpdateButtons(colList, e);
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            ColumnsListView colList = CurrentList;
            if (colList == null)
                return;

            ColParams colParams = ColumnsLists[colList];
            if (colParams == null)
                return;

            if (colList.SelectedRows.Count == 1)
            {
                colList.SuspendLayout();
                int row = colList.SelectedRows[0].Index;
                if (row < colList.Rows.Count - 1)
                {
                    int col = colList.Columns["OrderIndex"].Index;
                    object a = colList[col, row].Value;
                    object b = colList[col, row + 1].Value;

                    colList[col, row].Value = b;
                    colList[col, row + 1].Value = a;
                }
                colList.Sort(colList.Columns["OrderIndex"], ListSortDirection.Ascending);
                colList.ResumeLayout();
            }
            UpdateButtons(colList, e);
        }

        private void UpdateButtons(object sender, EventArgs e)
        {
            bool up = false;
            bool down = false;

            ColumnsListView colList = sender as ColumnsListView;
            if (colList == null)
                return;

            colList.SuspendLayout();
            if (colList.SelectedRows.Count == 1)
            {
                DataGridViewRow item = colList.SelectedRows[0];
                if (item.Index > 0)
                    up = true;
                if (item.Index < colList.Rows.Count - 1)
                    down = true;
            }
            colList.ResumeLayout();

            if (buttonUp.Enabled != up)
                buttonUp.Enabled = up;

            if (buttonDown.Enabled != down)
                buttonDown.Enabled = down;
        }

        private void buttonReset_Click(object sender, EventArgs e)
		{
			if(finalStyle.FriendlyName == tabControl.SelectedTab.Name)
			{
				finalStyle.OptionFolder.Options.Clear();
				optionFolder.Delete(finalStyle.OptionFolder.Name);
				finalStyle.ReloadData();
			}
			else
			{
				Style.ResetStyles();
				optionFolder.Delete(ColumnsLists[CurrentList].optionFolder.Name);
			}
			End(DialogResult.Cancel);
        }

        private void buttonResetSort_Click(object sender, EventArgs e)
        {
            ColumnsListView colList = CurrentList;
            if (colList == null)
                return;
            colList.ResetSortOrder();
        }

        #region Columns

        #region Nested type: Column

        public class Column
        {
            public Column(string name, int width, CheckBox check, ColumnList list)
            {
                Name = name;
                Width = width;

                Check = check;
                List = list;
            }

            #region Accessors

            public string Name { get; private set; }

            public int Width { get; private set; }

            public CheckBox Check { get; private set; }

            public ColumnList List { get; private set; }

            #endregion
        }

        #endregion

        #region Nested type: ColumnList

        public class ColumnList
        {
            private readonly Panel panel;
            private readonly int xOrigin;
            private readonly int yOffset;
            private readonly int yOrigin;

            public ColumnList(Panel panel)
            {
                this.panel = panel;

                List = new ArrayList();

                xOrigin = 10;
                yOrigin = 0;
                yOffset = 24;

                Count = 0;
            }

            #region Accessors

            public ArrayList List { get; private set; }

            public int Count { get; set; }

            #endregion

            public void Add(string name, int width, bool locked)
            {
                int realCount = List.Count;
                int scrollY = panel.AutoScrollPosition.Y;

                CheckBox check = new CheckBox
                                     {
                                         Checked = (width > 0),
                                         Location = new Point(xOrigin, yOrigin + 8 + realCount*yOffset + scrollY),
                                         Name = "checkBox" + Count,
                                         Enabled = !locked,
                                         Size = new Size(panel.Width - 100, 16),
                                         Text = name,
                                         TabIndex = Count
                                     };



                panel.Controls.Add(check);

                List.Add(new Column(name, width, check, this));

                Count++;
            }

            public void RemoveAt(int index)
            {
                Column column = (Column) List[index];
                List.RemoveAt(index);

                panel.Controls.Remove(column.Check);
            }

            public int FindByName(string name)
            {
                for (int i = 0; i < List.Count; i++)
                {
                    Column column = (Column) List[i];
                    if (column.Name == name)
                        return i;
                }

                return -1;
            }
        }

        #endregion

        #endregion

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsColumnsDialog));
			this.panelColumns = new System.Windows.Forms.Panel();
			this.list = new Kesco.App.Win.DocView.Settings.SettingsColumnsDialog.ColumnsListView();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.buttonUp = new System.Windows.Forms.Button();
			this.buttonDown = new System.Windows.Forms.Button();
			this.buttonReset = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.panelButtons = new System.Windows.Forms.Panel();
			this.buttonResetSort = new System.Windows.Forms.Button();
			this.panelColumns.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.list)).BeginInit();
			this.panelButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelColumns
			// 
			resources.ApplyResources(this.panelColumns, "panelColumns");
			this.panelColumns.Controls.Add(this.list);
			this.panelColumns.Controls.Add(this.tabControl);
			this.panelColumns.Controls.Add(this.buttonUp);
			this.panelColumns.Controls.Add(this.buttonDown);
			this.panelColumns.Name = "panelColumns";
			// 
			// list
			// 
			this.list.AllowUserToAddRows = false;
			this.list.AllowUserToDeleteRows = false;
			this.list.AllowUserToResizeColumns = false;
			this.list.AllowUserToResizeRows = false;
			resources.ApplyResources(this.list, "list");
			this.list.BackgroundColor = System.Drawing.SystemColors.Window;
			this.list.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.list.ColumnHeadersVisible = false;
			this.list.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.list.MultiSelect = false;
			this.list.Name = "list";
			this.list.RowHeadersVisible = false;
			this.list.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.list.ShowCellToolTips = false;
			// 
			// tabControl
			// 
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			// 
			// buttonUp
			// 
			resources.ApplyResources(this.buttonUp, "buttonUp");
			this.buttonUp.Name = "buttonUp";
			this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
			// 
			// buttonDown
			// 
			resources.ApplyResources(this.buttonDown, "buttonDown");
			this.buttonDown.Name = "buttonDown";
			this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
			// 
			// buttonReset
			// 
			resources.ApplyResources(this.buttonReset, "buttonReset");
			this.buttonReset.Name = "buttonReset";
			this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// panelButtons
			// 
			this.panelButtons.Controls.Add(this.buttonResetSort);
			this.panelButtons.Controls.Add(this.buttonReset);
			this.panelButtons.Controls.Add(this.buttonCancel);
			this.panelButtons.Controls.Add(this.buttonOK);
			resources.ApplyResources(this.panelButtons, "panelButtons");
			this.panelButtons.Name = "panelButtons";
			// 
			// buttonResetSort
			// 
			resources.ApplyResources(this.buttonResetSort, "buttonResetSort");
			this.buttonResetSort.Name = "buttonResetSort";
			this.buttonResetSort.Click += new System.EventHandler(this.buttonResetSort_Click);
			// 
			// SettingsColumnsDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.panelColumns);
			this.Controls.Add(this.panelButtons);
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsColumnsDialog";
			this.Load += new System.EventHandler(this.SettingsColumnsDialog_Load);
			this.panelColumns.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.list)).EndInit();
			this.panelButtons.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        #region Nested type: ColumnsListView

        public sealed class ColumnsListView : DataGridView, ICloneable
        {
            public ColumnsListView()
            {
                ShowCellToolTips = false;
                DoubleBuffered = true;
                AutoGenerateColumns = false;
                MultiSelect = false;
                SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                EditMode = DataGridViewEditMode.EditProgrammatically;
                AutoSize = false;
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                AllowUserToResizeRows = false;
                AllowUserToAddRows = false;
                AllowUserToDeleteRows = false;
                Dock = DockStyle.None;
                ColumnHeadersVisible = false;
                RowHeadersVisible = false;
                AllowUserToResizeColumns = false;
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

                Init();
            }

            private ColumnsListView(ColumnsListView list)
            {
                ShowCellToolTips = list.ShowCellToolTips;
                DoubleBuffered = list.DoubleBuffered;
                AutoGenerateColumns = list.AutoGenerateColumns;
                MultiSelect = list.MultiSelect;
                SelectionMode = list.SelectionMode;
                EditMode = list.EditMode;
                AutoSize = list.AutoSize;
                AutoSizeColumnsMode = list.AutoSizeColumnsMode;
                AutoSizeRowsMode = list.AutoSizeRowsMode;
                AllowUserToResizeRows = list.AllowUserToResizeRows;
                AllowUserToAddRows = list.AllowUserToAddRows;
                AllowUserToDeleteRows = list.AllowUserToDeleteRows;
                Dock = list.Dock;
                ColumnHeadersVisible = list.ColumnHeadersVisible;
                RowHeadersVisible = list.RowHeadersVisible;
                AllowUserToResizeColumns = list.AllowUserToResizeColumns;
                GridColor = list.GridColor;
                Anchor = list.Anchor;

                Init();
            }

            #region ICloneable Members

            public object Clone()
            {
                return new ColumnsListView(this);
            }

            #endregion

            private void ColumnsListView_CellClick(object sender, DataGridViewCellEventArgs e)
            {
                if (e == null)
                    return;

                int visibilityIndex = Columns["Visibility"].Index;
                int sortIndex = Columns["SortIndex"].Index;
                int sortDirectionIndex = Columns["SortDirection"].Index;

                if (e.ColumnIndex == visibilityIndex)
                {
                    int countUncheckedItems = 0;
                    this[e.ColumnIndex, e.RowIndex].Value =
                        (bool) this[Columns["IsKeyField"].Index, e.RowIndex].Value
                            ? true
                            : !(bool) this[e.ColumnIndex, e.RowIndex].Value;

                    for (int i = 0; i < Rows.Count; i++)
                        if (!(bool) this[e.ColumnIndex, i].Value)
                            countUncheckedItems++;

                    if (countUncheckedItems == Rows.Count)
                    {
                        this[e.ColumnIndex, e.RowIndex].Value = true;
                        countUncheckedItems--;
                    }

                    int friendlyNameIndex = Columns["FriendlyName"].Index;
                    if (countUncheckedItems == Rows.Count - 1)
                    {
                        for (int i = 0; i < Rows.Count; i++)
                            if ((bool) this[e.ColumnIndex, i].Value)
                                this[friendlyNameIndex, i].Style.Font = new Font(Font, FontStyle.Bold);
                    }
                    else
                    {
                        for (int i = 0; i < Rows.Count; i++)
                            this[friendlyNameIndex, i].Style.Font = new Font(Font, FontStyle.Regular);
                    }

                    if (!(bool) this[e.ColumnIndex, e.RowIndex].Value &&
                        !string.IsNullOrEmpty(this[sortIndex, e.RowIndex].Value.ToString()))
                    {
                        int curSortIndex = int.Parse(this[sortIndex, e.RowIndex].Value.ToString());
                        this[sortIndex, e.RowIndex].Value = string.Empty;
                        this[sortDirectionIndex, e.RowIndex].Value = SortOrder.None;

                        for (int i = 0; i < Rows.Count; i++)
                            if (!string.IsNullOrEmpty(this[sortIndex, i].Value.ToString()) &&
                                curSortIndex < int.Parse(this[sortIndex, i].Value.ToString()))
                                this[sortIndex, i].Value = int.Parse(this[sortIndex, i].Value.ToString()) - 1;
                    }
                }

                if (e.ColumnIndex == sortIndex && string.IsNullOrEmpty(this[e.ColumnIndex, e.RowIndex].Value.ToString()) &&
                    (bool) this[visibilityIndex, e.RowIndex].Value)
                {
                    int max = 0;
                    for (int i = 0; i < Rows.Count; i++)
                        if (!string.IsNullOrEmpty(this[e.ColumnIndex, i].Value.ToString()))
                        {
                            int _t = int.Parse(this[e.ColumnIndex, i].Value.ToString());
                            if (_t > max)
                                max = _t;
                        }

                    if (max == 5)
                    {
                        ResetSortOrder();
                        this[e.ColumnIndex, e.RowIndex].Value = "1";
                        this[sortDirectionIndex, e.RowIndex].Value = SortOrder.Ascending;
                    }
                    else
                    {
                        this[e.ColumnIndex, e.RowIndex].Value = (max + 1).ToString();
                        this[sortDirectionIndex, e.RowIndex].Value = SortOrder.Ascending;
                    }
                }

                if (e.ColumnIndex == sortDirectionIndex &&
                    (SortOrder) this[e.ColumnIndex, e.RowIndex].Value != SortOrder.None)
                    this[e.ColumnIndex, e.RowIndex].Value = (SortOrder) this[e.ColumnIndex, e.RowIndex].Value ==
                                                            SortOrder.Ascending
                                                                ? SortOrder.Descending
                                                                : SortOrder.Ascending;
            }

            public void ResetSortOrder()
            {
                int sortIndex = Columns["SortIndex"].Index;
                int sortDirectionIndex = Columns["SortDirection"].Index;
                for (int i = 0; i < Rows.Count; i++)
                {
                    this[sortIndex, i].Value = string.Empty;
                    this[sortDirectionIndex, i].Value = SortOrder.None;
                }
            }

			public void Reset()
			{
				int sortIndex = Columns["SortIndex"].Index;
				int sortDirectionIndex = Columns["SortDirection"].Index;
				for(int i = 0; i < Rows.Count; i++)
				{
					this[sortIndex, i].Value = string.Empty;
					this[sortDirectionIndex, i].Value = SortOrder.None;
				}
			}

            private void Init()
            {
                RowTemplate.Height = Font.Height + 5;
                Columns.Clear();
                Columns.Add(new DataGridViewCheckBoxColumn());
                Columns[0].DefaultCellStyle.NullValue = false;
                Columns[0].CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Columns[0].Name = "Visibility";
                Columns[0].Width = (int) (Font.Size*3);
                Columns.Add("SortIndex", string.Empty);
                Columns[1].Width = (int) (Font.Size*3);
                Columns[1].CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Columns.Add("SortDirection", string.Empty);
                Columns[2].Width = (int) Font.Size;
                Columns[2].CellTemplate = new SortOrderCell();
                Columns.Add("FriendlyName", string.Empty);
                Columns[3].Width = 140;
                Columns[3].CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Columns.Add("Name", string.Empty);
                Columns[4].Visible = false;
                Columns.Add("IsKeyField", string.Empty);
                Columns[5].Visible = false;
                Columns.Add("OrderIndex", string.Empty);
                Columns[6].Visible = false;

                CellBorderStyle = DataGridViewCellBorderStyle.None;
                ShowCellToolTips = false;
                BackgroundColor = DefaultCellStyle.BackColor;

                Sort(Columns["OrderIndex"], ListSortDirection.Ascending);
                CellClick += ColumnsListView_CellClick;
            }
        }

        #endregion

        #region Nested type: SortOrderCell

        public sealed class SortOrderCell : DataGridViewTextBoxCell
        {
            protected override void Paint(Graphics graphics, Rectangle clipBounds,
                                          Rectangle cellBounds, int rowIndex,
                                          DataGridViewElementStates dataGridViewElementState, object value,
                                          object formattedValue, string errorText, DataGridViewCellStyle cellStyle,
                                          DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                          DataGridViewPaintParts paintParts)
            {
                graphics.FillRectangle(
                    new SolidBrush(dataGridViewElementState == DataGridViewElementStates.Displayed | Selected
                                       ? cellStyle.SelectionBackColor
                                       : cellStyle.BackColor), cellBounds);

                if ((SortOrder) Value != SortOrder.None)
                {
                    Point[] points = new Point[3];

                    int x = cellBounds.Width/8;
                    int y = cellBounds.Height/8;
                    if ((SortOrder) Value == SortOrder.Ascending)
                    {
                        points[0].X = cellBounds.X + (x*4);
                        points[0].Y = cellBounds.Y + y;
                        points[1].X = cellBounds.X + (x*7);
                        points[1].Y = cellBounds.Y + (y*7);
                        points[2].X = cellBounds.X + x;
                        points[2].Y = cellBounds.Y + (y*7);
                    }
                    else
                    {
                        points[0].X = cellBounds.X + x;
                        points[0].Y = cellBounds.Y + y;
                        points[1].X = cellBounds.X + (x*7);
                        points[1].Y = cellBounds.Y + y;
                        points[2].X = cellBounds.X + (x*4);
                        points[2].Y = cellBounds.Y + (y*7);
                    }

                    graphics.FillPolygon(
                        new SolidBrush(dataGridViewElementState == DataGridViewElementStates.Displayed | Selected
                                           ? cellStyle.SelectionForeColor
                                           : cellStyle.ForeColor), points);
                }
            }
        }

        #endregion
    }
}