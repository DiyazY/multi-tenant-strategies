using System.Collections.Generic;

namespace Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Url { get; set; }

        public virtual List<Post> Posts { get; set; }
    }
}
