using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace RetailEnterprise.Controllers.API
{
    public class SystemController : ApiController
    {
        /// <summary>
        /// Used by the IS to check the system up or down status
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetStatus()
        {
            return this.Request.CreateResponse(HttpStatusCode.OK,"");
        }

        /// <summary>
        /// Used by IS to restart the system
        /// </summary>
        [HttpGet]
        public void Restart()
        {
            System.Web.HttpRuntime.UnloadAppDomain();
        }

    }
}
