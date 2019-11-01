using System.Data;

namespace Kesco.App.Win.DocView.Common
{
    /// <summary>
    /// Персона
    /// </summary>
    public class Person
    {
        public Person(DataRow dr)
        {
            ID = dr.IsNull(Environment.PersonData.IDField) ? 0 : (int)dr[Environment.PersonData.IDField];
            Name = ID == 0
                       ? Environment.StringResources.GetString("Blocks.Person.All")
                       : (dr.IsNull(Environment.PersonData.NameField)
                              ? Environment.StringResources.GetString("Blocks.Person.Message1")
                              : dr[Environment.PersonData.NameField].ToString());
        }

        public int ID { get; private set; }

        public string Name { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
