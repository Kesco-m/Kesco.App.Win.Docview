using System.Collections;

namespace Kesco.App.Win.DocView.CommandManagement
{
    /// <summary>
    ///   CommandsList Collection Implementation
    /// </summary>
    public class CommandsList : SortedList
    {
		internal CommandManager Manager { get; set; }

		internal CommandsList(CommandManager amgr)
        {
            Manager = amgr;
        }

        // Commands collection interface
        public Command this[string cmdTag]
        {
            get { return base[cmdTag] as Command; }
        }

        public void Add(Command command)
        {
            command.Manager = Manager;
            base.Add(command.ToString(), command);
        }
    }
}