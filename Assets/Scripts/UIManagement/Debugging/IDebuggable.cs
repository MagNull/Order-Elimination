using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIManagement.Debugging
{
    public interface IDebuggable
    {
        void StartDebugging();

        void StopDebugging();
    }
}
