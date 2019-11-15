using ApiFP.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Security.Claims;
using ApiFP.Models;
using System.Net;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace ApiFP.Controllers
{
    [RoutePrefix("accounts")]
    public class AccountsController : BaseApiController
    {
        /*
        [Authorize]
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }
        */

        [Authorize]
        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        [Authorize]
        [HttpGet]
        [Route("user")]
        public async Task<IHttpActionResult> GetUserByName()
        {
            var user = await this.AppUserManager.FindByNameAsync(User.Identity.GetUserName());

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        [AllowAnonymous]
        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName,
                Cuit = createUserModel.Cuit,
                BusinessName = createUserModel.BusinessName,
                Profile = createUserModel.Profile,
                Category = createUserModel.Category,
                RubroOperativoFK = createUserModel.RubroOperativo,
                RubroOperativoDescripcion = createUserModel.RubroOperativoDescripcion,
                RubroExpensasFK = createUserModel.RubroExpensas,
                RubroExpensasDescripcion = createUserModel.RubroExpensasDescripcion
            };

            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);

            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }

            string code = await this.AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code }));

            await this.AppUserManager.SendEmailAsync(user.Id, "Confirmacion de cuenta portal proveedores", "Para confirmar la suscripcion de su cuenta haga click <a href=\"" + callbackUrl + "\">aquí</a>");

            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, TheModelFactory.Create(user));
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<Object> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                var response = Request.CreateResponse(HttpStatusCode.Moved);
                response.Headers.Location = new Uri(ConfigurationManager.AppSettings["URL_CONFIRMA_ACCOUNT"]);
                return response;
                //return Ok();
            }
            else
            {
                return GetErrorResult(result);
            }
        }

        [Authorize]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        /*
        [Authorize]
        [Route("user/{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {

            //Only SuperAdmin or Admin can delete users (Later when implement roles)

            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser != null)
            {
                IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                return Ok();

            }

            return NotFound();

        }
        */

        [AllowAnonymous]
        [HttpGet]
        [Route("rubros")]
        public IHttpActionResult GetRubros()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return Ok(db.Rubros.ToList());
        }

        [Authorize]
        [HttpGet]
        [Route("profile")]
        public async Task<ApplicationUser> GetProfile()
        {
            string userId = User.Identity.GetUserId();
            var user = await this.AppUserManager.FindByIdAsync(userId);
            user.PasswordHash = "";
            user.SecurityStamp = "";
            return user;

        }

        [Authorize]
        [HttpPatch]
        [Route("profile")]
        public async Task<ApplicationUser> UpdateProfile(ApplicationUser userProfile)
        {
            string userId = User.Identity.GetUserId();

            if (userId == userProfile.Id)
            {
                var user = await this.AppUserManager.FindByIdAsync(userId);
                user.LastName = userProfile.LastName;
                user.FirstName = userProfile.LastName;
                user.Cuit = userProfile.Cuit;
                user.BusinessName = userProfile.BusinessName;
                this.AppUserManager.Update(user);
                user.PasswordHash = "";
                user.SecurityStamp = "";
                return user;
            }

            return null;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("password-recover")]
        public async Task<ApplicationUser> RecoverPassword([FromBody] string userEmail)
        {
            var user = this.AppUserManager.FindByEmail(userEmail);
            if (user != null)
            {
                var token = this.AppUserManager.GeneratePasswordResetToken(user.Id);

                Random rnd = new Random();
                string newPass = rnd.Next(10000000, 99999999).ToString();

                var result = this.AppUserManager.ResetPassword(user.Id, token, newPass);                
                if (result.Succeeded)
                {
                    await this.AppUserManager.SendEmailAsync(user.Id, "Recuperación de cuenta portal proveedores", "La nueva contraseña de su cuenta es:" + newPass);
                }

                //var callbackUrlBuilder = new UriBuilder(ConfigurationManager.AppSettings["URL_RECOVER_PASSWORD"]);
                //var query = HttpUtility.ParseQueryString(callbackUrlBuilder.Query);
                //query["userId"] = user.Id;
                //query["token"] = token;
                //callbackUrlBuilder.Query = query.ToString();
                //var callbackUrl = callbackUrlBuilder.ToString();                
            }

            return null;
        }

        [AllowAnonymous]
        [HttpPatch]
        [Route("password-recover")]
        public async Task<ApplicationUser> UpdatePassword([FromBody] JObject data)
        {
            var user = this.AppUserManager.FindByEmail(data["userEmail"].ToString());
            if (user != null)
            {
                var token = this.AppUserManager.GeneratePasswordResetToken(user.Id);
                var result = this.AppUserManager.ResetPassword(user.Id, token, data["newPassword"].ToString());                                
            }

            return null;
        }
    }
}