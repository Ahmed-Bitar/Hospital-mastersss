using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalPark.Models
{
    public class ApplicationRole : IdentityRole<int>
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public string Id { get; set; } = Guid.NewGuid().ToString();
        //public string? RoleDescription { get; set; }
        public ApplicationRole()
        {
        }


        public ApplicationRole(string roleName) : base(roleName)
        {

        }
    }
}

