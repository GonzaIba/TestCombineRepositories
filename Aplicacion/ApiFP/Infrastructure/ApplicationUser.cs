using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ApiFP.Infrastructure
{
    public class ApplicationUser : IdentityUser
    {
        
        public string FirstName { get; set; }

        public string LastName { get; set; }
                
        public string Cuit { get; set; }
        
        public string BusinessName { get; set; }
        
        public string Profile { get; set; }

        public Rubro RubroOperativo { get; set; }

        [ForeignKey("RubroOperativo")]
        public int RubroOperativoFK { get; set; }

        [MaxLength(100), Column(TypeName = "varchar")]
        public string RubroOperativoDescripcion { get; set; }

        public Rubro RubroExpensas { get; set; }

        [ForeignKey("RubroExpensas")]
        public int RubroExpensasFK { get; set; }
        
        [MaxLength(100), Column(TypeName = "varchar")]
        public string RubroExpensasDescripcion { get; set; }


        /*
        [Required]
        public DateTime JoinDate { get; set; }
        */
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}