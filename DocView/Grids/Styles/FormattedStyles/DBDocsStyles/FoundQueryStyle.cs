namespace Kesco.App.Win.DocView.Grids.Styles.FormattedStyles.DBDocsStyles
{
    public class FoundQueryStyle : FoundStyle
    {
        private static FoundQueryStyle instance;

        protected FoundQueryStyle(DocGrid grid) : base(grid)
        {
            optionFolder = null;
            optionFolder = grid.OptionFolder.Folders.GetByNameForced("FoundStyle");
            FriendlyName = Environment.StringResources.GetString("Inquiries");
        }

        public static new bool HasInstance()
        {
            return instance != null;
        }

        public static new Style Instance(DocGrid grid)
		{
            return instance ?? (instance = new FoundQueryStyle(grid));
        }

		internal static new void DropInstance()
		{
			if(instance != null)
				instance = null;
		}
	}
}