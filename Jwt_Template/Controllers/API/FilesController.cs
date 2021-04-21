using Jwt_Template.Filters;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Jwt_Template.Controllers.API
{
    public class FilesController : ApiController
    {
        [JwtAuthentication]
        [HttpGet] 
        public HttpResponseMessage FileUpload()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }    
    }
}
