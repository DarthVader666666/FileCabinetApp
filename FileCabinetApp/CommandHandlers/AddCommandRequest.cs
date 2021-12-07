using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides command line command and parameters.
    /// </summary>
    public class AddCommandRequest
    {
        /// <summary>
        /// Gets or sets Command to execute.
        /// </summary>
        /// <value>string.</value>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets parameters of the command.
        /// </summary>
        /// <value>string.</value>
        public string Parameters { get; set; }
    }
}
