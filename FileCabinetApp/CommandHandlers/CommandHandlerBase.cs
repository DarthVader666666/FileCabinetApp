using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// CommandHandler Base Class.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        /// <summary>
        /// Gets nextHandler.
        /// </summary>
        /// <value>ICommandHandler.</value>
        public ICommandHandler NextHandler
        {
            get { return this.nextHandler; }
        }

        /// <summary>
        /// Sets next handler reference.
        /// </summary>
        /// <param name="handler">Next handler.</param>
        public void SetNext(ICommandHandler handler)
        {
            this.nextHandler = handler;
        }

        /// <summary>
        /// Calls method or next handler.
        /// </summary>
        /// <param name="request">Provides command and parameters.</param>
        public abstract void Handle(AddCommandRequest request);
    }
}
