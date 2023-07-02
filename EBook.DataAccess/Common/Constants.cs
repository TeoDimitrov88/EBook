using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBook.DataAccess.Common
{
    public static class Constants
    {
        public const string AdminRole = "Admin";
        public const string EmployeeRole = "Employee";
        public const string UserRole = "User";
        public const string CompanyRole = "Company";


        public const string PendingStatus = "Pending";
		public const string ApprovedStatus = "Approved";
		public const string InProcessStatus = "Processing";
		public const string ShippedStatus = "Shipped";
		public const string CancelledStatus = "Cancelled";
		public const string RefundedStatus = "Refunded";


		public const string PaymentPendingStatus = "Pending";
		public const string PaymentApprovedStatus = "Approved";
		public const string PaymentDelayedStatus = "ApprovedForDelayedPayment";
		public const string PaymentRejectedStatus = "Rejected";
	}
}
