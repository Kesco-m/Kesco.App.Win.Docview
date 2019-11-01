namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.PathNodes
{
    /// <summary>
    ///   Summary description for PathNode.
    /// </summary>
    public class PathNode : Node
    {
        protected PathNode(string path)
        {
            Path = path;
        }

        #region Accessors

        public string Path { get; set; }

        #endregion
    }
}