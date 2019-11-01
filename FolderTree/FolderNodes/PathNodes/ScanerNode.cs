using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Misc;

namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.PathNodes
{
	/// <summary>
	/// Каталог сканера
	/// </summary>
	public class ScanerNode : PathNode
	{
		private static string rootTitle = Environment.StringResources.GetString("Scaner");

		protected ScanerNode(string path, bool root): base(path)
		{
			if(root)
			{
				unselectedCollapsed = Images.Scaner;
				selectedCollapsed = Images.Scaner;
				unselectedExpanded = Images.Scaner;
				selectedExpanded = Images.Scaner;
			}
			else
			{
				unselectedCollapsed = Images.SystemFolder;
				selectedCollapsed = Images.SystemFolder;
				unselectedExpanded = Images.OpenSystemFolder;
				selectedExpanded = Images.OpenSystemFolder;
			}

			UpdateImages();

			var dir = new DirectoryInfo(path);

			name = root ? rootTitle : dir.Name;

			Text = name;

			try
			{
				DirectoryInfo[] dirSubs = dir.GetDirectories();
				int subCount =
					dirSubs.Where(dirSub => (dirSub.Attributes & FileAttributes.Hidden) == 0).Count(
						dirSub => DirectoryAnalyser.IsAccessible(dirSub.FullName));

				if(subCount > 0)
					Nodes.Add(new TreeNode());
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public override void LoadSubNodes()
		{
			if(Nodes.Count <= 0 || Nodes[0].Text != "")
				return;
			try
			{
				Nodes.Clear();

				var dirSubs = (new DirectoryInfo(Path)).GetDirectories();

				TreeView.Cursor = Cursors.WaitCursor;
				foreach(var newSubNode in from dirSub in dirSubs
										  where (dirSub.Attributes & FileAttributes.Hidden) == 0
										  where DirectoryAnalyser.IsAccessible(dirSub.FullName)
										  select new ScanerNode(dirSub.FullName, false))
					Nodes.Add(newSubNode);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}

			base.LoadSubNodes();
			TreeView.Cursor = Cursors.Default;
		}

		public static ScanerNode CreateRoot()
		{
			if(Environment.IsConnected)
			{
				string rootPath = Lib.Win.Document.Environment.GetServers().FirstOrDefault(t => !string.IsNullOrEmpty(t.ScanPath)).ScanPath;
				if(!string.IsNullOrEmpty(rootPath) && DirectoryAnalyser.IsAccessible(rootPath))
					return new ScanerNode(rootPath, true);
			}
			return null;
		}

		public override void LoadDocs(Grids.DocGrid docGrid, bool clean, int curID, string filename = null)
		{
			this.curFileName = filename;
			docGrid.Sorted += Sorted;
			docGrid.LoadScans(Path, clean);
		}

		protected override void Sorted(object sender, EventArgs e)
		{
			Grids.DocGrid grid = sender as Grids.DocGrid ;
			if(grid == null)
				return;
			grid.Sorted -= Sorted;
			if(!string.IsNullOrEmpty(curFileName))
			{
				grid.SelectConditional(Environment.ImageReader.FullNameField, curFileName, false);
				curFileName = null;
			}
		}

		public override Context BuildContext()
		{
			return new Context(ContextMode.Scaner, Path);
		}

		public override bool IsScaner()
		{
			return true;
		}
	}
}