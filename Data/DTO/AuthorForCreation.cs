using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.DTO
{
    public class AuthorForCreation
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(50)]
        public string DateOfBirth { get; set; }
        [Required]
        [MaxLength(50)]
        public string Genre { get; set; }

        public IList<BookForCreation> Books { get; set; } = new List<BookForCreation>();
    }
}
