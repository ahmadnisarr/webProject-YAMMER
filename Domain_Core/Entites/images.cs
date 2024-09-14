using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Core
{
    public class images
    {
        [Required]
        public int Id { get; set; }
        public int postId { get; set; }
        public string userId { get; set; }
        public string imagePath { get; set; }

    }
}
