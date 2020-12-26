using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CSBelt.Models
{
    public class Events
    {
        [Key]
        public int EventId {get;set;}

        [Required]
        [Display(Name = "Title:")]
        public string Title {get;set;}

        [Required]
        [Display(Name = "Date and Time:")]
        public DateTime EventStart {get;set;}
        public DateTime EventEnd {get;set;}

        [Required]
        [Display(Name = "Duration:")]
        public string EventLength {get;set;}

        [Required]
        [Display(Name = "Description:")]
        public string Desc {get;set;}
        public int UserId {get;set;}
        public Users Planner {get;set;}
        public List<Participants> EventParticipants {get;set;}
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt {get;set;}
    }
}