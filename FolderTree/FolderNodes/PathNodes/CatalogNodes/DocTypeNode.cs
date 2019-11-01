namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.PathNodes.CatalogNodes
{
    /// <summary>
    ///   Summary description for DocTypeNode.
    /// </summary>
    public class DocTypeNode : CatalogNode
    {
        internal DocTypeNode(string path, string name, bool hasSubNodes) :
            base(path, name, hasSubNodes)
        {
        }

        protected override void SetImages()
        {
            unselectedCollapsed = Images.CatalogDocType;
            selectedCollapsed = Images.CatalogDocType;
            unselectedExpanded = Images.CatalogDocType;
            selectedExpanded = Images.CatalogDocType;
        }
    }
}