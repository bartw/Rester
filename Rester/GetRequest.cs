using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BeeWee.Rester
{
    public class GetRequest : Request
    {
        public GetRequest(string uri) : base(HttpMethod.Get, uri)
        {

        }
    }
}
