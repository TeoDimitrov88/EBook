﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EBook.Models.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [DisplayName("Display order")]
        [Range(1, 500, ErrorMessage = "Display order must be between 1 and 100")]
        public int DisplayOrder { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

    }
}
