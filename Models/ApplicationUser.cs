using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalPark.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        internal object AspNetUsers;

        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;

        public string Gender { set; get; }

        public string UserType { get; set; }
        public string PhoneNumber { get; set; }

    }
}
