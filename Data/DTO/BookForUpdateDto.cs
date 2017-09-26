using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.DTO
{
    public class BookForUpdateDto : BooksForManipulationsDto
    {
        [Required(ErrorMessage = "You should fill out the description")]
        public override string Description { get; set; }
    }
}
