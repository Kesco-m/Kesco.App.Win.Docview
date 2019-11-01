using System.Collections;

namespace Kesco.App.Win.DocView.CommandManagement
{
    //
    // Список комманд
    //
    public class CommandInstanceList : CollectionBase
    {
        private readonly Command _command;

        //
        // Список комманд
        //
        internal CommandInstanceList(Command acmd)
        {
            _command = acmd;
        }

        public void Add(object instance)
        {
            List.Add(instance);
        }

        public void Add(object[] items)
        {
            foreach (var item in items)
                Add(item);
        }

        public void Remove(object instance)
        {
            List.Remove(instance);
        }

        public object this[int index]
        {
            get { return List[index]; }
        }

        protected override void OnInsertComplete(int index, object value)
        {
            _command.Manager.GetCommandExecutor(value).InstanceAdded(value, _command);
        }
    }
}
