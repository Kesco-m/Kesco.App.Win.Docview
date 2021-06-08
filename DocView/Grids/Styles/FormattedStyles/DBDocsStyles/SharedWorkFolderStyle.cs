namespace Kesco.App.Win.DocView.Grids.Styles.FormattedStyles.DBDocsStyles
{
    /// <summary>
    /// Summary description for SharedWorkFolderStyle.
    /// </summary>
    public class SharedWorkFolderStyle : DBDocsStyle
    {
        private static SharedWorkFolderStyle instance;

        #region Constructor & Instance

        protected SharedWorkFolderStyle(DocGrid grid)
            : base(grid)
        {
        }

        public static new Style Instance(DocGrid grid)
        {
            return instance ?? (instance = new SharedWorkFolderStyle(grid));
        }

        #endregion

        public override bool IsDBDocsFullAccess()
        {
            return false;
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