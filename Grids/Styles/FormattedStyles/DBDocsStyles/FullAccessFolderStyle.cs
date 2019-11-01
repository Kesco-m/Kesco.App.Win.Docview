namespace Kesco.App.Win.DocView.Grids.Styles.FormattedStyles.DBDocsStyles
{
    /// <summary>
    /// Summary description for FullAccessFolderStyle.
    /// </summary>
    public class FullAccessFolderStyle : DBDocsStyle
    {
        private static FullAccessFolderStyle instance;

        #region Constructor & Instance

        protected FullAccessFolderStyle(DocGrid grid) : base(grid)
        {
        }

        public static new Style Instance(DocGrid grid)
        {
            return instance ?? (instance = new FullAccessFolderStyle(grid));
        }

        #endregion

		internal static bool HasInstance()
		{
			return instance != null;
		}

		internal static void DropInstance()
		{
			if(instance != null)
				instance = null;
		}
	}
}