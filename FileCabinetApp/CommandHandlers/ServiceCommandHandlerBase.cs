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
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService instance.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        /// <summary>
        /// Gets IFileCabinetService instance.
        /// </summary>
        /// <value>IFileCabinetService.</value>
        protected IFileCabinetService Service
        {
            get { return this.fileCabinetService; }
        }
    }
}
