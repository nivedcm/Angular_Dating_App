using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class CreateMessageDto
    {
        public string RecipiantUserName { get; set; }
        public string Content { get; set; }
    }
}
