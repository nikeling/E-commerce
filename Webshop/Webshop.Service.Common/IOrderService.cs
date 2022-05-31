using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webshop.Model;

namespace Webshop.Service.Common
{
	public interface IOrderService
	{
		void CreateNewOrder(List<ProductOrder> productOrders, PaymentModel payment);
		Task RemoveOrder(Order order);
	}
}
