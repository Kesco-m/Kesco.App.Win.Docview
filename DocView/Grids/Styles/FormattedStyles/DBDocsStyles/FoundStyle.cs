namespace Kesco.App.Win.DocView.Grids.Styles.FormattedStyles.DBDocsStyles
{
    public class FoundStyle : DBDocsStyle
    {
        private static FoundStyle instance;

        #region Constructor & Instance

        protected FoundStyle(DocGrid grid)
            : base(grid)
        {
            FriendlyName = Environment.StringResources.GetString("FolderTree.FolderNodes.FoundNode.Title1");//Environment.StringResources.GetString("Found");
        }

        public static new Style Instance(DocGrid grid)
        {
            return instance ?? (instance = new FoundStyle(grid));
        }

        #endregion

        public override bool IsFound()
        {
            return true;
        }

        public override bool IsShownInSettings()
        {
            return true;
        }

		internal static new bool HasInstance()
		{
			return instance != null;
		}

		internal static new void DropInstance()
		{
			if(instance != null)
				instance = null;
		}
	}
}