using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace Foodie.Models
{
    public class FoodieDatabase : DbContext
    {
        public DbSet<User> User { get; set; }
    }

    [Table("Users", Schema = "Public")]
    public class User
    {
        [Key]
        [Column("pId")]
        public Guid pId { get; set; }

        [DataType(DataType.Text)]
        [Column("Username")]
        public string Username { get; set; }

        [DataType(DataType.EmailAddress)]
        [Column("Email")]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        [Column("PasswordQuestion")]
        public string PasswordQuestion { get; set; }

        [DataType(DataType.Text)]
        [Column("PasswordAnswer")]
        public string PasswordAnswer { get; set; }

        [DataType(DataType.Password)]
        [Column("Password")]
        public string Password { get; set; }

        [DataType(DataType.DateTime)]
        [Column("LastLoginDate")]
        public DateTime LastLoginDate { get; set; }

        [DataType(DataType.DateTime)]
        [Column("LastPasswordChangedDate")]
        public DateTime LastPasswordChangedDate { get; set; }

        [DataType(DataType.DateTime)]
        [Column("CreationDate")]
        public DateTime CreationDate { get; set; }

        [Column("IsLockedOut")]
        public Boolean IsLockedOut { get; set; }

        [DataType(DataType.DateTime)]
        [Column("LastLockedOutDate")]
        public DateTime LastLockedOutDate { get; set; }

        [Column("FailedPasswordAttemptCount")]
        public UInt16 FailedPasswordAttemptCount { get; set; }

        [DataType(DataType.DateTime)]
        [Column("FailedPasswordAttemptWindowStart")]
        public DateTime FailedPasswordAttemptWindowStart { get; set; }

        [DataType(DataType.DateTime)]
        [Column("FailedPasswordAnswerAttemptWindowStart")]
        public DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }

        [Column("FailedPasswordAnswerAttemptCount")]
        public UInt16 FailedPasswordAnswerAttemptCount { get; set; }

        [Column("IsApproved")]
        public Boolean IsApproved { get; set; }

        [Column("ProfileType")]
        public ProfileType ProfileType { get; set; }

        [DataType(DataType.DateTime)]
        [Column("LastActivityDate")]
        public DateTime LastActivityDate { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangeEmailModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "New email address")]
        public string NewEmail { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Confirm new email address")]
        [Compare("NewEmail", ErrorMessage = "The new email address and confirmation email do not match.")]
        public string ConfirmEmail { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    [Table("Users", Schema = "Public")]
    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string SecurityQuestion { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string SecurityAnswer { get; set; }

        [Display(Name = "Select the profile type you want.")]
        public ProfileType ProfileType { get; set; }

        //This is the pId
        public Guid pId { get; set; }
    }

    public enum ProfileType{
        Visitor, //this is the default
        FoodCritic,
        Chef,
        FoodEnthusiast,
        Restaurant
    }
}
