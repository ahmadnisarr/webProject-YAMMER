using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Core
{
    public class comment
    {
        public int Id { get; set; }
        public int postId { get; set; }
        public string userId { get; set; }
        public string commentContent { get; set; }
        public DateTime commentDate { get; set; }

    }
}
