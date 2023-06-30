using EBook.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBook.Models.ViewModels
{
	public class ShoppingCartVM
	{
        public IEnumerable<ShoppingCart> ListCart { get; set; }
        public Order Order { get; set; }
    }
}
