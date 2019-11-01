using System.Linq;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView
{
    /// <summary>
    ///   Обычный тулбар с функцией возврата кнопки по ее тегу
    /// </summary>
    public class TaggedToolBar : ToolBar
    {
        public TaggedToolBar() : base()
        {
            DoubleBuffered = true;
        }

        public ToolBarButton Button(string tag)
        {
            return
                (from ToolBarButton b in Buttons where b.Tag != null select b).FirstOrDefault(
                    b => b.Tag.ToString() == tag);
        }
    }
}