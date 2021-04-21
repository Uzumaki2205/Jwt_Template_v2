using Jwt_Template.Repositories;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;

namespace Jwt_Template.Controllers.API
{
    public class AccountAPIController : ApiController
    {
        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult Login(Account user)
        {
            if (new AccountRepo().checkUser(user.UserName, user.Password) != null)
                return Ok();
            return NotFound();
        }
    }

    public class Account
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
