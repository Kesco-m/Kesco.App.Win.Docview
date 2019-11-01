namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.PathNodes.CatalogNodes
{
    /// <summary>
    ///   Summary description for CompanyNode.
    /// </summary>
    public class CompanyNode : CatalogNode
    {
        internal CompanyNode(string path, string name, bool hasSubNodes) :
            base(path, name, hasSubNodes)
        {
        }

        protected override void SetImages()
        {
            unselectedCollapsed = Images.CatalogCompany;
            selectedCollapsed = Images.CatalogCompany;
            unselectedExpanded = Images.CatalogCompany;
            selectedExpanded = Images.CatalogCompany;
        }
    }
}