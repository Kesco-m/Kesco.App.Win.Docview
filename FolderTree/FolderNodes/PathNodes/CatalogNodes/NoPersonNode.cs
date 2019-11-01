namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.PathNodes.CatalogNodes
{
    /// <summary>
    ///   Summary description for NoPersonNode.
    /// </summary>
    public class NoPersonNode : CatalogNode
    {
        internal NoPersonNode(string path, string name, bool hasSubNodes) :
            base(path, name, hasSubNodes)
        {
        }

        protected override void SetImages()
        {
            unselectedCollapsed = Images.NoPerson;
            selectedCollapsed = Images.NoPerson;
            unselectedExpanded = Images.NoPerson;
            selectedExpanded = Images.NoPerson;
        }
    }
}