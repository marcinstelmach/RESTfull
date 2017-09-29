using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Model
{
    public abstract class LinkedResourceBaseDto
    {
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}
