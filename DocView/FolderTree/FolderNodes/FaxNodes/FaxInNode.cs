using Kesco.App.Win.DocView.Misc;

namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.FaxNodes
{
    /// <summary>
    /// Узел входящих факсов
    /// </summary>
    public class FaxInNode : FaxNode
    {
        protected FaxInNode(int id, string name, string path) : base(name)
        {
            ID = id;
            this.name = name;
            this.path = path;

            Text = name;
        }

        public static FaxInNode CreateRoot(int id, string name, string path)
        {
            return new FaxInNode(id, name, path);
        }

		public override void LoadDocs(Grids.DocGrid docGrid, bool clean, int curID, string filename = null)
		{
			this.curID = curID;
			docGrid.Sorted += Sorted;
			docGrid.LoadFaxesIn(ID);
		}

        public override Context BuildContext()
        {
            return new Context(ContextMode.FaxIn, ID, Environment.CurEmp);
        }
    }
}