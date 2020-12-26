using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CSBelt.Models
{
    public class Participants
    {
        [Key]
        public int ParticipantsId {get;set;}
        public int UserId {get;set;}
        public int EventId {get;set;}
        public Users User {get;set;}
        public Events Event {get;set;}
    }
}