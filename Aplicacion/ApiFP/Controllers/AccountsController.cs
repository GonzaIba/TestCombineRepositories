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
using ApiFP.Helpers;

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
        public async Task<CreateUserBindingModel> GetProfile()
        {
            string userId = User.Identity.GetUserId();
            var user = await this.AppUserManager.FindByIdAsync(userId);

            var userDto = new CreateUserBindingModel()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Cuit = user.Cuit,
                BusinessName = user.BusinessName,
                Profile = user.Profile,                
                RubroOperativo = user.RubroOperativoFK,
                RubroOperativoDescripcion = user.RubroOperativoDescripcion,
                RubroExpensas = user.RubroExpensasFK,
                RubroExpensasDescripcion = user.RubroExpensasDescripcion
            };

            return userDto;
        }

        [Authorize]
        [HttpPatch]
        [Route("profile")]
        public async Task<CreateUserBindingModel> UpdateProfile(CreateUserBindingModel userProfile)
        {
            string userId = User.Identity.GetUserId();
            var user = await this.AppUserManager.FindByIdAsync(userId);

            user.FirstName = userProfile.FirstName;
            user.LastName = userProfile.LastName;
            user.Cuit = userProfile.Cuit;
            user.BusinessName = userProfile.BusinessName;
            user.Profile = userProfile.Profile;            
            user.RubroOperativoFK = userProfile.RubroOperativo;
            user.RubroOperativoDescripcion = userProfile.RubroOperativoDescripcion;
            user.RubroExpensasFK = userProfile.RubroExpensas;
            user.RubroExpensasDescripcion = userProfile.RubroExpensasDescripcion;

            this.AppUserManager.Update(user);

            return null;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("password-recover")]
        public async Task<ApplicationUser> RecoverPassword(JObject account)
        {
            // obtener token de cambio
            // crear html para email
            // crear url con token y usuario encriptado
            // enviar email

            var user = this.AppUserManager.FindByEmail(account["email"].ToString());
            if (user != null)
            {
                var token = this.AppUserManager.GeneratePasswordResetToken(user.Id);

                var callbackUrl = new Uri(Url.Link("RecoverAccountRoute", new { userId = EncryptHelper.Encript(user.Id), token = token }));

                var emailBody = "Para la recuperación de su cuenta de portal proveedores haga click <a href=\"" + callbackUrl + "\">aquí</a>.</br></br> Si usted no ha solicitad recuperar su cuenta por favor desestime este correo.";

                await this.AppUserManager.SendEmailAsync(user.Id, "Recuperación de cuenta portal proveedores", emailBody);
              
            }
            return null;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("password-recover", Name = "RecoverAccountRoute")]
        public async Task<Object> UpdatePassword(string userId = "", string token = "")
        {

            // validar userId
            // crear password nuevo
            // crear body html
            // enviar email con password
            // redireccionar a pagina de notificación de envio de mail con password

            var response = Request.CreateResponse(HttpStatusCode.Moved);

            if (!String.IsNullOrEmpty(userId))
            {
                var user = this.AppUserManager.FindById(EncryptHelper.Decript(userId));

                if (user != null)
                {
                    Random rnd = new Random();
                    string newPass = rnd.Next(10000000, 99999999).ToString();

                    var result = this.AppUserManager.ResetPassword(user.Id, token, newPass);


                    if (result.Succeeded)
                    {
                        var emailBody = "La nueva constraseña asignada para su cuenta de portal proveedores es: " + newPass + "</br></br>";

                        await this.AppUserManager.SendEmailAsync(user.Id, "Recuperación exitosa de cuenta portal proveedores", emailBody);
                        
                        response.Headers.Location = new Uri(ConfigurationManager.AppSettings["URL_RECOVERED_ACCOUNT"]);
                        return response;
                    }
                }
            }
            
            response.Headers.Location = new Uri(ConfigurationManager.AppSettings["URL_ERROR"]);
            return response;
        }
    }
}