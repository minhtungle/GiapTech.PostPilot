using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Public.Interfaces
{
    public interface IPermissionCheckerAppService
    {
        void CheckAccess(string controllerName, string actionName = "Index");
    }
}
