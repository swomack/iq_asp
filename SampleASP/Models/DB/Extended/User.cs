using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SampleASP.Models.DB
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        public string ConfirmPassword { get; set; }
    }


    public class UserMetadata
    {
        [Display(Name = "Username")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User name is required!")]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required!")]
        [MinLength(6, ErrorMessage = "Minimum 6 character required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password does not match")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email id is not valid!")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }

}