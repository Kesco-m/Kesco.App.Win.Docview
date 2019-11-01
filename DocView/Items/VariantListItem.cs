namespace Kesco.App.Win.DocView.Items
{
    public enum VariantType
    {
        Image,
        MainImage,
        ImageOriginal,
        MainImageOriginal,
        Data
    }

    public class VariantListItem : ListItem
    {
        public VariantListItem(int id, VariantType type, string text) : base(id, text)
        {
            Type = type;
        }

        public VariantType Type { get; set; }
    }
}