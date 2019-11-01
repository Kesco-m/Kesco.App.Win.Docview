using System.Windows.Forms;
using Kesco.Lib.Win.Data.DALC.Documents;

namespace Kesco.App.Win.DocView.Items
{
    public class RuleListItem : ListViewItem
    {
        public RuleListItem(Rule rule, string text) : base(text)
        {
            Rule = rule;
        }

        public RuleListItem(Rule rule, string[] values) : base(values)
        {
            Rule = rule;
        }

        public Rule Rule { get; private set; }

        public override string ToString()
        {
            return Text;
        }
    }
}