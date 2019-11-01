using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Misc;
using Kesco.Lib.Win.Document;
using Microsoft.Win32;

namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.PathNodes
{
	/// <summary>
	///   Узел дерева - системная папка
	/// </summary>
	public class SystemFolderNode : PathNode
	{
		protected SystemFolderNode(string path, bool root) : base(path)
		{
			unselectedCollapsed = Images.SystemFolder;
			selectedCollapsed = Images.SystemFolder;
			unselectedExpanded = Images.OpenSystemFolder;
			selectedExpanded = Images.OpenSystemFolder;

			UpdateImages();

			if(!Directory.Exists(path))
				return;
			var dir = new DirectoryInfo(path);

			name = root ? path : dir.Name;

			try
			{
				using(RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Office"))
				{
					NumberFormatInfo info = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
					if(info == null)
						return;
					info.NumberDecimalSeparator = ".";

					if(regKey != null)
					{
						double ver;
						string[] versions = regKey.GetSubKeyNames();
						foreach(string t in versions.Where(t => double.TryParse(t, NumberStyles.Float, info, out ver) && ver > 0))
						{
							using(RegistryKey versionKey = regKey.OpenSubKey(t + "\\Outlook\\Security"))
							{
								if(versionKey == null)
									continue;

								string opath = versionKey.GetValue("OutlookSecureTempFolder").ToString();
								if(!string.IsNullOrEmpty(opath) && opath.TrimEnd(System.IO.Path.DirectorySeparatorChar).Equals( name.TrimEnd(System.IO.Path.DirectorySeparatorChar), StringComparison.CurrentCultureIgnoreCase))
								{
									name = Environment.StringResources.GetString("outlookdir");

									opath = opath.TrimEnd(System.IO.Path.DirectorySeparatorChar).ToLower();
									GetFileType.OutlookSecureTempFolder = opath;

									break;
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}

			Text = name;

			try
			{
				DirectoryInfo[] dirSubs = dir.GetDirectories();
				int subCount = 0;
				if(dirSubs != null)
					subCount = dirSubs.Where(dirSub => (dirSub.Attributes & FileAttributes.Hidden) == 0).Count
						(dirSub => DirectoryAnalyser.IsAccessible(dirSub.FullName));

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
			try
			{
				if(Nodes.Count > 0 && string.IsNullOrEmpty(Nodes[0].Text)) // node's children loading needed
				{
					Nodes.Clear();

					DirectoryInfo[] dirSubs = (new DirectoryInfo(Path)).GetDirectories();

					TreeView.Cursor = Cursors.WaitCursor;
					foreach(DirectoryInfo dirSub in dirSubs)
						if((dirSub.Attributes & FileAttributes.Hidden) == 0 &&		// не скрытый каталог
							Directory.Exists(dirSub.FullName) && DirectoryAnalyser.IsAccessible(dirSub.FullName))	// есть доступ
						{
							SystemFolderNode newSubNode = new SystemFolderNode(dirSub.FullName, false);
							Nodes.Add(newSubNode);
						}

					TreeView.Cursor = Cursors.Default;
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}

			RemoveBoldRecursive();
		}

		public static SystemFolderNode CreateRoot(string path)
		{
			return new SystemFolderNode(path, true);
		}

		public override void LoadDocs(Grids.DocGrid docGrid, bool clean, int curID, string filename = null)
		{
			//this.curFileName = filename;
			if(Directory.Exists(Path))
			{
				docGrid.Sorted += Sorted;
				docGrid.LoadDiskImages(Path, clean);
			}
			else
				Remove();
		}

		protected override void Sorted(object sender, EventArgs e)
		{
			Grids.DocGrid grid = sender as Grids.DocGrid;
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
			return new Context(ContextMode.SystemFolder, Path);
		}

		public override void Nullify()
		{
			base.Nullify();
			try
			{
				Nodes.Clear();
				Nodes.Add(new TreeNode());
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public override bool IsSystemFolder()
		{
			return true;
		}
	}
}