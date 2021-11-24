using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public interface ICommandHandler
    {
        void SetNext(ICommandHandler handler);

        void Handle(AppCommandRequest request);
    }
}
