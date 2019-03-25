using System.ComponentModel.DataAnnotations;

namespace XWorkUp.AspNetCoreMvc.ViewModels
{
    public class AddRoleViewModel
    {
        [Required]
        [Display(Name = "Role name")]
        public string RoleName { get; set; }
    }
}