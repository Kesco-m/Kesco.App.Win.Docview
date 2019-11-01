using Kesco.App.Win.DocView.Misc;

namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.FaxNodes
{
    /// <summary>
    /// Узел исходящего факса
    /// </summary>
    public class FaxOutNode : FaxNode
    {
        protected FaxOutNode(int id, string name, string path) : base(name)
        {
            ID = id;
            this.name = name;
            this.path = path;

            Text = name;
        }

        public static FaxOutNode CreateRoot(int id, string name, string path)
        {
            return new FaxOutNode(id, name, path);
        }

		public override void LoadDocs(Grids.DocGrid docGrid, bool clean, int curID, string filename = null)
		{
			this.curID = curID;
			docGrid.Sorted += Sorted;
			docGrid.LoadFaxesOut(ID);
		}

        public override Context BuildContext()
        {
            return new Context(ContextMode.FaxOut, ID, Environment.CurEmp);
        }
    }
}