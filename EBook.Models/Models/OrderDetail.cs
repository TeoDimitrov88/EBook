using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBook.Models.Models
{
	public class OrderDetail
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		
		[ForeignKey(nameof(OrderId))]
		[ValidateNever]
		public Order Order { get; set; }
		public int ProductId { get; set; }

		[ForeignKey(nameof(ProductId))]
		[ValidateNever]
        public Product Product { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
