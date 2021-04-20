using Jwt_Template.Filters;
using Jwt_Template.Models;
using Jwt_Template.Repositories;
using System.Linq;
using System.Web.Http;

namespace Jwt_Template.Controllers.API
{
    public class AccountAPIController : ApiController
    {
        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult Login(tblUser user)
        {
            if (new AccountRepo().checkUser(user.Username, user.Password) != null)
                return Ok();
            return NotFound();
        }

        [JwtAuthentication]
        [HttpGet]
        public IHttpActionResult Dashboard()
        {
            var token = Request.Headers.GetValues("Authorization").First();
            string tokenUsername = JwtAuthenticationAttribute.ValidateUser(token.Replace("Bearer ", ""));

            if (tokenUsername != null)
                return Ok(tokenUsername);
            return null;
        }
    }
}
