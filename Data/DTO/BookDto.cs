using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DTO
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public Guid AuthorId { get; set; }
    }
}
