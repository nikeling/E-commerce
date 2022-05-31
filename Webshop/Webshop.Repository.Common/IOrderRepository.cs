using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webshop.Model;

namespace Webshop.Repository.Common
{
	public interface IOrderRepository
	{
		Task CreateNewOrder(List<ProductOrder> productOrders, PaymentModel payment);
		Task RemoveOrder(Order order);
	}
}
