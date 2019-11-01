namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.WorkNodes
{
	public class WorkNode : Node
	{
		protected new const int MaxLabelLength = 50;

		internal WorkNode(int id, string name)
		{
			ID = id;
			this.name = name;
			Text = name;
		}

		#region Accessors

		public Lib.Win.Data.Temp.Objects.Employee Emp { get; protected set; }

		#endregion
	}
}