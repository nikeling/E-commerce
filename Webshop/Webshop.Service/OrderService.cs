using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webshop.Model;
using Webshop.Repository;
using Webshop.Repository.Common;
using Webshop.Service.Common;

namespace Webshop.Service
{
	public class OrderService : IOrderService
	{
		IOrderRepository OrderRepository;

		public OrderService(IOrderRepository orderRepository)
		{
			this.OrderRepository = orderRepository;
		}

		public void CreateNewOrder(List<ProductOrder> productOrders, PaymentModel payment)
		{
			OrderRepository.CreateNewOrder(productOrders, payment);
		}

		public async Task RemoveOrder(Order order)
		{
			await OrderRepository.RemoveOrder(order);
		}

	}
}
