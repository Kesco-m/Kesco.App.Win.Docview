namespace Kesco.App.Win.DocView.Grids.Styles.FormattedStyles.DBDocsStyles
{
	public class WorkFolderStyle : DBDocsStyle
    {
        private static WorkFolderStyle instance;

        #region Constructor & Instance
		 protected WorkFolderStyle(DocGrid grid)
            : base(grid)
        {
            FriendlyName = Environment.StringResources.GetString("InWorkDoc");
        }


        public static new Style Instance(DocGrid grid)
        {
            return instance ?? (instance = new WorkFolderStyle(grid));
        }

        #endregion

        #region Columns

        public override int GetColumnWidth(string colName)
        {
            return colName == Environment.DocData.MessageField ? 119 : base.GetColumnWidth(colName);
        }

        public override string GetColumnHeaderName(string colName)
        {
            return colName == Environment.DocData.MessageField ? Environment.StringResources.GetString("Message") : base.GetColumnHeaderName(colName);
        }

        public override bool IsColumnSystem(string column)
        {
            bool baseIsSystem = false;

            if (colParams != null)
                try
                {
                    baseIsSystem = colParams.IsColumnSystem(column);
                }
                catch
                {
                    baseIsSystem = false;
                }

            return baseIsSystem || (column != Environment.DocData.IDField &&
                column != Environment.DocData.DocTypeField &&
                column != Environment.DocData.NameField &&
                column != Environment.DocData.DateField &&
                column != Environment.DocData.NumberField &&
                column != Environment.DocData.DescriptionField &&
                column != Environment.DocData.SpentField &&
                column != Environment.DocData.WorkDocReadField &&
                column != Environment.DocData.MessageField);
        }

        #endregion

        public override bool IsWorkFolder()
        {
            return true;
        }

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