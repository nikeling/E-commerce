using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Webshop.Model;
using Webshop.Service;
using Webshop.Service.Common;

namespace Webshop.Controllers
{
	public class OrderController : ApiController
	{
		IOrderService OrderService;

		public OrderController(IOrderService orderService)
		{
			this.OrderService = orderService;
		}
		
		// POST api/<controller>
		[HttpPost]
		[Route("api/orders/{paymentId}")]
		public HttpResponseMessage CreateNewOrder([FromBody]List<ProductOrder> productOrders, [FromUri]PaymentModel payment)
		{
			OrderService.CreateNewOrder(productOrders, payment);
			return Request.CreateResponse(HttpStatusCode.OK, "Order successfully added.");
		}

		// DELETE api/<controller>/5
		[HttpDelete]
		[Route("api/orders")]
		public async Task<HttpResponseMessage> RemoveOrder([FromBody]Order order)
		{
			await OrderService.RemoveOrder(order);
			return Request.CreateResponse(HttpStatusCode.OK, "Order successfully removed.");
		}
	}
}