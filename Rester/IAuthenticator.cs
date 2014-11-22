using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeWee.Rester
{
    public interface IAuthenticator
    {
        Dictionary<string, string> GetHeaders(Request request);
    }
}
