using Kesco.App.Win.DocView.Misc;
using Kesco.Lib.Win.Data.Temp.Objects;

namespace Kesco.App.Win.DocView
{
    /// <summary>
    /// Контекст
    /// </summary>
    public class Context
    {
        private const int ContextModeCount = 9; // ужасно!

        #region Constructors

        public Context(ContextMode mode)
        {
            Path = "";
            Mode = mode;
        }

        public Context(ContextMode mode, int id) : this(mode)
        {
            ID = id;
        }

        public Context(ContextMode mode, int id, Employee emp) : this(mode)
        {
            ID = id;
            Emp = emp;
        }

        public Context(ContextMode mode, string path) : this(mode)
        {
            Path = path;
        }

        #endregion

        #region Accessors

        public string Path { get; set; }

        public int ID { get; set; }

        public Employee Emp { get; set; }

        public ContextMode Mode { get; set; }

        #endregion

        public bool DBMode()
        {
            return
                WorkOrSharedFolderMode() ||
                (Mode == ContextMode.Catalog) ||
                (Mode == ContextMode.Found);
        }

        public bool WorkOrSharedFolderMode()
        {
            return
                (Mode == ContextMode.WorkFolder) ||
                (Mode == ContextMode.SharedWorkFolder);
        }

        public bool IDMode()
        {
            return
                WorkOrSharedFolderMode() ||
                (Mode == ContextMode.Found) ||
                (Mode == ContextMode.FaxIn) ||
                (Mode == ContextMode.FaxOut) ||
                (Mode == ContextMode.Document);
        }

        public bool AtHome(Context context)
        {
            if (context != null && context.Mode == Mode)
            {
                if (IDMode())
                {
                    if (ID == context.ID)
                        return true;
                }
                else
                {
                    return (Path == context.Path); // считаем, что если нет id, то path выставлено
                }
            }

            return false;
        }

        public static Context BuildContext(string contextModeStr, string path, int id, Employee emp)
        {
            for (int i = 0; i < ContextModeCount; i++)
            {
                var cMode = (ContextMode) i;
                if (cMode.ToString() == contextModeStr)
                    return new Context(cMode) {Path = path, ID = id, Emp = emp};
            }

            return null;
        }
    }
}