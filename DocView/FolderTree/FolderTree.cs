using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using Kesco.App.Win.DocView.FolderTree.FolderNodes;
using Kesco.App.Win.DocView.FolderTree.FolderNodes.FaxNodes;
using Kesco.App.Win.DocView.FolderTree.FolderNodes.PathNodes;
using Kesco.App.Win.DocView.FolderTree.FolderNodes.PathNodes.CatalogNodes;
using Kesco.App.Win.DocView.FolderTree.FolderNodes.WorkNodes;
using Kesco.Lib.Win.Data.Temp.Objects;

namespace Kesco.App.Win.DocView.FolderTree
{
    /// <summary>
    ///   Дерево папок документов
    /// </summary>
    public class FolderTree : TreeView
    {
        private CatalogNode catalogNode; // узел архива
        private ScanerNode scanerNode; // узел сканера
        private SynchronizedCollection<FaxInNode> faxInNodes; // узлы входящих факсов
        private SynchronizedCollection<FaxOutNode> faxOutNodes; // узлы выходящих факсов

        private SynchronizedCollection<KeyValuePair<int, WorkFolderNode>> workFolderNodes;
        // узлы рабочих папок

        private SynchronizedCollection<KeyValuePair<int, DocumentNode>> documentNodes;
        // узлы документов-папок

        private SynchronizedCollection<KeyValuePair<int, FoundNode>> foundNodes;
        // узлы папок найденных документов

        private SharedWorkFolderNode sharedWorkFolderNode; // узел расшаренных рабочих папок

        private ToolTip toolTip;
        private Node lastOverNode;
        private IContainer components;
        private const int ToolTipTimeout = 100; // таймаут, после которого выводится тултип (в мс)

        private SolidBrush highlightBrush;
        private SolidBrush backgroundBrush;
        private TextFormatFlags textFormatFlags;

        /// <summary>
        ///   Перезагрузка писем в гриде
        /// </summary>
        public delegate void ChangeDataInGridDelegate(Node selectedNode, bool selectNode);

        /// <summary>
        ///   Событие - изменение данных в grid-e
        /// </summary>
        public event ChangeDataInGridDelegate ChangeDataInGrid;

        public Node PreviouseSelectedNode;

        private void InitializeComponent()
        {
            components = new Container();
            var resources = new ResourceManager(typeof (FolderTree));
        }

        public FolderTree()
        {
            InitializeComponent();

            highlightBrush = new SolidBrush(SystemColors.Highlight);
            backgroundBrush = new SolidBrush(BackColor);
            textFormatFlags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter;

			DrawMode = TreeViewDrawMode.OwnerDrawText;
			DrawNode += FolderTree_DrawNode;

            toolTip = new ToolTip {InitialDelay = ToolTipTimeout};

            BeforeExpand += BeforeExpandHandler;
            AfterExpand += AfterExpandCollapseHandler;

            AfterLabelEdit += AfterLabelEditHandler;
            MouseUp += MouseUpHandler;

            DragOver += On_DragOver;
            DragEnter += On_DragEnter;
            DragDrop += On_DragDrop;

            MouseMove += On_MouseMove;
            BeforeSelect += FolderTree_BeforeSelect;
            AfterSelect += FolderTree_AfterSelect;
        }

        private void FolderTree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            PreviouseSelectedNode = SelectedNode;
        }

        private void FolderTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (PreviouseSelectedNode == null)
                PreviouseSelectedNode = SelectedNode;
        }

        private void FolderTree_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
			if(!e.Node.IsVisible)
				return;

			if(e.Node.IsSelected)
			{
				e.Graphics.FillRectangle(highlightBrush, e.Node.Bounds);
				TextRenderer.DrawText(e.Graphics, e.Node.Text, e.Node.NodeFont, e.Node.Bounds,
									  SystemColors.HighlightText, textFormatFlags);
			}
			else
			{
				e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
				TextRenderer.DrawText(e.Graphics, e.Node.Text, e.Node.NodeFont, e.Node.Bounds, e.Node.ForeColor,
									  textFormatFlags);
			}
        }

        #region Accessors

        public CatalogNode CatalogNode
        {
            get { return catalogNode; }
        }

        public ScanerNode ScanerNode
        {
            get { return scanerNode; }
        }

        public FaxNode FaxInNode
        {
            get { return (faxInNodes != null && faxInNodes.Count > 0) ? faxInNodes[0] : null; }
        }

        public FaxNode FaxOutNode
        {
            get { return (faxOutNodes != null && faxOutNodes.Count > 0) ? faxOutNodes[0] : null; }
        }

        public WorkFolderNode WorkFolderNode
        {
            get
            {
                return workFolderNodes != null
                           ? (from t in workFolderNodes where t.Key == Environment.CurEmp.ID select t.Value).
                                 FirstOrDefault()
                           : null;
            }
        }

        public SharedWorkFolderNode SharedWorkFolderNode
        {
            get { return sharedWorkFolderNode; }
        }

        public FoundNode FoundNodes(int empID)
        {
            return foundNodes != null
                       ? (from t in foundNodes where t.Key == empID select t.Value).FirstOrDefault()
                       : null;
        }

        public FoundNode FoundNode
        {
            get
            {
                return foundNodes != null
                           ? (from t in foundNodes where t.Key == Environment.CurEmp.ID select t.Value).FirstOrDefault()
                           : null;
            }
        }

        #endregion

        //из файла commctrl.h
        private const int TVS_NOTOOLTIPS = 0x0080;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.Style |= TVS_NOTOOLTIPS;
                return createParams;
            }
        }

        public SystemFolderNode AddSystemFolder(string path, string fileName)
        {
            int i;
			for(i = 0; i < Nodes.Count; i++)
			{
				var node = Nodes[i] as Node;
				if(node == null || !node.IsSystemFolder())
					break;

				var sysNode = node as SystemFolderNode;
				if(sysNode == null)
					continue;

				if(path.IndexOf(sysNode.Path) != -1) // new folder lays under opened folder
				{
					// find and select needed node
					sysNode.LoadSubNodes();
					//  sysNode.EnsureVisible();
					while(sysNode.Nodes.Count > 0 && sysNode.Path != path)
					{
						foreach( var subNode in sysNode.Nodes.Cast<TreeNode>().Select(treeNode => treeNode as SystemFolderNode).Where(subNode => path.IndexOf(subNode.Path) != -1))
						{
							sysNode = subNode;
							break;
						}
						sysNode.LoadSubNodes();
					}
					sysNode.EnsureVisible();

					SelectedNode = sysNode;
					return sysNode;
				}

				if(path.CompareTo(sysNode.Path) < 0) // sorted place found
					break;
			}

            SystemFolderNode newNode = SystemFolderNode.CreateRoot(path);

            if (i < Nodes.Count)
                Nodes.Insert(i, newNode);
            else
                Nodes.Add(newNode);

            // убираем болд
            newNode.RemoveBoldRecursive();

			newNode.SetCurFileName(fileName);
            SelectedNode = newNode;
            return newNode;
        }

        public DocumentNode AddDocumentNode(int docID, bool needVisible, string nodeTitle, bool links)
        {
            if (documentNodes == null)
                documentNodes = new SynchronizedCollection<KeyValuePair<int, DocumentNode>>();
            int indexOf = -1;
            for (int i = 0; i < documentNodes.Count && indexOf == -1; i++)
                if (documentNodes[i].Key == docID)
                    indexOf = i;

            if (indexOf == -1)
            {
                try
                {
                    var newNode = new DocumentNode(docID, docID.ToString(), nodeTitle, links);
                    documentNodes.Add(new KeyValuePair<int, DocumentNode>(docID, newNode));
                    if (!Environment.UserSettings.LinkDocIDs.Contains(docID))
                        Environment.UserSettings.LinkDocIDs.Add(docID);

                    if (workFolderNodes.Count > 0)
                    {
                        int placeid = WorkFolderNode.Index;
                        Nodes.Insert(placeid + 1, newNode);
                        lastOverNode = null;
                    }
                    else
                        Nodes.Insert(1, newNode);
                    // убираем болд
                    newNode.RemoveBoldRecursive();

                    return newNode;
                }
                catch
                {
                    if (Environment.UserSettings.LinkDocIDs.Contains(docID))
                        Environment.UserSettings.LinkDocIDs.Remove(docID);
                    return null;
                }
            }

            DocumentNode docNode = documentNodes[indexOf].Value;
            docNode.LoadSubNodes();
            if (!Nodes.Contains(docNode))
            {
                if (workFolderNodes.Count > 0)
                {
                    int placeid = WorkFolderNode.Index;
                    Nodes.Insert(placeid + 1, docNode);
                    lastOverNode = null;
                }
                else
                    Nodes.Insert(1, docNode);
            }
            if (needVisible)
                docNode.EnsureVisible();
            return docNode;
        }

        #region Selection

		public void SelectScanerFolder(string path, ScanerNode node)
		{
			// find and select needed node
			node.LoadSubNodes();
			while((node.Nodes.Count > 0) && (node.Path.ToLower() != path.ToLower()))
			{
				bool found = false;
				for(int i = 0; i < node.Nodes.Count; i++)
				{
					var subNode = (ScanerNode)node.Nodes[i];
					if(path.ToLower().IndexOf(subNode.Path.ToLower()) == -1)
						continue;
					node = subNode;
					found = true;
					break;
				}

				if(found)
					node.LoadSubNodes();
				else
					break;
			}

			node.EnsureVisible();
			SelectedNode = node;
		}

		public bool SelectWorkFolder(int wfID, Employee emp, int docID)
		{
			// find and select needed node
			object obj = null;
			for(int i = 0; i < workFolderNodes.Count && obj == null; i++)
				if(workFolderNodes[i].Key == emp.ID)
					obj = workFolderNodes[i].Value;
#if AdvancedLogging
				Lib.Log.Logger.DurationMetter( "SelectWorkFolder");
#endif
			if(obj is WorkFolderNode)
			{
				var node = obj as WorkFolderNode;
				bool atHome = false;
				if(SelectedNode is WorkFolderNode)
				{
					var selNode = SelectedNode as WorkFolderNode;
					atHome = (selNode.ID == wfID) && (emp.Equals(selNode.Emp));
				}

				if(!atHome)
				{
					using(DataSet ds = Environment.FolderData.GetParentFolders(wfID))
					{
						if(ds != null)
						{
							DataTable dt = ds.Tables[Environment.FolderData.TableName];

							for(int i = 0; i < dt.Rows.Count; i++)
							{
								var curID = (int)dt.Rows[i][Environment.FolderData.IDField];

								for(int j = 0; j < node.Nodes.Count; j++)
								{
									var subNode = (WorkFolderNode)node.Nodes[j];
									if(subNode.ID == curID)
									{
										node = subNode;
										break;
									}
								}
							}
						}
					}
					if(node != null)
					{
						node.EnsureVisible();
						node.SetCurID(docID);
						SelectedNode = node;
					}
				}
				return atHome;
			}
			return false;
		}

		private void SelectN<T>(int id, SynchronizedCollection<T> coll)  where T : Node
		{
			if(coll == null)
				return;
			T selNode = coll.FirstOrDefault<T>(x => x.ID == id);
			if(selNode != null)
			{
				selNode.EnsureVisible();
				SelectedNode = selNode;
			}
		}

        public void SelectFaxIn(int id)
        {
			SelectN<FaxInNode>(id, faxInNodes);
        }


        public void SelectFaxOut(int id)
        {
			SelectN<FaxOutNode>(id, faxOutNodes);
        }

        public void SelectSharedWorkFolder(int id, SharedWorkFolderNode swfNode, int curID)
        {
            // find and select needed node
            SharedWorkFolderNode node = swfNode;

			if(SelectedNode != swfNode) // ???
			{
				using(DataSet ds = Environment.SharedFolderData.GetParentFolders(id))
				{
					if(ds != null)
					{
						DataTable dt = ds.Tables[Environment.SharedFolderData.TableName];

						foreach(DataRow dr in dt.Rows)
						{
							SharedWorkFolderNode subNode = node.Nodes.Cast<SharedWorkFolderNode>().FirstOrDefault(x => x.ID == (int)dr[Environment.SharedFolderData.IDField]);
							if(subNode != null)
							{
								node = subNode;
								break;
							}
						}
					}
				}
			}

            node.EnsureVisible();
			node.SetCurID(curID);
            SelectedNode = node;
        }

        public void SelectFoundFolder(int id, FoundNode node, int curID)
        {
            FoundNode found = FindFoundNode(id, node);
            if (found != null)
                node = found;

            node.EnsureVisible();
			node.SetCurID(curID);
            SelectedNode = node;
        }

        public FoundNode FindFoundNode(int id, FoundNode node)
        {
            return node == null ? null : node.ID == id ? node : node.Nodes.Cast<FoundNode>().FirstOrDefault(subNode => subNode.ID == id);
        }

        public void SelectCatalogNode(string path, int curID)
        {
            // find and select needed node
            SuspendLayout();
            CatalogNode node = catalogNode;
            if (node == null)
            {
                ResumeLayout();
                return;
            }

            char[] cpinChar = CatalogNode.CatalogPathInitial.ToCharArray();
            string[] steps = path.Replace(" ", "").Trim(cpinChar).Split(cpinChar);

            for (int i = 0; i < steps.Length; i++)
            {
                Console.WriteLine("{0}: Step: {1} {2}", DateTime.Now.ToString("HH:mm:ss fff"), i, steps[i]);
                bool changed = false;

                // searching for node
                for (int j = 0; j < node.Nodes.Count && node.Nodes[j].Text != ""; j++)
                {
                    var subNode = node.Nodes[j] as CatalogNode;
                    if (subNode != null && subNode.Path == CatalogNode.CatalogPathInitial + steps[i])
                    {
                        node = subNode;
                        changed = true;
                        break;
                    }
                }

                if (changed) // node found
                    node.EnsureVisible();
                else // not found -> reloading
                {
                    node.Nodes.Clear();
                    node.Nodes.Add(new TreeNode());

                    Console.WriteLine("{0}: Load subnodes", DateTime.Now.ToString("HH:mm:ss fff"));
                    node.LoadSubNodes();

                    // searching for node again
                    for (int j = 0; j < node.Nodes.Count; j++)
                    {
                        var subNode = (CatalogNode) node.Nodes[j];
                        if (subNode.Path == CatalogNode.CatalogPathInitial + steps[i])
                        {
                            node = subNode;
                            changed = true;
                            break;
                        }
                    }
                }

                if (!changed)
                    break;
            }
			node.SetCurID(curID);
            ResumeLayout();
            SelectedNode = node;
        }

        public DocumentNode SelectDocumentNode(int docID, int curID)
        {
            SuspendLayout();
            DocumentNode node = documentNodes.FirstOrDefault(x => x.Key == docID).Value;
			node.SetCurID(curID);
            ResumeLayout();
            if (node != null)
                SelectedNode = node;
            return node;
        }

        #endregion

        private void AfterExpandCollapseHandler(object sender, TreeViewEventArgs e)
        {
            var node = (Node) e.Node;
            if (node != null)
                node.UpdateImages();
        }

        private void BeforeExpandHandler(object sender, TreeViewCancelEventArgs e)
        {
            var node = (Node) e.Node;
            if (node != null && node.Nodes.Count > 0 && node.Nodes[0].Text == "")
                node.LoadSubNodes();
        }

        private void MouseUpHandler(object sender, MouseEventArgs e)
        {
            var node = (Node) GetNodeAt(e.X, e.Y);
            if (node != null)
                node.On_MouseUp(e);
        }

        private void AfterLabelEditHandler(object sender, NodeLabelEditEventArgs e)
        {
            var node = (Node) e.Node;
            if (node != null)
                node.On_AfterLabelEdit(e);
        }

        /// <summary>
        ///   Событие - изменение информации в grid-e
        /// </summary>
        public virtual void OnChangeDataInGrid(Node selectedNode, bool selectNode)
        {
            if (ChangeDataInGrid != null)
                ChangeDataInGrid(selectedNode, selectNode);
        }

        #region Drag'n'Drop

        private Node dropNode; //DragDroppedNode

        private Node overNode;
        //хранит элемент, над которым в данный момент происходит перетаскивание (для Expand)

        private void On_DragDrop(object sender, DragEventArgs e)
        {
            var nodeTo = (Node) GetNodeAt(PointToClient(new Point(e.X, e.Y)));
            Node node = dropNode;

            if (overNode != null)
            {
                overNode.BackColor = BackColor;
                overNode = null;
            }

            if (node == null || node.Equals(nodeTo) || nodeTo == null)
                return;

            bool dropAllowed = (e.KeyState & 0x08) == 0
                                   ? node.DropAllowed(nodeTo)
                                   : node.SortedDropAllowed(nodeTo);

            if (!dropAllowed)
                return;

            try
            {
                if ((e.KeyState & 0x08) == 0) // not CTRL
                    node.UnsortedMove(nodeTo);
                else
                    node.SortedMove(nodeTo);

                SelectedNode = node;
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void On_DragEnter(object sender, DragEventArgs e)
        {
            TreeNode node = GetNodeAt(e.X, e.Y);
            if (node is WorkFolderNode)
                e.Effect = DragDropEffects.Move;
        }

        private void On_DragOver(object sender, DragEventArgs e)
        {
            var node = (Node) GetNodeAt(PointToClient(new Point(e.X, e.Y)));

            if (node == null)
            {
                if (overNode != null)
                    overNode.BackColor = BackColor;
                return;
            }

            if (node.Equals(overNode))
                return;

            if (overNode != null)
                overNode.BackColor = BackColor;

            overNode = node;

            bool dropAllowed = (e.KeyState & 0x08) == 0 ? dropNode.DropAllowed(overNode) : dropNode.SortedDropAllowed(overNode);

            e.Effect = dropAllowed ? DragDropEffects.Move : DragDropEffects.None;

            if (overNode.NextVisibleNode != null)
                overNode.NextVisibleNode.EnsureVisible();
            if (overNode.PrevVisibleNode != null)
                overNode.PrevVisibleNode.EnsureVisible();
        }

        #endregion

        private void KillToolTip()
        {
            if (toolTip != null)
                toolTip.SetToolTip(this, null);
        }

        private void ShowToolTip(Node node)
        {
            if (node == null)
                return;

            string tipText = node.GetToolTip();
            if (!string.IsNullOrEmpty(tipText))
                toolTip.SetToolTip(this, tipText);
        }

        private void On_MouseMove(object sender, MouseEventArgs e)
        {
            if (FindForm() != Form.ActiveForm)
                return;

            var node = (Node) GetNodeAt(e.X, e.Y);
            if (node != null)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        if (SelectedNode == node)
                        {
                            dropNode = node;
                            DoDragDrop(node, DragDropEffects.Move);
                        }
                        break;
                }

                if (node != lastOverNode)
                {
                    KillToolTip();
                    ShowToolTip(node);
                }
            }
            else
                KillToolTip();

            lastOverNode = node;
        }

        public void RemoveBold()
        {
            foreach (Node node in Nodes)
            {
                node.RemoveBoldRecursive();
            }
        }

        #region Creation

        private void PrepareAndAdd(Node node)
        {
            node.RemoveBold();
            Nodes.Add(node);
        }

        public void CreateWorkFolderRoot(EventHandler LoadComplete = null)
        {
            int[] empIDs = Environment.CurrentEmpIDs();
            workFolderNodes = new SynchronizedCollection<KeyValuePair<int, WorkFolderNode>>();

            PrepareAndAddWorkFolder(Environment.CurEmp, LoadComplete);

            foreach (  var emp in  from empID in empIDs where !Environment.CurEmp.ID.Equals(empID) select new Employee(empID, Environment.EmpData))
            {
                PrepareAndAddWorkFolder(emp);
            }
        }

		private void PrepareAndAddWorkFolder(Employee emp,EventHandler LoadComplete = null)
		{
			WorkFolderNode workFolderNode = WorkFolderNode.CreateRoot(emp, LoadComplete);

			PrepareAndAdd(workFolderNode);
			workFolderNodes.Add(new KeyValuePair<int, WorkFolderNode>(emp.ID, workFolderNode));
		}

        public void CreateFoundRoot()
        {
            int[] empIDs = Environment.CurrentEmpIDs();
            foundNodes = new SynchronizedCollection<KeyValuePair<int, FoundNode>>();

            PrepareAndAddFoundNode(Environment.CurEmp);

            foreach (
                var emp in
                    from empID in empIDs
                    where !Environment.CurEmp.ID.Equals(empID)
                    select new Employee(empID, Environment.EmpData))
            {
                PrepareAndAddFoundNode(emp);
            }
        }

        private void PrepareAndAddFoundNode(Employee employee)
        {
            FoundNode foundNode =
                FoundNode.CreateRoot(employee);

            PrepareAndAdd(foundNode);
            foundNodes.Add(new KeyValuePair<int, FoundNode>(employee.ID, foundNode));
        }

        public void CreateCatalogRoot()
        {
            catalogNode = CatalogNode.CreateRoot();
            PrepareAndAdd(catalogNode);
        }

		public void CreateSharedWorkFolderRoot(bool fullAccessOnly)
		{
			sharedWorkFolderNode = SharedWorkFolderNode.CreateRoot(fullAccessOnly);
			if(!fullAccessOnly || sharedWorkFolderNode.Nodes.Count > 0)
				PrepareAndAdd(sharedWorkFolderNode);
		}

        public void CreateFaxInRoot()
        {
			CreateFaxRoot<FaxInNode>(faxInNodes, Environment.FaxFolderData.GetFaxInFolders, FolderNodes.FaxNodes.FaxInNode.CreateRoot);
        }

		public void CreateFaxOutRoot()
		{
			CreateFaxRoot<FaxOutNode>(faxOutNodes, Environment.FaxFolderData.GetFaxOutFolders, FolderNodes.FaxNodes.FaxOutNode.CreateRoot);
		}

		public void CreateFaxRoot<T>(SynchronizedCollection<T> faxNodes, Func<String, DataTable> execFunc, Func<int, string, string, T> CreateRoot) where T : FaxNode
		{
			faxNodes = new SynchronizedCollection<T>();

			using(DataTable dt = execFunc(Environment.CurCultureInfo.TwoLetterISOLanguageName))
			using(DataTableReader dr = dt.CreateDataReader())
			{
				while(dr.Read())
				{
					var nodeID = (int)dr[Environment.FaxFolderData.IDField];
					var nodeName = (string)dr[Environment.FaxFolderData.NameField];
					var nodePath = (string)dr[Environment.FaxFolderData.NetworkPathField];
					var server = Lib.Win.Document.Environment.GetServers().FirstOrDefault(t => Directory.Exists(Path.Combine(t.FaxPath, nodePath)));

					if(server != null && !string.IsNullOrEmpty(server.FaxPath))
					{
						if(DirectoryAnalyser.IsAccessible(Path.Combine(server.FaxPath, nodePath)))
						{
							T node = CreateRoot(nodeID, nodeName, Path.Combine(server.FaxPath, nodePath));
							PrepareAndAdd(node);
							faxNodes.Add(node);
						}
					}
				}
				dr.Close();
				dr.Dispose();
				dt.Dispose();
			}
		}

        public void CreateScanerRoot()
        {
            scanerNode = ScanerNode.CreateRoot();
			if(scanerNode != null)
				PrepareAndAdd(scanerNode);
        }

        #endregion

        public Context GetContext()
        {
            return SelectedNode != null ? SelectedNode.BuildContext() : null;
        }

        public void NullifyNodes()
        {
            SelectedNode = null;

            foreach (Node node in Nodes)
                node.Nullify();
        }

        public new Node SelectedNode
        {
            get
            {
                try
                {
                    return base.SelectedNode as Node;
                }
                catch
                {
                    return null;
                }
            }
            set { base.SelectedNode = value; }
        }

        #region Update Status

		public void UpdateWorkFolderStatus()
		{
			UpdateWorkFolderStatus(true, true);
		}

		public void UpdateWorkFolderStatus(bool recursive, bool refresh)
		{
			if(workFolderNodes == null)
				return;

			try
			{
			Lib.Log.Logger.EnterMethod(this, "UpdateWorkFolderStatus(bool recursive)");

#if AdvancedLogging
			using (Lib.Log.Logger.DurationMetter("UpdateWorkFolderStatus Environment.ReloadReadData();"))
#endif
				if(refresh)
					Environment.ReloadReadData();

#if AdvancedLogging
			Lib.Log.Logger.Message("UpdateWorkFolderStatus Begin foreach (KeyValuePair<int, WorkFolderNode> t in workFolderNodes)");
#endif
				foreach(KeyValuePair<int, WorkFolderNode> t in workFolderNodes)
				{
					WorkFolderNode node = t.Value;
                node.UpdateStatusBegin(recursive);
				}

#if AdvancedLogging
			Lib.Log.Logger.Message("UpdateWorkFolderStatus End foreach (KeyValuePair<int, WorkFolderNode> t in workFolderNodes)");
#endif
			}
			finally
			{
				Lib.Log.Logger.LeaveMethod(this, "UpdateWorkFolderStatus(bool recursive)");
			}
		}

        public void UpdateFaxStatus()
        {
            UpdateFaxInStatus();
            UpdateFaxOutStatus();
        }

		public void UpdateFaxInStatus()
		{
			if(faxInNodes == null)
				return;

			try
			{
#if AdvancedLogging
                Lib.Log.Logger.EnterMethod(this,"UpdateFaxInStatus");
#endif
				foreach(FaxInNode node in faxInNodes)
					node.UpdateStatus();
			}
			finally
			{
#if AdvancedLogging
                Lib.Log.Logger.LeaveMethod(this, "UpdateFaxInStatus");
#endif
			}
		}

        public void UpdateFaxOutStatus()
        {
            if (faxOutNodes == null)
                return;

            try
            {
#if AdvancedLogging
                Lib.Log.Logger.EnterMethod(this, "UpdateFaxOutStatus");
#endif
                foreach (FaxOutNode node in faxOutNodes)
                    node.UpdateStatus();
            }
            finally
            {
#if AdvancedLogging
                Lib.Log.Logger.LeaveMethod(this, "UpdateFaxOutStatus");
#endif
            }
        }

        #endregion
    }
}