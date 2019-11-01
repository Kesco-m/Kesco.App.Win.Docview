using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Document.Extentions;

namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.WorkNodes
{
	/// <summary>
	/// Папка ВРаботе
	/// </summary>
	public class WorkFolderNode : WorkNode
	{
		public event EventHandler UpdateNode;

		private void OnUpdateNode()
		{
			if(UpdateNode != null)
				UpdateNode(this, EventArgs.Empty);
		}


		private static string rootTitle = Environment.StringResources.GetString("InWork");

		protected MenuItem newItem;
		protected MenuItem renameItem;
		protected MenuItem deleteItem;
		protected MenuItem separator;
		protected MenuItem shareItem;
		protected MenuItem filterItem;

		private WorkFolderNode(int id, string name, Employee emp, EventHandler LoadComlete = null) : base(id, name)
		{
			Emp = emp ?? Environment.CurEmp;
			CancelOperation();
			// если папка другого сотрудника, указываем это
			if(emp != null && (emp.ID > 0 && !emp.Equals(Environment.CurEmp) && id == rootID && emp.ShortName != null))
				this.name = string.Concat(rootTitle, " (", (Environment.CurCultureInfo.Name.StartsWith("ru") ? emp.ShortName : emp.ShortEngName), ")");

			if(id == rootID)
			{
				this.UpdateNode = LoadComlete;
				unselectedCollapsed = Images.WorkFolder;
				selectedCollapsed = Images.WorkFolder;
				unselectedExpanded = Images.WorkFolder;
				selectedExpanded = Images.WorkFolder;
			}
			else
			{
				unselectedCollapsed = Images.SystemFolder;
				selectedCollapsed = Images.SystemFolder;
				unselectedExpanded = Images.OpenSystemFolder;
				selectedExpanded = Images.OpenSystemFolder;
			}

			UpdateImages();

			if(id == rootID)
			{
				CancelOperation();
				LoadSubNodes();
			}

			// пункты меню
			newItem = new MenuItem();
			renameItem = new MenuItem();
			deleteItem = new MenuItem();

			shareItem = new MenuItem();

			separator = new MenuItem();

			filterItem = new MenuItem();

			// добавить
			newItem.Text = Environment.StringResources.GetString("CreateFolder");
			newItem.Click += newItem_Click;

			// переименовать
			renameItem.Text = Environment.StringResources.GetString("Rename");
			renameItem.Click += renameItem_Click;

			// удалить
			deleteItem.Text = Environment.StringResources.GetString("Delete");
			deleteItem.Click += deleteItem_Click;

			// разделитель
			separator.Text = "-";

			// разрешить/запретить общий доступ к папке
			shareItem.Text = Environment.StringResources.GetString("SharedAccess");
			shareItem.Click += shareItem_Click;

			// создать правило фильтрации
			filterItem.Text = Environment.StringResources.GetString("RulesSort") + "...";
			filterItem.Click += filterItem_Click;
		}

		private WorkFolderNode Populate(DataRow dr, DataRelation rel)
		{
			WorkFolderNode node = new WorkFolderNode(
				(int)dr[Environment.FolderData.IDField], // id
				(string)dr[Environment.FolderData.NameField], // text
				Emp);

			foreach(DataRow row in dr.GetChildRows(rel))
			{
				WorkFolderNode newNode = Populate(row, rel);
				node.Nodes.Add(newNode);
			}

			return node;
		}

		/// <summary>
		///   Папки документов ВРаботе содержат папку с укзанным id
		/// </summary>
		/// <param name="id"> Код папки </param>
		/// <param name="empID"> Код сотрудника </param>
		public bool ContainsByID(int id, int empID)
		{
			if(id == 0) //корень
				return true;
			bool result = false;

			foreach(WorkFolderNode wfnode in Nodes)
			{
				if((wfnode.ID == id && wfnode.Emp.ID == empID) || wfnode.ContainsByID(id, empID))
					result = true;
			}
			return result;
		}

		/// <summary>
		///   Получает папку ВРаботе укзанным id
		/// </summary>
		/// <param name="id"> Код папки </param>
		/// <param name="empID"> Код сотрудника </param>
		public WorkFolderNode GetByID(int id, int empID)
		{
			WorkFolderNode result = null;
			for(int i = 0; i < Nodes.Count && result == null; i++)
			{
				WorkFolderNode wfnode = Nodes[i] as WorkFolderNode;
				if(wfnode != null)
				{
					if(wfnode.ID == id && wfnode.Emp.ID == empID)
						result = wfnode;
					else
						result = wfnode.GetByID(id, empID);
				}
			}
			return result;
		}

		/// <summary>
		///   Удаляет из папок ВРаботе папку с указанным кодом
		/// </summary>
		/// <param name="id"> Код папки </param>
		/// <param name="empID"> Код сотрудника </param>
		public void RemoveByID(int id, int empID)
		{
			if(id == 0)
				return;
			foreach(WorkFolderNode wfnode in Nodes)
			{
				if(wfnode.ID == id && wfnode.Emp.ID == empID)
					wfnode.Remove();
				else
					wfnode.RemoveByID(id, empID);
			}
		}

		public override void On_MouseUp(MouseEventArgs e)
		{
			switch(e.Button)
			{
				case MouseButtons.Right:
					TreeView.SelectedNode = this;
					newItem.Enabled = Emp.ID == Environment.CurEmp.ID;
					if(ID == 0 || Emp.ID != Environment.CurEmp.ID)
					{
						renameItem.Enabled = false;
						deleteItem.Enabled = false;
						shareItem.Enabled = false;
						filterItem.Enabled = false;
					}

					var contextMenu = new ContextMenu(new[] { newItem, renameItem, deleteItem, separator, filterItem });

					contextMenu.Show(TreeView, new Point(e.X, e.Y));
					break;
			}
		}

		#region Context Menu

		protected void newItem_Click(object sender, EventArgs e)
		{
			Kesco.Lib.Win.Document.EnterStringDialog dialog = new Kesco.Lib.Win.Document.EnterStringDialog(Environment.StringResources.GetString("FolderName"),
					Environment.StringResources.GetString("FolderNameInput"), Environment.StringResources.GetString("NewFolder"));
			dialog.DialogEvent += NewItemDialog_DialogEvent;
			dialog.Show();
		}

		private void NewItemDialog_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
		{
			Kesco.Lib.Win.Document.EnterStringDialog dialog = e.Dialog as Kesco.Lib.Win.Document.EnterStringDialog;
			if(dialog == null || dialog.DialogResult != DialogResult.OK)
				return;

			string folderName = dialog.Input;
			int result = Environment.FolderData.New(ID, folderName);
			if(result != 0)
			{
				var newNode = new WorkFolderNode(result, folderName, Emp);

				Nodes.Add(newNode);
				newNode.RemoveBold();
				newNode.EnsureVisible();
				TreeView.SelectedNode = newNode;
			}
		}

		protected void renameItem_Click(object sender, EventArgs e)
		{
			Kesco.Lib.Win.Document.EnterStringDialog dialog = new Kesco.Lib.Win.Document.EnterStringDialog(Environment.StringResources.GetString("FolderName"),
													   Environment.StringResources.GetString("FolderNameInput"), name);
			dialog.DialogEvent += RenameItemDialog_DialogEvent;
			dialog.Show();
		}

		private void RenameItemDialog_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
		{
			Kesco.Lib.Win.Document.EnterStringDialog dialog = e.Dialog as Kesco.Lib.Win.Document.EnterStringDialog;
			if(dialog == null || dialog.DialogResult != DialogResult.OK)
				return;

			string label = dialog.Input;
			if(label != name)
			{
				if(label.Length > MaxLabelLength)
					label = label.Remove(MaxLabelLength, label.Length - MaxLabelLength);

				bool result = Environment.FolderData.Rename(ID, label);

				if(result)
				{
					name = label;
					UpdateStatusBegin(false);
				}
			}
		}

		protected void deleteItem_Click(object sender, EventArgs e)
		{
			if(
				MessageBox.Show(
					Environment.StringResources.GetString(
						"FolderTree.FolderNodes.WorkFolderNode.deleteItem_Click.Message1") + System.Environment.NewLine +
					System.Environment.NewLine +
					Environment.StringResources.GetString(
						"FolderTree.FolderNodes.WorkFolderNode.deleteItem_Click.Message2"),
					Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				if(DeleteAll())
					Remove();
		}

		protected void shareItem_Click(object sender, EventArgs e)
		{
			Dialogs.ShareWorkFolder dialog = new Dialogs.ShareWorkFolder(ID, name);
			dialog.DialogEvent += ShareWorkFolder_DialogEvent;
			dialog.Show();
		}

		private void ShareWorkFolder_DialogEvent(object source, Lib.Win.DialogEventArgs e)
		{
			UpdateImages();
		}

		protected void filterItem_Click(object sender, EventArgs e)
		{
			PropertiesDialogs.PropertiesFolderRuleViewDialog dialog = new PropertiesDialogs.PropertiesFolderRuleViewDialog(ID, name);
			dialog.Show();
		}

		#endregion

        #region Update

        /// <summary>
        /// Обновляет статус папки ВРаботе. Асинхронно.
        /// </summary>
        /// <param name="recursive"></param>
		public void UpdateStatusBegin(bool recursive, EventHandler LoadComlete = null)
	    {
            if (TreeView == null)
                return;

            var updateParams = new WorkFolderUpdateParams
            {
                Recursive = recursive,
                Font = TreeView.Font
            };

            Task.Factory.StartNew(UpdateAsync, updateParams);

            if (recursive)
                for (int i = 0; i < Nodes.Count; i++)
                {
                    var newNode = Nodes[i] as WorkFolderNode;

                    if (newNode != null)
                        newNode.UpdateStatusBegin(true);
                }
			if(LoadComlete != null)
				LoadComlete(this, EventArgs.Empty);
	    }

        /// <summary>
        /// Получить данные. Асинхронно.
        /// </summary>
        /// <param name="param"></param>
        private void UpdateAsync(object param)
        {
            var updateParams = (WorkFolderUpdateParams)param;

            try
            {
                DataRow dr = Environment.WorkFolderData(ID, Emp.ID);

                if (dr == null)
                    return;

                var recursive = updateParams.Recursive;
                var font = updateParams.Font;

                int count = dr[Environment.FolderData.UnreadField] is int ? (int)dr[Environment.FolderData.UnreadField] : 0;
                int countAll = dr[Environment.FolderData.AllDocsCountField] is int ? (int)dr[Environment.FolderData.AllDocsCountField] : 0;
                string txt = dr[Environment.FolderData.NameField] as string ?? name;

                var result = new WorkFolderUpdateParams();

                if (count > 0)
                {
                    result.Font = updateParams.Font;
                    result.Text = string.Concat(txt, " (", count.ToString(), "/", countAll.ToString(), ")");
                    result.ExpandParent = true;
                    result.Recursive = recursive;
                }
                else
                {
                    result.Font = new Font(font, FontStyle.Regular);
                    result.Text = string.Concat(txt, " (", countAll.ToString(), ")");
                }

                this.UiThreadBeginInvoke(() =>
                {
                    UpdateStatusEnd(result);
                });
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

	    /// <summary>
	    /// Обновляет статус папки ВРаботе. Асинхронно.
	    /// </summary>
	    /// <param name="updateParams"></param>
	    private void UpdateStatusEnd(WorkFolderUpdateParams updateParams)
        {
			NodeFont = updateParams.Font;
            Text = updateParams.Text;
			OnUpdateNode();
            if (updateParams.ExpandParent)
                ExpandParent(updateParams.Recursive);
        }

	    #endregion

		private bool DeleteAll()
		{
			bool result = true;

			foreach(WorkFolderNode node in Nodes)
			{
				result = node.DeleteAll();
				if(!result)
					break;
			}

			if(result)
				result = Environment.WorkDocData.RemoveDocsFromWorkFolder(ID);
			if(result)
				result = Environment.FolderData.Delete(ID);

			return result;
		}

		#region Drop

		public override bool DropAllowed(Node ground)
		{
			return ground is WorkFolderNode && !IsAncestor(ground);
		}

		public override bool SortedDropAllowed(Node ground)
		{
			var wfNode = ground as WorkFolderNode;
			return wfNode != null && (!IsAncestor(ground) && (wfNode.ID != 0));
		}

		#endregion

		#region Move

		public override bool UnsortedMove(Node where)
		{
			try
			{
				var wfNode = (WorkFolderNode)where;
				if(wfNode == null)
					return false;
				if(Environment.FolderData.UnsortedMove(ID, wfNode.ID))
				{
					TreeView.BeginUpdate();

					Remove();
					wfNode.Nodes.Add(this);

					TreeView.EndUpdate();

					return true;
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}

			return false;
		}

		public override bool SortedMove(Node before)
		{
			try
			{
				var wfNode = (WorkFolderNode)before;
				if(wfNode == null)
					return false;
				bool result = Environment.FolderData.SortedMove(ID, wfNode.ID);
				if(result)
				{
					TreeView.BeginUpdate();

					Remove();
					wfNode.Parent.Nodes.Insert(wfNode.Index, this);

					TreeView.EndUpdate();
				}

				return result;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}

			return false;
		}

		#endregion

		public override void UpdateImages()
		{
			if(ID > 0)
			{
				unselectedCollapsed = Images.SystemFolder;
				selectedCollapsed = Images.SystemFolder;
				unselectedExpanded = Images.OpenSystemFolder;
				selectedExpanded = Images.OpenSystemFolder;
			}

			base.UpdateImages();
		}

		public void ExpandParent(bool recursive)
		{
			if(Parent != null && Parent != this)
			{
				Parent.Expand();
				if(recursive && Parent is WorkFolderNode)
					((WorkFolderNode)Parent).ExpandParent(recursive);
			}
		}

		public override void LoadSubNodes()
		{
			Nodes.Clear();
			if(TreeView != null)
				TreeView.Cursor = Cursors.WaitCursor;
			CancellationToken ct = source.Token;
			Task<DataSet> task = System.Threading.Tasks.Task<DataSet>.Factory.StartNew(() => { return Environment.FolderData.GetFolders(Emp.ID, ct); }, ct);
			Task taskc = System.Threading.Tasks.Task.Factory.ContinueWhenAny<DataSet>( new Task<DataSet>[ ] { task }, (taskC) =>
			{
				if(taskC.Status != TaskStatus.RanToCompletion)
					return;
				if(ct.IsCancellationRequested)
					ct.ThrowIfCancellationRequested();
				DataSet ds ;
				lock(this)
				{
					 ds = taskC.Result;
				}
				if(ds != null && ds.Tables.Contains(Environment.FolderData.TableName) && ds.Relations.Contains(Environment.FolderData.ParentRelation))
					using(DataTable dt = ds.Tables[Environment.FolderData.TableName])
					{
						lock(this)
						{
							var rel = ds.Relations[Environment.FolderData.ParentRelation];
							foreach(DataRow dr in dt.Rows)
							{
								DataRow[ ] drs = dr.GetParentRows(rel);

								if(drs != null && drs.Length == 0)
								{

									if(this.TreeView.InvokeRequired)
										this.TreeView.BeginInvoke(new Action<DataRow>((DataRow dR) =>
											{
												var node = Populate(dR, rel);
												Nodes.Add(node);
												if(ct.IsCancellationRequested)
													ct.ThrowIfCancellationRequested();
												if(dr == dt.Rows[dt.Rows.Count - 1])
													node.UpdateStatusBegin(false, UpdateNode);
												else
													node.UpdateStatusBegin(false);
											}), dr);
									else
									{
										WorkFolderNode node = Populate(dr, rel);
										Nodes.Add(node);
									}
								}
							}
						}
						ds.Dispose();
					}
				else
					OnUpdateNode();
			},ct);
			
			if(TreeView != null)
				TreeView.Cursor = Cursors.Default;

			base.LoadSubNodes();
		}

		public static WorkFolderNode CreateRoot(Employee emp, EventHandler LoadComlete = null)
		{
			return new WorkFolderNode(rootID, rootTitle, emp, LoadComlete);
		}

		public override void LoadDocs(Grids.DocGrid docGrid, bool clean, int curID, string filename = null)
		{
			docGrid.LoadWorkFolderDocs(ID, Emp, clean, curID);
		}

		public override Context BuildContext()
		{
			return new Context(Misc.ContextMode.WorkFolder, ID, Emp);
		}

		public override void Nullify()
		{
			base.Nullify();
			CancelOperation();
			LoadSubNodes();
		}

		public override bool IsWork()
		{
			return true;
		}

		public override bool IsWorkFolder()
		{
			return true;
		}

		public override void RemoveBold()
		{
			UpdateStatusBegin(false);
		}

		public override void RemoveBoldRecursive()
		{
			UpdateStatusBegin(true);
		}

		/// <summary>
		/// Создание новой папки в работе
		/// </summary>
		/// <param name="parentNode">Родитель</param>
		/// <param name="newFolderName">Имя новой папки</param>
		/// <returns>Новая папка</returns>
		public static WorkFolderNode CreateNewFolder(WorkFolderNode parentNode, string newFolderName)
		{
			if(parentNode != null)
				try
				{
					int result = Environment.FolderData.New(parentNode.ID, newFolderName);
					if(result != 0)
					{
						var newNode = new WorkFolderNode(result, newFolderName, parentNode.Emp);

						parentNode.Nodes.Add(newNode);
						newNode.RemoveBold();
						newNode.EnsureVisible();
						return newNode;
					}
				}
				catch(Exception ex)
				{
					Lib.Win.Data.Env.WriteToLog(ex);
				}

			return parentNode;
		}

		public WorkFolderNode AddExistingFolder(int nodeID, int empID)
		{
			if(nodeID <= 0)
				return null;
			try
			{
				DataRow dr = Environment.WorkFolderData(nodeID, empID);
				if(dr == null)
				{
					Environment.ReloadReadData();
					dr = Environment.WorkFolderData(nodeID, empID);
				}

			    WorkFolderNode newNode = null;

			    if (dr != null)
			    {
                    int parentID = (int)dr[Environment.FolderData.ParentField];

                    WorkFolderNode parentNode = ContainsByID(parentID, empID)
                                                    ? GetByID(parentID, empID)
                                                    : AddExistingFolder(parentID, empID);

			        if (parentNode != null)
			        {
                        newNode = new WorkFolderNode(nodeID, (string)dr[Environment.FolderData.NameField], parentNode.Emp);

                        parentNode.Nodes.Add(newNode);
                        newNode.RemoveBold();
                    }
			        else
			        {
                        Lib.Win.Data.Env.WriteToLog("parentNode == null");
			        }
			    }
			    else
			    {
                    Lib.Win.Data.Env.WriteToLog("dr == null");
			    }

				return newNode;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}

			return null;
		}
	}
}