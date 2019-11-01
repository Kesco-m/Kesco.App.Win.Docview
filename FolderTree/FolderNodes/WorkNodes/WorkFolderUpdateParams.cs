using System.Drawing;

namespace Kesco.App.Win.DocView.FolderTree.FolderNodes.WorkNodes
{
    /// <summary>
    /// Параметры обновления рабочей папки
    /// </summary>
    internal struct WorkFolderUpdateParams
    {
        /// <summary>
        /// Рекурсивное обновление вложенных папок
        /// </summary>
        internal bool Recursive { get; set; }

        /// <summary>
        /// Текст папки
        /// </summary>
        internal string Text { get; set; }

        /// <summary>
        /// Шрифт
        /// </summary>
        internal Font Font { get; set; }

        /// <summary>
        /// Развернуть родительскую папку
        /// </summary>
        internal bool ExpandParent { get; set; }
    }
}
