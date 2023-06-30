using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBook.Models.Models
{
	public class Order
	{
		public int Id { get; set; }
		public string ApplicationUserId { get; set; }

		[ForeignKey(nameof(ApplicationUserId))]
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime ShippingDate { get; set; }

		public decimal OrderTotal { get; set; }

        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
		public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime PaymentDueDate { get; set; }
		public string? SessionId { get; set; }
        public string? PaymentIntenId { get; set; }
        public string PhoneNumber { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
		public string State { get; set; }
        public string PostCode { get; set; }
        public string Name { get; set; }
    }
}
