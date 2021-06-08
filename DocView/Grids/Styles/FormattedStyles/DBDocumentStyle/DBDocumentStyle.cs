using Kesco.App.Win.DocView.Grids.Styles.FormattedStyles.DBDocsStyles;

namespace Kesco.App.Win.DocView.Grids.Styles.FormattedStyles.DBDocumentStyle
{
	public class DBDocumentStyle : DBDocsStyle
	{
		private static DBDocumentStyle instance;

		public DBDocumentStyle(DocGrid grid) : base(grid)
		{
			needBoldField = Environment.DocData.WorkDocReadField;
			FriendlyName = Environment.StringResources.GetString("Document");
		}

		public override void Init()
		{
			base.Init();
			//colParams.WasInited = false;
		}

		public static new Style Instance(DocGrid grid)
		{
			return instance ?? (instance = new DBDocumentStyle(grid));
		}

		public override bool IsDocument()
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