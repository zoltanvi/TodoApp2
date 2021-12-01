namespace TodoApp2.Core
{
    public class CommandObject
    {
        public static CommandObject NotHandled { get; } = new CommandObject(false);
        
        /// <summary>
        /// Is the command handled?
        /// </summary>
        public bool Handled { get; set; }

        public object CommandArgument { get; set; }

        public object CommandResult { get; set; }
        
        public string UndoMessage { get; set; }

        public CommandObject(bool handled, object commandResult = null, object commandArgument = null,
            string undoMessage = null)
        {
            Handled = handled;
            CommandArgument = commandArgument;
            CommandResult = commandResult;
            UndoMessage = undoMessage;
        }

    }
}
