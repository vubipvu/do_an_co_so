using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace BaiTh.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName {  get; set; }

    }
}
