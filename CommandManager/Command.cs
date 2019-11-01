namespace Kesco.App.Win.DocView.CommandManagement
{
    public class Command
    {
        // Members
        private readonly CommandInstanceList _commandInstances;
        protected bool enabled;
        protected bool check;
        protected int imageIndex;

        // Constructor
        public Command(string strTag, ExecuteHandler handlerExecute, UpdateHandler handlerUpdate)
        {
            _commandInstances = new CommandInstanceList(this);
            Tag = strTag;
            OnUpdate += handlerUpdate;
            OnExecute += handlerExecute;
        }

        // CommandInstances collection
        public CommandInstanceList CommandInstances
        {
            get { return _commandInstances; }
        }

        // Tag property: Unique internal name for each command
        public string Tag { get; private set; }

        // Manager property: maintain association with parent command manager
        internal CommandManager Manager { get; set; }

        // Enabled property
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                foreach (object instance in _commandInstances)
                    Manager.GetCommandExecutor(instance).Enable(instance, enabled);
            }
        }

        // Checked property
        public bool Checked
        {
            get { return check; }
            set
            {
                check = value;
                foreach (object instance in _commandInstances)
                    Manager.GetCommandExecutor(instance).Check(instance, check);
            }
        }

        public int ImageIndex
        {
            get { return imageIndex; }
            set
            {
                imageIndex = value;
                foreach (object instance in _commandInstances)
                    Manager.GetCommandExecutor(instance).ImageIndex(instance, imageIndex);
            }
        }

        public override string ToString()
        {
            return Tag;
        }

        // Methods to trigger events
        public void Execute()
        {
            if (OnExecute != null)
                OnExecute(this);
        }

        // Methods to trigger events if command enabled
        public void ExecuteIfEnabled()
        {
            if (OnExecute != null && enabled)
                OnExecute(this);
        }

        internal void ProcessUpdates()
        {
            if (OnUpdate != null)
                OnUpdate(this);
        }

        // Events
        public delegate void UpdateHandler(Command cmd);

        public event UpdateHandler OnUpdate;

        public delegate void ExecuteHandler(Command cmd);

        public event ExecuteHandler OnExecute;
    }
}