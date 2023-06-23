﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBook.Models.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public string ISBN { get; set; } = null!;
        [Required]
        public string Author { get; set; }
        [Required]
        [Range(1, 10000)]
        public decimal ListPrice { get; set; }
        [Required]
        [Range(1, 10000)]
        public decimal Price { get; set; }
        [Required]
        [Range(1, 10000)]
        public decimal priceFor25 { get; set; }
        [Required]
        [Range(1, 10000)]
        public decimal priceFor50 { get; set; }
        [Required]
        [ValidateNever]
        public string ImageURL { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        [Required]
        public int CoverTypeId { get; set; }
        [ForeignKey("CoverTypeId")]
        [ValidateNever]
        public CoverType CoverType { get; set; }
    }
}
