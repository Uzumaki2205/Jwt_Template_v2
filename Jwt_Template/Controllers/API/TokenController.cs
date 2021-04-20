using Jwt_Template.Filters;
using System.Net.Http;
using System.Web.Http;

namespace Jwt_Template.Controllers.API
{
    public class ValidToken
    {
        public string Token { get; set; }
        public string Username { get; set; }
    }
    public class TokenController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage GenerateToken(ValidToken valid)
        {
            if (valid.Token != null || valid.Username == null)
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest);

            string token = JwtManager.GenerateToken(valid.Username);
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, token);
        }

        [HttpPost]
        public IHttpActionResult ValidateToken(ValidToken ValidToken)
        {
            if (ValidToken.Token == null || ValidToken.Username == null)
                return BadRequest();
            string username = null;
            JwtAuthenticationAttribute.ValidateToken(ValidToken.Token, out username);

            if (ValidToken.Username.Equals(username))
                return Ok();
            return Unauthorized();
        }
    }
}
