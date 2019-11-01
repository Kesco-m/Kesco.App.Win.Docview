namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.PathNodes.CatalogNodes
{
    /// <summary>
    ///   Summary description for DocTypeSynonymGroupNode.
    /// </summary>
    public class DocTypeSynonymGroupNode : CatalogNode
    {
        internal DocTypeSynonymGroupNode(string path, string name, bool hasSubNodes) :
            base(path, name, hasSubNodes)
        {
        }

        protected override void SetImages()
        {
            unselectedCollapsed = Images.SynonymGroup;
            selectedCollapsed = Images.SynonymGroup;
            unselectedExpanded = Images.SynonymGroup;
            selectedExpanded = Images.SynonymGroup;
        }
    }
}