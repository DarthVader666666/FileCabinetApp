using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Defines command handler methods.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Sets next command handler.
        /// </summary>
        /// <param name="handler">next handler.</param>
        void SetNext(ICommandHandler handler);

        /// <summary>
        /// Calls method or next handler.
        /// </summary>
        /// <param name="request">Provides command and parameters.</param>
        void Handle(AddCommandRequest request);
    }
}
