using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace WholesaleEnterprise.Controllers.API
{
    public class SystemController : ApiController
    {
        // returns just an object, for intergration testing
        public HttpResponseMessage GetStatus()
        {
            return this.Request.CreateResponse(HttpStatusCode.OK,"");
        }

    }
}
