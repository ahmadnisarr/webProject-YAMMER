using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain_Core
{
    public class Posting
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string userId { get; set; }
        public string publication    { get; set; }
        public DateTime postDate    { get; set; }

    }
}
