using System.Collections;

namespace Kesco.App.Win.DocView.CommandManagement
{
	// Command Executor base class
	public abstract class CommandExecutor
	{
		protected Hashtable hashInstances = new Hashtable();

		public virtual void InstanceAdded(object item, Command cmd)
		{
			hashInstances.Add(item, cmd);
		}

		protected Command GetCommandForInstance(object item)
		{
			return hashInstances[item] as Command;
		}

		// Interface for derived classed to implement
		public abstract void Enable(object item, bool bEnable);
		public abstract void Check(object item, bool bCheck);
		public abstract void ImageIndex(object item, int index);
	}
}