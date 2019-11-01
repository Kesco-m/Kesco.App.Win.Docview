#region

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Kesco.App.Win.DocView.FolderTree.FolderNodes.WorkNodes;
using Kesco.App.Win.DocView.Forms;
using Kesco.App.Win.DocView.Misc;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Data.DALC.Documents.Search;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Document;
using Kesco.Lib.Win.Document.Search;

#endregion

namespace Kesco.App.Win.DocView.FolderTree.FolderNodes
{
    /// <summary>
    ///   Summary description for FoundNode.
    /// </summary>
    public class FoundNode : WorkNode
    {
        protected new const int MaxLabelLength = 300;

        private string toolTipText;

        private static string rootTitle = Environment.StringResources.GetString("FolderTree.FolderNodes.FoundNode.Title1");

        protected MenuItem findItem;
        protected MenuItem findIDItem;

        protected MenuItem separator1;

        protected MenuItem newItem;

        protected MenuItem separator2;

        protected MenuItem editItem;
        protected MenuItem renameItem;
        protected MenuItem deleteItem;

        protected MenuItem separator3;

        protected MenuItem refreshItem;

        protected FoundNode(int id, string name, Employee emp, string xml)
            : base(id, name)
        {
            Emp = emp;
            XML = xml;
            toolTipText = id == rootID ? base.GetToolTip() : Options.GetText(xml);

            Text = name;

            // если папка другого сотрудника, указываем это
            if (emp.ID > 0 && !emp.Equals(Environment.CurEmp) && id == rootID)
            {
                if (emp.ShortName != null)
                {
                    this.name = string.Concat(rootTitle, " (", (Environment.CurCultureInfo.Name.StartsWith("ru") ? emp.ShortName : emp.ShortEngName), ")");
                    Text = this.name;
                }
            }

            if (id == rootID) // загрузить "запросы"
            {
                unselectedCollapsed = Images.Found;
                selectedCollapsed = Images.Found;
                unselectedExpanded = Images.Found;
                selectedExpanded = Images.Found;
            }
            else
            {
                unselectedCollapsed = Images.CatalogDocType;
                selectedCollapsed = Images.CatalogDocType;
                unselectedExpanded = Images.CatalogDocType;
                selectedExpanded = Images.CatalogDocType;
            }

            UpdateImages();

            if (rootID == id)
                LoadSubNodes();

            // пункты меню
            findItem = new MenuItem();
            findIDItem = new MenuItem();

            separator1 = new MenuItem();

            newItem = new MenuItem();

            separator2 = new MenuItem();

            editItem = new MenuItem();
            renameItem = new MenuItem();
            deleteItem = new MenuItem();

            separator3 = new MenuItem();

            refreshItem = new MenuItem();

            // Найти
            findItem.Text = Environment.StringResources.GetString("Find") + "...";
            findItem.Shortcut = Shortcut.F3;
            findItem.Click += findItem_Click;

            // найти по коду
            findIDItem.Text = Environment.StringResources.GetString("FindID") + "...";
            findIDItem.Shortcut = Shortcut.CtrlF3;
            findIDItem.Click += findIDItem_Click;

            // добавить
            newItem.Text = Environment.StringResources.GetString("CreateInquiry");
            newItem.Click += newItem_Click;

            // разделители
            separator1.Text = "-";
            separator2.Text = "-";
            separator3.Text = "-";

            // редактировать
            editItem.Text = Environment.StringResources.GetString(rootID == id ? "ChangeSearch" : "ToChange");
            editItem.Click += editItem_Click;

            // переименовать
            renameItem.Text = Environment.StringResources.GetString("Rename");
            renameItem.Click += renameItem_Click;

            // удалить
            deleteItem.Text = Environment.StringResources.GetString("Delete");
            deleteItem.Click += deleteItem_Click;

            //обновить
            refreshItem.Text = Environment.StringResources.GetString("Refresh");
            refreshItem.Click += (sender, args) => Environment.CmdManager.Commands["RefreshDocs"].Execute();
        }

        public string XML { get; private set; }
        
        public override bool IsFound()
        {
            return true;
        }

        public override void LoadSubNodes()
        {
            Nodes.Clear();

            if (TreeView != null)
                TreeView.Cursor = Cursors.WaitCursor;

#if AdvancedLogging
            using (Lib.Log.Logger.DurationMetter("FoundNode LoadSubNodes() using(DataTable dt = Environment.QueryData.GetSearchParamSets(Emp.ID))"))
#endif
            using (DataTable dt = Environment.QueryData.GetSearchParamSets(Emp.ID))
            using (DataTableReader dr = dt.CreateDataReader())
            {
                while (dr.Read())
                {
                    var nodeID = (int) dr[Environment.QueryData.IDField];
                    if (nodeID == rootID)
                        continue;

                    var nodeName = (string) dr[Environment.QueryData.NameField];
                    var nodeXML = (string) dr[Environment.QueryData.XMLField];
                    var nodeEmpID = (int) dr[Environment.QueryData.EmpIDField];
                    var nodeEmp = new Employee(nodeEmpID, Environment.EmpData);
                    var node = new FoundNode(nodeID, nodeName, nodeEmp, nodeXML);
                    Nodes.Add(node);
                }
                dr.Close();
                dr.Dispose();
                dt.Dispose();
            }

            if (TreeView != null)
                TreeView.Cursor = Cursors.Default;

            base.LoadSubNodes();
        }


        public override void On_MouseUp(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            TreeView.SelectedNode = this;

            var contextMenu = new ContextMenu();

            contextMenu.MenuItems.Add(findItem);
            contextMenu.MenuItems.Add(findIDItem);
            contextMenu.MenuItems.Add(separator1);
            contextMenu.MenuItems.Add(newItem);

            if (Emp.ID == Environment.CurEmp.ID)
                if (ID > 0)
                {
                    contextMenu.MenuItems.Add(separator2);
                    contextMenu.MenuItems.Add(renameItem);
                    contextMenu.MenuItems.Add(editItem);
                    contextMenu.MenuItems.Add(deleteItem);
                }
                else
                {
                    if (Environment.SearchXml.Length > 0)
                    {
                        contextMenu.MenuItems.Add(separator2);
                        contextMenu.MenuItems.Add(editItem);
                    }
                }

            contextMenu.MenuItems.Add(separator3);
            contextMenu.MenuItems.Add(refreshItem);

            if (contextMenu.MenuItems.Count > 0)
                contextMenu.Show(TreeView, new Point(e.X, e.Y));
        }

        #region Context Menu

        protected void findItem_Click(object sender, EventArgs e)
        {
            Environment.CmdManager.Commands["Search"].ExecuteIfEnabled();
        }

        protected void findIDItem_Click(object sender, EventArgs e)
        {
            Environment.CmdManager.Commands["FindID"].ExecuteIfEnabled();
        }

        protected void newItem_Click(object sender, EventArgs e)
        {
            var dialog = new OptionsDialog(0, OptionsDialog.EnabledFeatures.Clear | OptionsDialog.EnabledFeatures.SaveAs);
            dialog.DialogEvent += SearchDialog_DialogEvent;
            dialog.Show();
        }

        protected void editItem_Click(object sender, EventArgs e)
        {
            if (ID == 0)
            {
                var dialog = new XmlSearchForm(Environment.SearchXml, OptionsDialog.EnabledFeatures.All);
                dialog.DialogEvent += SearchDialog_DialogEvent;
                dialog.Show();
            }
            else
            {
                var dialog = new OptionsDialog(ID, OptionsDialog.EnabledFeatures.Clear | OptionsDialog.EnabledFeatures.Save);
                dialog.DialogEvent += SearchDialog_DialogEvent;
                dialog.Show();
            }
        }

        private void SearchDialog_DialogEvent(object source, DialogEventArgs e)
        {
            var dialog = e.Dialog as OptionsDialog;
            if (dialog == null)
                return;

            switch (dialog.DialogResult)
            {
                case DialogResult.Yes:
                    {
                        int id = dialog.GetID();
                        if (id > 0)
                        {
                            var tree = (FolderTree) TreeView;
                            Environment.Refresh();
                            tree.SelectFoundFolder(id, tree.FoundNode, 0);
                        }
                        else
                            Program.MainFormDialog.SearchDialog_DialogEvent(source, e);
                    }
                    break;
                case DialogResult.OK:
                    {
                        int id = dialog.GetID();
                        if (id == 0)
                            Program.MainFormDialog.SearchDialog_DialogEvent(source, e);
                    }
                    break;
            }
        }

        protected void renameItem_Click(object sender, EventArgs e)
        {
            RenameQuery(name);
        }

        private void RenameQuery(string oldName)
        {
            var dialog =
                new EnterStringDialog(Environment.StringResources.GetString("InquiryName"),
                                                       Environment.StringResources.GetString("InquiryNameInput"),
                                                       oldName);
            dialog.DialogEvent += Rename_DialogEvent;
            dialog.Show();
        }

        private void Rename_DialogEvent(object source, DialogEventArgs e)
        {
            var dialog = e.Dialog as EnterStringDialog;
            if (dialog == null) 
                return;

            switch (dialog.DialogResult)
            {
                case DialogResult.OK:
                    {
                        string label = dialog.Input;
                        if (label != name)
                        {
                            if (label.Length > MaxLabelLength)
                                label = label.Remove(MaxLabelLength, label.Length - MaxLabelLength);

                            bool save = false;

                            int oldID = Environment.QueryData.GetIDByName(label, Emp.ID);
                            if (oldID == 0) // запроса с таким именем нет
                                save = true;
                            else // запрос с таким именем есть - что будем делать?
                            {
                                if (
                                    MessageBox.Show(
                                        Environment.StringResources.GetString(
                                            "FolderTree.FolderNodes.FoundNode.Rename_DialogEvent.Message1") +
                                        System.Environment.NewLine +
                                        Environment.StringResources.GetString(
                                            "FolderTree.FolderNodes.FoundNode.Rename_DialogEvent.Message2"),
                                        Environment.StringResources.GetString(
                                            "FolderTree.FolderNodes.FoundNode.Rename_DialogEvent.Title1"),
                                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes)
                                    // затираем старый запрос новым
                                    save = true;
                                else // возвращаемся к вводу имени запроса
                                    RenameQuery(label);
                            }

                            if (save && Environment.QueryData.Rename(ID, label))
                            {
                                name = label;
                                Text = label;

                                if (oldID > 0) // удаляем запрос, который затерли
                                    DeleteQuery(oldID);
                            }
                        }
                    }
                    break;
            }
        }

        protected void deleteItem_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show(
                    Environment.StringResources.GetString("FolderTree.FolderNodes.FoundNode.deleteItem_Click.Message1") +
                    " \"" + name + "\"?", Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                DeleteQuery(ID);
        }

        private void DeleteQuery(int idToDel)
        {
            if (!Environment.QueryData.Delete(idToDel))
                return;

            if (idToDel == ID)
                Remove();
            else
            {
                // поиск нужного узла и удаление его
                var tree = TreeView as FolderTree;
                if (tree == null)
                    return;
                FoundNode nodeToDel = tree.FindFoundNode(idToDel, tree.FoundNode);
                if (nodeToDel != null)
                    nodeToDel.Remove();
            }
        }

        #endregion

        public static FoundNode CreateRoot(Employee employee)
        {
            return new FoundNode(rootID, rootTitle, employee, null);
        }

		public override void LoadDocs(Grids.DocGrid docGrid, bool clean, int curID, string filename = null)
		{
			if(ID == rootID)
				docGrid.LoadFoundDocs(Emp, clean, curID);
			else
				docGrid.LoadQueryDocs(XML, Emp, clean, curID);
		}

        public override Context BuildContext()
        {
            return new Context(ContextMode.Found, ID, Emp);
        }

        public override string GetToolTip()
        {
            return toolTipText;
        }
    }
}