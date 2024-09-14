using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Core
{
    public class userProfile
    {
        [Required]
        public int Id { get; set; }
        public string userId { get; set; }

        public string UserName { get; set; }
        [NotAllowedWords(new string[] { "badword", "spam" })]
        public string Fname { get; set; }    
        [NotAllowedWords(new string[] { "badword", "spam" })]
        public string Lname { get; set; }
        public string OrganizationName { get; set; }
            
        public string country { get; set; }

        [StringLength(11,ErrorMessage =" 11 digit")]
        public string PhoneNumber { get; set; }
        public string DOB { get; set; }
        public string lives { get; set; }

        [Required(ErrorMessage ="Department must be required")]
        public string department { get; set; }
        public string about {  get; set; }
        public string profilePath { get; set; }

    }
}
