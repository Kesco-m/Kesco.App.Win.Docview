namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.PathNodes.CatalogNodes
{
    /// <summary>
    ///   Summary description for PersonNode.
    /// </summary>
    public class PersonNode : CatalogNode
    {
        internal PersonNode(string path, string name, bool hasSubNodes) :
            base(path, name, hasSubNodes)
        {
        }

        protected override void SetImages()
        {
            unselectedCollapsed = Images.CatalogPerson;
            selectedCollapsed = Images.CatalogPerson;
            unselectedExpanded = Images.CatalogPerson;
            selectedExpanded = Images.CatalogPerson;
        }
    }
}