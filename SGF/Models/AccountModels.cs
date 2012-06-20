using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using SGF.Models.EF;
using SGF.App_GlobalResources;

namespace SGF.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "DisplayOldPassword", ResourceType = typeof(SGFResource))]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "DisplayNewPassword", ResourceType = typeof(SGFResource))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "DisplayConfirmPassword", ResourceType = typeof(SGFResource))]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {   
        [Required]
        [Display(Name = "DisplayUsername", ResourceType = typeof(SGFResource))]
        public string UserName { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "DisplayPassword", ResourceType = typeof(SGFResource))]
        public string Password { get; set; }

        [Display(Name = "DisplayRememberMe", ResourceType = typeof(SGFResource))]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        //DisplayName
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        //Email => Username
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        //Password
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        //Nationality
        public int NationalityId { get; set; }
        [Display(Name = "Nationality")]
        public IEnumerable<SGF_R_COUNTRY> Countries { get; set; }

        //LanguagePref
        public int LanguageId { get; set; }
        [Display(Name = "Language")]
        public IEnumerable<SGF_R_LANGUAGE> Languages { get; set; }
    }

    public class AccountModel
    {
        //UserId da BD
        public int UserId { get; set; }

        //DisplayName
        public string Name { get; set; }

        //Email => Username
        public string Email { get; set; }

        public AccountModel(int _userId, string _name, string _email)
        {
            UserId = _userId;
            Name = _name;
            Email = _email;
        }
        

        /*
        //Nationality
        public int NationalityId { get; set; }
        [Display(Name = "Nationality of origin")]
        public IEnumerable<SGF_R_COUNTRY> Countries { get; set; }

        //LanguagePref
        public int LanguageId { get; set; }
        [Display(Name = "Display language")]
        public IEnumerable<SGF_R_LANGUAGE> Languages { get; set; }
         * */
    }
}
