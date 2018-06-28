using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Post
    {
        public Post()
        {
            createdAt = DateTime.Now;
            id = new Guid();
        }

        public string message { get; set; }

        public string userName { get; set; }

        public DateTime createdAt { get; }

        public Guid id { get; }
    }
}
