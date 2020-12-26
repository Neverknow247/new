using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CSBelt.Models
{
    public class Users
    {
        [Key]
        public int UserId {get;set;}

        [Required]
        [MinLength(2)]
        [Display(Name = "Name:")]
        public string Name {get;set;}

        [Required]
        [Display(Name = "Email:")]
        [EmailAddress]
        public string Email {get;set;}

        [Required]
        [Display(Name = "Password:")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage="Password must contain at least one lowercase letter, one uppercase letter, one number, one special character and be at least 8 characters long!")]
        // [MinLength(8, ErrorMessage="Password must be at least 8 characters long!")]
        public string Password {get;set;}
        public List<Events> CreatedEvents {get;set;}
        public List<Participants> EventsAttending {get;set;}
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt {get;set;}

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Required]
        [Display(Name = "Confirm Password")]
        public string Confirm {get;set;} 
    }
}