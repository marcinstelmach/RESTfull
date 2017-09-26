using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.DTO
{
    public abstract class BooksForManipulationsDto
    {
        [Required(ErrorMessage = "You should fill out title")]
        [MaxLength(100, ErrorMessage = "The title shouldn't have more than 100 characters")]
        public string Title { get; set; }

        [MaxLength(500, ErrorMessage = "The description shouldn't have more than 500 characters")]
        public virtual string Description { get; set; }


    }
}
