using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handlers common Seperclass.
    /// </summary>
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// Common File Service for command handlers.
        /// </summary>
        protected IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="service">FileCabinetService instance.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService service)
        {
            this.fileCabinetService = service;
        }
    }
}
